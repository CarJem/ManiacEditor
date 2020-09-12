using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class SpikeCrusher : EntityRenderer
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



            var Animation = LoadAnimation(GetSetupAnimation(), d, 3, 5);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
            Animation = LoadAnimation(GetSetupAnimation(), d, 3, 0);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);

        }

        public override string GetSetupAnimation()
        {
            return GetSpriteAnimationPath("/Platform.bin", "Platform", new string[] { "GHZ", "CPZ", "SPZ1", "SPZ2", "FBZ", "PSZ1", "PSZ2", "SSZ1", "SSZ2", "HCZ", "MSZ", "OOZ", "LRZ1", "LRZ2", "MMZ", "TMZ1", "AIZ" });
        }

        public override string GetObjectName()
        {
            return "SpikeCrusher";
        }
    }
}
