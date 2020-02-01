using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class TVPole : EntityRenderer
    {

        public override void Draw(GraphicsHandler d, SceneEntity entity, Classes.Editor.Scene.Sets.EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            var value = entity.attributesMap["length"].ValueUInt16 + 1;
            var editorAnim = Interfaces.Base.MapEditor.Instance.EntityDrawing.LoadAnimation2("TVPole", d.DevicePanel, 1, 0, false, false, false);
            var editorAnim2 = Interfaces.Base.MapEditor.Instance.EntityDrawing.LoadAnimation2("TVPole", d.DevicePanel, 1, 1, false, false, false);
            var editorAnim3 = Interfaces.Base.MapEditor.Instance.EntityDrawing.LoadAnimation2("TVPole", d.DevicePanel, 1, 2, false, false, false);
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
                        d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
                            x + (wEven ? frame2.Frame.PivotX : -frame.Frame.Width) + (-value / 2 + xx) * frame2.Frame.Width,
                            y + frame2.Frame.PivotY,
                            frame2.Frame.Width, frame2.Frame.Height, false, Transparency);
                    }
                    else if (xx == value)
                    {
                        d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame3),
                            x + (wEven ? frame3.Frame.PivotX : -frame3.Frame.Width) + (-value / 2 + xx) * frame3.Frame.Width,
                            y + frame3.Frame.PivotY,
                            frame3.Frame.Width, frame3.Frame.Height, false, Transparency);
                    }
                    else
                    {
                        d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame2),
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
