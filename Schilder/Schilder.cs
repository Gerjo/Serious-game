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

namespace Schilder
{
    public class Schilder : GameWorld
    {
        public SmartCanvas SmartCanvas { get; private set; }
        public ColorSelector ColorSelector { get; private set; }
        public PaintBrush PaintBrush { get; private set; }
        public DrawingContainer DrawingAssets;

        public Schilder(Game game)
            : base(game)
        {
            // Available images, can easily add or remove drawings here.
            string[] names = new string[] { "turtle", "peacock", "bear" };

            // Load a random drawing:
            DrawingAssets   = new DrawingContainer(names[SeriousGameLib.PersistentStorage.LastSchilderDrawingIndex++ % names.Length]);
            
            PaintBrush      = new PaintBrush(this);
            SmartCanvas     = new SmartCanvas(this);
            ColorSelector   = new ColorSelector(this);
            TrophyScreen    = new TrophyScreen(this);

            AudioFactory.AddSoundEffect("schildertheme", "Schilder/Audio/Schilder");
            AudioFactory.AddSoundEffect("paint", "Schilder/Audio/paint");

            AudioFactory.PlayOnce("schildertheme",true);
            

            //AddGameObject(new Wallpaper(this));
            AddGameObject(SmartCanvas);
            AddGameObject(ColorSelector);
            AddGameObject(PaintBrush);

            AddGameObject(TrophyScreen);

            game.IsMouseVisible = false;
        }

        // The "3D Camera" arrived at the mini-game:
        public override void OnCameraArrive()
        {
            Narrator.Instance.ShowText(NarratorText.SchilderWelcomeText);
            
            Narrator.Instance.AddButton(NarratorText.SchilderButtonSend);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Player finishes the game!
            if (Narrator.Instance.IsMouseLeftClick(NarratorText.SchilderButtonSend))
            {
                int score = SmartCanvas.GetPercentageComplete();
                Trophies trophy = GetTrophy(score);

                TrophyScreen.Show(trophy, NarratorText.SchilderWinScreenCaptions[(int)trophy], NarratorText.SchilderWinScreenText[(int)trophy]);
            }
        }

        // Translate the percentage into a trophy
        private Trophies GetTrophy(int score)
        {
            if (score > 85) return Trophies.Gold;
            else if (score > 70) return Trophies.Silver;
            else return Trophies.Bronze;
        }

        // Called when the mini-game is deleted:
        public override void CleanUp()
        {
            SmartCanvas.CleanUp();
        }
    }
}
