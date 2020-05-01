using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class TippingPlatform : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp Properties)
        {
            DevicePanel d = Properties.Graphics;
            Classes.Scene.EditorEntity entity = Properties.EditorObject;

            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;


            int direction = 0;
            bool fliph = false;
            bool flipv = false;

            if (entity.AttributeExists("direction", AttributeTypes.UINT8)) direction = (int)entity.attributesMap["direction"].ValueUInt8;
            if (direction == 1) fliph = true;

            var Animation = LoadAnimation("Platform", d, 1, 0);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
        }

        public override string GetObjectName()
        {
            return "TippingPlatform";
        }
    }
}
