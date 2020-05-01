using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Dragonfly : EntityRenderer
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
        }

        public override string GetObjectName()
        {
            return "Dragonfly";
        }
    }
}
