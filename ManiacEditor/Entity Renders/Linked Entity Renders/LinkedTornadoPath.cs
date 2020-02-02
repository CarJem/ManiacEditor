using System.Linq;

namespace ManiacEditor.Entity_Renders
{
    public class LinkedTornadoPath : LinkedRenderer
    {
        public override void Draw(Classes.Core.Draw.GraphicsHandler d, RSDKv5.SceneEntity currentEntity, Classes.Core.Scene.Sets.EditorEntity ObjectInstance)
        {
            ushort slotID = currentEntity.SlotID;
            ushort targetSlotID = (ushort)(currentEntity.SlotID + 1);

            ObjectInstance.DrawBase(d);

            //if (goProperty == 1 && destinationTag == 0) return; // probably just a destination

            // this is the start of a WarpDoor, find its partner(s)
            var tornadoPaths = currentEntity.Object.Entities.Where(e => e.SlotID == targetSlotID);

            if (tornadoPaths != null
                && tornadoPaths.Any())
            {
                // some destinations seem to be duplicated, so we must loop
                foreach (var tp in tornadoPaths)
                {
                    DrawLinkArrow(d, currentEntity, tp);
                }
            }
        }

        public override string GetObjectName()
        {
            return "TornadoPath";
        }

    }
}
