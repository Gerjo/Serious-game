using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeriousGameLib;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Aflevering.GameObjects
{
    public class Lantaarnpaal : GameObject3D
    {

        public Lantaarnpaal()
        {
            Model = Content.Load<Model>(@"Aflevering\Models\Lantaarnpaal");

            Scale = new Vector3(0.5f, 0.5f, 0.5f);

            RotateY = 0.0f;
        }
    }
}