using System;
using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class PlaneSwitch : EntityRenderer
    {
        public override void Draw(Structures.EntityRenderProp properties)
        {
            Classes.Editor.Draw.GraphicsHandler d = properties.Graphics;
            SceneEntity entity = properties.Object; 
            Classes.Editor.Scene.Sets.EditorEntity e = properties.EditorObject;
            int x = properties.X;
            int y = properties.Y;
            int Transparency = properties.Transparency;
            int index = properties.Index;
            int previousChildCount = properties.PreviousChildCount;
            int platformAngle = properties.PlatformAngle;
            Methods.Entities.EntityAnimator Animation = properties.Animations;
            bool selected  = properties.isSelected;
            const int LeftDist = 1,
                      LeftPlane = 2,
                      RightDist = 4,
                      RightPlane = 8;

            var flags = (int)entity.attributesMap["flags"].ValueEnum;
            var size = (int)(entity.attributesMap["size"].ValueEnum) - 1;
            var angle = entity.attributesMap["angle"].ValueInt32;

            int frameDist = (flags & LeftDist) > 0 ? 1 : 0;
            int framePlane = (flags & LeftPlane) > 0 ? 2 : 0;
            var editorAnim = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("PlaneSwitch", d.DevicePanel, 0, frameDist + framePlane, false, false, false);

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
                        x - frame.Frame.Width,
                        (y + (hEven ? frame.Frame.PivotY : -frame.Frame.Height) + (-size / 2 + yy) * frame.Frame.Height),
                        x + pivotOffsetX, y + pivotOffsetY, angle);

                    d.DrawBitmap(new Classes.Editor.Draw.GraphicsHandler.GraphicsInfo(frame), drawCoords[0] + drawOffsetX, drawCoords[1] + drawOffsetY, frame.Frame.Width, frame.Frame.Height, false, Transparency);
                }
            }

            frameDist = (flags & RightDist) > 0 ? 1 : 0;
            framePlane = (flags & RightPlane) > 0 ? 2 : 0;
            editorAnim = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("PlaneSwitch", d.DevicePanel, 0, frameDist + framePlane, false, false, false);

            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];
                Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);
                bool hEven = size % 2 == 0;
                for (int yy = 0; yy <= size; ++yy)
                {
                    int[] drawCoords = RotatePoints(
                        x,
                        (y + (hEven ? frame.Frame.PivotY : -frame.Frame.Height) + (-size / 2 + yy) * frame.Frame.Height),
                        x + pivotOffsetX, y + pivotOffsetY, angle);

                    d.DrawBitmap(new Classes.Editor.Draw.GraphicsHandler.GraphicsInfo(frame), drawCoords[0] + drawOffsetX, drawCoords[1] + drawOffsetY, frame.Frame.Width, frame.Frame.Height, false, Transparency);
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
        public override bool isObjectOnScreen(Classes.Editor.Draw.GraphicsHandler d, SceneEntity entity, Classes.Editor.Scene.Sets.EditorEntity e, int x, int y, int Transparency)
        {
            var size = (int)(entity.attributesMap["size"].ValueEnum);
            int bounds = (16 * size);

            return d.IsObjectOnScreen(x - bounds, y - bounds, bounds*2, bounds*2);
        }

        public override string GetObjectName()
        {
            return "PlaneSwitch";
        }
    }
}
