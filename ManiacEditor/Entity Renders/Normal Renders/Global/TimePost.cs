using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class TimePost : EntityRenderer
    {

        public override void Draw(GraphicsHandler d, SceneEntity entity, Classes.Editor.Scene.Sets.EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            var editorAnim = Interfaces.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("TimePost", d.DevicePanel, 0, -1, false, false, false);
            if (editorAnim != null && editorAnim.Frames.Count >= 2)
            {
                var frame = editorAnim.Frames[0];
                var frame2 = editorAnim.Frames[2];

                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
                    x + frame.Frame.PivotX,
                    y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame2),
                    x + frame2.Frame.PivotX,
                    y + frame2.Frame.PivotY,
                    frame2.Frame.Width, frame2.Frame.Height, false, Transparency);
            }
        }

        public override string GetObjectName()
        {
            return "TimePost";
        }
    }
}
