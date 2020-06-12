using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class SpikeLog : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp Properties)
        {
            DevicePanel d = Properties.Graphics;
            Classes.Scene.EditorEntity entity = Properties.EditorObject;

            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;

            byte frameType = entity.attributesMap["frame"].ValueUInt8;
            bool fliph = false;
            bool flipv = false;
            var Animation = LoadAnimation("SpikeLog", d, 0, frameType);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);

        }

        public override string GetObjectName()
        {
            return "SpikeLog";
        }
    }
}
