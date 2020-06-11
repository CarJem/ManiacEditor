using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class HangPoint : EntityRenderer
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

            var Animation = LoadAnimation("HangPoint", d, 0, 0);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y + length, Transparency, fliph, flipv);

            int repeat = 0;
            int lengthLeft = length;
            bool finalLoop = false;
            while (lengthLeft > 256)
            {
                repeat++;
                lengthLeft = lengthLeft - 256;
            }

            for (int i = 0; i < repeat + 1; i++)
            {
                if (i == repeat) finalLoop = true;
                int offset_y = y + (i * 256);
                Animation = LoadAnimation("HangPoint", d, 0, 1);
                DrawTexturePivotLengthLimit(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, offset_y, Transparency, (finalLoop ? lengthLeft : Animation.RequestedFrame.Height));
            }
            

        }

        public override string GetObjectName()
        {
            return "HangPoint";
        }
    }
}
