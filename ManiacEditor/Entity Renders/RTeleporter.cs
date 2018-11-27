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
    public class RTeleporter : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            bool fliph = false;
            bool flipv = false;
            var editorAnim = EditorEntity_ini.LoadAnimation2("RGenerator", d, 0, -1, fliph, flipv, false);
            var editorAnimBottom = EditorEntity_ini.LoadAnimation2("RGenerator", d, 0, -1, false, true, false);
            var editorAnimElectric = EditorEntity_ini.LoadAnimation2("RGenerator", d, 1, -1, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnimBottom != null && editorAnimBottom.Frames.Count != 0 && editorAnimElectric != null && editorAnimElectric.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];
                var frameB = editorAnimBottom.Frames[Animation.index];
                var frameE = editorAnimElectric.Frames[Animation.index];

                Animation.ProcessAnimation(frame.Entry.FrameSpeed, frame.Entry.Frames.Count, frame.Frame.Duration);
                Animation.ProcessAnimation2(frameE.Entry.FrameSpeed, frameE.Entry.Frames.Count, frameE.Frame.Duration);

                d.DrawBitmap(frame.Texture,
                    x + frame.Frame.CenterX,
                    y + frame.Frame.CenterY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);

                d.DrawBitmap(frameB.Texture,
                    x + frameB.Frame.CenterX - (fliph ? (frameB.Frame.Width - editorAnimBottom.Frames[0].Frame.Width) : 0),
                    y + frameE.Frame.Height/2,
                    frameB.Frame.Width, frameB.Frame.Height, false, Transparency);

                d.DrawBitmap(frameE.Texture,
                    x + frameE.Frame.CenterX - 22,
                    y + frameE.Frame.CenterY,
                    frameE.Frame.Width, frame.Frame.Height, false, Transparency);
                d.DrawBitmap(frameE.Texture,
                    x + frameE.Frame.CenterX - 6,
                    y + frameE.Frame.CenterY,
                    frameE.Frame.Width, frame.Frame.Height, false, Transparency);
                d.DrawBitmap(frameE.Texture,
                    x + frameE.Frame.CenterX + 10,
                    y + frameE.Frame.CenterY,
                    frameE.Frame.Width, frame.Frame.Height, false, Transparency);
            }
        }

        public override string GetObjectName()
        {
            return "RTeleporter";
        }
    }
}
