using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeriousGameLib;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Kantoor3D.GameObjects
{
    public class Telefoon : GameObject3D
    {
        public Telefoon()
        {
            //Model = Content.Load<Model>(@"Kantoor3D\Models\telephone_ruw");
            Position = new Vector3(-7.6f, 3f, -5.0f);

            BoundingBoxScale = new Vector3(0.7f, 0.6f, 0.3f);
            BoundingBoxOffset = new Vector3(-0.1f, 0.35f, 0.0f);

            Scale = new Vector3(0.20f, 0.20f, 0.20f);

            RotateY = 0.0f;
        }
    }
}
