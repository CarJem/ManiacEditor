using RSDKv5;
using SystemColors = System.Drawing.Color;

namespace ManiacEditor.Entity_Renders
{
    public class Current : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp Properties)
        {
            DevicePanel d = Properties.Graphics;
            Classes.Scene.EditorEntity e = Properties.EditorObject;
            
            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;

            var widthPixels = (int)(e.attributesMap["size"].ValueVector2.X.High);
            var heightPixels = (int)(e.attributesMap["size"].ValueVector2.Y.High);
            var width = (int)widthPixels / 16;
            var height = (int)heightPixels / 16;

            var Animation = Methods.Drawing.ObjectDrawing.LoadAnimation(Properties.Graphics, "EditorAssets", 0, 1);

            if (width != 0 && height != 0)
            {
                int x1 = x + widthPixels / -2;
                int x2 = x + widthPixels / 2 - 1;
                int y1 = y + heightPixels / -2;
                int y2 = y + heightPixels / 2 - 1;


                d.DrawLine(x1, y1, x1, y2, SystemColors.White);
                d.DrawLine(x1, y1, x2, y1, SystemColors.White);
                d.DrawLine(x2, y2, x1, y2, SystemColors.White);
                d.DrawLine(x2, y2, x2, y1, SystemColors.White);

                d.DrawLine(x1-16, y1-16, x1-16, y2+16, SystemColors.Blue);
                d.DrawLine(x1-16, y1-16, x2+16, y1-16, SystemColors.Blue);
                d.DrawLine(x2+16, y2+16, x1-16, y2+16, SystemColors.Blue);
                d.DrawLine(x2+16, y2+16, x2+16, y1-16, SystemColors.Blue);

                // draw corners
                for (int i = 0; i < 4; i++)
                {
                    bool right = (i & 1) > 0;
                    bool bottom = (i & 2) > 0;

                    int realX = (x + widthPixels / (right ? 2 : -2)) - (right ? Animation.RequestedFrame.Width : 0);
                    int realY = (y + heightPixels / (bottom ? 2 : -2) - (bottom ? Animation.RequestedFrame.Height : 0));
                    DrawTexture(Properties.Graphics, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, realX, realY, Transparency, right, bottom);


                }
            }
        }

        public override bool isObjectOnScreen(DevicePanel d, Classes.Scene.EditorEntity entity, int x, int y, int Transparency)
        {
            var widthPixels = (int)(entity.attributesMap["size"].ValueVector2.X.High);
            var heightPixels = (int)(entity.attributesMap["size"].ValueVector2.Y.High);
            return d.IsObjectOnScreen(x - widthPixels / 16, y - heightPixels / 16, widthPixels, heightPixels);
        }

        public override string GetObjectName()
        {
            return "Current";
        }
    }
}
