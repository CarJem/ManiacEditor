using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class FlameSpring : EntityRenderer
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
            int valveType;
            int animID;
            switch (type)
            {
                case 0:
                    animID = 0;
                    valveType = 0;
                    break;
                case 1:
                    animID = 0;
                    valveType = 1;
                    break;
                case 2:
                    animID = 0;
                    valveType = 2;
                    break;
                case 3:
                    animID = 2;
                    valveType = 0;
                    break;
                case 4:
                    animID = 2;
                    valveType = 1;
                    break;
                case 5:
                    animID = 2;
                    valveType = 2;
                    break;
                default:
                    animID = 0;
                    valveType = 0;
                    break;
            }

            var Animation = LoadAnimation("FBZ/FlameSpring.bin", d, 0, animID);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);

            if (valveType == 2 || valveType == 0)
            {
                Animation = LoadAnimation("FBZ/FlameSpring.bin", d, 1, 0);
                DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x - 14, y - 4, Transparency, fliph, flipv);
            }
            if (valveType == 1 || valveType == 0)
            {
                Animation = LoadAnimation("FBZ/FlameSpring.bin", d, 1, 1);
                DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x + 13, y - 4, Transparency, fliph, flipv);
            }
        }

        public override string GetObjectName()
        {
            return "FlameSpring";
        }
    }
}
