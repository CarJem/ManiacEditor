using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Dragonfly : EntityRenderer
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

            var Animation = LoadAnimation(GetSetupAnimation(), d, 0, 0);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
        }

        public override string GetSetupAnimation()
        {
            return GetSpriteAnimationPath("/Dragonfly.bin", "Dragonfly", new string[] { "PSZ2", "PSZ1" });
        }

        public override string GetObjectName()
        {
            return "Dragonfly";
        }
    }
}
