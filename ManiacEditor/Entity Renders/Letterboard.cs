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
    public class Letterboard : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency)
        {
            int letterID = (int)entity.attributesMap["letterID"].ValueUInt8;
            bool controller = entity.attributesMap["controller"].ValueBool;
            bool fliph = false;
            bool flipv = false;
            var editorAnim = e.LoadAnimation2("Letterboard", d, 0, -1, fliph, flipv, false);
            var editorAnim2 = e.LoadAnimation2("Letterboard", d, 1, letterID - 1, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnim2 != null && editorAnim2.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                var frame2 = editorAnim2.Frames[e.index];

                //ProcessAnimation(frame.Entry.FrameSpeed, frame.Entry.Frames.Count, frame.Frame.Duration);

                if (letterID == 0 || controller == true)
                {
                    d.DrawBitmap(frame.Texture,
                        x + frame.Frame.CenterX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                        y + frame.Frame.CenterY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
                }
                else
                {
                    d.DrawBitmap(frame2.Texture,
                        x + frame.Frame.CenterX - (fliph ? (frame.Frame.Width - editorAnim.Frames[letterID].Frame.Width) : 0),
                        y + frame.Frame.CenterY + (flipv ? (frame.Frame.Height - editorAnim.Frames[letterID].Frame.Height) : 0),
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
                }

            }
        }

        public override string GetObjectName()
        {
            return "Letterboard";
        }
    }
}
