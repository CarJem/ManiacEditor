using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class SeltzerBottle : EntityRenderer
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
            if (direction == 1)
            {
                fliph = true;
            }

            var Animation = LoadAnimation("MSZ/Seltzer.bin", d, 0, 0);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
            Animation = LoadAnimation("MSZ/Seltzer.bin", d, 1, 0);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
            Animation = LoadAnimation("MSZ/Seltzer.bin", d, 0, 1);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
            Animation = LoadAnimation("MSZ/Seltzer.bin", d, 0, 2);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
            Animation = LoadAnimation("MSZ/Seltzer.bin", d, 0, 3);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
            Animation = LoadAnimation("MSZ/Seltzer.bin", d, 0, 5);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x + (fliph ? 10 : 0), y, Transparency, fliph, flipv);
            Animation = LoadAnimation("MSZ/Seltzer.bin", d, 0, 4);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, false, false);
        }

        public override string GetObjectName()
        {
            return "SeltzerBottle";
        }
    }
}
