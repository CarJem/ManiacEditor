using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class RTeleporter : EntityRenderer
    {

        public override void Draw(Structures.EntityLoadOptions properties)
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
            var editorAnim = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("RGenerator", d.DevicePanel, 0, -1, fliph, flipv, false);
            var editorAnimBottom = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("RGenerator", d.DevicePanel, 0, -1, false, true, false);
            var editorAnimElectric = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("RGenerator", d.DevicePanel, 1, -1, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnimBottom != null && editorAnimBottom.Frames.Count != 0 && editorAnimElectric != null && editorAnimElectric.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];
                var frameB = editorAnimBottom.Frames[Animation.index];
                var frameE = editorAnimElectric.Frames[Animation.index];

                Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);
                Animation.ProcessAnimation2(frameE.Entry.SpeedMultiplyer, frameE.Entry.Frames.Count, frameE.Frame.Delay);

                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame),
                    x + frame.Frame.PivotX,
                    y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);

                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frameB),
                    x + frameB.Frame.PivotX - (fliph ? (frameB.Frame.Width - editorAnimBottom.Frames[0].Frame.Width) : 0),
                    y + frameE.Frame.Height/2,
                    frameB.Frame.Width, frameB.Frame.Height, false, Transparency);

                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frameE),
                    x + frameE.Frame.PivotX - 22,
                    y + frameE.Frame.PivotY,
                    frameE.Frame.Width, frame.Frame.Height, false, Transparency);
                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frameE),
                    x + frameE.Frame.PivotX - 6,
                    y + frameE.Frame.PivotY,
                    frameE.Frame.Width, frame.Frame.Height, false, Transparency);
                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frameE),
                    x + frameE.Frame.PivotX + 10,
                    y + frameE.Frame.PivotY,
                    frameE.Frame.Width, frame.Frame.Height, false, Transparency);
            }
        }

        public override string GetObjectName()
        {
            return "RTeleporter";
        }
    }
}
