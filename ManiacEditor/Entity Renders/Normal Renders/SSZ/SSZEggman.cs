using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class SSZEggman : EntityRenderer
    {

        public override void Draw(Classes.Core.Draw.GraphicsHandler d, SceneEntity entity, Classes.Core.Scene.Sets.EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            var editorAnim = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("EggmanSSZ", d.DevicePanel, 0, -1, false, false, false);
            var editorAnimMobile = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("EggmanSSZ", d.DevicePanel, 5, -1, false, false, false);
            var editorAnimSeat = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("EggmanSSZ", d.DevicePanel, 4, -1, false, false, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnimMobile != null && editorAnimMobile.Frames.Count != 0 && editorAnimSeat != null && editorAnimSeat.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];
                var frameMobile = editorAnimMobile.Frames[0];
                var frameSeat = editorAnimSeat.Frames[0];

                Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);

                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frameSeat),
                    x + frameSeat.Frame.PivotX,
                    y + frameSeat.Frame.PivotY,
                    frameSeat.Frame.Width, frameSeat.Frame.Height, false, Transparency);
                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame),
                    x + frame.Frame.PivotX,
                    y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frameMobile),
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
