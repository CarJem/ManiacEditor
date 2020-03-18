using System;
using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class ZipLine : EntityRenderer
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
            int angle = (int)(entity.attributesMap["angle"].ValueInt32);
            int length = (int)(entity.attributesMap["length"].ValueEnum/1.4);
            bool fliph = false;
            bool flipv = false;
            var editorAnim = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("ZipLine", d.DevicePanel, 0, -1, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];
                var frame2 = editorAnim.Frames[1];


                d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame),
                    x + frame.Frame.PivotX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                    y + frame.Frame.PivotY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
                d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame2),
                    x + frame2.Frame.PivotX - (fliph ? (frame2.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                    y + frame2.Frame.PivotY + (flipv ? (frame2.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                    frame2.Frame.Width, frame2.Frame.Height, false, Transparency);

                if (length != 0)
                {
                    int[] processPoints = RotatePoints(x + length, y + length, x, y, -angle + 32);
                    d.DrawLine(x, y, processPoints[0], processPoints[1], System.Drawing.Color.FromArgb(255, 49, 48, 115));
                    d.DrawLine(x, y-1, processPoints[0], processPoints[1] - 1, System.Drawing.Color.FromArgb(255, 99, 97, 165));
                    d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame2),
                    processPoints[0] + frame2.Frame.PivotX - (fliph ? (frame2.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0) - 2,
                    processPoints[1] + frame2.Frame.PivotY + (flipv ? (frame2.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0) - 1,
                    frame2.Frame.Width, frame2.Frame.Height, false, Transparency);
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
            return "ZipLine";
        }
    }
}
