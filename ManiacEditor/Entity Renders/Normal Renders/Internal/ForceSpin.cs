using System;
using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class ForceSpin : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp Properties)
        {
            Classes.Scene.EditorEntity e = Properties.EditorObject;
            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;

            var size = (int)(e.attributesMap["size"].ValueEnum) - 1;
            var angle = e.attributesMap["angle"].ValueInt32;

            var editorAnim = LoadAnimation("PlaneSwitch", Properties.Graphics, 0, 4);

            const int pivotOffsetX = -8, pivotOffsetY = 0;
            const int drawOffsetX = 0, drawOffsetY = -8;

            bool hEven = size % 2 == 0;
            for (int yy = 0; yy <= size; ++yy)
            {
                int[] drawCoords = RotatePoints(
                    x - editorAnim.RequestedFrame.Width / 2,
                    (y + (hEven ? editorAnim.RequestedFrame.PivotY : -editorAnim.RequestedFrame.Height) + (-size / 2 + yy) * editorAnim.RequestedFrame.Height),
                    x + pivotOffsetX, y + pivotOffsetY, angle);

                DrawTexture(Properties.Graphics, editorAnim, editorAnim.RequestedAnimID, editorAnim.RequestedFrameID, drawCoords[0] + drawOffsetX, drawCoords[1] + drawOffsetY, Transparency);
            }
        }

        private static int[] RotatePoints(double initX, double initY, double centerX, double centerY, int angle)
        {
            initX -= centerX;
            initY -= centerY;

            if (initX == 0 && initY == 0)
            {
                int[] results2 = { (int)centerX, (int)centerY };
                return results2;
            }

            const double FACTOR = 40.743665431525205956834243423364;

            double hypo = Math.Sqrt(Math.Pow(initX, 2) + Math.Pow(initY, 2));
            double initAngle = Math.Acos(initX / hypo);
            if (initY < 0) initAngle = 2 * Math.PI - initAngle;
            double newAngle = initAngle - angle / FACTOR;
            double finalX = hypo * Math.Cos(newAngle) + centerX;
            double finalY = hypo * Math.Sin(newAngle) + centerY;

            int[] results = { (int)Math.Round(finalX), (int)Math.Round(finalY) };
            return results;
        }

        public override string GetObjectName()
        {
            return "ForceSpin";
        }
    }
}
