using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class UIVsResults : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp properties)
        {
            Methods.Draw.GraphicsHandler d = properties.Graphics;
            SceneEntity entity = properties.Object; 
            Classes.Scene.Sets.EditorEntity e = properties.EditorObject;
            int x = properties.X;
            int y = properties.Y;
            int Transparency = properties.Transparency;
            int index = properties.Index;
            int previousChildCount = properties.PreviousChildCount;
            int platformAngle = properties.PlatformAngle;
            Methods.Entities.EntityAnimator Animation = properties.Animations;
            bool selected  = properties.isSelected;
			string text = "Text" + Methods.Editor.SolutionState.CurrentLanguage;
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
			
			var editorAnim = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation("EditorUIRender", d.DevicePanel, 5, 0, false, false, false);
			var editorAnimPlayerText = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation(text, d.DevicePanel, 12, player, false, false, false);
			if (editorAnim != null && editorAnim.Frames.Count != 0)
			{
				var frame = editorAnim.Frames[Animation.index];
				d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX, y + frame.Frame.PivotY + 40,
					frame.Frame.Width, frame.Frame.Height, false, Transparency);
			}
			if (editorAnimPlayerText != null && editorAnimPlayerText.Frames.Count != 0)
			{
				var frame = editorAnimPlayerText.Frames[Animation.index];
				d.DrawBitmap(new Methods.Draw.GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX + 36, y + frame.Frame.PivotY - 26,
					frame.Frame.Width, frame.Frame.Height, false, Transparency);
			}
		}

		public override string GetObjectName()
        {
            return "UIVsResults";
        }
    }
}
