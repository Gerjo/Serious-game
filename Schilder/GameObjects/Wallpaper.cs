using SeriousGameLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace Schilder
{
    public class Wallpaper : GameObject
    {
        private Rectangle _monitorViewPort;

        public Wallpaper(GameWorld game)
            : base(game)
        {
            Texture                 = Content.Load<Texture2D>("Schilder/Images/muur voor kantoor");
            _monitorViewPort        = new Rectangle();
        }

        // Will resize the viewport, if it has changed.
        public override void Update(GameTime gameTime)
        {
            if (_monitorViewPort.Width != _owner.GraphicsDevice.Viewport.Width || _monitorViewPort.Height != _owner.GraphicsDevice.Viewport.Height)
            {
                _monitorViewPort.Width  = _owner.GraphicsDevice.Viewport.Width;
                _monitorViewPort.Height = _owner.GraphicsDevice.Viewport.Height;
            }
        }

        public override void Draw(GameTime gameTime, Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, _monitorViewPort, Color.White);
        }
    }
}
