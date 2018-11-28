using System;
using System.Collections.Generic;
using System.Drawing;
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
    public class Outro_Intro_Object : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            bool fliph = false;
            bool flipv = false;
            var editorAnim = EditorEntity_ini.LoadAnimation2("EditorIcons2", d, 0, 3, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[Animation.index];

                d.DrawBitmap(frame.Texture,
                    x + frame.Frame.CenterX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                    y + frame.Frame.CenterY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }

            if (entity.Object.Name.Name == "LRZ1Intro")
            {
                var editorAnim3 = EditorEntity_ini.LoadAnimation2("IntroSub", d, 0, 0, fliph, flipv, false);
                if (editorAnim3 != null && editorAnim3.Frames.Count != 0)
                {
                    var frame = editorAnim3.Frames[Animation.index];

                    d.DrawBitmap(frame.Texture,
                        x + frame.Frame.CenterX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                        y + frame.Frame.CenterY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
                }
            }


            int widthPixels = attribMap.AttributesMapPositionHighX("size", entity) * 2;
            var heightPixels = attribMap.AttributesMapPositionHighY("size", entity) * 2;
            var width = (int)widthPixels / 16;
            var height = (int)heightPixels / 16;

            var editorAnim2 = EditorEntity_ini.LoadAnimation2("EditorAssets", d, 0, 1, false, false, false);

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

                    editorAnim2 = EditorEntity_ini.LoadAnimation2("EditorAssets", d, 0, 1, right, bottom, false);
                    if (editorAnim2 != null && editorAnim2.Frames.Count != 0)
                    {
                        var frame = editorAnim2.Frames[Animation.index];
                        Animation.ProcessAnimation(frame.Entry.FrameSpeed, frame.Entry.Frames.Count, frame.Frame.Duration);
                        d.DrawBitmap(frame.Texture,
                            (x + widthPixels / (right ? 2 : -2)) - (right ? frame.Frame.Width : 0),
                            (y + heightPixels / (bottom ? 2 : -2) - (bottom ? frame.Frame.Height : 0)),
                            frame.Frame.Width, frame.Frame.Height, false, Transparency);

                    }
                }
            }
        }

        public override string GetObjectName()
        {
            return "Outro_Intro_Object";
        }
    }
}
