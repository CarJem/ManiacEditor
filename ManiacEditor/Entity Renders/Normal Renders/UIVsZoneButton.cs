using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class UIVsZoneButton : EntityRenderer
    {

        public override void Draw(GraphicsHandler d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            int zoneID = (int)entity.attributesMap["zoneID"].ValueVar;

            var editorAnim = Editor.Instance.EntityDrawing.LoadAnimation("SaveSelect", d.DevicePanel, 17, zoneID, false, false, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX, y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }

            var editorAnimFrame = Editor.Instance.EntityDrawing.LoadAnimation("EditorUIRender", d.DevicePanel, 4, 0, false, false, false);
            if (editorAnimFrame != null && editorAnimFrame.Frames.Count != 0)
            {
                var frame = editorAnimFrame.Frames[0];
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX, y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }
        }

        public override string GetObjectName()
        {
            return "UIVsZoneButton";
        }
    }
}
