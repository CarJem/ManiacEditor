using RSDKv5;
using System;

namespace ManiacEditor.Actions
{
    class ActionEntityPropertyChange : IAction
    {
        SceneEntity entity;
        string tag;
        object oldValue;
        object newValue;
        Action<SceneEntity, string, object, object> setValue;

        public string Description => $"Changing {tag} on {entity.Object.Name} from {oldValue} to {newValue}";

        public ActionEntityPropertyChange(SceneEntity entity, string tag, object oldValue, object newValue, Action<SceneEntity, string, object, object> setValue)
        {
            this.entity = entity;
            this.tag = tag;
            this.oldValue = oldValue;
            this.newValue = newValue;
            this.setValue = setValue;
        }

        public void Undo()
        {
            setValue(entity, tag, oldValue, newValue);
        }

        public IAction Redo()
        {
            return new ActionEntityPropertyChange(entity, tag, newValue, oldValue, setValue);
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
