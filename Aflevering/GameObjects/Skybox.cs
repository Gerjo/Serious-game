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
    public class Skybox : GameObject3D
    {
        public Skybox()
        {
            Model = Content.Load<Model>(@"Aflevering\Skybox\skybox2");
            Position = new Vector3(0.0f, 0.0f, 0.0f);

            BoundingBoxScale = new Vector3(0.7f, 0.6f, 0.3f);
            BoundingBoxOffset = new Vector3(-0.1f, 0.35f, 0.0f);

            Scale = new Vector3(5.0f, 5.0f, 5.0f);

            RotateY = 270.0f;

        }

        public void update(Vector3 position) 
        {
            Position = position;
        }

    }
}
