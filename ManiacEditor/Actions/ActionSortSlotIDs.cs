using RSDKv5;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ManiacEditor.Actions
{
    class ActionSortSlotIDs : IAction
    {
        IList<SceneEntity> entities;
        IList<ushort> EntitiesOrderedSlotIDs;
        IList<ushort> orderedSlotIDs;
        Action<IList<SceneEntity>, IList<ushort>> setValue;

        public string Description => $"Sorting the Slot ID Order of {entities.Count} Entities";

        public ActionSortSlotIDs(IList<SceneEntity> entities, IList<ushort> orderedSlotIDs, IList<ushort> EntitiesOrderedSlotIDs, Action<IList<SceneEntity>, IList<ushort>> setValue)
        {
            this.entities = entities;
            this.orderedSlotIDs = orderedSlotIDs;
            this.setValue = setValue;
            this.EntitiesOrderedSlotIDs = EntitiesOrderedSlotIDs;
        }

        public void Undo()
        {
            setValue(entities, EntitiesOrderedSlotIDs);
        }

        public IAction Redo()
        {
            return new ActionSortSlotIDs(entities, EntitiesOrderedSlotIDs, orderedSlotIDs, setValue);
        }
    }
}
