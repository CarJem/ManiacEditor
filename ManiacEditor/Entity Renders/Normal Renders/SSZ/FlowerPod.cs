using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class FlowerPod : EntityRenderer
    {

        public override void Draw(Structures.EntityLoadOptions properties)
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
            bool fliph = false;
            bool flipv = false;
            var editorAnim = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("FlowerPod", d.DevicePanel, 0, 0, fliph, flipv, false);
            var editorAnimHead = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("FlowerPod", d.DevicePanel, 1, 0, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnimHead != null && editorAnimHead.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                var frameHead = editorAnimHead.Frames[0];

                //Animation Currently Doesn't work
                //Animation.ProcessAnimation(frameHead.Entry.SpeedMultiplyer, frameHead.Entry.Frames.Count, frameHead.Frame.Delay);

                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frameHead),
                    x + frameHead.Frame.PivotX,
                    y + frameHead.Frame.PivotY,
                    frameHead.Frame.Width, frameHead.Frame.Height, false, Transparency);
                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame),
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
