using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using SeriousGameLib;

namespace Ontwerpen
{
    class CardLogic
    {
        private int _columns;
        private int _rows;
        private int _totalCards;
        public int Turns;
        public int X;
        public int Y; // The screen position of the card layout
        public int PositionX;
        public int PositionY; 

        public int Width;
        public int Height;
        
        private Card[,] cards; // The grid array
        public List<Texture2D> CardFaces; // Stores all of the card face textures

        private List<Card> flippedCards; // Stores references to the Cards that are flipped over

        private Random someNum = new Random(); // Used to pull a random card face Id from the list

        private GameTime gameTime;
        private Viewport _viewport;

        private int matches;

        private double now;
        private double unflipDelay; // Time in milliseconds that must elapse before non-matching cards are flipped over
        private double unflipStartTime; // A time marker that's set when a non-match is found

        private MouseState mouseCurrent;
        private MouseState mouseLast;

        private Ontwerpen _owner;

        public CardLogic(Ontwerpen owner, int cols, int rows, Viewport viewport)
        {
            _owner      = owner;
            _columns    = cols;
            _rows       = rows;
            _viewport   = viewport;
            _totalCards = rows * cols;
            X           = ((_viewport.Width / 2) - (104 * 2));
            Y           = ((_viewport.Height / 2) - (104 * 2));

            unflipDelay     = 1000;
            unflipStartTime = 0;
            CardFaces       = new List<Texture2D>();
            flippedCards    = new List<Card>();
            gameTime        = new GameTime();
        }

        public void InitCards()
        {
            PositionX   = 0;
            PositionY   = 0;
            matches     = 0;

            // Create a list of the Card face Ids to draw from
            // elements are stored in matching pairs (1,1,2,2,3,3,etc.)
            List<int> Ids = new List<int>();
            for(int i=0; i<(_totalCards/2); i++)
            {                
                Ids.Add(i);
                Ids.Add(i);
            }
         
            // Create a jagged array grid to hold the Card instances: 
            cards = new Card[_rows, _columns];

            // Loop through the grid and create a new Card instance for each element:
            for (int i = 0; i < _rows; i++)
            {
                for (int j = 0; j < _columns; j++)
                {
                    // Randomly pick a face Id from the list:
                    int randomId = someNum.Next(Ids.Count);
                    // Create the Card instance:
                    cards[i,j] = new Card(i,j, Ids[randomId]);
                    // Remove the face Id from the list to keep other cards from choosing it:
                    Ids.RemoveAt(randomId); 
                }
            }  
        }// end InitCards()

        public void DrawCards(SpriteBatch cardBatch, Texture2D cardBack, Texture2D cardBackHover)
        {
            // Loop through the grid and draw the cards:
            Width  = cardBack.Width;
            Height = cardBack.Height;

            for (int i = 0; i < _rows; i++)
            {
                for (int j = 0; j < _columns; j++)
                {
                    Card theCard = cards[i,j];
                    Texture2D theTexture;
                    if (theCard.IsFlipped) theTexture = CardFaces[theCard.Id];

                    else if (!_owner.AdviceToCancelInput && !_owner.TrophyScreen.Visible && new Rectangle((cardBack.Width * j) + X, ((i % _rows) * cardBack.Height) + Y, cardBack.Width, cardBack.Height).Contains(new Point(Mouse.GetState().X, Mouse.GetState().Y))) theTexture = cardBackHover;

                    else theTexture = cardBack;

                    theCard.Draw(cardBatch, theTexture, (cardBack.Width * j) + X, ((i % _rows) * cardBack.Height) + Y);
                }
            }
        }

        public void Update(MouseState mouseCurrent, MouseState mouseLast, double now)
        {
            this.now = now;
            this.mouseCurrent = mouseCurrent;
            this.mouseLast = mouseLast;
           
            if(!_owner.TrophyScreen.Visible) FlipCard();
            UnflipCards(mouseCurrent, mouseLast);
        }

        public void FlipCard()
        {
            if (!_owner.AdviceToCancelInput && (mouseCurrent.LeftButton == ButtonState.Released) && (mouseLast.LeftButton == ButtonState.Pressed) && 
                (mouseCurrent.X > X) && (mouseCurrent.X < (X + (_columns * Width))) && 
                (mouseCurrent.Y > Y) && (mouseCurrent.Y < (Y + (_rows * Height))))
            {

                for (int i = 0; i < _columns; i++) {
                    if ((X + (Width * i) < mouseCurrent.X) && (mouseCurrent.X < X + (Width * (i + 1))))
                    {
                        PositionX = i;
                    }
                }
                for (int i = 0; i < _rows; i++) { 
                    if((Y + (Height * i) < mouseCurrent.Y) && (mouseCurrent.Y < Y + (Height * (i+1)))){
                        PositionY = i;
                    }
                }

               
                Card theCard = cards[PositionY, PositionX];

                if (theCard.IsFlipped)
                {
                    // The card is already flipped, error sound
                    AudioFactory.PlayOnce("bliep");
                }
                else
                {
                    // The card hasn't been flipped.
                    int totalFlipped = flippedCards.Count;
                    if (totalFlipped < 2 && unflipStartTime == 0)
                    {
                        // We haven't flipped two cards,
                        // and we're not waiting for non-matching cards to unflip
                        // Flip sound effect
                        AudioFactory.PlayOnce("flipSound");
                        // Flip that card!
                        theCard.IsFlipped = true;
                        flippedCards.Add(theCard); // Store this card
                        if (totalFlipped >= 1)
                            checkMatch();
                    }
                }
            }
        }

        public void checkMatch()
        {
            // Compare the face Id's of the two flipped cards:
            if (flippedCards[0].Id == flippedCards[1].Id)
            {
                
                matches++;
                Turns++;
                if (matches >= _totalCards / 2)
                {
                    _owner.WinGame(Turns);
                }
                else
                {
                    flippedCards.Clear();
                    Narrator.Instance.ShowText(NarratorText.OntwerpenMotivational, Turns);
                }
            }
            else
            {
                Turns++;
                unflipStartTime = now;
                Narrator.Instance.ShowText(NarratorText.OntwerpenDemotivational, Turns);
            }
        }

        public void UnflipCards(MouseState mouseCurrent, MouseState mouseLast)
        {
            if (unflipStartTime > 0)
            {
                double elapsed = now - unflipStartTime;
                if ((mouseCurrent.LeftButton == ButtonState.Pressed) && (mouseLast.LeftButton == ButtonState.Released))
                {
                    unflipDelay = 0;
                }
                if (elapsed > unflipDelay)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        flippedCards[i].IsFlipped = false;
                    }
                    flippedCards.Clear();
                    unflipStartTime = 0;
                }
                 
            }
            unflipDelay = 1000;
        }
    }
}
