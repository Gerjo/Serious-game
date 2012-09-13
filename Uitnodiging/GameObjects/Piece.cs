using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeriousGameLib;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Uitnodiging
{
    public class Piece : GameObject
    {
        public int _x = 0;
        public int _y = 0;

        private int pieceWidth = 120;
        private int pieceHeight = 120;

        private int x = 50;
        private int y = 50;

        public int _id;
        public int _image;
        public bool selected = false;
        public bool spawned = false;

        public Piece(GameWorld owner, int id, int x, int y)
            : base(owner)
        {
            _x = x;
            _y = y;
            _id = id;
            _image = id + 1;

            Texture = Content.Load<Texture2D>("Uitnodiging/SheepScape/sheepscape_" + ((_image < 10) ? "0" : "") + _image);

        }

        public bool Locate(MouseState mouseCurrent) {
            if (mouseCurrent.X > _x + x && mouseCurrent.X < _x + x + pieceWidth && mouseCurrent.Y > _y + y && mouseCurrent.Y < _y + y + pieceHeight) 
            {
                return true;
            }
            return false;
        }


        public void Move(MouseState mouseCurrent)
        {
            _x = mouseCurrent.X - Texture.Width / 2;

            _y = mouseCurrent.Y - Texture.Height / 2;
        }

        public override void Draw(GameTime gameTime, Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            if (spawned)
            {
                spriteBatch.Draw(Texture, new Rectangle(_x, _y, Texture.Width, Texture.Height), Color.White);
            }
        }

        public override void Update(GameTime gameTime)
        {

        }
    }
}
