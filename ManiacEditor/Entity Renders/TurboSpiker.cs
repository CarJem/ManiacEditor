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
    public class TurboSpiker : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency)
        {
            int type = (int)entity.attributesMap["type"].ValueVar;
            int direction = (int)entity.attributesMap["direction"].ValueUInt8;
            bool fliph = false;
            bool flipv = false;
            if (direction == 1)
            {
                fliph = true;
            }
            var editorAnim = e.LoadAnimation2("TurboSpiker", d, 0, -1, fliph, flipv, false);
            var editorAnimShell = e.LoadAnimation2("TurboSpiker", d, 3, 0, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnimShell != null && editorAnimShell.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[e.index];
                var frameShell = editorAnimShell.Frames[0];

                e.ProcessAnimation(frame.Entry.FrameSpeed, frame.Entry.Frames.Count, frame.Frame.Duration);

                d.DrawBitmap(frameShell.Texture,
                    x + frameShell.Frame.CenterX - (fliph ? (frameShell.Frame.Width - editorAnimShell.Frames[0].Frame.Width) : 0),
                    y + frameShell.Frame.CenterY + (flipv ? (frameShell.Frame.Height - editorAnimShell.Frames[0].Frame.Height) : 0),
                    frameShell.Frame.Width, frameShell.Frame.Height, false, Transparency);
                d.DrawBitmap(frame.Texture,
                    x + frame.Frame.CenterX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                    y + frame.Frame.CenterY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);

            }
        }

        public override string GetObjectName()
        {
            return "TurboSpiker";
        }
    }
}
