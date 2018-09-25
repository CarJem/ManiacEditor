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
    public class SignPost
    {

        public void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency)
        {
            var editorAnim = e.LoadAnimation2("SignPost", d, 0, -1, false, false, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[e.index];
                e.ProcessAnimation(1, frame.Entry.Frames.Count, frame.Frame.Duration);
                d.DrawBitmap(frame.Texture, x + frame.Frame.CenterX, y + frame.Frame.CenterY,
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
            }
            if (Properties.Settings.Default.UsePrePlusOffsets)
            {
                editorAnim = e.LoadAnimation2("SignPost", d, 4, -1, false, false, false);
                if (editorAnim != null && editorAnim.Frames.Count != 0)
                {
                    for (int i = 0; i < editorAnim.Frames.Count; ++i)
                    {
                        if (i == 1)
                            continue;
                        var frame = editorAnim.Frames[i];
                        d.DrawBitmap(frame.Texture, x + frame.Frame.CenterX, y + frame.Frame.CenterY,
                            frame.Frame.Width, frame.Frame.Height, false, Transparency);
                    }
                }
            }
            else
            {
                editorAnim = e.LoadAnimation2("SignPost", d, 6, -1, false, false, false);
                if (editorAnim != null && editorAnim.Frames.Count != 0)
                {
                    for (int i = 0; i < editorAnim.Frames.Count; ++i)
                    {
                        if (i == 1)
                            continue;
                        var frame = editorAnim.Frames[i];
                        d.DrawBitmap(frame.Texture, x + frame.Frame.CenterX, y + frame.Frame.CenterY,
                            frame.Frame.Width, frame.Frame.Height, false, Transparency);
                    }
                }
            }

        }
    }
}
