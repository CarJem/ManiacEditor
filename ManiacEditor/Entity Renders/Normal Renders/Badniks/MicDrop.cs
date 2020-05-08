using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class MicDrop : EntityRenderer
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

            int distance = e.attributesMap["distance"].ValueUInt16;
            d.DrawLine(x, y, x, y + distance, System.Drawing.Color.Black);
            var Animation = LoadAnimation("MicDrop", d, 0, 0);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y + distance, Transparency, fliph, flipv);
        }

        public override string GetObjectName()
        {
            return "MicDrop";
        }
    }
}
