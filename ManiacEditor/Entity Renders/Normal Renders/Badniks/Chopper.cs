using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Chopper : EntityRenderer
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
            bool charge = e.attributesMap["charge"].ValueBool;
            bool fliph = false;
            bool flipv = false;
            int animID;
            if (type == 1)
            {
                if (charge == true)
                {
                    animID = 3;
                }
                else
                {
                    animID = 1;
                }

            }
            else
            {
                animID = 0;
            }
            if (direction == 1)
            {
                fliph = true;
            }
            if (direction == 1)
            {
                fliph = true;
            }

            var Animation = LoadAnimation("Chopper", d, animID, 0);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
        }

        public override string GetObjectName()
        {
            return "Chopper";
        }
    }
}
