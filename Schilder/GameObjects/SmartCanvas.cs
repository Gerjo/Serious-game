using SeriousGameLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;


namespace Schilder
{

    public class SmartCanvas : GameObject
    {

        private SpriteFont _debugFont;
        private Texture2D _canvasWood;
        private Texture2D _thinLine;
        private Texture2D _outcome;
        private Texture2D _colorSample;
        private Texture2D _outline;
        private List<int> _checkpoints;

        private MouseState _currentMouseState;
        private MouseState _previousMouseState;

        private uint[] _drawingPixels;
        private uint[] _drawingColors;

        private List<FloodFill> _floodFillInstances;

        private Rectangle _bigCanvasFrame;
        private int _bigCanvasPadding   = 20;
        private int _smallCanvasPadding = 15;

        private double _currentDrawingPercentage;
        private double _previousDrawingPercentage;


        private Point? _undoLastClick;
        private uint _undoLastColor;

        public SmartCanvas(GameWorld game)
            : base(game)
        {
            DrawingContainer drawingAssets = (game as Schilder).DrawingAssets;

            Position        = new Vector2(400, 100);
            _debugFont      = Content.Load<SpriteFont>("Fotograaf/Fonts/defaultfont");
            _canvasWood     = Content.Load<Texture2D>("Schilder/Images/darkwood512");
            _outline        = Content.Load<Texture2D>(drawingAssets.Outline);
            _thinLine       = Content.Load<Texture2D>(drawingAssets.Thinline);
            _colorSample    = Content.Load<Texture2D>(drawingAssets.Colored);
            _outcome        = new Texture2D(game.GraphicsDevice, _thinLine.Width, _thinLine.Height);

            _bigCanvasFrame = new Rectangle((int)(Position.X - _bigCanvasPadding), (int)(Position.Y - _bigCanvasPadding), _outcome.Width + _bigCanvasPadding * 2, _outcome.Height + _bigCanvasPadding * 2);

            LoadAssetPixels();
            LoadCheckPoints(drawingAssets.CheckPoints);

            _currentMouseState = _previousMouseState = Mouse.GetState();

            _floodFillInstances = new List<FloodFill>();

            // Load the initial score:
            _currentDrawingPercentage = _previousDrawingPercentage = GetPercentageComplete();
        }

        // Implementing our own "clean up" routine allows us to kill any alive threads:
        public override void CleanUp()
        {
            foreach (FloodFill instance in _floodFillInstances)
                instance.Stop();
            
            _floodFillInstances.Clear();
        }

        // Creates the outcome texture, and set the initial pixels:
        private void LoadAssetPixels()
        {
            _drawingPixels = new uint[_thinLine.Height * _thinLine.Width];
            _drawingColors = new uint[_thinLine.Height * _thinLine.Width];
            _thinLine.GetData<uint>(_drawingPixels);
            _colorSample.GetData<uint>(_drawingColors);
        }

        // Search for checkpoints in the checkpoints image:
        private void LoadCheckPoints(string asset)
        {
            _checkpoints = new List<int>();
            Texture2D tex   = Content.Load<Texture2D>(asset);
            uint[] pixels   = new uint[tex.Width * tex.Height];
            tex.GetData<uint>(pixels);

            for (int i = 0; i < pixels.Length; ++i)
            {
                if (pixels[i] == 4278255360) // green checkpoint color
                    _checkpoints.Add(i);
            }
        }

        // Checks if the player has clicked!
        public override void Update(GameTime gameTime)
        {
            _currentMouseState = Mouse.GetState();

            ClickActionHandler(gameTime);
            DeleteDeadThreads(gameTime);

            HandleUndoButton();
            DrawUndoButton();

            _previousMouseState = _currentMouseState;
        }

        // Turns a Color object into it's uint (packed) counterpart.
        private Color UintToColor(uint color)
        {
            byte a = (byte)(color >> 24);
            byte r = (byte)(color >> 16);
            byte g = (byte)(color >> 8);
            byte b = (byte)(color >> 0);
          
            return new Color(r, g, b, a);
        }

        // Takes a point and translate it into an array index position:
        private int PointToIndex(Point p)
        {
            return _outcome.Width * p.Y + p.X;
        }

        // Handle any user drawing actions:
        private void ClickActionHandler(GameTime gameTime)
        {
            if (!_owner.TrophyScreen.Visible)
            {
                if (_currentMouseState.LeftButton.Equals(ButtonState.Pressed) && !_previousMouseState.LeftButton.Equals(ButtonState.Pressed))
                {
                    Point clickLocation = new Point(_currentMouseState.X - (int)Position.X, _currentMouseState.Y - (int)Position.Y);

                    // Make sure we click on the drawing. May have to account for positional offset.
                    if (clickLocation.X < _outcome.Width && clickLocation.Y < _outcome.Height && clickLocation.X >= 0 && clickLocation.Y >= 0)
                    {
                        uint newColor = (_owner as Schilder).ColorSelector.PaintColor.PackedValue;

                        // Check if the target already has the same color:
                        if (_drawingPixels[PointToIndex(clickLocation)] != newColor)
                        {
                            AudioFactory.PlayOnce("paint", false);
                            // Take not of the "undo" parameter:
                            _undoLastClick = clickLocation;
                            _undoLastColor = _drawingPixels[PointToIndex(clickLocation)];

                            SpawnFloodFillInstance(clickLocation, newColor);
                        }
                    }
                }
            }
        }

