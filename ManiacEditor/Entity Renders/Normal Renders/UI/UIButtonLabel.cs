using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class UIButtonLabel : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp properties)
        {
            Classes.Core.Draw.GraphicsHandler d = properties.Graphics;
            SceneEntity entity = properties.Object; 
            Classes.Core.Scene.Sets.EditorEntity e = properties.EditorObject;
            int x = properties.X;
            int y = properties.Y;
            int Transparency = properties.Transparency;
            int index = properties.Index;
            int previousChildCount = properties.PreviousChildCount;
            int platformAngle = properties.PlatformAngle;
            EditorAnimations Animation = properties.Animations;
            bool selected  = properties.isSelected;
            string text = "Text" + Classes.Core.SolutionState.CurrentLanguage;
            int type = (int)entity.attributesMap["type"].ValueUInt8;
            int frameID = (int)entity.attributesMap["frameID"].ValueEnum;
            int listID = (int)entity.attributesMap["listID"].ValueEnum;
            var editorAnim = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation(text, d.DevicePanel, listID, frameID, false, false, false);
            var editorAnimType = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation("ButtonLabel", d.DevicePanel, 0, type, false, false, false);
            if (editorAnimType != null && editorAnimType.Frames.Count != 0)
            {
                var frame = editorAnimType.Frames[Animation.index];
                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX, y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }
            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];
                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX, y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }



        }

        public override string GetObjectName()
        {
            return "UIButtonLabel";
        }
    }
}
