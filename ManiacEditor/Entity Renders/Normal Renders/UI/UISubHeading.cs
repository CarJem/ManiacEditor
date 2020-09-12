using ManiacEditor.Classes.Scene;
using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class UISubHeading : EntityRenderer
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
            int listID = (int)entity.attributesMap["listID"].ValueEnum;
            int frameID = (int)entity.attributesMap["frameID"].ValueEnum;
			int width = (int)entity.attributesMap["size"].ValueVector2.X.High;
			int height = (int)entity.attributesMap["size"].ValueVector2.Y.High;
			int align = (int)entity.attributesMap["align"].ValueEnum;
			double alignmentVal = 0;
            d.DrawQuad(x - (width / 2) - height, y - (height / 2), x + (width / 2) + height, y + (height / 2), System.Drawing.Color.FromArgb(Transparency, System.Drawing.Color.Black), System.Drawing.Color.FromArgb(Transparency, System.Drawing.Color.Black), 0);
            var Animation = LoadAnimation(text, d, listID, frameID);
            DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, Transparency, fliph, flipv);
        }

        public override string GetObjectName()
        {
            return "UISubHeading";
        }
    }
}
