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
    public class GiantPistol : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            bool fliph = false;
            bool flipv = false;
            int direction = (int)entity.attributesMap["direction"].ValueUInt8;
            if (direction == 1)
            {
                fliph = true;
            }
            var editorAnim = e.EditorInstance.EditorEntity_ini.LoadAnimation2("Pistol", d, 0, 0, fliph, flipv, false);
            var editorAnim2 = e.EditorInstance.EditorEntity_ini.LoadAnimation2("Pistol", d, 0, 1, fliph, flipv, false);
            var editorAnim3 = e.EditorInstance.EditorEntity_ini.LoadAnimation2("Pistol", d, 4, 0, fliph, flipv, false);
            var editorAnim4 = e.EditorInstance.EditorEntity_ini.LoadAnimation2("Pistol", d, 5, 0, fliph, flipv, false);
            var editorAnim5 = e.EditorInstance.EditorEntity_ini.LoadAnimation2("Pistol", d, 6, 0, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnim2 != null && editorAnim2.Frames.Count != 0 && editorAnim3 != null && editorAnim3.Frames.Count != 0 && editorAnim4 != null && editorAnim4.Frames.Count != 0 && editorAnim5 != null && editorAnim5.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                var frame2 = editorAnim2.Frames[0];
                var frame3 = editorAnim3.Frames[0];
                var frame4 = editorAnim4.Frames[0];
                var frame5 = editorAnim5.Frames[0];

                d.DrawBitmap(frame2.Texture,
                    x + frame2.Frame.CenterX - (fliph ? 76 : 0),
                    y + frame2.Frame.CenterY,
                    frame2.Frame.Width, frame2.Frame.Height, false, Transparency);
                d.DrawBitmap(frame.Texture,
                    x + frame.Frame.CenterX,
                    y + frame.Frame.CenterY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
                d.DrawBitmap(frame3.Texture,
                    x + frame3.Frame.CenterX - (fliph ? 59 : 0),
                    y + frame3.Frame.CenterY,
                    frame3.Frame.Width, frame3.Frame.Height, false, Transparency);
                d.DrawBitmap(frame5.Texture,
                    x + frame5.Frame.CenterX - (fliph ? frame.Frame.Width + 4: 0),
                    y + frame5.Frame.CenterY,
                    frame5.Frame.Width, frame5.Frame.Height, false, Transparency);

            }
        }

        public override string GetObjectName()
        {
            return "GiantPistol";
        }
    }
}
