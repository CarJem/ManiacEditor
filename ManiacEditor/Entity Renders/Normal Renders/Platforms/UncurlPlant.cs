using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class UncurlPlant : EntityRenderer
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
            if (direction == 0) fliph = true;

            var Animation = LoadAnimation("SSZ1/Plants.bin", d, 1, 0);

            int new_x = x + (fliph ? 112 : -112);

            for (int i = 0; i < 8; i++)
            {
                int offset_x = new_x + (fliph ? -(16 * i) : (16 * i));
                DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, offset_x, y, Transparency, fliph, flipv);
            }
        }

        public override string GetObjectName()
        {
            return "UncurlPlant";
        }
    }
}
