using RSDKv5;
using SystemColors = System.Drawing.Color;

namespace ManiacEditor.Entity_Renders
{
    public class OutroIntroObject : EntityRenderer
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
            var editorAnim = LoadAnimation("EditorIcons2", d, 0, 3);
            DrawTexturePivotNormal(d, editorAnim, editorAnim.RequestedAnimID, editorAnim.RequestedFrameID, x, y, Transparency, fliph, flipv);


            if (e.Object.Name.Name == "LRZ1Intro")
            {
                editorAnim = LoadAnimation("LRZ1/IntroSub.bin", d, 0, 0);
                DrawTexturePivotNormal(d, editorAnim, editorAnim.RequestedAnimID, editorAnim.RequestedFrameID, x, y, Transparency, fliph, flipv);
            }


            int widthPixels = Methods.Entities.AttributeHandler.AttributesMapPositionHighX("size", e) * 2;
            var heightPixels = Methods.Entities.AttributeHandler.AttributesMapPositionHighY("size", e) * 2;
            var width = (int)widthPixels / 16;
            var height = (int)heightPixels / 16;

            DrawBounds(d, x, y, widthPixels, heightPixels, Transparency, SystemColors.White, SystemColors.Transparent);
        }

        public override string GetObjectName()
        {
            return "Outro_Intro_Object";
        }
    }
}
