using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SeriousGameLib;
using Microsoft.Xna.Framework.Graphics;

namespace Fotograaf
{
    public static class Tools
    {
        private static Texture2D _texturePixel  = null;
        private static Texture2D _textureCircle = null;
        private static SpriteFont _spriteFont   = null;

        public static void DrawPixel(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, Vector2 position)
        {
            DrawPixel(spriteBatch, position, Color.White);
        }

        public static void DrawPixel(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, Vector2 position, Color color)
        {
            if (_texturePixel == null) _texturePixel = GameObject.Content.Load<Texture2D>("Fotograaf/Images/cross");
            
            spriteBatch.Draw(_texturePixel, new Vector2(position.X - _texturePixel.Width / 2, position.Y - _texturePixel.Height / 2), color);
        }

        public static void DrawCircle(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, Vector2 position)
        {
            DrawCircle(spriteBatch, position, Color.White);
        }

        public static void DrawCircle(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, Vector2 position, Color color)
        {
            if (_textureCircle == null) _textureCircle = GameObject.Content.Load<Texture2D>("Fotograaf/Images/circle");

            spriteBatch.Draw(_textureCircle, new Vector2(position.X - _textureCircle.Width / 2, position.Y - _textureCircle.Height / 2), color);
        }

        public static void DrawText(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, Vector2 position, string text)
        {
            DrawText(spriteBatch, position, Color.Black, text);
        }

        public static void DrawText(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, Vector2 position, Color color, string text)
        {
            if (_spriteFont == null) _spriteFont = GameObject.Content.Load<SpriteFont>("Fotograaf/Fonts/defaultfont");

            spriteBatch.DrawString(_spriteFont, text, position, color);
        }

        public static void DrawTextCentered(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, Vector2 position, string text)
        {
            DrawTextCentered(spriteBatch, position, Color.Black, text);
        }

        public static void DrawTextCentered(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, Vector2 position, Color color, string text)
        {
            if (_spriteFont == null) _spriteFont = GameObject.Content.Load<SpriteFont>("Fotograaf/Fonts/defaultfont");

            Vector2 boundingBox = _spriteFont.MeasureString(text);

            spriteBatch.DrawString(_spriteFont, text, new Vector2((float)Math.Ceiling(position.X - boundingBox.X / 2), position.Y), color);
        }
    }
}
