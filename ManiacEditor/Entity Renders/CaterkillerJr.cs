﻿using System;
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
    public class CaterkillerJr : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency)
        {
            //int type = (int)entity.attributesMap["type"].ValueUInt8;
            //int direction = (int)entity.attributesMap["direction"].ValueUInt8;
            bool fliph = false;
            bool flipv = false;
            var editorAnim = e.LoadAnimation2("CaterkillerJr", d, 0, -1, true, flipv, false);
            var editorAnim2 = e.LoadAnimation2("CaterkillerJr", d, 1, -1, fliph, flipv, false);
            var editorAnim3 = e.LoadAnimation2("CaterkillerJr", d, 2, -1, fliph, flipv, false);
            var editorAnim4 = e.LoadAnimation2("CaterkillerJr", d, 3, -1, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnim2 != null && editorAnim2.Frames.Count != 0 && editorAnim3 != null && editorAnim3.Frames.Count != 0 && editorAnim4 != null && editorAnim4.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                var frame2 = editorAnim2.Frames[0];
                var frame3 = editorAnim3.Frames[0];
                var frame4 = editorAnim4.Frames[0];

                //ProcessAnimation(frame.Entry.FrameSpeed, frame.Entry.Frames.Count, frame.Frame.Duration);

                d.DrawBitmap(frame.Texture,
                    x + frame.Frame.CenterX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                    y + frame.Frame.CenterY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
                for (int i = 0; i <= 6; i++)
                {
                    if (i <= 3 && i >= 1)
                    {
                        d.DrawBitmap(frame2.Texture,
                            x + frame2.Frame.CenterX + (i * frame2.Frame.Width) + (fliph ? (frame2.Frame.Width - editorAnim2.Frames[0].Frame.Width) : 0),
                            y + frame2.Frame.CenterY + (flipv ? (frame2.Frame.Height - editorAnim2.Frames[0].Frame.Height) : 0),
                            frame2.Frame.Width, frame2.Frame.Height, false, Transparency);
                    }
                    if (i == 4)
                    {
                        d.DrawBitmap(frame3.Texture,
                            x + frame3.Frame.CenterX + (i * frame3.Frame.Width) + (frame2.Frame.Width - 5) + (fliph ? (frame3.Frame.Width - editorAnim3.Frames[0].Frame.Width) : 0),
                            y + frame3.Frame.CenterY + (flipv ? (frame3.Frame.Height - editorAnim3.Frames[0].Frame.Height) : 0),
                            frame3.Frame.Width, frame3.Frame.Height, false, Transparency);
                    }
                    if (i >= 5)
                    {
                        d.DrawBitmap(frame4.Texture,
                            x + frame4.Frame.CenterX + (i * frame4.Frame.Width) + frame2.Frame.Width + frame3.Frame.Width + (fliph ? (frame4.Frame.Width - editorAnim4.Frames[0].Frame.Width) : 0),
                            y + frame4.Frame.CenterY + (flipv ? (frame4.Frame.Height - editorAnim4.Frames[0].Frame.Height) : 0),
                            frame4.Frame.Width, frame4.Frame.Height, false, Transparency);
                    }
                }


            }
        }

        public override string GetObjectName()
        {
            return "CaterkillerJr";
        }
    }
}
