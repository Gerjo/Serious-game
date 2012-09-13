using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text;

namespace Ontwerpen
{
    class Card
    {
        public bool IsFlipped;
        public int Id;
        public int X;
        public int Y;

        public Card(int x, int y, int id)
        {
            this.X     = x;
            this.Y     = y;
            Id          = id;
            IsFlipped   = false;
        }

        public void Draw(SpriteBatch cardBatch, Texture2D cardBack, int x, int y)
        {
            cardBatch.Draw(cardBack, new Rectangle(x, y, cardBack.Width, cardBack.Height), Color.White);
        }
    }
}
