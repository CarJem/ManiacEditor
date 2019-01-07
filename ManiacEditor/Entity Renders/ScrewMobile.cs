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
    public class ScrewMobile : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            bool fliph = false;
            bool flipv = false;
            var editorAnim = e.EditorInstance.EditorEntity_ini.LoadAnimation2("ScrewMobile", d, 0, 0, fliph, flipv, false);
            var editorAnimSeat = e.EditorInstance.EditorEntity_ini.LoadAnimation2("ScrewMobile", d, 0, 1, fliph, flipv, false);
            var editorAnimLaunch = e.EditorInstance.EditorEntity_ini.LoadAnimation2("ScrewMobile", d, 0, 2, fliph, flipv, false);
            var editorAnimPropel = e.EditorInstance.EditorEntity_ini.LoadAnimation2("ScrewMobile", d, 1, -1, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnimSeat != null && editorAnimSeat.Frames.Count != 0 && editorAnimLaunch != null && editorAnimLaunch.Frames.Count != 0 && editorAnimPropel != null && editorAnimPropel.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                var frameSeat = editorAnimSeat.Frames[0];
                var frameLaunch = editorAnimLaunch.Frames[0];
                var framePropel = editorAnimPropel.Frames[Animation.index];

                Animation.ProcessAnimation(framePropel.Entry.FrameSpeed, framePropel.Entry.Frames.Count, framePropel.Frame.Duration);

                d.DrawBitmap(frameSeat.Texture,
                    x + frameSeat.Frame.CenterX,
                    y + frameSeat.Frame.CenterY,
                    frameSeat.Frame.Width, frameSeat.Frame.Height, false, Transparency);
                d.DrawBitmap(frame.Texture,
                    x + frame.Frame.CenterX,
                    y + frame.Frame.CenterY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
                d.DrawBitmap(frameLaunch.Texture,
                    x + frameLaunch.Frame.CenterX,
                    y + frameLaunch.Frame.CenterY,
                    frameLaunch.Frame.Width, frameLaunch.Frame.Height, false, Transparency);
                d.DrawBitmap(framePropel.Texture,
                    x + framePropel.Frame.CenterX,
                    y + framePropel.Frame.CenterY,
                    framePropel.Frame.Width, framePropel.Frame.Height, false, Transparency);


            }
        }

        public override string GetObjectName()
        {
            return "ScrewMobile";
        }
    }
}
