using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Fotograaf
{
    [Serializable]
    public class Waypoint
    {
        public enum AnimModes { Walking, Jumping };
        public AnimModes AnimMode {get; set;}
        public Vector2 Position { get; set; }
        public bool IsBoundToMouse { get; set; }

        public Waypoint()
            : this(0, 0, AnimModes.Walking)
        {
        }

        public Waypoint(int spawnX, int spawnY)
            : this(spawnX, spawnY, AnimModes.Walking)
        {
        }

        public Waypoint(int spawnX, int spawnY, AnimModes animMode) {
            this.Position  = new Vector2(spawnX, spawnY);
            this.AnimMode  = animMode;
            IsBoundToMouse = false;
        }

        public override string ToString()
        {
            return "[Waypoint (x:" + Position.X + " y:" + Position.Y +  " animMode:" + AnimMode + ")]";
        }
    }
}
