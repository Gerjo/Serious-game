using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeriousGameLib;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Kantoor3D.GameObjects
{
    public class Computerscherm : GameObject3D
    {
        public Computerscherm()
        {
            Model = Content.Load<Model>(@"Kantoor3D\Models\Computerscherm_3DWereld_Ready");
            Position = new Vector3(10.7f, 3.4f, 0.0f);

            BoundingBoxScale = new Vector3(0.6f, 0.55f, 0.9f);
            BoundingBoxOffset = new Vector3(-0.2f, 0.15f, 0f);

            Scale = new Vector3(0.25f, 0.25f, 0.25f);

            RotateY = 180.0f;

            MiniGameType        = MiniGames.MENU;
            MiniGameAngle       = new Vector3(-90, 0, 0); // Look down at the table.
            MiniGamePosition    = new Vector3(Position.X - 2f, Position.Y + .5f, Position.Z); // Hover slightly above the table.
        }
    }
}
