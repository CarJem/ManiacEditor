using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class UIMedallionPanel : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp properties)
        {
            Classes.Editor.Draw.GraphicsHandler d = properties.Graphics;
            SceneEntity entity = properties.Object; 
            Classes.Editor.Scene.Sets.EditorEntity e = properties.EditorObject;
            int x = properties.X;
            int y = properties.Y;
            int Transparency = properties.Transparency;
            int index = properties.Index;
            int previousChildCount = properties.PreviousChildCount;
            int platformAngle = properties.PlatformAngle;
            Methods.Entities.EntityAnimator Animation = properties.Animations;
            bool selected  = properties.isSelected;
			var editorAnim = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation("MedallionPanel", d.DevicePanel, 0, 2, false, false, false);
			x -= 38;
			y -= 16;
			if (editorAnim != null && editorAnim.Frames.Count != 0)
			{
				var frame = editorAnim.Frames[Animation.index];
				for (int mx = 0; mx < 8; mx++)
				{
					for (int my = 0; my < 4; my++)
					{
						d.DrawBitmap(new Classes.Editor.Draw.GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX + (14 * mx), y + frame.Frame.PivotY + (16 * my),
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
