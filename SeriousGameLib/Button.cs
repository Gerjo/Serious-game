using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SeriousGameLib
{
    // Helper class/container for buttons.
    public class Button
    {
        private static Texture2D _textureLeft;
        private static Texture2D _textureMiddle;
        private static Texture2D _textureRight;
        private static Texture2D _textureLeftHover;
        private static Texture2D _textureMiddleHover;
        private static Texture2D _textureRightHover;
        private static SpriteFont _font;

        public string Label { get; private set; }
        public int Width { get { return _boundingBox.Width; } }
        public int Height { get { return _boundingBox.Height; } }

        private Vector2 _textSize;
        private Rectangle _middleRect;
        private Vector2 _textPosition;
        private Vector2 _rightPosition;
        private Rectangle _boundingBox;
        private Vector2 _offset;

        public bool HadHover;

        public Button(string label)
        {
            this.Label = label;

            if (_textureLeft == null)
            {
                _textureLeft = GameObject.Content.Load<Texture2D>("Hud/Button/button_left");
                _textureMiddle = GameObject.Content.Load<Texture2D>("Hud/Button/button_middle");
                _textureRight = GameObject.Content.Load<Texture2D>("Hud/Button/button_right");
                _textureLeftHover = GameObject.Content.Load<Texture2D>("Hud/Button/button_left_hover");
                _textureMiddleHover = GameObject.Content.Load<Texture2D>("Hud/Button/button_middle_hover");
                _textureRightHover = GameObject.Content.Load<Texture2D>("Hud/Button/button_right_hover");
                _font = GameObject.Content.Load<SpriteFont>("Hud/Fonts/buttonFont");
            }

            // Note: Most dimensions will be offset to their location at Draw time.
            _textSize = _font.MeasureString(Label);
            _textPosition = new Vector2(_textureLeft.Width, -4);
            _rightPosition = new Vector2(_textureLeft.Width + _textSize.X, 0);
            _middleRect = new Rectangle(0, 0, (int)_textSize.X, _textureLeft.Height);

            _boundingBox = new Rectangle(0, 0, _middleRect.Width + _textureLeft.Width + _textureRight.Width, _middleRect.Height);
        }

        public bool Contains(MouseState mouse)
        {
            return Contains(new Point(mouse.X, mouse.Y));
        }

        public bool Contains(Point mouse)
        {
            return _boundingBox.Contains(new Point(mouse.X, mouse.Y));
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 offset, MouseState mouse)
        {
            Draw(spriteBatch, offset, new Point(mouse.X, mouse.Y));
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 offset, Point mouse)
        {
            _offset = offset;
            _boundingBox.X = (int)offset.X;
            _boundingBox.Y = (int)offset.Y;

            // Temp change the offset to simulate some "hover action".
            string hovername = Contains(mouse) ? "_hover" : "";

            _middleRect.X = (int)offset.X + _textureLeft.Width;
            _middleRect.Y = (int)offset.Y;

            // Mouse hover!
            if (Contains(mouse))
            {
                spriteBatch.Draw(_textureLeftHover, offset, Color.White);
                spriteBatch.Draw(_textureMiddleHover, _middleRect, Color.White);
                spriteBatch.Draw(_textureRightHover, offset + _rightPosition, Color.White);
                HadHover = true;
            }
            else
            {
                spriteBatch.Draw(_textureLeft, offset, Color.White);
                spriteBatch.Draw(_textureMiddle, _middleRect, Color.White);
                spriteBatch.Draw(_textureRight, offset + _rightPosition, Color.White);
                HadHover = false;
            }

            spriteBatch.DrawString(_font, Label, offset + _textPosition, Color.Black);
        }
    }
}
