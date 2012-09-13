using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeriousGameLib;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Kantoor3D.GameObjects
{
    public class Hanglamp : GameObject3D
    {
        public Hanglamp()
        {
            Model = Content.Load<Model>(@"Kantoor3D\Models\hanglamp");

            BoundingBoxScale = new Vector3(0.7f, 0.6f, 0.3f);
            BoundingBoxOffset = new Vector3(-0.1f, 0.35f, 0.0f);

            Scale = new Vector3(0.40f, 0.40f, 0.40f);

            RotateY = 0.0f;
        }
    }
}
