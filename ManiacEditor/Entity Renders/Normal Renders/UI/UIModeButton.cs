using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class UIModeButton : EntityRenderer
    {
        public override void Draw(GraphicsHandler d, SceneEntity entity, Classes.Editor.Scene.Sets.EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            string text = "Text" + Classes.Editor.SolutionState.CurrentLanguage;
            int buttonID = (int)entity.attributesMap["buttonID"].ValueEnum;
            bool disabled = entity.attributesMap["disabled"].ValueBool;
			if (buttonID == 3)
			{
				buttonID = 4;
			}
			double alignmentVal = 0;
			var editorAnim = Interfaces.Base.MapEditor.Instance.EntityDrawing.LoadAnimation("MainIcons", d.DevicePanel, 0, buttonID, false, false, false);
			var editorAnim2 = Interfaces.Base.MapEditor.Instance.EntityDrawing.LoadAnimation("MainIcons", d.DevicePanel, 1, buttonID, false, false, false);
			var editorAnim3 = Interfaces.Base.MapEditor.Instance.EntityDrawing.LoadAnimation("MainIcons", d.DevicePanel, 0, 3, false, false, false);
			var editorAnim4 = Interfaces.Base.MapEditor.Instance.EntityDrawing.LoadAnimation("MainIcons", d.DevicePanel, 1, 3, false, false, false);
			var editorAnim5 = Interfaces.Base.MapEditor.Instance.EntityDrawing.LoadAnimation(text, d.DevicePanel, 1, buttonID, false, false, false);
			if (editorAnim != null && editorAnim.Frames.Count != 0 && !disabled)
            {
                var frame = editorAnim.Frames[0];
				var frame2 = editorAnim2.Frames[0];
				var frame3 = editorAnim3.Frames[0];
				var frame4 = editorAnim4.Frames[0];
				var frame5 = editorAnim5.Frames[0];

				alignmentVal = (22 / 2);

				if (buttonID == 2)
				{
					d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame4), x + frame4.Frame.PivotX + (int)alignmentVal, y + frame4.Frame.PivotY - 10,
						frame4.Frame.Width, frame4.Frame.Height, false, Transparency);
					d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame3), x + frame3.Frame.PivotX + (int)alignmentVal, y + frame3.Frame.PivotY - 10,
						frame3.Frame.Width, frame3.Frame.Height, false, Transparency);
				}
				d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame2), x + frame2.Frame.PivotX + (int)alignmentVal, y + frame2.Frame.PivotY - 10,
					frame2.Frame.Width, frame2.Frame.Height, false, Transparency);
				d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX + (int)alignmentVal, y + frame.Frame.PivotY - 10,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
				e.DrawUIButtonBack(d, x, y, 120, 20, frame.Frame.Width, frame.Frame.Height, Transparency);
				d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame5), x + frame5.Frame.PivotX + (int)alignmentVal, y + frame5.Frame.PivotY,
					frame5.Frame.Width, frame5.Frame.Height, false, Transparency);


			}


        }

        public override string GetObjectName()
        {
            return "UIModeButton";
        }
    }
}
