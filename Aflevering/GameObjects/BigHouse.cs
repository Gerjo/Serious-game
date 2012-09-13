using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeriousGameLib;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Aflevering.GameObjects
{
    public class BigHouse : GameObject3D
    {

        public BigHouse()
        {
            Model = Content.Load<Model>(@"Aflevering\Models\BigHouse_HarunExportFix");

            Scale = new Vector3(2.0f, 2.0f, 2.0f);

            RotateY = 0.0f;
        }
    }
}