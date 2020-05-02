using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Clapperboard : EntityRenderer
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

            int direction = (int)e.attributesMap["direction"].ValueUInt8;
            int flipState = 0;
            if (direction == 1)
            {
                fliph = true;
                flipState = 3;
            }
            else
            {
                fliph = false;
                flipState = 2;
            }

            var Animation = LoadAnimation("Clapperboard", d, 1, 0);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x + (direction == 1 ? -104 : 0), y, Transparency, fliph, flipv);
            Animation = LoadAnimation("Clapperboard", d, 0, 1);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x + (direction == 1 ? -104 : 0), y, Transparency, fliph, flipv);
            Animation = LoadAnimation("Clapperboard", d, 0, flipState);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, false, false);
        }

        public override string GetObjectName()
        {
            return "Clapperboard";
        }
    }
}
