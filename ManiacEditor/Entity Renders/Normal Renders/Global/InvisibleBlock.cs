using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class InvisibleBlock : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp Properties)
        {
            DevicePanel d = Properties.Graphics;
            Classes.Scene.EditorEntity e = Properties.EditorObject;
            SceneEntity entity = e.Entity;
            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;

            var width = (int)(entity.attributesMap["width"].ValueUInt8);
            var height = (int)(entity.attributesMap["height"].ValueUInt8);

            var Animation = Methods.Entities.EntityDrawing.LoadAnimation(Properties.Graphics, "EditorAssets", 2, 10);

            bool wEven = width % 2 == 0;
            bool hEven = height % 2 == 0;
            for (int xx = 0; xx <= width; ++xx)
            {
                for (int yy = 0; yy <= height; ++yy)
                {
                    int realX = x + (wEven ? Animation.RequestedFrame.PivotX : -Animation.RequestedFrame.Width) + (-width / 2 + xx) * Animation.RequestedFrame.Width;
                    int realY = y + (hEven ? Animation.RequestedFrame.PivotY : -Animation.RequestedFrame.Height) + (-height / 2 + yy) * Animation.RequestedFrame.Height;
                    DrawTexture(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, realX, realY, Transparency);
                }
            }
        }

        public override bool isObjectOnScreen(DevicePanel d, SceneEntity entity, Classes.Scene.EditorEntity e, int x, int y, int Transparency)
        {
            var width = (int)(entity.attributesMap["width"].ValueUInt8);
            var height = (int)(entity.attributesMap["height"].ValueUInt8);
            int widthPixels = width * 16;
            int heightPixels = height * 16;
            return d.IsObjectOnScreen(x - 8 - widthPixels / 2, y - 8 - heightPixels / 2, widthPixels + 8, heightPixels + 8);
        }

        public override string GetObjectName()
        {
            return "InvisibleBlock";
        }
    }
}
