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
    public class MonkeyDude : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency)
        {
            bool fliph = false;
            bool flipv = false;
            var editorAnim = e.LoadAnimation2("MonkeyDude", d, 0, -1, fliph, flipv, false);
            var editorAnimTail = e.LoadAnimation2("MonkeyDude", d, 1, -1, fliph, flipv, false);
            var editorAnimArm = e.LoadAnimation2("MonkeyDude", d, 2, -1, fliph, flipv, false);
            var editorAnimHand = e.LoadAnimation2("MonkeyDude", d, 3, -1, fliph, flipv, false);
            var editorAnimCoconut = e.LoadAnimation2("MonkeyDude", d, 4, -1, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnimTail != null && editorAnimTail.Frames.Count != 0 && editorAnimArm != null && editorAnimArm.Frames.Count != 0 && editorAnimCoconut != null && editorAnimCoconut.Frames.Count != 0 && editorAnimHand != null && editorAnimHand.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[e.index];
                var frameTail = editorAnimTail.Frames[e.index];
                var frameArm = editorAnimArm.Frames[e.index];
                var frameHand = editorAnimHand.Frames[e.index];
                var frameCoconut = editorAnimCoconut.Frames[e.index];
                int i;
                for (i = 0; i < 4; i++)
                {
                    d.DrawBitmap(frameArm.Texture,
    (x - 15) + frameArm.Frame.CenterX * i - 3,
    (y - 5) + frameArm.Frame.CenterY * i - 3,
    frameArm.Frame.Width, frameArm.Frame.Height, false, Transparency);
                }
                i++;
                d.DrawBitmap(frameHand.Texture,
                    (x - 15) + frameHand.Frame.CenterX * i,
                    (y - 5) + frameHand.Frame.CenterY * i,
                    frameHand.Frame.Width, frameHand.Frame.Height, false, Transparency);
                i++;
                d.DrawBitmap(frameCoconut.Texture,
                    (x - 15) + frameCoconut.Frame.CenterX * i,
                    (y - 5) + frameCoconut.Frame.CenterY * i,
                    frameCoconut.Frame.Width, frameCoconut.Frame.Height, false, Transparency);
                d.DrawBitmap(frame.Texture,
                    x + frame.Frame.CenterX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                    y + frame.Frame.CenterY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
                d.DrawBitmap(frameTail.Texture,
                    x + frameTail.Frame.CenterX - (fliph ? (frameTail.Frame.Width - editorAnimTail.Frames[0].Frame.Width) : 0),
                    y + frameTail.Frame.CenterY + (flipv ? (frameTail.Frame.Height - editorAnimTail.Frames[0].Frame.Height) : 0),
                    frameTail.Frame.Width, frameTail.Frame.Height, false, Transparency);

            }
        }

        public override string GetObjectName()
        {
            return "MonkeyDude";
        }
    }
}
