using RSDKv5;
using System;

namespace ManiacEditor.Actions
{
    class ActionSwapSlotIDs : IAction
    {
        SceneEntity EntityA;
        SceneEntity EntityB;
        ushort SlotA;
        ushort SlotB;
        Action<SceneEntity, SceneEntity, ushort, ushort> setValue;

        public string Description => $"Swap SlotID's of {EntityA.Object.Name.Name} ({SlotA}) and {EntityB.Object.Name.Name} ({SlotB})";

        public ActionSwapSlotIDs(SceneEntity entityA, SceneEntity entityB, ushort slotA, ushort slotB, Action<SceneEntity, SceneEntity, ushort, ushort> setValue)
        {
            this.EntityA = entityA;
            this.EntityB = entityB;
            this.SlotA = slotA;
            this.SlotB = slotB;
            this.setValue = setValue;
        }

        public void Undo()
        {
            setValue(EntityA, EntityB, SlotB, SlotA);
        }

        public IAction Redo()
        {
            return new ActionSwapSlotIDs(EntityA, EntityB, SlotB, SlotA, setValue);
        }
    }
}
