using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class HotaruHiWatt : EntityRenderer
    {

        public override void Draw(GraphicsHandler d, SceneEntity entity, Classes.Editor.Scene.Sets.EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            var editorAnim = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("HotaruHiWatt", d.DevicePanel, 0, -1, false, false, false);
            var editorAnimBulb = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("HotaruHiWatt", d.DevicePanel, 1, -1, false, false, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnimBulb != null && editorAnimBulb.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];
                var frameBulb = editorAnimBulb.Frames[Animation.index];

                Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);
                Animation.ProcessAnimation2(frameBulb.Entry.SpeedMultiplyer, frameBulb.Entry.Frames.Count, frameBulb.Frame.Delay);

                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
                    x + frame.Frame.PivotX,
                    y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frameBulb),
                    x + frameBulb.Frame.PivotX,
                    y + frameBulb.Frame.PivotY,
                    frameBulb.Frame.Width, frameBulb.Frame.Height, false, Transparency);
            }
        }

        public override string GetObjectName()
        {
            return "HotaruHiWatt";
        }
    }
}
