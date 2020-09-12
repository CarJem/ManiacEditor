using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class DirectorChair : EntityRenderer
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
            int size = (int)e.attributesMap["size"].ValueEnum;



            var Animation = LoadAnimation("SPZ1/DirectorChair.bin", d, 1, 0);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
            Animation = LoadAnimation("SPZ1/DirectorChair.bin", d, 1, 0);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y - (size * 5 + 8), Transparency, fliph, flipv);
            Animation = LoadAnimation("SPZ1/DirectorChair.bin", d, 1, 1);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y - (size * 5 + 8), Transparency, fliph, flipv);
            Animation = LoadAnimation("SPZ1/DirectorChair.bin", d, 1, 2);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y - (size * 5 + 8), Transparency, fliph, flipv);
        }

        public override string GetObjectName()
        {
            return "DirectorChair";
        }
    }
}
