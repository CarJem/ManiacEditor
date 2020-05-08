using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Turntable : EntityRenderer
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


            int type = (int)e.attributesMap["type"].ValueUInt8;
            int animID;

            if (type == 1)
            {
                animID = 1;
            }
            else
            {
                animID = 0;
            }


            var Animation = LoadAnimation("Turntable", d, animID, 0);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
        }

        public override string GetObjectName()
        {
            return "Turntable";
        }
    }
}
