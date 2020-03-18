using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class SSZEggman : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp properties)
        {
            Methods.Draw.GraphicsHandler d = properties.Graphics;
            SceneEntity entity = properties.Object; 
            Classes.Scene.Sets.EditorEntity e = properties.EditorObject;
            int x = properties.X;
            int y = properties.Y;
            int Transparency = properties.Transparency;
            int index = properties.Index;
            int previousChildCount = properties.PreviousChildCount;
            int platformAngle = properties.PlatformAngle;
            Methods.Entities.EntityAnimator Animation = properties.Animations;
            bool selected  = properties.isSelected;
            var editorAnim = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("EggmanSSZ", d.DevicePanel, 0, -1, false, false, false);
            var editorAnimMobile = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("EggmanSSZ", d.DevicePanel, 5, -1, false, false, false);
            var editorAnimSeat = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("EggmanSSZ", d.DevicePanel, 4, -1, false, false, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnimMobile != null && editorAnimMobile.Frames.Count != 0 && editorAnimSeat != null && editorAnimSeat.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];
                var frameMobile = editorAnimMobile.Frames[0];
                var frameSeat = editorAnimSeat.Frames[0];

                Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);

                d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frameSeat),
                    x + frameSeat.Frame.PivotX,
                    y + frameSeat.Frame.PivotY,
                    frameSeat.Frame.Width, frameSeat.Frame.Height, false, Transparency);
                d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame),
                    x + frame.Frame.PivotX,
                    y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
                d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frameMobile),
                    x + frameMobile.Frame.PivotX,
                    y + frameMobile.Frame.PivotY,
                    frameMobile.Frame.Width, frameMobile.Frame.Height, false, Transparency);
            }
        }

        public override string GetObjectName()
        {
            return "SSZEggman";
        }
    }
}
