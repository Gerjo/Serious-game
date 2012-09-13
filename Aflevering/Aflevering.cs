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
using Aflevering.GameObjects;

namespace Aflevering
{
    public class Aflevering : GameWorld3D
    {
        public RaceHud RaceHud { get; set; }
        private SpriteBatch infoBatch;
        public Busje Busje;
        public Skybox Skybox;
        MouseState previousMouseState;
        SpriteFont Speed;
        Terrain terrain;

        private DateTime starttime;
        private TimeSpan leveltime;


        public Aflevering(Game game)
            : base(game)
        {
            RaceHud      = new RaceHud(this);
            starttime = DateTime.Now;
            //DebugMode = true;

            LoadWorld(@"Content\Aflevering\Worlds\Aflevering.xml");

            Busje = GetGameObject3D("Busje") as Busje;

            PlayerObject    = GetGameObject3D("Busje");
            infoBatch       = new SpriteBatch(game.GraphicsDevice);

            Speed           = Game.Content.Load<SpriteFont>("Aflevering/Fonts/Speed");

            AudioFactory.AddSoundEffect("crash", "Aflevering/Audio/crash"); 
            AudioFactory.AddSoundEffect("rocket", "Aflevering/Audio/rocket");
            AudioFactory.AddSoundEffect("gasoline", "Aflevering/Audio/benzine");
            AudioFactory.AddSoundEffect("speed1", "Aflevering/Audio/speed1");
            AudioFactory.AddSoundEffect("speed2", "Aflevering/Audio/speed2");
            AudioFactory.AddSoundEffect("speed3", "Aflevering/Audio/speed3");
            AudioFactory.AddSoundEffect("afleveringtheme", "Aflevering/Audio/Race-music2");

            AudioFactory.PlayOnce("afleveringtheme", true);

            //rotate the camera slightly to view the top of the model
            Camera.Rotate(0.0f, 20.0f);

            Mouse.SetPosition(0,0);

            Skybox = new Skybox();

            AddGameObject(Skybox);

            CollissionEvent += new CollissionEventHandler(Aflevering_CollissionEvent);

            string[] textures = { @"Aflevering\Terrain\grass", @"Aflevering\Terrain\sand", @"Aflevering\Terrain\rock", @"Aflevering\Terrain\snow" };
            terrain = new Terrain(game, @"Aflevering\Terrain\bartmap", textures, Camera);
            terrain.Initialize();
            
            game.IsMouseVisible = true;
        }

        public override void Initialize()
        {
            base.Initialize();

            TrophyScreen = new TrophyScreen(this);
        }

        private Trophies GetTrophy(int timeSeconds)
        {
            if (timeSeconds < 28) return Trophies.Gold;
            else if (timeSeconds < 35) return Trophies.Silver;
            else return Trophies.Bronze;
        }

        void Aflevering_CollissionEvent(GameObject3D gameObject)
        {
            switch (gameObject.UID)
            {
                case "PowerupBenzine":
                    gameObject.Visible = false;
                    Busje.benzine += 100;
                    AudioFactory.PlayOnce("gasoline");   
                    break;
                case "PowerupRocket":
                    gameObject.Visible = false;
                    Busje.speedPower = true;
                    AudioFactory.PlayOnce("rocket");  
                    break;
                case "PowerupLightning":
                    gameObject.Visible = false;
                    Busje.lightning += 1;
                    break;
                case "obstacleRocks":
                    gameObject.Visible = false;
                    Busje.hitObstacle = true;
                    AudioFactory.PlayOnce("crash");   
                    break;
                case "obstacleSewer":
                    gameObject.Visible = false;
                    Busje.hitObstacle = true;
                    AudioFactory.PlayOnce("crash");   
                    break;
            }   
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            if (TrophyScreen != null && !TrophyScreen.Visible && Busje.benzineInTank <= 0) TrophyScreen.Show(Trophies.Bronze, NarratorText.AfleveringBenzine[0], NarratorText.AfleveringBenzine[1]);

            if (Busje.z > -500)
            {
                leveltime = DateTime.Now - starttime;
            }
            else 
            {
                Trophies trophy = GetTrophy((int)leveltime.TotalSeconds);
                if (TrophyScreen != null && !TrophyScreen.Visible)
                {
                    TrophyScreen.Show(trophy, NarratorText.AfleveringWinScreenCaptions[(int)trophy], NarratorText.UitnodigingWinScreenText[(int)trophy]);

                }
            }

            terrain.Update(gameTime);

            Skybox.update(PlayerObject.Position);

            Busje.update(gameTime, keyboardState, Camera);
            (GetGameObject3D("PowerupBenzine") as PowerupBenzine).update(gameTime);
            (GetGameObject3D("PowerupRocket") as PowerupRocket).update(gameTime);

            mouseInput();

            RaceHud.Update(gameTime);
            RaceHud.SetFuelPercentage(MathHelper.Clamp(Busje.benzineInTank / 130.0f * 100.0f, 0.0f, 100.0f));
            RaceHud.SetSpeedPercentage(Busje.speed * 180.0f);

            TrophyScreen.Update(gameTime);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            terrain.Draw(gameTime);
            base.Draw(gameTime);
            infoBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone);
            //infoBatch.DrawString(Speed, "Time: : " + leveltime, new Vector2(450, 45), Color.Black);

            if(TrophyScreen.Visible) TrophyScreen.Draw(gameTime, infoBatch);

            RaceHud.Draw(gameTime, infoBatch);
            infoBatch.End();
        }

        public void mouseInput() 
        {      
            MouseState mouseState = Mouse.GetState();
            if (mouseState.X != previousMouseState.X || mouseState.Y != previousMouseState.Y)
            {   
                Camera.Rotate((mouseState.X - previousMouseState.X) * -0.1f, 0.0f);
                Busje.adjustCamera += (mouseState.X - previousMouseState.X) * 0.1f;
            }
            previousMouseState = mouseState;           
        }
    }
}
