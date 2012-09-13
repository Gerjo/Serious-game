using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeriousGameLib;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Kantoor3D.GameObjects
{
    public class Buro : GameObject3D
    {
        public Buro()
        {
            Model = Content.Load<Model>(@"Kantoor3D\Models\Buro_3DWereld_Ready");
            Position = new Vector3(10.0f, 0.0f, 0.0f);

            BoundingBoxScale = new Vector3(0.35f, 0.3f, 0.77f);
            BoundingBoxOffset = new Vector3(0.25f, 0.8f, -0.15f);
        }
    }
}
