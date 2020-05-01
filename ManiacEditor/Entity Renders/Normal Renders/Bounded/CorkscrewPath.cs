using RSDKv5;
using SystemColors = System.Drawing.Color;
using System;
using System.Drawing;

namespace ManiacEditor.Entity_Renders
{
    public class CorkscrewPath : EntityRenderer
    {
        Point[] LastSineWave { get; set; } = new Point[0];
        int[] LastValues { get; set; } = new int[4] { 0, 0, 0, 0 };

        public override void Draw(Structures.EntityRenderProp Properties)
        {
            DevicePanel d = Properties.Graphics;

            Classes.Scene.EditorEntity e = Properties.EditorObject;
            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;

            var period = (int)(e.attributesMap["period"].ValueEnum);
            var amplitude = (int)(e.attributesMap["amplitude"].ValueEnum * 3.5);
            var Animation = LoadAnimation("EditorIcons", d, 0, 4);

            if (RequireUpdate(x, y, amplitude, period))
            {
                LastSineWave = GetSineWave(x - (period / 2), y - (amplitude / 2), period, amplitude);
            }
            DrawSineWave(d, LastSineWave);


            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, false, false);
            DrawBounds(d, x, y, period, amplitude, Transparency, SystemColors.White, SystemColors.Transparent);
        }
        public bool RequireUpdate(int ix, int iy, int period, int amplitude)
        {
            int[] currentValues = new int[4] { ix, iy, period, amplitude };
            if (currentValues != LastValues)
            {
                LastValues = currentValues;
                return true;
            }
            else
            {
                return false;
            }
        }
        public Point[] GetSineWave(int ix, int iy, int period, int amplitude)
        {

            Point[] sineWave = new Point[period];

            ix = ix - (period / 4);
            int periodOffset = (period / 4);
            int i = 0;
            for (int x = periodOffset; x < period + periodOffset; x++)
            {
                int y = (int)((Math.Sin((double)x * 2.0 * Math.PI / period) + 1.0) * (amplitude - 1) / 2.0);
                sineWave[i] = new Point(ix + x, iy + y);
                i++;
            }
            return sineWave;
        }
        public void DrawSineWave(DevicePanel d, Point[] points)
        {
            if (points == null || points.Length == 0) return;
            var lastPoint = points[0];
            for (int i = 1; i < points.Length; i++)
            {
                d.DrawLine(lastPoint.X, lastPoint.Y, points[i].X, points[i].Y, SystemColors.GreenYellow, 4);
                lastPoint = points[i];
            }
        }

        public override string GetObjectName()
        {
            return "CorkscrewPath";
        }
    }
}
