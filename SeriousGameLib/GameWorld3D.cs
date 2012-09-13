using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SeriousGameLib
{
    public delegate void CollissionEventHandler(GameObject3D gameObject);

    public class GameWorld3D : DrawableGameComponent
    {
        public event CollissionEventHandler CollissionEvent;
        public event CollissionEventHandler MouseCollissionHoverEnter;
        public event CollissionEventHandler MouseCollissionHoverExit;
        public event CollissionEventHandler MouseCollissionClick;

        public Camera3D Camera { get; set; }

        public GameObject3D PlayerObject { get; set; }

        public bool DrawWorld { get; set; }
        public TrophyScreen TrophyScreen { get; set; }
        public bool ForceUnload { get; set; }

        //debug mode: draw transformed bounding boxes
        public bool DebugMode { get; set; }
        private BasicEffect _debugEffect;

        

        protected Game _owner;

        private List<GameObject3D> _gameObjects;
        public GameWorld3D(Game game)
            : base(game)
        {
            _owner = game;
            _gameObjects = new List<GameObject3D>();
            Camera = new Camera3D(game.GraphicsDevice.Viewport.AspectRatio);
            Camera.Position = new Vector3(1, 1, 10);

            _debugEffect = new BasicEffect(game.GraphicsDevice);

            DrawWorld = true;

            DebugMode = false;

            ForceUnload = false;
        }

        public IEnumerable<GameObject3D> GetVisibleGameObjects()
        {
            for (int i = 0; i < _gameObjects.Count; ++i)
            {
                if (_gameObjects[i].Visible)
                {
                    yield return _gameObjects[i];
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            HandleCollission();
            HandleMouseCollission();

            base.Update(gameTime);
        }

        private GameObject3D _currentHoveredGameObject;
        private MouseState _previousMouseState;
        private void HandleMouseCollission()
        {
            if (Camera.PlayerControllable)
            {
                MouseState mouseState = Mouse.GetState();
                GameObject3D intersectedObject = GetClosestMouseIntersectedObject(mouseState.X, mouseState.Y);

                if (_currentHoveredGameObject != intersectedObject && intersectedObject != null)
                {
                    if (intersectedObject.FireMouseOverEvent)
                    {
                        if (MouseCollissionHoverEnter != null) MouseCollissionHoverEnter(intersectedObject);
                    }
                }
                else if (_currentHoveredGameObject != intersectedObject && intersectedObject == null)
                {
                    if (_currentHoveredGameObject.FireMouseOverEvent)
                    {
                        if (MouseCollissionHoverExit != null) MouseCollissionHoverExit(_currentHoveredGameObject);
                    }
                }

                if (intersectedObject != null && mouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
                {
                    if (intersectedObject.FireMouseClickEvent)
                    {
                        if (MouseCollissionClick != null) MouseCollissionClick(intersectedObject);
                    }
                }

                _currentHoveredGameObject = intersectedObject;
                _previousMouseState = mouseState;
            }
        }

        private void HandleCollission()
        {
            if (PlayerObject != null && Camera.PlayerControllable)
            {
                foreach (GameObject3D gameObject in GetVisibleGameObjects())
                {
                    if (gameObject.CollissionBehaviour == CollissionType.EVENTFIRE && PlayerObject.Intersects(gameObject))
                    {
                        if (CollissionEvent != null)
                        {
                            CollissionEvent(gameObject);
                        }
                    }
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {

            base.Draw(gameTime);
            if(DrawWorld)
            {
                foreach (GameObject3D gameObject in GetVisibleGameObjects())
                {
                    for (int i = 0; i < gameObject.Model.Meshes.Count; ++i)
                    {
                        for(int e = 0; e < gameObject.Model.Meshes[i].Effects.Count; ++e)
                        {
                            BasicEffect effect = gameObject.Model.Meshes[i].Effects[e] as BasicEffect;
                            Quaternion meshRotation = Quaternion.CreateFromAxisAngle(Vector3.Up, (gameObject.MeshRotationX.ContainsKey(gameObject.Model.Meshes[i].Name) ? float.Parse(gameObject.MeshRotationX[gameObject.Model.Meshes[i].Name].ToString()) : 0.0f)) *
                                                        Quaternion.CreateFromAxisAngle(Vector3.Right, (gameObject.MeshRotationY.ContainsKey(gameObject.Model.Meshes[i].Name) ? float.Parse(gameObject.MeshRotationY[gameObject.Model.Meshes[i].Name].ToString()) : 0.0f)) *
                                                        Quaternion.CreateFromAxisAngle(Vector3.Backward, (gameObject.MeshRotationZ.ContainsKey(gameObject.Model.Meshes[i].Name) ? float.Parse(gameObject.MeshRotationZ[gameObject.Model.Meshes[i].Name].ToString()) : 0.0f));

                            //effect.EnableDefaultLighting();
                            effect.LightingEnabled = true;
                            effect.PreferPerPixelLighting = true;
                            effect.EnableDefaultLighting();
                            //effect.SpecularPower
                            
                            effect.Alpha = gameObject.Alpha;

                            effect.World = Matrix.CreateTranslation(-gameObject.Model.Meshes[i].BoundingSphere.Center) * Matrix.CreateFromQuaternion(meshRotation) * Matrix.CreateTranslation(gameObject.Model.Meshes[i].BoundingSphere.Center) *
                                            gameObject.BoneTransforms[gameObject.Model.Meshes[i].ParentBone.Index] *
                                            Matrix.CreateScale(gameObject.Scale) *
                                            Matrix.CreateRotationY(MathHelper.ToRadians(gameObject.RotateY)) *
                                            Matrix.CreateRotationZ(MathHelper.ToRadians(gameObject.RotateZ)) *
                                            Matrix.CreateTranslation(gameObject.Position);

                            effect.View = Camera.View;
                            effect.Projection = Camera.Projection;
                        }

                        gameObject.Model.Meshes[i].Draw();
                    }

                    if (DebugMode)
                    {
                        _debugEffect.View = Camera.View;
                        _debugEffect.Projection = Camera.Projection;
                        _debugEffect.World = Matrix.Identity;

                        List<Vector3> points = new List<Vector3>(gameObject.TransformedCombinedBoundingBox.GetCorners());

                        VertexPositionColor[] vertices = new VertexPositionColor[points.Count];

                        for (int i = 0; i < points.Count; ++i)
                        {
                            vertices[i] = new VertexPositionColor(points[i], Color.Red);
                        }

                        for(int i = 0; i < _debugEffect.CurrentTechnique.Passes.Count; ++i)
                        {
                            _debugEffect.CurrentTechnique.Passes[i].Apply();
                            GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, vertices, 0, vertices.Length - 1);
                        }
                    }
                }
            }

            
        }

        public void AddGameObject(GameObject3D newObject)
        {
            if (newObject != null)
            {
                _gameObjects.Add(newObject);
            }
            else
            {
                throw new NullReferenceException();
            }
        }

        public bool PositionCollidesWithVisibleObject(Vector3 position, float width, float height)
        {
            foreach (GameObject3D gameObject in GetVisibleGameObjects())
            {
                if (gameObject.CollissionBehaviour == CollissionType.BLOCK && gameObject.PositionIntersects(position, width, height))
                {
                    return true;
                }
                else if (gameObject.CollissionBehaviour == CollissionType.EVENTFIRE && gameObject.PositionIntersects(position, width, height))
                {
                    if (CollissionEvent != null)
                    {
                        CollissionEvent(gameObject);
                    }
                }
            }

            return false;
        }

        public GameObject3D GetClosestMouseIntersectedObject(int mouseX, int mouseY)
        {
            Vector3 nearsource = new Vector3((float)mouseX, (float)mouseY, 0.0f);
            Vector3 farsource = new Vector3((float)mouseX, (float)mouseY, 1.0f);
            Vector3 nearPoint = GraphicsDevice.Viewport.Unproject(nearsource, Camera.Projection, Camera.View, Matrix.Identity);
            Vector3 farPoint = GraphicsDevice.Viewport.Unproject(farsource, Camera.Projection, Camera.View, Matrix.Identity);

            Vector3 rayDirection = Vector3.Normalize(farPoint - nearPoint);
            Ray ray = new Ray(nearPoint, rayDirection);

            GameObject3D rv = null;
            float bestDistance = float.MaxValue;

            foreach (GameObject3D gameObject in GetVisibleGameObjects())
            {
               if (gameObject.FireMouseOverEvent || gameObject.FireMouseClickEvent)
               {
                    float? distance = ray.Intersects(gameObject.TransformedCombinedBoundingBox);
                    if (distance != null)
                    {
                        if (rv == null)
                        {
                            rv = gameObject;
                            bestDistance = (float)distance;
                        }
                        else if (distance < bestDistance)
                        {
                            rv = gameObject;
                            bestDistance = (float)distance;
                        }
                    }
                }
            }

            return rv;
        }

        public BoundingBox? WorldBounds { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <returns>Returns true if position is inside the BoundingBox or if WorldBounds is null</returns>
        public bool PositionInWorldBounds(Vector3 position)
        {
            if (WorldBounds == null) return true;

            Ray ray = new Ray(position, Vector3.Zero);
            return (ray.Intersects((BoundingBox)WorldBounds) != null);
        }

        public void LoadWorld(string worldfile)
        {
            _gameObjects = GameWorld3DLoader.LoadMap(System.Xml.Linq.XDocument.Load(worldfile));
        }

        public GameObject3D GetGameObject3D(string uid)
        {
            return (from o in _gameObjects
                    where o.UID == uid
                    select o).FirstOrDefault();
        }
         
        public void MovePlayer(Vector3 position)
        {
            if(PlayerObject != null && Camera.PlayerControllable)
            {
                Vector3 oldPosition = PlayerObject.Position;
                PlayerObject.Position = position;

                bool collissionBlock = false;
                foreach (GameObject3D gameObject in GetVisibleGameObjects())
                {
                    if (gameObject.CollissionBehaviour == CollissionType.BLOCK && PlayerObject.Intersects(gameObject))
                    {
                        collissionBlock = true;
                    }
                }

                if (!PositionInWorldBounds(PlayerObject.Position)) collissionBlock = true;

                if (collissionBlock)
                {
                    PlayerObject.Position = oldPosition;
                }
            }
        }
    } 
}