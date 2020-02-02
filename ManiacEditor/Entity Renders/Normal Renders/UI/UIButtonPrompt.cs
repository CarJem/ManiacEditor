using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class UIButtonPrompt : EntityRenderer
    {

        public override void Draw(Classes.Core.Draw.GraphicsHandler d, SceneEntity entity, Classes.Core.Scene.Sets.EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            string text = "Text" + Classes.Core.SolutionState.CurrentLanguage;
            int promptID = (int)entity.attributesMap["promptID"].ValueEnum;
            int buttonID = (int)entity.attributesMap["buttonID"].ValueEnum;
            var editorAnim = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation("Buttons", d.DevicePanel, Classes.Core.SolutionState.CurrentControllerButtons, buttonID, false, false, false);
            var editorAnim2 = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation("Buttons", d.DevicePanel, 0, 0, false, false, false);
            var editorAnimButton = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation(text, d.DevicePanel, 0, promptID, false, false, false);
            if (editorAnim2 != null && editorAnim2.Frames.Count != 0)
            {
                var frame = editorAnim2.Frames[Animation.index];
                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX, y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }
            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];
                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX, y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }
			if (editorAnimButton != null && editorAnimButton.Frames.Count != 0)
			{
				var frameBackground = editorAnimButton.Frames[Animation.index];
				d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frameBackground), x + frameBackground.Frame.PivotX, y + frameBackground.Frame.PivotY,
					frameBackground.Frame.Width, frameBackground.Frame.Height, false, Transparency);
			}


        }

        public override string GetObjectName()
        {
            return "UIButtonPrompt";
        }
    }
}
