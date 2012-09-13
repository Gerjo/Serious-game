using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeriousGameLib;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Aflevering.GameObjects
{
    public class Tree : GameObject3D
    {

        public Tree()
        {
            Model = Content.Load<Model>(@"Aflevering\Models\LowPoly_Tree");

            Scale = new Vector3(0.8f, 0.8f, 0.8f);

            RotateY = 0.0f;
        }
    }
}