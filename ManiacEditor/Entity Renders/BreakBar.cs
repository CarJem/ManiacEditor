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
    public class BreakBar : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            var length = (short)(entity.attributesMap["length"].ValueUInt16);
            var orientation = entity.attributesMap["orientation"].ValueUInt8;
            if (orientation >= 2)
            {
                orientation = 0;
            }
            var editorAnim = e.EditorInstance.EditorEntity_ini.LoadAnimation2("BreakBar", d, orientation, -1, false, false, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                var frameTop = editorAnim.Frames[0];
                var frameBase = editorAnim.Frames[1];
                var frameBottom = editorAnim.Frames[2];
                for (int i = -length / 2; i <= length / 2; ++i)
                {
                    if (orientation == 1)
                    {

                    }
                    d.DrawBitmap(frameBase.Texture, x + frameBase.Frame.CenterX + (orientation == 1 ? i * frameBase.Frame.Width : 0),
                        y + frameBase.Frame.CenterY + (orientation == 0 ? i * frameBase.Frame.Height  : 0),
                        frameBase.Frame.Width, frameBase.Frame.Height, false, Transparency);
                }
                d.DrawBitmap(frameTop.Texture, x + frameTop.Frame.CenterX - (orientation == 1 ? (length / 2 + 1) * frameBase.Frame.Width : 0) + (orientation == 1 ? 4 : 0),
                    y + frameTop.Frame.CenterY - (orientation == 0 ? (length / 2 + 1) * frameBase.Frame.Height : 0) + (orientation == 0 ? 4 : 0),
                    frameTop.Frame.Width, frameTop.Frame.Height, false, Transparency);
                d.DrawBitmap(frameBottom.Texture, x + frameBottom.Frame.CenterX + (orientation == 1 ? (length / 2 + 1) * frameBase.Frame.Width : 0) - (orientation == 1 ? 4 : 0),
                    y + frameBottom.Frame.CenterY + (orientation == 0 ? (length / 2 + 1) * frameBase.Frame.Height : 0) - (orientation == 0 ? 4 : 0),
                    frameBottom.Frame.Width, frameBottom.Frame.Height, false, Transparency);

            }
        }

        public override string GetObjectName()
        {
            return "BreakBar";
        }
    }
}
