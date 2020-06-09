using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RSDKv5;

namespace ManiacEditor.Methods.Entities
{
    public class ExportableSceneObject
    {
        /// <summary>
        /// the name of this type of object
        /// </summary>
        public NameIdentifier Name
        {
            get;
            set;
        }
        /// <summary>
        /// the names and types of each attribute of this object
        /// </summary>
        public List<AttributeInfo> Attributes = new List<AttributeInfo>();
        public ExportableSceneObject(SceneObject sceneObject)
        {
            Name = sceneObject.Name;
            Attributes = sceneObject.Attributes;
        }
    }
    public class ExportableZoneObject
    {

        public List<ExportableSceneObject> Objects = new List<ExportableSceneObject>();
        public string Zone = "";

        public ExportableZoneObject(string zone)
        {
            Zone = zone;
        }

        public ExportableZoneObject(string zone, List<SceneObject> objects)
        {
            Zone = zone;
            Objects = objects.ConvertAll<ExportableSceneObject>(x => new ExportableSceneObject(x)).ToList();
        }
    }
    public class ImportableZoneObject
    {

        public List<SceneObject> Objects = new List<SceneObject>();
        public string Zone = "";

        public ImportableZoneObject(string zone)
        {
            Zone = zone;
        }

        public ImportableZoneObject(string zone, List<SceneObject> objects)
        {
            Zone = zone;
            Objects = objects;
        }
    }
}
