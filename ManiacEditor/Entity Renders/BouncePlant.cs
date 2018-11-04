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
    public class BouncePlant : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency)
        {
            int direction = (int)entity.attributesMap["direction"].ValueUInt8;
            bool fliph = false;
            bool flipv = false;
            if (direction == 0)
            {
                fliph = true;
            }
            var editorAnim = e.LoadAnimation2("Plants", d, 1, -1, fliph, flipv, false);
            x += (fliph ? 42 : -42);
            y += -42;
            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];

                for (int i = 0; i < 8; i++)
                {
                    d.DrawBitmap(frame.Texture,
                        x + frame.Frame.CenterX + (fliph ? -(12 * i) : (12 * i)),
                        y + frame.Frame.CenterY + 12*i,
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
                }

            }
        }

        public override string GetObjectName()
        {
            return "BouncePlant";
        }
    }
}
