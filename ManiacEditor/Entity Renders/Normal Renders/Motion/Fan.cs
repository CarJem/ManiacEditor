using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Fan : EntityRenderer
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
            int direction = (int)e.attributesMap["direction"].ValueUInt8;
            int animID = 0;
            if (type == 0)
            {
                animID = 0;
                if (direction == 1)
                {
                    flipv = true;
                }
            }
            else if (type == 1)
            {
                animID = 1;
                if (direction == 1)
                {
                    fliph = true;
                }
            }




            string anim = GetSpriteAnimationPath("/Fan.bin", "Fan", new string[] { "HCZ", "OOZ" }, "HCZ"); ;
            var Animation = LoadAnimation(anim, d, animID, 0);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
        }

        public override string GetObjectName()
        {
            return "Fan";
        }
    }
}
