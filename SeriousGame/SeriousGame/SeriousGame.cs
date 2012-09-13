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
using System.Configuration;
using SeriousGameLib;
using System.Threading;

namespace SeriousGameLib
{
    public class SeriousGame : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager _graphics;
        private GameWorld3D _gameWorld3D;
        private PreLoader _preloader;
        private SpriteBatch _spriteBatch;

        public SeriousGame()
        {
            Window.Title = "Essenza Media Game";
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreparingDeviceSettings += new EventHandler<PreparingDeviceSettingsEventArgs>(_graphics_PreparingDeviceSettings);

            _graphics.PreferredBackBufferWidth  = int.Parse(ConfigurationManager.AppSettings["Width"]);
            _graphics.PreferredBackBufferHeight = int.Parse(ConfigurationManager.AppSettings["Height"]);

            _graphics.PreferMultiSampling = true;

            Content.RootDirectory = "Content";
            SeriousGameLib.GameObject.Content   = Content;
            SeriousGameLib.GameObject3D.Content = Content;
            SeriousGameLib.GameWorld.Content    = Content;
            SeriousGameLib.AudioFactory.Content = Content;

            _graphics.SynchronizeWithVerticalRetrace = bool.Parse(ConfigurationManager.AppSettings["RestrictFrameRate"]);
            AudioFactory.IsMuted = Boolean.Parse(ConfigurationManager.AppSettings["MuteSounds"]);

            Window.AllowUserResizing = true;

            IsMouseVisible = true;

//release should run full screen
#if !DEBUG
            _graphics.IsFullScreen = false;
            //_graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            //_graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
#endif
        }

        void _graphics_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            PresentationParameters pp = e.GraphicsDeviceInformation.PresentationParameters;
            pp.RenderTargetUsage = RenderTargetUsage.PreserveContents;
            e.GraphicsDeviceInformation.PresentationParameters = pp;
        }

        protected override void Initialize()
        {

            // Bypass, no preloader - just Aflevering.
            if (ConfigurationManager.AppSettings["CurrentMiniGame"] == "Aflevering")
            {
                _gameWorld3D = new Aflevering.Aflevering(this);
                Components.Add(_gameWorld3D);

            }
            else
            {   
#if DEBUG 
                _gameWorld3D = new Kantoor3D.Kantoor3D(this);
                Components.Add(_gameWorld3D);
#else 
                // Load the preloader stuff:
                _spriteBatch = new SpriteBatch(GraphicsDevice);
                _preloader   = new PreLoader(this);
                _preloader.Thread1.Start();
                //_preloader.Thread2.Start();
                //_preloader.Thread3.Start();
#endif
            }

            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_gameWorld3D == null && !_preloader.Thread1.IsAlive)
            {
                _gameWorld3D = new Kantoor3D.Kantoor3D(this);
                Components.Add(_gameWorld3D);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // If the Kantoor3D isn't loaded yet, run the animated preloader:
            if (_gameWorld3D == null)
            {
                _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                _preloader.Draw(gameTime, _spriteBatch);
                _spriteBatch.End();
            }

            base.Draw(gameTime);
        }

        ~SeriousGame()
        {
            if (_preloader.Thread1.IsAlive) _preloader.Thread1.Abort();
        }
    }
}
