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
    public class Ring : EntityRenderer
    {
        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency)
        {
            //int type = (int)entity.attributesMap["type"].ValueVar;
            //int moveType = (int)entity.attributesMap["moveType"].ValueVar;
            //int angle = (int)entity.attributesMap["angle"].ValueInt32;

            int type = (int)e.FetchAttribute.AttributesMapVar("type", entity);
            int moveType = (int)e.FetchAttribute.AttributesMapVar("moveType", entity);
            int angle = (int)e.FetchAttribute.AttributesMapInt32("angle", entity);
            UInt32 speed = e.FetchAttribute.AttributesMapUint32("speed", entity);

            bool fliph = false;
            bool flipv = false;

            //int amplitudeX = (int)entity.attributesMap["amplitude"].ValuePosition.X.High;
            //int amplitudeY = (int)entity.attributesMap["amplitude"].ValuePosition.Y.High;

            int amplitudeX = (int)e.FetchAttribute.AttributesMapPositionHighX("amplitude", entity);
            int amplitudeY = (int)e.FetchAttribute.AttributesMapPositionHighY("amplitude", entity);

            int angleStateX = 0;
            int angleStateY = 0;
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



            var editorAnim = e.LoadAnimation2("Ring", d, animID, -1, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && animID >= 0)
            {
                var frame = editorAnim.Frames[e.index];
                e.ProcessAnimation(frame.Entry.FrameSpeed, 16, frame.Frame.Duration);
                if (moveType != 2 && moveType != 1)
                {
                    e.ProcessMovingPlatform(angle);
                    angle = e.platformAngle;
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
                        d.DrawBitmap(frame.Texture, (x + newX) + frame.Frame.CenterX, (y - newY) + frame.Frame.CenterY,
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
                        position = e.EditorAnimations.ProcessMovingPlatform2(posX, 0, x, y, frame.Frame.Width, frame.Frame.Height, speed);
                    }
                    if (amplitudeX == 0 && amplitudeY != 0)
                    {
                        position = e.EditorAnimations.ProcessMovingPlatform2(0, posY, x, y, frame.Frame.Width, frame.Frame.Height, speed);
                    }
                    if (amplitudeX != 0 && amplitudeY != 0)
                    {
                        // Since we can don't know how to do it other than x or y yet
                        position = e.EditorAnimations.ProcessMovingPlatform2(posX, posY, x, y, frame.Frame.Width, frame.Frame.Height, speed);
                    }

                    d.DrawBitmap(frame.Texture, (x + position[0]) + frame.Frame.CenterX, (y - position[1]) + frame.Frame.CenterY,
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
                }
                else
                {
                    d.DrawBitmap(frame.Texture,
                        x + frame.Frame.CenterX,
                        y + frame.Frame.CenterY,
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
