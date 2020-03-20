using RSDKv5;
using SystemColors = System.Drawing.Color;
using System.Linq;

namespace ManiacEditor.Entity_Renders
{
    public class UIControl : EntityRenderer
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

            var widthPixels = (int)(e.attributesMap["size"].ValueVector2.X.High);
            var heightPixels = (int)(e.attributesMap["size"].ValueVector2.Y.High);
            var width = (int)widthPixels / 16;
            var height = (int)heightPixels / 16;

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

                // draw corners
                for (int i = 0; i < 4; i++)
                {
                    bool right = (i & 1) > 0;
                    bool bottom = (i & 2) > 0;

                    var Animation = Methods.Entities.EntityDrawing.LoadAnimation(d, "EditorAssets", 0, 1);
                    DrawTexture(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID,
                        (x + widthPixels / (right ? 2 : -2)) - (right ? Animation.RequestedFrame.Width : 0),
                        (y + heightPixels / (bottom ? 2 : -2) - (bottom ? Animation.RequestedFrame.Height : 0)),
                        Transparency, right, bottom);
                }
            }
        }

        public override string GetObjectName()
        {
            return "UIControl";
        }
    }
}
