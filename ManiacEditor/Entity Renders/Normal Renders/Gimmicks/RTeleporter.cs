using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class RTeleporter : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp Properties)
        {
            DevicePanel d = Properties.Graphics;
            Classes.Scene.EditorEntity e = Properties.EditorObject;
            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;

            var editorAnim = LoadAnimation("RGenerator", d, 0, 0);
            DrawTexturePivotNormal(d, editorAnim, editorAnim.RequestedAnimID, editorAnim.RequestedFrameID, x, y, Transparency, false, false);
            editorAnim = LoadAnimation("RGenerator", d, 0, 0);
            DrawTexture(d, editorAnim, editorAnim.RequestedAnimID, editorAnim.RequestedFrameID, x + editorAnim.RequestedFrame.PivotX, y + editorAnim.RequestedFrame.Height/2, Transparency, false, true);
            editorAnim = LoadAnimation("RGenerator", d, 1, 0);
            DrawTexturePivotNormal(d, editorAnim, editorAnim.RequestedAnimID, editorAnim.RequestedFrameID, x - 22, y, Transparency, false, false);
            DrawTexturePivotNormal(d, editorAnim, editorAnim.RequestedAnimID, editorAnim.RequestedFrameID, x - 6, y, Transparency, false, false);
            DrawTexturePivotNormal(d, editorAnim, editorAnim.RequestedAnimID, editorAnim.RequestedFrameID, x + 10, y, Transparency, false, false);;
        }

        public override string GetObjectName()
        {
            return "RTeleporter";
        }
    }
}
