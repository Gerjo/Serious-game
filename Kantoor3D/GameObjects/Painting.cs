using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeriousGameLib;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Kantoor3D.GameObjects
{
    public class Painting : GameObject3D
    {
        public Painting()
        {
            Model = Content.Load<Model>(@"Kantoor3D\Models\painting");

            BoundingBoxScale = new Vector3(0.7f, 0.6f, 0.3f);
            BoundingBoxOffset = new Vector3(-0.1f, 0.35f, 0.0f);

            Scale = new Vector3(1.0f, 1.0f, 1.0f);

            RotateY = 0.0f;
        }
    }
}
