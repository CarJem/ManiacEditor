﻿using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Template : EntityRenderer
    {

        public override void Draw(GraphicsHandler d, SceneEntity entity, Classes.Edit.Scene.Sets.EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            //Code Goes Here
        }

        public override string GetObjectName()
        {
            return "Template";
        }
    }
}
