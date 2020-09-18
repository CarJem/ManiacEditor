using System;
using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class YoyoPulley : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp properties)
        {
            DevicePanel d = properties.Graphics;
            Classes.Scene.EditorEntity entity = properties.EditorObject;
            int x = properties.DrawX;
            int y = properties.DrawY;
            int Transparency = properties.Transparency;
            int direction = (int)entity.attributesMap["direction"].ValueUInt8;
            int pullDir = (int)entity.attributesMap["pullDir"].ValueUInt8;
            int length = (int)(entity.attributesMap["length"].ValueEnum);
            int angle = (int)(entity.attributesMap["angle"].ValueEnum);
            bool flipSides = false;
            if (direction == 0) flipSides = true;



            x -= (flipSides ? -18 : 18);

            //y += 26;
            int[] processPoints = RotatePoints(x + length + 26, y, x, y, (int)(angle / -1.995));

            d.DrawLine(x + 2, y, processPoints[0] + 2, processPoints[1], System.Drawing.Color.FromArgb(255, 0, 32, 0));
            d.DrawLine(x + 1, y, processPoints[0] + 1, processPoints[1], System.Drawing.Color.FromArgb(255, 198, 32, 0));
            d.DrawLine(x, y, processPoints[0], processPoints[1], System.Drawing.Color.FromArgb(255, 231, 130, 0));
            d.DrawLine(x - 1, y, processPoints[0] - 1, processPoints[1], System.Drawing.Color.FromArgb(255, 198, 32, 0));
            d.DrawLine(x - 2, y, processPoints[0] - 2, processPoints[1], System.Drawing.Color.FromArgb(255, 0, 32, 0));

            var editorAnimHandle = LoadAnimation("SSZ1/SDashWheel.bin", d, 3, 0);
            DrawTexturePivotPlus(d, editorAnimHandle, editorAnimHandle.RequestedAnimID, editorAnimHandle.RequestedFrameID, processPoints[0], processPoints[1], -(flipSides ? -5 : 5), (flipSides ? 10 : -10), Transparency, false, false, angle);

            var editorAnim = LoadAnimation("SSZ1/SDashWheel.bin", d, 0, 0);
            DrawTexturePivotNormal(d, editorAnim, editorAnim.RequestedAnimID, editorAnim.RequestedFrameID, x, y, Transparency);
            var editorAnimKnob = LoadAnimation("SSZ1/SDashWheel.bin", d, 2, 0);
            DrawTexturePivotNormal(d, editorAnimKnob, editorAnimKnob.RequestedAnimID, editorAnimKnob.RequestedFrameID, x, y, Transparency);
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
            return "YoyoPulley";
        }
    }
}
