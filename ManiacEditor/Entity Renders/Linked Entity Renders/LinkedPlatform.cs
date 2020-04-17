using System.Linq;

namespace ManiacEditor.Entity_Renders
{
    public class LinkedPlatform : LinkedRenderer
    {
        public override void Draw(Structures.LinkedEntityRenderProp properties)
        {
            ushort slotID = properties.EditorObject.SlotID;
            int childCount = properties.EditorObject.GetAttribute("childCount").ValueEnum;
            ushort[] targetSlotIDs = new ushort[childCount];
            for (int i = 0; i < childCount; i++)
            {
                targetSlotIDs[i] = (ushort)(slotID + (i+1));
            }


            var tagged = Methods.Solution.CurrentSolution.Entities.Entities.ToList().Where(e => targetSlotIDs.Contains(e.SlotID));

            if (tagged != null && tagged.Any())
            {
                foreach (var t in tagged)
                {
                    DrawCenteredLinkArrow(properties.Graphics, properties.EditorObject, t);
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
