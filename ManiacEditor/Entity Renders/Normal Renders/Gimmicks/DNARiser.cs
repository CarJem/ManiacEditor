using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class DNARiser : EntityRenderer
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

            var Animation = LoadAnimation("CPZ/DNARiser.bin", d, 0, 0);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency - 111, fliph, flipv);

            Animation = LoadAnimation("CPZ/DNARiser.bin", d, 2, 0);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x - 24, y, Transparency, fliph, flipv);

            Animation = LoadAnimation("CPZ/DNARiser.bin", d, 4, 0);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x + 24, y, Transparency, fliph, flipv);
        }

        public override string GetObjectName()
        {
            return "DNARiser";
        }
    }
}
