using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class UIButton : EntityRenderer
    {
        public override void Draw(Structures.EntityRenderProp Properties)
        {
            DevicePanel d = Properties.Graphics;

            Classes.Scene.EditorEntity e = Properties.EditorObject;
            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;

            string text = "Text" + Methods.Solution.SolutionState.Main.CurrentManiaUILanguage;
            int frameID = (int)e.attributesMap["frameID"].ValueEnum;
            int listID = (int)e.attributesMap["listID"].ValueEnum;
            int align = (int)e.attributesMap["align"].ValueEnum;
            int width = (int)e.attributesMap["size"].ValueVector2.X.High;
            int height = (int)e.attributesMap["size"].ValueVector2.Y.High;
            bool invisible = e.attributesMap["invisible"].ValueBool;
            double alignmentVal = 0;
            var editorAnim = Methods.Entities.EntityDrawing.LoadAnimation(d, text, listID, frameID);
            switch (align)
            {
                case 0:
                    alignmentVal = -((width / 2)) - editorAnim.RequestedFrame.PivotY;
                    break;
                default:
                    alignmentVal = editorAnim.RequestedFrame.PivotX + (22 / 2);
                    break;
            }
            d.DrawRectangle(x - width / 2, y - height / 2, x + width / 2, y + height / 2, System.Drawing.Color.FromArgb(Transparency, System.Drawing.Color.Black));
            DrawTexture(d, editorAnim, listID, frameID, x + (int)alignmentVal, y + editorAnim.RequestedFrame.PivotY, Transparency);


        }

        public override string GetObjectName()
        {
            return "UIButton";
        }
    }
}
