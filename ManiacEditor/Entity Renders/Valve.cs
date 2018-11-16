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
    public class Valve : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency)
        {
            int direction = (int)entity.attributesMap["direction"].ValueUInt8;
            bool fliph = false;
            bool flipv = false;

            switch (direction)
            {
                case 0:
                    break;
                case 1:
                    fliph = true;
                    break;
                case 2:
                    flipv = true;
                    break;
                case 3:
                    flipv = true;
                    fliph = true;
                    break;
            }

            var editorAnim = e.LoadAnimation2("Valve", d, 0, -1, fliph, flipv, false);
            var editorAnim2 = e.LoadAnimation2("Valve", d, 2, 0, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnim2 != null && editorAnim2.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[e.index];
                var frame2 = editorAnim2.Frames[0];

                e.ProcessAnimation(frame.Entry.FrameSpeed, frame.Entry.Frames.Count, frame.Frame.Duration);

                d.DrawBitmap(frame2.Texture,
                    x + frame2.Frame.CenterX ,
                    y + frame2.Frame.CenterY - frame2.Frame.Height,
                    frame2.Frame.Width, frame2.Frame.Height, false, Transparency);
                d.DrawBitmap(frame.Texture,
                    x + frame.Frame.CenterX,
                    y + frame.Frame.CenterY - frame.Frame.Height,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);

            }
        }

        public override string GetObjectName()
        {
            return "Valve";
        }
    }
}
