using System;
using System.Linq;
using System.Drawing;

namespace ManiacEditor.Entity_Renders
{
    public class LinkedBeanstalk : LinkedRenderer
    {
        public override void Draw(Structures.LinkedEntityRenderProp properties)
        {
            ushort slotID = properties.EditorObject.SlotID;
            ushort targetSlotID = (ushort)(properties.EditorObject.SlotID + 1);
            Int32 bezCtrlAngle = properties.EditorObject.attributesMap["bezCtrlAngle"].ValueInt32;
            Int32 bezCtrlLength = properties.EditorObject.attributesMap["bezCtrlLength"].ValueInt32;

            var currentEntity = properties.EditorObject;

            var beanstalkPaths = properties.EditorObject.Entities.Entities.Where(e => e.SlotID == targetSlotID);

            if (beanstalkPaths != null && beanstalkPaths.Any())
            {
                // some destinations seem to be duplicated, so we must loop
                foreach (var tp in beanstalkPaths)
                {
                    if (tp.Object.Name.ToString() == "Beanstalk")
                    {              
                        Point Start = new Point(currentEntity.Position.X.High, currentEntity.Position.Y.High);
                        Point End = new Point(tp.Position.X.High, tp.Position.Y.High);

                        properties.Graphics.DrawLine(Start.X, Start.Y, End.X, End.Y, Color.ForestGreen, 4);
                    }
                }
            }

            properties.EditorObject.DrawBase(properties.Graphics);
        }

        public override string GetObjectName()
        {
            return "Beanstalk";
        }

    }
}
