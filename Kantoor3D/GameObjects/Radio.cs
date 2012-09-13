using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeriousGameLib;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Kantoor3D.GameObjects
{
    public class Radio : GameObject3D
    {
        public Radio()
        {
            Model = Content.Load<Model>(@"Kantoor3D\Models\radio");

            BoundingBoxScale = new Vector3(0.7f, 0.6f, 0.3f);
            BoundingBoxOffset = new Vector3(-0.1f, 0.35f, 0.0f);

            Scale = new Vector3(0.30f, 0.30f, 0.30f);

            RotateY = 0.0f;

            /*
              <gameobject>
    <class>Radio</class>
    <uid>radio</uid>
    <position>7.5,2.8,9.5</position>
    <rotationY>220</rotationY>
    <rotationZ>0</rotationZ>
    <collission>block</collission>
    <firemouseover>true</firemouseover>
    <firemouseclick>true</firemouseclick>
  </gameobject>*/
        }
    }
}
