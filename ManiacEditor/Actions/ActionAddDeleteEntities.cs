using System;
using System.Collections.Generic;

namespace ManiacEditor.Actions
{
    class ActionAddDeleteEntities : IAction
    {
        Action<List<Classes.Scene.EditorEntity>> addEntity;
        Action<List<Classes.Scene.EditorEntity>> deleteEntity;
        List<Classes.Scene.EditorEntity> entities;
        bool add;

        public string Description => GenerateActionDescription();

        public ActionAddDeleteEntities(List<Classes.Scene.EditorEntity> entities, bool add, Action<List<Classes.Scene.EditorEntity>> addEntity, Action<List<Classes.Scene.EditorEntity>> deleteEntity)
        {
            this.entities = entities;
            this.add = add;
            this.addEntity = addEntity;
            this.deleteEntity = deleteEntity;
        }

        public void Undo()
        {
            if (add)
                deleteEntity(entities);
            else
                addEntity(entities);
        }

        public IAction Redo()
        {
            return new ActionAddDeleteEntities(entities, !add, addEntity, deleteEntity);
        }

        private string GenerateActionDescription()
        {
            string action;
            string name = null;
            if (add)
            {
                action = "Adding";
            }
            else
            {
                action = "Deleting";
            }

            if (null == entities)
            {
                // this shouldn't happen
                name = "object";
            }
            else if (entities.Count == 1)
            {
                name = entities[0]?.Object?.Name?.ToString();
            }
            else
            {
                name = $"{entities.Count} Objects";
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                // this probably shouldn't happen either
                name = "object";
            }

            return $"{action} {name}";
        }

    }
}