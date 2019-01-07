using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ManiacEditor;
using Microsoft.Xna.Framework;
using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class YoyoPulley : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            int direction = (int)entity.attributesMap["direction"].ValueUInt8;
            int pullDir = (int)entity.attributesMap["pullDir"].ValueUInt8;
            int length = (int)(entity.attributesMap["length"].ValueVar);
            int angle = (int)(entity.attributesMap["angle"].ValueVar);
            bool fliph = false;
            bool flipSides = false;
            bool flipv = false;
            if (direction == 0)
            {
                flipSides = true;
            }
            var editorAnim = e.EditorInstance.EditorEntity_ini.LoadAnimation2("SDashWheel", d, 0, 0, fliph, flipv, false);
            var editorAnimKnob = e.EditorInstance.EditorEntity_ini.LoadAnimation2("SDashWheel", d, 2, 0, fliph, flipv, false);
            var editorAnimHandle = e.EditorInstance.EditorEntity_ini.LoadAnimation2("SDashWheel", d, 3, 0, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnimKnob != null && editorAnimKnob.Frames.Count != 0 && editorAnimHandle != null && editorAnimHandle.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                var frameKnob = editorAnimKnob.Frames[0];
                var frameHandle = editorAnimHandle.Frames[0];

                x -= (flipSides ? -18 : 18);

                //y += 26;
                int[] processPoints = RotatePoints(x + length + 26, y, x, y, (int)(angle / -1.995));

                d.DrawLine(x + 2, y, processPoints[0] + 2, processPoints[1], System.Drawing.Color.FromArgb(255, 0, 32, 0));
                d.DrawLine(x + 1, y, processPoints[0] + 1, processPoints[1], System.Drawing.Color.FromArgb(255, 198, 32, 0));
                d.DrawLine(x, y, processPoints[0], processPoints[1], System.Drawing.Color.FromArgb(255, 231, 130, 0));
                d.DrawLine(x - 1, y, processPoints[0] - 1, processPoints[1], System.Drawing.Color.FromArgb(255, 198, 32, 0));
                d.DrawLine(x - 2, y, processPoints[0] - 2, processPoints[1], System.Drawing.Color.FromArgb(255, 0, 32, 0));
                d.DrawBitmap(frameHandle.Texture,
                processPoints[0] + frameHandle.Frame.CenterX - (flipSides ? -5 : 5),
                processPoints[1] + frameHandle.Frame.CenterY + (flipSides ? 10 : -10),
                frameHandle.Frame.Width, frameHandle.Frame.Height, false, Transparency);

                d.DrawBitmap(frame.Texture,
                    x + frame.Frame.CenterX,
                    y + frame.Frame.CenterY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
                d.DrawBitmap(frameKnob.Texture,
                    x + frameKnob.Frame.CenterX,
                    y + frameKnob.Frame.CenterY,
                    frameKnob.Frame.Width, frameKnob.Frame.Height, false, Transparency);



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
            return "YoyoPulley";
        }
    }
}
