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
    public class Spikes
    {

        public void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, bool isFBZ)
        {
            var value = entity.attributesMap["type"];
            bool fliph = false;
            bool flipv = false;
            int animID = 0;

            // Down
            if (value.ValueVar == 1)
            {
                flipv = true;
                animID = 0;
            }
            // Right
            if (value.ValueVar == 2)
            {
                animID = 1;
            }
            // Left
            if (value.ValueVar == 3)
            {
                fliph = true;
                animID = 1;
            }

            int count = (entity.attributesMap.ContainsKey("count") ? (int)entity.attributesMap["count"].ValueUInt8 : 2);

            // Is it a value that defaults to 2?
            if (count < 2)
                count = 2;

            int offset1 = 0, offset2 = 0;
            bool extra = false;
            count *= 2; // I made all this with an incorrect assumption so here's a cheap fix
            int count2 = count >> 2;
            var editorAnim = e.LoadAnimation2("Spikes", d, animID, 0, fliph, flipv, false);
            if (isFBZ)
            {
                editorAnim = e.LoadAnimation2("Spikes", d, animID, -1, fliph, flipv, false);
            }

            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                if (isFBZ)
                {
                    frame = editorAnim.Frames[e.index];
                    e.ProcessAnimation(frame.Entry.FrameSpeed, frame.Entry.Frames.Count, frame.Frame.Duration);
                }


                if (value.ValueVar == 0 || value.ValueVar == 1)
                {
                    // Is count indivisible by 4?
                    if (count % 4 != 0)
                    {
                        offset1 = frame.Frame.Width / 4;
                        count -= 2;
                        extra = true;
                    }

                    // Is count divisible by 8?
                    if (count % 8 == 0)
                    {
                        offset2 = frame.Frame.Width / 2;
                    }

                    // Draw each set of spikes
                    int max = (count2 + 1) / 2;
                    for (int i = -count2 / 2; i < max; ++i)
                    {
                        d.DrawBitmap(frame.Texture, x + frame.Frame.CenterX + (i * (frame.Frame.Width)) - offset1 + offset2, y + frame.Frame.CenterY,
                            frame.Frame.Width, frame.Frame.Height, false, Transparency);
                    }

                    // Draw one more overlapping if needed
                    if (extra)
                    {
                        d.DrawBitmap(frame.Texture, x + frame.Frame.CenterX + ((max - 1) * (frame.Frame.Width)) + offset1 + offset2, y + frame.Frame.CenterY,
                            frame.Frame.Width, frame.Frame.Height, false, Transparency);
                    }
                }
                else if (value.ValueVar == 2 || value.ValueVar == 3)
                {
                    // Is count indivisible by 4?
                    if (count % 4 != 0)
                    {
                        offset1 = frame.Frame.Height / 4;
                        count -= 2;
                        extra = true;
                    }

                    // Is count divisible by 8?
                    if (count % 8 == 0)
                    {
                        offset2 = frame.Frame.Height / 2;
                    }

                    // Draw each set of spikes
                    int max = (count2 + 1) / 2;
                    for (int i = -count2 / 2; i < max; ++i)
                    {
                        d.DrawBitmap(frame.Texture, x + frame.Frame.CenterX, y + frame.Frame.CenterY + (i * (frame.Frame.Height)) - offset1 + offset2,
                            frame.Frame.Width, frame.Frame.Height, false, Transparency);
                    }

                    // Draw one more overlapping if needed
                    if (extra)
                    {
                        d.DrawBitmap(frame.Texture, x + frame.Frame.CenterX, y + frame.Frame.CenterY + ((max - 1) * (frame.Frame.Height)) + offset1 + offset2,
                            frame.Frame.Width, frame.Frame.Height, false, Transparency);
                    }
                }

            }
        }
    }
}
