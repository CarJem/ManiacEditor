﻿using System;
using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class CircleBumper : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp Properties)
        {
            DevicePanel d = Properties.Graphics;
            
            Classes.Scene.EditorEntity e = Properties.EditorObject;
            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;

            int type = (int)e.attributesMap["type"].ValueEnum;
            int speed = (int)e.attributesMap["speed"].ValueEnum;
            int angle = (int)e.attributesMap["angle"].ValueInt32;
            int amplitudeX = (int)e.attributesMap["amplitude"].ValueVector2.X.High;
            int amplitudeY = (int)e.attributesMap["amplitude"].ValueVector2.Y.High; 
            bool fliph = false;
            bool flipv = false;

            var Animation = LoadAnimation(GetSetupAnimation(), d, 0, 0);
            if (type == 1) DrawCircleBumperAlt(d, Animation, x, y, Transparency, amplitudeX, amplitudeY, angle);
            else DrawCircleBumperNormal(d, Animation, x, y, Transparency, amplitudeX, amplitudeY, angle);
        }

        private void DrawCircleBumperNormal(DevicePanel d, Methods.Drawing.ObjectDrawing.EditorAnimation Animation, int x, int y, int Transparency, int amplitudeX, int amplitudeY, int angle)
        {
            if ((amplitudeX != 0 || amplitudeY != 0))
            {
                double xd = x;
                double yd = y;
                double x2 = x + amplitudeX - amplitudeX / 3.7;
                double y2 = y + amplitudeY - amplitudeY / 3.7;
                double radius = Math.Pow(x2 - xd, 2) + Math.Pow(y2 - yd, 2);
                int radiusInt = (int)Math.Sqrt(radius);
                int newX = (int)(radiusInt * Math.Cos(Math.PI * angle / 128));
                int newY = (int)(radiusInt * Math.Sin(Math.PI * angle / 128));

                d.DrawLine(x, y, x + newX, y - newY, System.Drawing.Color.Yellow, 3);
                d.DrawEllipse(x, y, amplitudeX, amplitudeY, System.Drawing.Color.White, 2);

                DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, (x + newX), (y - newY), Transparency);
            }
            else DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency);
        }
        private void DrawCircleBumperAlt(DevicePanel d, Methods.Drawing.ObjectDrawing.EditorAnimation Animation, int x, int y, int Transparency, int amplitudeX, int amplitudeY, int angle)
        {
            int[] position = new int[2] { 0, 0 };
            int posX = amplitudeX;
            int posY = amplitudeY;
            //Negative Values only work atm
            if (amplitudeX <= -1) posX = -posX;
            if (amplitudeY <= -1) posY = -posY;

            position = new int[2] { posX, posY };

            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, (x + position[0]), (y - position[0]), Transparency);
        }

        public override string GetSetupAnimation()
        {
            string BinName = "/CircleBumper.bin";
            string UnlockName = "CircleBumper";

            if (Methods.Solution.CurrentSolution.IZ_Stage != null && Methods.Solution.CurrentSolution.IZ_Stage.Unlocks != null)
            {
                var unlocks = Methods.Solution.CurrentSolution.IZ_Stage.Unlocks;

                if (unlocks.Contains("SPZ2_" + UnlockName)) return "SPZ2" + BinName;
                else if (unlocks.Contains("SPZ1_" + UnlockName)) return "SPZ1" + BinName;
            }

            string SetupType = Methods.Solution.CurrentSolution.Entities.SetupObject.Replace("Setup", "");
            if (SetupType == "SPZ2") return "SPZ2" + BinName;
            else return "SPZ1" + BinName;
        }

        public override string GetObjectName()
        {
            return "CircleBumper";
        }
    }
}
