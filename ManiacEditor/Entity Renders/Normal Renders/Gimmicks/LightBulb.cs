using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class LightBulb : EntityRenderer
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

            var Animation = LoadAnimation("LightBulb", d, 0, 2);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency);
            Animation = LoadAnimation("LightBulb", d, 0, 1);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency);
        }

        public override string GetObjectName()
        {
            return "LightBulb";
        }
    }
}
