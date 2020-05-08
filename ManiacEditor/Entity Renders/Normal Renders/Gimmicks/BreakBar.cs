using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class BreakBar : EntityRenderer
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

            var length = (short)(e.attributesMap["length"].ValueUInt16);
            var orientation = e.attributesMap["orientation"].ValueUInt8;
            if (orientation >= 2) orientation = 0;



            var Animation = LoadAnimation("BreakBar", d, orientation, 1);
            int baseWidth = Animation.RequestedFrame.Width;
            int baseHeight = Animation.RequestedFrame.Height;


            for (int i = -length / 2; i <= length / 2; ++i)
            {
                Animation = LoadAnimation("BreakBar", d, orientation, 1);
                DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x + (orientation == 1 ? i * baseWidth : 0), y + (orientation == 0 ? i * baseHeight : 0), Transparency, fliph, flipv);
            }

            Animation = LoadAnimation("BreakBar", d, orientation, 0);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x - (orientation == 1 ? (length / 2 + 1) * baseWidth : 0) + (orientation == 1 ? 4 : 0), y - (orientation == 0 ? (length / 2 + 1) * baseHeight : 0) + (orientation == 0 ? 4 : 0), Transparency, fliph, flipv);

            Animation = LoadAnimation("BreakBar", d, orientation, 2);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x + (orientation == 1 ? (length / 2 + 1) * baseWidth : 0) - (orientation == 1 ? 4 : 0), y + (orientation == 0 ? (length / 2 + 1) * baseHeight : 0) - (orientation == 0 ? 4 : 0), Transparency, fliph, flipv);
        }

        public override string GetObjectName()
        {
            return "BreakBar";
        }
    }
}
