using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class PlatformNode : EntityRenderer
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

            var Animation = LoadAnimation("EditorIcons2", d, 0, 4);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency);
        }

        public override string GetObjectName()
        {
            return "PlatformNode";
        }
    }
}
