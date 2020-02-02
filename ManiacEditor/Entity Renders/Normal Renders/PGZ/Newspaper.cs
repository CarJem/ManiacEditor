using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Newspaper : EntityRenderer
    {

        public override void Draw(GraphicsHandler d, SceneEntity entity, Classes.Editor.Scene.Sets.EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            int type = (int)entity.attributesMap["type"].ValueUInt8;
            int frameID = 0;
            switch (type)
            {
                case 0:
                    frameID = 0;
                    break;
                case 1:
                    frameID = 1;
                    break;
                case 2:
                    frameID = 2;
                    break;
                case 3:
                    frameID = 3;
                    break;
            }
            bool fliph = false;
            bool flipv = false;
            var editorAnim = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("Newspaper", d.DevicePanel, 1, frameID, fliph, flipv, false);
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
            return "Newspaper";
        }
    }
}
