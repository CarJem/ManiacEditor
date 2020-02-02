using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class PushSpring : EntityRenderer
    {

        public override void Draw(GraphicsHandler d, SceneEntity entity, Classes.Editor.Scene.Sets.EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            bool fliph = false;
            bool flipv = false;

            int type = (int)entity.attributesMap["type"].ValueUInt8;
            int direction = (int)entity.attributesMap["direction"].ValueUInt8;
            int animID = 0;

            switch (type)
            {
                case 0:
                    break;
                case 1:
                    animID = 1;
                    break;
                default:
                    break;
            }

            switch (direction)
            {
                case 0:
                    break;
                case 1:
                    fliph = true;
                    break;
                case 2:
                    flipv = true;
                    break;
                case 3:
                    fliph = true;
                    flipv = true;
                    break;
            }

            var editorAnim = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("PushSpring", d.DevicePanel, animID, 0, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];

                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
                    x + (fliph ? -frame.Frame.PivotX - frame.Frame.Width : frame.Frame.PivotX),
                    y + (flipv ? -frame.Frame.PivotY - frame.Frame.Height : frame.Frame.PivotY),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);

            }
        }

        public override string GetObjectName()
        {
            return "PushSpring";
        }
    }
}
