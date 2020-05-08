using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class SSZSpikeBall : EntityRenderer
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

            int direction = (int)e.attributesMap["direction"].ValueUInt8;
            int type = (int)e.attributesMap["type"].ValueUInt8;
            int animID = 0;
            switch (direction)
            {
                case 0:
                    animID = 0;
                    break;
                case 1:
                    animID = 1;
                    break;
                case 2:
                    animID = 2;
                    break;
                case 3:
                    animID = 3;
                    break;
            }


            if (type == 0)
            {
                var Animation = LoadAnimation("SpikeBall", d, 0, animID);
                DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
            }
            else
            {
                var Animation = LoadAnimation("SpikeBall", d, 1, 0);
                DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
            }
        }

        public override string GetObjectName()
        {
            return "SSZSpikeBall";
        }
    }
}
