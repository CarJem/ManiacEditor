using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class WoodChipper : EntityRenderer
    {

        public override void Draw(Structures.EntityLoadOptions properties)
        {
            Classes.Core.Draw.GraphicsHandler d = properties.Graphics;
            SceneEntity entity = properties.Object; 
            Classes.Core.Scene.Sets.EditorEntity e = properties.EditorObject;
            int x = properties.X;
            int y = properties.Y;
            int Transparency = properties.Transparency;
            int index = properties.Index;
            int previousChildCount = properties.PreviousChildCount;
            int platformAngle = properties.PlatformAngle;
            EditorAnimations Animation = properties.Animations;
            bool selected  = properties.isSelected;
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
            var editorAnim = Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2("WoodChipper", d.DevicePanel, 0, -1, fliph, flipv, false);
            if (editorAnim != null && editorAnim.Frames.Count != 0)
            {
                var frame = editorAnim.Frames[0];
                var frame2 = editorAnim.Frames[1];
                var frame3 = editorAnim.Frames[2];
                var frame4 = editorAnim.Frames[6];

                var log1 = editorAnim.Frames[3];
                var log2 = editorAnim.Frames[4];
                var log3 = editorAnim.Frames[5];

                //Animation.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay);

                if (size > 0)
                {
                    int repeat = 1;
                    int sizeMemory = size;
                    bool finalLoop = false;
                        d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(log1),
                            x + log1.Frame.PivotX - (fliph ? (log1.Frame.Width - editorAnim.Frames[3].Frame.Width) : 0),
                            y + log1.Frame.PivotY - size + (flipv ? (log1.Frame.Height - editorAnim.Frames[3].Frame.Height) : 0),
                            log1.Frame.Width, log1.Frame.Height, false, Transparency);
                        d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(log2),
                            x + log2.Frame.PivotX - (fliph ? (log2.Frame.Width - editorAnim.Frames[4].Frame.Width) : 0),
                            y + log2.Frame.PivotY - size + (flipv ? (log2.Frame.Height - editorAnim.Frames[4].Frame.Height) : 0),
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
                            d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(log2),
                                    x + log2.Frame.PivotX - (fliph ? (log2.Frame.Width - editorAnim.Frames[4].Frame.Width) : 0),
                                    y + log2.Frame.PivotY - size + log2.Frame.Height * i + (flipv ? (log2.Frame.Height - editorAnim.Frames[4].Frame.Height) : 0),
                                    log2.Frame.Width, (finalLoop ? sizeMemory : log2.Frame.Height), false, Transparency);
                        }
                    }


                    }
                
                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame),
                    x + frame.Frame.PivotX - (fliph ? (frame.Frame.Width - editorAnim.Frames[0].Frame.Width) : 0),
                    y + frame.Frame.PivotY + (flipv ? (frame.Frame.Height - editorAnim.Frames[0].Frame.Height) : 0),
                    frame.Frame.Width, frame.Frame.Height, false, Transparency);
                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame2),
                    x + frame2.Frame.PivotX - (fliph ? (frame2.Frame.Width - 92) : 0),
                    y + frame2.Frame.PivotY + (flipv ? (frame2.Frame.Height - editorAnim.Frames[1].Frame.Height) : 0),
                    frame2.Frame.Width, frame2.Frame.Height, false, Transparency);
                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame3),
                    x + frame3.Frame.PivotX - (fliph ? (frame2.Frame.Width + 41) : 0),
                    y + frame3.Frame.PivotY + (flipv ? (frame3.Frame.Height - editorAnim.Frames[2].Frame.Height) : 0),
                    frame3.Frame.Width, frame3.Frame.Height, false, Transparency);
                d.DrawBitmap(new Classes.Core.Draw.GraphicsHandler.GraphicsInfo(frame4),
                    x + frame4.Frame.PivotX - (fliph ? (frame2.Frame.Width + 31) : 0),
                    y + frame4.Frame.PivotY + (flipv ? (frame4.Frame.Height - editorAnim.Frames[6].Frame.Height) : 0),
                    frame4.Frame.Width, frame4.Frame.Height, false, Transparency);
                    
            }
        }

        public override string GetObjectName()
        {
            return "WoodChipper";
        }
    }
}
