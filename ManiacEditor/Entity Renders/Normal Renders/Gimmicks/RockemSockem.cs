using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class RockemSockem : EntityRenderer
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


            Animation = LoadAnimation(GetSetupAnimation(), d, 1, 0);
            for (int i = 0; i < 6; i++)
            {
                DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y - 8 - (i * 4), Transparency, fliph, flipv);
            }

            Animation = LoadAnimation(GetSetupAnimation(), d, 2, 0);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y - 44, Transparency, fliph, flipv);
        }

        public override string GetSetupAnimation()
        {
            return GetSpriteAnimationPath("/RockemSockem.bin", "RockemSockem", new string[] { "SPZ1", "SPZ2" });
        }

        public override string GetObjectName()
        {
            return "RockemSockem";
        }
    }
}
