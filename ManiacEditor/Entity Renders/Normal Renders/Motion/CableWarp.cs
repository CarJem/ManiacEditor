using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class CableWarp : EntityRenderer
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

            int animID;
            int frameID;
            if (type == 2)
            {
                animID = 2;
                frameID = -1;
            }
            else
            {
                animID = 0;
                frameID = 0;
            }


            if (type != 2)
            {
                var Animation = LoadAnimation("SPZ2/CableWarp.bin", d, animID, frameID);
                DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
            }
            else
            {
                var Animation = LoadAnimation("Global/PlaneSwitch.bin", d, 0, 5);
                DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
            }
        }

        public override string GetObjectName()
        {
            return "CableWarp";
        }
    }
}
