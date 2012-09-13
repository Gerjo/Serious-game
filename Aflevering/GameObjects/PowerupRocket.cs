using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeriousGameLib;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Aflevering.GameObjects
{
    public class PowerupRocket : GameObject3D
    {
        public float y= 0;
        public bool movingUp;

        public PowerupRocket()
        {
            Model = Content.Load<Model>(@"Aflevering\Models\PU_Rocket2");

            BoundingBoxScale = new Vector3(1.2f, 12.0f, 1.2f);
            BoundingBoxOffset = new Vector3(0.0f, 0.4f, 0.0f);

            Scale = new Vector3(0.4f, 0.4f, 0.4f);

        }

        public void update(GameTime gametime)
        {
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
