using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeriousGameLib;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Kantoor3D.GameObjects
{
    public class KeyboardModel : GameObject3D
    {
        public KeyboardModel()
        {
            Model = Content.Load<Model>(@"Kantoor3D\Models\Keyboard");
            Position = new Vector3(9.0f, 2.4f, 0.1f);

            BoundingBoxScale = new Vector3(0.85f, 0.6f, 0.3f);
            BoundingBoxOffset = new Vector3(-0.1f, 0.35f, 0.0f);

            Scale = new Vector3(0.30f, 0.30f, 0.30f);

            RotateY = 270.0f;
        }
    }
}
