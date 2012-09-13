using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeriousGameLib;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Kantoor3D.GameObjects
{
    public class Watercooler : GameObject3D
    {
        public Watercooler()
        {
            Model = Content.Load<Model>(@"Kantoor3D\Models\Watercooler_WithoutSecretFactoryHack");
            Position = new Vector3(10.0f, 0.0f, -10.0f);

            BoundingBoxScale = new Vector3(0.4f, 0.9f, 0.4f);
            BoundingBoxOffset = new Vector3(0.25f, 0.3f, 0.0f);

            Scale = new Vector3(0.3f, 0.3f, 0.3f);

            RotateY = -45.0f;
        }
    }
}
