using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class UISaveSlot : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp Properties)
        {
            DevicePanel d = Properties.Graphics;

            Classes.Scene.EditorEntity e = Properties.EditorObject;
            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;

            //int frameID = (int)entity.attributesMap["listID"].ValueEnum;
            int type = (int)e.attributesMap["type"].ValueEnum;
            string text = "Text" + Methods.Solution.SolutionState.CurrentManiaUILanguage;







            if (type == 1)
            {
                var editorAnim = LoadAnimation("SaveSelect", d, 0, 0);
                DrawTexturePivotNormal(d, editorAnim, 0, 0, x, y, Transparency);
                var editorAnimBorder = LoadAnimation("SaveSelect", d, 0, 1);
                DrawTexturePivotNormal(d, editorAnimBorder, 0, 1, x, y, Transparency);
                var editorAnimBackground = LoadAnimation("SaveSelect", d, 0, 2);
                DrawTexturePivotNormal(d, editorAnimBackground, 0, 2, x, y, Transparency);
                var editorAnimText = LoadAnimation(text, d, 2, 0);
                DrawTexturePivotNormal(d, editorAnimText, 2, 0, x, y, Transparency);
                var editorAnimNoSave = LoadAnimation(text, d, 2, 2);
                DrawTexturePivotNormal(d, editorAnimNoSave, 2, 2, x, y, Transparency);
            }
            else
            {
                var editorAnimActualRender = LoadAnimation("EditorUIRender", d, 3, 0);
                DrawTexturePivotNormal(d, editorAnimActualRender, 3, 0, x, y, Transparency);

                /*
                var editorAnimBackground = LoadAnimation("SaveSelect", d, 0, 2);
                DrawTexturePivotNormal(d, editorAnimBackground, 0, 2, x, y - (editorAnimBackground.RequestedFrame.PivotY / 2) - 6, Transparency);
                var editorAnimText = LoadAnimation(text, d, 2, 0);
                DrawTexturePivotNormal(d, editorAnimText, 2, 0, x, y - (editorAnimText.RequestedFrame.PivotY / 2) - 6, Transparency);\
                */

                var editorAnimActualRenderBorder = LoadAnimation("EditorUIRender", d, 3, 1);
                DrawTexturePivotNormal(d, editorAnimActualRender, 3, 1, x, y, Transparency);
            }



        }

        public override string GetObjectName()
        {
            return "UISaveSlot";
        }
    }
}
