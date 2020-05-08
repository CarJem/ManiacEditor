using RSDKv5;
using SystemColors = System.Drawing.Color;
using System.Linq;

namespace ManiacEditor.Entity_Renders
{
    public class BGSwitch : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp Properties)
        {
            DevicePanel d = Properties.Graphics;
            Classes.Scene.EditorEntity e = Properties.EditorObject;
            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;

            var Animation = Methods.Draw.ObjectDrawing.LoadAnimation(Properties.Graphics, "EditorIcons");
            DrawTexturePivotNormal(d, Animation, 0, 5, x, y, Transparency);


            var widthPixels = (int)(e.attributesMap["size"].ValueVector2.X.High) * 2;
            var heightPixels = (int)(e.attributesMap["size"].ValueVector2.Y.High) * 2;
            var width = (int)widthPixels / 16;
            var height = (int)heightPixels / 16;

            DrawBounds(d, x, y, widthPixels, heightPixels, Transparency, SystemColors.White, SystemColors.Transparent);
        }

        public override bool isObjectOnScreen(DevicePanel d, Classes.Scene.EditorEntity entity, int x, int y, int Transparency)
        {
            var widthPixels = (int)(entity.attributesMap["size"].ValueVector2.X.High) * 2;
            var heightPixels = (int)(entity.attributesMap["size"].ValueVector2.Y.High) * 2;
            return d.IsObjectOnScreen(x - widthPixels / 2, y - heightPixels / 2, widthPixels, heightPixels);
        }

        public override string GetObjectName()
        {
            return "BGSwitch";
        }
    }
}
