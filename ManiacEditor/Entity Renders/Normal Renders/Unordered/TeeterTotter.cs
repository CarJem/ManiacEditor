using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class TeeterTotter : EntityRenderer
    {

        public override void Draw(GraphicsHandler d, SceneEntity entity, Classes.Editor.Scene.Sets.EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            var value = entity.attributesMap["length"].ValueUInt32;
            var editorAnim = Editor.Instance.EntityDrawing.LoadAnimation2("TeeterTotter", d.DevicePanel, 0, 0, false, false, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                for (int i = -(int)value; i < value; ++i)
                {
                    d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX + (i * (frame.Frame.Width + 2)),
                        y + frame.Frame.PivotY, frame.Frame.Width, frame.Frame.Height, false, Transparency);
                }
            }
        }

        public override string GetObjectName()
        {
            return "TeeterTotter";
        }
    }
}
