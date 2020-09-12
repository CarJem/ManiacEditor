using ManiacEditor.Classes.Scene;
using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class UICharButton : EntityRenderer
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
            string text = "UI/Text" + Methods.Solution.SolutionState.Main.CurrentManiaUILanguage + ".bin";

            int characterID = (int)e.attributesMap["characterID"].ValueUInt8;
            int characterID_text = characterID;
            if (characterID >= 3) characterID++;

            d.DrawRectangle(x - 48, y - 48, x + 48, y + 48, System.Drawing.Color.FromArgb(128, 255, 255, 255));

            var Animation = LoadAnimation("EditorUIRender", d, 1, 1);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);

            Animation = LoadAnimation(text, d, 8, characterID_text);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y + 32, Transparency, fliph, flipv);

            Animation = LoadAnimation("UI/SaveSelect.bin", d, 1, characterID);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y - 8, Transparency, fliph, flipv);
        }

        public override string GetObjectName()
        {
            return "UICharButton";
        }
    }
}
