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
    public class UIVsResults : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
			string text = "Text" + e.EditorInstance.CurrentLanguage;
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
			
			var editorAnim = e.EditorInstance.EditorEntity_ini.LoadAnimation("EditorUIRender", d, 5, 0, false, false, false);
			var editorAnimPlayerText = e.EditorInstance.EditorEntity_ini.LoadAnimation(text, d, 12, player, false, false, false);
			if (editorAnim != null && editorAnim.Frames.Count != 0)
			{
				var frame = editorAnim.Frames[Animation.index];
				d.DrawBitmap(frame.Texture, x + frame.Frame.CenterX, y + frame.Frame.CenterY + 40,
					frame.Frame.Width, frame.Frame.Height, false, Transparency);
			}
			if (editorAnimPlayerText != null && editorAnimPlayerText.Frames.Count != 0)
			{
				var frame = editorAnimPlayerText.Frames[Animation.index];
				d.DrawBitmap(frame.Texture, x + frame.Frame.CenterX + 36, y + frame.Frame.CenterY - 26,
					frame.Frame.Width, frame.Frame.Height, false, Transparency);
			}
		}

		public override string GetObjectName()
        {
            return "UIVsResults";
        }
    }
}
