using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class ScrewMobile : EntityRenderer
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
            EditorAnimations Animation = properties.Animations;
            bool selected  = properties.isSelected;
            bool fliph = false;
            bool flipv = false;
            var editorAnim = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("ScrewMobile", d.DevicePanel, 0, 0, fliph, flipv, false);
            var editorAnimSeat = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("ScrewMobile", d.DevicePanel, 0, 1, fliph, flipv, false);
            var editorAnimLaunch = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("ScrewMobile", d.DevicePanel, 0, 2, fliph, flipv, false);
            var editorAnimPropel = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("ScrewMobile", d.DevicePanel, 1, -1, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnimSeat != null && editorAnimSeat.Frames.Count != 0 && editorAnimLaunch != null && editorAnimLaunch.Frames.Count != 0 && editorAnimPropel != null && editorAnimPropel.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                var frameSeat = editorAnimSeat.Frames[0];
                var frameLaunch = editorAnimLaunch.Frames[0];
                var framePropel = editorAnimPropel.Frames[Animation.index];

                Animation.ProcessAnimation(framePropel.Entry.SpeedMultiplyer, framePropel.Entry.Frames.Count, framePropel.Frame.Delay);

                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frameSeat),
                    x + frameSeat.Frame.PivotX,
                    y + frameSeat.Frame.PivotY,
                    frameSeat.Frame.Width, frameSeat.Frame.Height, false, Transparency);
                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame),
                    x + frame.Frame.PivotX,
                    y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frameLaunch),
                    x + frameLaunch.Frame.PivotX,
                    y + frameLaunch.Frame.PivotY,
                    frameLaunch.Frame.Width, frameLaunch.Frame.Height, false, Transparency);
                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(framePropel),
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
