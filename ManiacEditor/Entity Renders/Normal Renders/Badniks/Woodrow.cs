using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Woodrow : EntityRenderer
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

            int type = (int)e.attributesMap["type"].ValueUInt8;
            int direction = (int)e.attributesMap["direction"].ValueUInt8;
            int animID;
            if (type == 1)
            {
                animID = 2;
            }
            else
            {
                animID = 0;
            }
            if (direction == 1)
            {
                fliph = true;
            }


            var Animation = LoadAnimation("Woodrow", d, animID, 0);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x + (fliph ? -Animation.RequestedFrame.PivotX - 6 : 0), y + (flipv ? 0 : 0), Transparency, fliph, flipv);
        }

        public override string GetObjectName()
        {
            return "Woodrow";
        }
    }
}
