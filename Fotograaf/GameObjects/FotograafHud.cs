using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeriousGameLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Fotograaf
{
    public class FotograafHud : GameObject
    {
        public ScreenShot[] ScreenShots { get; private set; }
        public int NumScreenshots {  get; private set; }


        public FotograafHud(GameWorld owner)
            : base(owner)
        {
            Visible         = false;
            NumScreenshots  = 0;
            ScreenShots    = new ScreenShot[5];
        }

        public void AddScreenshot(ScreenShot screenshot)
        {
            if (NumScreenshots < 5)
            {
                ScreenShots[NumScreenshots] = screenshot;
                NumScreenshots++;
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            for (int i = 0; i < NumScreenshots; ++i )
            {
                ScreenShots[i].Position        = new Vector2(200 + i * 140, _owner.GraphicsDevice.Viewport.Height - 105);
                ScreenShots[i].ShowFilmStrip   = true;
                ScreenShots[i].Draw(spriteBatch);
            }
        }

        public override void Update(GameTime gameTime)
        {

        }
    }
}
