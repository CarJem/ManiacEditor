using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class ElectroMagnet : EntityRenderer
    {

        public override void Draw(GraphicsHandler d, SceneEntity entity, Classes.Editor.Scene.Sets.EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            bool fliph = false;
            bool flipv = false;
            bool invisible = entity.attributesMap["invisible"].ValueBool;
            var editorAnim = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("ElectroMagnet", d.DevicePanel, 0, 0, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && invisible == false)
            {
                var frame = editorAnim.Frames[0];
                // Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);
                    d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
                        x + frame.Frame.PivotX + (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width * 2) : 0),
                        y + frame.Frame.PivotY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }
        }

        public override string GetObjectName()
        {
            return "ElectroMagnet";
        }
    }
}
