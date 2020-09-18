using ManiacEditor.Classes.Scene;
using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Press : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp properties)
        {
            DevicePanel d = properties.Graphics;
            Classes.Scene.EditorEntity entity = properties.EditorObject;
            int x = properties.DrawX;
            int y = properties.DrawY;
            int Transparency = properties.Transparency;
            int size = (int)entity.attributesMap["size"].ValueUInt16;
            int offTop = (int)entity.attributesMap["offTop"].ValueEnum;
            int offBottom = (int)entity.attributesMap["offBottom"].ValueEnum;
            bool fliph = false;
            bool flipv = false;








            bool hEven = (size % 2 == 0);

            var frame = LoadAnimation("PSZ1/Press.bin", d, 0, 6);
            var frame_height = frame.RequestedFrame.Height;
            var frame_width = frame.RequestedFrame.Width;

            for (int y2 = 0; y2 <= size; ++y2)
            {
                DrawTexture(d, frame, frame.RequestedAnimID, frame.RequestedFrameID,
                    x + -frame_width + (-1 / 2 + 1) * frame_width + frame.RequestedFrame.PivotX,
                    y + -frame_height + (-size / 2 + y2) * frame_height,Transparency);
                if (y2 == size)
                {
                    y2 = y2 + 2;
                    var crankTop2 = LoadAnimation("PSZ1/Press.bin", d, 2, 0);
                    DrawTexture(d, crankTop2, crankTop2.RequestedAnimID, crankTop2.RequestedFrameID,
                        x + crankTop2.RequestedFrame.PivotX,
                        y + -crankTop2.RequestedFrame.Height + (-size / 2 + y2) * frame_height,Transparency);
                }
            }
            int yy = 0;

            var platformEndCap = LoadAnimation("PSZ1/Press.bin", d, 0, 3);
            DrawTexture(d, platformEndCap, platformEndCap.RequestedAnimID, platformEndCap.RequestedFrameID,
                 x + platformEndCap.RequestedFrame.PivotX,
                 y + -platformEndCap.RequestedFrame.Height + (-size / 2 + yy) * frame_height + offTop - platformEndCap.RequestedFrame.PivotY - (hEven ? 0 : 4),Transparency);
            var platform = LoadAnimation("PSZ1/Press.bin", d, 0, 3);
            DrawTexture(d, platform, platform.RequestedAnimID, platform.RequestedFrameID,
                x + platform.RequestedFrame.PivotX,
                y + -platform.RequestedFrame.Height + (-size / 2 + yy) * frame_height + offTop - platform.RequestedFrame.PivotY - (hEven ? 0 : 4),Transparency);

            var platformEndCap2 = LoadAnimation("PSZ1/Press.bin", d, 0, 4);
            DrawTexture(d, platformEndCap2, platformEndCap2.RequestedAnimID, platformEndCap2.RequestedFrameID,
                x + platformEndCap2.RequestedFrame.PivotX,
                y + -platformEndCap2.RequestedFrame.Height + (-size / 2 + yy) * frame_height + offBottom - platformEndCap2.RequestedFrame.PivotY - (hEven ? 0 : 4),Transparency);

            platform = LoadAnimation("PSZ1/Press.bin", d, 0, 3);
            DrawTexture(d, platform, platform.RequestedAnimID, platform.RequestedFrameID,
                x + platform.RequestedFrame.PivotX,
                y + -platform.RequestedFrame.Height + (-size / 2 + yy) * frame_height + offBottom - platform.RequestedFrame.PivotY - (hEven ? 0 : 4), Transparency);

            var crankHolder = LoadAnimation("PSZ1/Press.bin", d, 0, 0);
            DrawTexture(d, crankHolder, crankHolder.RequestedAnimID, crankHolder.RequestedFrameID,
                x + crankHolder.RequestedFrame.PivotX + 74,
                y + -crankHolder.RequestedFrame.Height + (-size / 2 + yy) * frame_height + crankHolder.RequestedFrame.PivotY + 16,Transparency);

            var crankHandle = LoadAnimation("PSZ1/Press.bin", d, 0, 2);
            DrawTexture(d, crankHandle, crankHandle.RequestedAnimID, crankHandle.RequestedFrameID,
                x + crankHandle.RequestedFrame.PivotX + 56,
                y + -crankHandle.RequestedFrame.Height + (-size / 2 + yy) * frame_height + crankHandle.RequestedFrame.PivotY,Transparency);

            var crankTop = LoadAnimation("PSZ1/Press.bin", d, 2, 0);
            DrawTexture(d, crankTop, crankTop.RequestedAnimID, crankTop.RequestedFrameID,
                x + crankTop.RequestedFrame.PivotX,
                y + -crankTop.RequestedFrame.Height + (-size / 2 + yy) * frame_height + crankTop.RequestedFrame.PivotY, Transparency);
        }

        public override bool isObjectOnScreen(DevicePanel d, EditorEntity entity, int x, int y, int Transparency)
        {
            //TODO: Validate
            int size = (int)entity.attributesMap["size"].ValueUInt16;
            int realHeight = size * 16;
            int y_offset = (realHeight != 0 ? (realHeight / 2) : 0);
            return d.IsObjectOnScreen(x - 20, y - y_offset, 40, realHeight);
        }

        public override string GetObjectName()
        {
            return "Press";
        }
    }
}
