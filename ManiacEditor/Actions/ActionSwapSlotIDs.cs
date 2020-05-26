using RSDKv5;
using System;
using ManiacEditor.Classes.Scene;

namespace ManiacEditor.Actions
{
    class ActionSwapSlotIDs : IAction
    {
        EditorEntity EntityA;
        EditorEntity EntityB;

        ushort SlotA
        { 
            get
            {
                return EntityA.SlotID;
            } 
        }
        ushort SlotB
        {
            get
            {
                return EntityB.SlotID;
            }
        }

        Action<EditorEntity, EditorEntity, bool> setValue;

        public string Description => $"Swap SlotID's of {EntityA.Object.Name.Name} ({SlotA}) and {EntityB.Object.Name.Name} ({SlotB})";

        public ActionSwapSlotIDs(EditorEntity entityA, EditorEntity entityB, Action<EditorEntity, EditorEntity, bool> setValue)
        {
            this.EntityA = entityA;
            this.EntityB = entityB;
            this.setValue = setValue;
        }

        public void Undo()
        {
            setValue(EntityA, EntityB, true);
        }

        public IAction Redo()
        {
            return new ActionSwapSlotIDs(EntityA, EntityB, setValue);
        }
    }
}
