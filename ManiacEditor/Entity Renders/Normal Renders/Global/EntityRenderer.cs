using RSDKv5;
using System;

namespace ManiacEditor.Entity_Renders
{
    [Serializable]
    public abstract class EntityRenderer
    {

        public abstract string GetObjectName();

        public virtual void Draw(Structures.EntityRenderProp properties)
        {

        }

        public virtual bool isObjectOnScreen(DevicePanel d, SceneEntity entity, Classes.Scene.Sets.EditorEntity e, int x, int y, int Transparency)
        {
            return d.IsObjectOnScreen(x, y, 20, 20);
        }


    }
}
