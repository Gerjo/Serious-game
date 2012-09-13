using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace SeriousGameLib
{
    public abstract class GameObject
    {
        public Vector2 Position { get; set; }
        public Texture2D Texture { get; set; }
        public bool Visible { get; set; }
        public string Name { get; set; }
       
        public static ContentManager Content;
        
        protected GameWorld _owner;

        public GameObject() {}

        public GameObject(GameWorld owner, bool visible = true)
        {
            Visible = visible;
            _owner = owner;
        }

        public bool CollidesWith(GameObject other)
        {
            if (!CanCheckCollissionWith(other)) return false;

            return GetBounds().Intersects(other.GetBounds());
        }

        public bool Contains(GameObject other)
        {
            if (!CanCheckCollissionWith(other)) return false;

            return GetBounds().Contains(other.GetBounds());
        }
        
        public Rectangle GetBounds()
        {
            return new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);
        }

        private bool CanCheckCollissionWith(GameObject other)
        {
            return Texture != null && other.Texture != null;
        }

        public abstract void Update(GameTime gameTime);
        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
        public virtual void CleanUp() { }
    }
}
