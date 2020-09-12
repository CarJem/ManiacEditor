using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class IceSpring : EntityRenderer
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

            int animID = (int)e.attributesMap["type"].ValueEnum;
            var flipFlag = e.attributesMap["flipFlag"].ValueEnum;

            // Handle springs being flipped in both planes
            // Down
            if ((flipFlag & 0x02) == 0x02)
                flipv = true;
            // Left
            if ((flipFlag & 0x01) == 0x01)
                fliph = true;


            var Animation = LoadAnimation("PSZ2/IceSpring.bin", d, animID % 3, 0);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
        }

        public void IceDraw(DevicePanel d, int x, int y, int Transparency)
        {
            bool fliph = false;
            bool flipv = false;

            var Animation = LoadAnimation("PSZ2/IceSpring.bin", d, 0, 0);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
        }

        public override string GetObjectName()
        {
            return "IceSpring";
        }
    }
}
