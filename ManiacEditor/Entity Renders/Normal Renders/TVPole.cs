using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ManiacEditor;
using Microsoft.Xna.Framework;
using RSDKv5;
using SharpDX.Direct3D9;

namespace ManiacEditor.Entity_Renders
{
    public class TVPole : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            var value = entity.attributesMap["length"].ValueUInt16 + 1;
            var editorAnim = Editor.Instance.EntityDrawing.LoadAnimation2("TVPole", d, 1, 0, false, false, false);
            var editorAnim2 = Editor.Instance.EntityDrawing.LoadAnimation2("TVPole", d, 1, 1, false, false, false);
            var editorAnim3 = Editor.Instance.EntityDrawing.LoadAnimation2("TVPole", d, 1, 2, false, false, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnim2 != null && editorAnim2.Frames.Count != 0 && editorAnim3 != null && editorAnim3.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                var frame2 = editorAnim2.Frames[0];
                var frame3 = editorAnim3.Frames[0];
                bool wEven = false;
                for (int xx = 0; xx <= value; ++xx)
                {
                    if (xx == 0)
                    {
                        d.DrawBitmap(frame.Texture,
                            x + (wEven ? frame2.Frame.PivotX : -frame.Frame.Width) + (-value / 2 + xx) * frame2.Frame.Width,
                            y + frame2.Frame.PivotY,
                            frame2.Frame.Width, frame2.Frame.Height, false, Transparency);
                    }
                    else if (xx == value)
                    {
                        d.DrawBitmap(frame3.Texture,
                            x + (wEven ? frame3.Frame.PivotX : -frame3.Frame.Width) + (-value / 2 + xx) * frame3.Frame.Width,
                            y + frame3.Frame.PivotY,
                            frame3.Frame.Width, frame3.Frame.Height, false, Transparency);
                    }
                    else
                    {
                        d.DrawBitmap(frame2.Texture,
                            x + (wEven ? frame2.Frame.PivotX : -frame2.Frame.Width) + (-value / 2 + xx) * frame2.Frame.Width,
                            y + frame2.Frame.PivotY,
                            frame2.Frame.Width, frame2.Frame.Height, false, Transparency);
                    }

                }
            }
        }

        public override string GetObjectName()
        {
            return "TVPole";
        }
    }
}
