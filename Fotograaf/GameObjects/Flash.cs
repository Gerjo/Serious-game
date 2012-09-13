using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeriousGameLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Fotograaf
{
    public class Flash : GameObject
    {
        private Color _color;
        private Rectangle _viewPort;
        private double _flashTimer;
        private bool _isFadingToVisible;

        public Flash(GameWorld owner)
            : base(owner)
        {
            Texture             = Content.Load<Texture2D>("Fotograaf/Images/flash");
            _color              = new Color(225,225,225, 225);
            _viewPort           = new Rectangle();
            _flashTimer         = 0;
            _isFadingToVisible  = false;

            Visible             = true;
        }

        public void doFlash()
        {
            _isFadingToVisible = true;
        }

        public override void Update(GameTime gameTime)
        {
            _viewPort = new Rectangle(0, 0, _owner.GraphicsDevice.Viewport.Width, _owner.GraphicsDevice.Viewport.Height);

            if ((_flashTimer += gameTime.ElapsedGameTime.Milliseconds) > 10)
            {
                if (_isFadingToVisible)
                {
                    _color = new Color(225, 225, 225, _color.A - 60);
                    if (_color.A <= 0) _isFadingToVisible = false;
                }
                else
                {
                    _color = new Color(225, 225, 225, _color.A + 60);
                }
                
                _flashTimer = 0;
            }
        }

        public override void Draw(GameTime gameTime, Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            if (_color.A >= 225) return;
            spriteBatch.Draw(Texture, _viewPort, _color);
        }
    }
}
