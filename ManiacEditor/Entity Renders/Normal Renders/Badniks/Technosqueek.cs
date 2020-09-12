using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Technosqueek : EntityRenderer
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
            int animID = 0;
            if (type == 0)
            {
                animID = 0;
                if (direction == 1)
                {
                    fliph = true;
                }
                if (direction == 2)
                {
                    flipv = true;
                }
                if (direction == 3)
                {
                    flipv = true;
                    fliph = true;
                }
            }
            else if (type == 1)
            {
                animID = 3;
                if (direction == 1)
                {
                    fliph = true;
                }
                if (direction == 2)
                {
                    flipv = true;
                }
                if (direction == 3)
                {
                    flipv = true;
                    fliph = true;
                }
            }
            else if (type == 2)
            {
                animID = 0;
                if (direction == 1)
                {
                    fliph = true;
                }
                if (direction == 2)
                {
                    flipv = true;
                }
                if (direction == 3)
                {
                    flipv = true;
                    fliph = true;
                }

            }
            else if (type == 3)
            {
                animID = 3;
                if (direction == 1)
                {
                    fliph = true;
                }
                if (direction == 2)
                {
                    flipv = true;
                }
                if (direction == 3)
                {
                    flipv = true;
                    fliph = true;
                }

            }

            var Animation = LoadAnimation("FBZ/Technosqueek.bin", d, animID, 0);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x - (fliph ? 2 : 0), y + (flipv ? 2 : 0), Transparency, fliph, flipv);
        }

        public override string GetObjectName()
        {
            return "Technosqueek";
        }
    }
}
