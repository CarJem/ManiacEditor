using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class UIMedallionPanel : EntityRenderer
    {

        public override void Draw(GraphicsHandler d, SceneEntity entity, Classes.Editor.Scene.Sets.EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
			var editorAnim = Editor.Instance.EntityDrawing.LoadAnimation("MedallionPanel", d.DevicePanel, 0, 2, false, false, false);
			x -= 38;
			y -= 16;
			if (editorAnim != null && editorAnim.Frames.Count != 0)
			{
				var frame = editorAnim.Frames[Animation.index];
				for (int mx = 0; mx < 8; mx++)
				{
					for (int my = 0; my < 4; my++)
					{
						d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX + (14 * mx), y + frame.Frame.PivotY + (16 * my),
	frame.Frame.Width, frame.Frame.Height, false, Transparency);
					}
				}

			}
			/*
			if (editorAnim2 != null && editorAnim2.Frames.Count != 0)
			{
				var frame = editorAnim2.Frames[Animation.index];
				d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX, y + frame.Frame.PivotY,
					frame.Frame.Width, frame.Frame.Height, false, Transparency);
			}
			if (editorAnim3 != null && editorAnim3.Frames.Count != 0)
			{
				var frame = editorAnim3.Frames[Animation.index];
				d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX, y + frame.Frame.PivotY,
					frame.Frame.Width, frame.Frame.Height, false, Transparency);
			}*/
		}

		public override string GetObjectName()
        {
            return "UIMedallionPanel";
        }
    }
}
