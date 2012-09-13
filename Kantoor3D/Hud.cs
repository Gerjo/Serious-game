using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SeriousGameLib;
using System;

namespace Kantoor3D
{
    public class Hud
    {
        private Texture2D _frameLeft;
        private Texture2D _frameRight;
        private Texture2D _frameTop;
        private Texture2D _frameBottom;

        private Texture2D _frameLeftTop;
        private Texture2D _frameRightTop;
        private Texture2D _frameLeftBottom;
        private Texture2D _frameRightBottom;
        private Texture2D _terug_naar_kantoor;

        private SpriteFont _comicAndy;

        private Texture2D _trophiesBackground;
        private Texture2D _EssenzaMediaGame;

        private Kantoor3D _owner;

        public Hud(Kantoor3D owner)
        {
            _owner              = owner;
            _frameLeft          = SeriousGameLib.GameObject.Content.Load<Texture2D>("Hud/Frame/frame_left");
            _frameRight         = SeriousGameLib.GameObject.Content.Load<Texture2D>("Hud/Frame/frame_right");
            _frameTop           = SeriousGameLib.GameObject.Content.Load<Texture2D>("Hud/Frame/frame_top");
            _frameBottom        = SeriousGameLib.GameObject.Content.Load<Texture2D>("Hud/Frame/frame_bottom");
            _frameLeftTop       = SeriousGameLib.GameObject.Content.Load<Texture2D>("Hud/Frame/frame_top_Left");
            _frameRightTop      = SeriousGameLib.GameObject.Content.Load<Texture2D>("Hud/Frame/frame_top_Right");
            _frameLeftBottom    = SeriousGameLib.GameObject.Content.Load<Texture2D>("Hud/Frame/frame_bottom_Left");
            _frameRightBottom   = SeriousGameLib.GameObject.Content.Load<Texture2D>("Hud/Frame/frame_bottom_Right");
            _terug_naar_kantoor = SeriousGameLib.GameObject.Content.Load<Texture2D>("Hud/terug_naar_kantoor");

            _trophiesBackground = SeriousGameLib.GameObject.Content.Load<Texture2D>("Hud/trophies_background");
            _EssenzaMediaGame   = SeriousGameLib.GameObject.Content.Load<Texture2D>("Hud/EssenzaMediaGame");

            _comicAndy          = SeriousGameLib.GameObject.Content.Load<SpriteFont>("Hud/Fonts/TrophyFont");
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime, object miniGame)
        {
            DrawTrophies(spriteBatch);
            DrawFrame(spriteBatch);
            DrawReturnToOffice(spriteBatch, miniGame);
            DrawEssenzaMediaGame(spriteBatch);
        }

        private void DrawTrophies(SpriteBatch spriteBatch)
        {
            Vector2 pos = new Vector2(_owner.GraphicsDevice.Viewport.Width - _trophiesBackground.Width, 0);
            spriteBatch.Draw(_trophiesBackground, pos, Color.White);

            spriteBatch.DrawString(_comicAndy, " " + SeriousGameLib.PersistentStorage.Trophies[(int)Trophies.Gold],   pos + new Vector2( 90, 20), new Color(155, 120, 24));
            spriteBatch.DrawString(_comicAndy, " " + SeriousGameLib.PersistentStorage.Trophies[(int)Trophies.Silver], pos + new Vector2(250, 23), new Color(155, 120, 24));
            spriteBatch.DrawString(_comicAndy, " " + SeriousGameLib.PersistentStorage.Trophies[(int)Trophies.Bronze], pos + new Vector2(395, 20), new Color(155, 120, 24));
        }

