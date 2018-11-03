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
    public class Decoration : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency)
        {
            bool flipv = false;
            bool fliph = false;
            var type = entity.attributesMap["type"].ValueUInt8;
            var direction = entity.attributesMap["direction"].ValueUInt8;
            var repeatSpacing = entity.attributesMap["repeatSpacing"].ValuePosition;
            var repeatTimes = entity.attributesMap["repeatTimes"].ValuePosition;
            var rotSpeed = entity.attributesMap["rotSpeed"].ValueVar;
            int offsetX = (int)repeatSpacing.X.High;
            int repeatX = (int)repeatTimes.X.High + 1;
            int offsetY = (int)repeatSpacing.Y.High;
            int repeatY = (int)repeatTimes.Y.High + 1;
            switch (direction)
            {
                case 0:
                    break;
                case 1:
                    fliph = true;
                    break;
                case 2:
                    flipv = true;
                    break;
                case 3:
                    flipv = true;
                    fliph = true;
                    break;
            }
            
            var editorAnim = e.LoadAnimation2("Decoration", d, type, -1, fliph, flipv, false);
            if (type == 2)
            {
                editorAnim = e.LoadAnimation2("Decoration", d, type, -1, fliph, flipv, false);
            }
            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                if (e.index >= editorAnim.Frames.Count)
                    e.index = 0;
                var frame = editorAnim.Frames[e.index];
                e.ProcessAnimation(frame.Entry.FrameSpeed, frame.Entry.Frames.Count, frame.Frame.Duration);
                for (int yy = 0; yy < repeatY; yy++)
                {
                    for (int xx = 0; xx < repeatX; xx++)
                    {
                        d.DrawBitmap(frame.Texture, x + frame.Frame.CenterX + offsetX*xx, y + frame.Frame.CenterY + offsetY*yy,
                            frame.Frame.Width, frame.Frame.Height, false, Transparency);
                    }
                }

            }
        }

        public override string GetObjectName()
        {
            return "Decoration";
        }
    }
}
