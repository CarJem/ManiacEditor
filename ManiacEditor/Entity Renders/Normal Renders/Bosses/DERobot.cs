using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class DERobot : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp Properties)
        {
            DevicePanel d = Properties.Graphics;
            Classes.Scene.EditorEntity e = Properties.EditorObject;
            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;

            int aniID = (int)e.attributesMap["aniID"].ValueEnum;
            int frameID = (int)e.attributesMap["frameID"].ValueEnum;
            bool fliph = false;
            bool flipv = false;
            var Animation = LoadAnimation("GHZ/DERobot.bin", d, aniID, frameID);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
        }

        public override string GetObjectName()
        {
            return "DERobot";
        }
    }
}
