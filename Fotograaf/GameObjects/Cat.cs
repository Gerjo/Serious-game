using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeriousGameLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Fotograaf
{
    public enum WalkingDirections { LEFT, RIGHT };
    
    public class Cat : GameObject
    {
        // Settings, free to edit these:
        private int _animationspeed      = 60;   // Higher number = slower.
        private float _spriteScale       = 0.5f; // Higher number = bigger image.
        private int _stepSizeSprite      = 230;  // Width of each single frame
        private int _jumpFrameIndex      = 6;    // This frame index is shown during the jump animation.
        private Vector2 _walkingVelocity = new Vector2(0.3f, 0.3f); // maximum speed per x time.
        private Vector2 _jumpingVelocity = new Vector2(0.5f, 0.5f); // maximum speed per x time.

        private int _stepTimer;
        private Rectangle _viewPortSprite;
        public  List<Waypoint> Waypoints;
        private int _walkToIndex;
        private Vector2 _lastSpeed;

        public Vector2 CatHead { get { return RealPosition + new Vector2((WalkingDirection == WalkingDirections.RIGHT) ? 30:-30, -10); } }

        public WalkingDirections WalkingDirection { 
            get {
                return (GetHorizontalMirror() == SpriteEffects.FlipHorizontally) ? WalkingDirections.LEFT : WalkingDirections.RIGHT;
            } 
        }

        public Waypoint.AnimModes AnimationMode
        {
            get {
                return Waypoints[_walkToIndex].AnimMode;
            }
        }

        public  Vector2 RealPosition { get { return Position - (_owner as Fotograaf).Wallpaper.ScrollOffset; } }

        public Cat(GameWorld owner)
            : base(owner)
        {
            Texture         = Content.Load<Texture2D>("Fotograaf/Images/catspritesheet");
            _viewPortSprite = new Rectangle(0, 0, _stepSizeSprite, Texture.Height);
            _stepTimer      = 0;
            _walkToIndex    = 1; // NB.: we start with 1, as 0 is the spawn location.

            LoadWaypoints();

            Position        = Waypoints[0].Position; // Spawn on the first waypoint.
        }

        // NB.: First waypoint is assumed as spawn location, too.
        public void LoadWaypoints()
        {
            if(Waypoints == null) Waypoints = new List<Waypoint>();

            // TODO: load from XML
            Waypoints.Add(new Waypoint(200,     220,    Waypoint.AnimModes.Jumping));
            Waypoints.Add(new Waypoint(500,     220,    Waypoint.AnimModes.Walking));
            Waypoints.Add(new Waypoint(300,      50,    Waypoint.AnimModes.Jumping));
        }
        
        public override void Update(GameTime gameTime)
        {
            // No waynodes available.
            if (Waypoints.Count() == 0) return;

            // This is more or less a potential (yet common!) null pointer prevention:
            if (Waypoints.Count() < 2 || Waypoints.Count() <= _walkToIndex) _walkToIndex = 0;

            // We have arrived at the destination, move towards the next waypoint:
            if (Vector2.DistanceSquared(Position, Waypoints[_walkToIndex].Position) < 20)
                if (Waypoints.Count() <= ++_walkToIndex) _walkToIndex = 0;

            DoAnimation(gameTime);
            DoMovement(gameTime);
        }

        private void DoAnimation(GameTime gameTime)
        {
            if (Waypoints[_walkToIndex].AnimMode == Waypoint.AnimModes.Walking)
            {
                if ((_stepTimer += gameTime.ElapsedGameTime.Milliseconds) > _animationspeed)
                {
                    if ((_viewPortSprite.X += _stepSizeSprite) >= Texture.Width) _viewPortSprite.X = 0;
                    _stepTimer = 0;
                }
            }
            else
            {
                _viewPortSprite.X = _stepSizeSprite * _jumpFrameIndex;
            }
        }

        private void DoMovement(GameTime gameTime)
        {
            Vector2 beforePosition  = Position;
            Vector2 direction       = Waypoints[_walkToIndex].Position - Position;
            direction.Normalize();

            if(AnimationMode == Waypoint.AnimModes.Walking)
                direction *= (_walkingVelocity * gameTime.ElapsedGameTime.Milliseconds);
            else
                direction *= (_jumpingVelocity * gameTime.ElapsedGameTime.Milliseconds);
            
            // Move the object!
            Position    += direction;

            // We moved over the target, so must resolve any access X or Y position:
            if ((beforePosition.X > Waypoints[_walkToIndex].Position.X && Position.X < Waypoints[_walkToIndex].Position.X) ||
                (beforePosition.X < Waypoints[_walkToIndex].Position.X && Position.X > Waypoints[_walkToIndex].Position.X))
                Position = new Vector2(Waypoints[_walkToIndex].Position.X, Position.Y);
            

            if ((beforePosition.Y > Waypoints[_walkToIndex].Position.Y && Position.Y < Waypoints[_walkToIndex].Position.Y) ||
                (beforePosition.Y < Waypoints[_walkToIndex].Position.Y && Position.Y > Waypoints[_walkToIndex].Position.Y))
                Position = new Vector2(Position.X, Waypoints[_walkToIndex].Position.Y);

            // Last speed is just to determine to walking direction in the Draw routine.
            _lastSpeed = Position - beforePosition;
        }

        public override void Draw(GameTime gameTime, Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {

            spriteBatch.Draw(Texture, RealPosition - new Vector2(_stepSizeSprite / 4, Texture.Height / 4), _viewPortSprite, Color.White, 0f, Vector2.Zero, _spriteScale, GetHorizontalMirror(), 0f);
            //Tools.DrawPixel(spriteBatch, RealPosition);
        }

        public SpriteEffects GetHorizontalMirror()
        {
            return (_lastSpeed.X < 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        }

        public void AddWaypoint(Vector2 waypoint) {
            Waypoints.Add(new Waypoint((int)waypoint.X, (int)waypoint.Y, Waypoint.AnimModes.Jumping));
        }
    }
}
