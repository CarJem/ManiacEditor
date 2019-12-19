using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class MicDrop : EntityRenderer
    {

        public override void Draw(GraphicsHandler d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            bool fliph = false;
            bool flipv = false;
            int distance = entity.attributesMap["distance"].ValueUInt16;
            var editorAnim = Editor.Instance.EntityDrawing.LoadAnimation2("MicDrop", d.DevicePanel, 0, -1, fliph, flipv, false);
            d.DrawLine(x, y, x, y + distance, System.Drawing.Color.Black);
            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];

                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
                    x + frame.Frame.PivotX,
                    y + frame.Frame.PivotY + distance,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }
        }

        public override string GetObjectName()
        {
            return "MicDrop";
        }
    }
}
