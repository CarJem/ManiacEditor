using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class UICharButton : EntityRenderer
    {

        public override void Draw(GraphicsHandler d, SceneEntity entity, Classes.Edit.Scene.Sets.EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {

            int characterID = (int)entity.attributesMap["characterID"].ValueUInt8;
            int characterID_text = characterID;
            if (characterID >= 3) characterID++;
            string text = "Text" + Classes.Editor.SolutionState.CurrentLanguage;
            var editorAnim = Editor.Instance.EntityDrawing.LoadAnimation(text, d.DevicePanel, 8, characterID_text, false, false, false);
            var editorAnimFrame = Editor.Instance.EntityDrawing.LoadAnimation("EditorUIRender", d.DevicePanel, 1, 1, false, false, false);
            var editorAnimIcon = Editor.Instance.EntityDrawing.LoadAnimation("SaveSelect", d.DevicePanel, 1, characterID, false, false, false);

            d.DrawRectangle(x - 48, y - 48, x + 48, y + 48, System.Drawing.Color.FromArgb(128, 255, 255, 255));

            if (editorAnimFrame != null && editorAnimFrame.Frames.Count != 0)
            {
                var frame = editorAnimFrame.Frames[Animation.index];
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX, y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }
            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX, y + frame.Frame.PivotY + 32,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }
            if (editorAnimIcon != null && editorAnimIcon.Frames.Count != 0)
            {
                var frame = editorAnimIcon.Frames[Animation.index];
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX, y + frame.Frame.PivotY - 8,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);

            }



        }

        public override string GetObjectName()
        {
            return "UICharButton";
        }
    }
}
