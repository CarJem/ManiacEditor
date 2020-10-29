using ManiacEditor.Classes.Scene;
using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class UIVsCharSelector : EntityRenderer
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
            int playerID = (int)entity.attributesMap["playerID"].ValueUInt8;
            int player = 8;
            switch (playerID)
            {
                case 0:
                    player = 8;
                    break;
                case 1:
                    player = 9;
                    break;
                case 2:
                    player = 10;
                    break;
                case 3:
                    player = 11;
                    break;

            }
            
            d.DrawRectangle(x - 48, y - 48, x + 48, y + 48, System.Drawing.Color.FromArgb(Transparency, 255, 255, 255));


            var editorAnimFrame = LoadAnimation("EditorUIRender", d, 1, 0);
            DrawTexturePivotNormal(d, editorAnimFrame, editorAnimFrame.RequestedAnimID, editorAnimFrame.RequestedFrameID, x, y, Transparency, fliph, flipv);
            var editorAnimBackground = LoadAnimation("UI/SaveSelect.bin", d, 14, 7);
            DrawTexturePivotNormal(d, editorAnimBackground, editorAnimBackground.RequestedAnimID, editorAnimBackground.RequestedFrameID, x, y - 8, Transparency, fliph, flipv);
            var editorAnimWaiting = LoadAnimation(text, d, 12, 7);
            DrawTexturePivotNormal(d, editorAnimWaiting, editorAnimWaiting.RequestedAnimID, editorAnimWaiting.RequestedFrameID, x, y - 8, Transparency, fliph, flipv);
            var editorAnimPlayerText = LoadAnimation(text, d, 12, player);
            DrawTexturePivotNormal(d, editorAnimPlayerText, editorAnimPlayerText.RequestedAnimID, editorAnimPlayerText.RequestedFrameID, x + 36, y - 38, Transparency, fliph, flipv);

        }

        public override string GetObjectName()
        {
            return "UIVsCharSelector";
        }
    }
}
