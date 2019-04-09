using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ManiacEditor.Entity_Renders
{
    public class LinkedBeanstalk : LinkedRenderer
    {
        public override void Draw(DevicePanel d, RSDKv5.SceneEntity currentEntity, EditorEntity ObjectInstance)
        {
            ushort slotID = currentEntity.SlotID;
            ushort targetSlotID = (ushort)(currentEntity.SlotID + 1);
            Int32 bezCtrlAngle = currentEntity.attributesMap["bezCtrlAngle"].ValueInt32;
            Int32 bezCtrlLength = currentEntity.attributesMap["bezCtrlLength"].ValueInt32;

            ObjectInstance.DrawBase(d);

            var beanstalkPaths = currentEntity.Object.Entities.Where(e => e.SlotID == targetSlotID);

            if (beanstalkPaths != null && beanstalkPaths.Any())
            {
                // some destinations seem to be duplicated, so we must loop
                foreach (var tp in beanstalkPaths)
                {
                    if (tp.Object.Name.ToString() == "Beanstalk")
                    {
                        //Int32 bezCtrlAngle2 = tp.attributesMap["bezCtrlAngle"].ValueInt32;
                        //Int32 bezCtrlLength2 = tp.attributesMap["bezCtrlLength"].ValueInt32;

                        /*
                        Point Start = new Point(currentEntity.Position.X.High, currentEntity.Position.Y.High);
                        Point End = new Point(tp.Position.X.High, tp.Position.Y.High);

                        int cx = (Start.X > End.X ? Start.X : End.X);
                        int cy = (Start.Y > End.Y ? Start.Y : End.Y);

                        Point p1 = new Point(End.X, Start.Y);
                        Point p2 = new Point(Start.X, End.Y);

                        float[] x = { Start.X, p2.X, End.X };
                        float[] y = { Start.Y, p2.Y, End.Y };

                        float[] xs, ys;
                        TestMySpline.CubicSpline.FitParametric(x, y, 100, out xs, out ys);
                        Point lastPoint = new Point(-1, -1);
                        foreach (var p in Extensions.CreateDataPoints(xs, ys))
                        {
                            if (lastPoint.X != -1)
                            {
                                d.DrawLine(p.X, p.Y, lastPoint.X, lastPoint.Y, Color.Red);
                            }
                            d.DrawRectangle(p.X, p.Y, p.X + 2, p.Y + 2, Color.Red);
                            lastPoint = new Point(p.X, p.Y);
                        }*/
                        //d.DrawRectangle(Middle.X, Middle.Y, Middle.X + 2, Middle.Y + 2, Color.Pink);
                        //d.DrawRectangle(Middle2.X, Middle2.Y, Middle2.X + 2, Middle2.Y + 2, Color.Pink);
                        //d.DrawRectangle(Middle3.X, Middle3.Y, Middle3.X + 2, Middle3.Y + 2, Color.Pink);

                    }
                }
            }
        }

        public override string GetObjectName()
        {
            return "Beanstalk";
        }

    }
}
