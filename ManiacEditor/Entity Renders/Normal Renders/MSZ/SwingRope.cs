using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class SwingRope : EntityRenderer
    {

        public override void Draw(Classes.Core.Draw.GraphicsHandler d, SceneEntity entity, Classes.Core.Scene.Sets.EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            int ropeSize = (int)entity.attributesMap["ropeSize"].ValueUInt8 + 1;
            var editorAnim = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("SwingRope", d.DevicePanel, 0, -1, false, false, false);
            var editorAnim2 = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("SwingRope", d.DevicePanel, 1, 0, false, false, false);
            var editorAnim3 = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("SwingRope", d.DevicePanel, 2, 0, false, false, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnim2 != null && editorAnim2.Frames.Count != 0 && editorAnim3 != null && editorAnim3.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                var frame2 = editorAnim2.Frames[0];
                var frame3 = editorAnim3.Frames[0];

                for (int i = 0; i < ropeSize; i++)
                {
                    d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame),
                        x + frame.Frame.PivotX,
                        y + frame.Frame.PivotY + frame.Frame.Height*i,
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
                }
                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame3),
                    x + frame3.Frame.PivotX,
                    y + frame3.Frame.PivotY,
                    frame3.Frame.Width, frame3.Frame.Height, false, Transparency);

                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame2),
                    x + frame2.Frame.PivotX,
                    y + frame2.Frame.PivotY + frame.Frame.Height * ropeSize,
                    frame2.Frame.Width, frame2.Frame.Height, false, Transparency);
            }
        }

        public override string GetObjectName()
        {
            return "SwingRope";
        }
    }
}
