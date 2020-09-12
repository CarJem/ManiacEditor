using ManiacEditor.Classes.Scene;
using RSDKv5;

namespace ManiacEditor.Entity_Renders
{
    public class UIMedallionPanel : EntityRenderer
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
			var Animation = LoadAnimation("UI/MedallionPanel.bin", d, 0, 2);
			x -= 38;
			y -= 16;

			for (int mx = 0; mx < 8; mx++)
			{
				for (int my = 0; my < 4; my++)
				{
					DrawTexturePivotNormal(d, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x + (14 * mx), y + (16 * my), Transparency, fliph, flipv);
				}
			}
		}

		public override string GetObjectName()
        {
            return "UIMedallionPanel";
        }
    }
}
