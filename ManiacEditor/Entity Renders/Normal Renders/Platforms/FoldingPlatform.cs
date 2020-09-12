using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class FoldingPlatform : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp Properties)
        {
            DevicePanel d = Properties.Graphics;
            Classes.Scene.EditorEntity entity = Properties.EditorObject;

            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;

            int direction = (int)entity.attributesMap["direction"].ValueUInt8;
            bool fliph = false;
            bool flipv = false;
            if (direction == 1)
            {
                fliph = true;
            }

            var Animation = LoadAnimation(GetSetupAnimation(), d, 2, 4);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
        }

        public override string GetSetupAnimation()
        {
            return GetSpriteAnimationPath("/Platform.bin", "Platform", new string[] { "GHZ", "CPZ", "SPZ1", "SPZ2", "FBZ", "PSZ1", "PSZ2", "SSZ1", "SSZ2", "HCZ", "MSZ", "OOZ", "LRZ1", "LRZ2", "MMZ", "TMZ1", "AIZ" });
        }

        public override string GetObjectName()
        {
            return "FoldingPlatform";
        }
    }
}
