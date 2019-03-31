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
    public class MeterDroid : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            var editorAnim = Editor.Instance.EntityDrawing.LoadAnimation2("MeterDroid", d, 1, -1, false, false, false);
            var editorAnim2 = Editor.Instance.EntityDrawing.LoadAnimation2("MeterDroid", d, 8, -1, false, false, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnim2 != null && editorAnim2.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];
                var frame2 = editorAnim2.Frames[Animation.index];


                Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);
                Animation.ProcessAnimation2(frame2.Entry.SpeedMultiplyer, frame2.Entry.Frames.Count, frame2.Frame.Delay);

                d.DrawBitmap(frame.Texture,
                    x + frame.Frame.PivotX,
                    y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
                d.DrawBitmap(frame2.Texture,
                    x + frame2.Frame.PivotX,
                    y + frame2.Frame.PivotY,
                    frame2.Frame.Width, frame2.Frame.Height, false, Transparency);
            }
        }

        public override string GetObjectName()
        {
            return "MeterDroid";
        }
    }
}
