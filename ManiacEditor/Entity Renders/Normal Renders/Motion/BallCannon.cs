using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class BallCannon : EntityRenderer
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

            bool selected = true;

            int x2 = x;
            int y2 = y;
            int angle = (int)e.attributesMap["angle"].ValueEnum;
            int type = (int)e.attributesMap["type"].ValueUInt8;
            int rotation = 0;
            int rotation2 = 0;
            int CorkState = 0;
            if (type == 0)
            {
                switch (angle)
                {
                    case 0:
                        rotation = 90;
                        rotation2 = 180;
                        y += 16;
                        x += 8;
                        y2 += 24;
                        x2 += 0;
                        break;
                    case 1:
                        rotation = 180;
                        rotation2 = 270;
                        y += 24;
                        x -= 0;
                        y2 += 16;
                        x2 += -8;
                        break;
                    case 2:
                        rotation = 270;
                        y += 16;
                        x -= 8;
                        y2 += 8;
                        x2 += 0;
                        break;
                    case 3:
                        rotation2 = 90;
                        y += 8;
                        x += 0;
                        y2 += 16;
                        x2 += 8;
                        break;
                    case 4:
                        rotation2 = 90;
                        rotation = 180;
                        y += 24;
                        x += 0;
                        y2 += 16;
                        x2 += 8;
                        break;
                    case 5:
                        rotation = 270;
                        rotation2 = 180;
                        y += 16;
                        x -= 8;
                        y2 += 24;
                        x2 += 0;
                        break;
                    case 6:
                        rotation2 = 270;
                        y += 8;
                        x += 0;
                        y2 += 16;
                        x2 -= 8;
                        break;
                    case 7:
                        rotation2 = 0;
                        rotation = 90;
                        y += 16;
                        x += 8;
                        y2 += 8;
                        x2 += 0;
                        break;
                    default:
                        rotation = 90;
                        rotation2 = 180;
                        y += 16;
                        x += 8;
                        y2 += 24;
                        x2 += 0;
                        break;
                }
            }
            else
            {
                switch (type)
                {
                    case 0:
                        break;
                    case 1:
                        CorkState = 3;
                        break;
                    case 2:
                        CorkState = 4;
                        break;
                }
            }

            var Animation = LoadAnimation("OOZ/BallCannon.bin", d, 1, 0);
            if (type == 1)
            {
                Animation = LoadAnimation("OOZ/BallCannon.bin", d, CorkState, 0);
                DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
            }
            else if (type == 2)
            {
                Animation = LoadAnimation("OOZ/BallCannon.bin", d, 1, 0);
                DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
            }
            else
            {
                if (selected)
                {
                    Animation = LoadAnimation("OOZ/BallCannon.bin", d, 0, 0);
                    DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x2, y2, Transparency - 125, fliph, flipv, rotation2);
                }

                Animation = LoadAnimation("OOZ/BallCannon.bin", d, 0, 0);
                DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv, rotation);
            }
        }

        public override string GetObjectName()
        {
            return "BallCannon";
        }
    }
}
