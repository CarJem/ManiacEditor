using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class CaterkillerJr : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp Properties)
        {
            DevicePanel d = Properties.Graphics;

            Classes.Scene.EditorEntity e = Properties.EditorObject;

            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;

            bool fliph = false;
            bool flipv = false;

            int direction = (int)e.attributesMap["direction"].ValueUInt8;
            if (direction == 1)
            {
                int x = Properties.DrawX + 37;
                fliph = true;
                var Animation = LoadAnimation("CaterkillerJr", d, 0, 0);
                DrawTexturePivotForced(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, !fliph, flipv);

                Animation = LoadAnimation("CaterkillerJr", d, 1, 0);
                DrawTexturePivotForced(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x - 12, y, Transparency, fliph, flipv);
                DrawTexturePivotForced(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x - 24, y, Transparency, fliph, flipv);
                DrawTexturePivotForced(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x - 36, y, Transparency, fliph, flipv);
                Animation = LoadAnimation("CaterkillerJr", d, 2, 0);
                DrawTexturePivotForced(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x - 47, y, Transparency, fliph, flipv);
                Animation = LoadAnimation("CaterkillerJr", d, 3, 0);
                DrawTexturePivotForced(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x - 55, y, Transparency, fliph, flipv);
                DrawTexturePivotForced(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x - 62, y, Transparency, fliph, flipv);
            }
            else
            {
                int x = Properties.DrawX;
                var Animation = LoadAnimation("CaterkillerJr", d, 0, 0);
                DrawTexturePivotForced(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, !fliph, flipv);

                Animation = LoadAnimation("CaterkillerJr", d, 1, 0);
                DrawTexturePivotForced(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x + 12, y, Transparency, fliph, flipv);
                DrawTexturePivotForced(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x + 24, y, Transparency, fliph, flipv);
                DrawTexturePivotForced(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x + 36, y, Transparency, fliph, flipv);
                Animation = LoadAnimation("CaterkillerJr", d, 2, 0);
                DrawTexturePivotForced(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x + 47, y, Transparency, fliph, flipv);
                Animation = LoadAnimation("CaterkillerJr", d, 3, 0);
                DrawTexturePivotForced(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x + 55, y, Transparency, fliph, flipv);
                DrawTexturePivotForced(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x + 62, y, Transparency, fliph, flipv);
            }



        }

        public override string GetObjectName()
        {
            return "CaterkillerJr";
        }
    }
}
