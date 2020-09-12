using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Bridge : EntityRenderer
    {
        public override void Draw(Structures.EntityRenderProp Properties)
        {
            DevicePanel d = Properties.Graphics;
            Classes.Scene.EditorEntity e = Properties.EditorObject;
            
            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;

            var value = e.attributesMap["length"].ValueUInt8;
            var Animation = LoadAnimation(d, GetSetupAnimation());
            bool wEven = value % 2 == 0;
            for (int xx = 0; xx <= value; ++xx)
            {
                int rx = x + (wEven ? Animation.RequestedFrame.PivotX : -Animation.RequestedFrame.Width) + (-value / 2 + xx) * Animation.RequestedFrame.Width;
                int ry = y + Animation.RequestedFrame.PivotY;
                DrawTexture(Properties.Graphics, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, rx, ry, Transparency);
            }
        }

        public override bool isObjectOnScreen(DevicePanel d, Classes.Scene.EditorEntity entity, int x, int y, int Transparency)
        {
            var length = entity.attributesMap["length"].ValueUInt8;
            int widthPixels = length * 16;
            int heightPixels = 16;
            return d.IsObjectOnScreen(x - widthPixels / 2, y - heightPixels / 2, widthPixels + 15, heightPixels);
        }

        public override string GetSetupAnimation()
        {
            return GetSpriteAnimationPath("/Bridge.bin", "Bridge", new string[] { "LRZ1", "HCZ", "GHZ" });
        }

        public override string GetObjectName()
        {
            return "Bridge";
        }
    }
}
