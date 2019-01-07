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
    public class HangConveyor : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            bool fliph = false;
            int direction = (int)entity.attributesMap["direction"].ValueUInt8;
            int length = (int)entity.attributesMap["length"].ValueUInt32*16;
            if (direction == 1)
            {
                fliph = true;
            }
            var editorAnim = e.EditorInstance.EditorEntity_ini.LoadAnimation2("HangConveyor", d, 0, -1, fliph, false, false);
            var editorAnimEnd = e.EditorInstance.EditorEntity_ini.LoadAnimation2("HangConveyor", d, 1, -1, !fliph, false, false);
            var editorAnimMid = e.EditorInstance.EditorEntity_ini.LoadAnimation2("HangConveyor", d, 2, -1, fliph, false, false);
            var editorAnimMid2 = e.EditorInstance.EditorEntity_ini.LoadAnimation2("HangConveyor", d, 2, -1, !fliph, false, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnimEnd != null && editorAnimEnd.Frames.Count != 0 && editorAnimMid != null && editorAnimMid.Frames.Count != 0 && editorAnimMid2 != null && editorAnimMid2.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];
                var frameEnd = editorAnimEnd.Frames[Animation.index];
                var frameMid = editorAnimMid.Frames[Animation.index];
                var frameMid2 = editorAnimMid2.Frames[Animation.index];

                Animation.ProcessAnimation(frame.Entry.FrameSpeed, frame.Entry.Frames.Count, frame.Frame.Duration);

                d.DrawBitmap(frame.Texture,
                    x + frame.Frame.CenterX + (direction == 1 ? length / 2 : -(length / 2)),
                    y + frame.Frame.CenterY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);

                d.DrawBitmap(frameEnd.Texture,
                    x + frameEnd.Frame.CenterX - (direction == 1 ? length / 2 : -(length / 2)),
                    y + frameEnd.Frame.CenterY,
                    frameEnd.Frame.Width, frameEnd.Frame.Height, false, Transparency);

                int start_x = x + frameEnd.Frame.CenterX - length / 2 + frameEnd.Frame.Width - 6;
                int start_x2 = x + frameEnd.Frame.CenterX - length / 2 + frameEnd.Frame.Width - 10;
                int length2 = (length / 16 ) - 1;
                for (int i = 0; i < length2; i++)
                {
                    d.DrawBitmap(frameMid.Texture,
                        start_x + frameMid.Frame.CenterX + 16*i,
                        y - 21 + frameMid.Frame.CenterY,
                        frameMid.Frame.Width, frameMid.Frame.Height, false, Transparency);
                }

                for (int i = 0; i < length2; i++)
                {
                    d.DrawBitmap(frameMid2.Texture,
                        start_x2 + frameMid2.Frame.CenterX + 16 * i,
                        y + 21 + frameMid2.Frame.CenterY,
                        frameMid2.Frame.Width, frameMid2.Frame.Height, false, Transparency);
                }
            }
        }

        public override string GetObjectName()
        {
            return "HangConveyor";
        }
    }
}
