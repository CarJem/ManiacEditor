using ManiacEditor.Classes.Scene;
using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class UITAZoneModule : EntityRenderer
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
            var editorAnimFrame = LoadAnimation("EditorUIRender", d, 0, 0);
            DrawTexturePivotNormal(d, editorAnimFrame, editorAnimFrame.RequestedAnimID, editorAnimFrame.RequestedFrameID, x, y, Transparency, fliph, flipv);
            var editorAnimBackground = LoadAnimation("UI/SaveSelect.bin", d, 10, 0);
            DrawTexturePivotNormal(d, editorAnimBackground, editorAnimBackground.RequestedAnimID, editorAnimBackground.RequestedFrameID, x - 107, y, Transparency, fliph, flipv);

            string text1 = entity.attributesMap["text1"].ValueString;
            string text2 = entity.attributesMap["text2"].ValueString;

            int listID = 3;
            int text_X = x - 46;
            int text_YAdjust = (text2 != "" ? 14 : 0);
            int text_Y = y - 8;

            int spacingAmount = 0;
            foreach (char symb in text1)
            {
                var editorAnim2 = GetFrameID(d, symb, listID);
                DrawTexture(d, editorAnim2, editorAnim2.RequestedAnimID, editorAnim2.RequestedFrameID, text_X + spacingAmount, text_Y + editorAnim2.RequestedFrame.PivotY - text_YAdjust, Transparency);
                spacingAmount = spacingAmount + editorAnim2.RequestedFrame.Width;
            }
            spacingAmount = 0;
            foreach (char symb in text2)
            {
                var editorAnim2 = GetFrameID(d, symb, listID);
                DrawTexture(d, editorAnim2, editorAnim2.RequestedAnimID, editorAnim2.RequestedFrameID, text_X + 32 + spacingAmount, text_Y + editorAnim2.RequestedFrame.PivotY + 28 - text_YAdjust, Transparency);
                spacingAmount = spacingAmount + editorAnim2.RequestedFrame.Width;
            }


        }

        public Methods.Drawing.ObjectDrawing.EditorAnimation GetFrameID(DevicePanel d, char letter, int listID)
        {
            var editorAnim = LoadAnimation("UI/UIElements.bin", d, listID, 0);
            for (int i = 0; i < editorAnim.RequestedAnimation.Frames.Count; i++)
            {
                editorAnim = LoadAnimation("UI/UIElements.bin", d, listID, i);
                if ((double)editorAnim.RequestedFrame.ID == (double)letter) return editorAnim;
            }

            return editorAnim;

        }

        public override string GetObjectName()
        {
            return "UITAZoneModule";
        }
    }
}
