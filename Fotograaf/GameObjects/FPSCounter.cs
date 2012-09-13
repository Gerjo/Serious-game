using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SeriousGameLib;
using Microsoft.Xna.Framework.Graphics;

namespace Fotograaf
{
    class FPSCounter : SeriousGameLib.GameObject
    {
        private int _updateCounter;
        private int _drawCounter;
        private int _updateCounterBuffer;
        private int _drawCounterBuffer;
        private double _updateCounterTimer;
        private double _drawCounterTimer;

        public Color Color { get; set; }

        public FPSCounter(GameWorld game) : base(game)
        {
            Position      = Vector2.Zero;
            Color         = Color.Black;
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            _updateCounter++;

            // FYI: This is more like an "avarage frames per second" counter.
            if ((_updateCounterTimer += gameTime.ElapsedGameTime.Milliseconds) > 250)
            {
                _updateCounterBuffer = _updateCounter * 4;
                _updateCounter       = 0;
                _updateCounterTimer  = 0;
            }
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime, Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            _drawCounter++;
             
            // FYI: This is more like an "avarage frames per second" counter.
            if ((_drawCounterTimer += gameTime.ElapsedGameTime.Milliseconds) > 250)
            {
                _drawCounterBuffer  = _drawCounter * 4;
                _drawCounter        = 0;
                _drawCounterTimer   = 0;
            }

            Tools.DrawText(spriteBatch, new Vector2(), Color, "Updates: " + _updateCounterBuffer + " per second. \nDraws: " + _drawCounterBuffer + " per second.");
        }
    }
}
