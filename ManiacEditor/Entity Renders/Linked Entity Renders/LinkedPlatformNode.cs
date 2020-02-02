using System.Linq;

namespace ManiacEditor.Entity_Renders
{
    public class LinkedPlatformNode : LinkedRenderer
    {
        public override void Draw(Classes.Core.Draw.GraphicsHandler d, RSDKv5.SceneEntity currentEntity, Classes.Core.Scene.Sets.EditorEntity ObjectInstance)
        {
            ushort slotID = currentEntity.SlotID;
            ushort targetSlotID = (ushort)(currentEntity.SlotID + 1);

            //if (goProperty == 1 && destinationTag == 0) return; // probably just a destination

            // this is the start of a WarpDoor, find its partner(s)
            var nodePaths = currentEntity.Object.Entities.Where(e => e.SlotID == targetSlotID);

            if (nodePaths != null
                && nodePaths.Any())
            {
                ObjectInstance.DrawBase(d);
                // some destinations seem to be duplicated, so we must loop
                foreach (var tp in nodePaths)
                {
                    DrawCenteredLinkArrow(d, currentEntity, tp);
                }
            }
            else
            {
                ObjectInstance.DrawBase(d);
            }
        }

        public override string GetObjectName()
        {
            return "PlatformNode";
        }
    }
}
