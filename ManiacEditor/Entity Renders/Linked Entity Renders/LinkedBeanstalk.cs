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
                        Int32 bezCtrlAngle2 = tp.attributesMap["bezCtrlAngle"].ValueInt32;
                        Int32 bezCtrlLength2 = tp.attributesMap["bezCtrlLength"].ValueInt32;
                        DrawCenteredSpline(d, currentEntity, tp, bezCtrlLength, bezCtrlAngle, bezCtrlAngle2, bezCtrlLength2);
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
