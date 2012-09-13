using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SeriousGameLib
{
    public class Narrator
    {
        // Not too gracefull.
        public static Narrator Instance;

        // Change to increase/decrease the letter delay.
        private int _delayBetweenLetter   = 5;
        private int _spacingBetweenButton = 3;

        private double _startCount      = 0;
        private string _newText         = "";
        private string _bufferText      = "";
        private bool _isRestarting;

        private SpriteFont _defaultFont;
        private Texture2D _narratorDolphin;
        private GraphicsDevice _graphicsDevice;

        private Texture2D _balloonTop;
        private Texture2D _balloonBottom;
        private Texture2D _balloonLeft;
        private Texture2D _balloonRight;

        private Texture2D _balloonTopLeft;
        private Texture2D _balloonTopRight;
        private Texture2D _balloonBottomLeft;
        private Texture2D _balloonBottomRight;

        private Texture2D _balloonArrow;
        private Texture2D _balloonCenter;

        private Vector2 _position;
        private Color _transparant;

        private MouseState _currentMouseState;
        private MouseState _previousMouseState;

        private int _size           = 30; // Width and height of the rounded corners.
        private Vector2 _padding    = new Vector2(20, 5); // Textballoon padding (left/right and top/bottom)
        private bool _isVisible     = false;

        private bool _useTypeWriterAnim = true;

        public bool HideDolphin { get; set; }

        private Dictionary<string, Button> _buttons;

        Model narratorModel;

        public bool HadHover { get; set; }

        public Narrator(GraphicsDevice graphicsDevice)
        {
            PlacementOffset = Vector2.Zero;

            narratorModel = SeriousGameLib.GameObject.Content.Load<Model>("Hud/narrator");

            _transparant         = Color.White;
            _position            = new Vector2();
            _graphicsDevice      = graphicsDevice;
            _defaultFont         = SeriousGameLib.GameObject.Content.Load<SpriteFont>("Hud/Fonts/NarratorFont");
            _narratorDolphin     = SeriousGameLib.GameObject.Content.Load<Texture2D>("Hud/dolphin");
            _defaultFont.LineSpacing = 25; // So why cant I do this via XML?

            _balloonTop          = SeriousGameLib.GameObject.Content.Load<Texture2D>("Hud/Balloon/BalloonTop");
            _balloonBottom       = SeriousGameLib.GameObject.Content.Load<Texture2D>("Hud/Balloon/BalloonBottom");
            _balloonLeft         = SeriousGameLib.GameObject.Content.Load<Texture2D>("Hud/Balloon/BalloonLeft");
            _balloonRight        = SeriousGameLib.GameObject.Content.Load<Texture2D>("Hud/Balloon/BalloonRight");
            _balloonTopLeft      = SeriousGameLib.GameObject.Content.Load<Texture2D>("Hud/Balloon/BalloonTopLeft");
            _balloonTopRight     = SeriousGameLib.GameObject.Content.Load<Texture2D>("Hud/Balloon/BalloonTopRight");
            _balloonBottomLeft   = SeriousGameLib.GameObject.Content.Load<Texture2D>("Hud/Balloon/BalloonBottomLeft");
            _balloonBottomRight  = SeriousGameLib.GameObject.Content.Load<Texture2D>("Hud/Balloon/BalloonBottomRight");
            _balloonCenter       = SeriousGameLib.GameObject.Content.Load<Texture2D>("Hud/Balloon/BalloonCenter");
            _balloonArrow        = SeriousGameLib.GameObject.Content.Load<Texture2D>("Hud/Balloon/BalloonArrow");

            Instance             = this;

            _currentMouseState = _previousMouseState = Mouse.GetState();

            _buttons = new Dictionary<string, Button>();

            Hide();
        }

        public void AddButton(string label)
        {
            if (!HasButton(label)) _buttons.Add(label, new Button(label));   
        }

        public void AddButton(string[] labels)
        {
            foreach (string label in labels) AddButton(label);
        }

        public void RemoveButton(string label)
        {
            if (HasButton(label)) _buttons.Remove(label);
        }

        public void RemoveAllButtons()
        {
            _buttons.Clear();
        }

        public string[] GetAllLabels()
        {
            string[] keys = new string[_buttons.Keys.Count];
            _buttons.Keys.CopyTo(keys, 0);
            return keys;
        }

        public bool HasButton(string label)
        {
            return _buttons.ContainsKey(label);
        }

        public bool IsMouseLeftClick(string label)
        {
            return (IsMouseHover(label) && 
                    _currentMouseState.LeftButton.Equals(ButtonState.Pressed) &&
                    !_previousMouseState.LeftButton.Equals(ButtonState.Pressed));
        }

        public bool IsMouseHover(string label)
        {
            return (HasButton(label) && _buttons[label].Contains(_currentMouseState));
        }

        public void ShowTextNoDelay(string[] newTexts, params object[] vargs)
        {
            ShowText(newTexts, vargs);
            _useTypeWriterAnim = false;
        }

        public void ShowTextNoDelay(string newText, params object[] vargs)
        {
            ShowText(newText, vargs);
            _useTypeWriterAnim = false;
        }

        public void ShowText(string[] newTexts, params object[] vargs)
        {
            ShowText(ArrayRand(newTexts), vargs);
        }

        public void ShowText(string newText, params object[] vargs)
        {
            _useTypeWriterAnim = true;
            newText = String.Format(newText, vargs);

            if (_bufferText != newText) 
            {
                _isRestarting   = true; // Setting this bool will reset the internal timer.
                _newText        = newText;
                _bufferText     = newText;
            }
            Show();
        }

        public void Hide()
        {
            _isVisible      = false;
            //_transparant.A  = 0;
            _bufferText     = "";
        }

        public void Show()
        {
            _isVisible = true;
            //_transparant.A = 255;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (HideDolphin) return;

            // Test is textbox is hidden:
            //if (_transparant.A <= 0)
            if (_isVisible && _newText != "")
            {
                Vector2 dimensions  = _defaultFont.MeasureString(_newText);
                _position = new Vector2(125, (_graphicsDevice.Viewport.Height - 65) - dimensions.Y) + PlacementOffset;

                DrawBalloon(spriteBatch, (int)(dimensions.X + _padding.X * 2), (int)(dimensions.Y + _padding.Y * 2));
                DrawText(spriteBatch, gameTime);
            }

            // Draws the dolphin:
            DrawNarrator(spriteBatch, gameTime);

            DrawButtons(spriteBatch, gameTime);
        }

        private void DrawText(SpriteBatch spriteBatch, GameTime gameTime)
        {
            // We're starting on a new buffer, so must reset the "start time":
            if (_isRestarting)
            {
                _startCount = gameTime.TotalGameTime.TotalMilliseconds;
                _isRestarting = false;
            }

            // Calculate the number of letters, could implement some sort of singleton here:
            int numLetters = (int)Math.Floor((gameTime.TotalGameTime.TotalMilliseconds - _startCount) / _delayBetweenLetter);

            // Can't render more text than is available:
            if (!_useTypeWriterAnim || numLetters > _newText.Length)
            {
                numLetters = _newText.Length;
                //_isRestarting = true;
            }

            // If the next character is a space - then proceed to show that too.
            else if (_newText.Length > (numLetters + 2) && _newText.Substring(numLetters, 1) == " ")
            {
                _startCount -= _delayBetweenLetter;
                numLetters++;
            }

            // Draw the buffer:
            spriteBatch.DrawString(_defaultFont, _newText.Substring(0, numLetters), _position + _padding, Color.Black);
        }

        public static Vector2 PlacementOffset { get; set; }
        private void DrawNarrator(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Vector2 position = new Vector2(15, _graphicsDevice.Viewport.Height - _narratorDolphin.Height - 15);// +PlacementOffset;
            spriteBatch.Draw(_narratorDolphin, position, Color.White);
        }

        private void DrawBalloon(SpriteBatch spriteBatch, int width, int height)
        {
            Vector2 topRight    = new Vector2(width - _size, 0);
            Vector2 bottomleft  = new Vector2(0, height - _size);

            // Draw the corners:
            spriteBatch.Draw(_balloonTopLeft, _position, _transparant);
            spriteBatch.Draw(_balloonTopRight, _position + topRight, _transparant);
            spriteBatch.Draw(_balloonBottomLeft, _position + bottomleft, _transparant);
            spriteBatch.Draw(_balloonBottomRight, _position + topRight + bottomleft, _transparant);

            Rectangle top    = new Rectangle((int)_position.X + _size, (int)_position.Y, width - _size * 2, _size);
            Rectangle left   = new Rectangle((int)_position.X, (int)_position.Y + _size, _size, height - _size * 2);
            Rectangle bottom = new Rectangle((int)_position.X + _size, (int)_position.Y + height - _size, width - _size * 2, _size);
            Rectangle right  = new Rectangle((int)_position.X + width - _size, (int)_position.Y + _size, _size, height - _size * 2);

            spriteBatch.Draw(_balloonTop, top, _transparant);
            spriteBatch.Draw(_balloonLeft, left, _transparant);
            spriteBatch.Draw(_balloonBottom, bottom, _transparant);
            spriteBatch.Draw(_balloonRight, right, _transparant);

            Rectangle center = new Rectangle((int)_position.X + _size, (int)_position.Y + _size, width - _size * 2, height - _size * 2);
            spriteBatch.Draw(_balloonCenter, center, _transparant);

            Rectangle arrow = new Rectangle((int)_position.X + 20, (int)_position.Y + height - 22, _balloonArrow.Width, _balloonArrow.Height);
            spriteBatch.Draw(_balloonArrow, arrow, _transparant);

        }

        private void DrawButtons(SpriteBatch spriteBatch, GameTime gameTime)
        {
            int posX = 120;
            bool HadHover = false;
            Point mouse = new Point(_currentMouseState.X, _currentMouseState.Y);
            foreach (KeyValuePair<String, Button> container in _buttons)
            {
                container.Value.Draw(spriteBatch, new Vector2(_position.X + posX, _graphicsDevice.Viewport.Height - 60), mouse);

                if (container.Value.HadHover) HadHover = true;

                posX += container.Value.Width + _spacingBetweenButton;
            }

           
        }

        public void Update(GameTime gameTime)
        {
             _previousMouseState = _currentMouseState;
             _currentMouseState = Mouse.GetState();

            if (_isVisible && _transparant.A < 255)
            {
                //_transparant.A++;
            }
            else if (!_isVisible && _transparant.A > 0) 
            {
                //_transparant.A--;
            }
        }

        // Return a random string from a collection of strings.
        private string ArrayRand(string[] data)
        {
            return data[new Random().Next(data.Length)];
        }
    }
}
