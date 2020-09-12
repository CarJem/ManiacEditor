using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class UITABanner : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp properties)
        {
            DevicePanel d = properties.Graphics;
            Classes.Scene.EditorEntity e = properties.EditorObject;
            int x = properties.DrawX;
            int y = properties.DrawY;
            int Transparency = properties.Transparency;
            var editorAnimBackground = LoadAnimation("UI/SaveSelect.bin", d, 7, 0);
            DrawTexturePivotNormal(d, editorAnimBackground, editorAnimBackground.RequestedAnimID, editorAnimBackground.RequestedFrameID, x - 107, y, Transparency, false, false);
            var editorAnimFrame = LoadAnimation("EditorUIRender", d, 2, 0);
            DrawTexturePivotNormal(d, editorAnimFrame, editorAnimFrame.RequestedAnimID, editorAnimFrame.RequestedFrameID, x, y, Transparency, false, false);
        }

        public override string GetObjectName()
        {
            return "UITABanner";
        }
    }
}
