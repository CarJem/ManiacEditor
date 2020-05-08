using RSDKv5;
using SystemColors = System.Drawing.Color;

namespace ManiacEditor.Entity_Renders
{
    public class PetalPile : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp Properties)
        {
            DevicePanel d = Properties.Graphics;

            Classes.Scene.EditorEntity e = Properties.EditorObject;
            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;

            var widthPixels = (int)(e.attributesMap["pileSize"].ValueVector2.X.High);
            var heightPixels = (int)(e.attributesMap["pileSize"].ValueVector2.Y.High);
            var width = (int)widthPixels;
            var height = (int)heightPixels;

            DrawBounds(d, x, y, width, height, Transparency, SystemColors.White, SystemColors.Transparent);


        }

        public override string GetObjectName()
        {
            return "PetalPile";
        }
    }
}
