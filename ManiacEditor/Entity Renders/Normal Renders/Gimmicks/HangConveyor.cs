using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class HangConveyor : EntityRenderer
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

            int direction = (int)e.attributesMap["direction"].ValueUInt8;
            int length = (int)e.attributesMap["length"].ValueUInt32 * 16;
            if (direction == 1) fliph = true;
            var Animation = LoadAnimation("HangConveyor", d, 0, 0);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x + (direction == 1 ? length / 2 : -(length / 2)), y, Transparency, fliph, flipv);
            Animation = LoadAnimation("HangConveyor", d, 1, 0);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x - (direction == 1 ? length / 2 : -(length / 2)), y, Transparency, !fliph, flipv);


            var frameEnd_Frame_PivotX = Animation.RequestedFrame.PivotX;
            var frameEnd_Frame_Width = Animation.RequestedFrame.Width;
            int start_x = x + frameEnd_Frame_PivotX - length / 2 + frameEnd_Frame_Width- 6;
            int start_x2 = x + frameEnd_Frame_PivotX - length / 2 + frameEnd_Frame_Width- 10;
            int length2 = (length / 16) - 1;
            for (int i = 0; i < length2; i++)
            {
                Animation = LoadAnimation("HangConveyor", d, 2, 0);
                DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, start_x + 16 * i, y - 21, Transparency, fliph, flipv);
            }

            for (int i = 0; i < length2; i++)
            {
                Animation = LoadAnimation("HangConveyor", d, 2, 0);
                DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, start_x2 + 16 * i, y + 21, Transparency, !fliph, flipv);
            }
        }

        public override string GetObjectName()
        {
            return "HangConveyor";
        }
    }
}
