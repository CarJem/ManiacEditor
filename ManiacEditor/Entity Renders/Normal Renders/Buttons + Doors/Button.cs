using RSDKv5;
using System.Collections.Generic;

namespace ManiacEditor.Entity_Renders
{
    public class Button : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp Properties)
        {
            DevicePanel d = Properties.Graphics;
            Classes.Scene.EditorEntity e = Properties.EditorObject;
            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;

            bool fliph = false;
            bool flipv = false;

            int type = (int)e.attributesMap["type"].ValueEnum;
            int animID = 0;
            if (type == 0 || type == 1)
            {
                animID = 0;
            }
            if (type == 2 || type == 3)
            {
                animID = 1;
            }
            if (type == 3)
            {
                fliph = true;
            }
            if (type == 1)
            {
                flipv = true;
            }

            string SetupType = GetSetupAnimation();
            var Animation = LoadAnimation(SetupType, d, animID, 2);


            if (SetupType.StartsWith("HCZ"))
            {
                Animation = LoadAnimation(GetSetupAnimation(), d, animID, 2);
                DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
            }
            if (SetupType.StartsWith("HCZ") || SetupType.StartsWith("LRZ2") || SetupType.StartsWith("LRZ1") || SetupType.StartsWith("FBZ"))
            {
                Animation = LoadAnimation(GetSetupAnimation(), d, animID, 1);
                DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
            }


            Animation = LoadAnimation(GetSetupAnimation(), d, animID, 0);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
        }

        public override string GetSetupAnimation()
        {
            return GetSpriteAnimationPath("/Button.bin", "Button", new string[] { "LRZ1", "HCZ", "TMZ1", "MMZ", "FBZ" });
        }

        public override string GetObjectName()
        {
            return "Button";
        }
    }
}
