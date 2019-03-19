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
    public class FilmProjector : EntityRenderer
    {
        public DateTime lastFrametime;
        public int index = 0;

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            bool fliph = false;
            bool flipv = false;
            var editorAnim = e.EditorInstance.EntityDrawing.LoadAnimation2("FilmProjector", d, 0, 0, fliph, flipv, false);
            var editorAnim2 = e.EditorInstance.EntityDrawing.LoadAnimation2("FilmProjector", d, 0, 1, fliph, flipv, false);
            var editorAnim4 = e.EditorInstance.EntityDrawing.LoadAnimation2("FilmProjector", d, 1, 0, fliph, flipv, false);
            var editorAnim5 = e.EditorInstance.EntityDrawing.LoadAnimation2("FilmProjector", d, 2, 0, fliph, flipv, false);
            var editorAnim6 = e.EditorInstance.EntityDrawing.LoadAnimation2("FilmProjector", d, 1, 0, fliph, flipv, false);
            var editorAnim7 = e.EditorInstance.EntityDrawing.LoadAnimation2("FilmProjector", d, 2, 0, fliph, flipv, false);
            var editorAnim3 = e.EditorInstance.EntityDrawing.LoadAnimation2("FilmProjector", d, 3, -1, fliph, flipv, false);

            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnim2 != null && editorAnim2.Frames.Count != 0 && editorAnim3 != null && editorAnim3.Frames.Count != 0 && editorAnim4 != null && editorAnim4.Frames.Count != 0 && editorAnim5 != null && editorAnim5.Frames.Count != 0 && editorAnim6 != null && editorAnim6.Frames.Count != 0 && editorAnim7 != null && editorAnim7.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                var frame2 = editorAnim2.Frames[0];
                var frame4 = editorAnim4.Frames[0];
                var frame5 = editorAnim5.Frames[0];
                var frame6 = editorAnim4.Frames[0];
                var frame7 = editorAnim5.Frames[0];
                var frame3 = editorAnim3.Frames[Animation.index];

                d.DrawBitmap(frame5.Texture,
                    x + 42 + frame5.Frame.PivotX - (fliph ? (frame5.Frame.Width - editorAnim5.Frames[0].Frame.Width) : 0),
                    y - 68 + frame5.Frame.PivotY + (flipv ? (frame5.Frame.Height - editorAnim5.Frames[0].Frame.Height) : 0),
                    frame5.Frame.Width, frame5.Frame.Height, false, Transparency);
                d.DrawBitmap(frame4.Texture,
                    x + 42 + frame4.Frame.PivotX - (fliph ? (frame4.Frame.Width - editorAnim4.Frames[0].Frame.Width) : 0),
                    y - 68 + frame4.Frame.PivotY + (flipv ? (frame4.Frame.Height - editorAnim4.Frames[0].Frame.Height) : 0),
                    frame4.Frame.Width, frame4.Frame.Height, false, Transparency);


                d.DrawBitmap(frame7.Texture,
                    x - 60 + frame7.Frame.PivotX - (fliph ? (frame7.Frame.Width - editorAnim7.Frames[0].Frame.Width) : 0),
                    y - 68 + frame7.Frame.PivotY + (flipv ? (frame7.Frame.Height - editorAnim7.Frames[0].Frame.Height) : 0),
                    frame7.Frame.Width, frame7.Frame.Height, false, Transparency);
                d.DrawBitmap(frame6.Texture,
                    x - 60 + frame6.Frame.PivotX - (fliph ? (frame6.Frame.Width - editorAnim6.Frames[0].Frame.Width) : 0),
                    y - 68 + frame6.Frame.PivotY + (flipv ? (frame6.Frame.Height - editorAnim6.Frames[0].Frame.Height) : 0),
                    frame6.Frame.Width, frame6.Frame.Height, false, Transparency);


                d.DrawBitmap(frame.Texture,
                    x + frame.Frame.PivotX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                    y + frame.Frame.PivotY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
                d.DrawBitmap(frame2.Texture,
                    x + 185 + frame2.Frame.PivotX - (fliph ? (frame2.Frame.Width - editorAnim2.Frames[0].Frame.Width) : 0),
                    y + frame2.Frame.PivotY + (flipv ? (frame2.Frame.Height - editorAnim2.Frames[0].Frame.Height) : 0),
                    frame2.Frame.Width, frame2.Frame.Height, false, Transparency);

                ProcessAnimation(frame3.Entry.SpeedMultiplyer, frame3.Entry.Frames.Count, frame3.Frame.Delay, 0, e.EditorInstance);

                d.DrawBitmap(frame3.Texture,
                    x + 185 + frame3.Frame.PivotX - (fliph ? (frame3.Frame.Width - editorAnim3.Frames[0].Frame.Width) : 0),
                    y + frame3.Frame.PivotY + (flipv ? (frame3.Frame.Height - editorAnim3.Frames[0].Frame.Height) : 0),
                    frame3.Frame.Width, frame3.Frame.Height, false, Transparency);

            }
        }

        public void ProcessAnimation(int speed, int frameCount, int duration, int startFrame = 0, Editor EditorInstance = null)
        {
            // Playback
            if (EditorInstance.ShowAnimations.IsChecked.Value && EditorInstance.UIModes.AnnimationsChecked)
            {
                if (speed > 0)
                {
                    int speed1 = speed * 64 / (duration == 0 ? 256 : duration);
                    if (speed1 == 0)
                        speed1 = 1;
                    if ((DateTime.Now - lastFrametime).TotalMilliseconds > 1024 / speed1)
                    {
                        index++;
                        lastFrametime = DateTime.Now;
                    }
                }
            }
            else index = 0 + startFrame;
            if (index >= frameCount / 2)
                index = 0;

        }

        public override bool isObjectOnScreen(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency)
        {
            //TO-DO: Improve
            int bounds = 600;
            return d.IsObjectOnScreen(x - bounds / 2, y - bounds / 2, bounds, bounds);
        }

        public override string GetObjectName()
        {
            return "FilmProjector";
        }
    }
}
