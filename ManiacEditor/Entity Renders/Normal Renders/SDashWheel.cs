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
    public class SDashWheel : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            int direction = (int)entity.attributesMap["direction"].ValueUInt8;
            bool fliph = false;
            bool flipv = false;
            if (direction == 0)
            {
                fliph = true;
            }
            var editorAnim = Editor.Instance.EntityDrawing.LoadAnimation2("SDashWheel", d, 0, 0, fliph, flipv, false);
            var editorAnimKnob = Editor.Instance.EntityDrawing.LoadAnimation2("SDashWheel", d, 2, 0, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnimKnob != null && editorAnimKnob.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                var frameKnob = editorAnimKnob.Frames[0];

                d.DrawBitmap(frame.Texture,
                    x + frame.Frame.PivotX,
                    y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
                d.DrawBitmap(frameKnob.Texture,
                    x + frameKnob.Frame.PivotX,
                    y + frameKnob.Frame.PivotY,
                    frameKnob.Frame.Width, frameKnob.Frame.Height, false, Transparency);
            }
        }

        public override string GetObjectName()
        {
            return "SDashWheel";
        }
    }
}
