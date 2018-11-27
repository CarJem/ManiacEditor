using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ManiacEditor;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class TetherBall : EntityRenderer
    {
        //TODO: Get the Angle Calculations Correct
        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            int type = (int)entity.attributesMap["type"].ValueUInt8;
            double angleStart = entity.attributesMap["angleStart"].ValueVar; //Because they used values over the int limit
            double angleEnd = entity.attributesMap["angleEnd"].ValueVar; //Because they used values over the int limit
            int chainCount = (int)entity.attributesMap["chainCount"].ValueUInt8;
            bool fliph = false;
            bool flipv = false;
            bool drawType = true;


            // For anything bigger than the average rotation (there are a couple of instances of this in Mania that are used by the devs)
            if (angleStart >= 1024 || angleEnd >= 1024)
            {   
                    if (angleStart >= 1024)
                    {
                        angleStart = angleStart % 1024;
                    }
                    if (angleEnd >= 1024)
                    {
                        angleEnd = angleEnd % 1024;
                    }
            }


            int animID;
            switch (type)
            {
                case 0:
                    animID = 0;
                    break;
                case 1:
                    animID = 0;
                    flipv = true;
                    break;
                case 2:
                    animID = 1;
                    break;
                case 3:
                    animID = 1;
                    flipv = true;
                    break;
                default:
                    animID = 0;
                    drawType = false;
                    break;

            }

            var editorAnim = EditorEntity_ini.LoadAnimation2("TetherBall", d, 0, animID, fliph, flipv, false);
            var editorAnim2 = EditorEntity_ini.LoadAnimation2("TetherBall", d, 0, 2, fliph, flipv, false);
            var editorAnim3 = EditorEntity_ini.LoadAnimation2("TetherBall", d, 0, 3, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnim2 != null && editorAnim2.Frames.Count != 0 && editorAnim3 != null && editorAnim3.Frames.Count != 0)
            {

                var frame = editorAnim.Frames[Animation.index];
                var frame2 = editorAnim2.Frames[Animation.index];
                var frame3 = editorAnim3.Frames[Animation.index];

                double angleStartInt = (-angleStart / 4);

                // TetherBall Line
                
                for (int i = 0; i < chainCount; i++)
                {
                    int x_alt = x + 6;
                    int[] linePoints = RotatePoints(x_alt + (frame2.Frame.Width) * i, y, x, y, angleStartInt);
                    
                    d.DrawBitmap(frame2.Texture,
                        linePoints[0] + frame2.Frame.CenterX,
                        linePoints[1] + frame2.Frame.CenterY,
                        frame2.Frame.Width, frame2.Frame.Height, false, Transparency);
                }



                //TetherBall Ball
                int length = (16 * chainCount) + 16;
                int[] processPoints;
                processPoints = RotatePoints(x + length, y, x, y, angleStartInt);

                d.DrawBitmap(frame3.Texture,
                    processPoints[0] + frame3.Frame.CenterX,
                    processPoints[1] + frame3.Frame.CenterY,
                    frame3.Frame.Width, frame3.Frame.Height, false, Transparency);

                // TetherBall Center
                if (drawType == true)
                {
                    d.DrawBitmap(frame.Texture,
                        x + frame.Frame.CenterX - (fliph ? (frame2.Frame.Width - editorAnim2.Frames[0].Frame.Width) : 0),
                        y + frame.Frame.CenterY + (flipv ? (frame2.Frame.Height - editorAnim2.Frames[0].Frame.Height) : 0),
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
                }

            }
        }
        private static int[] RotatePoints(double initX, double initY, double centerX, double centerY, double angle)
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
            return "TetherBall";
        }
    }
}
