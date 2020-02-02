using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Sol : EntityRenderer
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
            int direction = (int)entity.attributesMap["direction"].ValueUInt8;
            bool fireOrbs = entity.attributesMap["fireOrbs"].ValueBool;

            switch (direction)
            {
                case 0:
                    break;
                case 1:
                    fliph = true;
                    break;
                case 2:
                    flipv = true;
                    break;
            }

            var editorAnim = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("Sol", d.DevicePanel, 0, 0, fliph, flipv, false);
            var editorAnim2 = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("Sol", d.DevicePanel, 1, -1, false, false, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnim2 != null && editorAnim2.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                var frame2 = editorAnim2.Frames[Animation.index];

                Animation.ProcessAnimation(frame2.Entry.SpeedMultiplyer, frame2.Entry.Frames.Count, frame2.Frame.Delay);

                if (!fireOrbs)
                {
                    d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame2),
                        x + frame2.Frame.PivotX + 16,
                        y + frame2.Frame.PivotY,
                        frame2.Frame.Width, frame2.Frame.Height, false, Transparency);
                    d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame2),
                        x - frame2.Frame.PivotX - 30,
                        y + frame2.Frame.PivotY,
                        frame2.Frame.Width, frame2.Frame.Height, false, Transparency);
                }
                else
                {
                    d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame2),
                        x + frame2.Frame.PivotX + 16,
                        y + frame2.Frame.PivotY,
                        frame2.Frame.Width, frame2.Frame.Height, false, Transparency-100);
                    d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame2),
                        x - frame2.Frame.PivotX - 30,
                        y + frame2.Frame.PivotY,
                        frame2.Frame.Width, frame2.Frame.Height, false, Transparency-100);
                }


                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame),
                    x + frame.Frame.PivotX,
                    y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);

            }
        }

        public override string GetObjectName()
        {
            return "Sol";
        }
    }
}
