using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class UISubHeading : EntityRenderer
    {

        public override void Draw(GraphicsHandler d, SceneEntity entity, Classes.Editor.Scene.Sets.EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            string text = "Text" + Classes.Editor.SolutionState.CurrentLanguage;
            int listID = (int)entity.attributesMap["listID"].ValueEnum;
            int frameID = (int)entity.attributesMap["frameID"].ValueEnum;
			int width = (int)entity.attributesMap["size"].ValueVector2.X.High;
			int height = (int)entity.attributesMap["size"].ValueVector2.Y.High;
			int align = (int)entity.attributesMap["align"].ValueEnum;
			double alignmentVal = 0;
			var editorAnim = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation(text, d.DevicePanel, listID, frameID, false, false, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];
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
            return "UISubHeading";
        }
    }
}
