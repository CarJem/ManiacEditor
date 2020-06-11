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
            int line_y1 = (y - 1);

            if (type == 0 || type == 1) d.DrawLine(x, y, x, y - 32, System.Drawing.Color.FromArgb(Transparency, System.Drawing.Color.Black), 3);
            if (type == 0 || type == 2) d.DrawLine(x, y, x, y + 32, System.Drawing.Color.FromArgb(Transparency, System.Drawing.Color.Black), 3);
            if (type == 0 || type == 1) d.DrawLine(x, y, x, y - 31, System.Drawing.Color.FromArgb(Transparency, System.Drawing.Color.Yellow), 1);
            if (type == 0 || type == 2) d.DrawLine(x, y, x, y + 31, System.Drawing.Color.FromArgb(Transparency, (type == 0 ? System.Drawing.Color.Red : System.Drawing.Color.Yellow)), 1);

            d.DrawLine(line_x1, line_y1, line_x2, line_y1, System.Drawing.Color.FromArgb(Transparency, System.Drawing.Color.Black), 3);
            d.DrawLine(line_x1 + 1, line_y1, line_x2 - 1, line_y1, System.Drawing.Color.FromArgb(Transparency, System.Drawing.Color.White), 1);

            var Animation = Methods.Drawing.ObjectDrawing.LoadAnimation(Properties.Graphics, "EditorIcons", 0, 2);
            DrawTexturePivotNormal(Properties.Graphics, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency);

        }

        public override string GetObjectName()
        {
            return "BoundsMarker";
        }
    }
}
