using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RSDKv5;

namespace ManiacEditor.Methods.Entities
{
    public struct ObjectSearchPair
    {
        public List<string> TargetNames { get; private set; }
        public List<SceneObject> SourceObjects { get; private set; }

        public ObjectSearchPair(List<string> names, List<SceneObject> objects)
        {
            TargetNames = names;
            SourceObjects = objects;
        }
    }
    public struct MultiZoneObjectSearchPair
    {
        public List<string> TargetNames { get; private set; }
        public List<ImportableZoneObject> SourceObjects { get; private set; }

        public MultiZoneObjectSearchPair(List<string> names, List<ImportableZoneObject> objects)
        {
            TargetNames = names;
            SourceObjects = objects;
        }
    }



    public struct ObjectExportPair
    {
        public List<string> TargetNames { get; private set; }
        public List<ExportableSceneObject> SourceObjects { get; private set; }

        public ObjectExportPair(ObjectSearchPair pair)
        {
            TargetNames = pair.TargetNames;
            SourceObjects = pair.SourceObjects.ConvertAll(x => new ExportableSceneObject(x)).ToList();
        }
    }
    public struct MultiZoneObjectExportPair
    {
        public List<string> TargetNames { get; private set; }
        public List<ExportableZoneObject> SourceObjects { get; private set; }

        public MultiZoneObjectExportPair(MultiZoneObjectSearchPair pair)
        {
            TargetNames = pair.TargetNames;
            SourceObjects = pair.SourceObjects.ConvertAll(x => new ExportableZoneObject(x.Zone, x.Objects)).ToList();
        }
    }
}
