using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SeriousGameLib;

namespace Kantoor3D.GameObjects
{
    public class Player : GameObject3D
    {
        public float Width { get; set; }
        public float Height { get; set; }

        public Player()
        {
            Width = 0.1f;
            Height = 4.0f;

            Visible = false;

            Position = new Vector3(10.0f, 0.0f, 0.0f);

            IsPlayerControlled = true;
            CollissionBehaviour = CollissionType.NONE;

            UID = "player";

            UpdatePlayerBoundingBox();
        }

        private void UpdatePlayerBoundingBox()
        {
            float halfWidth = Width / 2.0f;
            float halfHeight = Height / 2.0f;

            CombinedBoundingBox = new BoundingBox(new Vector3(-halfWidth, -Height, -halfWidth),
                                                  new Vector3(halfWidth, Height, halfWidth));
        }
    }
}
