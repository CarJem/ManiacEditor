using System;
using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class ZipLine : EntityRenderer
    {
        int[] LastValues { get; set; } = new int[4] { 0, 0, 0, 0 };
        int[] LastRotatePoints { get; set; } = new int[2] { 0, 0 };
        public override void Draw(Structures.EntityRenderProp Properties)
        {
            DevicePanel d = Properties.Graphics;

            Classes.Scene.EditorEntity e = Properties.EditorObject;
            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;

            int angle = (int)(e.attributesMap["angle"].ValueInt32);
            int length = (int)(e.attributesMap["length"].ValueEnum/1.4);
            bool fliph = false;
            bool flipv = false;

            var editorAnim = LoadAnimation("GHZ/ZipLine.bin", d, 0, 0);
            DrawTexturePivotNormal(d, editorAnim, editorAnim.RequestedAnimID, editorAnim.RequestedFrameID, x, y, Transparency);
            var editorAnim2 = LoadAnimation("GHZ/ZipLine.bin", d, 0, 1);

            if (AreValuesDiffrent(x + length, y + length, x, y, -angle + 32))
            {
                LastRotatePoints = RotatePoints(x + length, y + length, x, y, -angle + 32);
            }

            if (length != 0)
            {
                d.DrawLine(x, y, LastRotatePoints[0], LastRotatePoints[1], System.Drawing.Color.FromArgb(255, 49, 48, 115));
                d.DrawLine(x, y - 1, LastRotatePoints[0], LastRotatePoints[1] - 1, System.Drawing.Color.FromArgb(255, 99, 97, 165));
                DrawTexturePivotNormal(d, editorAnim2, editorAnim2.RequestedAnimID, editorAnim2.RequestedFrameID, LastRotatePoints[0], LastRotatePoints[1], Transparency);
            }

        }

        private bool AreValuesDiffrent(int initX, int initY, int centerX, int centerY, int angle)
        {
            int[] CurrentValues = new int[] { initX, initY, centerX, centerY, angle };
            if (LastValues != CurrentValues)
            {
                LastValues = CurrentValues;
                return true;
            }
            else return false;
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
