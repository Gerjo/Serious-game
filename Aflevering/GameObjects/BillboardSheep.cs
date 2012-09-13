using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeriousGameLib;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Aflevering.GameObjects
{
    public class BillBoardSheep : GameObject3D
    {

        public BillBoardSheep()
        {
            Model = Content.Load<Model>(@"Aflevering\Models\Billboard_Sheep");

            Scale = new Vector3(1.5f, 1.5f, 1.5f);

            RotateY = 0.0f;
        }
    }
}