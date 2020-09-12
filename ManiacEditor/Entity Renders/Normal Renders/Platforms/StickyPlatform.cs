using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class StickyPlatform : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp Properties)
        {
            DevicePanel d = Properties.Graphics;
            Classes.Scene.EditorEntity entity = Properties.EditorObject;

            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;

            int type = (int)entity.attributesMap["type"].ValueEnum;
            int amplitudeX = (int)(entity.attributesMap["amplitude"].ValueVector2.X.High);
            int amplitudeY = (int)(entity.attributesMap["amplitude"].ValueVector2.Y.High);
            bool fliph = false;
            bool flipv = false;
            int animID = 0;
            switch (type)
            {
                case 0:
                    break;
                case 1:
                    flipv = true;
                    break;
                case 2:
                    animID = 3;
                    break;
                case 3:
                    animID = 3;
                    fliph = true;
                    break;
                default:
                    break;
            }

            if (amplitudeX != 0 || amplitudeY != 0)
            {
                d.DrawArrow(x, y, x - amplitudeX, y - amplitudeY, System.Drawing.Color.YellowGreen, 2);
                d.DrawArrow(x, y, x + amplitudeX, y + amplitudeY, System.Drawing.Color.YellowGreen, 2);
            }

            var editorAnim = LoadAnimation("CPZ/StickyPlatform.bin", d, animID, 0);
            DrawTexturePivotNormal(d, editorAnim, editorAnim.RequestedAnimID, editorAnim.RequestedFrameID, x, y, Transparency, fliph, flipv);

        }

        public override string GetObjectName()
        {
            return "StickyPlatform";
        }
    }
}
