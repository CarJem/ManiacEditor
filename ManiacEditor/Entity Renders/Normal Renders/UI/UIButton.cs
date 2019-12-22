using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class UIButton : EntityRenderer
    {
        public override void Draw(GraphicsHandler d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            string text = "Text" + Editor.Instance.Options.CurrentLanguage;
            int frameID = (int)entity.attributesMap["frameID"].ValueEnum;
            int listID = (int)entity.attributesMap["listID"].ValueEnum;
            int align = (int)entity.attributesMap["align"].ValueEnum;
            int width = (int)entity.attributesMap["size"].ValueVector2.X.High;
            int height = (int)entity.attributesMap["size"].ValueVector2.Y.High;
            bool invisible = entity.attributesMap["invisible"].ValueBool;
            double alignmentVal = 0;
            var editorAnim = Editor.Instance.EntityDrawing.LoadAnimation(text, d.DevicePanel, listID, frameID, false, false, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && !invisible)
            {
                var frame = editorAnim.Frames[0];
                switch (align)
                {
                    case 0:
                        alignmentVal = -((width / 2)) - frame.Frame.PivotY;
                        break;
                    default:
                        alignmentVal = frame.Frame.PivotX + (22 / 2);
                        break;
                }
                e.DrawUIButtonBack(d, x, y, width, height, frame.Frame.Width, frame.Frame.Height, Transparency);
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame), x + (int)alignmentVal, y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }


        }

        public override string GetObjectName()
        {
            return "UIButton";
        }
    }
}
