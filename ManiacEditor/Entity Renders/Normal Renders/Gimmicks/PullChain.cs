using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class PullChain : EntityRenderer
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
            bool decorMode = e.attributesMap["decorMode"].ValueBool;
            int length = (int)e.attributesMap["length"].ValueUInt32;
            int frameID = 0;
            if (decorMode == true) frameID = 1;





            var Animation = LoadAnimation("HCZ/PullChain.bin", d, 0, frameID);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
            if (length != 0)
            {
                for (int i = 0; i < length; i++)
                {
                    Animation = LoadAnimation("HCZ/PullChain.bin", d, 1, frameID);
                    DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y - Animation.RequestedFrame.Height * i, Transparency, fliph, flipv);
                }
            }
        }

        public override string GetObjectName()
        {
            return "PullChain";
        }
    }
}
