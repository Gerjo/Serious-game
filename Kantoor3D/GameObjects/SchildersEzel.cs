using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeriousGameLib;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Kantoor3D.GameObjects
{
    public class SchildersEzel : GameObject3D
    {
        public SchildersEzel()
        {
            Model = Content.Load<Model>(@"Kantoor3D\Models\Schildersezel_WithoutSecretFactoryHack");
            Position = new Vector3(0.0f, 2.75f, -10.0f);

            BoundingBoxScale = new Vector3(0.4f, 0.85f, 0.4f);
            BoundingBoxOffset = new Vector3(-0.1f, 0.2f, 0.0f);

            Scale = new Vector3(0.40f, 0.40f, 0.40f);

            RotateY = 90.0f;

            MiniGameType        = MiniGames.SCHILDER;
            MiniGamePosition    = new Vector3(Position.X + 0.8f, Position.Y + 1f, Position.Z+6);
            MiniGameAngle       = new Vector3(0, 0, 0);
        }
    }
}
