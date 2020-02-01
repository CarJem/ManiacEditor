using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class PrintBlock : EntityRenderer
    {

        public override void Draw(GraphicsHandler d, SceneEntity entity, Classes.Editor.Scene.Sets.EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            int letter = (int)entity.attributesMap["letter"].ValueUInt8;
            int duration = (int)entity.attributesMap["duration"].ValueUInt16;
            bool fliph = false;
            bool flipv = false;
            int frameID = 0;
            if (letter >= 11)
            {
                entity.attributesMap["letter"].ValueUInt8 = 11;
            }
            if (duration != 0)
            {
                frameID = 4;
            }

           var editorAnim = Interfaces.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("PrintBlock", d.DevicePanel, letter, frameID, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];

                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame),
                    x + frame.Frame.PivotX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                    y + frame.Frame.PivotY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }
        }

        public override string GetObjectName()
        {
            return "PrintBlock";
        }
    }
}
