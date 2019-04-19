using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Iwamodoki : EntityRenderer
    {

        public override void Draw(GraphicsHandler d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            bool fliph = false;
            bool flipv = false;
            int type = (int)entity.attributesMap["type"].ValueVar;
            var editorAnim = Editor.Instance.EntityDrawing.LoadAnimation2("Iwamodoki", d.DevicePanel, type, 6, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && type >= 0)
            {
                var frame = editorAnim.Frames[Animation.index];

                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
                    x + frame.Frame.PivotX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                    y + frame.Frame.PivotY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }
        }

        public override string GetObjectName()
        {
            return "Iwamodoki";
        }
    }
}
