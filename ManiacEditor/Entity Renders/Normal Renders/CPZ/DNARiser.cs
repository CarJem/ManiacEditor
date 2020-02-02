using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class DNARiser : EntityRenderer
    {

        public override void Draw(Classes.Core.Draw.GraphicsHandler d, SceneEntity entity, Classes.Core.Scene.Sets.EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            //int type = (int)entity.attributesMap["type"].ValueUInt8;
            //int direction = (int)entity.attributesMap["direction"].ValueUInt8;
            bool fliph = false;
            bool flipv = false;
            var editorAnim = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("DNARiser", d.DevicePanel, 0, -1, fliph, flipv, false);
            var editorAnim2 = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("DNARiser", d.DevicePanel, 2, -1, fliph, flipv, false);
            var editorAnim3 = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("DNARiser", d.DevicePanel, 4, -1, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnim2 != null && editorAnim2.Frames.Count != 0 && editorAnim3 != null && editorAnim3.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                var frame2 = editorAnim2.Frames[0];
                var frame3 = editorAnim3.Frames[0];

                //ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);

                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame),
                    x + frame.Frame.PivotX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                    y + frame.Frame.PivotY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                    frame.Frame.Width, frame.Frame.Height, false, 145);
                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame2),
                    x + frame2.Frame.PivotX - 24 - (fliph ? (frame2.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                    y + frame2.Frame.PivotY + (flipv ? (frame2.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                    frame2.Frame.Width, frame2.Frame.Height, false, Transparency);
                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame3),
                    x + frame3.Frame.PivotX + 24 + (fliph ? (frame3.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                    y + frame3.Frame.PivotY + (flipv ? (frame3.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                    frame3.Frame.Width, frame3.Frame.Height, false, Transparency);
            }
        }

        public override string GetObjectName()
        {
            return "DNARiser";
        }
    }
}
