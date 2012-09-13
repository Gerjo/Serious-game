using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeriousGameLib;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Aflevering.GameObjects
{
    public class Busje : GameObject3D
    {
        private float x = 0; //- = links, + = rechts
        private float y = 0;
        public float z = 0; //- = voor, + = achter
        public float speed = 0;
        private float rotatie = 0.00f;
        private float rotatiesnelheid = 0f;
        private Physics Physics;

        public float benzine = 100;
        public float benzineInTank = 100; // Some start value, else the lose screen is triggered right away.
        public float lightning;

        public bool speedPower = false;
        public bool hitObstacle = false;
        
        public float eventTime = 1000;
        public float startObs = -100;
        public float startPow = -100;

        public float adjustCamera;

        public Busje()
        {
            Model = Content.Load<Model>(@"Aflevering\Models\BusjeMetJuisteOorsprongWielen");
            //Position = new Vector3(x, y, z);

            BoundingBoxScale = new Vector3(2.5f, 2.5f, 5.5f);
            BoundingBoxOffset = new Vector3(-2.5f, 1.5f, 4.0f);

            Scale = new Vector3(0.20f, 0.20f, 0.20f);

            RotateY = 0.0f;            

            //using physics class to calculate physics for this model
            Physics = new Physics();
            //for (int i = 1; i < 5; i++)
            //{
            //    for (int l = 1; l < 3; l++)
            //    {
            //        Console.WriteLine("<gameobject>");
            //        Console.WriteLine("  <class>ParkBench</class>");
            //        Console.WriteLine("  <uid>parkBench</uid>");
            //        Console.WriteLine("  <position>-13,0.4," + (-240 + i * -20) + "</position>");
            //        Console.WriteLine("  <rotationY>0</rotationY>");
            //        Console.WriteLine("  <rotationZ>0</rotationZ>");
            //        Console.WriteLine("  <collission>none</collission>");
            //        Console.WriteLine("  <firemouseover>false</firemouseover>");
            //        Console.WriteLine("  <firemouseclick>false</firemouseclick>");
            //        Console.WriteLine("</gameobject>");
            //    }
            //}
        }

        public override Vector3 Position
        {
            get
            {
                return base.Position;
            }
            set
            {
                base.Position = value;
                x = Position.X;
                y = Position.Y;
                z = Position.Z;
            }
        }

        public void update(GameTime gametime, KeyboardState keyboardState, Camera3D Camera)
        {                    

            //set distance to object, higher speed means a slightly further off camera
            float CameraDistance = 6.0f + speed * 15;

            //calculates x and z distance to the object and places the camera directly behind the model
            Vector3 CameraRelativePosition = new Vector3(CameraDistance * (float)Math.Sin(MathHelper.ToRadians(RotateY + adjustCamera)), 2.7f, CameraDistance * (float)Math.Cos(MathHelper.ToRadians(RotateY + adjustCamera)));
            Camera.Position = Position + CameraRelativePosition;
           
            //calculate speed 
            speed = Physics.update(speed, keyboardState, gametime);

            //if on obstacle has been hit, decrease speed
            speed = onObstacleHit(speed, gametime);

            //check if powerup is available and use boost
            speed = useSpeedPower(speed, gametime);

            //calculate rotation of object and camera 
            RotateY = Physics.Rotate(speed, keyboardState, Camera);

            //rotate front wheels horizontally
            steerFrontWheels(keyboardState);

            //rotationspeed of the four wheels
            rotatiesnelheid = speed * 0.5f;
            //set rotation of specific mesh
            SetMeshRotationZ("voorwiellinks", rotatie += rotatiesnelheid);
            SetMeshRotationZ("voorwielrechts", rotatie);
            SetMeshRotationZ("achterwiellinks", rotatie + 180.0f);
            SetMeshRotationZ("achterwielrechts", rotatie + 180.0f);

            benzineInTank = benzine + z;
            if(benzineInTank < 0)
            {                
                if (speed > 0)
                {
                    speed -= 0.005f;
                }
                else 
                {
                    speed = 0;
                }
            }

            //x,z position relative to rotation
            z -= speed * (float)Math.Cos(MathHelper.ToRadians(RotateY));
            x -= speed * (float)Math.Sin(MathHelper.ToRadians(RotateY));
            
            //roadbounds
            if (x > 5) 
            {
                x = 5;
                speed *= 0.999f;
            }else if( x < -5)
            {
                x = -5;
                speed *= 0.999f;
            }

            if (speed > 0)
            {
                AudioFactory.PlayOnceChangePitch("speed2", speed * 0.5f);
            }
            
            //updates the position with the new x and z values
            Position = new Vector3(x, y, z);
            
        }

        public float onObstacleHit(float Speed, GameTime gametime)
        {
            if (hitObstacle)
            {
                startObs = (float)gametime.TotalGameTime.TotalMilliseconds;
                hitObstacle = false;
            }
            float now = (float)gametime.TotalGameTime.TotalMilliseconds;
            if (now - startObs < eventTime)
            {
               
                Speed *= 0.99f;
            }
            return Speed;
        }

        public float useSpeedPower (float Speed, GameTime gametime) 
        {            
            if (speedPower)
            {
                startPow = (float)gametime.TotalGameTime.TotalMilliseconds;
                speedPower = false;
            }
            float now = (float)gametime.TotalGameTime.TotalMilliseconds;
            if (now - startPow < eventTime) 
            {
                Speed *= 1.02f;
            }
            return Speed;
        }

        public void steerFrontWheels(KeyboardState keyboardState) 
        {
            //reset wheels when not steering
            SetMeshRotationX("voorwiellinks", 0.0f);
            SetMeshRotationX("voorwielrechts", 0.0f);

            //steering left, rotate front wheels slightly to the left
            if (keyboardState.IsKeyDown(Keys.Left))
            {
                SetMeshRotationX("voorwiellinks", 0.4f);
                SetMeshRotationX("voorwielrechts", 0.4f);
            }
            //steering right, rotate front wheels slightly to the right
            if (keyboardState.IsKeyDown(Keys.Right))
            {
                SetMeshRotationX("voorwiellinks", -0.4f);
                SetMeshRotationX("voorwielrechts", -0.4f);
            }   
        }

    }
}
