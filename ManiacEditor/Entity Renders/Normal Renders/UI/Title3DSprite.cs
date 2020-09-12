using ManiacEditor.Classes.Scene;
using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Title3DSprite : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp properties)
        {
            DevicePanel d = properties.Graphics;
            EditorEntity e = properties.EditorObject;
            int x = properties.DrawX;
            int y = properties.DrawY;
            int Transparency = properties.Transparency;
            bool fliph = false;
            bool flipv = false;


            int frameID = (int)e.attributesMap["frame"].ValueEnum;
            var Animation = LoadAnimation("Title/Background.bin", d, 5, frameID);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
        }

        public override string GetObjectName()
        {
            return "Title3DSprite";
        }
    }
}
