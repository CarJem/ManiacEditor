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
    public class Buggernaut : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency)
        {
            int direction = (int)entity.attributesMap["direction"].ValueUInt8;
            bool fliph = false;
            bool flipv = false;
            if (direction == 1)
            {
                fliph = true;
            }
            var editorAnim = e.LoadAnimation2("Buggernaut", d, 0, 0, fliph, flipv, false);
            var editorAnimWings = e.LoadAnimation2("Buggernaut", d, 2, -1, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnimWings != null && editorAnimWings.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                var frameWings = editorAnimWings.Frames[e.index];

                e.ProcessAnimation(frameWings.Entry.FrameSpeed, frameWings.Entry.Frames.Count, frameWings.Frame.Duration);

                d.DrawBitmap(frameWings.Texture,
                    x + frameWings.Frame.CenterX - (fliph ? (frameWings.Frame.Width - editorAnimWings.Frames[0].Frame.Width) : 0),
                    y + frameWings.Frame.CenterY + (flipv ? (frameWings.Frame.Height - editorAnimWings.Frames[0].Frame.Height) : 0),
                    frameWings.Frame.Width, frameWings.Frame.Height, false, Transparency - 150);
                d.DrawBitmap(frame.Texture,
                    x + frame.Frame.CenterX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                    y + frame.Frame.CenterY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);

            }
        }

        public override string GetObjectName()
        {
            return "Buggernaut";
        }
    }
}
