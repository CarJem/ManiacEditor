using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class UIVsResults : EntityRenderer
    {

        public override void Draw(Classes.Core.Draw.GraphicsHandler d, SceneEntity entity, Classes.Core.Scene.Sets.EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
			string text = "Text" + Classes.Core.SolutionState.CurrentLanguage;
			int playerID = (int)entity.attributesMap["playerID"].ValueUInt8;
			int player = 8;
			switch (playerID)
			{
				case 0:
					player = 8;
					break;
				case 1:
					player = 9;
					break;
				case 2:
					player = 10;
					break;
				case 3:
					player = 11;
					break;

			}
			
			var editorAnim = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation("EditorUIRender", d.DevicePanel, 5, 0, false, false, false);
			var editorAnimPlayerText = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation(text, d.DevicePanel, 12, player, false, false, false);
			if (editorAnim != null && editorAnim.Frames.Count != 0)
			{
				var frame = editorAnim.Frames[Animation.index];
				d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX, y + frame.Frame.PivotY + 40,
					frame.Frame.Width, frame.Frame.Height, false, Transparency);
			}
			if (editorAnimPlayerText != null && editorAnimPlayerText.Frames.Count != 0)
			{
				var frame = editorAnimPlayerText.Frames[Animation.index];
				d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX + 36, y + frame.Frame.PivotY - 26,
					frame.Frame.Width, frame.Frame.Height, false, Transparency);
			}
		}

		public override string GetObjectName()
        {
            return "UIVsResults";
        }
    }
}
