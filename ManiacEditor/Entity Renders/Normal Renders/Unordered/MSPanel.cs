using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class MSPanel : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp properties)
        {
            Classes.Editor.Draw.GraphicsHandler d = properties.Graphics;
            SceneEntity entity = properties.Object; 
            Classes.Editor.Scene.Sets.EditorEntity e = properties.EditorObject;
            int x = properties.X;
            int y = properties.Y;
            int Transparency = properties.Transparency;
            int index = properties.Index;
            int previousChildCount = properties.PreviousChildCount;
            int platformAngle = properties.PlatformAngle;
            Methods.Entities.EntityAnimator Animation = properties.Animations;
            bool selected  = properties.isSelected;
            var editorAnim = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("MSPanel", d.DevicePanel, 0, 0, false, false, false);
            var editorAnimPanel = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("MSPanel", d.DevicePanel, 1, -1, false, false, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnimPanel != null && editorAnimPanel.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                var framePanel = editorAnimPanel.Frames[Animation.index];

                Animation.ProcessAnimation(framePanel.Entry.SpeedMultiplyer, framePanel.Entry.Frames.Count, framePanel.Frame.Delay);

                d.DrawBitmap(new Classes.Editor.Draw.GraphicsHandler.GraphicsInfo(frame),
                    x + frame.Frame.PivotX,
                    y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
                d.DrawBitmap(new Classes.Editor.Draw.GraphicsHandler.GraphicsInfo(framePanel),
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
