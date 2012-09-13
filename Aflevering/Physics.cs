using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Aflevering.GameObjects;
using SeriousGameLib;

namespace Aflevering
{
    class Physics
    { 
        float acceleration = 0.12f;  
        float deceleration = 0.02f;
        float friction;
        float rotation = 0.0f;

        public float update(float speed, KeyboardState keyboardState, GameTime gametime)
        {
                //maxixum speed is reached when friction equals the added amount of speed    
                friction = speed * speed * 0.02f;

                if (keyboardState.IsKeyDown(Keys.Up))
                {
                    //speed is increased by the acceleration
                    speed += acceleration * (float)gametime.ElapsedGameTime.TotalSeconds;
                }
                if (keyboardState.IsKeyDown(Keys.Down))
                {
                    if (speed > 0)
                    {   
                        //speed is decreased by the deceleration, the brakes
                        speed -= deceleration * (float)gametime.ElapsedGameTime.TotalSeconds;
                    }
                }
                if (speed > 0)
                {
                    if (keyboardState.IsKeyDown(Keys.Left))
                    {
                        //losing speed while steering
                        speed -= 0.003f * (float)gametime.ElapsedGameTime.TotalSeconds;
                    }
                    if (keyboardState.IsKeyDown(Keys.Right))
                    {
                        //losing speed while steering
                        speed -= 0.003f * (float)gametime.ElapsedGameTime.TotalSeconds;
                    }
                }
                //finally, friction is deducted from the speed and the speed is returned
                speed -= friction;
                return speed;    
        }

        public float Rotate(float speed, KeyboardState keyboardState, Camera3D Camera) 
        {
            //we cannot turn while not moving
            if (speed > 0)
            {
                if (keyboardState.IsKeyDown(Keys.Left))
                {
                    rotation += 0.96f;
                    //camera rotation 630.0f equals 360 degrees of rotation
                    Camera.Rotate(-1.68f, 0.0f);
                }
                if (keyboardState.IsKeyDown(Keys.Right))
                {
                    rotation -= 0.96f;
                    //camera rotation 630.0f equals 360 degrees of rotation
                    Camera.Rotate(1.68f, 0.0f);
                }
            }
            return rotation;
        }
    }

}
