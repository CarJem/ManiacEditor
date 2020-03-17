using System;
using System.Linq;
using RSDKv5;
using System.Data;

namespace ManiacEditor.Entity_Renders
{
    public class Platform : EntityRenderer
    {

        //EditorAnimations platformMove = new EditorAnimations();
        public static int PlatformWidth = 0;
        public static int PlatformHight = 0;
        public static int PlatformOffsetX = 0;
        public static int PlatformOffsetY = 0;



        public override void Draw(Structures.EntityRenderProp Properties)
        {
            DevicePanel d = Properties.Graphics;
            Classes.Scene.EditorEntity e = Properties.EditorObject;
            SceneEntity entity = e.Entity;
            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;

            int angle = (int)entity.attributesMap["angle"].ValueInt32;
            int angleRotate = (int)entity.attributesMap["angle"].ValueInt32;
            int type = (int)entity.attributesMap["type"].ValueEnum;
            int amplitudeX = (int)entity.attributesMap["amplitude"].ValueVector2.X.High;
            int amplitudeY = (int)entity.attributesMap["amplitude"].ValueVector2.Y.High;
            int childCount = (int)entity.attributesMap["childCount"].ValueEnum;
            bool hasTension = entity.attributesMap["hasTension"].ValueBool;
            int speed = Methods.Entities.AttributeHandler.AttributesMapVar("speed", entity);
            int angleStateX = 0;
            int angleStateY = 0;

            int FrameIDAttribute = 0;

            int AnimID = 0;
            int FrameID = 0;

            switch (entity.attributesMap["frameID"].Type)
            {
                case AttributeTypes.UINT8:
                    FrameIDAttribute = entity.attributesMap["frameID"].ValueUInt8;
                    break;
                case AttributeTypes.INT8:
                    FrameIDAttribute = entity.attributesMap["frameID"].ValueInt8;
                    break;
                case AttributeTypes.ENUM:
                    FrameIDAttribute = (int)entity.attributesMap["frameID"].ValueEnum;
                    break;
            }

            DrawStandardPlatform(d, x, y, Transparency, FrameIDAttribute);
        }


        private void DrawStandardPlatform(DevicePanel d, int x, int y, int Transparency, int AttributeFrameID)
        {
            var Animation1 = LoadAnimation("Platform", d, 0, AttributeFrameID);
            DrawTexturePivotNormal(d, Animation1, Animation1.RequestedAnimID, Animation1.RequestedFrameID, x, y, Transparency);
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

        public override bool isObjectOnScreen(DevicePanel d, SceneEntity entity, Classes.Scene.EditorEntity e, int x, int y, int Transparency)
        {
            var attribute = entity.attributesMap["frameID"];
            int angle = (int)entity.attributesMap["angle"].ValueInt32;
            int angleRotate = (int)entity.attributesMap["angle"].ValueInt32;
            int type = (int)entity.attributesMap["type"].ValueEnum;
            int amplitudeX = (int)entity.attributesMap["amplitude"].ValueVector2.X.High;
            int amplitudeY = (int)entity.attributesMap["amplitude"].ValueVector2.Y.High;
            int childCount = (int)entity.attributesMap["childCount"].ValueEnum;
            bool hasTension = entity.attributesMap["hasTension"].ValueBool;

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


        public override string GetObjectName()
        {
            return "Platform";
        }
    }
}
