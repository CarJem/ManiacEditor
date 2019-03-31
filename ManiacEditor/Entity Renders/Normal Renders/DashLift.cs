using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ManiacEditor;
using Microsoft.Xna.Framework;
using RSDKv5;
using SystemColors = System.Drawing.Color;

namespace ManiacEditor.Entity_Renders
{
    public class DashLift : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            bool fliph = false;
            bool flipv = false;
            int startOff = (int)entity.attributesMap["startOff"].ValueVar;
            int length = (int)entity.attributesMap["length"].ValueVar;

            var widthPixels = (int)(64);
            var heightPixels = (int)(length);
            var width = (int)widthPixels / 16;
            var height = (int)heightPixels / 16;

            var editorAnim2 = Editor.Instance.EntityDrawing.LoadAnimation2("EditorAssets", d, 0, 1, false, false, false);

            if (width != 0 && height != 0)
            {
                int x1 = x + widthPixels / -2;
                int x2 = x + widthPixels / 2 - 1;
                int y1 = y + heightPixels / -2;
                int y2 = y + heightPixels / 2 - 1;


                d.DrawLine(x1, y1, x1, y2, SystemColors.White);
                d.DrawLine(x1, y1, x2, y1, SystemColors.White);
                d.DrawLine(x2, y2, x1, y2, SystemColors.White);
                d.DrawLine(x2, y2, x2, y1, SystemColors.White);

                // draw corners
                for (int i = 0; i < 4; i++)
                {
                    bool right = (i & 1) > 0;
                    bool bottom = (i & 2) > 0;

                    editorAnim2 = Editor.Instance.EntityDrawing.LoadAnimation2("EditorAssets", d, 0, 1, right, bottom, false);
                    if (editorAnim2 != null && editorAnim2.Frames.Count != 0)
                    {
                        var frame2 = editorAnim2.Frames[0];
                        d.DrawBitmap(frame2.Texture,
                            (x + widthPixels / (right ? 2 : -2)) - (right ? frame2.Frame.Width : 0),
                            (y + heightPixels / (bottom ? 2 : -2) - (bottom ? frame2.Frame.Height : 0)),
                            frame2.Frame.Width, frame2.Frame.Height, false, Transparency);

                    }
                }
            }

            var editorAnim = Editor.Instance.EntityDrawing.LoadAnimation2("Platform", d, 2, -1, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];
                Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);

                d.DrawBitmap(frame.Texture,
                    x + frame.Frame.PivotX,
                    y + frame.Frame.PivotY + (startOff > length ? length / 2 : startOff / 2),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }

        }

        public override string GetObjectName()
        {
            return "DashLift";
        }
    }
}
