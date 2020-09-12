using ManiacEditor.Classes.Scene;
using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class UIHeading : EntityRenderer
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
            string text = "UI/Headings" + Methods.Solution.SolutionState.Main.CurrentManiaUILanguage + ".bin";
            int listID = (int)e.attributesMap["headingID"].ValueEnum;

            var Animation = LoadAnimation("UI/UIElements.bin", d, 0, 0);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
            Animation = LoadAnimation(text, d, listID, 0);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);

        }

        public override string GetObjectName()
        {
            return "UIHeading";
        }
    }
}
