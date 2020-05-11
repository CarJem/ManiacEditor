using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace ManiacEditor.Entity_Renders
{
    public class LinkedPlatformControl : LinkedRenderer
    {
        public override void Draw(Structures.LinkedEntityRenderProp Properties)
        {
            Properties.EditorObject.DrawBase(Properties.Graphics);

            var result = DrawNodeLines(Properties);
            DrawChildLines(Properties, result);
        }
        private void DrawChildLines(Structures.LinkedEntityRenderProp Properties, ushort TargetSlotID)
        {
            ushort slotID = Properties.EditorObject.SlotID;
            int NodeCount = Properties.EditorObject.attributesMap["nodeCount"].ValueEnum;
            int ControlTag = Properties.EditorObject.attributesMap["buttonTag"].ValueEnum;
            int ChildCount = Properties.EditorObject.attributesMap["childCount"].ValueEnum;

            List<Classes.Scene.EditorEntity> ChildPoints = new List<Classes.Scene.EditorEntity>();

            ushort CurrentTargetSlotID = (ushort)(TargetSlotID + 1);
            for (int i = 0; i < ChildCount; i++)
            {
                bool DoesTargetExist = Properties.EditorObject.Entities.Entities.Exists(x => x.SlotID == TargetSlotID && x.Name == "Platform");

                if (DoesTargetExist)
                {
                    var Target1 = Properties.EditorObject.Entities.Entities.Where(x => x.SlotID == TargetSlotID && x.Name == "Platform").First();
                    ChildPoints.Add(Target1);
                }

                TargetSlotID = (ushort)(TargetSlotID + 1);
            }

            if (ChildPoints != null && ChildPoints.Any())
            {
                foreach (var Child in ChildPoints)
                {
                    DrawCenteredLinkArrow(Properties.Graphics, Properties.EditorObject, Child, Color.Red);
                }
            }
        }
        private ushort DrawNodeLines(Structures.LinkedEntityRenderProp Properties)
        {
            ushort SlotID = Properties.EditorObject.SlotID;
            int NodeCount = Properties.EditorObject.attributesMap["nodeCount"].ValueEnum - 1;

            ushort TargetSlotID = SlotID;
            ushort TargetSlotIDNext = SlotID;

            ManiacEditor.Classes.Scene.EditorEntity FirstNode = null;
            ManiacEditor.Classes.Scene.EditorEntity LastNode = null;

            List<Tuple<Classes.Scene.EditorEntity, Classes.Scene.EditorEntity>> NodePairPoints = new List<Tuple<Classes.Scene.EditorEntity, Classes.Scene.EditorEntity>>();


            if (Properties.EditorObject.Entities.Entities.Exists(x => x.SlotID > SlotID && x.Name == "PlatformNode"))
            {
                FirstNode = Properties.EditorObject.Entities.Entities.Where(x => x.SlotID > SlotID && x.Name == "PlatformNode").First();

                TargetSlotID = FirstNode.SlotID;
                TargetSlotIDNext = (ushort)(FirstNode.SlotID + 1);

                int Remainder = (NodeCount % 2 == 1 ? 1 : 0);

                for (int i = 0; i < NodeCount;)
                {
                    bool DoesTarget1Exist = Properties.EditorObject.Entities.Entities.Exists(x => x.SlotID == TargetSlotID && x.Name == "PlatformNode");
                    bool DoesTarget2Exist = Properties.EditorObject.Entities.Entities.Exists(x => x.SlotID == TargetSlotIDNext && x.Name == "PlatformNode");

                    if (DoesTarget1Exist && DoesTarget2Exist)
                    {
                        var Target1 = Properties.EditorObject.Entities.Entities.Where(x => x.SlotID == TargetSlotID && x.Name == "PlatformNode").First();
                        var Target2 = Properties.EditorObject.Entities.Entities.Where(x => x.SlotID == TargetSlotIDNext && x.Name == "PlatformNode").First();

                        NodePairPoints.Add(new Tuple<Classes.Scene.EditorEntity, Classes.Scene.EditorEntity>(Target1, Target2));
                        i++;
                    }

                    TargetSlotID = (ushort)(TargetSlotID + 1);
                    TargetSlotIDNext = (ushort)(TargetSlotIDNext + 1);
                }


            }

            if (NodePairPoints != null && NodePairPoints.Any())
            {
                foreach (var NodePair in NodePairPoints)
                {
                    DrawCenteredLinkArrow(Properties.Graphics, NodePair.Item1, NodePair.Item2);
                    LastNode = NodePair.Item2;
                }

                if (LastNode != null && FirstNode != null)
                {
                    DrawCenteredLinkArrow(Properties.Graphics, LastNode, FirstNode);
                    TargetSlotID = LastNode.SlotID;
                }

            }

            return TargetSlotID;
        }

        public override string GetObjectName()
        {
            return "PlatformControl";
        }
    }
}
