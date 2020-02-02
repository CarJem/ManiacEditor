using RSDKv5;
using System;

namespace ManiacEditor.Entity_Renders
{
    [Serializable]
    public abstract class EntityRenderer
    {

        public abstract string GetObjectName();

        public virtual void Draw(Classes.Core.Draw.GraphicsHandler d, SceneEntity entity, Classes.Core.Scene.Sets.EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {

        }

        public virtual bool isObjectOnScreen(Classes.Core.Draw.GraphicsHandler d, SceneEntity entity, Classes.Core.Scene.Sets.EditorEntity e, int x, int y, int Transparency)
        {
            return d.IsObjectOnScreen(x, y, 20, 20);
        }


    }
}
