using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Toxomister : EntityRenderer
    {

        public override void Draw(GraphicsHandler d, SceneEntity entity, Classes.Editor.Scene.Sets.EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            int direction = (int)entity.attributesMap["direction"].ValueUInt8;
            bool fliph = false;
            bool flipv = false;

            switch (direction)
            {
                case 0:
                    break;
                case 1:
                    flipv = true;
                    break;
                    /*
                case 2:
                    flipv = true;
                    break;
                case 3:
                    flipv = true;
                    fliph = true;
                    break;
                    */
            }

            var editorAnim = Editor.Instance.EntityDrawing.LoadAnimation2("Toxomister", d.DevicePanel, 0, 0, fliph, flipv, false);
            var editorAnim2 = Editor.Instance.EntityDrawing.LoadAnimation2("Toxomister", d.DevicePanel, 1, 0, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnim2 != null && editorAnim2.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                var frame2 = editorAnim2.Frames[0];

                Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);

                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame2),
                    x + (fliph ? -frame2.Frame.PivotX - frame2.Frame.Width : frame2.Frame.PivotX),
                    y + (flipv ? -frame2.Frame.PivotY - frame2.Frame.Height : frame2.Frame.PivotY),
                    frame2.Frame.Width, frame2.Frame.Height, false, Transparency);
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
                    x + (fliph ? -frame.Frame.PivotX - frame.Frame.Width : frame.Frame.PivotX),
                    y + (flipv ? -frame.Frame.PivotY - frame.Frame.Height : frame.Frame.PivotY),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);

            }
        }

        public override string GetObjectName()
        {
            return "Toxomister";
        }
    }
}
