using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class FlowerPod : EntityRenderer
    {

        public override void Draw(GraphicsHandler d, SceneEntity entity, Classes.Editor.Scene.Sets.EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            bool fliph = false;
            bool flipv = false;
            var editorAnim = Interfaces.Base.MapEditor.Instance.EntityDrawing.LoadAnimation2("FlowerPod", d.DevicePanel, 0, 0, fliph, flipv, false);
            var editorAnimHead = Interfaces.Base.MapEditor.Instance.EntityDrawing.LoadAnimation2("FlowerPod", d.DevicePanel, 1, 0, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnimHead != null && editorAnimHead.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                var frameHead = editorAnimHead.Frames[0];

                //Animation Currently Doesn't work
                //Animation.ProcessAnimation(frameHead.Entry.SpeedMultiplyer, frameHead.Entry.Frames.Count, frameHead.Frame.Delay);

                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frameHead),
                    x + frameHead.Frame.PivotX,
                    y + frameHead.Frame.PivotY,
                    frameHead.Frame.Width, frameHead.Frame.Height, false, Transparency);
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
                    x + frame.Frame.PivotX,
                    y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);

            }
        }

        public override string GetObjectName()
        {
            return "FlowerPod";
        }
    }
}
