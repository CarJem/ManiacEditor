using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Newtron : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp Properties)
        {
            DevicePanel d = Properties.Graphics;
            Classes.Scene.EditorEntity e = Properties.EditorObject;
            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;

            int type = (int)e.attributesMap["type"].ValueUInt8;
            int direction = (int)e.attributesMap["direction"].ValueUInt8;
            bool fliph = false;
            bool flipv = false;
            int animID;
            if (type == 1)
            {
                animID = 2;
            }
            else
            {
                animID = 0;
            }
            if (direction == 1)
            {
                fliph = true;
            }

            var Animation = Methods.Entities.EntityDrawing.LoadAnimation(Properties.Graphics, "Newtron", animID, 0);
            DrawTexturePivotNormal(Properties.Graphics, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
        }

        public override string GetObjectName()
        {
            return "Newtron";
        }
    }
}
