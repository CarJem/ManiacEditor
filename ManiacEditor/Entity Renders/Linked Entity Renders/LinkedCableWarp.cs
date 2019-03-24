using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ManiacEditor.Entity_Renders
{
    public class LinkedCableWarp : LinkedRenderer
    {
        public override void Draw(DevicePanel d, RSDKv5.SceneEntity currentEntity, EditorEntity ObjectInstance)
        {
            ushort slotID = currentEntity.SlotID;
            ushort targetSlotID = (ushort)(currentEntity.SlotID + 1);
            int type = (int)currentEntity.attributesMap["type"].ValueVar;

            ObjectInstance.DrawBase(d);

            var beanstalkPaths = currentEntity.Object.Entities.Where(e => e.SlotID == targetSlotID);

            if (beanstalkPaths != null && beanstalkPaths.Any())
            {
                // some destinations seem to be duplicated, so we must loop
                foreach (var tp in beanstalkPaths)
                {
                    if (tp.Object.Name.ToString() == "CableWarp")
                    {
                        if (tp.AttributeExists("type", RSDKv5.AttributeTypes.VAR))
                        {
                            int targetType = (int)tp.attributesMap["type"].ValueVar;
                            if (targetType == 1 || targetType == 2) DrawCenteredLinkArrow(d, currentEntity, tp);
                        }

                    }
                }
            }
        }

        public override string GetObjectName()
        {
            return "CableWarp";
        }

    }
}
