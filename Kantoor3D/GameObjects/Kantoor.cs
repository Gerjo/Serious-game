using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeriousGameLib;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Kantoor3D.GameObjects
{
    public class Kantoor : GameObject3D
    {
        public Kantoor()
        {
            Model = Content.Load<Model>(@"Kantoor3D\Models\Kantoor Alpha");
            this.Position = Vector3.Zero;

            BoundingBoxScale = new Vector3(0.5f, 0.45f, 0.45f);
            BoundingBoxOffset = new Vector3(2f, 1f, 0f);
        }
    }
}
