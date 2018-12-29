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
    public class UIButtonBack : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            var editorAnim2 = EditorEntity_ini.LoadAnimation("Buttons", d, 0, 0, true, false, false);
            var editorAnim3 = EditorEntity_ini.LoadAnimation("Buttons", d, 0, 0, true, false, false);
            var editorAnim4 = EditorEntity_ini.LoadAnimation("Buttons", d, 0, 0, false, true, false);
            var editorAnim5 = EditorEntity_ini.LoadAnimation("Buttons", d, 0, 0, false, true, false);
            if (editorAnim2 != null && editorAnim2.Frames.Count != 0)
            {
                int x2 = x - 64;
                int y2 = y;
                var frame = editorAnim2.Frames[0];
                d.DrawBitmap(frame.Texture, x2 + frame.Frame.CenterX, y2 + frame.Frame.CenterY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }
            if (editorAnim3 != null && editorAnim3.Frames.Count != 0)
            {
                int x2 = x - 72;
                int y2 = y + 8;
                var frame = editorAnim3.Frames[0];
                d.DrawBitmap(frame.Texture, x2 + frame.Frame.CenterX, y2 + frame.Frame.CenterY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }
            if (editorAnim4 != null && editorAnim4.Frames.Count != 0)
            {
                int x2 = x - 8;
                int y2 = y + 8;
                var frame = editorAnim4.Frames[0];
                d.DrawBitmap(frame.Texture, x2 + frame.Frame.CenterX, y2 + frame.Frame.CenterY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }
            if (editorAnim5 != null && editorAnim5.Frames.Count != 0)
            {
                int x2 = x;
                int y2 = y;
                var frame = editorAnim5.Frames[0];
                d.DrawBitmap(frame.Texture, x2 + frame.Frame.CenterX, y2 + frame.Frame.CenterY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }


        }

        public override string GetObjectName()
        {
            return "UIButtonBack";
        }
    }
}
