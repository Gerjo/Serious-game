using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeriousGameLib;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Kantoor3D.GameObjects
{
    public class MouseModel : GameObject3D
    {
        public MouseModel()
        {
            Model = Content.Load<Model>(@"Kantoor3D\Models\ComputerMouse");
            Position = new Vector3(9.0f, 2.4f, 1.3f);

            BoundingBoxScale = new Vector3(0.7f, 0.6f, 0.3f);
            BoundingBoxOffset = new Vector3(-0.1f, 0.35f, 0.0f);

            Scale = new Vector3(0.05f, 0.05f, 0.05f);

            RotateY = 90.0f;
        }
    }
}
