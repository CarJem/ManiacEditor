using ManiacEditor.Classes.Scene;
using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class UIButtonPrompt : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp properties)
        {
            DevicePanel d = properties.Graphics;
            EditorEntity entity = properties.EditorObject;
            int x = properties.DrawX;
            int y = properties.DrawY;
            int Transparency = properties.Transparency;
            bool fliph = false;
            bool flipv = false;

            string text = "UI/Text" + Methods.Solution.SolutionState.Main.CurrentManiaUILanguage + ".bin";
            int promptID = (int)entity.attributesMap["promptID"].ValueEnum;
            int buttonID = (int)entity.attributesMap["buttonID"].ValueEnum;

            var Animation = LoadAnimation("UI/Buttons.bin", d, Methods.Solution.SolutionState.Main.CurrentManiaUIControllerType, buttonID);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
            Animation = LoadAnimation("UI/Buttons.bin", d, 0, 0);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
            Animation = LoadAnimation(text, d, 0, promptID);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
        }

        public override string GetObjectName()
        {
            return "UIButtonPrompt";
        }
    }
}
