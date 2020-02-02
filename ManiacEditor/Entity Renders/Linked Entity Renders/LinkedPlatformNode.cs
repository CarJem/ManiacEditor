using System.Linq;

namespace ManiacEditor.Entity_Renders
{
    public class LinkedPlatformNode : LinkedRenderer
    {
        public override void Draw(Structures.LinkedEntityRenderProp properties)
        {
            ushort slotID = properties.Object.SlotID;
            ushort targetSlotID = (ushort)(properties.Object.SlotID + 1);

            //if (goProperty == 1 && destinationTag == 0) return; // probably just a destination

            // this is the start of a WarpDoor, find its partner(s)
            var nodePaths = properties.Object.Object.Entities.Where(e => e.SlotID == targetSlotID);

            if (nodePaths != null
                && nodePaths.Any())
            {
                properties.EditorObject.DrawBase(properties.Graphics);
                // some destinations seem to be duplicated, so we must loop
                foreach (var tp in nodePaths)
                {
                    DrawCenteredLinkArrow(properties.Graphics, properties.Object, tp);
                }
            }
            else
            {
                properties.EditorObject.DrawBase(properties.Graphics);
            }
        }

        public override string GetObjectName()
        {
            return "PlatformNode";
        }
    }
}
