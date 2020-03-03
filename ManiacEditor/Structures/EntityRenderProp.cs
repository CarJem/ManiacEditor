using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiacEditor.Structures
{
    public struct EntityRenderProp
    {
        public Classes.Scene.Sets.EditorEntity EditorObject { get; set; }
        public DevicePanel Graphics { get; set; }
        public int DrawX { get; set; }
        public int DrawY { get; set; }
        public int Transparency { get; set; }

        public EntityRenderProp(DevicePanel d, Classes.Scene.Sets.EditorEntity e, int X, int Y, int _Transparency)
        {
            Graphics = d;
            EditorObject = e;
            DrawX = X;
            DrawY = Y;
            Transparency = _Transparency;
        }

    }
}
