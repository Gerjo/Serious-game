
#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using SeriousGameLib;
#endregion

namespace Ontwerpen
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Ontwerpen : GameWorld
    {

        SpriteFont scoreBoard;

        private Texture2D _cardBack;
        private Texture2D _cardBackhover;

        MouseState mouseCurrent;
        MouseState mouseLast;

        CardLogic cardLogic;
        Viewport viewport;

        int columns = 4;
        int rows    = 4;


        public Ontwerpen(Game game) : base(game)
        {
            viewport    = game.GraphicsDevice.Viewport;
            cardLogic   = new CardLogic(this, columns, rows, viewport);

            game.IsMouseVisible = true;
            LoadContent();

            Initialize();

            TrophyScreen = new TrophyScreen(this);
            AddGameObject(TrophyScreen);
        }

        public override void OnCameraArrive() {
            Narrator.Instance.ShowText(NarratorText.OntwerpenWelcomeText);
        }

        public void Initialize()
        {
            
            mouseCurrent    = Mouse.GetState();
            mouseLast       = Mouse.GetState();

            cardLogic.InitCards();
            AudioFactory.PlayOnce("ontwerpentheme", true);
            
            //AudioFactory.PlayOnce("songMenu", true);
        }

        public void WinGame(int turns)
        {
            Trophies trophy;
            if (turns < 15) trophy = Trophies.Gold;
            else if (turns < 20) trophy = Trophies.Silver;
            else trophy = Trophies.Bronze;

            TrophyScreen.Show(trophy, NarratorText.OntwerpenWinScreenCaptions[(int)trophy], NarratorText.OntwerpenWinScreenText[(int)trophy]);
        }

        protected void LoadContent()
        {
            int[,] cells = new int[3, 3];

            for (int i = 0; i < 8; i++)
            {
                cardLogic.CardFaces.Add(Game.Content.Load<Texture2D>("Ontwerpen/AnimalCards/card" + i));
            }
            _cardBack       = Game.Content.Load<Texture2D>("Ontwerpen/AnimalCards/cardBack");
            _cardBackhover  = Game.Content.Load<Texture2D>("Ontwerpen/AnimalCards/cardBackHover"); 

            AudioFactory.AddSoundEffect("flipSound", "Ontwerpen/Audio/flip");
            AudioFactory.AddSoundEffect("bliep",     "Ontwerpen/Audio/bliep01");
            AudioFactory.AddSoundEffect("ontwerpentheme",  "Ontwerpen/Audio/memory");
        }

        public override void Update(GameTime gameTime)
        {
            mouseCurrent = Mouse.GetState();
            cardLogic.Update(mouseCurrent, mouseLast, (double)gameTime.TotalGameTime.TotalMilliseconds);
            mouseLast = mouseCurrent;
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            cardLogic.DrawCards(spriteBatch, _cardBack, _cardBackhover);
            base.Draw(gameTime, spriteBatch);
        }
    }
}