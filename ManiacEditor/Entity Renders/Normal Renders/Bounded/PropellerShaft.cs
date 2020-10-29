using RSDKv5;
using SystemColors = System.Drawing.Color;

namespace ManiacEditor.Entity_Renders
{
    public class PropellerShaft : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp Properties)
        {
            DevicePanel d = Properties.Graphics;

            Classes.Scene.EditorEntity e = Properties.EditorObject;
            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;


            var height_value = (int)(e.attributesMap["size"].ValueEnum);
            var height = (height_value > 0 ? height_value / 2 : height_value);
            var width = 6;

            if (width != -1 && height != -1)
            {
                int x1 = (x + width);
                int x2 = (x - width) - 1;
                int y1 = (y + height * 2);
                int y2 = (y - height * 2);

                DrawBounds2(d, x1, y1, x2, y2, Transparency, SystemColors.White, SystemColors.Transparent);
            }

            var Animation = LoadAnimation("EditorIcons", d, 0, 6);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency);
        }

        public override bool isObjectOnScreen(DevicePanel d, Classes.Scene.EditorEntity e, int x, int y, int Transparency)
        {
            var widthPixels = (int)(13) * 16;
            var heightPixels = (int)(e.attributesMap["size"].ValueEnum * 2 - 1) * 16;
            return d.IsObjectOnScreen(x - widthPixels / 2, y - heightPixels / 2, widthPixels + 15, heightPixels + 15);
        }

        public override string GetObjectName()
        {
            return "PropellerShaft";
        }
    }
}
