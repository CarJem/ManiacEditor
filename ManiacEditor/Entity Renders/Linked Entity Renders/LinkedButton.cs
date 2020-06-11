using System.Linq;

namespace ManiacEditor.Entity_Renders
{
    public class LinkedButton : LinkedRenderer
    {
        public override void Draw(Structures.LinkedEntityRenderProp properties)
        {
            ushort slotID = properties.EditorObject.SlotID;
            ushort targetSlotID = (ushort)(properties.EditorObject.SlotID + 1);
            uint ButtonTag = properties.EditorObject.GetAttribute("tag").ValueUInt8;

            var tagged = properties.EditorObject.Entities.Entities.ToList().Where(e => e.SlotID > slotID && e.Entity.AttributeExists("buttonTag", RSDKv5.AttributeTypes.ENUM));
            var triggers = tagged.Where(e => e.Entity.GetAttribute("buttonTag").ValueEnum == ButtonTag);

            if (triggers != null && triggers.Any())
            {
                DrawCenteredLinkArrow(properties.Graphics, properties.EditorObject, triggers.First());
            }
            properties.EditorObject.DrawBase(properties.Graphics);
        }

        public override string GetObjectName()
        {
            return "Button";
        }
    }
}
