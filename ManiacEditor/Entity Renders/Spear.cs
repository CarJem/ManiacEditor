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
    public class Spear : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency)
        {
            bool fliph = false; 
            bool flipv = false;
            int orientation = (int)entity.attributesMap["orientation"].ValueUInt8;
            int animID = 0;
            switch (orientation)
            {
                case 1:
                    animID = 1;
                    break;
                case 2:
                    flipv = true;
                    break;
                case 3:
                    fliph = true;
                    animID = 1;
                    break;
            }

            var editorAnim = e.LoadAnimation2("Spear", d, animID, 0, fliph, flipv, false);
            var editorAnimSpear = e.LoadAnimation2("Spear", d, animID, 1, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnimSpear != null && editorAnimSpear.Frames.Count != 0 && animID >= 0)
            {
                var frame = editorAnim.Frames[0];
                var frameSpear = editorAnimSpear.Frames[0];

                d.DrawBitmap(frameSpear.Texture,
                    x + (fliph ? -frameSpear.Frame.CenterX - frameSpear.Frame.Width : frameSpear.Frame.CenterX),
                    y + (flipv ? -frameSpear.Frame.CenterY - frameSpear.Frame.Height : frameSpear.Frame.CenterY),
                    frameSpear.Frame.Width, frameSpear.Frame.Height, false, Transparency);
                d.DrawBitmap(frame.Texture,
                    x + (fliph ? -frame.Frame.CenterX - frame.Frame.Width : frame.Frame.CenterX),
                    y + (flipv ? -frame.Frame.CenterY - frame.Frame.Height : frame.Frame.CenterY),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);

            }
        }

        public override string GetObjectName()
        {
            return "Spear";
        }
    }
}
