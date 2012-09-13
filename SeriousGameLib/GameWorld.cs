using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace SeriousGameLib
{
    public abstract class GameWorld
    {
        //public SpriteBatch SpriteBatch { get; protected set; }
        public static ContentManager Content;
        public Game Game { get; private set; }
        public GraphicsDevice GraphicsDevice { get; set; }
        public TrophyScreen TrophyScreen { get; protected set; }

        public bool AdviceToCancelInput { get; set; } // When hud items are hovered, this is set to true.

        // Setting this to "true" will cause the Kantoor3D to unload said mini-game.
        public bool ForceUnload { get; set; }

        public MiniGames LoadAfter { get; set; }

        public GameWorld(Game game)
        {
            LoadAfter       = MiniGames.NONE;
            this.Game       = game;
            GraphicsDevice  = game.GraphicsDevice;
            _gameObjects    = new HashSet<GameObject>();
        }

        private HashSet<GameObject> _gameObjects;

        public IEnumerable<GameObject> GameObjects
        {
            get
            {
                return _gameObjects;
            }
        }

        public GameObject GetGameObjectByName(string name)
        {
            var rv = (from o in _gameObjects
                      where o.Name == name
                      select o).Single();

            return rv;
        }

        public IEnumerable<GameObject> GetVisibleGameObjects()
        {
            foreach (GameObject gameObject in _gameObjects)
            {
                if (gameObject.Visible)
                {
                    yield return gameObject;
                }
            }
        }

        public IEnumerable<GameObject> GetHiddenGameObjects()
        {
            foreach (GameObject gameObject in _gameObjects)
            {
                if (!gameObject.Visible)
                {
                    yield return gameObject;
                }
            }
        }

        public void AddGameObject(GameObject newObject)
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

        public void RemoveGameObject(GameObject gameObject)
        {
            _gameObjects.Remove(gameObject);
        }

        public virtual void Update(GameTime gameTime)
        {
            foreach (GameObject gameObject in GameObjects)
            {
                gameObject.Update(gameTime);
            }
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (GameObject gameObject in GetVisibleGameObjects())
                gameObject.Draw(gameTime, spriteBatch);
        }

        public virtual void CleanUp() { }
        public abstract void OnCameraArrive();
    }
}
