using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Shiversaw : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp Properties)
        {
            DevicePanel d = Properties.Graphics;
            Classes.Scene.EditorEntity entity = Properties.EditorObject;
            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;
            bool fliph = false;
            bool flipv = false;
            var editorAnim = LoadAnimation("PSZ1/Shiversaw.bin", d, 0, 0);
            DrawTexturePivotNormal(d, editorAnim, editorAnim.RequestedAnimID, editorAnim.RequestedFrameID, x, y, Transparency, fliph, flipv);
            var editorAnim2 = LoadAnimation("PSZ1/Shiversaw.bin", d, 1, 0);
            DrawTexturePivotNormal(d, editorAnim2, editorAnim2.RequestedAnimID, editorAnim2.RequestedFrameID, x, y, Transparency, fliph, flipv);
        }

        public override string GetObjectName()
        {
            return "Shiversaw";
        }
    }
}
