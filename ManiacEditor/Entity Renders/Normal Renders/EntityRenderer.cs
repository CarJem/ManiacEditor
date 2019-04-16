using RSDKv5;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiacEditor.Entity_Renders
{
    [Serializable]
    public abstract class EntityRenderer
    {

        public abstract string GetObjectName();

        public virtual void Draw(GraphicsHandler d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {

        }

        public virtual bool isObjectOnScreen(GraphicsHandler d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency)
        {
            return d.IsObjectOnScreen(x, y, 20, 20);
        }


    }
}
