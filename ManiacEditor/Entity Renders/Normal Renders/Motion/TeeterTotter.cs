using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class TeeterTotter : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp Properties)
        {
            DevicePanel d = Properties.Graphics;
            Classes.Scene.EditorEntity e = Properties.EditorObject;
            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;
            var value = e.attributesMap["length"].ValueUInt32;
            var editorAnim = LoadAnimation("TMZ1/TeeterTotter.bin", d, 0, 0);
            for (int i = -(int)value; i < value; ++i)
            {
                DrawTexturePivotNormal(d, editorAnim, editorAnim.RequestedAnimID, editorAnim.RequestedFrameID, x + (i * (editorAnim.RequestedFrame.Width + 2)), y, Transparency);
            }
        }

        public override string GetObjectName()
        {
            return "TeeterTotter";
        }
    }
}
