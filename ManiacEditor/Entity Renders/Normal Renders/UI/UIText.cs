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
                int animID = (highlighted ? 1 : 0);
                var Animation = GetFrameID(d, symb, animID);
                if (Animation != null)
                {
                    DrawTexturePivotPlus(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, spacingAmount, 0, Transparency, fliph, flipv);
                    spacingAmount = spacingAmount + Animation.RequestedFrame.Width;
                }

            }

            
        }

        public Methods.Drawing.ObjectDrawing.EditorAnimation GetFrameID(DevicePanel d, char letter, int animID)
        {
            var editorAnim = LoadAnimation("LSelect/Text.bin", d, animID, 0);
            for (int i = 0; i < editorAnim.RequestedAnimation.Frames.Count; i++)
            {
                editorAnim = LoadAnimation("LSelect/Text.bin", d, animID, i);
                if ((double)editorAnim.RequestedFrame.ID == (double)letter) return editorAnim;
            }
            return null;
        }

        public override string GetObjectName()
        {
            return "UIText";
        }
    }
}
