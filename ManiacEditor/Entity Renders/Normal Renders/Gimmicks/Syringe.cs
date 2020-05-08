using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Syringe : EntityRenderer
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


            int type = (int)e.attributesMap["type"].ValueEnum;
            System.Drawing.Color colour;
            switch (type)
            {
                case 0:
                    colour = System.Drawing.Color.FromArgb(140, 0, 8, 192);
                    break;
                case 1:
                    colour = System.Drawing.Color.FromArgb(140, 8, 184, 0);
                    break;
                case 2:
                    colour = System.Drawing.Color.FromArgb(140, 56, 168, 240);
                    break;
                default:
                    colour = System.Drawing.Color.FromArgb(140, 56, 168, 240);
                    break;
            }
            int animID = 0;

            //ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);
            d.DrawRectangle(x - 14, y + 80, x + 14, y, colour);
            var Animation = LoadAnimation("Syringe", d, animID, 0);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
            Animation = LoadAnimation("Syringe", d, animID, 1);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
        }

        public override string GetObjectName()
        {
            return "Syringe";
        }
    }
}
