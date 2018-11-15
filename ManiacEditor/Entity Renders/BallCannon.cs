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
    public class BallCannon : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency)
        {
            int x2 = x;
            int y2 = y;
            bool flipv = false;
            bool fliph = false;
            int angle = (int)entity.attributesMap["angle"].ValueVar;
            int rotation = 0;
            switch (angle)
            {
                case 0:
                    rotation = 90;
                    break;
                case 1:
                    rotation = 180;
                    break;
                case 2:
                    rotation = 270;
                    break;
                case 3:
                    break;
                case 4:
                    rotation = 180;
                    break;
                case 5:
                    rotation = 270; 
                    break;
                case 6:
                    break;
                case 7:
                    rotation = 90;
                    break;
            }
            var editorAnim = e.LoadAnimation2("BallCannon", d, 0, -1, fliph, flipv, false, rotation);
            var editorAnim2 = e.LoadAnimation2("BallCannon", d, 1, -1, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnim2 != null && editorAnim2.Frames.Count != 0)
            {
            
                var frame = editorAnim.Frames[e.index];
                var frame2 = editorAnim2.Frames[e.EditorAnimations.index];

                e.ProcessAnimation(frame.Entry.FrameSpeed, frame.Entry.Frames.Count, frame.Frame.Duration);
                e.EditorAnimations.ProcessAnimation2(frame2.Entry.FrameSpeed, frame2.Entry.Frames.Count, frame2.Frame.Duration);
                /*
                d.DrawBitmap(frame2.Texture,
                    x2 + frame2.Frame.CenterX,
                    y2 + frame2.Frame.CenterY,
                    frame2.Frame.Width, frame2.Frame.Height, false, Transparency);
                    */

                d.DrawBitmap(frame.Texture,
                    x + frame.Frame.CenterX,
                    y + frame.Frame.CenterY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);


            }
        }

        public override string GetObjectName()
        {
            return "BallCannon";
        }
    }
}
