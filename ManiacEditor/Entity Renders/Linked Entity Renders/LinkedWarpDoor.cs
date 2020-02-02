using System.Linq;

namespace ManiacEditor.Entity_Renders
{
    public class LinkedWarpDoor : LinkedRenderer
    {
        public override void Draw(Classes.Core.Draw.GraphicsHandler d, RSDKv5.SceneEntity currentEntity, Classes.Core.Scene.Sets.EditorEntity ObjectInstance)
        {
            int goProperty = currentEntity.GetAttribute("go").ValueEnum;
            int destinationTag = currentEntity.GetAttribute("destinationTag").ValueEnum;
            byte tag = currentEntity.GetAttribute("tag").ValueUInt8;

            ObjectInstance.DrawBase(d);
            if (goProperty == 1 && destinationTag == 0) return; // probably just a destination

            // this is the start of a WarpDoor, find its partner(s)
            var warpDoors = currentEntity.Object.Entities.Where(e => e.GetAttribute("tag").ValueUInt8 == destinationTag);

            if (warpDoors != null && warpDoors.Any())
            {
                // some destinations seem to be duplicated, so we must loop
                foreach (var wd in warpDoors)
                {
                    DrawLinkArrow(d, currentEntity, wd);
                }
            }
        }

        public override string GetObjectName()
        {
            return "WarpDoor";
        }
    }
}
