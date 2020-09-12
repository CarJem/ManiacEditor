using ManiacEditor.Methods.Solution;
using RSDKv5;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ManiacEditor.Entity_Renders
{
    public class Platform : EntityRenderer
    {
        #region Definition
        private int LastFrameIDAttribute { get; set; }
        private int RealAnimID { get; set; }
        private int RealFrameID { get; set; }
        #endregion

        public Platform() : base()
        {
            RealAnimID = -1;
            RealFrameID = -1;
            LastFrameIDAttribute = -1;
        }


        #region Variants
        private void DrawNormalTensionPlatform(DevicePanel d, int x, int y, int Transparency, int amplitudeX, int amplitudeY, int angle, bool hasTension, int AttributeFrameID)
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
                int tensionCount = radiusInt / 16;

                d.DrawLine(x, y, x + newX, y - newY, System.Drawing.Color.Yellow, 3);
                d.DrawEllipse(x, y, amplitudeX, amplitudeY, System.Drawing.Color.White, 1);

                if (hasTension)
                {
                    for (int i = 0; i < tensionCount; i++)
                    {
                        int[] linePoints = RotatePoints(x + (16) * i, y, x, y, angle);
                        int currentX = linePoints[0];
                        int currentY = linePoints[1];
                        if (i == 0)
                        {
                            //Tension Center
                            var AnimationCenter = LoadAnimation(GetSetupAnimation(), d, 1, 2);
                            DrawTexturePivotNormal(d, AnimationCenter, AnimationCenter.RequestedAnimID, AnimationCenter.RequestedFrameID, currentX, currentY, Transparency);
                        }
                        else
                        {
                            //Tension Bar Segment
                            var AnimationRow = LoadAnimation(GetSetupAnimation(), d, 1, 1);
                            DrawTexturePivotNormal(d, AnimationRow, AnimationRow.RequestedAnimID, AnimationRow.RequestedFrameID, currentX, currentY, Transparency);
                        }
                    }
                }

                if (AttributeFrameID <= -1) return;
                string AnimName = GetSetupAnimation();
                var Animation = LoadAnimation(AnimName, d);
                if (LastFrameIDAttribute != AttributeFrameID) UpdateRealAttributeFrameID(Animation, AttributeFrameID);
                DrawTexturePivotNormal(d, Animation, RealAnimID, RealFrameID, x + newX, y - newY, Transparency);
                //DrawHitbox(d, Animation, GetSetupAnimation(), System.Drawing.Color.FromArgb(128, 0, 255, 0), RealAnimID, RealFrameID, x + newX, y - newY, Transparency, false, false, 0);
            }
        }


        private void DrawTensionBallPlatform(DevicePanel d, int x, int y, int Transparency, int amplitudeX, int amplitudeY, int angle, bool hasTension, int AttributeFrameID)
        {
            if ((amplitudeX != 0 || amplitudeY != 0))
            {
                int tensionCount = amplitudeY;
                int i = 0;
                int[] linePoints = RotatePoints(x, y + (16) * i, x, y, angle);
                for (i = 0; i <= tensionCount; i++)
                {
                    linePoints = RotatePoints(x, y + (16) * i, x, y, angle);
                    int currentX = linePoints[0];
                    int currentY = linePoints[1];
                    if (i == 0)
                    {
                        //Tension Center
                        var AnimationCenter = LoadAnimation(GetSetupAnimation(), d, 1, 2);
                        DrawTexturePivotNormal(d, AnimationCenter, AnimationCenter.RequestedAnimID, AnimationCenter.RequestedFrameID, currentX, currentY, Transparency);
                    }
                    else if (i == tensionCount)
                    {
                        //Tension Bar Segment
                        var AnimationRow = LoadAnimation(GetSetupAnimation(), d, 1, 0);
                        DrawTexturePivotNormal(d, AnimationRow, AnimationRow.RequestedAnimID, AnimationRow.RequestedFrameID, currentX, currentY, Transparency);
                    }
                    else
                    {
                        //Tension Ball
                        var Animation = LoadAnimation(GetSetupAnimation(), d, 1, 1);
                        DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, currentX, currentY, Transparency);
                    }
                }
            }
        }
        private void DrawStandardMovingPlatform(DevicePanel d, int x, int y, int Transparency, int amplitudeX, int amplitudeY, int angle, bool hasTension, int AttributeFrameID)
        {
            string AnimName = GetSetupAnimation();
            var Animation = LoadAnimation(AnimName, d);
            if (LastFrameIDAttribute != AttributeFrameID) UpdateRealAttributeFrameID(Animation, AttributeFrameID);
            d.DrawLine(x - amplitudeX, y - amplitudeY, x + amplitudeX, y + amplitudeY, System.Drawing.Color.Yellow, 3);
            if (AttributeFrameID <= -1) return;
            DrawTexturePivotNormal(d, Animation, RealAnimID, RealFrameID, x, y, Transparency);
            //DrawHitbox(d, Animation, "Solid", System.Drawing.Color.FromArgb(128, 0, 255, 0), RealAnimID, RealFrameID, x, y, Transparency, false, false, 0);
        }
        private void DrawMovingPlatformSeven(DevicePanel d, int x, int y, int Transparency, int amplitudeX, int amplitudeY, int angle, bool hasTension, int AttributeFrameID)
        {
            string AnimName = GetSetupAnimation();
            var Animation = LoadAnimation(AnimName, d);
            if (LastFrameIDAttribute != AttributeFrameID) UpdateRealAttributeFrameID(Animation, AttributeFrameID);

            int x1 = x - (amplitudeX / 2);
            int x2 = x + (amplitudeX / 2);
            int y1 = y - (amplitudeY / 2);
            int y2 = y + (amplitudeY / 2);

            d.DrawLine(x1, x2, y1, y2, System.Drawing.Color.Yellow, 3);
            if (AttributeFrameID <= -1) return;
            DrawTexturePivotNormal(d, Animation, RealAnimID, RealFrameID, x, y, Transparency);
            //DrawHitbox(d, Animation, "Solid", System.Drawing.Color.FromArgb(128, 0, 255, 0), RealAnimID, RealFrameID, x, y, Transparency, false, false, 0);
        }
        private void DrawStandardPlatform(DevicePanel d, int x, int y, int Transparency, int AttributeFrameID)
        {
            string AnimName = GetSetupAnimation();
            var Animation = LoadAnimation(AnimName, d);
            if (LastFrameIDAttribute != AttributeFrameID) UpdateRealAttributeFrameID(Animation, AttributeFrameID);
            if (AttributeFrameID <= -1) return;
            DrawTexturePivotNormal(d, Animation, RealAnimID, RealFrameID, x, y, Transparency);
            //DrawHitbox(d, Animation, "Solid", System.Drawing.Color.FromArgb(128, 0, 255, 0), RealAnimID, RealFrameID, x, y, Transparency, false, false, 0);
        }
        private void DrawFallingPlatform(DevicePanel d, int x, int y, int Transparency, int amplitudeX, int amplitudeY, int angle, bool hasTension, int AttributeFrameID)
        {
            var Animation = LoadAnimation(GetSetupAnimation(), d);
            if (LastFrameIDAttribute != AttributeFrameID) UpdateRealAttributeFrameID(Animation, AttributeFrameID);
            if (AttributeFrameID <= -1) return;
            DrawTexturePivotNormal(d, Animation, RealAnimID, RealFrameID, x, y, Transparency, false, false, 0, System.Drawing.Color.FromArgb(255,255,0,0));
            //DrawHitbox(d, Animation, "Solid", System.Drawing.Color.FromArgb(128, 0, 255, 0), RealAnimID, RealFrameID, x, y, Transparency, false, false, 0);
        }
        #endregion

        #region Other

        private void UpdateRealAttributeFrameID(Methods.Drawing.ObjectDrawing.EditorAnimation Animation, int AttributeFrameID)
        {
            if (Animation.Animation != null)
            {
                var frames = Animation.Animation.Animations.Take<Animation.AnimationEntry>(Animation.Animation.Animations.Count).SelectMany(x => x.Frames).ToList();
                if (frames.Count - 1 > AttributeFrameID && AttributeFrameID >= 0)
                {
                    var element = frames.ElementAt(AttributeFrameID);
                    RealAnimID = Animation.Animation.Animations.IndexOf(Animation.Animation.Animations.Where(x => x.Frames.Contains(element)).FirstOrDefault());
                    RealFrameID = Animation.Animation.Animations[RealAnimID].Frames.IndexOf(element);
                }
                else
                {
                    RealAnimID = 0;
                    RealFrameID = 0;
                }
                LastFrameIDAttribute = AttributeFrameID;
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
        #endregion

        public override void Draw(Structures.EntityRenderProp Properties)
        {
            DevicePanel d = Properties.Graphics;
            Classes.Scene.EditorEntity e = Properties.EditorObject;
            
            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;

            int angle = (int)e.attributesMap["angle"].ValueInt32;
            int type = (int)e.attributesMap["type"].ValueEnum;
            int amplitudeX = (int)e.attributesMap["amplitude"].ValueVector2.X.High;
            int amplitudeY = (int)e.attributesMap["amplitude"].ValueVector2.Y.High;
            int childCount = (int)e.attributesMap["childCount"].ValueEnum;
            bool hasTension = e.attributesMap["hasTension"].ValueBool;
            int speed = Methods.Entities.AttributeHandler.AttributesMapVar("speed", e);

            int FrameIDAttribute = 0;

            switch (e.attributesMap["frameID"].Type)
            {
                case AttributeTypes.UINT8:
                    FrameIDAttribute = e.attributesMap["frameID"].ValueUInt8;
                    break;
                case AttributeTypes.INT8:
                    FrameIDAttribute = e.attributesMap["frameID"].ValueInt8;
                    break;
                case AttributeTypes.ENUM:
                    FrameIDAttribute = (int)e.attributesMap["frameID"].ValueEnum;
                    break;
            }

            if (type == 1) DrawFallingPlatform(d, x, y, Transparency, amplitudeX, amplitudeY, angle, hasTension, FrameIDAttribute);
            else if (type == 2) DrawStandardMovingPlatform(d, x, y, Transparency, amplitudeX, amplitudeY, angle, hasTension, FrameIDAttribute);
            else if (type == 3) DrawNormalTensionPlatform(d, x, y, Transparency, amplitudeX, amplitudeY, angle, hasTension, FrameIDAttribute);
            else if (type == 4) DrawTensionBallPlatform(d, x, y, Transparency, amplitudeX, amplitudeY, angle, hasTension, FrameIDAttribute);
            else if (type == 7) DrawMovingPlatformSeven(d, x, y, Transparency, amplitudeX, amplitudeY, angle, hasTension, FrameIDAttribute);
            else DrawStandardPlatform(d, x, y, Transparency, FrameIDAttribute);
        }
        public override bool isObjectOnScreen(DevicePanel d, Classes.Scene.EditorEntity entity, int x, int y, int Transparency)
        {
            var attribute = entity.attributesMap["frameID"];
            int angle = (int)entity.attributesMap["angle"].ValueInt32;
            int angleRotate = (int)entity.attributesMap["angle"].ValueInt32;
            int type = (int)entity.attributesMap["type"].ValueEnum;
            int amplitudeX = (int)entity.attributesMap["amplitude"].ValueVector2.X.High;
            int amplitudeY = (int)entity.attributesMap["amplitude"].ValueVector2.Y.High;
            int childCount = (int)entity.attributesMap["childCount"].ValueEnum;
            bool hasTension = entity.attributesMap["hasTension"].ValueBool;

            int PlatformOffsetX = 0;
            int PlatformOffsetY = 0;
            int PlatformWidth = 0;
            int PlatformHight = 0;

            if (type == 0 || type == 1)
            {
                return d.IsObjectOnScreen(x - PlatformOffsetX - PlatformWidth / 2, y - PlatformOffsetY - PlatformHight / 2, PlatformWidth, PlatformHight);
            }
            else
            {
                //Default Type
                return d.IsObjectOnScreen(x - PlatformOffsetX - PlatformWidth / 2, y - PlatformOffsetY - PlatformHight / 2, PlatformWidth, PlatformHight);
            }

        }

        public override string GetSetupAnimation()
        {
            return GetSpriteAnimationPath("/Platform.bin", "Platform", new string[] { "GHZ", "CPZ", "SPZ1", "SPZ2", "FBZ", "PSZ1", "PSZ2", "SSZ1", "SSZ2", "HCZ", "MSZ", "OOZ", "LRZ1", "LRZ2", "MMZ", "TMZ1", "AIZ" });
        }
        public override string GetObjectName()
        {
            return "Platform";
        }
    }
}
