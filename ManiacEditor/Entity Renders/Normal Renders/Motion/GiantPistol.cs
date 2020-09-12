using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class GiantPistol : EntityRenderer
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





            int direction = (int)e.attributesMap["direction"].ValueUInt8;
            if (direction == 1)
            {
                fliph = true;
            }

            var Animation = LoadAnimation("MSZ/Pistol.bin", d, 0, 1);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x - (fliph ? 76 : 0), y, Transparency, fliph, flipv);

            Animation = LoadAnimation("MSZ/Pistol.bin", d, 0, 0);
            int width = Animation.RequestedFrame.Width;
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);

            Animation = LoadAnimation("MSZ/Pistol.bin", d, 4, 0);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x - (fliph ? 59 : 0), y, Transparency, fliph, flipv);

            Animation = LoadAnimation("MSZ/Pistol.bin", d, 6, 0);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x - (fliph ? width + 4 : 0), y, Transparency, fliph, flipv);
        }

        public override string GetObjectName()
        {
            return "GiantPistol";
        }
    }
}
