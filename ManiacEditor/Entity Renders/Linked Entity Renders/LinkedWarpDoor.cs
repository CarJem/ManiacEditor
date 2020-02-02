using System.Linq;

namespace ManiacEditor.Entity_Renders
{
    public class LinkedWarpDoor : LinkedRenderer
    {
        public override void Draw(Structures.LinkedEntityRenderProp properties)
        {
            int goProperty = properties.Object.GetAttribute("go").ValueEnum;
            int destinationTag = properties.Object.GetAttribute("destinationTag").ValueEnum;
            byte tag = properties.Object.GetAttribute("tag").ValueUInt8;

            properties.EditorObject.DrawBase(properties.Graphics);
            if (goProperty == 1 && destinationTag == 0) return; // probably just a destination

            // this is the start of a WarpDoor, find its partner(s)
            var warpDoors = properties.Object.Object.Entities.Where(e => e.GetAttribute("tag").ValueUInt8 == destinationTag);

            if (warpDoors != null && warpDoors.Any())
            {
                // some destinations seem to be duplicated, so we must loop
                foreach (var wd in warpDoors)
                {
                    DrawLinkArrow(properties.Graphics, properties.Object, wd);
                }
            }
        }

        public override string GetObjectName()
        {
            return "WarpDoor";
        }
    }
}
