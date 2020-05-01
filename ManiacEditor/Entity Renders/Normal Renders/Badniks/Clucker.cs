using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Clucker : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp Properties)
        {
            DevicePanel d = Properties.Graphics;

            Classes.Scene.EditorEntity e = Properties.EditorObject;
            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;

            int direction = (int)e.attributesMap["direction"].ValueUInt8;
            bool fliph = false;
            bool flipv = false;
            if (direction == 1)
            {
                flipv = true;
            }

            var Animation = LoadAnimation("Clucker", d, 0, 0);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
            Animation = LoadAnimation("Clucker", d, 0, 1);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y + (flipv ? (16) : -16), Transparency, fliph, flipv);
            Animation = LoadAnimation("Clucker", d, 0, 2);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y + (flipv ? (-4) : -16), Transparency, fliph, flipv);
        }

        public override string GetObjectName()
        {
            return "Clucker";
        }
    }
}
