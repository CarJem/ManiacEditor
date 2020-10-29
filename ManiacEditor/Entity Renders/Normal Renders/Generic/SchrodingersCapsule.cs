using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class SchrodingersCapsule : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp properties)
        {
            DevicePanel d = properties.Graphics;
            Classes.Scene.EditorEntity entity = properties.EditorObject;
            int x = properties.DrawX;
            int y = properties.DrawY;
            int Transparency = properties.Transparency;

            var editorAnimInside = LoadAnimation("AIZ/SchrodingersCapsule.bin", d, 0, 2);
            DrawTexturePivotNormal(d, editorAnimInside, editorAnimInside.RequestedAnimID, editorAnimInside.RequestedFrameID, x, y, Transparency);

            var mightyAnim = LoadAnimation("AIZ/SchrodingersCapsule.bin", d, 3, 0);
            DrawTexturePivotNormal(d, mightyAnim, mightyAnim.RequestedAnimID, mightyAnim.RequestedFrameID, x + 15, y, Transparency);
            var rayAnim = LoadAnimation("AIZ/SchrodingersCapsule.bin", d, 4, 0);
            DrawTexturePivotNormal(d, rayAnim, rayAnim.RequestedAnimID, rayAnim.RequestedFrameID, x - 15, y, Transparency);

            var editorAnim = LoadAnimation("AIZ/SchrodingersCapsule.bin", d, 0, 0);
            DrawTexturePivotNormal(d, editorAnim, editorAnim.RequestedAnimID, editorAnim.RequestedFrameID, x, y, Transparency);

            var editorAnim2 = LoadAnimation("AIZ/SchrodingersCapsule.bin", d, 0, 1);
            DrawTexturePivotNormal(d, editorAnim2, editorAnim2.RequestedAnimID, editorAnim2.RequestedFrameID, x, y, Transparency);

            var editorAnimExclamation = LoadAnimation("AIZ/SchrodingersCapsule.bin", d, 0, 3);
            DrawTexturePivotNormal(d, editorAnimExclamation, editorAnimExclamation.RequestedAnimID, editorAnimExclamation.RequestedFrameID, x, y, Transparency);
            var editorAnimButton = LoadAnimation("AIZ/SchrodingersCapsule.bin", d, 1, 0);
            DrawTexturePivotNormal(d, editorAnimButton, editorAnimButton.RequestedAnimID, editorAnimButton.RequestedFrameID, x, y, Transparency);

        }

        public override string GetObjectName()
        {
            return "SchrodingersCapsule";
        }
    }
}
