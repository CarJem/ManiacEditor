using RSDKv5;
using SystemColors = System.Drawing.Color;

namespace ManiacEditor.Entity_Renders
{
    public class DashLift : EntityRenderer
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

            int startOff = (int)e.attributesMap["startOff"].ValueEnum;
            int length = (int)e.attributesMap["length"].ValueEnum;

            var widthPixels = (int)(64);
            var heightPixels = (int)(length);
            var width = (int)widthPixels / 16;
            var height = (int)heightPixels / 16;

            DrawBounds(d, x, y, widthPixels, heightPixels, Transparency, SystemColors.White, SystemColors.Transparent);

            var Animation = LoadAnimation("Platform", d, 2, 0);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y + (startOff > length ? length / 2 : startOff / 2), Transparency, fliph, flipv);

        }

        public override string GetObjectName()
        {
            return "DashLift";
        }
    }
}
