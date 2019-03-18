using RSDKv5;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ManiacEditor.Actions
{
    class ActionOptimizeSlotIDs : IAction
    {
        List<SceneEntity> entities;
        Action<List<SceneEntity>> setValue;

        public string Description => $"Sorting the Slot ID Order of {entities.Count} Entities";

        public ActionOptimizeSlotIDs(List<SceneEntity> entities, Action<List<SceneEntity>> setValue)
        {
            this.entities = entities;
            this.setValue = setValue;
        }

        public void Undo()
        {
            setValue(entities);
        }

        public IAction Redo()
        {
            return new ActionOptimizeSlotIDs(entities, setValue);
        }
    }
}
