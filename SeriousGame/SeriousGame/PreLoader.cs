using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using System.IO;
using System.Threading;
using Microsoft.Xna.Framework.Graphics;

namespace SeriousGameLib
{
    public class PreLoader
    {

        private Stack<string> _filesToLoad;
        private Object _threadLock = new Object();
        private Texture2D[] _preLoaderText;
        private Game game;

        private const int frameDelay  = 50;
        private int frameDelayCounter = 0;
        private int loadImageIndex = 0;

        public Thread Thread1 { get; private set; }
        //public Thread Thread2 { get; private set; }
        //public Thread Thread3 { get; private set; }

        private float _totalFiles  = 1;
        private float _filesLoaded = 0;

        private SpriteFont _defaultFont;
        private Texture2D _background;

        private bool _forceStop;

        public PreLoader(Game game)
        {
            this.game       = game;
            _filesToLoad    = new Stack<string>(250); // 250 is roughly the figure we want.
            _preLoaderText  = new Texture2D[8];
            Thread1         = new Thread(new ThreadStart(Run));
            //Thread2         = new Thread(new ThreadStart(Run));
            //Thread3         = new Thread(new ThreadStart(Run));

            _defaultFont    = game.Content.Load<SpriteFont>("Preloader/font_preload");
            _background     = game.Content.Load<Texture2D>("Preloader/spashscreen");
            for (int i = 0; i < _preLoaderText.Length; ++i)
                _preLoaderText[i] = game.Content.Load<Texture2D>("Preloader/loader_" + i);

            // Recursively load all folders and file into a collection:
            GetAllFolders(game.Content.RootDirectory);

            _totalFiles = _filesToLoad.Count;
        }

        // Should be called by the thread.
        public void Run()
        {

            while (_filesToLoad.Count > 0 && !_forceStop)
            {
                string file, ext;

                // Seperate lock since I simply do not trust the microsoft documentation.
                lock (_threadLock)
                {
                    file = _filesToLoad.Pop();
                    ext = file.Substring(file.Length - 4);
                }

                if (ext == ".xnb") // Check required incase files are set to "not compile"
                {
                    try
                    {
                        game.Content.Load<object>(file.Substring(0, file.Length - 4).Substring(game.Content.RootDirectory.Length + 1));
                        _filesLoaded++;
                    }
                    catch (Exception) { }
                }
            }
            

        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if((frameDelayCounter += gameTime.ElapsedGameTime.Milliseconds) > frameDelay) {
                frameDelayCounter = 0;
                loadImageIndex = (loadImageIndex + 1) % 8;
            }

            spriteBatch.Draw(_background, new Rectangle(0, 0, game.GraphicsDevice.Viewport.Width, game.GraphicsDevice.Viewport.Height), Color.White);

            // Rotating loader image:
            spriteBatch.Draw(
                    _preLoaderText[loadImageIndex],
                    new Vector2((game.GraphicsDevice.Viewport.Width - _preLoaderText[loadImageIndex].Width) / 2, (game.GraphicsDevice.Viewport.Height - _preLoaderText[loadImageIndex].Height) / 2), 
                    Color.White);

            string text = "Bezig met laden, vooruitgang: " + (int)(100 / _totalFiles * _filesLoaded) + "% klaar!";
            Vector2 textSize = _defaultFont.MeasureString(text);

            // 2 iterations, each offset by i - this causes a "shadow" effect:
            for(int i = 0; i < 2; ++i)
            spriteBatch.DrawString(
                    _defaultFont,
                    text,
                    new Vector2((game.GraphicsDevice.Viewport.Width - textSize.X - i) / 2, (game.GraphicsDevice.Viewport.Height - textSize.Y) / 2 + 100 - i),
                    (i == 1) ? Color.Black:new Color(211, 214, 58));

            
        }

        // Recursive routine to list all files and paths.
        private void GetAllFolders(string path)
        {
            string[] folders = Directory.GetDirectories(path);
            foreach (string folder in folders)
            {
                foreach(string file in Directory.GetFiles(folder)) _filesToLoad.Push(file);
                
                GetAllFolders(folder);
            }
        }

        ~PreLoader()
        {
            _filesToLoad.Clear();
            if(Thread1.IsAlive) Thread1.Abort();
        }
    }
}
