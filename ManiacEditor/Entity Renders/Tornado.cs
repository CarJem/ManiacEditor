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
    public class Tornado : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency)
        {
            bool fliph = false;
            bool flipv = false;
            var editorAnim = e.LoadAnimation2("Tornado", d, 0, 0, fliph, flipv, false);
            var editorAnim2 = e.LoadAnimation2("Tornado", d, 1, -1, fliph, flipv, false);
            var editorAnim3 = e.LoadAnimation2("Tornado", d, 2, -1, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnim2 != null && editorAnim2.Frames.Count != 0 && editorAnim3 != null && editorAnim3.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                var frame2 = editorAnim2.Frames[e.index];
                var frame3 = editorAnim3.Frames[e.EditorAnimations.index];


                e.ProcessAnimation(frame2.Entry.FrameSpeed, frame2.Entry.Frames.Count, frame2.Frame.Duration);
                e.EditorAnimations.ProcessAnimation2(frame3.Entry.FrameSpeed, frame3.Entry.Frames.Count, frame3.Frame.Duration);

                d.DrawBitmap(frame3.Texture,
                    x + frame3.Frame.CenterX,
                    y + frame3.Frame.CenterY,
                    frame3.Frame.Width, frame3.Frame.Height, false, Transparency);
                d.DrawBitmap(frame.Texture,
                    x + frame.Frame.CenterX,
                    y + frame.Frame.CenterY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
                d.DrawBitmap(frame2.Texture,
                    x + frame2.Frame.CenterX,
                    y + frame2.Frame.CenterY,
                    frame2.Frame.Width, frame2.Frame.Height, false, Transparency);





            }
        }

        public override string GetObjectName()
        {
            return "Tornado";
        }
    }
}
