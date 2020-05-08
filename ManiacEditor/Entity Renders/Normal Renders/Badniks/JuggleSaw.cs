using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class JuggleSaw : EntityRenderer
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
            bool hasSaw = e.attributesMap["hasSaw"].ValueBool;
            int animID;
            if (direction == 2)
            {
                if (hasSaw)
                {
                    animID = 4;
                }
                else
                {
                    animID = 3;
                }

            }
            else if (direction == 3)
            {
                fliph = true;
                if (hasSaw)
                {
                    animID = 4;
                }
                else
                {
                    animID = 3;
                }

            }
            else
            {
                if (hasSaw)
                {
                    animID = 1;
                }
                else
                {
                    animID = 0;
                }
            }
            if (direction == 1)
            {
                flipv = true;
            }


            var Animation = LoadAnimation("JuggleSaw", d, animID, 0);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x + (fliph ? (hasSaw ? (38) : 16) : 0), y + (flipv ? (hasSaw ? (37) : 15) : 0), Transparency, fliph, flipv);
        }

        public override string GetObjectName()
        {
            return "JuggleSaw";
        }
    }
}
