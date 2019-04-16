using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ManiacEditor;
using Microsoft.Xna.Framework;
using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class UIButtonPrompt : EntityRenderer
    {

        public override void Draw(GraphicsHandler d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            string text = "Text" + Editor.Instance.UIModes.CurrentLanguage;
            int promptID = (int)entity.attributesMap["promptID"].ValueVar;
            int buttonID = (int)entity.attributesMap["buttonID"].ValueVar;
            var editorAnim = Editor.Instance.EntityDrawing.LoadAnimation("Buttons", d.DevicePanel, Editor.Instance.UIModes.CurrentControllerButtons, buttonID, false, false, false);
            var editorAnim2 = Editor.Instance.EntityDrawing.LoadAnimation("Buttons", d.DevicePanel, 0, 0, false, false, false);
            var editorAnimButton = Editor.Instance.EntityDrawing.LoadAnimation(text, d.DevicePanel, 0, promptID, false, false, false);
            if (editorAnim2 != null && editorAnim2.Frames.Count != 0)
            {
                var frame = editorAnim2.Frames[Animation.index];
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX, y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }
            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];
                d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frame), x + frame.Frame.PivotX, y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }
			if (editorAnimButton != null && editorAnimButton.Frames.Count != 0)
			{
				var frameBackground = editorAnimButton.Frames[Animation.index];
				d.DrawBitmap(new GraphicsHandler.GraphicsInfo(frameBackground), x + frameBackground.Frame.PivotX, y + frameBackground.Frame.PivotY,
					frameBackground.Frame.Width, frameBackground.Frame.Height, false, Transparency);
			}


        }

        public override string GetObjectName()
        {
            return "UIButtonPrompt";
        }
    }
}
