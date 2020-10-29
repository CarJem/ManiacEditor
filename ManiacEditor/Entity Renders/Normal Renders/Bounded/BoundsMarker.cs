using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class BoundsMarker : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp Properties)
        {
            DevicePanel d = Properties.Graphics;
            Classes.Scene.EditorEntity e = Properties.EditorObject;



            int type = (int)(e.attributesMap["type"].ValueUInt8);
            int width = (int)(e.attributesMap["width"].ValueEnum);
            int offset = (int)(e.attributesMap["offset"].ValueEnum);

            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;

            int line_x1 = x - width / 2;
            int line_x2 = x + width / 2;
            int line_y1 = (y - 2);
            int line_y2 = (y + 1);

            d.DrawRectangle(line_x1, line_y1, line_x2, line_y2, System.Drawing.Color.FromArgb(Transparency, System.Drawing.Color.White), System.Drawing.Color.FromArgb(Transparency, System.Drawing.Color.Black), 1);

            var Animation = Methods.Drawing.ObjectDrawing.LoadAnimation(Properties.Graphics, "EditorIcons", 0, 2);

            if (type == 0)
            {
                Animation = Methods.Drawing.ObjectDrawing.LoadAnimation(Properties.Graphics, "EditorIcons2", 0, 23);
            }
            else if (type == 1)
            {
                Animation = Methods.Drawing.ObjectDrawing.LoadAnimation(Properties.Graphics, "EditorIcons2", 0, 21);
            }
            else if (type == 2)
            {
                Animation = Methods.Drawing.ObjectDrawing.LoadAnimation(Properties.Graphics, "EditorIcons2", 0, 22);
            }

            DrawTexturePivotNormal(Properties.Graphics, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency);

        }

        public override string GetObjectName()
        {
            return "BoundsMarker";
        }
    }
}
