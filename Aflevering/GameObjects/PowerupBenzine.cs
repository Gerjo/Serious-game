using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeriousGameLib;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Aflevering.GameObjects
{
    public class PowerupBenzine : GameObject3D
    {

        public float y;
        public bool movingUp;

        public PowerupBenzine()
        {            
            y = 0;
            Model = Content.Load<Model>(@"Aflevering\Models\PU_Benzine");

            BoundingBoxScale = new Vector3(1.2f, 1.4f, 1.2f);
            BoundingBoxOffset = new Vector3(0.0f, 0.0f, 0.0f);

            Scale = new Vector3(0.25f, 0.25f, 0.25f);

        }

        public void update(GameTime gametime) {
            RotateY += .1f;
            switch (movingUp)
            {
                case true:
                    if (y < 50f)
                    {
                        Position += new Vector3(0.0f, 0.004f, 0.0f);
                        y++;
                    }
                    else
                    {
                        movingUp = false;
                    }
                    break;
                case false:
                    if (y > 0)
                    {
                        Position -= new Vector3(0.0f, 0.004f, 0.0f);
                        y--;
                    }
                    else
                    {
                        movingUp = true;
                    }
                    break;
            }
        }
    }
}
