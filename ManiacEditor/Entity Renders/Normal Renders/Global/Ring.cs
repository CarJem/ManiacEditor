using System;
using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Ring : EntityRenderer
    {
        public override void Draw(Structures.EntityRenderProp properties)
        {
            Classes.Core.Draw.GraphicsHandler d = properties.Graphics;
            SceneEntity entity = properties.Object; 
            Classes.Core.Scene.Sets.EditorEntity e = properties.EditorObject;
            int x = properties.X;
            int y = properties.Y;
            int Transparency = properties.Transparency;
            int index = properties.Index;
            int previousChildCount = properties.PreviousChildCount;
            int platformAngle = properties.PlatformAngle;
            Methods.Entities.EntityAnimator Animation = properties.Animations;
            bool selected  = properties.isSelected;
            //int type = (int)entity.attributesMap["type"].ValueEnum;
            //int moveType = (int)entity.attributesMap["moveType"].ValueEnum;
            //int angle = (int)entity.attributesMap["angle"].ValueInt32;

            int type = (int)Methods.Entities.AttributeHandler.AttributesMapVar("type", entity);
            int moveType = (int)Methods.Entities.AttributeHandler.AttributesMapVar("moveType", entity);
            int angle = (int)Methods.Entities.AttributeHandler.AttributesMapInt32("angle", entity);
            UInt32 speed = Methods.Entities.AttributeHandler.AttributesMapUint32("speed", entity);

            bool fliph = false;
            bool flipv = false;

            //int amplitudeX = (int)entity.attributesMap["amplitude"].ValueVector2.X.High;
            //int amplitudeY = (int)entity.attributesMap["amplitude"].ValueVector2.Y.High;

            int amplitudeX = (int)Methods.Entities.AttributeHandler.AttributesMapPositionHighX("amplitude", entity);
            int amplitudeY = (int)Methods.Entities.AttributeHandler.AttributesMapPositionHighY("amplitude", entity);

            int animID;
            switch (type)
            {
                case 0:
                    animID = 0;
                    break;
                case 1:
                    animID = 1;
                    break;
                case 2:
                    animID = 2;
                    break;
                default:
                    animID = 0;
                    break;
            }



            var editorAnim = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("Ring", d.DevicePanel, animID, -1, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && animID >= 0)
            {
                var frame = editorAnim.Frames[Animation.index];

                if (type >= 2)
                {
                    Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, 16, frame.Frame.Delay);
                }
                else
                {
                    Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);
                }

                if (moveType == 2)
                {
                    //e.ProcessMovingPlatform(angle);
                    angle = Animation.platformAngle;

                    if (type >= 2)
                    {
                        Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, 16, frame.Frame.Delay);
                    }
                    else
                    {
                        Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);
                    }
                }


                if ((amplitudeX != 0 || amplitudeY != 0) && moveType == 2)
                {
                        double xd = x;
                        double yd = y;
                        double x2 = x + amplitudeX - amplitudeX / 3.7;
                        double y2 = y + amplitudeY - amplitudeY / 3.7;
                        double radius = Math.Pow(x2 - xd, 2) + Math.Pow(y2 - yd, 2);
                        int radiusInt = (int)Math.Sqrt(radius);
                        int newX = (int)(radiusInt * Math.Cos(Math.PI * angle / 128));
                        int newY = (int)(radiusInt * Math.Sin(Math.PI * angle / 128));
                        d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame), (x + newX) + frame.Frame.PivotX, (y - newY) + frame.Frame.PivotY,
                           frame.Frame.Width, frame.Frame.Height, false, Transparency);
                }
                else if (moveType == 1)
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
                        position = Animation.ProcessMovingPlatform2(posX, posY, x, y, frame.Frame.Width, frame.Frame.Height, (int)speed);
                    }

                    d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame), (x + position[0]) + frame.Frame.PivotX, (y - position[1]) + frame.Frame.PivotY,
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
                }
                else
                {
                    d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame),
                        x + frame.Frame.PivotX,
                        y + frame.Frame.PivotY,
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
                }


            }
        }

        public override string GetObjectName()
        {
            return "Ring";
        }
    }
}
