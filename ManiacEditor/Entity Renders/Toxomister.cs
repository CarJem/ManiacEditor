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
    public class Toxomister : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            int direction = (int)entity.attributesMap["direction"].ValueUInt8;
            bool fliph = false;
            bool flipv = false;

            switch (direction)
            {
                case 0:
                    break;
                case 1:
                    flipv = true;
                    break;
                    /*
                case 2:
                    flipv = true;
                    break;
                case 3:
                    flipv = true;
                    fliph = true;
                    break;
                    */
            }

            var editorAnim = EditorEntity_ini.LoadAnimation2("Toxomister", d, 0, 0, fliph, flipv, false);
            var editorAnim2 = EditorEntity_ini.LoadAnimation2("Toxomister", d, 1, 0, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && editorAnim2 != null && editorAnim2.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                var frame2 = editorAnim2.Frames[0];

                Animation.ProcessAnimation(frame.Entry.FrameSpeed, frame.Entry.Frames.Count, frame.Frame.Duration);

                d.DrawBitmap(frame2.Texture,
                    x + (fliph ? -frame2.Frame.CenterX - frame2.Frame.Width : frame2.Frame.CenterX),
                    y + (flipv ? -frame2.Frame.CenterY - frame2.Frame.Height : frame2.Frame.CenterY),
                    frame2.Frame.Width, frame2.Frame.Height, false, Transparency);
                d.DrawBitmap(frame.Texture,
                    x + (fliph ? -frame.Frame.CenterX - frame.Frame.Width : frame.Frame.CenterX),
                    y + (flipv ? -frame.Frame.CenterY - frame.Frame.Height : frame.Frame.CenterY),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);

            }
        }

        public override string GetObjectName()
        {
            return "Toxomister";
        }
    }
}