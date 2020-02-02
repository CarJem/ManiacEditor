using System.Linq;

namespace ManiacEditor.Entity_Renders
{
    public class LinkedCableWarp : LinkedRenderer
    {
        public override void Draw(Structures.LinkedEntityRenderProp properties)
        {
            ushort slotID = properties.Object.SlotID;
            ushort targetSlotID = (ushort)(properties.Object.SlotID + 1);
            int type = (int)properties.Object.attributesMap["type"].ValueEnum;

            properties.EditorObject.DrawBase(properties.Graphics);

            var beanstalkPaths = properties.Object.Object.Entities.Where(e => e.SlotID == targetSlotID);

            if (beanstalkPaths != null && beanstalkPaths.Any())
            {
                // some destinations seem to be duplicated, so we must loop
                foreach (var tp in beanstalkPaths)
                {
                    if (tp.Object.Name.ToString() == "CableWarp")
                    {
                        if (tp.AttributeExists("type", RSDKv5.AttributeTypes.ENUM))
                        {
                            int targetType = (int)tp.attributesMap["type"].ValueEnum;
                            if (targetType == 1 || targetType == 2) DrawCenteredLinkArrow(properties.Graphics, properties.Object, tp);
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
