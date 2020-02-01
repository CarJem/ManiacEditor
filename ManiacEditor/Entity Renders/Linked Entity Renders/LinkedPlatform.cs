using System.Linq;

namespace ManiacEditor.Entity_Renders
{
    public class LinkedPlatform : LinkedRenderer
    {
        public override void Draw(GraphicsHandler d, RSDKv5.SceneEntity currentEntity, EditorEntity ObjectInstance)
        {
            ushort slotID = currentEntity.SlotID;
            int childCount = currentEntity.GetAttribute("childCount").ValueEnum;
            ushort[] targetSlotIDs = new ushort[childCount];
            for (int i = 0; i < childCount; i++)
            {
                targetSlotIDs[i] = (ushort)(slotID + (i+1));
            }


            var tagged = Classes.Edit.Solution.Entities.Entities.Where(e => targetSlotIDs.Contains(e.Entity.SlotID));

            if (tagged != null && tagged.Any())
            {
                foreach (var t in tagged)
                {
                    DrawCenteredLinkArrow(d, currentEntity, t.Entity);
                }
            }
            ObjectInstance.DrawBase(d);
        }

        public override string GetObjectName()
        {
            return "Platform";
        }
    }
}
