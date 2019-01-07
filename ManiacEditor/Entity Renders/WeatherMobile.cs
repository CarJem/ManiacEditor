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
    public class WeatherMobile : EntityRenderer
    {
        static int SEPERATE_VALUE = 145;
        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            bool fliph = false;
            bool flipv = false;
            var editorAnim = e.EditorInstance.EditorEntity_ini.LoadAnimation2("WeatherMobile", d, 0, 0, fliph, flipv, false);
            var editorAnim2 = e.EditorInstance.EditorEntity_ini.LoadAnimation2("WeatherMobile", d, 0, 1, fliph, flipv, false);
            var editorAnim3 = e.EditorInstance.EditorEntity_ini.LoadAnimation2("WeatherMobile", d, 0, 2, fliph, flipv, false);
            var editorAnim4 = e.EditorInstance.EditorEntity_ini.LoadAnimation2("WeatherMobile", d, 0, 3, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnim2 != null && editorAnim2.Frames.Count != 0 && editorAnim3 != null && editorAnim3.Frames.Count != 0 && editorAnim4 != null && editorAnim4.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                var frame2 = editorAnim2.Frames[0];
                var frame3 = editorAnim3.Frames[0];
                var frame4 = editorAnim4.Frames[0];

                //Animation.ProcessAnimation(frame.Entry.FrameSpeed, frame.Entry.Frames.Count, frame.Frame.Duration);

                d.DrawBitmap(frame.Texture,
                    x + frame.Frame.CenterX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                    y - SEPERATE_VALUE + frame.Frame.CenterY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
                d.DrawBitmap(frame4.Texture,
                    x + frame4.Frame.CenterX - (fliph ? (frame4.Frame.Width - editorAnim4.Frames[0].Frame.Width) : 0),
                    y - SEPERATE_VALUE + frame4.Frame.CenterY + (flipv ? (frame4.Frame.Height - editorAnim4.Frames[0].Frame.Height) : 0),
                    frame4.Frame.Width, frame4.Frame.Height, false, Transparency);
                d.DrawBitmap(frame2.Texture,
                    x + frame2.Frame.CenterX - (fliph ? (frame2.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                    y - SEPERATE_VALUE + frame2.Frame.CenterY + (flipv ? (frame2.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                    frame2.Frame.Width, frame.Frame.Height, false, 100);
                d.DrawBitmap(frame3.Texture,
                    x + frame3.Frame.CenterX - (fliph ? (frame3.Frame.Width - editorAnim3.Frames[0].Frame.Width) : 0),
                    y - SEPERATE_VALUE + frame3.Frame.CenterY + (flipv ? (frame3.Frame.Height - editorAnim3.Frames[0].Frame.Height) : 0),
                    frame3.Frame.Width, frame3.Frame.Height, false, 100);

            }
        }

        public override string GetObjectName()
        {
            return "WeatherMobile";
        }
    }
}
