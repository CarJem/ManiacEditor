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
    public class GenericTrigger
    {

        public void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency)
        {
            var widthPixels = (int)(entity.attributesMap["size"].ValuePosition.X.High) * 2;
            var heightPixels = (int)(entity.attributesMap["size"].ValuePosition.Y.High) * 2;
            var width = (int)widthPixels / 16;
            var height = (int)heightPixels / 16;

            //Draw Icon
            var editorAnim = e.LoadAnimation2("EditorIcons2", d, 0, 5, false, false, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                var frame2 = editorAnim.Frames[e.index];

                d.DrawBitmap(frame2.Texture,
                    x + frame2.Frame.CenterX,
                    y + frame2.Frame.CenterY,
                    frame2.Frame.Width, frame2.Frame.Height, false, Transparency);

                editorAnim = e.LoadAnimation2("EditorAssets", d, 0, 0, false, false, false);

                if (width != 0 && height != 0)
                {
                    // draw corners
                    for (int i = 0; i < 4; i++)
                    {
                        bool right = (i & 1) > 0;
                        bool bottom = (i & 2) > 0;

                        editorAnim = e.LoadAnimation2("EditorAssets", d, 0, 0, right, bottom, false);
                        if (editorAnim != null && editorAnim.Frames.Count != 0)
                        {
                            var frame = editorAnim.Frames[e.index];
                            e.ProcessAnimation(frame.Entry.FrameSpeed, frame.Entry.Frames.Count, frame.Frame.Duration);
                            d.DrawBitmap(frame.Texture,
                                (x + widthPixels / (right ? 2 : -2)) - (right ? frame.Frame.Width : 0),
                                (y + heightPixels / (bottom ? 2 : -2) - (bottom ? frame.Frame.Height : 0)),
                                frame.Frame.Width, frame.Frame.Height, false, Transparency);

                        }
                    }

                    // draw top and bottom
                    for (int i = 0; i < 2; i++)
                    {
                        bool bottom = (i & 1) > 0;

                        editorAnim = e.LoadAnimation2("EditorAssets", d, 0, 1, false, bottom, false);
                        if (editorAnim != null && editorAnim.Frames.Count != 0)
                        {
                            var frame = editorAnim.Frames[e.index];
                            e.ProcessAnimation(frame.Entry.FrameSpeed, frame.Entry.Frames.Count, frame.Frame.Duration);
                            bool wEven = width % 2 == 0;
                            for (int j = 1; j < width; j++)
                                d.DrawBitmap(frame.Texture,
                                    (x + (wEven ? frame.Frame.CenterX : -frame.Frame.Width) + (-width / 2 + j) * frame.Frame.Width),
                                    (y + heightPixels / (bottom ? 2 : -2) - (bottom ? frame.Frame.Height : 0)),
                                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
                        }
                    }

                    // draw sides
                    for (int i = 0; i < 2; i++)
                    {
                        bool right = (i & 1) > 0;

                        editorAnim = e.LoadAnimation2("EditorAssets", d, 0, 2, right, false, false);
                        if (editorAnim != null && editorAnim.Frames.Count != 0)
                        {
                            var frame = editorAnim.Frames[e.index];
                            e.ProcessAnimation(frame.Entry.FrameSpeed, frame.Entry.Frames.Count, frame.Frame.Duration);
                            bool hEven = height % 2 == 0;
                            for (int j = 1; j < height; j++)
                                d.DrawBitmap(frame.Texture,
                                    (x + widthPixels / (right ? 2 : -2)) - (right ? frame.Frame.Width : 0),
                                    (y + (hEven ? frame.Frame.CenterY : -frame.Frame.Height) + (-height / 2 + j) * frame.Frame.Height),
                                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
                        }
                    }
                }
            }


        }
    }
}
