using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ManiacEditor.Entity_Renders
{
    public class LinkedWarpDoor : LinkedRenderer
    {
        public override void Draw(DevicePanel d, RSDKv5.SceneEntity currentEntity, EditorEntity ObjectInstance)
        {
            uint goProperty = currentEntity.GetAttribute("go").ValueVar;
            uint destinationTag = currentEntity.GetAttribute("destinationTag").ValueVar;
            byte tag = currentEntity.GetAttribute("tag").ValueUInt8;

            ObjectInstance.Draw(d);
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
