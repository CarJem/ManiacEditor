using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Tubinaut : EntityRenderer
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
            var editorAnim = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("Tubinaut", d.DevicePanel, 0, 0, fliph, flipv, false);
            var editorAnim2 = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("Tubinaut", d.DevicePanel, 8, -1, fliph, flipv, false);
            var editorAnim3 = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("Tubinaut", d.DevicePanel, 1, 0, fliph, flipv, false);
            var editorAnim4 = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("Tubinaut", d.DevicePanel, 2, 8, fliph, flipv, false);
            var editorAnim5 = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("Tubinaut", d.DevicePanel, 3, 12, fliph, flipv, false);
            var editorAnim6 = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("Tubinaut", d.DevicePanel, 0, 1, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnim2 != null && editorAnim2.Frames.Count != 0 && editorAnim3 != null && editorAnim3.Frames.Count != 0 && editorAnim4 != null && editorAnim4.Frames.Count != 0 && editorAnim5 != null && editorAnim5.Frames.Count != 0 && editorAnim6 != null && editorAnim6.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                var frame2 = editorAnim2.Frames[Animation.index];
                var frame3 = editorAnim4.Frames[0];
                var frame4 = editorAnim3.Frames[0];
                var frame5 = editorAnim5.Frames[0];
                var frame6 = editorAnim6.Frames[0];

                Animation.ProcessAnimation(frame2.Entry.SpeedMultiplyer, frame2.Entry.Frames.Count, frame2.Frame.Delay);

                if (Controls.Base.MainEditor.Instance.EditorToolbar.ShowAnimations.IsChecked.Value == true && Classes.Core.SolutionState.AllowSpriteAnimations)
                {

                    d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame6),
                        x + frame6.Frame.PivotX - (fliph ? (frame6.Frame.Width - editorAnim6.Frames[0].Frame.Width) : 0),
                        y + frame6.Frame.PivotY + (flipv ? (frame6.Frame.Height - editorAnim6.Frames[0].Frame.Height) : 0),
                        frame6.Frame.Width, frame6.Frame.Height, false, Transparency);
                    d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame2),
                        x + frame2.Frame.PivotX - (fliph ? (frame2.Frame.Width - editorAnim2.Frames[0].Frame.Width) : 0),
                        y + frame2.Frame.PivotY + (flipv ? (frame2.Frame.Height - editorAnim2.Frames[0].Frame.Height) : 0),
                        frame2.Frame.Width, frame2.Frame.Height, false, Transparency);
                }
                else
                {
                    d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame),
                        x + frame.Frame.PivotX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                        y + frame.Frame.PivotY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
                    d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame3),
                        x + 16 + frame3.Frame.PivotX - (fliph ? (frame3.Frame.Width - editorAnim3.Frames[0].Frame.Width) : 0),
                        y + 16 + frame3.Frame.PivotY + (flipv ? (frame3.Frame.Height - editorAnim3.Frames[0].Frame.Height) : 0),
                        frame3.Frame.Width, frame3.Frame.Height, false, Transparency);
                    d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame4),
                        x - 16 + frame4.Frame.PivotX - (fliph ? (frame4.Frame.Width - editorAnim4.Frames[0].Frame.Width) : 0),
                        y + 16 + frame4.Frame.PivotY + (flipv ? (frame4.Frame.Height - editorAnim4.Frames[0].Frame.Height) : 0),
                        frame4.Frame.Width, frame4.Frame.Height, false, Transparency);
                    d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame5),
                        x + frame5.Frame.PivotX - (fliph ? (frame5.Frame.Width - editorAnim5.Frames[0].Frame.Width) : 0),
                        y - 24 + frame5.Frame.PivotY + (flipv ? (frame5.Frame.Height - editorAnim5.Frames[0].Frame.Height) : 0),
                        frame5.Frame.Width, frame5.Frame.Height, false, Transparency);
                }

            }
        }

        public override string GetObjectName()
        {
           return "Tubinaut";
        }
    }
}
