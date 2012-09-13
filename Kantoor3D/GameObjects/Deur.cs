using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeriousGameLib;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Kantoor3D.GameObjects
{
    public class Deur : GameObject3D
    {
        public Deur()
        {
            
            Model = Content.Load<Model>(@"Kantoor3D\Models\deur_completenew");

            BoundingBoxScale    = new Vector3(0.7f, 0.6f, 0.3f);
            BoundingBoxOffset   = new Vector3(-0.1f, 0.35f, 0.0f);

            Scale = new Vector3(0.90f, 0.90f, 0.90f);

            MiniGameType        = MiniGames.AFLEVERING;
            MiniGameAngle       = new Vector3(90, 0, 0);
            MiniGamePosition    = new Vector3(Position.X-8, Position.Y + 2, Position.Z - 1);
        }
    }
}
