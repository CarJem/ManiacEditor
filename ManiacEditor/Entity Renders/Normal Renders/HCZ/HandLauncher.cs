using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class HandLauncher : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp properties)
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
            Methods.Entities.EntityAnimator Animation = properties.Animations;
            bool selected  = properties.isSelected;
            int direction = (int)entity.attributesMap["direction"].ValueUInt8;
            bool fliph = false;
            bool flipv = false;
            if (direction == 1)
            {
                fliph = true;
            }
            var editorAnim = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("HandLauncher", d.DevicePanel, 0, -1, fliph, flipv, false);
            var editorAnimHand = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("HandLauncher", d.DevicePanel, 1, 0, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnimHand != null && editorAnimHand.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];
                var frameHand = editorAnimHand.Frames[0];

                Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);

                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame),
                    x + frame.Frame.PivotX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                    y + frame.Frame.PivotY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);

                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frameHand),
                    x + frameHand.Frame.PivotX - (fliph ? (frameHand.Frame.Width - editorAnimHand.Frames[0].Frame.Width) : 0),
                    y + frameHand.Frame.PivotY + (flipv ? (frameHand.Frame.Height - editorAnimHand.Frames[0].Frame.Height) : 0),
                    frameHand.Frame.Width, frameHand.Frame.Height, false, Transparency);
            }
        }

        public override string GetObjectName()
        {
            return "HandLauncher";
        }
    }
}
