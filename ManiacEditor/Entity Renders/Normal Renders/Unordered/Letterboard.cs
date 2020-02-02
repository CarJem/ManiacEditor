using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Letterboard : EntityRenderer
    {

        public override void Draw(Classes.Core.Draw.GraphicsHandler d, SceneEntity entity, Classes.Core.Scene.Sets.EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            int letterID = (int)entity.attributesMap["letterID"].ValueUInt8;
            bool controller = entity.attributesMap["controller"].ValueBool;
            bool fliph = false;
            bool flipv = false;
            var editorAnim = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("Letterboard", d.DevicePanel, 0, -1, fliph, flipv, false);
            var editorAnim2 = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("Letterboard", d.DevicePanel, 1, letterID - 1, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnim2 != null && editorAnim2.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                var frame2 = editorAnim2.Frames[Animation.index];

                //ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);

                if (letterID == 0 || controller == true)
                {
                    d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame),
                        x + frame.Frame.PivotX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                        y + frame.Frame.PivotY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
                }
                else
                {
                    d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame2),
                        x + frame.Frame.PivotX - (fliph ? (frame.Frame.Width - editorAnim.Frames[letterID].Frame.Width) : 0),
                        y + frame.Frame.PivotY + (flipv ? (frame.Frame.Height - editorAnim.Frames[letterID].Frame.Height) : 0),
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
                }

            }
        }

        public override string GetObjectName()
        {
            return "Letterboard";
        }
    }
}
