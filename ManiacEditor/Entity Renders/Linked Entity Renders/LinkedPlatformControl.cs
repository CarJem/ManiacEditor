using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace ManiacEditor.Entity_Renders
{
    public class LinkedPlatformControl : LinkedRenderer
    {
        public override void Draw(Structures.LinkedEntityRenderProp properties)
        {
            ushort slotID = properties.Object.SlotID;
            ushort targetSlotID = (ushort)(properties.Object.SlotID + 1);
            int NodeCount = properties.Object.attributesMap["nodeCount"].ValueEnum;
            int ControlTag = properties.Object.attributesMap["buttonTag"].ValueEnum;
            int ChildCount = properties.Object.attributesMap["childCount"].ValueEnum;

            properties.EditorObject.DrawBase(properties.Graphics);
            int minimumSlot = (int)targetSlotID;
            int maximumSlot = (int)targetSlotID + (int)NodeCount;

            List<Tuple<Classes.Core.Scene.Sets.EditorEntity, Classes.Core.Scene.Sets.EditorEntity>> NodePoints = new List<Tuple<Classes.Core.Scene.Sets.EditorEntity, Classes.Core.Scene.Sets.EditorEntity>>();
            List<Classes.Core.Scene.Sets.EditorEntity> ChildPoints = new List<Classes.Core.Scene.Sets.EditorEntity>();

            int currentTargetSlotID = targetSlotID;
            int remainder = (NodeCount % 2 == 1 ? 1 : 0);

            for (int i = 0; i < NodeCount; i++)
            {
                int targetID1 = currentTargetSlotID;
                int targetID2 = currentTargetSlotID + 1;
                if (targetID2 >= maximumSlot)
                {
                    targetID2 = minimumSlot;
                    var target1 = Classes.Core.Solution.Entities.Entities.Where(e => e.Entity.SlotID == targetID1).First();
                    var target2 = Classes.Core.Solution.Entities.Entities.Where(e => e.Entity.SlotID == targetID2).First();

                    NodePoints.Add(new Tuple<Classes.Core.Scene.Sets.EditorEntity, Classes.Core.Scene.Sets.EditorEntity>(target1, target2));
                }
                currentTargetSlotID = currentTargetSlotID + 1;
            }

            if (NodePoints != null && NodePoints.Any())
            {
                foreach (var pairs in NodePoints)
                {
                    DrawCenteredLinkArrow(properties.Graphics, pairs.Item1.Entity, pairs.Item2.Entity);
                }
            }


            for (int i = 0; i < ChildCount; i++)
            {
                int targetID = currentTargetSlotID;


                var target1 = Classes.Core.Solution.Entities.Entities.Where(e => e.Entity.SlotID == targetID).First();

                ChildPoints.Add(target1);
                currentTargetSlotID = currentTargetSlotID + 1;
            }

            if (NodePoints != null && NodePoints.Any())
            {
                foreach (var children in ChildPoints)
                {
                    DrawCenteredLinkArrow(properties.Graphics, properties.Object, children.Entity, Color.Red);
                }
            }

            var tagged = Classes.Core.Solution.Entities.Entities.Where(e => e.Entity.AttributeExists("tag", RSDKv5.AttributeTypes.UINT8));
            var triggers = tagged.Where(e => e.Entity.GetAttribute("tag").ValueUInt8 == ControlTag);

            if (triggers != null && triggers.Any())
            {
                foreach (var t in triggers)
                {
                    DrawLinkArrow(properties.Graphics, properties.Object, t.Entity);
                }
            }


        }

        public override string GetObjectName()
        {
            return "PlatformControl";
        }
    }
}
