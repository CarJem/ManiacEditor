using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class ForceUnstick : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp Properties)
        {
            DevicePanel d = Properties.Graphics;

            Classes.Scene.EditorEntity e = Properties.EditorObject;
            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;

            var width = (int)(e.attributesMap["width"].ValueUInt8);
            var height = (int)(e.attributesMap["height"].ValueUInt8);
            bool breakClimb = e.attributesMap["breakClimb"].ValueBool;
            int type;
            if (breakClimb)
                type = 9;
            else
                type = 6;
            var editorAnim = LoadAnimation("Global/ItemBox.bin", d, 2, type);
            if (editorAnim != null && editorAnim.RequestedFrame != null)
            {
                bool wEven = width % 2 == 0;
                bool hEven = height % 2 == 0;
                for (int xx = 0; xx <= width; ++xx)
                {
                    for (int yy = 0; yy <= height; ++yy)
                    {
                        int drawX = x + (wEven ? editorAnim.RequestedFrame.PivotX : -editorAnim.RequestedFrame.Width) + (-width / 2 + xx) * editorAnim.RequestedFrame.Width;
                        int drawY = y + (hEven ? editorAnim.RequestedFrame.PivotY : -editorAnim.RequestedFrame.Height) + (-height / 2 + yy) * editorAnim.RequestedFrame.Height;
                        DrawTexture(d, editorAnim, editorAnim.RequestedAnimID, editorAnim.RequestedFrameID, drawX, drawY, Transparency);
                    }
                }
            }
        }

        /*
        public override bool isObjectOnScreen(DevicePanel d, Classes.Scene.EditorEntity entity, int x, int y, int Transparency)
        {
            var width = (int)(entity.attributesMap["width"].ValueUInt8);
            var height = (int)(entity.attributesMap["height"].ValueUInt8);
            int widthPixels = width * 16;
            int heightPixels = height * 16;
            return d.IsObjectOnScreen(x - 8 - widthPixels / 2, y - 8 - heightPixels / 2, widthPixels + 8, heightPixels + 8);
        }
        */

        public override string GetObjectName()
        {
            return "ForceUnstick";
        }
    }
}
