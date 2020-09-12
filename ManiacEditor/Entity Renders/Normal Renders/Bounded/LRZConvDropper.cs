using RSDKv5;
using SystemColors = System.Drawing.Color;

namespace ManiacEditor.Entity_Renders
{
    public class LRZConvDropper : EntityRenderer
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
            var Animation = LoadAnimation("LRZ2/LRZConvDropper.bin", d, 0, 0);

            var width = (int)(e.attributesMap["detectSize"].ValueVector2.X.High - 1);
            var height = (int)(e.attributesMap["detectSize"].ValueVector2.Y.High - 1);
            var offsetX = (int)(e.attributesMap["detectOffset"].ValueVector2.X.High - 1);
            var offsetY = (int)(e.attributesMap["detectOffset"].ValueVector2.Y.High - 1);

            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);

            DrawBounds(d, x + offsetX, y + offsetY, width, height, Transparency, SystemColors.White, SystemColors.Transparent);
        }

        public override bool isObjectOnScreen(DevicePanel d, Classes.Scene.EditorEntity entity, int x, int y, int Transparency)
        {
            var widthPixels = (int)(entity.attributesMap["detectSize"].ValueVector2.X.High - 1) / 16;
            var heightPixels = (int)(entity.attributesMap["detectSize"].ValueVector2.Y.High - 1) / 16;
            var offsetX = (int)(entity.attributesMap["detectOffset"].ValueVector2.X.High - 1) / 16;
            var offsetY = (int)(entity.attributesMap["detectOffset"].ValueVector2.Y.High - 1) / 16;

            x += offsetX;
            y += offsetY;
            return d.IsObjectOnScreen(x - widthPixels / 2, y - heightPixels / 2, widthPixels + 15, heightPixels + 15);
        }

        public override string GetObjectName()
        {
            return "LRZConvDropper";
        }
    }
}
