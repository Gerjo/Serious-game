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
using SeriousGameLib;
using Fotograaf.GameObjects;

namespace Fotograaf
{
    public class Fotograaf : GameWorld
    {
        public Flash Flash { get; private set; }
        public Camera Camera { get; private set; }
        public Cat Cat { get; private set; }
        public FotograafHud Hud { get; private set; }
        public Wallpaper Wallpaper { get; private set; }
        public MapEditor MapEditor { get; private set; }

        private Texture2D _screenshotTexture;
        private RenderTarget2D[] _renderBuffers;

        private byte _bufferIndex;
        private byte _previousBufferIndex;

        public Fotograaf(Game game) : base(game)
        {
            Flash        = new Flash(this);
            Camera       = new Camera(this);
            Cat          = new Cat(this);
            Wallpaper    = new Wallpaper(this);
            MapEditor    = new MapEditor(this);
            Hud          = new FotograafHud(this);
            TrophyScreen = new TrophyScreen(this);

            AddGameObject(Wallpaper);
            //AddGameObject(new FPSCounter(this));
            AddGameObject(Cat);
            AddGameObject(Flash);
            AddGameObject(Camera);
            AddGameObject(MapEditor);
            AddGameObject(Hud);
            AddGameObject(TrophyScreen);

            _bufferIndex   = 0;
            _renderBuffers = new RenderTarget2D[10];

            AudioFactory.AddSoundEffect("photoClick", "Fotograaf/Songs/camera-click4");
            AudioFactory.AddSoundEffect("zoom", "Fotograaf/Songs/camera-focus");
            AudioFactory.AddSoundEffect("fotograaftheme", "Fotograaf/Songs/background");

            AudioFactory.PlayOnce("fotograaftheme", true);

            game.IsMouseVisible = false;

        }

        public override void OnCameraArrive() {
            if(!MapEditor.IsEnabled) Narrator.Instance.ShowText(NarratorText.FotograafWelcomeText);
        }

        // Draw the whole lot, also sets the render target:
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            _previousBufferIndex = _bufferIndex;

            if (++_bufferIndex >= _renderBuffers.Length) _bufferIndex = 0;
            
            // Recreate the buffer if it's null or the window has been resized.
            if (_renderBuffers[_bufferIndex] == null || GraphicsDevice.Viewport.Width != _renderBuffers[_bufferIndex].Width || GraphicsDevice.Viewport.Height != _renderBuffers[_bufferIndex].Height)
                _renderBuffers[_bufferIndex] = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            // Set our custom render target, the entire game, during this frame, will be rendered here.
            GraphicsDevice.SetRenderTarget(_renderBuffers[_bufferIndex]);

            bool isTrophyVisible = TrophyScreen.Visible;
            TrophyScreen.Visible = false;
            base.Draw(gameTime, spriteBatch);
            TrophyScreen.Visible = isTrophyVisible;

            // Store a snapshot (screenshot), this is used when the player takes a picture of the animal. 
            _screenshotTexture = _renderBuffers[_bufferIndex];
            spriteBatch.End();

            // Restore the default render target.
            GraphicsDevice.SetRenderTarget(null);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone); // SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone

            // Draw the "texture" actually onscreen. The double buffer double buffered.
            spriteBatch.Draw(_renderBuffers[_bufferIndex], Vector2.Zero, Color.White);
            
            // These items are drawn separately so they wont appear on the "screenshot"
            Camera.Draw(gameTime, spriteBatch);
            Flash.Draw(gameTime, spriteBatch);

            if (TrophyScreen.Visible) TrophyScreen.Draw(gameTime, spriteBatch);

            Hud.Draw(gameTime, spriteBatch);
            //spriteBatch.End();
        }

        // Propegate the Update event to any gameobjects:
        public override void Update(GameTime gameTime)
        {

            if (Hud.NumScreenshots == 4 && !TrophyScreen.Visible) FinishGame();

            if (Hud.NumScreenshots > 0) Narrator.Instance.Hide();

            Input.Update(gameTime);
            base.Update(gameTime);
        }

        private void FinishGame()
        {
            Trophies trophy;
            int totalScore = 0;
            for (int i = 0; i < 4; ++i)
            {
                totalScore += (int)Hud.ScreenShots[i].PhotoScrore;
            }

            if (totalScore > 250) trophy = Trophies.Gold;
            else if (totalScore > 180) trophy = Trophies.Silver;
            else trophy = Trophies.Bronze;

            TrophyScreen.Show(trophy, NarratorText.FotograafWinScreenCaptions[(int)trophy], NarratorText.FotograafWinScreenText[(int)trophy]);
        }

        // Returns a texture containing a screenshot. Using this is VERY memory intentsive.
        public Texture2D GetScreenShotStill()
        {
            RenderTarget2D buff = _renderBuffers[_previousBufferIndex];

            _renderBuffers[_previousBufferIndex] = null;

            return buff;
        }

        // Returns a texture reference to the current screen. USE WITH CAUTION!
        public Texture2D GetScreenShotStream()
        {

            return _renderBuffers[_previousBufferIndex];
        }

        // Scrolloffset based on the deadzone and stuff:
        public Vector2 GetScrollOffset()
        {
            return Wallpaper.ScrollOffset;
        }
    }
}
