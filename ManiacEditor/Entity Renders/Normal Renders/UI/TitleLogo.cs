using ManiacEditor.Classes.Scene;
using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class TitleLogo : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp properties)
        {
            DevicePanel d = properties.Graphics;
            EditorEntity e = properties.EditorObject;
            int x = properties.DrawX;
            int y = properties.DrawY;
            int Transparency = properties.Transparency;
            bool fliph = false;
            bool flipv = false;

            bool mirrorFrames = false;
            int frameID = (int)e.attributesMap["type"].ValueEnum;
            var Animation = LoadAnimation("Title/Logo.bin", d, frameID, 0);
            if (frameID == 2)
            {
                DrawTexture(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x - 118 - 2, y + Animation.RequestedFrame.PivotY, Transparency, fliph, flipv);
            }
            else
            {
                DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
            }
            if (frameID == 1 || frameID == 2 || frameID == 0) mirrorFrames = true;
            if (mirrorFrames)
            {
                if (frameID == 2)
                {
                    Animation = LoadAnimation("Title/Logo.bin", d, frameID, 0);
                    DrawTexture(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x + 118 + 3 - Animation.RequestedFrame.Width, y + Animation.RequestedFrame.PivotY, Transparency, true, flipv);
                }
                else
                {
                    DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x - Animation.RequestedFrame.PivotX, y, Transparency, true, flipv);
                }
            }

        }

        public override string GetObjectName()
        {
            return "TitleLogo";
        }
    }
}
