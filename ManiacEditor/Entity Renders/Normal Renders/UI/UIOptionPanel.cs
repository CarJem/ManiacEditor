using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class UIOptionPanel : EntityRenderer
    {
        public override void Draw(Structures.EntityRenderProp properties)
        {
            DevicePanel d = properties.Graphics;
            Classes.Scene.EditorEntity entity = properties.EditorObject;
            int x = properties.DrawX;
            int y = properties.DrawY;
            int Transparency = properties.Transparency;
            string text = "UI/Text" + Methods.Solution.SolutionState.Main.CurrentManiaUILanguage + ".bin";
            int topListID = (int)entity.attributesMap["topListID"].ValueEnum;
            int topFrameID = (int)entity.attributesMap["topFrameID"].ValueEnum;
            int botListID = (int)entity.attributesMap["botListID"].ValueEnum;
            int botFrameID = (int)entity.attributesMap["botFrameID"].ValueEnum;
            int panelSize = (int)entity.attributesMap["panelSize"].ValueEnum;
            bool botAlignRight = entity.attributesMap["botAlignRight"].ValueBool;
            bool botHidden = entity.attributesMap["botHidden"].ValueBool;

            panelSize /= 2;

            int top = x - 212;
            int left = y - panelSize;
            int bottom = x + 212;
            int right = y + panelSize;

            d.DrawRectangle(top, left, bottom, right, System.Drawing.Color.FromArgb(Transparency, 49, 162, 247));

            d.DrawRectangle(top, left, x - 6, left+24, System.Drawing.Color.FromArgb(Transparency, 0, 0, 0));
            //e.DrawTriangle(d, x - 6, left + 12, 10, 24, 10, 10, 1, Transparency);

            if (!botHidden)
            {
                d.DrawRectangle(bottom, right, x - 6, right - 24, System.Drawing.Color.FromArgb(Transparency, 0, 0, 0));
                //e.DrawTriangle(d, x - 6, right - 12, 24, 24, 10, 10, 0, Transparency);
            }

            bool fliph = false;
            bool flipv = false;


            var editorAnimTop = LoadAnimation(text, d, topListID, topFrameID);
            int topX = top + 68;
            int topY = left + 12;

            DrawTexturePivotNormal(d, editorAnimTop, editorAnimTop.RequestedAnimID, editorAnimTop.RequestedFrameID, topX, topY, Transparency, fliph, flipv);

            if (!botHidden)
            {
                var editorAnimBot = LoadAnimation(text, d, botListID, botFrameID);
                int botX = x + (botAlignRight ? editorAnimBot.RequestedFrame.Width - 6 : 0);
                int botY = right - 12;

                DrawTexturePivotNormal(d, editorAnimBot, editorAnimBot.RequestedAnimID, editorAnimBot.RequestedFrameID, botX, botY, Transparency, fliph, flipv);
            }
        }

        public override string GetObjectName()
        {
            return "UIOptionPanel";
        }
    }
}
