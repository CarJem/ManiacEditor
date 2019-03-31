using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using ManiacEditor;
using Microsoft.Xna.Framework;
using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class UIModeButton : EntityRenderer
    {
        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            string text = "Text" + Editor.Instance.UIModes.CurrentLanguage;
            int buttonID = (int)entity.attributesMap["buttonID"].ValueVar;
            bool disabled = entity.attributesMap["disabled"].ValueBool;
			if (buttonID == 3)
			{
				buttonID = 4;
			}
			double alignmentVal = 0;
			var editorAnim = Editor.Instance.EntityDrawing.LoadAnimation("MainIcons", d, 0, buttonID, false, false, false);
			var editorAnim2 = Editor.Instance.EntityDrawing.LoadAnimation("MainIcons", d, 1, buttonID, false, false, false);
			var editorAnim3 = Editor.Instance.EntityDrawing.LoadAnimation("MainIcons", d, 0, 3, false, false, false);
			var editorAnim4 = Editor.Instance.EntityDrawing.LoadAnimation("MainIcons", d, 1, 3, false, false, false);
			var editorAnim5 = Editor.Instance.EntityDrawing.LoadAnimation(text, d, 1, buttonID, false, false, false);
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
					d.DrawBitmap(frame4.Texture, x + frame4.Frame.PivotX + (int)alignmentVal, y + frame4.Frame.PivotY - 10,
						frame4.Frame.Width, frame4.Frame.Height, false, Transparency);
					d.DrawBitmap(frame3.Texture, x + frame3.Frame.PivotX + (int)alignmentVal, y + frame3.Frame.PivotY - 10,
						frame3.Frame.Width, frame3.Frame.Height, false, Transparency);
				}
				d.DrawBitmap(frame2.Texture, x + frame2.Frame.PivotX + (int)alignmentVal, y + frame2.Frame.PivotY - 10,
					frame2.Frame.Width, frame2.Frame.Height, false, Transparency);
				d.DrawBitmap(frame.Texture, x + frame.Frame.PivotX + (int)alignmentVal, y + frame.Frame.PivotY - 10,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
				e.DrawUIButtonBack(d, x, y, 120, 20, frame.Frame.Width, frame.Frame.Height, Transparency);
				d.DrawBitmap(frame5.Texture, x + frame5.Frame.PivotX + (int)alignmentVal, y + frame5.Frame.PivotY,
					frame5.Frame.Width, frame5.Frame.Height, false, Transparency);


			}


        }

        public override string GetObjectName()
        {
            return "UIModeButton";
        }
    }
}
