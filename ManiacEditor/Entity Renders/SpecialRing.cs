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
    public class SpecialRing : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency)
        {
            bool fliph = false;
            bool flipv = false;
            var editorAnim = e.LoadAnimation("SuperSpecialRing", d, 0, -1, fliph, flipv, false, 0);
            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[e.index];

                e.ProcessAnimation(frame.Entry.FrameSpeed, frame.Entry.Frames.Count, frame.Frame.Duration);

                d.DrawBitmap(frame.Texture,
                    x + frame.Frame.CenterX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                    y + frame.Frame.CenterY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }
        }

        public override string GetObjectName()
        {
            return "SpecialRing";
        }
    }
}
