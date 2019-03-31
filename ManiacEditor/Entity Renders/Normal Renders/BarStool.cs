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
    public class BarStool : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            bool fliph = false;
            bool flipv = false;
            int height = (int)entity.attributesMap["height"].ValueUInt8;
            var editorAnim = Editor.Instance.EntityDrawing.LoadAnimation2("BarStool", d, 0, -1, fliph, flipv, false);
            var editorAnimBase = Editor.Instance.EntityDrawing.LoadAnimation2("BarStool", d, 1, height, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnimBase != null && editorAnimBase.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];
                var frame2 = editorAnimBase.Frames[0];

                Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);

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
            return "BarStool";
        }
    }
}
