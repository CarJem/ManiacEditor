using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class HCZOneWayDoor : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp Properties)
        {
            DevicePanel d = Properties.Graphics;
            Classes.Scene.EditorEntity e = Properties.EditorObject;
            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;

            int length = (int)(e.attributesMap["length"].ValueEnum) - 1;
            int orientation = (int)(e.attributesMap["orientation"].ValueUInt8);
            bool fliph = false;
            bool flipv = false;
            int width = 0;
            int height = 0;
            switch (orientation) {
                case 0:
                    height = length;
                    break;
                case 1:
                    width = length;
                    break;
                case 2:
                    height = length;
                    break;
                case 3:
                    width = length;
                    break;
                default:
                    height = length;
                    break;
            }


            var Animation = LoadAnimation("HCZ/ButtonDoor.bin", d, 0, 0);
            
            bool wEven = width % 2 == 0;
            bool hEven = height % 2 == 0;
            for (int xx = 0; xx <= width; ++xx)
            {
                for (int yy = 0; yy <= height; ++yy)
                {
                    DrawTexture(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, 
                        x + (wEven ? Animation.RequestedFrame.PivotX : -Animation.RequestedFrame.Width) + (-width / 2 + xx) * Animation.RequestedFrame.Width,
                        y + (hEven ? Animation.RequestedFrame.PivotY : -Animation.RequestedFrame.Height) + (-height / 2 + yy) * Animation.RequestedFrame.Height, 
                        Transparency, fliph, flipv);
                }
            }
        }

        public override string GetObjectName()
        {
            return "HCZOneWayDoor";
        }
    }
}
