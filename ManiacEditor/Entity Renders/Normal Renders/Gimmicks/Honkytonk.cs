using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Honkytonk : EntityRenderer
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

            int angle = (int)e.attributesMap["angle"].ValueEnum;
            int rotation = (int)(angle / -0.71);


            var Animation = LoadAnimation("MSZ/HonkyTonk.bin", d, 0, 0);

            int offset_x = (int)(Animation.RequestedFrame.Width / 2);
            int offset_y = (int)(Animation.RequestedFrame.Height / 2);

            DrawTexture(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x - offset_x, y - offset_y, Transparency, false, false, rotation);

            Animation = LoadAnimation("MSZ/HonkyTonk.bin", d, 0, 1);

            offset_x = (int)(Animation.RequestedFrame.Width / 2);
            offset_y = (int)(Animation.RequestedFrame.Height / 2);

            DrawTexture(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x - offset_x, y - offset_y, Transparency, false, false, rotation);

            Animation = LoadAnimation("MSZ/HonkyTonk.bin", d, 0, 2);

            offset_x = (int)(Animation.RequestedFrame.Width / 2);
            offset_y = (int)(Animation.RequestedFrame.Height / 2);

            DrawTexture(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x - offset_x, y - offset_y, Transparency, false, false, rotation);

            Animation = LoadAnimation("MSZ/HonkyTonk.bin", d, 0, 3);

            offset_x = (int)(Animation.RequestedFrame.Width / 2);
            offset_y = (int)(Animation.RequestedFrame.Height / 2);

            DrawTexture(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x - offset_x, y - offset_y, Transparency, false, false, rotation);


        }

        public override string GetObjectName()
        {
            return "Honkytonk";
        }
    }
}
