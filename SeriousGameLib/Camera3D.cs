using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SeriousGameLib
{
    public delegate void CameraArrivedEventHandler(GameObject3D targetObject, bool reversed);

    public class Camera3D
    {
        public event CameraArrivedEventHandler CameraArrived;

        public Vector3 Position { get; set; }
        public Matrix Projection { get; private set; }

        public bool LockY { get; set; }
        public bool LockRotation { get; set; }
        public float LockRotationMaxDegrees { get; set; }
        public float LockRotationMinDegrees { get; set; }

        private Quaternion _rotation;
        private float _yaw;
        private float _pitch;
        private float _roll;

        public Camera3D(float aspectRatio)
        {
            _rotation = new Quaternion();
            _yaw = 0.0f;
            _pitch = 0.0f;
            _roll = 0.0f;

            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                                                             aspectRatio,
                                                             0.1f,
                                                             10000.0f);

            PlayerControllable = true;
        }

        public void SetPosition(Vector3 newPosition)
        {
            if (!LockY)
            {
                Position = newPosition;
            }
            else
            {
                newPosition.Y = Position.Y;
                Position = newPosition;
            }
        }

        public Vector3 GetMoveToLookAtPosition(Vector3 direction, float amount)
        {
            return Position + Vector3.Transform(direction * amount, Matrix.CreateFromQuaternion(_rotation));
        }

        public void MoveToLookAt(Vector3 direction, float amount)
        {
            if (PlayerControllable)
            {
                if (!LockY)
                {
                    Position += Vector3.Transform(direction * amount, Matrix.CreateFromQuaternion(_rotation));
                }
                else
                {
                    Vector3 newPosition = Position + Vector3.Transform(direction * amount, Matrix.CreateFromQuaternion(_rotation));
                    newPosition.Y = Position.Y;
                    Position = newPosition;
                }
            }

        }

        public Vector3 GetMoveToPosition(Vector3 direction, float amount)
        {
            return Position + direction * amount;
        }

        public void MoveTo(Vector3 direction, float amount)
        {
            if (PlayerControllable)
            Position += direction * amount;
        }

        public Vector3 LookAt
        {
            get
            {
                return Vector3.Transform(Position, (Matrix.CreateFromQuaternion(_rotation)));
            }
        }

        public void LookAtGameObject(GameObject3D v)
        {
            _yaw    = MathHelper.ToRadians(v.MiniGameAngle.X);// x / 100.0f;    // links naar rechts rond kijken
            _pitch  = MathHelper.ToRadians(v.MiniGameAngle.Y);// y / 100.0f;  // up / down kijken
            _roll   = MathHelper.ToRadians(v.MiniGameAngle.Z);//

            Quaternion qYaw   = Quaternion.CreateFromAxisAngle(Vector3.Up, _yaw);
            Quaternion qPitch = Quaternion.CreateFromAxisAngle(Vector3.Right, _pitch);
            Quaternion qRoll  = Quaternion.CreateFromAxisAngle(Vector3.Backward, _roll);

            _rotation = qYaw * qPitch * qRoll;

            Position = v.MiniGamePosition;// new Vector3(v.Position.X, 5, v.Position.Z);
        }

        public Matrix View
        {
            get
            {
                  return Matrix.Invert(Matrix.CreateFromQuaternion(_rotation) * Matrix.CreateTranslation(Position));
            }
        }
        
        public void Rotate(float x, float y)
        {
            if (PlayerControllable)
            {
                _yaw -= x / 100.0f;   // links naar rechts rond kijken
                _pitch -= y / 100.0f; // up / down kijken

                float yawdegrees = MathHelper.ToDegrees(_yaw);
                float pitchdegrees = MathHelper.ToDegrees(_pitch);

                if (LockRotation) 
                {
                    pitchdegrees = MathHelper.Clamp(pitchdegrees, LockRotationMinDegrees, LockRotationMaxDegrees);
                    _pitch = MathHelper.ToRadians(pitchdegrees);
                }

                if (yawdegrees > 360 || yawdegrees < -360) _yaw = MathHelper.ToRadians(yawdegrees % 360);
                if (pitchdegrees > 360 || pitchdegrees < -360) _pitch = MathHelper.ToRadians(pitchdegrees % 360);

                Quaternion qYaw = Quaternion.CreateFromAxisAngle(Vector3.Up, _yaw);
                Quaternion qPitch = Quaternion.CreateFromAxisAngle(Vector3.Right, _pitch);
                Quaternion qRoll = Quaternion.CreateFromAxisAngle(Vector3.Backward, _roll);

                _rotation = qYaw * qPitch * qRoll;
            }
        }

        public bool PlayerControllable { get; set; }
        public void Update(GameTime gameTime)
        {
            if (!PlayerControllable && _movementTimePassed < _totalTime && !_reverseMovement)
            {
                float elapsed = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                _movementTimePassed += elapsed;
                _yaw += _yawStep * elapsed;
                _pitch += _pitchStep * elapsed;
                _roll += _rollStep * elapsed;
                Position += _movementVector * elapsed;

                Quaternion qYaw = Quaternion.CreateFromAxisAngle(Vector3.Up, _yaw);
                Quaternion qPitch = Quaternion.CreateFromAxisAngle(Vector3.Right, _pitch);
                Quaternion qRoll = Quaternion.CreateFromAxisAngle(Vector3.Backward, _roll);

                _rotation = qYaw * qPitch * qRoll;
            }
            else if (!PlayerControllable && _movementTimePassed < _totalTime && _reverseMovement)
            {
                float elapsed = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                _movementTimePassed += elapsed;
                _yaw -= _yawStep * elapsed;
                _pitch -= _pitchStep * elapsed;
                _roll -= _rollStep * elapsed;
                Position -= _movementVector * elapsed;

                Quaternion qYaw = Quaternion.CreateFromAxisAngle(Vector3.Up, _yaw);
                Quaternion qPitch = Quaternion.CreateFromAxisAngle(Vector3.Right, _pitch);
                Quaternion qRoll = Quaternion.CreateFromAxisAngle(Vector3.Backward, _roll);

                _rotation = qYaw * qPitch * qRoll;
            }
            else if (!PlayerControllable && _movementTimePassed >= _totalTime && _reverseMovement)
            {
                PlayerControllable = true;
                if (CameraArrived != null)
                {
                    CameraArrived(_targetObject, true);
                }
            }
            else if (!PlayerControllable && _movementTimePassed >= _totalTime && !_reverseMovement)
            {
                LookAtGameObject(_targetObject);
                PlayerControllable = true;

                if (CameraArrived != null)
                {
                    CameraArrived(_targetObject, false);
                }
            }
        }

        private float _totalTime = 0.0f;
        private float _movementTimePassed = 0.0f;
        GameObject3D _targetObject = null;
        float _yawStep, _pitchStep, _rollStep;
        Vector3 _movementVector = Vector3.Zero;
        public void MoveToGameObject(GameObject3D gameObject, float arriveInMiliseconds)
        {
            PlayerControllable = false;

            _totalTime = arriveInMiliseconds;
            _movementTimePassed = 0.0f;
            _targetObject = gameObject;
            _reverseMovement = false;

            _yawStep = (MathHelper.ToRadians(_targetObject.MiniGameAngle.X) - _yaw) / arriveInMiliseconds;
            _pitchStep = (MathHelper.ToRadians(_targetObject.MiniGameAngle.Y) - _pitch) / arriveInMiliseconds;
            _rollStep = (MathHelper.ToRadians(_targetObject.MiniGameAngle.Z) - _roll) / arriveInMiliseconds;
            _movementVector = Vector3.Divide(_targetObject.MiniGamePosition - Position, arriveInMiliseconds);
        }

        bool _reverseMovement = false;
        public void ReturnFromLastGameObject()
        {
            _reverseMovement = true;
            PlayerControllable = false;
            _movementTimePassed = 0.0f;
        }
    }
}
