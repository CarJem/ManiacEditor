using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class BoundsMarker : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp Properties)
        {
            DevicePanel d = Properties.Graphics;
            Classes.Scene.EditorEntity e = Properties.EditorObject;
            
            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;

            int width = (int)(e.attributesMap["width"].ValueEnum);
            int offset = (int)(e.attributesMap["offset"].ValueEnum);

            int line_x1 = x - width / 2;
            int line_x2 = x + width / 2;
            int line_y1 = (y - 1);
            int line_y2 = (y - 1) + 1;


            //d.DrawRectangle(line_x1, line_y1, line_x2, line_y2, System.Drawing.Color.White, System.Drawing.Color.Black, 1);

            var Animation = Methods.Drawing.ObjectDrawing.LoadAnimation(Properties.Graphics, "EditorIcons", 0, 2);
            DrawTexturePivotNormal(Properties.Graphics, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency);

        }

        public override string GetObjectName()
        {
            return "BoundsMarker";
        }
    }
}
