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

        public RSDKv5.SceneEntity Object { get; set; }

        public Classes.Scene.Sets.EditorEntity EditorObject { get; set; }

        public LinkedEntityRenderProp(DevicePanel d, RSDKv5.SceneEntity currentEntity, Classes.Scene.Sets.EditorEntity ObjectInstance)
        {
            Graphics = d;
            Object = currentEntity;
            EditorObject = ObjectInstance;
        }
    }
}
