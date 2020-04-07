using RSDKv5;
using SystemColors = System.Drawing.Color;


namespace ManiacEditor.Entity_Renders
{
    public class ShopWindow : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp Properties)
        {
            DevicePanel d = Properties.Graphics;

            Classes.Scene.EditorEntity e = Properties.EditorObject;
            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;
            var widthPixels = (int)(e.attributesMap["size"].ValueVector2.X.High) * 2;
            var heightPixels = (int)(e.attributesMap["size"].ValueVector2.Y.High) * 2;
            var width = (int)widthPixels / 16;
            var height = (int)heightPixels / 16;
            var Animation = LoadAnimation("EditorAssets", d, 0, 1);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, false, false);
            DrawBounds(d, x, y, widthPixels, heightPixels, Transparency, SystemColors.White, SystemColors.FromArgb(128, SystemColors.MediumPurple));
        }

        public override string GetObjectName()
        {
            return "ShopWindow";
        }
    }
}
