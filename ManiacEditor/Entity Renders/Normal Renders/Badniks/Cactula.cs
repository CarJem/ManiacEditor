using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Cactula : EntityRenderer
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

            var Animation = LoadAnimation("MSZ/Cactula.bin", d, 0, 0);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);

            var Animation2 = LoadAnimation("MSZ/Cactula.bin", d, 0, 1);
            DrawTexturePivotNormal(d, Animation2, Animation2.RequestedAnimID, Animation2.RequestedFrameID, x, y, Transparency, fliph, flipv);

            var Animation3 = LoadAnimation("MSZ/Cactula.bin", d, 1, 5);
            DrawTexturePivotNormal(d, Animation3, Animation3.RequestedAnimID, Animation3.RequestedFrameID, x, y, Transparency, fliph, flipv);
        }

        public override string GetObjectName()
        {
            return "Cactula";
        }
    }
}
