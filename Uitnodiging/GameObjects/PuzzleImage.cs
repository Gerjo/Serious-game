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
    public class PuzzleImage : GameObject
    {
        public int _x = 89;
        public int _y = 89;

        public PuzzleImage(GameWorld owner)
            : base(owner, false)
        {
            Texture = Content.Load<Texture2D>("Uitnodiging/Sheepscape/sheepscapeDull");
        }


        public override void Draw(GameTime gameTime, Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, new Rectangle(_x, _y, Texture.Width, Texture.Height), Color.White);
        }

        public override void Update(GameTime gameTime)
        {
            
        }
    }
}
