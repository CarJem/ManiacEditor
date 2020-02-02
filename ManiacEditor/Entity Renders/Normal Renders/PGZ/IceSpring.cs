using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class IceSpring : EntityRenderer
    {

        public override void Draw(GraphicsHandler d, SceneEntity entity, Classes.Editor.Scene.Sets.EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            int animID = (int)entity.attributesMap["type"].ValueEnum;
            var flipFlag = entity.attributesMap["flipFlag"].ValueEnum;
            bool fliph = false;
            bool flipv = false;

            // Handle springs being flipped in both planes
            // Down
            if ((flipFlag & 0x02) == 0x02)
                flipv = true;
            // Left
            if ((flipFlag & 0x01) == 0x01)
                fliph = true;

            var editorAnim = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("IceSpring", d.DevicePanel, animID % 3, -1, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && animID >= 0)
            {
                var frame = editorAnim.Frames[Animation.index];

                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
                    x + frame.Frame.PivotX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                    y + frame.Frame.PivotY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }
        }

        public void IceDraw(GraphicsHandler d, SceneEntity entity, Classes.Editor.Scene.Sets.EditorEntity e, int x, int y, int Transparency)
        {
            int animID = 0;
            bool fliph = false;
            bool flipv = false;

            var editorAnim = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("IceSpring", d.DevicePanel, 0, 0, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && animID >= 0)
            {
                var frame = editorAnim.Frames[0];

                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
                    x + frame.Frame.PivotX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                    y + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }
        }

        public override string GetObjectName()
        {
            return "IceSpring";
        }
    }
}
