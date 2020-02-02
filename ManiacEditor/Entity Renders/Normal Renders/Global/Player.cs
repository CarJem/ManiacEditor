using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Player : EntityRenderer
    {

        public override void Draw(GraphicsHandler d, SceneEntity entity, Classes.Editor.Scene.Sets.EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            int id = (int)entity.attributesMap["characterID"].ValueEnum;
            if (id > 7)
            {
                entity.attributesMap["characterID"].ValueEnum = 7;
            }
            var editorAnim = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("PlayerIcons", d.DevicePanel, 0, id, false, false, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX, y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }
        }

        public override string GetObjectName()
        {
            return "Player";
        }
    }
}
