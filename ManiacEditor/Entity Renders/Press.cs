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
    public class Press : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            int size = (int)entity.attributesMap["size"].ValueUInt16;
            int offTop = (int)entity.attributesMap["offTop"].ValueVar;
            int offBottom = (int)entity.attributesMap["offBottom"].ValueVar;
            bool fliph = false;
            bool flipv = false;
            var editorAnim = EditorEntity_ini.LoadAnimation2("Press", d, 0, -1, fliph, flipv, false);
            var editorAnim2 = EditorEntity_ini.LoadAnimation2("Press", d, 2, -1, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnim2 != null && editorAnim2.Frames.Count != 0)
            {
                var crankTop = editorAnim2.Frames[0];
                var crankHandle = editorAnim.Frames[2];
                var crankHolder = editorAnim.Frames[0];
                var frame = editorAnim.Frames[6];
                var platform = editorAnim.Frames[3];
                var platformEndCap = editorAnim.Frames[5];
                var platformEndCap2 = editorAnim.Frames[4];
                bool hEven = (size % 2 == 0);

                for (int y2 = 0; y2 <= size; ++y2)
                {
                    d.DrawBitmap(frame.Texture,
                        x + -frame.Frame.Width + (-1 / 2 + 1) * frame.Frame.Width + frame.Frame.CenterX,
                        y + -frame.Frame.Height + (-size / 2 + y2) * frame.Frame.Height,
                        platform.Frame.Width, platform.Frame.Height, false, Transparency);
                    if (y2 == size)
                    {
                        y2 = y2 + 2;
                        d.DrawBitmap(crankTop.Texture,
                            x + crankTop.Frame.CenterX,
                            y + -crankTop.Frame.Height + (-size / 2 + y2) * frame.Frame.Height,
                            crankTop.Frame.Width, crankTop.Frame.Height, false, Transparency);
                    }
                }
                int yy = 0;

                    d.DrawBitmap(platformEndCap.Texture,
                        x + platformEndCap.Frame.CenterX,
                        y + -platformEndCap.Frame.Height + (-size / 2 + yy) * frame.Frame.Height + offTop - platformEndCap.Frame.CenterY - (hEven ? 0 : 4),
                        platformEndCap.Frame.Width, platformEndCap.Frame.Height, false, Transparency);
                    d.DrawBitmap(platform.Texture,
                        x + platform.Frame.CenterX,
                        y + -platform.Frame.Height + (-size / 2 + yy) * frame.Frame.Height + offTop - platform.Frame.CenterY - (hEven ? 0 : 4),
                        platform.Frame.Width, platform.Frame.Height, false, Transparency);

                    d.DrawBitmap(platformEndCap2.Texture,
                        x + platformEndCap2.Frame.CenterX,
                        y + -platformEndCap2.Frame.Height + (-size / 2 + yy) * frame.Frame.Height + offBottom - platformEndCap2.Frame.CenterY - (hEven ? 0 : 4),
                        platformEndCap2.Frame.Width, platformEndCap2.Frame.Height, false, Transparency);
                    d.DrawBitmap(platform.Texture,
                        x + platform.Frame.CenterX,
                        y + -platform.Frame.Height + (-size / 2 + yy) * frame.Frame.Height + offBottom - platform.Frame.CenterY - (hEven ? 0 : 4),
                        platform.Frame.Width, platform.Frame.Height, false, Transparency);

                    d.DrawBitmap(crankHolder.Texture,
                        x + crankHolder.Frame.CenterX + 74,
                        y + -crankHolder.Frame.Height + (-size / 2 + yy) * frame.Frame.Height + crankHolder.Frame.CenterY + 16,
                        crankHolder.Frame.Width, crankHolder.Frame.Height, false, Transparency);
                    d.DrawBitmap(crankHandle.Texture,
                        x + crankHandle.Frame.CenterX + 56,
                        y + -crankHandle.Frame.Height + (-size / 2 + yy) * frame.Frame.Height + crankHandle.Frame.CenterY,
                        crankHandle.Frame.Width, crankHandle.Frame.Height, false, Transparency);
                    d.DrawBitmap(crankTop.Texture,
                        x + crankTop.Frame.CenterX,
                        y + -crankTop.Frame.Height + (-size / 2 + yy) * frame.Frame.Height + crankTop.Frame.CenterY,
                        crankTop.Frame.Width, crankTop.Frame.Height, false, Transparency);
            }
        }

        public override string GetObjectName()
        {
            return "Press";
        }
    }
}
