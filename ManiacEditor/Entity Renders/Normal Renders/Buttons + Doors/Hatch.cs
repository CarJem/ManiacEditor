using RSDKv5;
using SystemColors = System.Drawing.Color;

namespace ManiacEditor.Entity_Renders
{
    public class Hatch : EntityRenderer
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

            var widthPixels = (int)(e.attributesMap["subOff2"].ValueVector2.X.High) * 16;
            var heightPixels = (int)(e.attributesMap["subOff2"].ValueVector2.Y.High) * 16;
            var width = (int)widthPixels / 16;
            var height = (int)heightPixels / 16;
            int offsetX = (int)(e.attributesMap["subOff1"].ValueVector2.X.High);
            int offsetY = (int)(e.attributesMap["subOff1"].ValueVector2.Y.High);

            var Animation = LoadAnimation("OOZ/Hatch.bin", d, 0, 1);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
            Animation = LoadAnimation("OOZ/Hatch.bin", d, 0, 0);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);

            DrawBounds(d, x + offsetX, y + offsetY, widthPixels, heightPixels, Transparency, SystemColors.White, SystemColors.Transparent);
        }

        public override string GetObjectName()
        {
            return "Hatch";
        }
    }
}
