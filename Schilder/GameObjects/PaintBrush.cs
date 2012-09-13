using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeriousGameLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Schilder
{
    public class PaintBrush : GameObject
    {
        private Rectangle _sourceRect;

        public PaintBrush(GameWorld game)
            : base(game)
        {
            Texture = Content.Load<Texture2D>("schilder/images/brush");

            _sourceRect = new Rectangle(0, 0, Texture.Width, Texture.Height);

            // This class gets drawn via Kantoor3D not via Schilder.
            Visible = false;
        }

        public override void Update(GameTime gameTime)
        {

        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (_owner.TrophyScreen.Visible) return;
            Position = new Vector2(Mouse.GetState().X, Mouse.GetState().Y - Texture.Height);
            spriteBatch.Draw(Texture, Position, (_owner as Schilder).ColorSelector.PaintColor);
            spriteBatch.Draw(Texture, Position, _sourceRect, (_owner as Schilder).ColorSelector.PaintColor, 0, Vector2.Zero, 0, SpriteEffects.None, 1);
        }
    }
}
