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

namespace Uitnodiging
{
    public class Uitnodiging : GameWorld
    {
        public PuzzleImage PuzzleImage { get; set; }

        SpriteFont scoreBoard;
        DateTime StartTime;
        TimeSpan levelTime;

        Grid Grid;
        SpawnCards SpawnCards;
        Piece[] Piece;

        MouseState mouseCurrent;
        MouseState mouseLast;

        public int match;

        public int numberOfPieces = 25;
        private bool moving = false;

        public Uitnodiging(Game game)
            : base(game)
        {
            Piece = new Piece[numberOfPieces];
            for (int i = 0; i < numberOfPieces; i++)
            {
                Piece[i] = new Piece(this, i, i + 2000, 2000);
                AddGameObject(Piece[i]);
            }

            Grid = new Grid();
            PuzzleImage = new PuzzleImage(this);
            SpawnCards = new SpawnCards();
            AddGameObject(PuzzleImage);
            mouseCurrent = Mouse.GetState();
            mouseLast = Mouse.GetState();
            SpawnCards.piece = Piece;
            SpawnCards.initSpawns();
          
            scoreBoard = Game.Content.Load<SpriteFont>("Uitnodiging/Fonts/timerFont");
            StartTime = DateTime.Now;

            game.IsMouseVisible = true;

            AudioFactory.AddSoundEffect("sheeptheme", "Uitnodiging/Audio/sheep");
            AudioFactory.AddSoundEffect("card", "Uitnodiging/Audio/card");
            AudioFactory.PlayOnce("sheeptheme", true);
            
            TrophyScreen = new TrophyScreen(this);
            AddGameObject(TrophyScreen);
        }

        public override void OnCameraArrive() 
        {
            //Narrator.PlacementOffset = new Vector2(675f, -50f);
            Narrator.Instance.ShowText(NarratorText.UitnodigingWelcomeText);
        }

        private void DrawText()
        {
            Narrator.Instance.ShowTextNoDelay(NarratorText.UitnodigingTimer, GetTimeRemaining());
        }

        private string GetTimeRemaining()
        {

            int seconden = (int)levelTime.TotalSeconds % 60;
            int minuten = (int)levelTime.TotalMinutes;

            string output0 = "";
            string output1 = "";
            string output2 = "";

            if (minuten == 1) output1 += "1 minuut";
            else if (minuten > 1) output1 += minuten + " minuten";

            if (seconden == 1) output2 += "1 seconde";
            else if (seconden >= 1) output2 += seconden + " seconden";

            if (minuten != 0)
            {
                if (seconden != 0) output0 = output1 + " en " + output2;
                else output0 = output1;
            }
            else output0 = output2;

            return output0;
        }

        private Trophies GetTrophy(int timeSeconds)
        {
            if(timeSeconds < 60 * 2) return Trophies.Gold;
            else if (timeSeconds < 60 * 3.5) return Trophies.Silver;
            else return Trophies.Bronze;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //base.Draw(gameTime, spriteBatch);
            PuzzleImage.Draw(gameTime, spriteBatch);

            DrawText();
            for (int i = 24; i >= 0; i--)
            {
                Piece[i].Draw(gameTime, spriteBatch);
            }
            for (int i = 24; i >= 0; i--)
            {
                if (Piece[i].selected) 
                    Piece[i].Draw(gameTime, spriteBatch);
            }

            if (TrophyScreen.Visible) TrophyScreen.Draw(gameTime, spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            
            mouseCurrent = Mouse.GetState();
            match = 0;

            for (int i = 0; i < numberOfPieces; i++)
            {
                if (moving != true && mouseCurrent.LeftButton == ButtonState.Pressed && !Piece[i].selected && Piece[i].Locate(mouseCurrent))
                {
                    Piece[i].selected = true;
                    moving = true;
                }
                else if ((Piece[i].selected == true) && (mouseCurrent.LeftButton == ButtonState.Released))
                {                    
                    Piece[i].selected = false;
                    if (Grid.checkGrid(mouseCurrent, Piece[i])) 
                    {
                        AudioFactory.PlayOnce("card", false);
                        SpawnCards.spawnNext();
                    }   
                    moving = false;
                }
                if (Grid.checkSolved(Piece[i]))
                {
                    match++;
                }
                if (Piece[i].selected)
                {
                    while (mouseCurrent.LeftButton == ButtonState.Pressed)
                    {
                        Piece[i].Move(mouseCurrent);
                        
                        break;
                    }
                }
            }

            if (match != numberOfPieces)
            {
                levelTime = DateTime.Now - StartTime;
            } 

            // Won the game!
            else if(!TrophyScreen.Visible) 
            {
                Trophies trophy = GetTrophy((int)levelTime.TotalSeconds);
                TrophyScreen.Show(trophy, NarratorText.UitnodigingWinScreenCaptions[(int)trophy], NarratorText.UitnodigingWinScreenText[(int)trophy]);
            }

            mouseLast = mouseCurrent;
            base.Update(gameTime);
        }
    }
}
