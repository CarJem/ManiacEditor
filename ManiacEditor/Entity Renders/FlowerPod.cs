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
    public class FlowerPod : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            bool fliph = false;
            bool flipv = false;
            var editorAnim = e.EditorInstance.EditorEntity_ini.LoadAnimation2("FlowerPod", d, 0, 0, fliph, flipv, false);
            var editorAnimHead = e.EditorInstance.EditorEntity_ini.LoadAnimation2("FlowerPod", d, 1, 0, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnimHead != null && editorAnimHead.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                var frameHead = editorAnimHead.Frames[0];

                //Animation Currently Doesn't work
                //Animation.ProcessAnimation(frameHead.Entry.FrameSpeed, frameHead.Entry.Frames.Count, frameHead.Frame.Duration);

                d.DrawBitmap(frameHead.Texture,
                    x + frameHead.Frame.CenterX,
                    y + frameHead.Frame.CenterY,
                    frameHead.Frame.Width, frameHead.Frame.Height, false, Transparency);
                d.DrawBitmap(frame.Texture,
                    x + frame.Frame.CenterX,
                    y + frame.Frame.CenterY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);

            }
        }

        public override string GetObjectName()
        {
            return "FlowerPod";
        }
    }
}
