using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class MonkeyDude : EntityRenderer
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


            var Animation = LoadAnimation("MonkeyDude", d, 0, 0);

            int i;
            for (i = 0; i < 4; i++)
            {
                Animation = LoadAnimation("MonkeyDude", d, 2, 0);
                DrawTexture(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x - 15 - i * 5, y - 5 - i * 5, Transparency, fliph, flipv);
            }
            i++;
            Animation = LoadAnimation("MonkeyDude", d, 3, 0);
            DrawTexture(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x - 15 - 4 * 5, y - 5 - 4 * 5, Transparency, fliph, flipv);
            i++;
            Animation = LoadAnimation("MonkeyDude", d, 4, 0);
            DrawTexture(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x - 15 - 5 * 5, y - 5 - 5 * 5, Transparency, fliph, flipv);

            Animation = LoadAnimation("MonkeyDude", d, 0, 0);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);

            Animation = LoadAnimation("MonkeyDude", d, 1, 0);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
        }

        public override string GetObjectName()
        {
            return "MonkeyDude";
        }
    }
}
