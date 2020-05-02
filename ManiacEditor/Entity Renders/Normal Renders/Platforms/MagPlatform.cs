using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class MagPlatform : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp Properties)
        {
            DevicePanel d = Properties.Graphics;

            Classes.Scene.EditorEntity e = Properties.EditorObject;
            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;

            bool fliph = false;
            bool flipv = false;

            int length = (int)e.attributesMap["length"].ValueEnum;
            int i = 0;          

            int lengthMemory = length;
            int repeat = 0;
            int lengthLeft = length;
            bool finalLoop = false;
            int start_y = y - length - 4;
            int length_y = y - length + 24;
            while (lengthLeft > 256)
            {
                repeat++;
                lengthLeft = lengthLeft - 256;
            }

            var Animation = LoadAnimation("Platform", d, 3, 1);
            DrawTexturePivotLengthLimit(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, length_y - (i * 256), Transparency, lengthMemory);


            for (i = 1; i < repeat + 1; i++)
            {
                if (i == repeat) finalLoop = true;
                int offset_y = y + (i * 256);
                Animation = LoadAnimation("Platform", d, 3, 1);
                DrawTexturePivotLengthLimit(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, offset_y, Transparency, (finalLoop ? lengthLeft : 24));
            }

            Animation = LoadAnimation("Platform", d, 3, 0);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, start_y, Transparency, fliph, flipv);
            Animation = LoadAnimation("Platform", d, 3, 2);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
        }

        public override string GetObjectName()
        {
            return "MagPlatform";
        }
    }
}
