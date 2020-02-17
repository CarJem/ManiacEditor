using System.Linq;

namespace ManiacEditor.Entity_Renders
{
    public class LinkedPlatform : LinkedRenderer
    {
        public override void Draw(Structures.LinkedEntityRenderProp properties)
        {
            ushort slotID = properties.Object.SlotID;
            int childCount = properties.Object.GetAttribute("childCount").ValueEnum;
            ushort[] targetSlotIDs = new ushort[childCount];
            for (int i = 0; i < childCount; i++)
            {
                targetSlotIDs[i] = (ushort)(slotID + (i+1));
            }


            var tagged = Classes.Editor.Solution.Entities.Entities.Values.ToList().Where(e => targetSlotIDs.Contains(e.Entity.SlotID));

            if (tagged != null && tagged.Any())
            {
                foreach (var t in tagged)
                {
                    DrawCenteredLinkArrow(properties.Graphics, properties.Object, t.Entity);
                }
            }
            properties.EditorObject.DrawBase(properties.Graphics);
        }

        public override string GetObjectName()
        {
            return "Platform";
        }
    }
}
