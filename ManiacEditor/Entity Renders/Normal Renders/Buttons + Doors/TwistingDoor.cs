using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class TwistingDoor : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp Properties)
        {
            DevicePanel d = Properties.Graphics;
            Classes.Scene.EditorEntity e = Properties.EditorObject;
            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;

            int type = (int)e.attributesMap["type"].ValueEnum;
            int direction = (int)e.attributesMap["direction"].ValueUInt8;
            bool fliph = false;
            bool flipv = false;
            int animID = 0;
            switch (type)
            {
                case 0:
                    animID = 0;
                    break;
                case 1:
                    animID = 1;
                    break;
                case 2:
                    animID = 2;
                    break;
                case 3:
                    animID = 3;
                    break;

            }
            switch (direction)
            {
                case 0:
                    break;
                case 1:
                    flipv = true;
                    break;
                default:
                    break;
            }



            var Animation = LoadAnimation("TwistingDoor", d, animID, 0);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x - (fliph ? (10) : 0), y, Transparency, fliph, flipv);
        }

        public override string GetObjectName()
        {
            return "TwistingDoor";
        }
    }
}
