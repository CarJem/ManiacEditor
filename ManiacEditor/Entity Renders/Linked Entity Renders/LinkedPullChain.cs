using System.Linq;

namespace ManiacEditor.Entity_Renders
{
    public class LinkedPullChain : LinkedRenderer
    {
        public override void Draw(Structures.LinkedEntityRenderProp properties)
        {
            ushort slotID = properties.Object.SlotID;
            ushort targetSlotID = (ushort)(properties.Object.SlotID + 1);
            uint ButtonTag = properties.Object.GetAttribute("tag").ValueUInt8;
            bool decorMode = properties.Object.GetAttribute("decorMode").ValueBool;

            if (!decorMode)
            {
                var tagged = Classes.Core.Solution.Entities.Entities.Where(e => e.Entity.AttributeExists("buttonTag", RSDKv5.AttributeTypes.ENUM));
                var triggers = tagged.Where(e => e.Entity.GetAttribute("buttonTag").ValueEnum == ButtonTag);

                if (triggers != null && triggers.Any())
                {
                    foreach (var t in triggers)
                    {
                        DrawCenteredLinkArrow(properties.Graphics, properties.Object, t.Entity);
                    }
                }
                properties.EditorObject.DrawBase(properties.Graphics);
            }

        }

        public override string GetObjectName()
        {
            return "PullChain";
        }
    }
}
