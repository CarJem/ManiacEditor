using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Button : EntityRenderer
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
            int type = (int)entity.attributesMap["type"].ValueEnum;
            bool fliph = false;
            bool flipv = false;
            int animID = 0;
            if (type == 0 || type == 1)
            {
                animID = 0;
            }
            if (type == 2 || type == 3)
            {
                animID = 1;
            }
            if (type == 3)
            {
                fliph = true;
            }
            if (type == 1)
            {
                flipv = true;
            }
            var editorAnim = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("Button", d.DevicePanel, animID, -1, fliph, flipv, false);
            var editorAnim2 = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("Button", d.DevicePanel, animID, 1, fliph, flipv, false);
            var editorAnim3 = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("Button", d.DevicePanel, animID, 2, fliph, flipv, false);
            if (editorAnim2 != null && editorAnim2.Frames.Count != 0)
            {
                var frame2 = editorAnim2.Frames[Animation.index];

                    d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame2),
                        x + frame2.Frame.PivotX - (fliph ? (frame2.Frame.Width - editorAnim2.Frames[0].Frame.Width - 7) : 0),
                        y + frame2.Frame.PivotY + (flipv ? (frame2.Frame.Height - editorAnim2.Frames[0].Frame.Height - 7) : 0),
                        frame2.Frame.Width, frame2.Frame.Height, false, Transparency);
            }
            if (editorAnim3 != null && editorAnim3.Frames.Count != 0)
            {
                var frame3 = editorAnim3.Frames[Animation.index];

                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame3),
                    x + frame3.Frame.PivotX - (fliph ? (frame3.Frame.Width - editorAnim3.Frames[0].Frame.Width - 7) : 0),
                    y + frame3.Frame.PivotY + (flipv ? (frame3.Frame.Height - editorAnim3.Frames[0].Frame.Height - 7) : 0),
                    frame3.Frame.Width, frame3.Frame.Height, false, Transparency);
            }
            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];
                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame),
                    x + frame.Frame.PivotX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width + 9) : 0),
                    y + frame.Frame.PivotY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height + 9) : 0),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);



            }
        }

        public override string GetObjectName()
        {
            return "Button";
        }
    }
}
