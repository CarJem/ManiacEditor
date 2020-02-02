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

        public override void Draw(Classes.Core.Draw.GraphicsHandler d, SceneEntity entity, Classes.Core.Scene.Sets.EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            int length = (int)entity.attributesMap["length"].ValueEnum;
            int angle = (int)entity.attributesMap["angle"].ValueEnum;
            bool fliph = false;
            bool flipv = false;
            var editorAnim = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("PaperRoller", d.DevicePanel, 0, -1, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];
                int[] newPos = RotatePoints(x - (length / 2), y, x, y, -angle);
                int[] newPos2 = RotatePoints(x + (length / 2), y, x, y, -angle);


                if (length >= 1)
                {
                    System.Drawing.Color[] RollerColors = new System.Drawing.Color[4];
                    Animation.ProcessPaperRollerRotatingColors();

                        switch (Animation.PaperRollerIndex)
                        {
                            case 0:
                                RollerColors[0] = color1default;
                                RollerColors[1] = color2default;
                                RollerColors[2] = color3default;
                                RollerColors[3] = color4default;
                                break;
                            case 1:
                                RollerColors[1] = color1default;
                                RollerColors[2] = color2default;
                                RollerColors[3] = color3default;
                                RollerColors[0] = color4default;
                                break;
                            case 2:
                                RollerColors[2] = color1default;
                                RollerColors[3] = color2default;
                                RollerColors[0] = color3default;
                                RollerColors[1] = color4default;
                                break;
                            case 3:
                                RollerColors[3] = color1default;
                                RollerColors[0] = color2default;
                                RollerColors[1] = color3default;
                                RollerColors[2] = color4default;
                                break;
                        }


                    int[] newPosAngle = RotatePoints(newPos[0] - 21, newPos[1], newPos[0], newPos[1], -angle + 64);
                    int[] newPosAngle2 = RotatePoints(newPos[0] + 21, newPos[1], newPos[0], newPos[1], -angle + 64);
                    int[] newPos2Angle = RotatePoints(newPos2[0] - 21, newPos2[1], newPos2[0], newPos2[1], -angle + 64);
                    int[] newPos2Angle2 = RotatePoints(newPos2[0] + 21, newPos2[1], newPos2[0], newPos2[1], -angle + 64);

                    d.DrawLine(newPosAngle[0], newPosAngle[1], newPos2Angle[0], newPos2Angle[1], RollerColors[0]);
                    d.DrawLine(newPosAngle2[0], newPosAngle2[1], newPos2Angle2[0], newPos2Angle2[1], RollerColors[0]);

                    newPosAngle = RotatePoints(newPos[0] - 22, newPos[1], newPos[0], newPos[1], -angle + 64);
                    newPosAngle2 = RotatePoints(newPos[0] + 22, newPos[1], newPos[0], newPos[1], -angle + 64);
                    newPos2Angle = RotatePoints(newPos2[0] - 22, newPos2[1], newPos2[0], newPos2[1], -angle + 64);
                    newPos2Angle2 = RotatePoints(newPos2[0] + 22, newPos2[1], newPos2[0], newPos2[1], -angle + 64);
                    d.DrawLinePaperRoller(newPosAngle[0], newPosAngle[1], newPos2Angle[0], newPos2Angle[1], RollerColors[0], RollerColors[1], RollerColors[2], RollerColors[3]);
                    d.DrawLinePaperRoller(newPosAngle2[0], newPosAngle2[1], newPos2Angle2[0], newPos2Angle2[1], RollerColors[0], RollerColors[1], RollerColors[2], RollerColors[3]);



                }

                Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);


                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame),
                    newPos[0] + frame.Frame.PivotX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                    newPos[1] + frame.Frame.PivotY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);

                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame),
                    newPos2[0] + frame.Frame.PivotX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                    newPos2[1] + frame.Frame.PivotY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);



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
