using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class FBZMissile : EntityRenderer
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

            int type = (int)e.attributesMap["type"].ValueEnum;
            int frameID = 0;
            if (type == 2) frameID = 3;
            if (type == 1) frameID = 1;
            var Animation = LoadAnimation("Missile", d, 1, 0);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
        }

        public override string GetObjectName()
        {
            return "FBZMissile";
        }
    }
}
