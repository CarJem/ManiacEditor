using System;
using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class ForceSpin : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp properties)
        {
            Methods.Draw.GraphicsHandler d = properties.Graphics;
            SceneEntity entity = properties.Object; 
            Classes.Scene.Sets.EditorEntity e = properties.EditorObject;
            int x = properties.X;
            int y = properties.Y;
            int Transparency = properties.Transparency;
            int index = properties.Index;
            int previousChildCount = properties.PreviousChildCount;
            int platformAngle = properties.PlatformAngle;
            Methods.Entities.EntityAnimator Animation = properties.Animations;
            bool selected  = properties.isSelected;

            var size = (int)(entity.attributesMap["size"].ValueEnum) - 1;
            var angle = entity.attributesMap["angle"].ValueInt32;

            var editorAnim = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("PlaneSwitch", d.DevicePanel, 0, 4, true, false, false);

            const int pivotOffsetX = -8, pivotOffsetY = 0;
            const int drawOffsetX = 0, drawOffsetY = -8;

            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];
                Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);
                bool hEven = size % 2 == 0;
                for (int yy = 0; yy <= size; ++yy)
                {
                    int[] drawCoords = RotatePoints(
                        x - frame.Frame.Width / 2,
                        (y + (hEven ? frame.Frame.PivotY : -frame.Frame.Height) + (-size / 2 + yy) * frame.Frame.Height),
                        x + pivotOffsetX, y + pivotOffsetY, angle);

                    d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame), drawCoords[0] + drawOffsetX, drawCoords[1] + drawOffsetY, frame.Frame.Width, frame.Frame.Height, false, Transparency);
                }
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
