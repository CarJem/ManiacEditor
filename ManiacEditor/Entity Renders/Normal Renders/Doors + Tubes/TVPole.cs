using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class TVPole : EntityRenderer
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
            var value = entity.attributesMap["length"].ValueUInt16 + 1;
            var editorAnim = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("TVPole", d.DevicePanel, 1, 0, false, false, false);
            var editorAnim2 = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("TVPole", d.DevicePanel, 1, 1, false, false, false);
            var editorAnim3 = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("TVPole", d.DevicePanel, 1, 2, false, false, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnim2 != null && editorAnim2.Frames.Count != 0 && editorAnim3 != null && editorAnim3.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                var frame2 = editorAnim2.Frames[0];
                var frame3 = editorAnim3.Frames[0];
                bool wEven = false;
                for (int xx = 0; xx <= value; ++xx)
                {
                    if (xx == 0)
                    {
                        d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame),
                            x + (wEven ? frame2.Frame.PivotX : -frame.Frame.Width) + (-value / 2 + xx) * frame2.Frame.Width,
                            y + frame2.Frame.PivotY,
                            frame2.Frame.Width, frame2.Frame.Height, false, Transparency);
                    }
                    else if (xx == value)
                    {
                        d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame3),
                            x + (wEven ? frame3.Frame.PivotX : -frame3.Frame.Width) + (-value / 2 + xx) * frame3.Frame.Width,
                            y + frame3.Frame.PivotY,
                            frame3.Frame.Width, frame3.Frame.Height, false, Transparency);
                    }
                    else
                    {
                        d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame2),
                            x + (wEven ? frame2.Frame.PivotX : -frame2.Frame.Width) + (-value / 2 + xx) * frame2.Frame.Width,
                            y + frame2.Frame.PivotY,
                            frame2.Frame.Width, frame2.Frame.Height, false, Transparency);
                    }

                }
            }
        }

        public override string GetObjectName()
        {
            return "TVPole";
        }
    }
}
