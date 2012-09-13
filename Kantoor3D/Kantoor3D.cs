using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using SeriousGameLib;
using Kantoor3D.GameObjects;

namespace Kantoor3D
{
    public enum GameStates { MINIGAME, KANTOOR, AFLEVERING, PAUSED };

    public class Kantoor3D : GameWorld3D
    {
        private const float playerWidth = 0.1f;
        private const float playerHeight = 4.0f;

        private SpriteBatch _spiteBatch;
        private float movementSpeed = 7.0f;

        public GameWorld CurrentMiniGame;
        public GameStates CurrentGameState;

        public Hud Hud { get; set; }
        public Narrator Narrator { get; set; }

        private MouseState _previousMouseState;

        public Aflevering.Aflevering Aflevering { get; set; }

        public static Kantoor3D Instance; // Next time we'll plan our project before coding, ok?

        public Kantoor3D(Game game)
            : base(game)
        {
            LoadWorld(@"Content\Kantoor3D\Worlds\Kantoor3D.xml");
            WorldBounds = GetGameObject3D("kantoor").TransformedCombinedBoundingBox;

            PlayerObject = new Player();
            AddGameObject(PlayerObject);
                
            Narrator        = new Narrator(game.GraphicsDevice);
            _spiteBatch       = new SpriteBatch(game.GraphicsDevice);

            ResetMouse();
            _previousMouseState = Mouse.GetState();

            CurrentGameState   = GameStates.KANTOOR;

            this.Hud = new Hud(this);

            Camera.CameraArrived += new CameraArrivedEventHandler(Camera_CameraArrived);
            Camera.Position = new Vector3(1, 4, 10);

            Camera.LockY = true;
            Camera.LockRotationMinDegrees = -40;
            Camera.LockRotationMaxDegrees = 40;
            Camera.LockRotation = true;

            CollissionEvent += new CollissionEventHandler(Kantoor3D_CollissionEvent);
            MouseCollissionHoverEnter += new CollissionEventHandler(Kantoor3D_MouseCollissionHoverEnter);
            MouseCollissionHoverExit += new CollissionEventHandler(Kantoor3D_MouseCollissionHoverExit);
            MouseCollissionClick += new CollissionEventHandler(Kantoor3D_MouseCollissionClick);

            AudioFactory.AddSoundEffect("walk", "Kantoor3D/Audio/walk");
            AudioFactory.AddSoundEffect("door", "Kantoor3D/Audio/door");
            AudioFactory.AddSoundEffect("kantoortheme", "Kantoor3D/Audio/kantoor");

            Instance = this;

            //LoadMiniGame(GetGameObject3D("tafelfotograaf"));
            LoadMiniGame(GetGameObject3D("computerscherm"));
        }

        void Kantoor3D_MouseCollissionClick(GameObject3D gameObject)
        {
            if (CurrentGameState == GameStates.KANTOOR)
            {
                if (gameObject.MiniGameType != MiniGames.NONE)
                {
                    LoadMiniGame(gameObject);
                }
            }
        }

        void Kantoor3D_MouseCollissionHoverExit(GameObject3D gameObject)
        {
            if (CurrentGameState == GameStates.KANTOOR)
            {
                Narrator.Hide();
            }
        }

        void Kantoor3D_MouseCollissionHoverEnter(GameObject3D gameObject)
        {
            // We are in a mini-game, so don't even bother doing anything.
            if (CurrentMiniGame != null) return;
            
            if (gameObject.MiniGameType != MiniGames.NONE)
            {
                Narrator.ShowText(NarratorText.HoverTexts[(int)gameObject.MiniGameType]);
            }
        }

        void Kantoor3D_CollissionEvent(GameObject3D gameObject)
        {
            //collission event
        }

