using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeriousGameLib;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Aflevering.GameObjects
{
    public class Finish : GameObject3D
    {

        public Finish()
        {
            Model = Content.Load<Model>(@"Aflevering\Models\Finish");

            Scale = new Vector3(1.4f, 0.6f, 1.6f);

        }
    }
}