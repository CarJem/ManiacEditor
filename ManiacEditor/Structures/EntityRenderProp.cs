using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiacEditor.Structures
{
    public struct EntityRenderProp
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Transparency { get; set; }
        public int Index { get; set; }
        public int PreviousChildCount { get; set; }
        public int PlatformAngle { get; set; }
        public RSDKv5.SceneEntity Object { get; set; }
        public Classes.Core.Scene.Sets.EditorEntity EditorObject { get; set; }
        public Methods.Entities.EntityAnimator Animations { get; set; }
        public Classes.Core.Draw.GraphicsHandler Graphics { get; set; }
        public bool isSelected { get; set; }

        public EntityRenderProp(Classes.Core.Draw.GraphicsHandler d, RSDKv5.SceneEntity entity, Classes.Core.Scene.Sets.EditorEntity e, int x, int y, int transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, Methods.Entities.EntityAnimator Animation = null, bool selected = false)
        {
            Graphics = d;
            Object = entity;
            EditorObject = e;
            X = x;
            Y = y;
            Transparency = transparency;
            Index = index;
            PreviousChildCount = previousChildCount;
            PlatformAngle = platformAngle;
            Animations = Animation;
            isSelected = selected;
        }

    }
}
