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
    public class SSZEggman : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            var editorAnim = e.EditorInstance.EditorEntity_ini.LoadAnimation2("EggmanSSZ", d, 0, -1, false, false, false);
            var editorAnimMobile = e.EditorInstance.EditorEntity_ini.LoadAnimation2("EggmanSSZ", d, 5, -1, false, false, false);
            var editorAnimSeat = e.EditorInstance.EditorEntity_ini.LoadAnimation2("EggmanSSZ", d, 4, -1, false, false, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnimMobile != null && editorAnimMobile.Frames.Count != 0 && editorAnimSeat != null && editorAnimSeat.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];
                var frameMobile = editorAnimMobile.Frames[0];
                var frameSeat = editorAnimSeat.Frames[0];

                Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);

                d.DrawBitmap(frameSeat.Texture,
                    x + frameSeat.Frame.PivotX,
                    y + frameSeat.Frame.PivotY,
                    frameSeat.Frame.Width, frameSeat.Frame.Height, false, Transparency);
                d.DrawBitmap(frame.Texture,
                    x + frame.Frame.PivotX,
                    y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
                d.DrawBitmap(frameMobile.Texture,
                    x + frameMobile.Frame.PivotX,
                    y + frameMobile.Frame.PivotY,
                    frameMobile.Frame.Width, frameMobile.Frame.Height, false, Transparency);
            }
        }

        public override string GetObjectName()
        {
            return "SSZEggman";
        }
    }
}
