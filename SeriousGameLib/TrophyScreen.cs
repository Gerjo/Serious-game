using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace SeriousGameLib
{

    public enum Trophies { Gold = 0, Silver = 1, Bronze = 2 };
    
    public class TrophyScreen : GameObject
    {
        private Texture2D _backgroundConfirm;
        private Texture2D _backgroundInactive;
        private Texture2D[] _backgroundWinScreen;
        private Rectangle _monitorViewport;
        private Vector2? _winScreenPos;
        private Vector2? _buttonPos;
        private string[] _buttonMemory;

        private Button _buttonGotoOffice;

        private MouseState _currentMouseState;
        private MouseState _previousMouseState;

        private Trophies _currentTrophy;

        private SpriteFont _captionFont;
        private SpriteFont _bodyTextFont;

        private string _captionText;
        private string _bodyText;

        private bool _isReturnConfirmState;

        private Button _yesReturn;
        private Button _noReturn;

        private bool _mouseStateOnReturn = true;

        private Game _game;
        private GraphicsDevice _graphicsDevice;

        private GameWorld3D _gameWorld3D;

        public TrophyScreen(GameWorld3D game)
        {
            _game           = game.Game;
            _graphicsDevice = game.GraphicsDevice;
            _gameWorld3D    = game;
            Initialize();
        }

        public TrophyScreen(GameWorld game)
            : base(game, false)
        {
            _game           = game.Game;
            _graphicsDevice = game.GraphicsDevice;

            Initialize();
        }

        private void Initialize()
        {
            _backgroundInactive = Content.Load<Texture2D>("Hud/inactive_background");
            _buttonGotoOffice = new Button(NarratorText.ButtonReturnToOffice);
            _monitorViewport = new Rectangle();
            _backgroundWinScreen = new Texture2D[3];
            _backgroundWinScreen[(int)Trophies.Gold] = Content.Load<Texture2D>("Hud/Winscreens/winscreen_gold");
            _backgroundWinScreen[(int)Trophies.Silver] = Content.Load<Texture2D>("Hud/Winscreens/winscreen_silver");
            _backgroundWinScreen[(int)Trophies.Bronze] = Content.Load<Texture2D>("Hud/Winscreens/winscreen_bronze");

            _captionFont = Content.Load<SpriteFont>("Hud/Fonts/WinCaptionFont");
            _bodyTextFont = Content.Load<SpriteFont>("Hud/Fonts/WinBodyFont");

            _currentMouseState = _previousMouseState = Mouse.GetState();
            _bodyTextFont.LineSpacing = 25; // So why cant I do this via XML?

            _backgroundConfirm = Content.Load<Texture2D>("Hud/confirm_background");

            _previousMouseState = _currentMouseState = Mouse.GetState();

            // In case update is called before draw:
            if (_yesReturn == null) _yesReturn = new Button(NarratorText.ButtonConfirmGotoOfficeYes);
            if (_noReturn == null) _noReturn = new Button(NarratorText.ButtonConfirmGotoOfficeNo);
        }


        public void ShowReturnToOfficeConfirm()
        {
            _mouseStateOnReturn = _game.IsMouseVisible;
            _game.IsMouseVisible = true;

            _buttonMemory = Narrator.Instance.GetAllLabels();
            Narrator.Instance.RemoveAllButtons();
            Narrator.Instance.Hide();
            Visible = true;

            _isReturnConfirmState = true;
        }

        public void HideReturnToOfficeConfirm()
        {
            Hide();

            _isReturnConfirmState = false;
        }

        // Show this window, and hides any narrator stuff and Issue the trophy.
        public void Show(Trophies currentTrophy, string captionText, string bodyText)
        {
            _mouseStateOnReturn = _game.IsMouseVisible;
            _game.IsMouseVisible = true;

            PersistentStorage.Trophies[(int)currentTrophy]++;

            _currentTrophy = currentTrophy;
            _buttonMemory  = Narrator.Instance.GetAllLabels();
            Visible        = true;
            _captionText   = captionText;
            _bodyText      = bodyText;

            Narrator.Instance.RemoveAllButtons();
            Narrator.Instance.Hide();

            _isReturnConfirmState = false;
            
        }

        // Hide this window, and restore any narrator stuff.
        public void Hide()
        {
            _game.IsMouseVisible = _mouseStateOnReturn;
            Visible = false;

            Narrator.Instance.RemoveAllButtons();
            Narrator.Instance.AddButton(_buttonMemory);
            Narrator.Instance.Show();
        }

        public override void Update(GameTime gameTime)
        {
            _previousMouseState = _currentMouseState;
            _currentMouseState = Mouse.GetState();

            if (!Visible) return;

            if (_currentMouseState.LeftButton.Equals(ButtonState.Pressed) && !_previousMouseState.LeftButton.Equals(ButtonState.Pressed))
            {
                if (_owner != null)
                {
                    if (_isReturnConfirmState && _yesReturn != null && _noReturn != null)
                    {
                        if (_yesReturn.Contains(_currentMouseState)) _owner.ForceUnload = true;
                        else if (_noReturn.Contains(_currentMouseState)) HideReturnToOfficeConfirm();
                    }
                }
                else
                {
                    if (_yesReturn.Contains(_currentMouseState)) _gameWorld3D.ForceUnload = true;

                    else if (_noReturn.Contains(_currentMouseState)) HideReturnToOfficeConfirm();
                }

                if (_buttonGotoOffice.Contains(_currentMouseState))
                {
                    if (_owner != null)
                    {
                        _owner.ForceUnload = true;
                    }
                    else
                    {
                        _gameWorld3D.ForceUnload = true;
                    }
                }
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //  Mark variables due for a resize:
            if (_monitorViewport.Height != _graphicsDevice.Viewport.Height || _monitorViewport.Width != _graphicsDevice.Viewport.Width)
            {
                _monitorViewport.Height = _graphicsDevice.Viewport.Height;
                _monitorViewport.Width  = _graphicsDevice.Viewport.Width;
                _winScreenPos           = _buttonPos = null;
            }

            DrawBackground(spriteBatch);

            if (!_isReturnConfirmState)
            {
                DrawWinScreen(spriteBatch);
                DrawButtons(spriteBatch);
                DrawText(spriteBatch);
            }

            // One call draws all!
            else DrawReturnConfirmation(spriteBatch);
        }

        // Draws the inactive grey overlay:
        private void DrawBackground(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_backgroundInactive, _monitorViewport, Color.White);
        }

        // The return to "kantoor" button:
        private void DrawButtons(SpriteBatch spriteBatch)
        {
            if (_buttonPos == null)
                _buttonPos = new Vector2(_monitorViewport.Width / 2 - _buttonGotoOffice.Width / 2, _monitorViewport.Height / 2 + 50);

            _buttonGotoOffice.Draw(spriteBatch, (Vector2)_buttonPos, _currentMouseState);
        }

        // Background on which the button and text sits ontop.
        private void DrawWinScreen(SpriteBatch spriteBatch)
        {
            if(_winScreenPos == null)
                _winScreenPos = new Vector2(_monitorViewport.Width / 2 - _backgroundWinScreen[0].Width / 2, _monitorViewport.Height / 2 - _backgroundWinScreen[0].Height / 2);

            spriteBatch.Draw(_backgroundWinScreen[(int)_currentTrophy], (Vector2)_winScreenPos, Color.White);
        }

        // Draws the caption and body text of the win popupbox
        private void DrawText(SpriteBatch spriteBatch) 
        {
            spriteBatch.DrawString(_captionFont, _captionText, new Vector2(190, 20) + (Vector2)_winScreenPos, Color.Black);
            spriteBatch.DrawString(_bodyTextFont, _bodyText, new Vector2(190, 70) + (Vector2)_winScreenPos, Color.Black);
        }

        // Draw and handle anything related to the "are you sure you want to go to kantoor3D".
        private void DrawReturnConfirmation(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_backgroundConfirm, new Vector2((_graphicsDevice.Viewport.Width - _backgroundConfirm.Width) / 2, (_graphicsDevice.Viewport.Height - _backgroundConfirm.Height) / 2), Color.White);
            
            Vector2 textSize        = _bodyTextFont.MeasureString(NarratorText.ConfirmGotoOfficeText);
            Vector2 textLocation    = new Vector2((_graphicsDevice.Viewport.Width - textSize.X) / 2, (_graphicsDevice.Viewport.Height/2 - 65));

            spriteBatch.DrawString(_bodyTextFont, NarratorText.ConfirmGotoOfficeText, textLocation, Color.Black);

            if (_yesReturn == null) _yesReturn = new Button(NarratorText.ButtonConfirmGotoOfficeYes);
            if (_noReturn == null) _noReturn   = new Button(NarratorText.ButtonConfirmGotoOfficeNo);

            _yesReturn.Draw(spriteBatch, new Vector2(_graphicsDevice.Viewport.Width / 2 - 0, (_graphicsDevice.Viewport.Height / 2 + 5)), Mouse.GetState());
            _noReturn.Draw(spriteBatch, new Vector2(_graphicsDevice.Viewport.Width / 2 - _noReturn.Width - 0, (_graphicsDevice.Viewport.Height / 2 + 5)), Mouse.GetState());
        }
    }
}
