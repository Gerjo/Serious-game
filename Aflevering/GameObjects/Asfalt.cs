using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeriousGameLib;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Aflevering.GameObjects
{
    public class Asfalt : GameObject3D
    {

        public Asfalt()
        {
            Model = Content.Load<Model>(@"Aflevering\Models\Asfalt");

            Scale = new Vector3(1.6f, 1.6f, 1.6f);

            RotateY = 0.0f;
        }
    }
}