        private void SpawnFloodFillInstance(Point clickLocation, uint newColor)
        {
            // While the FloodFill algorihtm is self sustained, we still keep track of any instances.
            _floodFillInstances.Add(
                new FloodFill(_drawingPixels, _drawingColors, _outcome.Width, _outcome.Height, clickLocation, newColor)
            );
        }

        // Deletes any dead or long running floodfill instances:
        private void DeleteDeadThreads(GameTime gameTime)
        {
            
            if (!gameTime.IsRunningSlowly)
            {
                DateTime now = DateTime.Now;

                for (int i = 0; i < _floodFillInstances.Count; ++i)
                {
                    if (!_floodFillInstances[i].IsRunning())
                    {
                        _floodFillInstances.RemoveAt(i);
                        if (i > 0) --i;
                    }

                    // Allow 10 seconds, before we assume a timeout and force a stop:
                    else if (now.Ticks - _floodFillInstances[i].StartTime.Ticks > 100000000) // 10000tick = 1ms // 1000 ms = 1 sec
                            _floodFillInstances[i].Stop();
                    
                }
            }
        }

        // Adds/removes the undo button, based on undo availability:
        private void DrawUndoButton() {
            // Show or hide the "undo" button:
            if (_undoLastClick == null || _owner.TrophyScreen.Visible)
            {
                Narrator.Instance.RemoveButton(NarratorText.SchilderButtonUndo);
                
            }
            else if (_undoLastClick != null )
            {
                Narrator.Instance.AddButton(NarratorText.SchilderButtonUndo);
                
            }        
        }

        // Handle any undo actions
        private void HandleUndoButton()
        {
            if (_undoLastClick != null)
            {
                if (Narrator.Instance.IsMouseLeftClick(NarratorText.SchilderButtonUndo))
                {
                    SpawnFloodFillInstance((Point)_undoLastClick, _undoLastColor);
                    _undoLastClick = null;
                }
            }
        }

        // Draw the painting and smaller side canvas:
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Set a "base" position relative to the window size.
            Position = new Vector2(700 - _outcome.Width / 2, _owner.GraphicsDevice.Viewport.Height / 2 - _outcome.Height / 2);
            _bigCanvasFrame.X = (int)(Position.X - _bigCanvasPadding);
            _bigCanvasFrame.Y = (int)(Position.Y - _bigCanvasPadding);

            // Copy our pixels into the texture, then render it.
            _outcome.SetData<uint>(_drawingPixels);

            spriteBatch.Draw(_canvasWood, _bigCanvasFrame, Color.White);
            spriteBatch.Draw(_outcome, Position, Color.White);
            spriteBatch.Draw(_outline, Position, Color.White);

            // Side drawing, the angled painting:
            Rectangle source    = new Rectangle(0, 0, _colorSample.Width, _colorSample.Height);
            float scale         = 0.4f;
            float rotation      = (float)MathHelper.ToRadians(-7f);
            Vector2 origin      = new Vector2(_colorSample.Width / 2, _colorSample.Height / 2);
            Vector2 pos         = Position + new Vector2(-100, 50);
            Rectangle smallDest = new Rectangle((int)pos.X, (int)pos.Y, (int)(_colorSample.Width * scale) + _smallCanvasPadding, (int)(_colorSample.Height * scale) + _smallCanvasPadding);

            spriteBatch.Draw(_canvasWood, smallDest, new Rectangle(0, 0, _canvasWood.Width, _canvasWood.Height), Color.White, rotation, new Vector2(_canvasWood.Width / 2, _canvasWood.Height / 2), SpriteEffects.None, 0);
            spriteBatch.Draw(_colorSample, pos, source, Color.White, rotation, origin, scale, SpriteEffects.None, 0);
            
            _previousDrawingPercentage  = _currentDrawingPercentage;
            _currentDrawingPercentage   = GetPercentageComplete();

            if (_currentDrawingPercentage > _previousDrawingPercentage)
                Narrator.Instance.ShowText(NarratorText.SchilderMotivational, _currentDrawingPercentage);
            else if (_currentDrawingPercentage < _previousDrawingPercentage)
                Narrator.Instance.ShowText(NarratorText.SchilderDemotivational, _currentDrawingPercentage);
            
            // Draws a shadow, as i is used as an offset.
            //for(byte i = 0; i < 2; ++i)
            //    spriteBatch.DrawString(_debugFont, _currentDrawingPercentage + "% accurate! (Anything above 75% is pretty good, above 90% is near impossible)", new Vector2(440 - i, 550 - i), (i == 0) ? Color.Black : Color.White);
        }

        // Calculates the score. This is raw data:
        private double GetDrawingScoreRaw()
        {
            // Calculate the difference between target and current drawing:
            double totalScore = 0;
            foreach (uint p in _checkpoints)
            {
                Color target = UintToColor(_drawingColors[p]);
                Color current = UintToColor(_drawingPixels[p]);

                Vector3 foo = new Vector3(Math.Abs(current.R - target.R), Math.Abs(current.G - target.G), Math.Abs(current.B - target.B));
                totalScore += foo.Z + foo.Y + foo.X;
            }

            return totalScore;
        }

        // Return the current score on a 0% to 100% range, use this to calculate a trophy level:
        public int GetPercentageComplete()
        {
            int maxScore = 255 * 3 * _checkpoints.Count;
            return (int) (100 - (100f / maxScore * GetDrawingScoreRaw()));
        }
    }
}
