using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeriousGameLib;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Aflevering.GameObjects
{
    public class Muur : GameObject3D
    {

        public Muur()
        {
            Model = Content.Load<Model>(@"Aflevering\Models\muur");

            Scale = new Vector3(0.4f, 0.4f, 0.4f);

        }
    }
}