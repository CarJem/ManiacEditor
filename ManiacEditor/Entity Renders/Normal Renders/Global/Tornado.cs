using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Tornado : EntityRenderer
    {

        public override void Draw(GraphicsHandler d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            bool fliph = false;
            bool flipv = false;
            var editorAnim = Editor.Instance.EntityDrawing.LoadAnimation2("Tornado", d.DevicePanel, 0, 0, fliph, flipv, false);
            var editorAnim2 = Editor.Instance.EntityDrawing.LoadAnimation2("Tornado", d.DevicePanel, 1, -1, fliph, flipv, false);
            var editorAnim3 = Editor.Instance.EntityDrawing.LoadAnimation2("Tornado", d.DevicePanel, 2, -1, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnim2 != null && editorAnim2.Frames.Count != 0 && editorAnim3 != null && editorAnim3.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                var frame2 = editorAnim2.Frames[Animation.index];
                var frame3 = editorAnim3.Frames[Animation.index3];


                Animation.ProcessAnimation(frame2.Entry.SpeedMultiplyer, frame2.Entry.Frames.Count, frame2.Frame.Delay);
                Animation.ProcessAnimation3(frame3.Entry.SpeedMultiplyer, frame3.Entry.Frames.Count, frame3.Frame.Delay);

                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame3),
                    x + frame3.Frame.PivotX,
                    y + frame3.Frame.PivotY,
                    frame3.Frame.Width, frame3.Frame.Height, false, Transparency);
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
            return "Tornado";
        }
    }
}
