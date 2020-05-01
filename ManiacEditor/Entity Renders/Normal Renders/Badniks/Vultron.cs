using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Vultron : EntityRenderer
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
            int direction = (int)e.attributesMap["direction"].ValueUInt8; 

            int frameID;
            switch (type)
            {
                case 0:
                    frameID = 0;
                    break;
                case 1:
                    frameID = 5;
                    break;
                default:
                    frameID = 0;
                    break;
            }
            switch(direction)
            {
                case 0:
                    fliph = false;
                    break;
                case 1:
                    fliph = true;
                    break;
                default:
                    fliph = false;
                    break;
            }


            var Animation = LoadAnimation("Vultron", d, 0, frameID);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
        }

        public override string GetObjectName()
        {
            return "Vultron";
        }
    }
}
