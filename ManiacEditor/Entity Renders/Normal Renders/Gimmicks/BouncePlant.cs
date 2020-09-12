using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class BouncePlant : EntityRenderer
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
            x += (fliph ? 42 : -42);
            y += -42;

            for (int i = 0; i < 8; i++)
            {
                var Animation = LoadAnimation("SSZ1/Plants.bin", d, 1, 0);
                DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x + (fliph ? -(12 * i) : (12 * i)), y + 12 * i, Transparency, fliph, flipv);
            }
        }

        public override string GetObjectName()
        {
            return "BouncePlant";
        }
    }
}
