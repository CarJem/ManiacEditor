﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using ManiacEditor;
using Microsoft.Xna.Framework;
using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class PimPom : EntityRenderer
    {
        Boolean boolState = true;
        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency)
        {
            int type = (int)entity.attributesMap["type"].ValueVar;
            int color = (int)entity.attributesMap["color"].ValueUInt8;
            int direction = (int)entity.attributesMap["direction"].ValueUInt8;
            int angle = (int)entity.attributesMap["angle"].ValueInt32;
            int length = (int)entity.attributesMap["length"].ValueUInt8;
            int gap = (int)entity.attributesMap["gap"].ValueUInt8;
            bool fliph = false;
            bool flipv = false;
            int animID;
            int frameID;
            switch (type)
            {
                case 0:
                    animID = 0;
                    switch (color)
                    {
                        case 0:
                            frameID = 0;
                            break;
                        case 1:
                            frameID = 1;
                            break;
                        case 2:
                            frameID = 2;
                            break;
                        default:
                            frameID = 0;
                            break;
                    }
                    break;
                case 1:
                    animID = 1;
                    frameID = 0;
                    break;
                case 2:
                    animID = 2;
                    frameID = 0;
                    break;
                case 3:
                    animID = 3;
                    frameID = 0;
                    break;
                default:
                    animID = 0;
                    frameID = 0;
                    break;

            }
            if (direction == 1)
            {
                fliph = true;
            }
            switch (angle)
            {
                case 0:
                    break;
                case 1:
                    animID = 2;
                    fliph = true;
                    break;
                case 2:
                    animID = 3;
                    flipv = true;
                    break;
                case 3:
                    animID = 2;
                    flipv = true;
                    fliph = true;
                    break;
                default:
                    break;
            }
            var editorAnim = e.LoadAnimation2("PimPom", d, animID, frameID, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0 && animID >= 0)
            {
                var frame = editorAnim.Frames[e.index];

                //e.ProcessAnimation(frame.Entry.FrameSpeed, frame.Entry.Frames.Count, frame.Frame.Duration);
                if (length != 0 && angle == 0)
                {
                    var value = entity.attributesMap["length"].ValueUInt8;
                    if (editorAnim != null && editorAnim.Frames.Count != 0)
                    {
                        bool wEven = value % 2 == 0;
                        for (int xx = 0; xx <= value; ++xx)
                        {
                            d.DrawBitmap(frame.Texture,
                                (x - 3) + (wEven ? frame.Frame.CenterX : -frame.Frame.Width) + (-value / 2 + xx) * (frame.Frame.Width + (gap*2)),
                                y + frame.Frame.CenterY,
                                frame.Frame.Width, frame.Frame.Height, false, Transparency);
                        }
                    }
                }
                else if (length != 0 && angle == 3)
                {
                    var value = entity.attributesMap["length"].ValueUInt8;
                    if (editorAnim != null && editorAnim.Frames.Count != 0)
                    {
                        bool wEven = value % 2 == 0;
                        for (int xx = 0; xx <= value; ++xx)
                        {
                            d.DrawBitmap(frame.Texture,
                                x + (wEven ? frame.Frame.CenterX : -frame.Frame.Width) + (-value / 2 + xx) * (frame.Frame.Width),
                                y + (wEven ? frame.Frame.CenterY : -frame.Frame.Height) + (-value / 2 + xx) * (frame.Frame.Height),
                                frame.Frame.Width, frame.Frame.Height, false, Transparency);
                        }
                    }
                }
                else if (length != 0 && angle == 1)
                {
                    var value = entity.attributesMap["length"].ValueUInt8;
                    if (editorAnim != null && editorAnim.Frames.Count != 0)
                    {
                        bool wEven = value % 2 == 0;
                        for (int xx = 0; xx <= value; ++xx)
                        {
                            d.DrawBitmap(frame.Texture,
                                (x - frame.Frame.Width) - (wEven ? frame.Frame.CenterX : -frame.Frame.Width) - (-value / 2 + xx) * (frame.Frame.Width),
                                y + (wEven ? frame.Frame.CenterY : -frame.Frame.Height) + (-value / 2 + xx) * (frame.Frame.Height - 2),
                                frame.Frame.Width, frame.Frame.Height, false, Transparency);
                        }
                    }
                }
                else if (length != 0 && angle == 2)
                {
                    var value = entity.attributesMap["length"].ValueUInt8;
                    if (editorAnim != null && editorAnim.Frames.Count != 0)
                    {
                        bool wEven = value % 2 == 0;
                        wEven = boolState;
                        for (int xx = 0; xx <= value; ++xx)
                        {
                            d.DrawBitmap(frame.Texture,
                                (x + 3) + frame.Frame.CenterX,
                                y + (wEven ? frame.Frame.CenterY : -frame.Frame.Height) + (-value / 2 + xx) * (frame.Frame.Height + (gap * 2)),
                                frame.Frame.Width, frame.Frame.Height, false, Transparency);
                        }
                    }
                }
                else {
                    d.DrawBitmap(frame.Texture,
                        x + frame.Frame.CenterX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                        y + frame.Frame.CenterY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
                }


            }
        }

        public override string GetObjectName()
        {
            return "PimPom";
        }
    }
}
