using RSDKv5;
using System.Linq;

namespace ManiacEditor.Entity_Renders
{
    public class WoodChipper : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp properties)
        {
            DevicePanel d = properties.Graphics;
            Classes.Scene.EditorEntity entity = properties.EditorObject;
            int x = properties.DrawX;
            int y = properties.DrawY;
            int Transparency = properties.Transparency;
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

            //var log3 = LoadAnimation("PSZ2/WoodChipper.bin", d, 0, 5);

            if (size > 0)
            {
                int repeat = 1;
                int sizeMemory = size;
                bool finalLoop = false;

                var log1 = LoadAnimation("PSZ2/WoodChipper.bin", d, 0, 3);
                DrawTexturePivotNormal(d, log1, log1.RequestedAnimID, log1.RequestedFrameID, x, y - size, Transparency, false, false);
                var log2 = LoadAnimation("PSZ2/WoodChipper.bin", d, 0, 4);
                DrawTexturePivotNormal(d, log2, log2.RequestedAnimID, log2.RequestedFrameID, x, y - size, Transparency, false, false);


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
                        if (i == repeat) finalLoop = true;
                        DrawTexturePivotNormalCustom(d, log2, log2.RequestedAnimID, log2.RequestedFrameID, x, y - size + log2.RequestedFrame.Height * i, Transparency, (finalLoop ? sizeMemory : log2.RequestedFrame.Height), false, false);
                    }
                }
            }

            var frame = LoadAnimation("PSZ2/WoodChipper.bin", d, 0, 0);
            DrawTexturePivotNormal(d, frame, frame.RequestedAnimID, frame.RequestedFrameID, x, y, Transparency, fliph, false);
            var frame2 = LoadAnimation("PSZ2/WoodChipper.bin", d, 0, 1);
            int frame2_width = frame.RequestedFrame.Width;
            DrawTexturePivotNormal(d, frame2, frame2.RequestedAnimID, frame2.RequestedFrameID, x - (fliph ? (frame2_width - 92) : 0), y, Transparency, fliph, false);
            var frame3 = LoadAnimation("PSZ2/WoodChipper.bin", d, 0, 2);
            DrawTexturePivotNormal(d, frame3, frame3.RequestedAnimID, frame3.RequestedFrameID, x - (fliph ? (frame2_width + 41) : 0), y, Transparency, fliph, false);
            var frame4 = LoadAnimation("PSZ2/WoodChipper.bin", d, 0, 6);
            DrawTexturePivotNormal(d, frame4, frame4.RequestedAnimID, frame4.RequestedFrameID, x - (fliph ? (frame2_width + 31) : 0), y, Transparency, fliph, false);
        }

        public void DrawTexturePivotNormalCustom(DevicePanel Graphics, Methods.Drawing.ObjectDrawing.EditorAnimation Animation, int AnimID, int FrameID, int x, int y, int Transparency, int height, bool FlipH = false, bool FlipV = false, int rotation = 0, System.Drawing.Color? color = null)
        {
            if (EntityRenderer.IsValidated(Animation, new System.Tuple<int, int>(AnimID, FrameID)))
            {
                var Frame = Animation.Animation.Animations[AnimID].Frames[FrameID];
                Graphics.DrawBitmap(Animation.Spritesheets.ElementAt(Frame.SpriteSheet).Value, x + Frame.PivotX, y + Frame.PivotY, Frame.X, Frame.Y, Frame.Width, height, false, Transparency, FlipH, FlipV, rotation, color);
            }
        }

        public override string GetObjectName()
        {
            return "WoodChipper";
        }
    }
}
