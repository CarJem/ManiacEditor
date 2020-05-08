using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Spear : EntityRenderer
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

            int orientation = (int)e.attributesMap["orientation"].ValueUInt8;
            int animID = 0;
            switch (orientation)
            {
                case 1:
                    animID = 1;
                    break;
                case 2:
                    flipv = true;
                    break;
                case 3:
                    fliph = true;
                    animID = 1;
                    break;
            }

            var Animation = LoadAnimation("Spear", d, animID, 1);
            DrawTexture(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x + (fliph ? -Animation.RequestedFrame.PivotX - Animation.RequestedFrame.Width : Animation.RequestedFrame.PivotX), y + (flipv ? -Animation.RequestedFrame.PivotY - Animation.RequestedFrame.Height : Animation.RequestedFrame.PivotY), Transparency, fliph, flipv);
            Animation = LoadAnimation("Spear", d, animID, 0);
            DrawTexture(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x + (fliph ? -Animation.RequestedFrame.PivotX - Animation.RequestedFrame.Width : Animation.RequestedFrame.PivotX), y + (flipv ? -Animation.RequestedFrame.PivotY - Animation.RequestedFrame.Height : Animation.RequestedFrame.PivotY), Transparency, fliph, flipv);
        }

        public override string GetObjectName()
        {
            return "Spear";
        }
    }
}
