using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class IceBomba : EntityRenderer
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
            int dir = e.attributesMap["dir"].ValueUInt8;
            int frameID = 0;
            switch (dir)
            {
                case 1:
                    fliph = true;
                    frameID = 4;
                    break;

            }

            var Animation = LoadAnimation(GetSetupAnimation(), d, 2, frameID);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y + 25, Transparency, fliph, flipv);

            Animation = LoadAnimation(GetSetupAnimation(), d, 0, 0);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);

            Animation = LoadAnimation(GetSetupAnimation(), d, 1, 0);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x - (fliph ? 6 : 0), y, Transparency, fliph, flipv);
        }

        public override string GetSetupAnimation()
        {
            return GetSpriteAnimationPath("/IceBomba.bin", "IceBomba", new string[] { "PSZ2", "PSZ1" });
        }

        public override string GetObjectName()
        {
            return "IceBomba";
        }
    }
}
