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
            var editorAnim = e.EditorInstance.EditorEntity_ini.LoadAnimation2("Tubinaut", d, 0, 0, fliph, flipv, false);
            var editorAnim2 = e.EditorInstance.EditorEntity_ini.LoadAnimation2("Tubinaut", d, 8, -1, fliph, flipv, false);
            var editorAnim3 = e.EditorInstance.EditorEntity_ini.LoadAnimation2("Tubinaut", d, 1, 0, fliph, flipv, false);
            var editorAnim4 = e.EditorInstance.EditorEntity_ini.LoadAnimation2("Tubinaut", d, 2, 8, fliph, flipv, false);
            var editorAnim5 = e.EditorInstance.EditorEntity_ini.LoadAnimation2("Tubinaut", d, 3, 12, fliph, flipv, false);
            var editorAnim6 = e.EditorInstance.EditorEntity_ini.LoadAnimation2("Tubinaut", d, 0, 1, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnim2 != null && editorAnim2.Frames.Count != 0 && editorAnim3 != null && editorAnim3.Frames.Count != 0 && editorAnim4 != null && editorAnim4.Frames.Count != 0 && editorAnim5 != null && editorAnim5.Frames.Count != 0 && editorAnim6 != null && editorAnim6.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                var frame2 = editorAnim2.Frames[Animation.index];
                var frame3 = editorAnim4.Frames[0];
                var frame4 = editorAnim3.Frames[0];
                var frame5 = editorAnim5.Frames[0];
                var frame6 = editorAnim6.Frames[0];

                Animation.ProcessAnimation(frame2.Entry.SpeedMultiplyer, frame2.Entry.Frames.Count, frame2.Frame.Delay);

                if (e.EditorInstance.ShowAnimations.IsChecked.Value == true && e.EditorInstance.AnnimationsChecked)
                {

                    d.DrawBitmap(frame6.Texture,
                        x + frame6.Frame.PivotX - (fliph ? (frame6.Frame.Width - editorAnim6.Frames[0].Frame.Width) : 0),
                        y + frame6.Frame.PivotY + (flipv ? (frame6.Frame.Height - editorAnim6.Frames[0].Frame.Height) : 0),
                        frame6.Frame.Width, frame6.Frame.Height, false, Transparency);
                    d.DrawBitmap(frame2.Texture,
                        x + frame2.Frame.PivotX - (fliph ? (frame2.Frame.Width - editorAnim2.Frames[0].Frame.Width) : 0),
                        y + frame2.Frame.PivotY + (flipv ? (frame2.Frame.Height - editorAnim2.Frames[0].Frame.Height) : 0),
                        frame2.Frame.Width, frame2.Frame.Height, false, Transparency);
                }
                else
                {
                    d.DrawBitmap(frame.Texture,
                        x + frame.Frame.PivotX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                        y + frame.Frame.PivotY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
                    d.DrawBitmap(frame3.Texture,
                        x + 16 + frame3.Frame.PivotX - (fliph ? (frame3.Frame.Width - editorAnim3.Frames[0].Frame.Width) : 0),
                        y + 16 + frame3.Frame.PivotY + (flipv ? (frame3.Frame.Height - editorAnim3.Frames[0].Frame.Height) : 0),
                        frame3.Frame.Width, frame3.Frame.Height, false, Transparency);
                    d.DrawBitmap(frame4.Texture,
                        x - 16 + frame4.Frame.PivotX - (fliph ? (frame4.Frame.Width - editorAnim4.Frames[0].Frame.Width) : 0),
                        y + 16 + frame4.Frame.PivotY + (flipv ? (frame4.Frame.Height - editorAnim4.Frames[0].Frame.Height) : 0),
                        frame4.Frame.Width, frame4.Frame.Height, false, Transparency);
                    d.DrawBitmap(frame5.Texture,
                        x + frame5.Frame.PivotX - (fliph ? (frame5.Frame.Width - editorAnim5.Frames[0].Frame.Width) : 0),
                        y - 24 + frame5.Frame.PivotY + (flipv ? (frame5.Frame.Height - editorAnim5.Frames[0].Frame.Height) : 0),
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
