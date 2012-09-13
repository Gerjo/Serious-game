using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Fotograaf
{
    public static class Input
    {
        private static MouseState currentMouseState;
        private static MouseState previousMouseState;
        private static KeyboardState currentKeyboardState;
        private static KeyboardState previousKeyboardState;

        public static bool IsTakePhotoButtonPressed()
        {
            if (currentMouseState.LeftButton == previousMouseState.LeftButton) return false;
            return currentMouseState.LeftButton.Equals(ButtonState.Pressed);
        }

        public static bool IsZoomIn()
        {
            int scroll = currentMouseState.ScrollWheelValue - previousMouseState.ScrollWheelValue;
            if (scroll < 0) return true;
           
            //if(previousKeyboardState.IsKeyDown(Keys.LeftShift)) return false;
            return currentKeyboardState.IsKeyDown(Keys.Down) || currentKeyboardState.IsKeyDown(Keys.Right); ;
        }

        public static bool IsZoomOut()
        {
            int scroll = currentMouseState.ScrollWheelValue - previousMouseState.ScrollWheelValue;
            if (scroll > 0) return true;
            //if(previousKeyboardState.IsKeyDown(Keys.LeftControl)) return false;
            return currentKeyboardState.IsKeyDown(Keys.Up) || currentKeyboardState.IsKeyDown(Keys.Left);
        }

        public static bool IsAddWayPoint()
        {
            return IsTakePhotoButtonPressed();
        }

        public static void Update(GameTime gameTime)
        {
            previousMouseState    = currentMouseState;
            currentMouseState     = Mouse.GetState();
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState  = Keyboard.GetState();
        }

        public static Vector2 GetMouseLocation()
        {
            return new Vector2(currentMouseState.X, currentMouseState.Y);
        }

        public static bool IsMouseMoving()
        {
            return (previousMouseState.X != currentMouseState.X || previousMouseState.Y != currentMouseState.Y);
        }

        public static bool IsKeyPressed(Keys key)
        {
            
            if (previousKeyboardState.IsKeyDown(key)) return false;
            return currentKeyboardState.IsKeyDown(key);
        }

        public static bool IsLeftMouseButtonDown()
        {
            return (currentMouseState.LeftButton.Equals(ButtonState.Pressed));
        }
    }
}
