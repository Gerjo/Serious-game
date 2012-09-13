using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeriousGameLib;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Aflevering.GameObjects
{
    public class ObstacleRocks : GameObject3D
    {
        public ObstacleRocks()
        {
            Model = Content.Load<Model>(@"Aflevering\Models\Obstacle_Rocks");
            Position = new Vector3(0.0f, 0.0f, 0.0f);

            BoundingBoxScale = new Vector3(1.4f, 1.4f, 1.4f);
            BoundingBoxOffset = new Vector3(0.0f, 0.0f, 0.0f);

            Scale = new Vector3(0.8f, 0.8f, 0.8f);

            RotateY = 0.0f;
        }

        

    }
}
