using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class TurboSpiker : EntityRenderer
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
            int direction = (int)e.attributesMap["direction"].ValueUInt8;
            if (direction == 1) fliph = true;


            var Animation = LoadAnimation("HCZ/TurboSpiker.bin", d, 3, 0);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
            Animation = LoadAnimation("HCZ/TurboSpiker.bin", d, 0, 0);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
        }

        public override string GetObjectName()
        {
            return "TurboSpiker";
        }
    }
}
