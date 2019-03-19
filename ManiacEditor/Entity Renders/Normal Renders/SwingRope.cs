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
    public class SwingRope : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            int ropeSize = (int)entity.attributesMap["ropeSize"].ValueUInt8 + 1;
            var editorAnim = e.EditorInstance.EntityDrawing.LoadAnimation2("SwingRope", d, 0, -1, false, false, false);
            var editorAnim2 = e.EditorInstance.EntityDrawing.LoadAnimation2("SwingRope", d, 1, 0, false, false, false);
            var editorAnim3 = e.EditorInstance.EntityDrawing.LoadAnimation2("SwingRope", d, 2, 0, false, false, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnim2 != null && editorAnim2.Frames.Count != 0 && editorAnim3 != null && editorAnim3.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                var frame2 = editorAnim2.Frames[0];
                var frame3 = editorAnim3.Frames[0];

                for (int i = 0; i < ropeSize; i++)
                {
                    d.DrawBitmap(frame.Texture,
                        x + frame.Frame.PivotX,
                        y + frame.Frame.PivotY + frame.Frame.Height*i,
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
                }
                d.DrawBitmap(frame3.Texture,
                    x + frame3.Frame.PivotX,
                    y + frame3.Frame.PivotY,
                    frame3.Frame.Width, frame3.Frame.Height, false, Transparency);

                d.DrawBitmap(frame2.Texture,
                    x + frame2.Frame.PivotX,
                    y + frame2.Frame.PivotY + frame.Frame.Height * ropeSize,
                    frame2.Frame.Width, frame2.Frame.Height, false, Transparency);
            }
        }

        public override string GetObjectName()
        {
            return "SwingRope";
        }
    }
}
