using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class ElectroMagnet : EntityRenderer
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

            bool invisible = e.attributesMap["invisible"].ValueBool;

            if (!invisible)
            {
                var Animation = LoadAnimation("ElectroMagnet", d, 0, 0);
                DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
            }

        }

        public override string GetObjectName()
        {
            return "ElectroMagnet";
        }
    }
}
