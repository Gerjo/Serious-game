using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeriousGameLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;
using System.Xml;


namespace Fotograaf.GameObjects
{
    // TODO: link to XML file.
    public class MapEditor : GameObject
    {
        public static string SaveFileName = "Waypoints.xml";

        private List<Waypoint> _coords;

        private bool _isVisible;

        public bool IsEnabled = false;

        public MapEditor(Fotograaf game)
            : base(game)
        {
            _isVisible = false;

            try
            {
                _coords = LoadWayPoints();
            }
            
            catch (Exception) // Note: the missing variable is not a bug. It prevents a "x is not used" warning.
            {
                _coords = new List<Waypoint>();
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (Input.IsKeyPressed(Keys.F12))
            {
                _isVisible = !_isVisible;
            }

            (_owner as Fotograaf).Cat.Waypoints = _coords;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
#if DEBUG
            IsEnabled = false;
            if (!_isVisible)
            {
                Tools.DrawText(spriteBatch, new Vector2(200, 20), "Press F12 to enter the editor.");
                return;
            }

            IsEnabled = true;

            bool placeNewWaypoint = Input.IsAddWayPoint();
            bool isChange         = false;


            Tools.DrawText(spriteBatch, new Vector2(200, 20), "Use left/right arrow keys do change the order, and use the up/down arrow keys to change the waypoint type. \nLeft click creates new waypoints, and the Delete button deletes them.");

            //Console.WriteLine(Input.IsTakePhotoButtonPressed());

            for(int i = 0; i < _coords.Count(); ++i) {
                Waypoint tmp = _coords[i];
                
                Tools.DrawPixel(spriteBatch, tmp.Position - (_owner as Fotograaf).GetScrollOffset());
                Tools.DrawTextCentered(spriteBatch, tmp.Position - (_owner as Fotograaf).GetScrollOffset(), "#" + i + " " + tmp.AnimMode.ToString());
 
                // Validate if this waypoint is within the mouse's range.
                if(Vector2.DistanceSquared(tmp.Position - (_owner as Fotograaf).GetScrollOffset(), Input.GetMouseLocation()) < 100) 
                {
                    Tools.DrawCircle(spriteBatch, tmp.Position - (_owner as Fotograaf).GetScrollOffset());

                    // Delete waypoint:
                    if (Input.IsKeyPressed(Keys.Delete))
                    {
                        _coords.RemoveAt(i);
                        isChange = true;
                        if (i > 0) --i;
                    }

                    // Toggle animation mode:
                    else if (Input.IsKeyPressed(Keys.Up) || Input.IsKeyPressed(Keys.Down))
                    {
                        tmp.AnimMode = (tmp.AnimMode == Waypoint.AnimModes.Walking) ? Waypoint.AnimModes.Jumping : Waypoint.AnimModes.Walking;
                        isChange = true;
                    }

                    // Decrement index
                    else if (Input.IsKeyPressed(Keys.Right))
                    {
                        if (_coords.Count() > i + 1)
                        {
                            Waypoint swap   = _coords[i + 1];
                            _coords[i + 1]  = tmp;
                            _coords[i]      = swap;
                            SaveWaypoints(_coords);
                            return;
                        }
                    }

                    // Increment index
                    else if (Input.IsKeyPressed(Keys.Left))
                    {
                        if (i > 0)
                        {
                            Waypoint swap   = _coords[i - 1];
                            _coords[i - 1]  = tmp;
                            _coords[i]      = swap;
                            SaveWaypoints(_coords);
                            return;
                        }
                    } 

                    // Delete all nodes with a higher index (exclusive). This is more or less expirimental.
                    else if(Input.IsKeyPressed(Keys.F11)) 
                    {
                        //Console.WriteLine(i + " " + _coords.Count());
                        _coords.RemoveRange(i+1, _coords.Count() - 1 - i);
                        SaveWaypoints(_coords);
                        return;
                    }

                    if (Input.IsLeftMouseButtonDown())
                    {
                        if (tmp.IsBoundToMouse) tmp.Position = Input.GetMouseLocation() + (_owner as Fotograaf).GetScrollOffset();

                        tmp.IsBoundToMouse  = true;
                        isChange            = true;
                    }
                    else tmp.IsBoundToMouse = false;
                        
                    
                    placeNewWaypoint = false; 
                }
            }

            if (placeNewWaypoint)
            {
                Vector2 pos  = Input.GetMouseLocation() + (_owner as Fotograaf).GetScrollOffset();
                Waypoint tmp = new Waypoint((int)pos.X, (int)pos.Y, Waypoint.AnimModes.Walking);

                _coords.Add(tmp);
            }

            if(isChange) SaveWaypoints(_coords);
#endif
        }



        // TODO: Use LINQ instead of this "crappy" code.
        public static void SaveWaypoints(List<Waypoint> waypoints) {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineHandling = NewLineHandling.Entitize;
            settings.NewLineOnAttributes = true;

            try
            {
                using (XmlWriter writer = XmlWriter.Create(SaveFileName, settings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("map");
                    writer.WriteStartAttribute("version", "0.1");
                    writer.WriteStartAttribute("title", "livingroom");

                    writer.WriteStartElement("nodes");
                    for (int i = 0; i < waypoints.Count(); ++i)
                    {
                        Waypoint waypoint = waypoints[i];
                        writer.WriteStartElement("node");
                        writer.WriteElementString("index", i.ToString());
                        writer.WriteElementString("x", waypoint.Position.X.ToString());
                        writer.WriteElementString("y", waypoint.Position.Y.ToString());
                        writer.WriteElementString("animMode", waypoint.AnimMode.ToString());
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();

                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        // TODO: Use LINQ instead of this "crappy" code.
        public static List<Waypoint> LoadWayPoints()
        {   
            List<Waypoint> buffer = new List<Waypoint>();
            Waypoint tmp          = null;

            using (var reader = new XmlTextReader(SaveFileName))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        switch (reader.Name)
                        {
                            case "node":
                                tmp = new Waypoint();
                                break;
                            case "x":
                                reader.Read();
                                tmp.Position = new Vector2(int.Parse(reader.Value), tmp.Position.Y);
                                break;
                            case "y":
                                reader.Read();
                                tmp.Position = new Vector2(tmp.Position.X, int.Parse(reader.Value));
                                break;
                            case "animMode":
                                reader.Read();
                                tmp.AnimMode = (Waypoint.AnimModes)Enum.Parse(typeof(Waypoint.AnimModes), reader.Value);
                                break;
                        }
                    }

                    if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "node")
                    {
                        if (tmp != null) buffer.Add(tmp);
                        //Console.WriteLine(tmp);
                    }
                }

                return buffer;
            }
        }
    }
}
