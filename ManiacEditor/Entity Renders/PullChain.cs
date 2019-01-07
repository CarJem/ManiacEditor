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
    public class PullChain : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            int type = (int)entity.attributesMap["type"].ValueVar;
            bool decorMode = entity.attributesMap["decorMode"].ValueBool;
            int length = (int)entity.attributesMap["length"].ValueUInt32;
            int frameID = 0;
            if (decorMode == true)
            {
                frameID = 1;
            }
            bool fliph = false;
            bool flipv = false;
            var editorAnim = e.EditorInstance.EditorEntity_ini.LoadAnimation2("PullChain", d, 0, frameID, fliph, flipv, false);
            var editorAnimChain = e.EditorInstance.EditorEntity_ini.LoadAnimation2("PullChain", d, 1, frameID, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnimChain != null && editorAnimChain.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];
                var frameChain = editorAnimChain.Frames[0];

                Animation.ProcessAnimation(frame.Entry.FrameSpeed, frame.Entry.Frames.Count, frame.Frame.Duration);

                d.DrawBitmap(frame.Texture,
                    x + frame.Frame.CenterX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                    y + frame.Frame.CenterY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
                if (length != 0)
                {
                    for (int i = 0; i < length; i++)
                    {
                        d.DrawBitmap(frameChain.Texture,
                        x + frameChain.Frame.CenterX,
                        y + frameChain.Frame.CenterY - frameChain.Frame.Height * i,
                        frameChain.Frame.Width, frameChain.Frame.Height, false, Transparency);
                    }
                }
            }
        }

        public override string GetObjectName()
        {
            return "PullChain";
        }
    }
}
