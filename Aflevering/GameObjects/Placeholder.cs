    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeriousGameLib;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Aflevering.GameObjects
{
    public class Placeholder : GameObject3D
    {
        private float x = 0; //- = links, + = rechts
        private float y = 0;
        private float z = 0; //- = voor, + = achter

        public Placeholder()
        {
            Model = Content.Load<Model>(@"Aflevering\Models\Placeholder");
            Position = new Vector3(x, y, z);

            BoundingBoxScale = new Vector3(0.7f, 0.6f, 0.3f);
            BoundingBoxOffset = new Vector3(-0.1f, 0.35f, 0.0f);

            Scale = new Vector3(0.20f, 0.20f, 0.20f);

            RotateY = 0.0f;
        }

        public void handleInput(GameTime gametime, KeyboardState keyboardState)
        {
            if (keyboardState.IsKeyDown(Keys.Up))
            {
                
                z -= 0.002f * (float)Math.Cos(MathHelper.ToRadians(RotateY));
                x -= 0.002f * (float)Math.Sin(MathHelper.ToRadians(RotateY));
            }
            if (keyboardState.IsKeyDown(Keys.Down)) 
            {
                z += 0.002f * (float)Math.Cos(MathHelper.ToRadians(RotateY));
                x += 0.002f * (float)Math.Sin(MathHelper.ToRadians(RotateY));
            }
            if (keyboardState.IsKeyDown(Keys.Left))
            {
                RotateY += 0.1f;
            }
            if (keyboardState.IsKeyDown(Keys.Right))
            {
                RotateY -= 0.1f;
            }
            Position = new Vector3(x, y, z);
        }

    }
}
