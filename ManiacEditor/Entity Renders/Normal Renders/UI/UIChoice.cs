using ManiacEditor.Classes.Scene;
using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class UIChoice : EntityRenderer
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

            int arrowWidth = (int)entity.attributesMap["arrowWidth"].ValueEnum;
            if (arrowWidth != 0) arrowWidth /= 2;
            int frameID = (int)entity.attributesMap["frameID"].ValueEnum;
            int listID = (int)entity.attributesMap["listID"].ValueEnum;
            bool auxIcon = entity.attributesMap["auxIcon"].ValueBool;
            bool noText = entity.attributesMap["noText"].ValueBool;
            int auxframeID = (int)entity.attributesMap["auxFrameID"].ValueEnum;
            int auxlistID = (int)entity.attributesMap["auxListID"].ValueEnum;
            int width = (int)entity.attributesMap["size"].ValueVector2.X.High;
            int height = (int)entity.attributesMap["size"].ValueVector2.Y.High;
            double alignmentVal = 0;
            int align = (int)entity.attributesMap["align"].ValueEnum;
            switch (align)
            {
                case 0:
                    alignmentVal = -(width / 2) + 16;
                    break;
                case 1:
                    alignmentVal = (22 / 2);
                    break;
            }

            d.DrawQuad(x - (width / 2) - height, y - (height / 2), x + (width / 2) + height, y + (height / 2), System.Drawing.Color.FromArgb(Transparency, System.Drawing.Color.Black), System.Drawing.Color.FromArgb(Transparency, System.Drawing.Color.Black), 0);

            if (noText == false)
            {
                var editorAnim = LoadAnimation(text, d, listID, frameID);
                DrawTexturePivotNormal(d, editorAnim, editorAnim.RequestedAnimID, editorAnim.RequestedFrameID, x + (int)alignmentVal, y, Transparency, fliph, flipv);
            }
            var leftArrow = LoadAnimation("UI/UIElements.bin", d, 2, 0);
            DrawTexturePivotNormal(d, leftArrow, leftArrow.RequestedAnimID, leftArrow.RequestedFrameID, x - arrowWidth + (int)alignmentVal, y, Transparency, fliph, flipv);
            var rightArrow = LoadAnimation("UI/UIElements.bin", d, 2, 1);
            DrawTexturePivotNormal(d, rightArrow, rightArrow.RequestedAnimID, rightArrow.RequestedFrameID, x + arrowWidth + (int)alignmentVal, y, Transparency, fliph, flipv);
            if (auxIcon)
            {
                var editorAnimIcon = LoadAnimation("UI/SaveSelect.bin", d, auxlistID, auxframeID);
                DrawTexturePivotNormal(d, editorAnimIcon, editorAnimIcon.RequestedAnimID, editorAnimIcon.RequestedFrameID, x + (int)alignmentVal, y, Transparency, fliph, flipv);
            }
        }

        public override string GetObjectName()
        {
            return "UIChoice";
        }
    }
}
