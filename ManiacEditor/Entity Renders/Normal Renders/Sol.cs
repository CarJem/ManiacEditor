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
    public class Sol : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            bool fliph = false;
            bool flipv = false;
            int direction = (int)entity.attributesMap["direction"].ValueUInt8;
            bool fireOrbs = entity.attributesMap["fireOrbs"].ValueBool;

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
            }

            var editorAnim = e.EditorInstance.EntityDrawing.LoadAnimation2("Sol", d, 0, 0, fliph, flipv, false);
            var editorAnim2 = e.EditorInstance.EntityDrawing.LoadAnimation2("Sol", d, 1, -1, false, false, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnim2 != null && editorAnim2.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                var frame2 = editorAnim2.Frames[Animation.index];

                Animation.ProcessAnimation(frame2.Entry.SpeedMultiplyer, frame2.Entry.Frames.Count, frame2.Frame.Delay);

                if (!fireOrbs)
                {
                    d.DrawBitmap(frame2.Texture,
                        x + frame2.Frame.PivotX + 16,
                        y + frame2.Frame.PivotY,
                        frame2.Frame.Width, frame2.Frame.Height, false, Transparency);
                    d.DrawBitmap(frame2.Texture,
                        x - frame2.Frame.PivotX - 30,
                        y + frame2.Frame.PivotY,
                        frame2.Frame.Width, frame2.Frame.Height, false, Transparency);
                }
                else
                {
                    d.DrawBitmap(frame2.Texture,
                        x + frame2.Frame.PivotX + 16,
                        y + frame2.Frame.PivotY,
                        frame2.Frame.Width, frame2.Frame.Height, false, Transparency-100);
                    d.DrawBitmap(frame2.Texture,
                        x - frame2.Frame.PivotX - 30,
                        y + frame2.Frame.PivotY,
                        frame2.Frame.Width, frame2.Frame.Height, false, Transparency-100);
                }


                d.DrawBitmap(frame.Texture,
                    x + frame.Frame.PivotX,
                    y + frame.Frame.PivotY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);

            }
        }

        public override string GetObjectName()
        {
            return "Sol";
        }
    }
}
