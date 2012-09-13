using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeriousGameLib;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Aflevering.GameObjects
{
    public class ParkBench : GameObject3D
    {
        public ParkBench()
        {
            Model = Content.Load<Model>(@"Aflevering\Models\ParkBench");

            Scale = new Vector3(1.0f, 1.0f, 1.0f);

           
        }

    }
}
