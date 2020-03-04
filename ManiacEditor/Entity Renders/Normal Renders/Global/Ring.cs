using System;
using RSDKv5;
using System.Linq;

namespace ManiacEditor.Entity_Renders
{
    public class Ring : EntityRenderer
    {
        public override void Draw(Structures.EntityRenderProp Properties)
        {
            Classes.Scene.EditorEntity e = Properties.EditorObject;
            SceneEntity entity = e.Entity;
            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;

            int type = (int)Methods.Entities.AttributeHandler.AttributesMapVar("type", entity);
            int moveType = (int)Methods.Entities.AttributeHandler.AttributesMapVar("moveType", entity);
            int angle = (int)Methods.Entities.AttributeHandler.AttributesMapInt32("angle", entity);
            UInt32 speed = Methods.Entities.AttributeHandler.AttributesMapUint32("speed", entity);

            bool fliph = false;
            bool flipv = false;

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



            var Animation = Methods.Entities.EntityDrawing.LoadAnimation(Properties.Graphics, "Ring");
            if (Animation != null && Animation.Animation != null)
            {
                if (Animation.Animation.Animations.Count != 0 && Animation.Animation.Animations.Count >= 1 && Animation.Animation.Animations[0].Frames != null && Animation.Animation.Animations[0].Frames.Count >= 1)
                {
                    var frame = Animation.Animation.Animations[0].Frames[0];


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
                        Properties.Graphics.DrawTexture(Animation.Spritesheets.ElementAt(frame.SpriteSheet).Value, (x + newX) + frame.PivotX, (y - newY) + frame.PivotY, frame.X, frame.Y, frame.Width, frame.Height, false, Transparency);
                    }
                    else if (moveType == 1)
                    {
                        int[] position = new int[2] { 0, 0 };
                        int posX = amplitudeX;
                        int posY = amplitudeY;
                        //Negative Values only work atm
                        if (amplitudeX <= -1) posX = -posX;
                        if (amplitudeY <= -1) posY = -posY;

                        Properties.Graphics.DrawTexture(Animation.Spritesheets.ElementAt(frame.SpriteSheet).Value, (x + position[0]) + frame.PivotX, (y - position[1]) + frame.PivotY, frame.X, frame.Y,
                            frame.Width, frame.Height, false, Transparency);
                    }
                    else
                    {
                        Properties.Graphics.DrawTexture(Animation.Spritesheets.ElementAt(frame.SpriteSheet).Value,
                            x + frame.PivotX,
                            y + frame.PivotY, frame.X, frame.Y,
                            frame.Width, frame.Height, false, Transparency);
                    }
                }
            }
        }

        public override string GetObjectName()
        {
            return "Ring";
        }
    }
}
