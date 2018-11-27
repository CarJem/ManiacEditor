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
    public class Tubinaut : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            bool fliph = false;
            bool flipv = false;
            var editorAnim = EditorEntity_ini.LoadAnimation2("Tubinaut", d, 0, 0, fliph, flipv, false);
            var editorAnim2 = EditorEntity_ini.LoadAnimation2("Tubinaut", d, 8, -1, fliph, flipv, false);
            var editorAnim3 = EditorEntity_ini.LoadAnimation2("Tubinaut", d, 1, 0, fliph, flipv, false);
            var editorAnim4 = EditorEntity_ini.LoadAnimation2("Tubinaut", d, 2, 8, fliph, flipv, false);
            var editorAnim5 = EditorEntity_ini.LoadAnimation2("Tubinaut", d, 3, 12, fliph, flipv, false);
            var editorAnim6 = EditorEntity_ini.LoadAnimation2("Tubinaut", d, 0, 1, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnim2 != null && editorAnim2.Frames.Count != 0 && editorAnim3 != null && editorAnim3.Frames.Count != 0 && editorAnim4 != null && editorAnim4.Frames.Count != 0 && editorAnim5 != null && editorAnim5.Frames.Count != 0 && editorAnim6 != null && editorAnim6.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                var frame2 = editorAnim2.Frames[Animation.index];
                var frame3 = editorAnim4.Frames[0];
                var frame4 = editorAnim3.Frames[0];
                var frame5 = editorAnim5.Frames[0];
                var frame6 = editorAnim6.Frames[0];

                Animation.ProcessAnimation(frame2.Entry.FrameSpeed, frame2.Entry.Frames.Count, frame2.Frame.Duration);

                if (Editor.Instance.ShowAnimations.Checked == true && Properties.EditorState.Default.annimationsChecked)
                {

                    d.DrawBitmap(frame6.Texture,
                        x + frame6.Frame.CenterX - (fliph ? (frame6.Frame.Width - editorAnim6.Frames[0].Frame.Width) : 0),
                        y + frame6.Frame.CenterY + (flipv ? (frame6.Frame.Height - editorAnim6.Frames[0].Frame.Height) : 0),
                        frame6.Frame.Width, frame6.Frame.Height, false, Transparency);
                    d.DrawBitmap(frame2.Texture,
                        x + frame2.Frame.CenterX - (fliph ? (frame2.Frame.Width - editorAnim2.Frames[0].Frame.Width) : 0),
                        y + frame2.Frame.CenterY + (flipv ? (frame2.Frame.Height - editorAnim2.Frames[0].Frame.Height) : 0),
                        frame2.Frame.Width, frame2.Frame.Height, false, Transparency);
                }
                else
                {
                    d.DrawBitmap(frame.Texture,
                        x + frame.Frame.CenterX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                        y + frame.Frame.CenterY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
                    d.DrawBitmap(frame3.Texture,
                        x + 16 + frame3.Frame.CenterX - (fliph ? (frame3.Frame.Width - editorAnim3.Frames[0].Frame.Width) : 0),
                        y + 16 + frame3.Frame.CenterY + (flipv ? (frame3.Frame.Height - editorAnim3.Frames[0].Frame.Height) : 0),
                        frame3.Frame.Width, frame3.Frame.Height, false, Transparency);
                    d.DrawBitmap(frame4.Texture,
                        x - 16 + frame4.Frame.CenterX - (fliph ? (frame4.Frame.Width - editorAnim4.Frames[0].Frame.Width) : 0),
                        y + 16 + frame4.Frame.CenterY + (flipv ? (frame4.Frame.Height - editorAnim4.Frames[0].Frame.Height) : 0),
                        frame4.Frame.Width, frame4.Frame.Height, false, Transparency);
                    d.DrawBitmap(frame5.Texture,
                        x + frame5.Frame.CenterX - (fliph ? (frame5.Frame.Width - editorAnim5.Frames[0].Frame.Width) : 0),
                        y - 24 + frame5.Frame.CenterY + (flipv ? (frame5.Frame.Height - editorAnim5.Frames[0].Frame.Height) : 0),
                        frame5.Frame.Width, frame5.Frame.Height, false, Transparency);
                }

            }
        }

        public override string GetObjectName()
        {
           return "Tubinaut";
        }
    }
}
