using SeriousGameLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace Fotograaf
{
    // TODO: Resolve out of screen when scrolling or window resizing.
    // TODO: make scrolling timebased rather than framebased.
    public class Wallpaper : GameObject
    {
        private int deadZoneRange    = 180;
        private int deadZoneSpeed    = 15;
        private int deadZoneDelay    = 10; // Delay in milliseconds between each "scroll"


        private float _deadZoneDelaycounter;
        private Rectangle _monitorViewPort;
        private Rectangle _wallpaperViewPort;
        public  Vector2 ScrollOffset { get { return new Vector2(_wallpaperViewPort.X, _wallpaperViewPort.Y); } }
        

        public Wallpaper(GameWorld game) : base(game)
        {
            Texture                 = Content.Load<Texture2D>("Fotograaf/Images/livingroom");
            _wallpaperViewPort      = new Rectangle();
            _deadZoneDelaycounter   = 0;
        }

        public override void Update(GameTime gameTime)
        {
            if (_monitorViewPort.Width != _owner.GraphicsDevice.Viewport.Width || _monitorViewPort.Height != _owner.GraphicsDevice.Viewport.Height)
            {
                _wallpaperViewPort.Width  = _monitorViewPort.Width  = _owner.GraphicsDevice.Viewport.Width;
                _wallpaperViewPort.Height = _monitorViewPort.Height = _owner.GraphicsDevice.Viewport.Height;
            }

            updateDeadzone(gameTime);
            
        }

        private void updateMousePosition()
        {
            Point location = new Point(Mouse.GetState().X, Mouse.GetState().Y);
            Point newLocation;

            if (location.X > _owner.GraphicsDevice.Viewport.Width)
            {
                newLocation = new Point(_owner.GraphicsDevice.Viewport.Width, location.Y);
            }

        }

        private void updateDeadzone(GameTime gameTime)
        {
            if ((_deadZoneDelaycounter += gameTime.ElapsedGameTime.Milliseconds) > deadZoneDelay)
                _deadZoneDelaycounter = 0;
            else return;

            MouseState mouse = Mouse.GetState();
            if (mouse.X > _monitorViewPort.Width - deadZoneRange && Texture.Width > (_wallpaperViewPort.X + _monitorViewPort.Width))
            {
                _wallpaperViewPort.X += deadZoneSpeed;
            }
            else if (mouse.X < deadZoneRange && _wallpaperViewPort.X > 0)
            {
                _wallpaperViewPort.X -= deadZoneSpeed;
            }

            if (mouse.Y > _monitorViewPort.Height - deadZoneRange && Texture.Height > (_wallpaperViewPort.Y + _monitorViewPort.Height))
            {
                _wallpaperViewPort.Y += deadZoneSpeed;
            }
            else if (mouse.Y < deadZoneRange && _wallpaperViewPort.Y > 0)
            {
                _wallpaperViewPort.Y -= deadZoneSpeed;
            }
        }

        public override void Draw(GameTime gameTime, Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, _monitorViewPort, _wallpaperViewPort, Color.White); 
        }
    }
}
