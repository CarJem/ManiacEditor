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
    public class TimeAttackGate : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            bool finish = entity.attributesMap["finishLine"].ValueBool;
            var editorAnimBase = Editor.Instance.EntityDrawing.LoadAnimation2("SpeedGate", d, 0, 0, false, false, false);
            var editorAnimTop = Editor.Instance.EntityDrawing.LoadAnimation2("SpeedGate", d, 1, 0, false, false, false);
            var editorAnimFins = Editor.Instance.EntityDrawing.LoadAnimation2("SpeedGate", d, finish ? 4 : 3, -1, false, false, false);
            if (editorAnimBase != null && editorAnimTop != null && editorAnimFins != null && editorAnimFins.Frames.Count != 0 && editorAnimTop.Frames.Count != 0 && editorAnimTop.Frames.Count != 0)
            {
                var frameBase = editorAnimBase.Frames[0];
                var frameTop = editorAnimTop.Frames[0];
                d.DrawBitmap(frameBase.Texture, x + frameBase.Frame.PivotX, y + frameBase.Frame.PivotY,
                    frameBase.Frame.Width, frameBase.Frame.Height, false, Transparency);
                d.DrawBitmap(frameTop.Texture, x + frameTop.Frame.PivotX, y + frameTop.Frame.PivotY,
                    frameTop.Frame.Width, frameTop.Frame.Height, false, Transparency);
                for (int i = 0; i < editorAnimFins.Frames.Count; ++i)
                {
                    var frame = editorAnimFins.Frames[i];
                    d.DrawBitmap(frame.Texture, x + frame.Frame.PivotX, y + frame.Frame.PivotY,
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
                }
            }
        }

        public override string GetObjectName()
        {
            return "TimeAttackGate";
        }
    }
}