        void Camera_CameraArrived(GameObject3D gameObject, bool isMovingToOrigin)
        {
            if (!isMovingToOrigin)
            {
                CurrentMiniGame = _tempWorld;

                CurrentGameState = GameStates.MINIGAME;

                if (CurrentMiniGame is Schilder.Schilder)
                {
                    GetGameObject3D("schildersezel").Visible = false;
                    GetGameObject3D("boekenkast").Visible = false;
                }
                if (CurrentMiniGame is Uitnodiging.Uitnodiging)
                {
                    GetGameObject3D("puzzeldoos").Visible = false;
                    GetGameObject3D("laptop").Visible = false;
                }
                if (CurrentMiniGame is Ontwerpen.Ontwerpen)
                {
                    GetGameObject3D("globe").Visible = false;
                    GetGameObject3D("memoryTableObject").Visible = false;
                    
                }

                // A sepearte function call is required as some GameWorld instances call the Initialize themselfs.
                if (CurrentMiniGame != null)
                {
                    CurrentMiniGame.OnCameraArrive();
                }

                if (gameObject.MiniGameType == MiniGames.AFLEVERING)
                {
                    Narrator.Hide();
                    Narrator.RemoveAllButtons();
                    AudioFactory.PlayOnce("door");

                    if (CurrentMiniGame != null)
                    {
                        CurrentMiniGame.CleanUp();
                        CurrentMiniGame = null;
                    }

                    Aflevering = new Aflevering.Aflevering(Game);
                    Aflevering.Initialize();
                    DrawWorld = false;
                    CurrentGameState = GameStates.AFLEVERING;
                    Narrator.Instance.Hide();
                    Narrator.Instance.HideDolphin = true;
                }
            }
            else
            {
                PlayerObject.Position = Camera.Position;
               
                Game.IsMouseVisible = true;

                if (lastLoadAfter != MiniGames.NONE)
                {
                    //load minigame
                    switch (lastLoadAfter)
                    {
                        case MiniGames.AFLEVERING:
                            Camera.MoveToGameObject(GetGameObject3D("deur"), 1000.0f);
                            break;
                        case MiniGames.FOTOGRAAF:
                            LoadMiniGame(GetGameObject3D("tafelfotograaf"));
                            break;
                        case MiniGames.ONTWERPEN:
                            LoadMiniGame(GetGameObject3D("tafelontwerpen"));
                            break;
                        case MiniGames.SCHILDER:
                            LoadMiniGame(GetGameObject3D("schildersezel"));
                            break;
                        case MiniGames.UITNODIGING:
                            LoadMiniGame(GetGameObject3D("tafeluitnodiging"));
                            break;
                    }

                    lastLoadAfter = MiniGames.NONE;
                }
            }
        }

        private void ResetMouse()
        {
            Viewport viewport = Game.GraphicsDevice.Viewport;
            Mouse.SetPosition((viewport.X + viewport.Width) / 2, (viewport.Y + viewport.Height) / 2); 
        }

        public void handleKantoorInput(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();

            //if (keyboardState.IsKeyDown(Keys.Escape)) Game.Exit();

            if (Camera.PlayerControllable)
            {
                if (keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.Up))
                {
                    Vector3 newPosition = Camera.GetMoveToLookAtPosition(Vector3.Forward, movementSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
                    MovePlayer(newPosition);
                    Camera.SetPosition(PlayerObject.Position);
                    AudioFactory.PlayOnce("walk");
                }

                if (keyboardState.IsKeyDown(Keys.S) || keyboardState.IsKeyDown(Keys.Down))
                {
                    Vector3 newPosition = Camera.GetMoveToLookAtPosition(Vector3.Backward, movementSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
                    MovePlayer(newPosition);
                    Camera.SetPosition(PlayerObject.Position);
                    AudioFactory.PlayOnce("walk");
                }

                if (keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left))
                {
                    Vector3 newPosition = Camera.GetMoveToLookAtPosition(Vector3.Left, movementSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
                    MovePlayer(newPosition);
                    Camera.SetPosition(PlayerObject.Position);
                    AudioFactory.PlayOnce("walk");
                }

                if (keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right))
                {
                    Vector3 newPosition = Camera.GetMoveToLookAtPosition(Vector3.Right, movementSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
                    MovePlayer(newPosition);
                    Camera.SetPosition(PlayerObject.Position);
                    AudioFactory.PlayOnce("walk");
                }
            }

            if (mouseState.X != _previousMouseState.X || mouseState.Y != _previousMouseState.Y)
            {
                Camera.Rotate((mouseState.X - _previousMouseState.X), (mouseState.Y - _previousMouseState.Y));
                if (Game.IsActive) ResetMouse();
                mouseState = Mouse.GetState();
            }

            _previousMouseState = mouseState;
        }

        MiniGames lastLoadAfter = MiniGames.NONE;
        public override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) Game.Exit();

            Camera.Update(gameTime);
            KeyboardState keyboardState = Keyboard.GetState();
            if((keyboardState.IsKeyDown(Keys.F10)))
            {
                PersistentStorage.resetTrophies();
            }
            // Checks whether the "unload" flag has been set in the current mini-game.
            // We are using this flag b.c the items in the SeriousGameLib cannot reference
            // to kantoor3D due to a (then created) circular depenancy.
            if (CurrentMiniGame != null && CurrentMiniGame.ForceUnload)
            {
                if (CurrentMiniGame.LoadAfter != MiniGames.NONE)
                {
                    lastLoadAfter = CurrentMiniGame.LoadAfter;
                    // Load game? well, first unload, then load.
                    UnloadMiniGame();
                }
                else
                {
                    UnloadMiniGame();
                }
            }

