using System.Linq;

namespace ManiacEditor.Entity_Renders
{
    public class LinkedAIZTornadoPath : LinkedRenderer
    {
        public override void Draw(Structures.LinkedEntityRenderProp properties)
        {
            ushort slotID = properties.Object.SlotID;
            ushort targetSlotID = (ushort)(properties.Object.SlotID + 1);

            properties.EditorObject.DrawBase(properties.Graphics);

            //if (goProperty == 1 && destinationTag == 0) return; // probably just a destination

            // this is the start of a WarpDoor, find its partner(s)
            var tornadoPaths = properties.Object.Object.Entities.Where(e => e.SlotID == targetSlotID);

            if (tornadoPaths != null
                && tornadoPaths.Any())
            {
                // some destinations seem to be duplicated, so we must loop
                foreach (var tp in tornadoPaths)
                {
                    DrawLinkArrow(properties.Graphics, properties.Object, tp);
                }
            }
        }

        public override string GetObjectName()
        {
            return "AIZTornadoPath";
        }
    }
}
