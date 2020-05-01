using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Flipper : EntityRenderer
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
            if (direction == 1) fliph = true;

            var Animation = LoadAnimation("Flipper", d, 0, 4);
            DrawTexturePivotPlus(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, (fliph ? -38 : 0), 0, Transparency, fliph, flipv);
        }

        public override string GetObjectName()
        {
            return "Flipper";
        }
    }
}
