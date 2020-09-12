using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class TwistedTubes : EntityRenderer
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
            int height = (int)entity.attributesMap["height"].ValueUInt8;
            var editorAnim2 = LoadAnimation("CPZ/TwistedTubes.bin", d, 0, 0);
            DrawTexturePivotNormal(d, editorAnim2, editorAnim2.RequestedAnimID, editorAnim2.RequestedFrameID, x, y, Transparency);
            var editorAnim = LoadAnimation("CPZ/TwistedTubes.bin", d, 0, 1);
            for (int i = 0; i < height * 2; i++)
            {
                DrawTexturePivotPlus(d, editorAnim, editorAnim.RequestedAnimID, editorAnim.RequestedFrameID, x, y + (i * 32), 0, 64, Transparency);
            }
        }

        public override string GetObjectName()
        {
            return "TwistedTubes";
        }
    }
}
