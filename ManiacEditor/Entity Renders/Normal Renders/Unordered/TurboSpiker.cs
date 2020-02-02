using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class TurboSpiker : EntityRenderer
    {

        public override void Draw(GraphicsHandler d, SceneEntity entity, Classes.Editor.Scene.Sets.EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            int type = (int)entity.attributesMap["type"].ValueEnum;
            int direction = (int)entity.attributesMap["direction"].ValueUInt8;
            bool fliph = false;
            bool flipv = false;
            if (direction == 1)
            {
                fliph = true;
            }
            var editorAnim = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("TurboSpiker", d.DevicePanel, 0, -1, fliph, flipv, false);
            var editorAnimShell = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("TurboSpiker", d.DevicePanel, 3, 0, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnimShell != null && editorAnimShell.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];
                var frameShell = editorAnimShell.Frames[0];

                Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);

                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frameShell),
                    x + frameShell.Frame.PivotX - (fliph ? (frameShell.Frame.Width - editorAnimShell.Frames[0].Frame.Width) : 0),
                    y + frameShell.Frame.PivotY + (flipv ? (frameShell.Frame.Height - editorAnimShell.Frames[0].Frame.Height) : 0),
                    frameShell.Frame.Width, frameShell.Frame.Height, false, Transparency);
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
                    x + frame.Frame.PivotX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                    y + frame.Frame.PivotY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);

            }
        }

        public override string GetObjectName()
        {
            return "TurboSpiker";
        }
    }
}
