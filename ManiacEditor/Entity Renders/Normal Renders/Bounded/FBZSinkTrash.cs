using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class FBZSinkTrash : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp Properties)
        {
            DevicePanel d = Properties.Graphics;

            Classes.Scene.EditorEntity e = Properties.EditorObject;
            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;

            var type = e.attributesMap["type"].ValueEnum;
            var widthPixels = (int)(e.attributesMap["size"].ValueVector2.X.High);
            var heightPixels = (int)(e.attributesMap["size"].ValueVector2.Y.High);

            if (widthPixels >= 1 && heightPixels >= 1)
            {
                int x1 = x - widthPixels / 2;
                int y1 = y - heightPixels / 2;
                int x2 = x + widthPixels / 2;
                int y2 = y + heightPixels / 2;


                DrawBounds2(d, x2, y2, x1, y1, Transparency, System.Drawing.Color.White, System.Drawing.Color.Gray);
            }
        }

        public override string GetObjectName()
        {
            return "FBZSinkTrash";
        }
    }
}
