using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Buggernaut : EntityRenderer
    {

        public override void Draw(GraphicsHandler d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            int direction = (int)entity.attributesMap["direction"].ValueUInt8;
            bool fliph = false;
            bool flipv = false;
            if (direction == 1)
            {
                fliph = true;
            }
            var editorAnim = Editor.Instance.EntityDrawing.LoadAnimation2("Buggernaut", d.DevicePanel, 0, 0, fliph, flipv, false);
            var editorAnimWings = Editor.Instance.EntityDrawing.LoadAnimation2("Buggernaut", d.DevicePanel, 2, -1, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnimWings != null && editorAnimWings.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                var frameWings = editorAnimWings.Frames[Animation.index];

                Animation.ProcessAnimation(frameWings.Entry.SpeedMultiplyer, frameWings.Entry.Frames.Count, frameWings.Frame.Delay);

                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frameWings),
                    x + frameWings.Frame.PivotX - (fliph ? (frameWings.Frame.Width - editorAnimWings.Frames[0].Frame.Width) : 0),
                    y + frameWings.Frame.PivotY + (flipv ? (frameWings.Frame.Height - editorAnimWings.Frames[0].Frame.Height) : 0),
                    frameWings.Frame.Width, frameWings.Frame.Height, false, Transparency - 150);
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
                    x + frame.Frame.PivotX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                    y + frame.Frame.PivotY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);

            }
        }

        public override string GetObjectName()
        {
            return "Buggernaut";
        }
    }
}
