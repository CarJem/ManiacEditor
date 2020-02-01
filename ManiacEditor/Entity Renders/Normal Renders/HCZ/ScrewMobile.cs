using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class ScrewMobile : EntityRenderer
    {

        public override void Draw(GraphicsHandler d, SceneEntity entity, Classes.Editor.Scene.Sets.EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            bool fliph = false;
            bool flipv = false;
            var editorAnim = Editor.Instance.EntityDrawing.LoadAnimation2("ScrewMobile", d.DevicePanel, 0, 0, fliph, flipv, false);
            var editorAnimSeat = Editor.Instance.EntityDrawing.LoadAnimation2("ScrewMobile", d.DevicePanel, 0, 1, fliph, flipv, false);
            var editorAnimLaunch = Editor.Instance.EntityDrawing.LoadAnimation2("ScrewMobile", d.DevicePanel, 0, 2, fliph, flipv, false);
            var editorAnimPropel = Editor.Instance.EntityDrawing.LoadAnimation2("ScrewMobile", d.DevicePanel, 1, -1, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnimSeat != null && editorAnimSeat.Frames.Count != 0 && editorAnimLaunch != null && editorAnimLaunch.Frames.Count != 0 && editorAnimPropel != null && editorAnimPropel.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                var frameSeat = editorAnimSeat.Frames[0];
                var frameLaunch = editorAnimLaunch.Frames[0];
                var framePropel = editorAnimPropel.Frames[Animation.index];

                Animation.ProcessAnimation(framePropel.Entry.SpeedMultiplyer, framePropel.Entry.Frames.Count, framePropel.Frame.Delay);

                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frameSeat),
                    x + frameSeat.Frame.PivotX,
                    y + frameSeat.Frame.PivotY,
                    frameSeat.Frame.Width, frameSeat.Frame.Height, false, Transparency);
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
                    x + frame.Frame.PivotX,
                    y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frameLaunch),
                    x + frameLaunch.Frame.PivotX,
                    y + frameLaunch.Frame.PivotY,
                    frameLaunch.Frame.Width, frameLaunch.Frame.Height, false, Transparency);
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(framePropel),
                    x + framePropel.Frame.PivotX,
                    y + framePropel.Frame.PivotY,
                    framePropel.Frame.Width, framePropel.Frame.Height, false, Transparency);


            }
        }

        public override string GetObjectName()
        {
            return "ScrewMobile";
        }
    }
}
