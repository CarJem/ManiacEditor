using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class LottoBall : EntityRenderer
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
            int lottoNum = (int)e.attributesMap["lottoNum"].ValueUInt8;
            bool ignoreNum = false;
            int frameID1 = 0;
            int frameID2 = 0;
            int frameID3 = 0;
            switch (type)
            {
                case 1:
                    frameID1 = 1;
                    break;
                case 2:
                    frameID1 = 2;
                    break;
                case 3:
                    frameID1 = 3;
                    break;
                default:
                    frameID1 = 0;
                    break;
            }
            if (type == 1 || type == 0)
            {
                if (lottoNum <= 9)
                {
                    frameID2 = 0;
                    frameID3 = lottoNum;
                }
                else if (lottoNum <= 99)
                {
                    int lottoNumReduced = lottoNum;
                    int lottoNumTens = 0;
                    for (int i = 0; lottoNumReduced <= 9; i++)
                    {
                        if (lottoNumReduced >= 10)
                        {
                            lottoNumReduced = lottoNumReduced - 10;
                            lottoNumTens++;
                        }
                    }
                    frameID2 = lottoNumTens;
                    frameID3 = lottoNumReduced;
                }
                else
                {
                    frameID2 = 9;
                    frameID3 = 9;
                }
            }
            if (type == 2 && lottoNum <= 9)
            {
                frameID2 = 10;
                frameID3 = lottoNum;
            }
            if (type == 3)
            {
                frameID2 = 11;
                ignoreNum = true;
            }

            var Animation = LoadAnimation("SPZ2/LottoBall.bin", d, 0, frameID1);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
            Animation = LoadAnimation("SPZ2/LottoBall.bin", d, 1, frameID2);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
            if (ignoreNum != true)
            {
                Animation = LoadAnimation("SPZ2/LottoBall.bin", d, 2, frameID3);
                DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
            }
        }

        public override string GetObjectName()
        {
            return "LottoBall";
        }
    }
}
