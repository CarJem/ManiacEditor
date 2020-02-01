using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class UIHeading : EntityRenderer
    {

        public override void Draw(GraphicsHandler d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            string text = "Headings" + Classes.Edit.SolutionState.CurrentLanguage;
            int listID = (int)entity.attributesMap["headingID"].ValueEnum;
            var editorAnim = Editor.Instance.EntityDrawing.LoadAnimation(text, d.DevicePanel, listID, 0, false, false, false);
            var editorAnimBar = Editor.Instance.EntityDrawing.LoadAnimation("UIElements", d.DevicePanel, 0, 0, false, false, false);
            if (editorAnimBar != null && editorAnimBar.Frames.Count != 0)
            {
                var frame = editorAnimBar.Frames[Animation.index];
                //Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX, y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }
            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];
                //Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX, y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }


        }

        public override string GetObjectName()
        {
            return "UIHeading";
        }
    }
}
