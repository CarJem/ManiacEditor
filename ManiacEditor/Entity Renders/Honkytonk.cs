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
    public class Honkytonk : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency)
        {
            bool fliph = false;
            bool flipv = false;
            int angle = (int)entity.attributesMap["angle"].ValueVar;
            int rotation = 0;
            int offsetX = 0;
            int offsetY = 0;
            e.rotateImageLegacyMode = true;
            switch (angle)
            {
                case 0:
                    offsetY = 12;
                    break;
                case 32:
                    rotation = 315;
                    offsetX = 67;
                    offsetY = 61;
                    break;
                case 160:
                    rotation = 135;
                    offsetX = 10;
                    offsetY = 72;
                    break;
                case 64:
                    rotation = 270;
                    offsetX = 55;
                    offsetY = 41;
                    break;
                case 224:
                    rotation = 45;
                    offsetX = 44;
                    offsetY = 94;
                    break;
                case 96:
                    rotation = 225;
                    offsetX = 34;
                    offsetY = 39;
                    break;
                case 128:
                    rotation = 180;
                    offsetX = 16;
                    offsetY = 53;
                    break;
                case 192:
                    rotation = 90;
                    offsetX = 25;
                    offsetY = 88;
                    break;
            }
            var editorAnim = e.LoadAnimation2("HonkyTonk", d, 0, 1, fliph, flipv, false, rotation);
            var editorAnim2 = e.LoadAnimation2("HonkyTonk", d, 0, 2, fliph, flipv, false, rotation);
            var editorAnim3 = e.LoadAnimation2("HonkyTonk", d, 0, 3, fliph, flipv, false, rotation);
            var editorAnim4 = e.LoadAnimation2("HonkyTonk", d, 0, 4, fliph, flipv, false, rotation);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnim2 != null && editorAnim2.Frames.Count != 0 && editorAnim3 != null && editorAnim3.Frames.Count != 0 && editorAnim4 != null && editorAnim4.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                var frame2 = editorAnim2.Frames[0];
                var frame3 = editorAnim3.Frames[0];
                var frame4 = editorAnim4.Frames[0];

                //e.ProcessAnimation(framePropel.Entry.FrameSpeed, framePropel.Entry.Frames.Count, framePropel.Frame.Duration, 5);

                d.DrawBitmap(frame.Texture,
                    x + frame.Frame.CenterX - offsetX,
                    y + frame.Frame.CenterY - offsetY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);

            }
        }

        public override string GetObjectName()
        {
            return "Honkytonk";
        }
    }
}
