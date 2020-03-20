using System;
using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class UIText : EntityRenderer
    {
        private static string HUDLevelSelectCharS = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ*+,-./: \'\"_^]\\[)(";
        private static char[] HUDLevelSelectChar;

        public override void Draw(Structures.EntityRenderProp Properties)
        {
            if (HUDLevelSelectChar == null) HUDLevelSelectChar = HUDLevelSelectCharS.ToCharArray();

            DevicePanel d = Properties.Graphics;
            
            Classes.Scene.EditorEntity e = Properties.EditorObject;
            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;

            bool fliph = false;
            bool flipv = false;

            string text = e.attributesMap["text"].ValueString;
            bool selectable = e.attributesMap["selectable"].ValueBool;
            bool highlighted = e.attributesMap["highlighted"].ValueBool;
            int spacingAmount = 0;
            foreach(char symb in text)
            {
                int frameID = GetFrameID(symb, Methods.Editor.SolutionState.LevelSelectChar);
                int animID = (highlighted ? 1 : 0);

                var Animation = LoadAnimation("Text", d, animID, frameID);
                DrawTexturePivotPlus(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, spacingAmount, 0, Transparency, fliph, flipv);
                spacingAmount = spacingAmount + Animation.RequestedFrame.Width;
            }

            
        }

        public int GetFrameID(char letter, char[] arry)
        {
            char[] symArray = arry;
            int position = 0;
            foreach (char sym in symArray)
            {
                if (sym.ToString().Equals(letter.ToString(), StringComparison.InvariantCultureIgnoreCase))
                {
                    //MessageBox.Show(String.Format("Sym: {0} Letter: {1} Pos: {2}", sym, letter, position));
                    return position;
                }
                position++;
            }
            return position;
        }

        public override string GetObjectName()
        {
            return "UIText";
        }
    }
}
