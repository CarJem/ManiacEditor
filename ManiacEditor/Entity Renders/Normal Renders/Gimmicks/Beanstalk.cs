using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Beanstalk : EntityRenderer
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
            int type = (int)e.attributesMap["type"].ValueUInt8;
            int animID = 0;
            int frameID = 0;
            int offset_x = 0;
            switch (type)
            {
                case 0:
                    animID = 4;
                    frameID = 9;
                    break;
                case 1:
                    animID = 0;
                    frameID = 0;
                    break;
                case 2:
                    animID = 2;
                    frameID = 0;
                    offset_x = -54;
                    break;
                case 3:
                    animID = 3;
                    frameID = 0;
                    offset_x = -24;
                    break;
            }

            if (direction == 1) fliph = true;

            var Animation = LoadAnimation("SSZ1/Beanstalk.bin", d, 0, 0);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
            Animation = LoadAnimation("SSZ1/Beanstalk.bin", d, animID, frameID);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x + (fliph ? offset_x : 0), y, Transparency, fliph, flipv);


        }

        public override string GetObjectName()
        {
            return "Beanstalk";
        }
    }
}
