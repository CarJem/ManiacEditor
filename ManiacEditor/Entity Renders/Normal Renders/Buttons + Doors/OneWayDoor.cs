using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class OneWayDoor : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp Properties)
        {
            DevicePanel d = Properties.Graphics;
            Classes.Scene.EditorEntity e = Properties.EditorObject;
            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;

            int direction = (int)e.attributesMap["direction"].ValueUInt8;
            bool fliph = false;
            bool flipv = false;
            int animID = 0;


            var Animation = LoadAnimation("OneWayDoor", d, animID, 0);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y + 32, Transparency, fliph, flipv);
        }

        public override string GetObjectName()
        {
            return "OneWayDoor";
        }
    }
}
