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
    public class SpikeFlail : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            bool fliph = false;
            bool flipv = false;
            int chainLength = (int)entity.attributesMap["chainLength"].ValueUInt8;            
            var editorAnim = e.EditorInstance.EditorEntity_ini.LoadAnimation2("SpikeFlail", d, 0, 0, fliph, flipv, false);
            var editorAnimBall = e.EditorInstance.EditorEntity_ini.LoadAnimation2("SpikeFlail", d, 1, 0, fliph, flipv, false);
            var editorAnimRing1 = e.EditorInstance.EditorEntity_ini.LoadAnimation2("SpikeFlail", d, 2, 0, fliph, flipv, false);
            var editorAnimRing2 = e.EditorInstance.EditorEntity_ini.LoadAnimation2("SpikeFlail", d, 3, 0, fliph, flipv, false);
            var editorAnimRing3 = e.EditorInstance.EditorEntity_ini.LoadAnimation2("SpikeFlail", d, 4, 0, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnimBall != null && editorAnimBall.Frames.Count != 0 && editorAnimRing1 != null && editorAnimRing1.Frames.Count != 0 && editorAnimRing2 != null && editorAnimRing2.Frames.Count != 0 && editorAnimRing3 != null && editorAnimRing3.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                var frameBall = editorAnimBall.Frames[0];
                var frameRing1 = editorAnimRing1.Frames[0];
                var frameRing2 = editorAnimRing2.Frames[0];
                var frameRing3 = editorAnimRing3.Frames[0];

                //Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);

                d.DrawBitmap(frame.Texture,
                    x + frame.Frame.PivotX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                    y + frame.Frame.PivotY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
               
                if (chainLength >= 1)
                {
                    for (int i = 0; i < chainLength; i++)
                    {
                        var frameRingI = editorAnimRing1.Frames[0];
                        if (i + 1 % 1 == 0)
                        {
                            frameRingI = editorAnimRing1.Frames[0];
                        }
                        else if (i + 1 % 2 == 0)
                        {
                            frameRingI = editorAnimRing2.Frames[0];
                        }
                        else if (i + 1 % 3 == 0)
                        {
                            frameRingI = editorAnimRing3.Frames[0];
                        }
                        else
                        {
                            frameRingI = editorAnimRing1.Frames[0];
                        }

                        d.DrawBitmap(frameRingI.Texture,
                            x + frameRingI.Frame.PivotX + frame.Frame.Width + frameRingI.Frame.Width*(i),
                            y + frameRingI.Frame.PivotY,
                            frameRingI.Frame.Width, frameRingI.Frame.Height, false, Transparency);
                    }
                    d.DrawBitmap(frameBall.Texture,
                        x + frameBall.Frame.PivotX + frame.Frame.Width + frameRing1.Frame.Width*(chainLength+1),
                        y + frameBall.Frame.PivotY + (flipv ? (frameBall.Frame.Height - editorAnimBall.Frames[0].Frame.Height) : 0),
                        frameBall.Frame.Width, frameBall.Frame.Height, false, Transparency);
                }
            }
        }

        public override string GetObjectName()
        {
            return "SpikeFlail";
        }
    }
}
