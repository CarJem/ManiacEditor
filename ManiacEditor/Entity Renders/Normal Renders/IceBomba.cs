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
    public class IceBomba : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            bool fliph = false;
            bool flipv = false;
            int dir = entity.attributesMap["dir"].ValueUInt8;
            int frameID = 0;
            switch (dir)
            {
                case 1:
                    fliph = true;
                    frameID = 4;
                    break;

            }
            var editorAnim = e.EditorInstance.EntityDrawing.LoadAnimation2("IceBomba", d, 0, 0, fliph, flipv, false);
            var editorAnim2 = e.EditorInstance.EntityDrawing.LoadAnimation2("IceBomba", d, 1, -1, fliph, flipv, false);
            var editorAnim3 = e.EditorInstance.EntityDrawing.LoadAnimation2("IceBomba", d, 2, frameID, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnim2 != null && editorAnim2.Frames.Count != 0 && editorAnim3 != null && editorAnim3.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                var frame2 = editorAnim2.Frames[Animation.index];
                var frame3 = editorAnim3.Frames[0];

                Animation.ProcessAnimation(frame2.Entry.SpeedMultiplyer, frame2.Entry.Frames.Count, frame2.Frame.Delay);

                d.DrawBitmap(frame3.Texture,
                    x - 5 + frame.Frame.PivotX - (fliph ? 6 : 0),
                    y + 10 + (flipv ? 0 : 0),
                    frame3.Frame.Width, frame3.Frame.Height, false, Transparency);
                d.DrawBitmap(frame.Texture,
                    x + frame.Frame.PivotX,
                    y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
                d.DrawBitmap(frame2.Texture,
                    x + frame2.Frame.PivotX - (fliph ? 6 : 0),
                    y + frame2.Frame.PivotY,
                    frame2.Frame.Width, frame2.Frame.Height, false, Transparency);

            }
        }

        public override string GetObjectName()
        {
            return "IceBomba";
        }
    }
}
