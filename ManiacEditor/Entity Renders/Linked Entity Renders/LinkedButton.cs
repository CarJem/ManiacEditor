using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ManiacEditor.Entity_Renders
{
    public class LinkedButton : LinkedRenderer
    {
        public override void Draw(DevicePanel d, RSDKv5.SceneEntity currentEntity, EditorEntity ObjectInstance)
        {
            ushort slotID = currentEntity.SlotID;
            ushort targetSlotID = (ushort)(currentEntity.SlotID + 1);
            uint ButtonTag = currentEntity.GetAttribute("tag").ValueUInt8;

            var tagged = ObjectInstance.EditorInstance.entities.entities.Where(e => e.Entity.AttributeExists("buttonTag", RSDKv5.AttributeTypes.VAR));
            var triggers = tagged.Where(e => e.Entity.GetAttribute("buttonTag").ValueVar == ButtonTag);

            if (triggers != null && triggers.Any())
            {
                foreach (var t in triggers)
                {
                    DrawCenteredLinkArrow(d, currentEntity, t.Entity);
                }
            }
            ObjectInstance.DrawBase(d);
        }

        public override string GetObjectName()
        {
            return "Button";
        }
    }
}
