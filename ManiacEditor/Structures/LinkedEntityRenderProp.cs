using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiacEditor.Structures
{
    public struct LinkedEntityRenderProp
    {
        public DevicePanel Graphics { get; set; }

        public Classes.Scene.EditorEntity EditorObject { get; set; }

        public LinkedEntityRenderProp(DevicePanel d, Classes.Scene.EditorEntity ObjectInstance)
        {
            Graphics = d;
            EditorObject = ObjectInstance;
        }
    }
}
