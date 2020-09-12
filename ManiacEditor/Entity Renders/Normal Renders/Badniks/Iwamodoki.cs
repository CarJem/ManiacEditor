using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Iwamodoki : EntityRenderer
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

            var Animation = LoadAnimation(GetSetupAnimation(), d, type, 6);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
        }

        public override string GetSetupAnimation()
        {
            return GetSpriteAnimationPath("/Iwamodoki.bin", "Iwamodoki", new string[] { "LRZ2", "LRZ1" });
        }

        public override string GetObjectName()
        {
            return "Iwamodoki";
        }
    }
}
