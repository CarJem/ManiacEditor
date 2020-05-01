using System;
using RSDKv5;
using System.Linq;

namespace ManiacEditor.Entity_Renders
{
    public class PlaneSwitch : EntityRenderer
    {
        public override void Draw(Structures.EntityRenderProp Properties)
        {
            DevicePanel d = Properties.Graphics;
            Classes.Scene.EditorEntity e = Properties.EditorObject;
            
            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;


            const int LeftDist = 1,
                      LeftPlane = 2,
                      RightDist = 4,
                      RightPlane = 8;

            var flags = (int)e.attributesMap["flags"].ValueEnum;
            var size = (int)(e.attributesMap["size"].ValueEnum) - 1;
            var angle = e.attributesMap["angle"].ValueInt32;

            int frameDist = (flags & LeftDist) > 0 ? 1 : 0;
            int framePlane = (flags & LeftPlane) > 0 ? 2 : 0;
            var Animation = Methods.Entities.EntityDrawing.LoadAnimation(Properties.Graphics, "PlaneSwitch");

            const int pivotOffsetX = -8, pivotOffsetY = 0;
            const int drawOffsetX = 0, drawOffsetY = -8;


            if (EntityRenderer.IsValidated(Animation, new System.Tuple<int, int>(0, frameDist + framePlane)))
            {
                var frame = Animation.Animation.Animations[0].Frames[frameDist + framePlane];
                bool hEven = size % 2 == 0;
                for (int yy = 0; yy <= size; ++yy)
                {
                    int[] drawCoords = RotatePoints(
                        x - frame.Width,
                        (y + (hEven ? frame.PivotY : -frame.Height) + (-size / 2 + yy) * frame.Height),
                        x + pivotOffsetX, y + pivotOffsetY, angle);

                    d.DrawTexture(Animation.Spritesheets.ElementAt(frame.SpriteSheet).Value, drawCoords[0] + drawOffsetX, drawCoords[1] + drawOffsetY, frame.X, frame.Y, frame.Width, frame.Height, false, Transparency);
                }
            }

            frameDist = (flags & RightDist) > 0 ? 1 : 0;
            framePlane = (flags & RightPlane) > 0 ? 2 : 0;
            Animation = Methods.Entities.EntityDrawing.LoadAnimation(d, "PlaneSwitch");

            if (EntityRenderer.IsValidated(Animation, new System.Tuple<int, int>(0, frameDist + framePlane)))
            {
                var frame = Animation.Animation.Animations[0].Frames[frameDist + framePlane];
                bool hEven = size % 2 == 0;
                for (int yy = 0; yy <= size; ++yy)
                {
                    int[] drawCoords = RotatePoints(
                        x,
                        (y + (hEven ? frame.PivotY : -frame.Height) + (-size / 2 + yy) * frame.Height),
                        x + pivotOffsetX, y + pivotOffsetY, angle);

                    d.DrawTexture(Animation.Spritesheets.ElementAt(frame.SpriteSheet).Value, drawCoords[0] + drawOffsetX, drawCoords[1] + drawOffsetY, frame.X, frame.Y, frame.Width, frame.Height, false, Transparency);
                }
            }
        }
        private static int[] RotatePoints(double initX, double initY, double centerX, double centerY, int angle)
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
        public override bool isObjectOnScreen(DevicePanel d, Classes.Scene.EditorEntity entity, int x, int y, int Transparency)
        {
            var size = (int)(entity.attributesMap["size"].ValueEnum);
            int bounds = (16 * size);

            return d.IsObjectOnScreen(x - bounds, y - bounds, bounds*2, bounds*2);
        }

        public override string GetObjectName()
        {
            return "PlaneSwitch";
        }
    }
}
