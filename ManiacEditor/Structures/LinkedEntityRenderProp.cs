using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiacEditor.Structures
{
    public struct LinkedEntityRenderProp
    {
        public Classes.Editor.Draw.GraphicsHandler Graphics { get; set; }

        public RSDKv5.SceneEntity Object { get; set; }

        public Classes.Editor.Scene.Sets.EditorEntity EditorObject { get; set; }

        public LinkedEntityRenderProp(Classes.Editor.Draw.GraphicsHandler d, RSDKv5.SceneEntity currentEntity, Classes.Editor.Scene.Sets.EditorEntity ObjectInstance)
        {
            Graphics = d;
            Object = currentEntity;
            EditorObject = ObjectInstance;
        }
    }
}
