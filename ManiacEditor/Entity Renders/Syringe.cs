using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ManiacEditor;
using Microsoft.Xna.Framework;
using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Syringe : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency)
        {
            int type = (int)entity.attributesMap["type"].ValueVar;
            System.Drawing.Color colour;
            switch (type)
            {
                case 0:
                    colour = System.Drawing.Color.FromArgb(140, 0, 8, 192);
                    break;
                case 1:
                    colour = System.Drawing.Color.FromArgb(140, 8, 184, 0);
                    break;
                case 2:
                    colour = System.Drawing.Color.FromArgb(140, 56, 168, 240);
                    break;
                default:
                    colour = System.Drawing.Color.FromArgb(140, 56, 168, 240);
                    break;
            }
            bool fliph = false;
            bool flipv = false;
            int animID = 0;
            var editorAnim = e.LoadAnimation2("Syringe", d, animID, -1, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && animID >= 0)
            {
                var frame = editorAnim.Frames[0];
                var frame2 = editorAnim.Frames[1];

                //ProcessAnimation(frame.Entry.FrameSpeed, frame.Entry.Frames.Count, frame.Frame.Duration);
                d.DrawRectangle(x - 14, y + 80, x + 14, y, colour);
                d.DrawBitmap(frame.Texture,
                    x + frame.Frame.CenterX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                    y + frame.Frame.CenterY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
                d.DrawBitmap(frame2.Texture,
                    x + frame2.Frame.CenterX - (fliph ? (frame2.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                    y + frame2.Frame.CenterY + (flipv ? (frame2.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                    frame2.Frame.Width, frame2.Frame.Height, false, Transparency);

            }
        }

        public override string GetObjectName()
        {
            return "Syringe";
        }
    }
}
