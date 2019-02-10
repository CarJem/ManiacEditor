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
            string text = "Text" + e.EditorInstance.CurrentLanguage;
            int buttonID = (int)entity.attributesMap["buttonID"].ValueVar;
            bool disabled = entity.attributesMap["disabled"].ValueBool;
			if (buttonID == 3)
			{
				buttonID = 4;
			}
			double alignmentVal = 0;
			var editorAnim = e.EditorInstance.EditorEntity_ini.LoadAnimation("MainIcons", d, 0, buttonID, false, false, false);
			var editorAnim2 = e.EditorInstance.EditorEntity_ini.LoadAnimation("MainIcons", d, 1, buttonID, false, false, false);
			var editorAnim3 = e.EditorInstance.EditorEntity_ini.LoadAnimation("MainIcons", d, 0, 3, false, false, false);
			var editorAnim4 = e.EditorInstance.EditorEntity_ini.LoadAnimation("MainIcons", d, 1, 3, false, false, false);
			var editorAnim5 = e.EditorInstance.EditorEntity_ini.LoadAnimation(text, d, 1, buttonID, false, false, false);
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
					d.DrawBitmap(frame4.Texture, x + frame4.Frame.CenterX + (int)alignmentVal, y + frame4.Frame.CenterY - 10,
						frame4.Frame.Width, frame4.Frame.Height, false, Transparency);
					d.DrawBitmap(frame3.Texture, x + frame3.Frame.CenterX + (int)alignmentVal, y + frame3.Frame.CenterY - 10,
						frame3.Frame.Width, frame3.Frame.Height, false, Transparency);
				}
				d.DrawBitmap(frame2.Texture, x + frame2.Frame.CenterX + (int)alignmentVal, y + frame2.Frame.CenterY - 10,
					frame2.Frame.Width, frame2.Frame.Height, false, Transparency);
				d.DrawBitmap(frame.Texture, x + frame.Frame.CenterX + (int)alignmentVal, y + frame.Frame.CenterY - 10,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
				e.DrawUIButtonBack(d, x, y, 120, 20, frame.Frame.Width, frame.Frame.Height);
				d.DrawBitmap(frame5.Texture, x + frame5.Frame.CenterX + (int)alignmentVal, y + frame5.Frame.CenterY,
					frame5.Frame.Width, frame5.Frame.Height, false, Transparency);


			}


        }

        public override string GetObjectName()
        {
            return "UIModeButton";
        }
    }
}
