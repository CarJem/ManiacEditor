using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class UIInfoLabel : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp Properties)
        {
            DevicePanel d = Properties.Graphics;
            Classes.Scene.EditorEntity e = Properties.EditorObject;

            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;

            string text = e.attributesMap["text"].ValueString;
            int width = (int)e.attributesMap["size"].ValueVector2.X.High;
            int height = (int)e.attributesMap["size"].ValueVector2.Y.High;
            int spacingAmount = 0;
            //e.DrawUIButtonBack(d, x, y, width, height, width, height, Transparency);
            if (width == 0) width = 1;
            int x2 = x - (width / 4);
            foreach (char symb in text)
            {
                int frameID = GetFrameID(symb, Methods.Solution.SolutionState.Main.MenuChar_Small);
                var editorAnim = LoadAnimation("UIElements", d, 4, frameID);
                DrawTexture(d, editorAnim, editorAnim.RequestedAnimID, editorAnim.RequestedFrameID, x2 + spacingAmount, y, Transparency);
                spacingAmount = spacingAmount + editorAnim.RequestedFrame.Width;
            }

            
        }

        public int GetFrameID(char letter, char[] arry)
        {
            char[] symArray = arry;
            int position = 0;
            foreach (char sym in symArray)
            {
                if (sym == letter) return position;
                position++;
            }
            return position;
        }

        public override string GetObjectName()
        {
            return "UIInfoLabel";
        }
    }
}
