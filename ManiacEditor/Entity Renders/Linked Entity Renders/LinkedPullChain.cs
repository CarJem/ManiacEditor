using System.Linq;

namespace ManiacEditor.Entity_Renders
{
    public class LinkedPullChain : LinkedRenderer
    {
        public override void Draw(GraphicsHandler d, RSDKv5.SceneEntity currentEntity, Classes.Edit.Scene.Sets.EditorEntity ObjectInstance)
        {
            ushort slotID = currentEntity.SlotID;
            ushort targetSlotID = (ushort)(currentEntity.SlotID + 1);
            uint ButtonTag = currentEntity.GetAttribute("tag").ValueUInt8;
            bool decorMode = currentEntity.GetAttribute("decorMode").ValueBool;

            if (!decorMode)
            {
                var tagged = Classes.Editor.Solution.Entities.Entities.Where(e => e.Entity.AttributeExists("buttonTag", RSDKv5.AttributeTypes.ENUM));
                var triggers = tagged.Where(e => e.Entity.GetAttribute("buttonTag").ValueEnum == ButtonTag);

                if (triggers != null && triggers.Any())
                {
                    foreach (var t in triggers)
                    {
                        DrawCenteredLinkArrow(d, currentEntity, t.Entity);
                    }
                }
                ObjectInstance.DrawBase(d);
            }

        }

        public override string GetObjectName()
        {
            return "PullChain";
        }
    }
}
