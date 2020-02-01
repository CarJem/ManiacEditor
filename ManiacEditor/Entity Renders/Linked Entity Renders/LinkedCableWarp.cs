using System.Linq;

namespace ManiacEditor.Entity_Renders
{
    public class LinkedCableWarp : LinkedRenderer
    {
        public override void Draw(GraphicsHandler d, RSDKv5.SceneEntity currentEntity, Classes.Editor.Scene.Sets.EditorEntity ObjectInstance)
        {
            ushort slotID = currentEntity.SlotID;
            ushort targetSlotID = (ushort)(currentEntity.SlotID + 1);
            int type = (int)currentEntity.attributesMap["type"].ValueEnum;

            ObjectInstance.DrawBase(d);

            var beanstalkPaths = currentEntity.Object.Entities.Where(e => e.SlotID == targetSlotID);

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
