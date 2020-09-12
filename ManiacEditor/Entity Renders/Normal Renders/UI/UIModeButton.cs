using ManiacEditor.Classes.Scene;
using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class UIModeButton : EntityRenderer
    {
		public override void Draw(Structures.EntityRenderProp properties)
		{
			DevicePanel d = properties.Graphics;
			EditorEntity e = properties.EditorObject;
			int x = properties.DrawX;
			int y = properties.DrawY;
			int Transparency = properties.Transparency;
			bool fliph = false;
			bool flipv = false;
			string text = "UI/Text" + Methods.Solution.SolutionState.Main.CurrentManiaUILanguage + ".bin";

			int width = 120;
			int height = 20;

			int buttonID = (int)e.attributesMap["buttonID"].ValueEnum;
            bool disabled = e.attributesMap["disabled"].ValueBool;
			if (buttonID == 3)
			{
				buttonID = 4;
			}
			double alignmentVal = 0;

			if (!disabled)
            {
				alignmentVal = (22 / 2);

				if (buttonID == 2)
				{
					var editorAnim4 = LoadAnimation("UI/MainIcons.bin", d, 1, 3);
					DrawTexturePivotNormal(d, editorAnim4, editorAnim4.RequestedAnimID, editorAnim4.RequestedFrameID, x + (int)alignmentVal, y - 10, Transparency, fliph, flipv);
					var editorAnim3 = LoadAnimation("UI/MainIcons.bin", d, 0, 3);
					DrawTexturePivotNormal(d, editorAnim3, editorAnim3.RequestedAnimID, editorAnim3.RequestedFrameID, x + (int)alignmentVal, y - 10, Transparency, fliph, flipv);

				}

				var editorAnim2 = LoadAnimation("UI/MainIcons.bin", d, 1, buttonID);
				DrawTexturePivotNormal(d, editorAnim2, editorAnim2.RequestedAnimID, editorAnim2.RequestedFrameID, x + (int)alignmentVal, y - 10, Transparency, fliph, flipv);
				var editorAnim = LoadAnimation("UI/MainIcons.bin", d, 0, buttonID);
				DrawTexturePivotNormal(d, editorAnim, editorAnim.RequestedAnimID, editorAnim.RequestedFrameID, x + (int)alignmentVal, y - 10, Transparency, fliph, flipv);


				d.DrawQuad(x - (width / 2) - height, y - (height / 2), x + (width / 2) + height, y + (height / 2), System.Drawing.Color.FromArgb(Transparency, System.Drawing.Color.Black), System.Drawing.Color.FromArgb(Transparency, System.Drawing.Color.Black), 0);

				var editorAnim5 = LoadAnimation(text, d, 1, buttonID);
				DrawTexturePivotNormal(d, editorAnim5, editorAnim5.RequestedAnimID, editorAnim5.RequestedFrameID, x + (int)alignmentVal, y, Transparency, fliph, flipv);
			}
		}

        public override string GetObjectName()
        {
            return "UIModeButton";
        }
    }
}
