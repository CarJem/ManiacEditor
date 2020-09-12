using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Ice : EntityRenderer
    {
        ItemBox itemBox = new ItemBox();
        Spikes spikes = new Spikes();
        IceSpring iceSpring = new IceSpring();
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
            int size = (int)e.attributesMap["size"].ValueUInt8;
            int animID = 0;
            int frameID = 0;
            switch (size)
            {
                case 0:
                    animID = 0;
                    frameID = 0;
                    break;
                case 1:
                    animID = 0;
                    frameID = 1;
                    break;
            }

            var Animation = LoadAnimation("PSZ2/Ice.bin", d, 9, 0);
            switch (type)
            {
                case 0:
                    break;
                case 1:
                    Animation = LoadAnimation("PSZ2/Ice.bin", d, 9, 0);
                    DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
                    break;
                case 2:
                    Animation = LoadAnimation("PSZ2/Ice.bin", d, 9, 1);
                    DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
                    break;
                case 3:
                    Animation = LoadAnimation("PSZ2/Ice.bin", d, 9, 2);
                    DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
                    break;
                case 4:
                    spikes.IceDraw(d, x, y, Transparency);
                    break;
                case 5:
                    itemBox.IceDraw(d, x, y, Transparency, 0);
                    break;
                case 6:
                    itemBox.IceDraw(d, x, y, Transparency, 1);
                    break;
                case 7:
                    itemBox.IceDraw(d, x, y, Transparency, 2);
                    break;
                case 8:
                    itemBox.IceDraw(d, x, y, Transparency, 3);
                    break;
                case 9:
                    itemBox.IceDraw(d, x, y, Transparency, 4);
                    break;
                case 10:
                    itemBox.IceDraw(d, x, y, Transparency, 5);
                    break;
                case 11:
                    itemBox.IceDraw(d, x, y, Transparency, 6);
                    break;
                case 12:
                    itemBox.IceDraw(d, x, y, Transparency, 7);
                    break;
                case 13:
                    itemBox.IceDraw(d, x, y, Transparency, 10);
                    break;
                case 14:
                    itemBox.IceDraw(d, x, y, Transparency, 11);
                    break;
                case 15:
                    itemBox.IceDraw(d, x, y, Transparency, 12);
                    break;
                case 16:
                    itemBox.IceDraw(d, x, y, Transparency, 13);
                    break;
                case 17:
                    itemBox.IceDraw(d, x, y, Transparency, 14);
                    break;
                case 18:
                    iceSpring.IceDraw(d, x, y, Transparency);
                    break;
                case 19:
                    itemBox.IceDraw(d, x, y, Transparency, 16);
                    break;
                case 20:
                    itemBox.IceDraw(d, x, y, Transparency, 17);
                    break;
                default:
                    itemBox.IceDraw(d, x, y, Transparency, -1);
                    break;
            }

            Animation = LoadAnimation("PSZ2/Ice.bin", d, animID, frameID);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
        }

        public override string GetObjectName()
        {
            return "Ice";
        }
    }
}
