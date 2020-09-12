using RSDKv5;
using SystemColors = System.Drawing.Color;

namespace ManiacEditor.Entity_Renders
{
    public class WaterfallSound : EntityRenderer
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
            var editorAnim = LoadAnimation("EditorIcons2", d, 0, 6);
            var width = (int)(e.attributesMap["size"].ValueVector2.X.High*2 - 1);
            var height = (int)(e.attributesMap["size"].ValueVector2.Y.High*2 - 1);
            DrawTexturePivotNormal(d, editorAnim, editorAnim.RequestedAnimID, editorAnim.RequestedFrameID, x, y, Transparency);

            if (e.Selected)
            {
                if (width != -1 && height != -1)
                {
                    bool wEven = width % 2 == 0;
                    bool hEven = height % 2 == 0;

                    int x1 = (x + (wEven ? -8 : -16) + (-width / 2 + width) * 16) + 15;
                    int x2 = (x + (wEven ? -8 : -16) + (-width / 2) * 16);
                    int y1 = (y + (hEven ? -8 : -16) + (-height / 2 + height) * 16) + 15;
                    int y2 = (y + (hEven ? -8 : -16) + (-height / 2) * 16);


                    d.DrawLine(x1, y1, x1, y2, SystemColors.White);
                    d.DrawLine(x1, y1, x2, y1, SystemColors.White);
                    d.DrawLine(x2, y2, x1, y2, SystemColors.White);
                    d.DrawLine(x2, y2, x2, y1, SystemColors.White);
                }
            }
        }

        public override bool isObjectOnScreen(DevicePanel d, Classes.Scene.EditorEntity e, int x, int y, int Transparency)
        {
            var widthPixels = (int)(e.attributesMap["size"].ValueVector2.X.High * 2 - 1) * 16;
            var heightPixels = (int)(e.attributesMap["size"].ValueVector2.Y.High * 2 - 1) * 16;
            return d.IsObjectOnScreen(x - widthPixels / 2, y - heightPixels / 2, widthPixels + 15, heightPixels + 15);
        }

        public override string GetObjectName()
        {
            return "WaterfallSound";
        }
    }
}
