using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeriousGameLib;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Menu
{
    public class Buttons : GameObject
    {
        private string[] _buttonNames = 
            // Note: You can change the order here.
            // Note: If you alter the order here, you must also change the array in the NarratorText class. - sorry about the crappy coding - gerjo
            new string[] { "fotograaf", "schilder", "aflevering", "uitnodiging", "ontwerpen", "kantoor" };

        private Texture2D[] _buttonTextures;
        private Texture2D[] _buttonTexturesHover;

        private Rectangle[] _buttonPositions;

        private MouseState _currentMouseState;
        private MouseState _previousMouseState;

        private Viewport _lastViewport;

        public Buttons(GameWorld owner)
            : base(owner)
        {

            _buttonTextures      = new Texture2D[_buttonNames.Length];
            _buttonTexturesHover = new Texture2D[_buttonNames.Length];
            _buttonPositions     = new Rectangle[_buttonNames.Length];

            for (int i = 0; i < _buttonNames.Length; ++i)
            {
                _buttonTextures[i]      = Content.Load<Texture2D>("Menu/Buttons/" + _buttonNames[i]);
                _buttonTexturesHover[i] = Content.Load<Texture2D>("Menu/Buttons/" + _buttonNames[i] + "_hover");
            }
            _currentMouseState = _previousMouseState = Mouse.GetState();

            CalculateBoundingBoxes();
        }

        public override void Update(GameTime gameTime)
        {

        }

        private void CalculateBoundingBoxes()
        {
            _lastViewport = _owner.GraphicsDevice.Viewport;
            Vector2 spacing = new Vector2(360, 130);

            Vector2 offset  = new Vector2(
                _lastViewport.Width / 2 - (_buttonTextures[0].Width + spacing.X) / 2,
                _lastViewport.Height / 2 - (spacing.X) / 2);

            

            for (int i = 0, column = 0; i < _buttonNames.Length; ++i)
            {
                if (i == 3) column++;

                _buttonPositions[i] = new Rectangle((int)(offset.X + spacing.X * column), (int)(offset.Y + spacing.Y * (i - column * 3)), _buttonTextures[i].Width, _buttonTextures[i].Height);
            }
        }

        private void LoadGame(string name)
        {
            switch (name)
            {
                case "aflevering":
                    _owner.LoadAfter = MiniGames.AFLEVERING;
                    break;
                case "fotograaf":
                    _owner.LoadAfter = MiniGames.FOTOGRAAF;
                    break;
                case"ontwerpen":
                    _owner.LoadAfter = MiniGames.ONTWERPEN;
                    break;
                case "schilder":
                    _owner.LoadAfter = MiniGames.SCHILDER;
                    break;
                case "uitnodiging":
                    _owner.LoadAfter = MiniGames.UITNODIGING;
                    break;
                case "kantoor":
                default:
                    // Just an unload will do :)
                    break;
            }
            _owner.ForceUnload = true; 
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            
            _previousMouseState = _currentMouseState;
            _currentMouseState  = Mouse.GetState();

            Point mouse = new Point(_currentMouseState.X, _currentMouseState.Y);

            // Window has been resized, so must recalculate the bounding boxes!
            if (!_lastViewport.Equals(_owner.GraphicsDevice.Viewport)) CalculateBoundingBoxes();

            bool HasHovered = false; // Singleton used since some bounding boxes overlap.
            for (int i = 0; i < _buttonNames.Length; ++i)
            {
                if (_buttonPositions[i].Contains(mouse) && !HasHovered)
                {
                    if (_currentMouseState.LeftButton.Equals(ButtonState.Pressed) && !_previousMouseState.LeftButton.Equals(ButtonState.Pressed))
                        LoadGame(_buttonNames[i]);

                    Narrator.Instance.ShowText(NarratorText.MenuHover[i]);

                    HasHovered = true;
                    spriteBatch.Draw(_buttonTexturesHover[i], _buttonPositions[i], Color.White);
                }
                else spriteBatch.Draw(_buttonTextures[i], _buttonPositions[i], Color.White);
            }
        }
    }
}
