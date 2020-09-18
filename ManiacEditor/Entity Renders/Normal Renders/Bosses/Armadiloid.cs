using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class Armadiloid : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp properties)
        {
            DevicePanel d = properties.Graphics;
            Classes.Scene.EditorEntity entity = properties.EditorObject; 
            int x = properties.DrawX;
            int y = properties.DrawY;
            int Transparency = properties.Transparency;
            int type = (int)entity.attributesMap["type"].ValueEnum;
            bool fliph = false;
            bool flipv = false;



            if (type == 0)
            {
                var editorAnim = LoadAnimation(GetSetupAnimation(), d, 0, 0);
                DrawTexturePivotNormal(d, editorAnim, editorAnim.RequestedAnimID, editorAnim.RequestedFrameID, x, y, Transparency);
                var editorAnimHead = LoadAnimation(GetSetupAnimation(), d, 1, 0);
                DrawTexturePivotNormal(d, editorAnimHead, editorAnimHead.RequestedAnimID, editorAnimHead.RequestedFrameID, x, y, Transparency);
                var editorAnimBoost = LoadAnimation(GetSetupAnimation(), d, 3, 0);
                DrawTexturePivotNormal(d, editorAnimBoost, editorAnimBoost.RequestedAnimID, editorAnimBoost.RequestedFrameID, x, y, Transparency);
            }
            else if (type == 1)
            {
                var editorAnimRider = LoadAnimation(GetSetupAnimation(), d, 4, 0);
                DrawTexturePivotNormal(d, editorAnimRider, editorAnimRider.RequestedAnimID, editorAnimRider.RequestedFrameID, x, y, Transparency);
            }
        }

        public override string GetSetupAnimation()
        {
            return GetSpriteAnimationPath("/Armadiloid.bin", "Armadiloid", new string[] { "MSZ" }, "MSZ");
        }

        public override string GetObjectName()
        {
            return "Armadiloid";
        }
    }
}
