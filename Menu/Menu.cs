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

namespace Menu
{
    public class Menu : GameWorld
    {
        public Menu(Game game) : base(game)
        {
            AddGameObject(new Wallpaper(this));
            AddGameObject(new Buttons(this));
            
            AudioFactory.AddSoundEffect("menutheme", "Menu/Audio/menu");
            AudioFactory.PlayOnce("menutheme", true);
        }

        public override void OnCameraArrive()
        {
            
        }
    }
}
