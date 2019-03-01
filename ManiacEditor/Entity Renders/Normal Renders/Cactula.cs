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
    public class Cactula : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            bool fliph = false;
            bool flipv = false;
            var editorAnim = e.EditorInstance.EditorEntity_ini.LoadAnimation2("Cactula", d, 0, 0, fliph, flipv, false);
            var editorAnim2 = e.EditorInstance.EditorEntity_ini.LoadAnimation2("Cactula", d, 0, 1, fliph, flipv, false);
            var editorAnim3 = e.EditorInstance.EditorEntity_ini.LoadAnimation2("Cactula", d, 1, -1, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnim2 != null && editorAnim2.Frames.Count != 0 && editorAnim3 != null && editorAnim3.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                var frameHead = editorAnim2.Frames[0];
                var framePropel = editorAnim3.Frames[5];

                //Animation.ProcessAnimation(framePropel.Entry.SpeedMultiplyer, framePropel.Entry.Frames.Count, framePropel.Frame.Delay, 5);

                d.DrawBitmap(frame.Texture,
                    x + frame.Frame.PivotX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                    y + frame.Frame.PivotY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);

                d.DrawBitmap(frameHead.Texture,
                    x + frameHead.Frame.PivotX - (fliph ? (frameHead.Frame.Width - editorAnim2.Frames[0].Frame.Width) : 0),
                    y + frameHead.Frame.PivotY + (flipv ? (frameHead.Frame.Height - editorAnim2.Frames[0].Frame.Height) : 0),
                    frameHead.Frame.Width, frameHead.Frame.Height, false, Transparency);

                d.DrawBitmap(framePropel.Texture,
                    x + framePropel.Frame.PivotX - (fliph ? (framePropel.Frame.Width - editorAnim3.Frames[0].Frame.Width) : 0),
                    y + framePropel.Frame.PivotY + (flipv ? (framePropel.Frame.Height - editorAnim3.Frames[0].Frame.Height) : 0),
                    framePropel.Frame.Width, framePropel.Frame.Height, false, Transparency);

            }
        }

        public override string GetObjectName()
        {
            return "Cactula";
        }
    }
}
