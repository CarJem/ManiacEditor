using System.Linq;

namespace ManiacEditor.Entity_Renders
{
    public class LinkedPlatformNode : LinkedRenderer
    {
        public override void Draw(Structures.LinkedEntityRenderProp properties)
        {
            properties.EditorObject.DrawBase(properties.Graphics);
            
            ushort slotID = properties.EditorObject.SlotID;
            ushort targetSlotID = (ushort)(properties.EditorObject.SlotID + 1);

            //if (goProperty == 1 && destinationTag == 0) return; // probably just a destination

            // this is the start of a WarpDoor, find its partner(s)
            var nodePaths = Methods.Solution.CurrentSolution.Entities.Entities.Where(e => e.SlotID == targetSlotID);

            if (nodePaths != null && nodePaths.ToList().Exists(x => x.Entity.Object.Name.Name == "PlatformNode"))
            {
                properties.EditorObject.DrawBase(properties.Graphics);
                // some destinations seem to be duplicated, so we must loop
                foreach (var tp in nodePaths)
                {
                    DrawCenteredLinkArrow(properties.Graphics, properties.EditorObject, tp);
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
