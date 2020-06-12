using RSDKv5;
using SystemColors = System.Drawing.Color;

namespace ManiacEditor.Entity_Renders
{
    public class RisingLava : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp Properties)
        {
            DevicePanel d = Properties.Graphics;
            Classes.Scene.EditorEntity e = Properties.EditorObject;
            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;

            int offsetX = (int)(e.attributesMap["offset"].ValueVector2.X.High);
            int offsetY = (int)(e.attributesMap["offset"].ValueVector2.Y.High);
            var widthPixels = (int)(e.attributesMap["size"].ValueVector2.X.High);
            var heightPixels = (int)(e.attributesMap["size"].ValueVector2.Y.High);

            DrawBounds(d, x, y, widthPixels, heightPixels, Transparency, SystemColors.White, SystemColors.Transparent);
        }

        public override string GetObjectName()
        {
            return "RisingLava";
        }
    }
}
