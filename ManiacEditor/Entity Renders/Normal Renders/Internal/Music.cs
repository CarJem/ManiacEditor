using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Music : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp Properties)
        {
            DevicePanel d = Properties.Graphics;
            Classes.Scene.EditorEntity e = Properties.EditorObject;
            
            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;

            var Animation = LoadAnimation("EditorIcons", d, 0, 1);
            DrawTexturePivotNormal(Properties.Graphics, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency);
        }

        public override string GetObjectName()
        {
            return "Music";
        }
    }
}
