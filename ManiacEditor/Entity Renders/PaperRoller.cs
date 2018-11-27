using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ManiacEditor;
using Microsoft.Xna.Framework;
using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class PaperRoller : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            int length = (int)entity.attributesMap["length"].ValueVar;
            int angle = (int)entity.attributesMap["angle"].ValueVar;
            bool fliph = false;
            bool flipv = false;
            var editorAnim = EditorEntity_ini.LoadAnimation2("PaperRoller", d, 0, -1, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];
                int[] newPos = RotatePoints(x - (length / 2), y, x, y, -angle);
                int[] newPos2 = RotatePoints(x + (length / 2), y, x, y, -angle);
                int[] newPosAngle = RotatePoints(newPos[0] - 23, newPos[1], newPos[0], newPos[1], -angle + 64);
                int[] newPosAngle2 = RotatePoints(newPos[0] + 24, newPos[1], newPos[0], newPos[1], -angle + 64);
                int[] newPos2Angle = RotatePoints(newPos2[0] - 23, newPos2[1], newPos2[0], newPos2[1], -angle + 64);
                int[] newPos2Angle2 = RotatePoints(newPos2[0] + 24, newPos2[1], newPos2[0], newPos2[1], -angle + 64);

                Animation.ProcessAnimation(frame.Entry.FrameSpeed, frame.Entry.Frames.Count, frame.Frame.Duration);


                d.DrawBitmap(frame.Texture,
                    newPos[0] + frame.Frame.CenterX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                    newPos[1] + frame.Frame.CenterY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);

                d.DrawBitmap(frame.Texture,
                    newPos2[0] + frame.Frame.CenterX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                    newPos2[1] + frame.Frame.CenterY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);

                if (length >= 1)
                {
                    d.DrawLine(newPosAngle[0], newPosAngle[1], newPos2Angle[0], newPos2Angle[1], System.Drawing.Color.LightGray);
                    d.DrawLine(newPosAngle2[0], newPosAngle2[1], newPos2Angle2[0], newPos2Angle2[1], System.Drawing.Color.LightGray);
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
            return "PaperRoller";
        }
    }
}
