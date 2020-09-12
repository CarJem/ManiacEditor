using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Jellygnite : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp Properties)
        {
            DevicePanel d = Properties.Graphics;

            Classes.Scene.EditorEntity e = Properties.EditorObject;
            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;

            int direction = (int)e.attributesMap["direction"].ValueUInt8;

            bool fliph = false;
            bool flipv = false;

            if (direction == 1)
            {
                fliph = true;
            }
            if (direction == 2)
            {
                flipv = true;
            }
            if (direction == 3)
            {
                flipv = true;
                fliph = true;
            }


            var Animation = LoadAnimation(GetSetupAnimation(), d, 0, 0);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);

            for (int i = 0; i < 4; i++)
            {

                Animation = LoadAnimation(GetSetupAnimation(), d, 3, 0);
                DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x + 12, y + 6 + 6 * i, Transparency, fliph, flipv);
                DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x - 12, y + 6 + 6 * i, Transparency, fliph, flipv);


                Animation = LoadAnimation(GetSetupAnimation(), d, 5, 0);
                DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y + 6 + 6 * i, Transparency, fliph, flipv);
            }
        }

        public override string GetSetupAnimation()
        {
            return GetSpriteAnimationPath("/Jellygnite.bin", "Jellygnite", new string[] { "HCZ", "HPZ" });
        }

        public override string GetObjectName()
        {
            return "Jellygnite";
        }
    }
}
