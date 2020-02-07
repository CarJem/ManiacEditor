using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class TurboSpiker : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp properties)
        {
            Classes.Editor.Draw.GraphicsHandler d = properties.Graphics;
            SceneEntity entity = properties.Object; 
            Classes.Editor.Scene.Sets.EditorEntity e = properties.EditorObject;
            int x = properties.X;
            int y = properties.Y;
            int Transparency = properties.Transparency;
            int index = properties.Index;
            int previousChildCount = properties.PreviousChildCount;
            int platformAngle = properties.PlatformAngle;
            Methods.Entities.EntityAnimator Animation = properties.Animations;
            bool selected  = properties.isSelected;
            int type = (int)entity.attributesMap["type"].ValueEnum;
            int direction = (int)entity.attributesMap["direction"].ValueUInt8;
            bool fliph = false;
            bool flipv = false;
            if (direction == 1)
            {
                fliph = true;
            }
            var editorAnim = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("TurboSpiker", d.DevicePanel, 0, -1, fliph, flipv, false);
            var editorAnimShell = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("TurboSpiker", d.DevicePanel, 3, 0, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnimShell != null && editorAnimShell.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];
                var frameShell = editorAnimShell.Frames[0];

                Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);

                d.DrawBitmap(new Classes.Editor.Draw.GraphicsHandler.GraphicsInfo(frameShell),
                    x + frameShell.Frame.PivotX - (fliph ? (frameShell.Frame.Width - editorAnimShell.Frames[0].Frame.Width) : 0),
                    y + frameShell.Frame.PivotY + (flipv ? (frameShell.Frame.Height - editorAnimShell.Frames[0].Frame.Height) : 0),
                    frameShell.Frame.Width, frameShell.Frame.Height, false, Transparency);
                d.DrawBitmap(new Classes.Editor.Draw.GraphicsHandler.GraphicsInfo(frame),
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
