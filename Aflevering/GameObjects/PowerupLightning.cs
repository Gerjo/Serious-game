using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeriousGameLib;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Aflevering.GameObjects
{
    public class PowerupLightning : GameObject3D
    {

        public float x, y, z;
        public bool movingUp;

        public PowerupLightning()
        {
            y = 1.0f;
            Model = Content.Load<Model>(@"Aflevering\Models\PU_Lightning");
            Position = new Vector3(0.0f, y, 0.0f);

            BoundingBoxScale = new Vector3(1.0f, 3.4f, 1.0f);
            BoundingBoxOffset = new Vector3(0.0f, 1.4f, 0.0f);

            Scale = new Vector3(.25f, .25f, .25f);

            RotateY = 0.0f;
        }

        public void update(GameTime gametime)
        {
            RotateY += .1f;
            switch (movingUp)
            {
                case true:
                    if (y < 1.2f)
                    {
                        y += 0.5f * (float)gametime.ElapsedGameTime.TotalSeconds;
                    }
                    else
                    {
                        movingUp = false;
                    }
                    break;
                case false:
                    if (y > 0.8f)
                    {
                        y -= 0.5f * (float)gametime.ElapsedGameTime.TotalSeconds;
                    }
                    else
                    {
                        movingUp = true;
                    }
                    break;
            }


            Position = new Vector3(0.0f, y, 0.0f);
        }
    }
}
