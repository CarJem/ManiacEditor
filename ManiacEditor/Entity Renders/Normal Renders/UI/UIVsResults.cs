using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class UIVsResults : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp properties)
        {
            DevicePanel d = properties.Graphics;
            Classes.Scene.EditorEntity e = properties.EditorObject;
            int x = e.PositionX;
            int y = e.PositionY;
            int Transparency = properties.Transparency;
			string text = "UI/Text" + Methods.Solution.SolutionState.Main.CurrentManiaUILanguage + ".bin";
			int playerID = (int)e.attributesMap["playerID"].ValueUInt8;
			int player = 8;
			switch (playerID)
			{
				case 0:
					player = 8;
					break;
				case 1:
					player = 9;
					break;
				case 2:
					player = 10;
					break;
				case 3:
					player = 11;
					break;

			}

			var editorAnim = Methods.Drawing.ObjectDrawing.LoadAnimation(d, "EditorUIRender", 5, 0);
			DrawTexture(d, editorAnim, 5, 0, x + editorAnim.RequestedFrame.PivotX, y + editorAnim.RequestedFrame.PivotY + 40, Transparency);

			editorAnim = Methods.Drawing.ObjectDrawing.LoadAnimation(d, text, 12, player);
			DrawTexture(d, editorAnim, 12, player, x + editorAnim.RequestedFrame.PivotX + 36, y + editorAnim.RequestedFrame.PivotY - 26, Transparency);
		}

		public override string GetObjectName()
        {
            return "UIVsResults";
        }
    }
}
