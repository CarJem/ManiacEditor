using RSDKv5;
using ManiacEditor.Extensions;

namespace ManiacEditor.Entity_Renders
{
    public class TVVan : EntityRenderer
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
            int objType = 0;
            switch (type)
            {
                case 0:
                    objType = 0;
                    break;
                case 1:
                    objType = 1;
                    break;
                case 2:
                    objType = 2;
                    break;
                case 3:
                    objType = 3;
                    break;
                case 4:
                    objType = 4;
                    break;
                case 5:
                    objType = 5;
                    break;
                case 6:
                    objType = 6;
                    break;
                case 7:
                    objType = 7;
                    break;
                case 8:
                    objType = 8;
                    break;
                case 9:
                    objType = 9;
                    break;
                case 10:
                    objType = 10;
                    break;
                case 11:
                    objType = 11;
                    break;
                case 12:
                    objType = 12;
                    break;
                case 13:
                    objType = 13;
                    break;
                case 14:
                    objType = 14;
                    break;
                default:
                    objType = 14;
                    break;

            }

            if (objType == 0 || objType == 12 || objType == 13) // Normal (TV Van)
            {
                var Animation = LoadAnimation("TVVan", d, 0, 7);
                DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
                Animation = LoadAnimation("TVVan", d, 0, 0);
                DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
                Animation = LoadAnimation("TVVan", d, 0, 1);
                DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
                Animation = LoadAnimation("TVVan", d, 0, 4);
                DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
                Animation = LoadAnimation("TVVan", d, 0, 2);
                DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
                Animation = LoadAnimation("TVVan", d, 0, 3);
                DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
                Animation = LoadAnimation("TVVan", d, 0, 5);
                DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
                Animation = LoadAnimation("TVVan", d, 0, 6);
                DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
                Animation = LoadAnimation("TVVan", d, 0, 8);
                DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);

                Animation = LoadAnimation("TVVan", d, 1, 0);
                DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
                Animation = LoadAnimation("TVVan", d, 2, 0);
                DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
            }

            if (objType == 1) // Reverse (TV Van)
            {
                int offset1 = -12, offset2 = 0, offset3 = 96, offset4 = 80, offset5 = 80, offset6 = 94, offset7 = -17, offset8 = -16, offset9 = 43, offset10 = 128, offset11 = 41;

                var Animation = LoadAnimation("TVVan", d, 0, 7);
                DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x + offset1, y, Transparency, true, flipv);
                Animation = LoadAnimation("TVVan", d, 0, 0);
                DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x + offset2, y, Transparency, true, flipv);
                Animation = LoadAnimation("TVVan", d, 0, 1);
                DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x + offset3, y, Transparency, true, flipv);
                Animation = LoadAnimation("TVVan", d, 0, 4);
                DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x + offset4, y, Transparency, true, flipv);
                Animation = LoadAnimation("TVVan", d, 0, 2);
                DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x + offset5, y, Transparency, true, flipv);
                Animation = LoadAnimation("TVVan", d, 0, 3);
                DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x + offset6, y, Transparency, true, flipv);
                Animation = LoadAnimation("TVVan", d, 0, 5);
                DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x + offset7, y, Transparency, true, flipv);
                Animation = LoadAnimation("TVVan", d, 0, 6);
                DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x + offset8, y, Transparency, true, flipv);
                Animation = LoadAnimation("TVVan", d, 0, 8);
                DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x + offset9, y, Transparency, true, flipv);

                Animation = LoadAnimation("TVVan", d, 1, 0);
                DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x + offset10, y, Transparency, true, flipv);
                Animation = LoadAnimation("TVVan", d, 2, 0);
                DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x + offset11, y, Transparency, true, flipv);
            }

            if (objType >= 14) //Game Gear TV
            {
                var Animation = LoadAnimation("TVVan", d, 0, 9);
                DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
                Animation = LoadAnimation("TVVan", d, 15, 0);
                DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
            }

            if (objType >= 2 && objType <= 10)
            {
                int offset1 = 0; 
                int offset2 = 0;
                int offset3 = -31;
                int offset4 = -31;
                int offset5 = 0;
                int offset6 = 0; 
                int offset7 = 0; 
                int offset8 = 40;
                int offset9 = 0; 
                int offset10 = 40; 
                int offset11 = 0;
                int offset12 = 52;



                bool DrawTop = objType.In(8, 10);
                bool DrawBottom = objType.In(9, 11);
                bool DrawN = objType.In(2, 5, 6, 11);
                bool DrawVH = objType.In(2, 4, 7, 8);
                bool DrawH = objType.In(3, 4, 6, 9);
                bool DrawV = objType.In(3, 5, 7, 10);

                var Animation = LoadAnimation("TVVan", d, 6, 0);

                if (DrawN) DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x + offset1, y + offset7, Transparency, false, false);
                if (DrawV) DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x + offset2, y + offset8, Transparency, false, true);
                if (DrawH) DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x + offset3, y + offset9, Transparency, true, false);
                if (DrawVH) DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x + offset4, y + offset10, Transparency, true, true);

                Animation = LoadAnimation("TVVan", d, 6, 1);

                if (DrawTop) DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x + offset5, y + offset11, Transparency, false, false);
                if (DrawBottom) DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x + offset6, y + offset12, Transparency, false, true);

                Animation = LoadAnimation("TVVan", d, 6, 2);
                DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, false, false);
            }

        }

        public override string GetObjectName()
        {
            return "TVVan";
        }
    }
}
