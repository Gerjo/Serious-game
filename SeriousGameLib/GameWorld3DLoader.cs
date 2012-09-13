using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Runtime.Remoting;
using System.Globalization;

namespace SeriousGameLib
{
    public class GameWorld3DLoader
    {
        private static readonly CultureInfo _parseCulture = new CultureInfo("EN-us");

        public static List<GameObject3D> LoadMap(XDocument world)
        {
            List<GameObject3D> mapObjects = new List<GameObject3D>();

            var remotingInfo = XElement.Load(world.CreateReader()).Element("remotinginfo");
            string gameobjectsAssemblyname = remotingInfo.Element("assemblyname").Value;
            string gameobjectsNamespace = remotingInfo.Element("namespace").Value;

            var gameobjects = XElement.Load(world.CreateReader()).Elements("gameobject");

            foreach (var gameobject in gameobjects)
            {
                string classvalue = gameobject.Element("class").Value;
                string uidvalue = gameobject.Element("uid").Value;
                string positionvalue = gameobject.Element("position").Value;
                string rotationYvalue = gameobject.Element("rotationY").Value;
                string rotationZvalue = gameobject.Element("rotationZ").Value;
                string collissionvalue = gameobject.Element("collission").Value;
                string firemouseovervalue = gameobject.Element("firemouseover").Value;
                string firemouseclickvalue = gameobject.Element("firemouseclick").Value;

                string[] splitPosition = positionvalue.Split(',');
                CollissionType collissionType = (CollissionType)Enum.Parse(typeof(CollissionType), collissionvalue, true);

                ObjectHandle wrappedObject = Activator.CreateInstance(gameobjectsAssemblyname, string.Format("{0}.{1}", gameobjectsNamespace, classvalue));
                GameObject3D unwrappedObject = wrappedObject.Unwrap() as GameObject3D;
                
                unwrappedObject.UID = uidvalue;
                unwrappedObject.RotateY = float.Parse(rotationYvalue, _parseCulture);
                unwrappedObject.RotateZ = float.Parse(rotationZvalue, _parseCulture);
                unwrappedObject.CollissionBehaviour = collissionType;
                unwrappedObject.FireMouseOverEvent = bool.Parse(firemouseovervalue);
                unwrappedObject.FireMouseClickEvent = bool.Parse(firemouseclickvalue);
                unwrappedObject.Position = new Microsoft.Xna.Framework.Vector3(float.Parse(splitPosition[0], _parseCulture),
                                                                               float.Parse(splitPosition[1], _parseCulture),
                                                                               float.Parse(splitPosition[2], _parseCulture));

                mapObjects.Add(unwrappedObject);
            }

            return mapObjects;
        }
    }
}
