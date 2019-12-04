using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class UFO_ItemBox : EntityRenderer
    {

        public override void Draw(GraphicsHandler d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            int id = (int)entity.attributesMap["type"].ValueEnum;
            if (id > 2)
            {
                entity.attributesMap["type"].ValueEnum = (int)2u;
            }
            var editorAnim = Editor.Instance.EntityDrawing.LoadAnimation2("Items", d.DevicePanel, 0, id, false, false, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX, y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }
        }

        public override string GetObjectName()
        {
            return "UFO_ItemBox";
        }
    }
}
