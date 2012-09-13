using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SeriousGameLib;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Fotograaf
{
    public class Camera : GameObject
    {
        private int _zoomDelay      = 20;
        private float _zoomStepSize = 0.2f;

        // Top, right, bottom, left.
        private int[] _cameraMargin = { 90, 190, 80, 95};

        private Vector2[] _guldenCoords;
        private ScreenShot _screenshotStream;
        private float _lastZoomAction;

        public Camera(GameWorld owner)
            : base(owner)
        {
            Texture             = Content.Load<Texture2D>("Fotograaf/Images/camera");
            Position            = Vector2.Zero;
            Visible             = false;
            _lastZoomAction     = 0;
            _screenshotStream   = new ScreenShot(); 

            // The gulden snede, coords are relative to the LCD screen center.
            _guldenCoords    = new Vector2[4];
            _guldenCoords[0] = new Vector2(-40, -20);   // Left top
            _guldenCoords[1] = new Vector2( 40, -20);   // Right top
            _guldenCoords[2] = new Vector2( 40,  20);   // Right bottom
            _guldenCoords[3] = new Vector2(-40,  20);   // Left bottom

        }

        private float CalculateScore()
        {
            WalkingDirections walkingDirection = (_owner as Fotograaf).Cat.WalkingDirection;
            Cat cat = (_owner as Fotograaf).Cat;
            float tentativeScore = 0;

            /*
            switch (walkingDirection)
            {
                case WalkingDirections.LEFT:
                    tentativeScore = Vector2.DistanceSquared(_guldenCoords[3] + Position, cat.RealPosition);
                    break;
                case WalkingDirections.RIGHT:
                    tentativeScore = Vector2.DistanceSquared(_guldenCoords[2] + Position, cat.RealPosition);
                    break;
            }*/


            tentativeScore = Vector2.Distance(cat.CatHead, Position);

            tentativeScore = (tentativeScore > 100) ? 0 : 100 - tentativeScore;

            return (int)tentativeScore;
        }

        public override void Update(GameTime gameTime)
        {
            UpdateCameraPosition();

            if ((_owner as Fotograaf).MapEditor.IsEnabled) return;

            // TODO: delay
            if (!_owner.TrophyScreen.Visible && !_owner.AdviceToCancelInput && Input.IsTakePhotoButtonPressed()) 
            {
                ScreenShot screenshot       = new ScreenShot();

                screenshot.SamplePosition   = new Vector2(Position.X, Position.Y);
                screenshot.Target           = _screenshotStream.Target;
                screenshot.ZoomLevel        = _screenshotStream.ZoomLevel;
                screenshot.ScreenshotData   = (_owner as Fotograaf).GetScreenShotStill();
                screenshot.PhotoScrore      = CalculateScore();

                (_owner as Fotograaf).Hud.AddScreenshot(screenshot);
                (_owner as Fotograaf).Flash.doFlash();

                AudioFactory.PlayOnce("photoClick");
            }

            if (!_owner.TrophyScreen.Visible && (_lastZoomAction += gameTime.ElapsedGameTime.Milliseconds) > _zoomDelay)
            {
                // TODO: implement time based delay. 
                if (!_owner.TrophyScreen.Visible &&Input.IsZoomIn() && _screenshotStream.ZoomLevel > 0.5)
                {
                    _screenshotStream.ZoomLevel -= _zoomStepSize;
                    AudioFactory.PlayOnce("zoom");

                    _lastZoomAction = 0;
                }
                if (Input.IsZoomOut() && _screenshotStream.ZoomLevel < 2)
                {
                    _screenshotStream.ZoomLevel += _zoomStepSize;
                    AudioFactory.PlayOnce("zoom");
                    _lastZoomAction = 0;
                }
            }
        }

        private void UpdateCameraPosition()
        {
            if (_owner.TrophyScreen.Visible) return;
            Position = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);

            if (Position.Y < _cameraMargin[0])
            {
                Position = new Vector2(Position.X, _cameraMargin[0]);
            }

            if (Position.X > _owner.GraphicsDevice.Viewport.Width - _cameraMargin[1])
            {
                Position = new Vector2(_owner.GraphicsDevice.Viewport.Width - _cameraMargin[1], Position.Y);
            }

            if (Position.Y > _owner.GraphicsDevice.Viewport.Height - _cameraMargin[2])
            {
                Position = new Vector2(Position.X, _owner.GraphicsDevice.Viewport.Height - _cameraMargin[2]);
            }

            if (Position.X < _cameraMargin[3])
            {
                Position = new Vector2(_cameraMargin[3], Position.Y);
            }
        }

        public override void Draw(GameTime gameTime, Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {

            _screenshotStream.Position       = Position;
            _screenshotStream.SamplePosition = Position;
            _screenshotStream.ScreenshotData = (_owner as Fotograaf).GetScreenShotStream();
            _screenshotStream.PhotoScrore    = CalculateScore();
            _screenshotStream.Draw(spriteBatch);

            spriteBatch.Draw(Texture, new Vector2(Position.X - Texture.Width / 2, Position.Y - Texture.Height / 2), Color.White);

            /*
            Vector2 catCenter    = (_owner as Fotograaf).Cat.Position;
            int bestSnedeIndex   = -1;
            float bestSnedeDist  = -1;
            float tmpDist        = -1;
            
            for (int i = 0; i < _guldenCoords.Length; ++i)
            {
                tmpDist = Vector2.DistanceSquared(_guldenCoords[i] + Position, (_owner as Fotograaf).Cat.RealPosition);
                if (bestSnedeDist == -1 || tmpDist < bestSnedeDist)
                {
                    bestSnedeDist  = tmpDist;
                    bestSnedeIndex = i;
                }
            }

            
            
            Tools.DrawPixel(spriteBatch, _guldenCoords[bestSnedeIndex] + Position);*/

            //Tools.DrawPixel(spriteBatch, (_owner as Fotograaf).Cat.CatHead);
        }
    }
}
