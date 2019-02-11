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
    public class HotaruHiWatt : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            var editorAnim = e.EditorInstance.EditorEntity_ini.LoadAnimation2("HotaruHiWatt", d, 0, -1, false, false, false);
            var editorAnimBulb = e.EditorInstance.EditorEntity_ini.LoadAnimation2("HotaruHiWatt", d, 1, -1, false, false, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnimBulb != null && editorAnimBulb.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];
                var frameBulb = editorAnimBulb.Frames[Animation.index];

                Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);
                Animation.ProcessAnimation2(frameBulb.Entry.SpeedMultiplyer, frameBulb.Entry.Frames.Count, frameBulb.Frame.Delay);

                d.DrawBitmap(frame.Texture,
                    x + frame.Frame.PivotX,
                    y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
                d.DrawBitmap(frameBulb.Texture,
                    x + frameBulb.Frame.PivotX,
                    y + frameBulb.Frame.PivotY,
                    frameBulb.Frame.Width, frameBulb.Frame.Height, false, Transparency);
            }
        }

        public override string GetObjectName()
        {
            return "HotaruHiWatt";
        }
    }
}
