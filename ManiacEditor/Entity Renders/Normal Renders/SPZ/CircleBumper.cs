﻿using System;
using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class CircleBumper : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp properties)
        {
            Methods.Draw.GraphicsHandler d = properties.Graphics;
            SceneEntity entity = properties.Object; 
            Classes.Scene.Sets.EditorEntity e = properties.EditorObject;
            int x = properties.X;
            int y = properties.Y;
            int Transparency = properties.Transparency;
            int index = properties.Index;
            int previousChildCount = properties.PreviousChildCount;
            int platformAngle = properties.PlatformAngle;
            Methods.Entities.EntityAnimator Animation = properties.Animations;
            bool selected  = properties.isSelected;
            int type = (int)entity.attributesMap["type"].ValueEnum;
            int speed = (int)entity.attributesMap["speed"].ValueEnum;
            int angle = (int)entity.attributesMap["angle"].ValueInt32;
            int amplitudeX = (int)entity.attributesMap["amplitude"].ValueVector2.X.High;
            int amplitudeY = (int)entity.attributesMap["amplitude"].ValueVector2.Y.High; 
            bool fliph = false;
            bool flipv = false;
            int animID = 0;
            var editorAnim = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("CircleBumper", d.DevicePanel, animID, -1, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && animID >= 0)
            {

                var frame = editorAnim.Frames[Animation.index];
                Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);
                if (type == 2)
                {
                    //Something is wrong here, wait untill I figure this out to define them
                    //Animation.ProcessMovingPlatform(angle,speed);
                    Animation.ProcessMovingPlatform(angle);
                    if ((amplitudeX != 0 || amplitudeY != 0))
                    {
                        double xd = x;
                        double yd = y;
                        double x2 = x + amplitudeX - amplitudeX / 3.7;
                        double y2 = y + amplitudeY - amplitudeY / 3.7;
                        double radius = Math.Pow(x2 - xd, 2) + Math.Pow(y2 - yd, 2);
                        int radiusInt = (int)Math.Sqrt(radius);
                        int newX = (int)(radiusInt * Math.Cos(Math.PI * Animation.platformAngle / 128));
                        int newY = (int)(radiusInt * Math.Sin(Math.PI * Animation.platformAngle / 128));
                        d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame), (x + newX) + frame.Frame.PivotX, (y - newY) + frame.Frame.PivotY,
                           frame.Frame.Width, frame.Frame.Height, false, Transparency);
                    }
                    else
                    {
                        d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame),
                            x + frame.Frame.PivotX,
                            y + frame.Frame.PivotY,
                            frame.Frame.Width, frame.Frame.Height, false, Transparency);
                    }
                }

                else if (type == 1)
                {
                    int[] position = new int[2] { 0, 0 };
                    int posX = amplitudeX;
                    int posY = amplitudeY;
                    //Negative Values only work atm
                    if (amplitudeX <= -1) posX = -posX;
                    if (amplitudeY <= -1) posY = -posY;

                    if (amplitudeX != 0 && amplitudeY == 0)
                    {
                        position = Animation.ProcessMovingPlatform2(posX, 0, x, y, frame.Frame.Width, frame.Frame.Height, (int)speed);
                    }
                    if (amplitudeX == 0 && amplitudeY != 0)
                    {
                        position = Animation.ProcessMovingPlatform2(0, posY, x, y, frame.Frame.Width, frame.Frame.Height, (int)speed);
                    }
                    if (amplitudeX != 0 && amplitudeY != 0)
                    {
                        // Since we can don't know how to do it other than x or y yet
                        position = Animation.ProcessMovingPlatform2D(posX, posY, x, y, frame.Frame.Width, frame.Frame.Height, (uint)speed);
                    }

                    else
                    {
                        d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX + position[0], y + frame.Frame.PivotY - position[1],
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
                    }
                }
                
                else
                {
                    if ((amplitudeX != 0 || amplitudeY != 0))
                    {
                        double xd = x;
                        double yd = y;
                        double x2 = x + amplitudeX - amplitudeX / 3.7;
                        double y2 = y + amplitudeY - amplitudeY / 3.7;
                        double radius = Math.Pow(x2 - xd, 2) + Math.Pow(y2 - yd, 2);
                        int radiusInt = (int)Math.Sqrt(radius);
                        int newX = (int)(radiusInt * Math.Cos(Math.PI * Animation.platformAngle / 128));
                        int newY = (int)(radiusInt * Math.Sin(Math.PI * Animation.platformAngle / 128));
                        d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame), (x + newX) + frame.Frame.PivotX, (y - newY) + frame.Frame.PivotY,
                           frame.Frame.Width, frame.Frame.Height, false, Transparency);
                    }
                    else
                    {
                        d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame),
                            x + frame.Frame.PivotX,
                            y + frame.Frame.PivotY,
                            frame.Frame.Width, frame.Frame.Height, false, Transparency);
                    }
                }


            }
        }

        public override string GetObjectName()
        {
            return "CircleBumper";
        }
    }
}
