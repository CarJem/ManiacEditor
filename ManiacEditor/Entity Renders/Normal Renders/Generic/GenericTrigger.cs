using RSDKv5;
using SystemColors = System.Drawing.Color;

namespace ManiacEditor.Entity_Renders
{
    public class GenericTrigger : EntityRenderer
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

            var widthPixels = (int)(e.attributesMap["size"].ValueVector2.X.High) * 2;
            var heightPixels = (int)(e.attributesMap["size"].ValueVector2.Y.High) * 2;
            var width = (int)widthPixels / 16;
            var height = (int)heightPixels / 16;

            var Animation = LoadAnimation("EditorIcons2", d, 0, 5);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);

            DrawBounds(d, x, y, widthPixels, heightPixels, Transparency, SystemColors.White, SystemColors.Transparent);


        }

        public override string GetObjectName()
        {
            return "GenericTrigger";
        }
    }
}
