using System;
using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class TetherBall : EntityRenderer
    {
        //TODO: Get the Angle Calculations Correct
        public override void Draw(Structures.EntityRenderProp Properties)
        {
            DevicePanel d = Properties.Graphics;

            Classes.Scene.EditorEntity e = Properties.EditorObject;
            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;

            bool fliph = false;
            bool flipv = false;

            int type = (int)e.attributesMap["type"].ValueUInt8;
            double angleStart = e.attributesMap["angleStart"].ValueEnum; //Because they used values over the int limit
            double angleEnd = e.attributesMap["angleEnd"].ValueEnum; //Because they used values over the int limit
            int chainCount = (int)e.attributesMap["chainCount"].ValueUInt8;
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

            var Animation = LoadAnimation("TetherBall", d, 0, 0);
            double angleStartInt = (-angleStart / 4);

            // TetherBall Line
                
            for (int i = 0; i < chainCount; i++)
            {
                Animation = LoadAnimation("TetherBall", d, 0, 2);
                int x_alt = x + 6;
                int[] linePoints = RotatePoints(x_alt + (Animation.RequestedFrame.Width) * i, y, x, y, angleStartInt);
                DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, linePoints[0], linePoints[1], Transparency, fliph, flipv);
            }



            //TetherBall Ball
            int length = (16 * chainCount) + 16;
            int[] processPoints;
            processPoints = RotatePoints(x + length, y, x, y, angleStartInt);

            Animation = LoadAnimation("TetherBall", d, 0, 3);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, processPoints[0], processPoints[1], Transparency, fliph, flipv);

            // TetherBall Center
            if (drawType == true) 
            {
                Animation = LoadAnimation("TetherBall", d, 0, animID);
                DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
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
