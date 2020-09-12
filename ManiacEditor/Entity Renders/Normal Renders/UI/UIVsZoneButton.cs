using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class UIVsZoneButton : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp Properties)
        {
            DevicePanel d = Properties.Graphics;
            Classes.Scene.EditorEntity e = Properties.EditorObject;
            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;

            int aniID = (int)e.attributesMap["aniID"].ValueEnum;
            int frameID = (int)e.attributesMap["frameID"].ValueEnum;
            bool fliph = false;
            bool flipv = false;

            int zoneID = (int)e.attributesMap["zoneID"].ValueEnum;

            var Animation = LoadAnimation("EditorUIRender", d, 4, 1);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x + 3, y, Transparency, fliph, flipv);
            Animation = LoadAnimation("UI/SaveSelect.bin", d, 17, zoneID);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
            Animation = LoadAnimation("EditorUIRender", d, 4, 0);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x + 3, y, Transparency, fliph, flipv);
        }

        public override string GetObjectName()
        {
            return "UIVsZoneButton";
        }
    }
}
