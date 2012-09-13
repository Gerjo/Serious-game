using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeriousGameLib;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Kantoor3D.GameObjects
{
    public class BuroStoel : GameObject3D
    {
        public BuroStoel()
        {
            Model = Content.Load<Model>(@"Kantoor3D\Models\Burostoel_3DWereld_Ready");
            Position = new Vector3(9.0f, 0.0f, 0.0f);

            BoundingBoxScale = new Vector3(0.6f, 0.75f, 0.6f);
            BoundingBoxOffset = new Vector3(-0.2f, 0.45f, 0);

            Scale = new Vector3(0.25f, 0.25f, 0.25f);
        }
    }
}
