using System;
using System.Drawing;
using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class PaperRoller : EntityRenderer
    {
        static System.Drawing.Color color1default = ColorTranslator.FromHtml("#F7F3EF");
        static System.Drawing.Color color2default = ColorTranslator.FromHtml("#D6BA9C");
        static System.Drawing.Color color3default = ColorTranslator.FromHtml("#9C7973");
        static System.Drawing.Color color4default = ColorTranslator.FromHtml("#5A696B");

        public override void Draw(Structures.EntityRenderProp Properties)
        {
            DevicePanel d = Properties.Graphics;

            Classes.Scene.EditorEntity e = Properties.EditorObject;
            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;

            bool fliph = false;
            bool flipv = false;

            int length = (int)e.attributesMap["length"].ValueEnum;
            int angle = (int)e.attributesMap["angle"].ValueEnum;

            var Animation = LoadAnimation("PaperRoller", d, 0, 0);

            int[] newPos = RotatePoints(x - (length / 2), y, x, y, -angle);
            int[] newPos2 = RotatePoints(x + (length / 2), y, x, y, -angle);


            if (length >= 1)
            {
                int[] newPosAngle = RotatePoints(newPos[0] - 21, newPos[1], newPos[0], newPos[1], -angle + 64);
                int[] newPosAngle2 = RotatePoints(newPos[0] + 21, newPos[1], newPos[0], newPos[1], -angle + 64);
                int[] newPos2Angle = RotatePoints(newPos2[0] - 21, newPos2[1], newPos2[0], newPos2[1], -angle + 64);
                int[] newPos2Angle2 = RotatePoints(newPos2[0] + 21, newPos2[1], newPos2[0], newPos2[1], -angle + 64);

                d.DrawLine(newPosAngle[0], newPosAngle[1], newPos2Angle[0], newPos2Angle[1], color1default, 2);
                d.DrawLine(newPosAngle2[0], newPosAngle2[1], newPos2Angle2[0], newPos2Angle2[1], color1default, 2);

                newPosAngle = RotatePoints(newPos[0] - 22, newPos[1], newPos[0], newPos[1], -angle + 64);
                newPosAngle2 = RotatePoints(newPos[0] + 22, newPos[1], newPos[0], newPos[1], -angle + 64);
                newPos2Angle = RotatePoints(newPos2[0] - 22, newPos2[1], newPos2[0], newPos2[1], -angle + 64);
                newPos2Angle2 = RotatePoints(newPos2[0] + 22, newPos2[1], newPos2[0], newPos2[1], -angle + 64);
                d.DrawLine(newPosAngle[0], newPosAngle[1], newPos2Angle[0], newPos2Angle[1], color2default, 2);
                d.DrawLine(newPosAngle2[0], newPosAngle2[1], newPos2Angle2[0], newPos2Angle2[1], color2default, 2);
                //d.DrawDashedLine(newPosAngle[0], newPosAngle[1], newPos2Angle[0], newPos2Angle[1], RollerColors[0], RollerColors[1], RollerColors[2], RollerColors[3], 2);
                //d.DrawDashedLine(newPosAngle2[0], newPosAngle2[1], newPos2Angle2[0], newPos2Angle2[1], RollerColors[0], RollerColors[1], RollerColors[2], RollerColors[3], 2);
            }

            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, newPos[0], newPos[1], Transparency, fliph, flipv);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, newPos2[0], newPos2[1], Transparency, fliph, flipv);        
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
