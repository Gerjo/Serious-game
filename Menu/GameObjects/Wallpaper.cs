using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeriousGameLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Menu
{
    public class Wallpaper : GameObject
    {
        private Rectangle _viewPort;

        public Wallpaper(GameWorld owner)
            : base(owner)
        {
            Texture = Content.Load<Texture2D>("Menu/Images/menubackground");

        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {

        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime, Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            if(_viewPort == null || _viewPort.Width != _owner.GraphicsDevice.Viewport.Width || _viewPort.Height != _owner.GraphicsDevice.Viewport.Height)
                _viewPort = new Rectangle(0, 0, _owner.GraphicsDevice.Viewport.Width, _owner.GraphicsDevice.Viewport.Height);

            spriteBatch.Draw(Texture, _viewPort, Color.White);
        }
    }
}
