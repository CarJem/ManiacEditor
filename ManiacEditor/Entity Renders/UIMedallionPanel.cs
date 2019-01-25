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
    public class UIMedallionPanel : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
			var editorAnim = e.EditorInstance.EditorEntity_ini.LoadAnimation("MedallionPanel", d, 0, 2, false, false, false);
			x -= 38;
			y -= 16;
			if (editorAnim != null && editorAnim.Frames.Count != 0)
			{
				var frame = editorAnim.Frames[Animation.index];
				for (int mx = 0; mx < 8; mx++)
				{
					for (int my = 0; my < 4; my++)
					{
						d.DrawBitmap(frame.Texture, x + frame.Frame.CenterX + (14 * mx), y + frame.Frame.CenterY + (16 * my),
	frame.Frame.Width, frame.Frame.Height, false, Transparency);
					}
				}

			}
			/*
			if (editorAnim2 != null && editorAnim2.Frames.Count != 0)
			{
				var frame = editorAnim2.Frames[Animation.index];
				d.DrawBitmap(frame.Texture, x + frame.Frame.CenterX, y + frame.Frame.CenterY,
					frame.Frame.Width, frame.Frame.Height, false, Transparency);
			}
			if (editorAnim3 != null && editorAnim3.Frames.Count != 0)
			{
				var frame = editorAnim3.Frames[Animation.index];
				d.DrawBitmap(frame.Texture, x + frame.Frame.CenterX, y + frame.Frame.CenterY,
					frame.Frame.Width, frame.Frame.Height, false, Transparency);
			}*/
		}

		public override string GetObjectName()
        {
            return "UIMedallionPanel";
        }
    }
}
