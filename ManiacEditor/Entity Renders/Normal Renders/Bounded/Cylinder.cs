using RSDKv5;
using SystemColors = System.Drawing.Color;

namespace ManiacEditor.Entity_Renders
{
    public class Cylinder : EntityRenderer
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
            int multiplierX = 0;
            int multiplierY = 0;
            int widthPixels = 0;
            int heightPixels = 0;
            switch (type)
            {
                case 0:
                    multiplierX = 2;
                    multiplierY = 2;
                    widthPixels = (int)(e.attributesMap["length"].ValueEnum) * multiplierX;
                    heightPixels = (int)(e.attributesMap["radius"].ValueEnum) * multiplierY;
                    break;
                case 1:
                    multiplierX = 2;
                    multiplierY = 2;
                    widthPixels = (int)(e.attributesMap["radius"].ValueEnum) * multiplierX;
                    heightPixels = (int)(e.attributesMap["length"].ValueEnum) * multiplierY;
                    break;
                case 2:
                    multiplierX = 2;
                    multiplierY = 2;
                    widthPixels = (int)(e.attributesMap["radius"].ValueEnum) * multiplierX;
                    heightPixels = (int)(e.attributesMap["length"].ValueEnum) * multiplierY;
                    break;
                case 3: //Reverse Direction of 3
                    multiplierX = 2;
                    multiplierY = 2;
                    widthPixels = (int)(e.attributesMap["radius"].ValueEnum) * multiplierX;
                    heightPixels = (int)(e.attributesMap["length"].ValueEnum) * multiplierY;
                    break;
                case 4:
                    multiplierX = 2;
                    multiplierY = 2;
                    widthPixels = (int)(e.attributesMap["length"].ValueEnum) * multiplierX;
                    heightPixels = (int)(e.attributesMap["radius"].ValueEnum) * multiplierY;
                    break;
                case 5: // Nothing Apparently
                    widthPixels = (int)(e.attributesMap["length"].ValueEnum) * multiplierX;
                    heightPixels = (int)(e.attributesMap["radius"].ValueEnum) * multiplierY;
                    break;
                case 6: // Nothing Apparently
                    widthPixels = (int)(e.attributesMap["length"].ValueEnum) * multiplierX;
                    heightPixels = (int)(e.attributesMap["radius"].ValueEnum) * multiplierY;
                    break;
                case 7:
                    multiplierX = 2;
                    multiplierY = 2;
                    widthPixels = (int)(e.attributesMap["radius"].ValueEnum) * multiplierX;
                    heightPixels = (int)(e.attributesMap["length"].ValueEnum) * multiplierY;
                    break;
            }

            var width = (int)widthPixels / 16;
            var height = (int)heightPixels / 16;

            var Animation = LoadAnimation("Cylinder", d, 0, 0);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, false, false);

            if (type == 5 || type == 6 || type > 7)
            {
                Animation = LoadAnimation("EditorAssets", d, 6, 0);
                DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, false, false);
            }
            DrawBounds(d, x, y, widthPixels, heightPixels, Transparency, SystemColors.White, SystemColors.Transparent);
        }

        public override bool isObjectOnScreen(DevicePanel d, Classes.Scene.EditorEntity entity, int x, int y, int Transparency)
        {
            int type = (int)entity.attributesMap["type"].ValueUInt8;
            int multiplierX = 0;
            int multiplierY = 0;
            int widthPixels = 0;
            int heightPixels = 0;
            switch (type)
            {
                case 0:
                    multiplierX = 2;
                    multiplierY = 2;
                    widthPixels = (int)(entity.attributesMap["length"].ValueEnum) * multiplierX;
                    heightPixels = (int)(entity.attributesMap["radius"].ValueEnum) * multiplierY;
                    break;
                case 1:
                    multiplierX = 2;
                    multiplierY = 2;
                    widthPixels = (int)(entity.attributesMap["radius"].ValueEnum) * multiplierX;
                    heightPixels = (int)(entity.attributesMap["length"].ValueEnum) * multiplierY;
                    break;
                case 2:
                    multiplierX = 2;
                    multiplierY = 2;
                    widthPixels = (int)(entity.attributesMap["radius"].ValueEnum) * multiplierX;
                    heightPixels = (int)(entity.attributesMap["length"].ValueEnum) * multiplierY;
                    break;
                case 3: //Reverse Direction of 3
                    multiplierX = 2;
                    multiplierY = 2;
                    widthPixels = (int)(entity.attributesMap["radius"].ValueEnum) * multiplierX;
                    heightPixels = (int)(entity.attributesMap["length"].ValueEnum) * multiplierY;
                    break;
                case 4:
                    multiplierX = 2;
                    multiplierY = 2;
                    widthPixels = (int)(entity.attributesMap["length"].ValueEnum) * multiplierX;
                    heightPixels = (int)(entity.attributesMap["radius"].ValueEnum) * multiplierY;
                    break;
                case 5: // Nothing Apparently
                    widthPixels = (int)(entity.attributesMap["length"].ValueEnum) * multiplierX;
                    heightPixels = (int)(entity.attributesMap["radius"].ValueEnum) * multiplierY;
                    break;
                case 6: // Nothing Apparently
                    widthPixels = (int)(entity.attributesMap["length"].ValueEnum) * multiplierX;
                    heightPixels = (int)(entity.attributesMap["radius"].ValueEnum) * multiplierY;
                    break;
                case 7:
                    multiplierX = 2;
                    multiplierY = 2;
                    widthPixels = (int)(entity.attributesMap["radius"].ValueEnum) * multiplierX;
                    heightPixels = (int)(entity.attributesMap["length"].ValueEnum) * multiplierY;
                    break;
            }
            if (widthPixels != 0 && heightPixels != 0)
            {
                return d.IsObjectOnScreen(x - widthPixels / 2, y - heightPixels / 2, widthPixels, heightPixels);
            }
            else
            {
                return d.IsObjectOnScreen(x - 16, y - 16, 32, 32);
            }

        }

        public override string GetObjectName()
        {
            return "Cylinder";
        }
    }
}
