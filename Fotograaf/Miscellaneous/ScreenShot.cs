using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using SeriousGameLib;

namespace Fotograaf
{
    public class ScreenShot
    {
        private static Texture2D filmstripTexture;
        private static SpriteFont debugFont; // TODO: possibly removed if we are no longer using this.

        public Texture2D ScreenshotData { get; set; }
        public Rectangle Source { get; set; }
        public Rectangle Target;
        public Vector2 Position { get; set; }
        public Vector2 SamplePosition { get; set; }
        public float ZoomLevel { get; set; }
        public int Width;
        public int Height;
        public bool ShowFilmStrip { get; set; }

        public float PhotoScrore { get; set; }

        public ScreenShot()
        {
            ShowFilmStrip   = false;
            Target          = new Rectangle();
            Source          = new Rectangle();
            Position        = Vector2.Zero;
            SamplePosition  = Vector2.Zero;
            ZoomLevel       = 0.8f; // default zoomlevel.
            ScreenshotData  = null;
            Height          = 100;
            Width           = 130;

            if (filmstripTexture == null) filmstripTexture = GameObject.Content.Load<Texture2D>("Fotograaf/Images/filmstrip");
            if (debugFont == null) debugFont = GameObject.Content.Load<SpriteFont>("Fotograaf/Fonts/defaultfont");
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (ScreenshotData != null)
            {
                Target.Width  = (int)(Width * (1 / ZoomLevel));
                Target.Height = (int)(Height * (1 / ZoomLevel));

                Target.X = (int)(SamplePosition.X - (Target.Width / 2));
                Target.Y = (int)(SamplePosition.Y - (Target.Height / 2));

                Vector2 spawnLocation = new Vector2(Position.X - (Target.Width * ZoomLevel / 2), Position.Y - (Target.Height * ZoomLevel / 2));

                spriteBatch.Draw(ScreenshotData, spawnLocation, Target, Color.White, 0f, Vector2.Zero, ZoomLevel, SpriteEffects.None, 0);

                if (ShowFilmStrip)
                    spriteBatch.Draw(filmstripTexture, new Vector2(Position.X - filmstripTexture.Width / 2, Position.Y - filmstripTexture.Height / 2), Color.White);

                // Always show score: TODO: hide
                //spriteBatch.DrawString(debugFont, "Score: " + PhotoScrore, spawnLocation - new Vector2(1, 1), Color.Black);
                //spriteBatch.DrawString(debugFont, "Score: " + PhotoScrore, spawnLocation, Color.White);
            }
        }
    }
}
