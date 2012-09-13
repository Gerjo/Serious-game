using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeriousGameLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Aflevering
{
    public class RaceHud
    {
        private GameWorld3D _owner;

        private Texture2D _gauges;
        private Texture2D _fuelDial;
        private Texture2D _speedDial;

        private float _fuelDialAngle  = 0;
        private float _speedDialAngle = 0;

        public RaceHud(GameWorld3D owner)
        {
            _owner      = owner;
            _gauges     = _owner.Game.Content.Load<Texture2D>("Aflevering/Images/fuel_gauge");
            _fuelDial   = _owner.Game.Content.Load<Texture2D>("Aflevering/Images/fuel_dial");
            _speedDial  = _owner.Game.Content.Load<Texture2D>("Aflevering/Images/speed_dial");
        }

        public void Update(GameTime gameTime)
        {
            
        }

        public void SetFuelPercentage(float fuelAmount)
        {
            _fuelDialAngle = 3.5f / 100.0f * fuelAmount - 1.75f;
        }
        public void SetSpeedPercentage(float speedAmount)
        {
            _speedDialAngle = 4.0f / 100.0f * speedAmount - 2.0f;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch) 
        {
            //_fuelDialAngle += 0.1f;
            //_speedDialAngle += 0.14f;

            int offset = 25;

            spriteBatch.Draw(_gauges, new Vector2(offset, _owner.Game.GraphicsDevice.Viewport.Height - _gauges.Height), Color.White);

            spriteBatch.Draw(_fuelDial, new Vector2(offset + 109, _owner.Game.GraphicsDevice.Viewport.Height - _fuelDial.Height + 109), new Rectangle(0, 0, _fuelDial.Width, _fuelDial.Height), Color.White, _fuelDialAngle, new Vector2(109, 109), 1, SpriteEffects.None, 0);

            spriteBatch.Draw(_speedDial, new Vector2(offset + 294, _owner.Game.GraphicsDevice.Viewport.Height - _speedDial.Height + 117), new Rectangle(0, 0, _speedDial.Width, _speedDial.Height), Color.White, _speedDialAngle, new Vector2(294, 117), 1, SpriteEffects.None, 0);
        
        }
    }
}
