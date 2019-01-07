using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using ManiacEditor;
using Microsoft.Xna.Framework;
using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class HangPoint : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            int length = (int)entity.attributesMap["length"].ValueVar;
            int i = 0;
            bool fliph = false;
            bool flipv = false;
            var editorAnim = e.EditorInstance.EditorEntity_ini.LoadAnimation2("HangPoint", d, 0, 0, fliph, flipv, false);
            var editorAnim2 = e.EditorInstance.EditorEntity_ini.LoadAnimation2("HangPoint", d, 0, 1, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnim2 != null && editorAnim2.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                var frame2 = editorAnim2.Frames[0];
                int lengthMemory = length;

                //Animation.ProcessAnimation(frame.Entry.FrameSpeed, frame.Entry.Frames.Count, frame.Frame.Duration);
                    int repeat = 0;
                    int lengthLeft = length;
                    bool finalLoop = false;
                    while (lengthLeft > 256)
                    {
                        repeat++;
                        lengthLeft = lengthLeft - 256;
                    }
                    d.DrawBitmap(frame2.Texture,
                        x + frame2.Frame.CenterX - (fliph ? (frame2.Frame.Width - editorAnim2.Frames[0].Frame.Width) : 0),
                        y - (i * 256) + (flipv ? (frame2.Frame.Height - editorAnim2.Frames[0].Frame.Height) : 0),
                        frame2.Frame.Width, lengthMemory, false, Transparency);
                    for (i = 1; i < repeat+1; i++)
                    {
                    if (i == repeat)
                    {
                        finalLoop = true;
                    }
                        d.DrawBitmap(frame2.Texture,
                            x + frame2.Frame.CenterX - (fliph ? (frame2.Frame.Width - editorAnim2.Frames[0].Frame.Width) : 0),
                                y + (i * 256) + (flipv ? (frame2.Frame.Height - editorAnim2.Frames[0].Frame.Height) : 0),
                                frame2.Frame.Width, (finalLoop ? lengthLeft : frame2.Frame.Height), false, Transparency);
                    }
                    d.DrawBitmap(frame.Texture,
                        x + frame.Frame.CenterX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                        y + length + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                            frame.Frame.Width, frame.Frame.Height, false, Transparency);

            }
        }

        public override string GetObjectName()
        {
            return "HangPoint";
        }
    }
}