        private void DrawFrame(SpriteBatch spriteBatch)
        {
            Rectangle left = new Rectangle(0, _frameLeftTop.Height, _frameLeft.Width, _owner.GraphicsDevice.Viewport.Height - _frameLeftBottom.Height - _frameLeftTop.Height);
            Rectangle top = new Rectangle(_frameLeftTop.Width, 0, _owner.GraphicsDevice.Viewport.Width - _frameLeftTop.Width - _frameRightTop.Width, _frameTop.Height);
            Rectangle right = new Rectangle(_owner.GraphicsDevice.Viewport.Width - _frameRight.Width, _frameRightTop.Height, _frameRight.Width, _owner.GraphicsDevice.Viewport.Height - _frameRightBottom.Height * 2);
            Rectangle bottom = new Rectangle(_frameLeftBottom.Width, _owner.GraphicsDevice.Viewport.Height - _frameBottom.Height, _owner.GraphicsDevice.Viewport.Width - _frameRightBottom.Width * 2, _frameTop.Height);

            spriteBatch.Draw(_frameLeft, left, Color.White);
            spriteBatch.Draw(_frameTop, top, Color.White);
            spriteBatch.Draw(_frameRight, right, Color.White);
            spriteBatch.Draw(_frameBottom, bottom, Color.White);

            Rectangle leftTop = new Rectangle(0, 0, _frameLeftTop.Width, _frameLeftTop.Height);
            Rectangle rightTop = new Rectangle(_owner.GraphicsDevice.Viewport.Width - _frameRightTop.Width, 0, _frameRightTop.Width, _frameRightTop.Height);

            Rectangle leftBottom = new Rectangle(0, _owner.GraphicsDevice.Viewport.Height - _frameLeftBottom.Height, _frameLeftBottom.Width, _frameLeftBottom.Height);
            Rectangle rightBottom = new Rectangle(_owner.GraphicsDevice.Viewport.Width - _frameRightTop.Width, _owner.GraphicsDevice.Viewport.Height - _frameLeftBottom.Height, _frameRightBottom.Width, _frameRightBottom.Height);

            spriteBatch.Draw(_frameLeftTop, leftTop, Color.White);
            spriteBatch.Draw(_frameRightTop, rightTop, Color.White);
            spriteBatch.Draw(_frameLeftBottom, leftBottom, Color.White);
            spriteBatch.Draw(_frameRightBottom, rightBottom, Color.White);
        }

        private void DrawReturnToOffice(SpriteBatch spriteBatch, object miniGame)
        {
            if (_owner.CurrentGameState == GameStates.MINIGAME || _owner.Aflevering != null)
            {
                Vector2 pos = new Vector2(_owner.GraphicsDevice.Viewport.Width - _terug_naar_kantoor.Width, _owner.GraphicsDevice.Viewport.Height - _terug_naar_kantoor.Height);
                spriteBatch.Draw(_terug_naar_kantoor, pos, Color.White);

                // Return the controls back to the "kantoor"
                if (new Rectangle((int)pos.X, (int)pos.Y, _terug_naar_kantoor.Width, _terug_naar_kantoor.Height).Contains(new Point(Mouse.GetState().X, Mouse.GetState().Y)))
                {

                    // Let the minigame know that the HUD is taking over controls:
                    if (_owner.CurrentMiniGame != null)
                        _owner.CurrentMiniGame.AdviceToCancelInput = true;

                    spriteBatch.Draw(_terug_naar_kantoor, pos, Color.Red);
                    if (Mouse.GetState().LeftButton.Equals(ButtonState.Pressed))
                    {
                        if(miniGame is GameWorld) {
                            if ((miniGame as GameWorld).TrophyScreen != null)
                                (miniGame as GameWorld).TrophyScreen.ShowReturnToOfficeConfirm();
                            else (miniGame as GameWorld).ForceUnload = true;

                        }
                        else if (miniGame is GameWorld3D)
                        {
                            if ((miniGame as GameWorld3D).TrophyScreen != null)
                                (miniGame as GameWorld3D).TrophyScreen.ShowReturnToOfficeConfirm();
                            else (miniGame as GameWorld3D).ForceUnload = true;
                        }
                    }

                }
                else
                {
                    spriteBatch.Draw(_terug_naar_kantoor, pos, Color.White);

                    // Let the minigame know that the HUD is inactive.
                    if (_owner.CurrentMiniGame != null)
                        _owner.CurrentMiniGame.AdviceToCancelInput = false;
                }
            }
        }

        private void DrawEssenzaMediaGame(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_EssenzaMediaGame, new Vector2(10, 10), Color.White);
        }

        public void Update(GameTime gameTime)
        {
            
        }
    }
}
