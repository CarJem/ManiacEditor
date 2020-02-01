using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class MSPanel : EntityRenderer
    {

        public override void Draw(GraphicsHandler d, SceneEntity entity, Classes.Editor.Scene.Sets.EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            var editorAnim = Interfaces.Base.MapEditor.Instance.EntityDrawing.LoadAnimation2("MSPanel", d.DevicePanel, 0, 0, false, false, false);
            var editorAnimPanel = Interfaces.Base.MapEditor.Instance.EntityDrawing.LoadAnimation2("MSPanel", d.DevicePanel, 1, -1, false, false, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnimPanel != null && editorAnimPanel.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                var framePanel = editorAnimPanel.Frames[Animation.index];

                Animation.ProcessAnimation(framePanel.Entry.SpeedMultiplyer, framePanel.Entry.Frames.Count, framePanel.Frame.Delay);

                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
                    x + frame.Frame.PivotX,
                    y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(framePanel),
                    x + framePanel.Frame.PivotX,
                    y + framePanel.Frame.PivotY,
                    framePanel.Frame.Width, framePanel.Frame.Height, false, Transparency);
            }
        }

        public override string GetObjectName()
        {
            return "MSPanel";
        }
    }
}
