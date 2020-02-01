using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace ManiacEditor.Entity_Renders
{
    public class LinkedPlatformControl : LinkedRenderer
    {
        public override void Draw(GraphicsHandler d, RSDKv5.SceneEntity currentEntity, EditorEntity ObjectInstance)
        {
            ushort slotID = currentEntity.SlotID;
            ushort targetSlotID = (ushort)(currentEntity.SlotID + 1);
            int NodeCount = currentEntity.attributesMap["nodeCount"].ValueEnum;
            int ControlTag = currentEntity.attributesMap["buttonTag"].ValueEnum;
            int ChildCount = currentEntity.attributesMap["childCount"].ValueEnum;

            ObjectInstance.DrawBase(d);
            int minimumSlot = (int)targetSlotID;
            int maximumSlot = (int)targetSlotID + (int)NodeCount;

            List<Tuple<EditorEntity, EditorEntity>> NodePoints = new List<Tuple<EditorEntity, EditorEntity>>();
            List<EditorEntity> ChildPoints = new List<EditorEntity>();

            int currentTargetSlotID = targetSlotID;
            int remainder = (NodeCount % 2 == 1 ? 1 : 0);

            for (int i = 0; i < NodeCount; i++)
            {
                int targetID1 = currentTargetSlotID;
                int targetID2 = currentTargetSlotID + 1;
                if (targetID2 >= maximumSlot)
                {
                    targetID2 = minimumSlot;
                    var target1 = Classes.Edit.Solution.Entities.Entities.Where(e => e.Entity.SlotID == targetID1).First();
                    var target2 = Classes.Edit.Solution.Entities.Entities.Where(e => e.Entity.SlotID == targetID2).First();

                    NodePoints.Add(new Tuple<EditorEntity, EditorEntity>(target1, target2));
                }
                currentTargetSlotID = currentTargetSlotID + 1;
            }

            if (NodePoints != null && NodePoints.Any())
            {
                foreach (var pairs in NodePoints)
                {
                    DrawCenteredLinkArrow(d, pairs.Item1.Entity, pairs.Item2.Entity);
                }
            }


            for (int i = 0; i < ChildCount; i++)
            {
                int targetID = currentTargetSlotID;


                var target1 = Classes.Edit.Solution.Entities.Entities.Where(e => e.Entity.SlotID == targetID).First();

                ChildPoints.Add(target1);
                currentTargetSlotID = currentTargetSlotID + 1;
            }

            if (NodePoints != null && NodePoints.Any())
            {
                foreach (var children in ChildPoints)
                {
                    DrawCenteredLinkArrow(d, currentEntity, children.Entity, Color.Red);
                }
            }

            var tagged = Classes.Edit.Solution.Entities.Entities.Where(e => e.Entity.AttributeExists("tag", RSDKv5.AttributeTypes.UINT8));
            var triggers = tagged.Where(e => e.Entity.GetAttribute("tag").ValueUInt8 == ControlTag);

            if (triggers != null && triggers.Any())
            {
                foreach (var t in triggers)
                {
                    DrawLinkArrow(d, currentEntity, t.Entity);
                }
            }


        }

        public override string GetObjectName()
        {
            return "PlatformControl";
        }
    }
}
