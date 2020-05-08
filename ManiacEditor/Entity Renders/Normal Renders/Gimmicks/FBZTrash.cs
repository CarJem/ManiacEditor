using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class FBZTrash : EntityRenderer
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

            int frameID = (int)e.attributesMap["frameID"].ValueEnum;

            var Animation = LoadAnimation("Trash", d, 0, frameID);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);

        }

        public override string GetObjectName()
        {
            return "FBZTrash";
        }
    }
}
