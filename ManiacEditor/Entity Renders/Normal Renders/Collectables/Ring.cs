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



            var Animation = Methods.Entities.EntityDrawing.LoadAnimation(Properties.Graphics, "Ring", type, 0);

            if ((amplitudeX != 0 || amplitudeY != 0) && moveType == 2)
            {
                double xd = x;
                double yd = y;
                double x2 = x + amplitudeX - amplitudeX / 3.7;
                double y2 = y + amplitudeY - amplitudeY / 3.7;
                double radius = Math.Pow(x2 - xd, 2) + Math.Pow(y2 - yd, 2);
                int radiusInt = (int)Math.Sqrt(radius);
                int newX = x + (int)(radiusInt * Math.Cos(Math.PI * angle / 128));
                int newY = y - (int)(radiusInt * Math.Sin(Math.PI * angle / 128));
                DrawTexturePivotNormal(Properties.Graphics, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, newX, newY, Transparency);
            }
            else if (moveType == 1)
            {
                int[] position = new int[2] { 0, 0 };
                int posX = amplitudeX;
                int posY = amplitudeY;
                //Negative Values only work atm
                if (amplitudeX <= -1) posX = -posX;
                if (amplitudeY <= -1) posY = -posY;

                DrawTexturePivotNormal(Properties.Graphics, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, (x + position[0]), (y - position[1]), Transparency);
            }
            else
            {
                DrawTexturePivotNormal(Properties.Graphics, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency);
            }
        }

        public override bool isObjectOnScreen(DevicePanel d, SceneEntity entity, Classes.Scene.EditorEntity e, int x, int y, int Transparency)
        {
            return d.IsObjectOnScreen(x, y, 20, 20);
        }
        public override string GetObjectName()
        {
            return "Ring";
        }
    }
}
