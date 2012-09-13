using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeriousGameLib;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Aflevering.GameObjects
{
    public class ObstacleSewer : GameObject3D
    {
        public ObstacleSewer()
        {
            Model = Content.Load<Model>(@"Aflevering\Models\Obstacle_Sewer");

            BoundingBoxScale = new Vector3(1.0f, 1.0f, 1.0f);
            BoundingBoxOffset = new Vector3(0.0f, 0.0f, 0.0f);

            Scale = new Vector3(0.7f, 0.7f, 0.7f);
        }
    }
}
