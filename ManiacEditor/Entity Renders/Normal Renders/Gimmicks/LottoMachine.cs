using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class LottoMachine : EntityRenderer
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


            var Animation = LoadAnimation("LottoMachine", d, 0, 1);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);

            Animation = LoadAnimation("LottoMachine", d, 0, 7);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);


            Animation = LoadAnimation("LottoMachine", d, 0, 8);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x + 128, y, Transparency, !fliph, flipv);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x + 128, y + 114, Transparency, !fliph, !flipv);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y + 114, Transparency, fliph, !flipv);


            Animation = LoadAnimation("LottoMachine", d, 0, 9);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x - 4, y, Transparency, fliph, flipv);

            Animation = LoadAnimation("LottoMachine", d, 2, 0);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y + 132, Transparency, fliph, flipv);
            Animation = LoadAnimation("LottoMachine", d, 0, 1);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);


            Animation = LoadAnimation("LottoMachine", d, 0, 2);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x - 136, y, Transparency, !fliph, flipv);
            Animation = LoadAnimation("LottoMachine", d, 0, 3);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x - 136 - 36, y, Transparency, !fliph, flipv);
            Animation = LoadAnimation("LottoMachine", d, 0, 4);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x - 136 - 72, y, Transparency, !fliph, flipv);
        }

        public override bool isObjectOnScreen(DevicePanel d, Classes.Scene.EditorEntity entity, int x, int y, int Transparency)
        {
            int bounds = 330;
            return d.IsObjectOnScreen(x - bounds / 2, y - bounds / 2, bounds, bounds);
        }

        public override string GetObjectName()
        {
            return "LottoMachine";
        }
    }
}
