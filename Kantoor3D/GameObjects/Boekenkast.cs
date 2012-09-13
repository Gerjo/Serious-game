using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeriousGameLib;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Kantoor3D.GameObjects
{
    public class Boekenkast : GameObject3D
    {
        public Boekenkast()
        {
            Model = Content.Load<Model>(@"Kantoor3D\Models\Boekenkast");

            BoundingBoxScale = new Vector3(0.7f, 0.6f, 0.3f);
            BoundingBoxOffset = new Vector3(-0.1f, 0.35f, 0.0f);

            Scale = new Vector3(0.55f, 0.55f, 0.55f);

            RotateY = 0.0f;
        }
    }
}
