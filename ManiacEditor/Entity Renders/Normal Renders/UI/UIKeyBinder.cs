using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class UIKeyBinder : EntityRenderer
    {
        public override void Draw(Structures.EntityRenderProp properties)
        {
            DevicePanel d = properties.Graphics;
            Classes.Scene.EditorEntity entity = properties.EditorObject;
            int x = properties.DrawX;
            int y = properties.DrawY;
            int Transparency = properties.Transparency;
            string text = "UI/Text" + Methods.Solution.SolutionState.Main.CurrentManiaUILanguage + ".bin";
            int type = (int)entity.attributesMap["type"].ValueUInt8;
            int inputID = (int)entity.attributesMap["inputID"].ValueUInt8;
            int width = 48;
            int height = 12;
            int frameID = 1;
            int listID = 0;
            switch (type)
            {
                case 0:
                    frameID = 7;
                    break;
                case 1:
                    frameID = 8;
                    break;
                case 2:
                    frameID = 9;
                    break;
                case 3:
                    frameID = 10;
                    break;
                case 4:
                    frameID = 13;
                    break;
                case 5:
                    frameID = 1;
                    break;
                case 6:
                    frameID = 3;
                    break;
                case 7:
                    frameID = 11;
                    break;
                case 8:
                    frameID = 12;
                    break;


            }
            d.DrawQuad(x - (width / 2) - height, y - (height / 2), x + (width / 2) + height, y + (height / 2), System.Drawing.Color.FromArgb(Transparency, System.Drawing.Color.Black), System.Drawing.Color.FromArgb(Transparency, System.Drawing.Color.Black), 0);

            var editorAnim = LoadAnimation(text, d, listID, frameID);
            DrawTexture(d, editorAnim, editorAnim.RequestedAnimID, editorAnim.RequestedFrameID, x , y + editorAnim.RequestedFrame.PivotY, Transparency, false, false);
            var editorAnimKey = LoadAnimation("UI/Buttons.bin", d, 1, 0);
            DrawTexture(d, editorAnimKey, editorAnimKey.RequestedAnimID, editorAnimKey.RequestedFrameID, x + editorAnimKey.RequestedFrame.PivotX - 16, y + editorAnimKey.RequestedFrame.PivotY, Transparency, false, false);
        }

        public override string GetObjectName()
        {
            return "UIKeyBinder";
        }
    }
}
