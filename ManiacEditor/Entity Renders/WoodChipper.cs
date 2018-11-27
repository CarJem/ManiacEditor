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
    public class WoodChipper : EntityRenderer
    {

        public override void Draw(DevicePanel d, SceneEntity entity, EditorEntity e, int x, int y, int Transparency, int index = 0, int previousChildCount = 0, int platformAngle = 0, EditorAnimations Animation = null, bool selected = false, AttributeValidater attribMap = null)
        {
            bool fliph = false;
            bool flipv = false;
            int size = entity.attributesMap["size"].ValueUInt16; 
            int direction = entity.attributesMap["direction"].ValueUInt8;
            switch (direction)
            {
                case 1:
                    fliph = true;
                    break;
            }
            var editorAnim = EditorEntity_ini.LoadAnimation2("WoodChipper", d, 0, -1, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                var frame2 = editorAnim.Frames[1];
                var frame3 = editorAnim.Frames[2];
                var frame4 = editorAnim.Frames[6];

                var log1 = editorAnim.Frames[3];
                var log2 = editorAnim.Frames[4];
                var log3 = editorAnim.Frames[5];

                //Animation.ProcessAnimation(frame.Entry.FrameSpeed, frame.Entry.Frames.Count, frame.Frame.Duration);

                if (size > 0)
                {
                    int repeat = 1;
                    int sizeMemory = size;
                    bool finalLoop = false;
                        d.DrawBitmap(log1.Texture,
                            x + log1.Frame.CenterX - (fliph ? (log1.Frame.Width - editorAnim.Frames[3].Frame.Width) : 0),
                            y + log1.Frame.CenterY - size + (flipv ? (log1.Frame.Height - editorAnim.Frames[3].Frame.Height) : 0),
                            log1.Frame.Width, log1.Frame.Height, false, Transparency);
                        d.DrawBitmap(log2.Texture,
                            x + log2.Frame.CenterX - (fliph ? (log2.Frame.Width - editorAnim.Frames[4].Frame.Width) : 0),
                            y + log2.Frame.CenterY - size + (flipv ? (log2.Frame.Height - editorAnim.Frames[4].Frame.Height) : 0),
                            log2.Frame.Width, log2.Frame.Height, false, Transparency);
                    if (size > 95)
                    {
                        repeat++;
                        sizeMemory = sizeMemory - 96;

                        while (sizeMemory > 80)
                        {
                            repeat++;
                            sizeMemory = sizeMemory - 80;
                        }
                        for (int i = 1; i < repeat + 1; i++)
                        {
                            if (i == repeat)
                            {
                                finalLoop = true;
                            }
                            d.DrawBitmap(log2.Texture,
                                    x + log2.Frame.CenterX - (fliph ? (log2.Frame.Width - editorAnim.Frames[4].Frame.Width) : 0),
                                    y + log2.Frame.CenterY - size + log2.Frame.Height * i + (flipv ? (log2.Frame.Height - editorAnim.Frames[4].Frame.Height) : 0),
                                    log2.Frame.Width, (finalLoop ? sizeMemory : log2.Frame.Height), false, Transparency);
                        }
                    }


                    }
                
                d.DrawBitmap(frame.Texture,
                    x + frame.Frame.CenterX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                    y + frame.Frame.CenterY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
                d.DrawBitmap(frame2.Texture,
                    x + frame2.Frame.CenterX - (fliph ? (frame2.Frame.Width - 92) : 0),
                    y + frame2.Frame.CenterY + (flipv ? (frame2.Frame.Height - editorAnim.Frames[1].Frame.Height) : 0),
                    frame2.Frame.Width, frame2.Frame.Height, false, Transparency);
                d.DrawBitmap(frame3.Texture,
                    x + frame3.Frame.CenterX - (fliph ? (frame2.Frame.Width + 41) : 0),
                    y + frame3.Frame.CenterY + (flipv ? (frame3.Frame.Height - editorAnim.Frames[2].Frame.Height) : 0),
                    frame3.Frame.Width, frame3.Frame.Height, false, Transparency);
                d.DrawBitmap(frame4.Texture,
                    x + frame4.Frame.CenterX - (fliph ? (frame2.Frame.Width + 31) : 0),
                    y + frame4.Frame.CenterY + (flipv ? (frame4.Frame.Height - editorAnim.Frames[6].Frame.Height) : 0),
                    frame4.Frame.Width, frame4.Frame.Height, false, Transparency);
                    
            }
        }

        public override string GetObjectName()
        {
            return "WoodChipper";
        }
    }
}
