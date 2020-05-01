using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class SwingRope : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp Properties)
        {
            Classes.Scene.EditorEntity e = Properties.EditorObject;
            DevicePanel d = Properties.Graphics;
            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;

            bool fliph = false;
            bool flipv = false;

            int ropeSize = (int)e.attributesMap["ropeSize"].ValueUInt8 + 1;


            var Animation = Methods.Entities.EntityDrawing.LoadAnimation(Properties.Graphics, "SwingRope", 0, 0);
            int frameHeight = Animation.RequestedFrame.Height;
            for (int i = 0; i < ropeSize; i++)
            {
                DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y + frameHeight * i, Transparency, fliph, flipv);
            }
            Animation = Methods.Entities.EntityDrawing.LoadAnimation(Properties.Graphics, "SwingRope", 2, 0);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
            Animation = Methods.Entities.EntityDrawing.LoadAnimation(Properties.Graphics, "SwingRope", 1, 0);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y + frameHeight * ropeSize, Transparency, fliph, flipv);
        }

        public override string GetObjectName()
        {
            return "SwingRope";
        }
    }
}
