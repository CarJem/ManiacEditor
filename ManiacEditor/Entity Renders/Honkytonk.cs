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
    public class Honkytonk : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            bool fliph = false;
            bool flipv = false;
            int angle = (int)entity.attributesMap["angle"].ValueVar;
            int rotation = (int)(angle / -0.71);


            var editorAnim = e.EditorInstance.EditorEntity_ini.LoadAnimation("HonkyTonk", d, 0, 0, fliph, flipv, true, rotation, true, false, false, true);
			var editorAnim2 = e.EditorInstance.EditorEntity_ini.LoadAnimation("HonkyTonk", d, 0, 1, fliph, flipv, true, rotation, true, false, false, true);
			var editorAnim3 = e.EditorInstance.EditorEntity_ini.LoadAnimation("HonkyTonk", d, 0, 2, fliph, flipv, true, rotation, true, false, false, true);
			var editorAnim4 = e.EditorInstance.EditorEntity_ini.LoadAnimation("HonkyTonk", d, 0, 3, fliph, flipv, true, rotation, true, false, false, true);
			var editorAnim5 = e.EditorInstance.EditorEntity_ini.LoadAnimation("HonkyTonk", d, 0, 4, fliph, flipv, true, rotation, true, false, false, true);


			if (editorAnim != null && editorAnim.Frames.Count != 0)
			{
				var frame = editorAnim.Frames[0];

				d.DrawBitmap(frame.Texture,
					x - (int)(frame.ImageWidth / 2),
					y - (int)(frame.ImageHeight / 2),
					frame.ImageWidth, frame.ImageHeight, false, Transparency);
			}
			if (editorAnim2 != null && editorAnim2.Frames.Count != 0)
			{
				var frame = editorAnim2.Frames[0];

				d.DrawBitmap(frame.Texture,
					x - (int)(frame.ImageWidth / 2),
					y - (int)(frame.ImageHeight / 2),
					frame.ImageWidth, frame.ImageHeight, false, Transparency);


			}
			if (editorAnim5 != null && editorAnim5.Frames.Count != 0)
			{
				var frame = editorAnim5.Frames[0];

				d.DrawBitmap(frame.Texture,
					x - (int)(frame.ImageWidth / 2),
					y - (int)(frame.ImageHeight / 2),
					frame.ImageWidth, frame.ImageHeight, false, Transparency);

			}


		}

        public override string GetObjectName()
        {
            return "Honkytonk";
        }
    }
}
