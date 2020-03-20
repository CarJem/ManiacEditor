using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Button : EntityRenderer
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
            int animID = 0;
            if (type == 0 || type == 1)
            {
                animID = 0;
            }
            if (type == 2 || type == 3)
            {
                animID = 1;
            }
            if (type == 3)
            {
                fliph = true;
            }
            if (type == 1)
            {
                flipv = true;
            }


            var Animation = LoadAnimation("Button", d, animID, 2);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);

            Animation = LoadAnimation("Button", d, animID, 1);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);

            Animation = LoadAnimation("Button", d, animID, 0);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
        }

        public override string GetObjectName()
        {
            return "Button";
        }
    }
}
