using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ManiacEditor;
using Microsoft.Xna.Framework;
using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class CircleBumper : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency)
        {
            int type = (int)entity.attributesMap["type"].ValueVar;
            int speed = (int)entity.attributesMap["speed"].ValueVar;
            int angle = (int)entity.attributesMap["angle"].ValueInt32;
            int amplitudeX = (int)entity.attributesMap["amplitude"].ValuePosition.X.High;
            int amplitudeY = (int)entity.attributesMap["amplitude"].ValuePosition.Y.High; 
            bool fliph = false;
            bool flipv = false;
            int animID = 0;
            var editorAnim = e.LoadAnimation2("CircleBumper", d, animID, -1, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && animID >= 0)
            {

                var frame = editorAnim.Frames[e.index];
                //e.ProcessAnimation(frame.Entry.FrameSpeed, frame.Entry.Frames.Count, frame.Frame.Duration);
                if (type == 2)
                {
                    //Something is wrong here, wait untill I figure this out to define them
                    //e.ProcessMovingPlatform(angle,speed);
                    e.ProcessMovingPlatform(angle);
                }


                if ((amplitudeX != 0 || amplitudeY != 0))
                {
                    double xd = x;
                    double yd = y;
                    double x2 = x + amplitudeX - amplitudeX / 3.7;
                    double y2 = y + amplitudeY - amplitudeY / 3.7;
                    double radius = Math.Pow(x2 - xd, 2) + Math.Pow(y2 - yd, 2);
                    int radiusInt = (int)Math.Sqrt(radius);
                    int newX = (int)(radiusInt * Math.Cos(Math.PI * e.platformAngle / 128));
                    int newY = (int)(radiusInt * Math.Sin(Math.PI * e.platformAngle / 128));
                    d.DrawBitmap(frame.Texture, (x + newX) + frame.Frame.CenterX, (y - newY) + frame.Frame.CenterY,
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
            return "CircleBumper";
        }
    }
}
