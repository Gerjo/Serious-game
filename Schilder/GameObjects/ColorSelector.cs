using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeriousGameLib;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Schilder
{
    public class ColorSelector : GameObject
    {
        private Texture2D _plusSymbol;
        private Texture2D _minusSymbol;

        private Texture2D _coloredStain;
        private Vector2 _coloredStainOffset;

        private Vector2[] _minusPositions;
        private Vector2 _plusOffset;

        private int _colorChangeCounter;
        private int _colorChangeDelay   = 50; // Delay between each in- or decrement.

        private SpriteFont _font;

        public Color PaintColor;

        public ColorSelector(GameWorld game)
            : base(game)
        {
            _font               = Content.Load<SpriteFont>("Fotograaf/Fonts/defaultfont");
            _plusSymbol         = Content.Load<Texture2D>("Schilder/Images/plus");
            _minusSymbol        = Content.Load<Texture2D>("Schilder/Images/minus");
            _coloredStain       = Content.Load<Texture2D>("Schilder/Images/stain");
            Texture             = Content.Load<Texture2D>("Schilder/Images/pallette_hd");
            Position            = new Vector2(20, _owner.GraphicsDevice.Viewport.Height / 2 - 80);
            _coloredStainOffset = new Vector2(165, 182);

            // NB.: These are relative to the Position vector.
            _minusPositions     = new Vector2[] {
                    new Vector2(70, 90),        // Red
                    new Vector2(175, 55),       // green
                    new Vector2(295, 75)};      // blue
            _plusOffset = new Vector2(40, 0);

            PaintColor = Color.Brown;
        }

        public override void Update(GameTime gameTime)
        {
            _colorChangeCounter += gameTime.ElapsedGameTime.Milliseconds;
        }

        private bool IsColorChangeTimerExpired()
        {
            if (_colorChangeCounter > _colorChangeDelay)
            {
                _colorChangeCounter = 0;
                return true;
            }
            return false;
        }

        private bool CheckMouseIntersect(Vector2 pos)
        {
            return (!_owner.TrophyScreen.Visible && 
                    Mouse.GetState().X > pos.X && Mouse.GetState().X < pos.X + _minusSymbol.Width &&
                    Mouse.GetState().Y > pos.Y && Mouse.GetState().Y < pos.Y + _minusSymbol.Height);
        }

        private void IncrementColor(int index)
        {
            if (!IsColorChangeTimerExpired()) return;
            switch (index)
            {
                case 0: if (PaintColor.R <= 244) PaintColor.R += 10; else PaintColor.R = 255; break;
                case 1: if (PaintColor.G <= 244) PaintColor.G += 10; else PaintColor.G = 255; break;
                case 2: if (PaintColor.B <= 244) PaintColor.B += 10; else PaintColor.B = 255; break;
                case 3: if (PaintColor.A <= 244) PaintColor.A += 10; else PaintColor.A = 255; break;
            }
        }

        private void DecrementColor(int index)
        {
            if (!IsColorChangeTimerExpired()) return;
            switch (index)
            {
                case 0: if (PaintColor.R > 10) PaintColor.R -= 10; else PaintColor.R = 2; break;
                case 1: if (PaintColor.G > 10) PaintColor.G -= 10; else PaintColor.G = 2; break;
                case 2: if (PaintColor.B > 10) PaintColor.B -= 10; else PaintColor.B = 2; break;
                case 3: if (PaintColor.A > 10) PaintColor.A -= 10; else PaintColor.A = 2; break;
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            
            spriteBatch.Draw(Texture, Position, Color.White);
            spriteBatch.Draw(_coloredStain, Position + _coloredStainOffset, PaintColor);

            for(int i = 0; i < _minusPositions.Length; ++i) {
                Vector2 pos = Position + _minusPositions[i];

                switch(i) {
                    case 0: spriteBatch.DrawString(_font, (int)(PaintColor.R / 2.55) + "%", pos + new Vector2(10, -10), Color.White); break;
                    case 1: spriteBatch.DrawString(_font, (int)(PaintColor.G / 2.55) + "%", pos + new Vector2(10, -10), Color.White); break;
                    case 2: spriteBatch.DrawString(_font, (int)(PaintColor.B / 2.55) + "%", pos + new Vector2(10, -10), Color.White); break;
                }


                if (CheckMouseIntersect(pos))
                {
                    if (Mouse.GetState().LeftButton.Equals(ButtonState.Pressed)) DecrementColor(i); 
                    spriteBatch.Draw(_minusSymbol, pos, Color.Red);
                }
                else spriteBatch.Draw(_minusSymbol, pos, Color.White);

                pos += _plusOffset;

                if (CheckMouseIntersect(pos))
                {
                    if (Mouse.GetState().LeftButton.Equals(ButtonState.Pressed)) IncrementColor(i);
                    spriteBatch.Draw(_plusSymbol, pos, Color.Red);
                }
                else spriteBatch.Draw(_plusSymbol, pos, Color.White);
                
            }
        }
    }
}
