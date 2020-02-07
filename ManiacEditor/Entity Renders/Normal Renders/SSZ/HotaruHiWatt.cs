using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class HotaruHiWatt : EntityRenderer
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
            var editorAnim = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("HotaruHiWatt", d.DevicePanel, 0, -1, false, false, false);
            var editorAnimBulb = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("HotaruHiWatt", d.DevicePanel, 1, -1, false, false, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnimBulb != null && editorAnimBulb.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];
                var frameBulb = editorAnimBulb.Frames[Animation.index];

                Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);
                Animation.ProcessAnimation2(frameBulb.Entry.SpeedMultiplyer, frameBulb.Entry.Frames.Count, frameBulb.Frame.Delay);

                d.DrawBitmap(new Classes.Editor.Draw.GraphicsHandler.GraphicsInfo(frame),
                    x + frame.Frame.PivotX,
                    y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
                d.DrawBitmap(new Classes.Editor.Draw.GraphicsHandler.GraphicsInfo(frameBulb),
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
