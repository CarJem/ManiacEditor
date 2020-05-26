using RSDKv5;
using System;
using System.Collections.Generic;
using ManiacEditor.Classes.Scene;
using System.Linq;

namespace ManiacEditor.Actions
{
    class ActionEntityPropertyChange : IAction
    {
        EditorEntity entity;
        string tag;
        object oldValue;
        object newValue;
        Action<EditorEntity, string, object, object, bool, bool> setValue;

        public string Description => $"Changing {tag} on {entity.Object.Name} from {(oldValue == null ? "UNKNOWN" : oldValue)} to {newValue}";

        public ActionEntityPropertyChange(EditorEntity entity, string tag, object oldValue, object newValue, Action<EditorEntity, string, object, object, bool, bool> setValue)
        {
            this.entity = entity;
            this.tag = tag;
            this.oldValue = oldValue;
            this.newValue = newValue;
            this.setValue = setValue;
        }

        public void Undo()
        {
            setValue.Invoke(entity, tag, oldValue, newValue, true, true);
        }

        public IAction Redo()
        {
            return new ActionEntityPropertyChange(entity, tag, newValue, oldValue, setValue);
        }
    }


    class EntityMultiplePropertyChanges
    {
        public EditorEntity Entity { get; set; }
        public object NewValue { get; set; }
        public object OldValue { get; set; }

        public EntityMultiplePropertyChanges GetUndoChange()
        {
            return new EntityMultiplePropertyChanges(Entity, OldValue, NewValue);
        }

        public EntityMultiplePropertyChanges()
        {

        }
        public EntityMultiplePropertyChanges(EditorEntity Entity)
        {
            this.Entity = Entity;
        }
        public EntityMultiplePropertyChanges(EditorEntity Entity, object NewValue, object OldValue)
        {
            this.Entity = Entity;
            this.NewValue = NewValue;
            this.OldValue = OldValue;
        }
    }

    class ActionEntityMultiplePropertyChange : IAction
    {
        string tag;
        List<EntityMultiplePropertyChanges> values;
        Action<string, List<EntityMultiplePropertyChanges>, bool> setValue;
        public string Description => $"Changing {tag} on {values.Count} Entities";

        public ActionEntityMultiplePropertyChange(string tag, List<EntityMultiplePropertyChanges> values, Action<string, List<EntityMultiplePropertyChanges>, bool> setValue)
        {
            this.values = values;
            this.tag = tag;
            this.setValue = setValue;
        }

        public void Undo()
        {
            setValue.Invoke(tag, values.ConvertAll(x => x.GetUndoChange()), true);
        }

        public IAction Redo()
        {
            return new ActionEntityMultiplePropertyChange(tag, values.ConvertAll(x => x.GetUndoChange()), setValue);
        }
    }

    class ActionEntityAttributeChange : IAction
    {
        SceneEntity entity;
        string name;
        object oldValue;
        object newValue;
        AttributeTypes type;
        Action<SceneEntity, string, object, object, AttributeTypes> setValue;

        public string Description => $"Changing {name} ({type}) on {entity.Object.Name} from {oldValue} to {newValue}";

        public ActionEntityAttributeChange(SceneEntity entity, string tag, object oldValue, object newValue, AttributeTypes type, Action<SceneEntity, string, object, object, AttributeTypes> setValue)
        {
            this.entity = entity;
            this.name = tag;
            this.oldValue = oldValue;
            this.newValue = newValue;
            this.setValue = setValue;
            this.type = type;
        }

        public void Undo()
        {
            setValue(entity, name, newValue, oldValue, type);
        }

        public IAction Redo()
        {
            return new ActionEntityAttributeChange(entity, name, newValue, oldValue, type, setValue);
        }
    }



}
