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
            //int direction = (int)entity.attributesMap["direction"].ValueUInt8;
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
            var editorAnim = LoadAnimation("StickyPlatform", d, animID, 0);
            DrawTexturePivotNormal(d, editorAnim, editorAnim.RequestedAnimID, editorAnim.RequestedFrameID, x, y, Transparency, fliph, flipv);

        }

        public override string GetObjectName()
        {
            return "StickyPlatform";
        }
    }
}
