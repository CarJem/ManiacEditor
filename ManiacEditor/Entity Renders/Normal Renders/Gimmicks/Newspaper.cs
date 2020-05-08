using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Newspaper : EntityRenderer
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
            int frameID = 0;
            switch (type)
            {
                case 0:
                    frameID = 0;
                    break;
                case 1:
                    frameID = 1;
                    break;
                case 2:
                    frameID = 2;
                    break;
                case 3:
                    frameID = 3;
                    break;
            }

            var Animation = LoadAnimation("Newspaper", d, 1, frameID);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
        }

        public override string GetObjectName()
        {
            return "Newspaper";
        }
    }
}