            switch (CurrentGameState)
            {
                case GameStates.KANTOOR:
                    handleKantoorInput(gameTime);
                    break;
                case GameStates.MINIGAME:
                    if (CurrentMiniGame != null) CurrentMiniGame.Update(gameTime);
                    break;
                case GameStates.AFLEVERING:
                    if (Aflevering != null) Aflevering.Update(gameTime);
                    if (Aflevering != null && Aflevering.ForceUnload)
                    {
                        DrawWorld = true;
                        Game.Components.Remove(Aflevering);
                        Camera.ReturnFromLastGameObject();
                        Aflevering = null;
                        CurrentGameState = GameStates.KANTOOR;
                        Narrator.Instance.HideDolphin = false;
                        Narrator.Instance.Show();
                        AudioFactory.PlayOnce("kantoortheme", true);
                    }
                    break;
            }


            // The game HUD is always shown. This too will render the "exit mini game" button.
            Hud.Update(gameTime);
            Narrator.Update(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            // This should render the 3D world which is handled by the super class.
            base.Draw(gameTime);

            if (Aflevering != null)
            {
                Aflevering.Draw(gameTime);
            }

            _spiteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone);

            // Propegate the draw event to any mini-game available:
            if (CurrentMiniGame != null) 
                CurrentMiniGame.Draw(gameTime, _spiteBatch);

            if (Aflevering != null) Hud.Draw(_spiteBatch, gameTime, Aflevering);
            else Hud.Draw(_spiteBatch, gameTime, CurrentMiniGame);

            Narrator.Draw(_spiteBatch, gameTime);

            // Custom draw call for the "schilder" his paintbrush. This is done sepperately due to a Z-index issue.
            if (CurrentMiniGame is Schilder.Schilder) 
                (CurrentMiniGame as Schilder.Schilder).PaintBrush.Draw(gameTime, _spiteBatch);

            _spiteBatch.End();
        }

        private GameWorld _tempWorld = null;
        // Loads a minigame within the Kantoor3D environment.
        public void LoadMiniGame(GameObject3D gameObject)
        {
            // Update the gamestate and have the "input" to the minigame rather than office.
            if (gameObject != null && gameObject.MiniGameType != MiniGames.NONE && Camera.PlayerControllable)
            {
                Narrator.Hide();

                switch (gameObject.MiniGameType)
                {
                    case MiniGames.ONTWERPEN:
                        _tempWorld = new Ontwerpen.Ontwerpen(_owner);
                        break;
                    case MiniGames.FOTOGRAAF:
                        _tempWorld = new Fotograaf.Fotograaf(_owner);
                        break;
                    case MiniGames.SCHILDER:
                        // Hide the easel as this minigame will draw his own easel.
                        _tempWorld = new Schilder.Schilder(_owner);
                        break;
                    case MiniGames.UITNODIGING:
                        _tempWorld = new Uitnodiging.Uitnodiging(_owner);
                        break;
                    case MiniGames.MENU:
                        _tempWorld = new Menu.Menu(_owner);
                        break;
                    case MiniGames.NONE:
                    default:
                        // The selection object has no minigame mapped to it.
                        break;
                }

                Camera.MoveToGameObject(gameObject, 1000);
            }
        }

        // Call this to "close" the current mini-game and return the camera to the origin.
        public void UnloadMiniGame()
        {
            Narrator.PlacementOffset = Vector2.Zero;

            if (CurrentMiniGame == null) return;
            MediaPlayer.Stop();
            Camera.ReturnFromLastGameObject();

            Narrator.Hide();
            Narrator.RemoveAllButtons();

            CurrentMiniGame.CleanUp();
            CurrentMiniGame = null;

            ResetMouse();

            // Mini-game "de schilder" hides the actual easel, so we must show it again!
            GetGameObject3D("schildersezel").Visible = true;
            GetGameObject3D("globe").Visible = true;
            GetGameObject3D("puzzeldoos").Visible = true;
            GetGameObject3D("boekenkast").Visible = true;
            GetGameObject3D("laptop").Visible = true;
            GetGameObject3D("memoryTableObject").Visible = true;

            Narrator.Instance.HideDolphin = false;

            // Return the input back to the office:
            CurrentGameState = GameStates.KANTOOR;
            AudioFactory.PlayOnce("kantoortheme", true);
        }
    }
}
