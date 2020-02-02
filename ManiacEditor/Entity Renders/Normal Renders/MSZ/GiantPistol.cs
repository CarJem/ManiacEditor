using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class GiantPistol : EntityRenderer
    {

        public override void Draw(GraphicsHandler d, SceneEntity entity, Classes.Editor.Scene.Sets.EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            bool fliph = false;
            bool flipv = false;
            int direction = (int)entity.attributesMap["direction"].ValueUInt8;
            if (direction == 1)
            {
                fliph = true;
            }
            var editorAnim = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("Pistol", d.DevicePanel, 0, 0, fliph, flipv, false);
            var editorAnim2 = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("Pistol", d.DevicePanel, 0, 1, fliph, flipv, false);
            var editorAnim3 = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("Pistol", d.DevicePanel, 4, 0, fliph, flipv, false);
            var editorAnim4 = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("Pistol", d.DevicePanel, 5, 0, fliph, flipv, false);
            var editorAnim5 = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("Pistol", d.DevicePanel, 6, 0, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnim2 != null && editorAnim2.Frames.Count != 0 && editorAnim3 != null && editorAnim3.Frames.Count != 0 && editorAnim4 != null && editorAnim4.Frames.Count != 0 && editorAnim5 != null && editorAnim5.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                var frame2 = editorAnim2.Frames[0];
                var frame3 = editorAnim3.Frames[0];
                var frame4 = editorAnim4.Frames[0];
                var frame5 = editorAnim5.Frames[0];

                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame2),
                    x + frame2.Frame.PivotX - (fliph ? 76 : 0),
                    y + frame2.Frame.PivotY,
                    frame2.Frame.Width, frame2.Frame.Height, false, Transparency);
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
                    x + frame.Frame.PivotX,
                    y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame3),
                    x + frame3.Frame.PivotX - (fliph ? 59 : 0),
                    y + frame3.Frame.PivotY,
                    frame3.Frame.Width, frame3.Frame.Height, false, Transparency);
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame5),
                    x + frame5.Frame.PivotX - (fliph ? frame.Frame.Width + 4: 0),
                    y + frame5.Frame.PivotY,
                    frame5.Frame.Width, frame5.Frame.Height, false, Transparency);

            }
        }

        public override string GetObjectName()
        {
            return "GiantPistol";
        }
    }
}
