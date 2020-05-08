using RSDKv5;
using SystemColors = System.Drawing.Color;

namespace ManiacEditor.Entity_Renders
{
    public class Water : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp Properties)
        {
            DevicePanel d = Properties.Graphics;

            Classes.Scene.EditorEntity e = Properties.EditorObject;
            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int Transparency = Properties.Transparency;

            int type = (int)e.attributesMap["type"].ValueEnum;
            var widthPixels = (int)(e.attributesMap["size"].ValueVector2.X.High);
            var heightPixels = (int)(e.attributesMap["size"].ValueVector2.Y.High);
            var heightY = (int)(e.attributesMap["height"].ValueVector2.Y.High);
            var heightX = (int)(e.attributesMap["height"].ValueVector2.X.High);
			int r = (int)(e.attributesMap["r"].ValueUInt8);
			int g = (int)(e.attributesMap["g"].ValueUInt8);
			int b = (int)(e.attributesMap["b"].ValueUInt8);
			var width = (int)widthPixels / 16;
            var height = (int)heightPixels / 16;
            bool fliph = false;
            bool flipv = false;
            bool showBounds = false;
            bool HCZBubbles = false;
            int animID = 0;
            switch (type)
            {
                case 0:
                    showBounds = false;
                    break;
                case 1:
                    showBounds = true;
                    break;
                case 2:
                    showBounds = false;
                    animID = 2;
                    break;
                case 3:
                    showBounds = true;
                    break;
                case 4:
                    showBounds = true;
                    break;
                case 5:
                    showBounds = false;
                    HCZBubbles = true;
                    break;
            }

            var editorAnim = LoadAnimation("Water", d, animID, 0);

            // Base Water + Bubble Source
            if (animID >= 0 && (type == 2 || type == 0))
            {
                DrawTexturePivotNormal(d, editorAnim, editorAnim.RequestedAnimID, editorAnim.RequestedFrameID, x, y, Transparency);
            }


            // HCZ Big Bubbles
            if (type == 5)
            {
                editorAnim = LoadAnimation("BigBubble", d, 7, 0);
                DrawTexturePivotNormal(d, editorAnim, editorAnim.RequestedAnimID, editorAnim.RequestedFrameID, x, y, Transparency);
            }



            if (type == 3)
            {
                editorAnim = LoadAnimation("Water", d, 0, 0);
                DrawTexturePivotNormal(d, editorAnim, editorAnim.RequestedAnimID, editorAnim.RequestedFrameID, x, y, Transparency);
            }

            // Bounded Water
            if (width != 0 && height != 0 && showBounds == true && HCZBubbles == false)
            {
                //Draw Icon
                editorAnim = LoadAnimation("Water", d, 0, 0);
                DrawTexturePivotNormal(d, editorAnim, editorAnim.RequestedAnimID, editorAnim.RequestedFrameID, x, y, Transparency);

                int x1 = x + widthPixels / -2;
                int x2 = x + widthPixels / 2 - 1;
                int y1 = y + heightPixels / -2;
                int y2 = y + heightPixels / 2 - 1;

				if (type != 1)
				{
					if (Methods.Solution.SolutionState.Main.ShowWaterLevel)
					{
                        if (!ManiacEditor.Properties.Settings.MyPerformance.UseSimplifedWaterRendering)
                        {
                            if (Methods.Solution.SolutionState.Main.AlwaysShowWaterLevel)
                            {
                                int startX = (Methods.Solution.SolutionState.Main.SizeWaterLevelwithBounds ? x1 : 0);
                                int endX = (Methods.Solution.SolutionState.Main.SizeWaterLevelwithBounds ? x2 : Methods.Solution.CurrentSolution.SceneWidth);

                                d.DrawRectangle(startX, heightX, endX, Methods.Solution.CurrentSolution.SceneHeight, Methods.Solution.SolutionState.Main.waterColor);
                                d.DrawLine(startX, heightX, endX, heightX, SystemColors.White);
                                for (int i = startX; i < endX; i = i + 32)
                                {
                                    DrawTexturePivotNormal(d, editorAnim, editorAnim.RequestedAnimID, editorAnim.RequestedFrameID, i, heightX, Transparency);
                                }
                            }

                        }
                        else
                        {
                            if (Methods.Solution.SolutionState.Main.AlwaysShowWaterLevel)
                            {
                                int startX = (Methods.Solution.SolutionState.Main.SizeWaterLevelwithBounds ? x1 : 0);
                                int endX = (Methods.Solution.SolutionState.Main.SizeWaterLevelwithBounds ? x2 : Methods.Solution.CurrentSolution.SceneWidth);
                                d.DrawRectangle(startX, heightX, endX, Methods.Solution.CurrentSolution.SceneHeight, Methods.Solution.SolutionState.Main.waterColor);
                                d.DrawLine(startX, heightX, endX, heightX, SystemColors.White);
                            }
                        }
                    }
				}
				else
				{
					int red = r - 150;
					int blue = b - 150;
					int green = g - 150;
					if (red > 255) red = 255;
					if (blue > 255) blue = 255;
					if (green > 255) green = 255;
					if (red < 0) red = 0;
					if (blue < 0) blue = 0;
					if (green < 0) green = 0;
					d.DrawRectangle(x1, y1, x2, y2, SystemColors.FromArgb(128, red, green, blue));
				}

                d.DrawLine(x1, y1, x1, y2, SystemColors.Aqua);
                d.DrawLine(x1, y1, x2, y1, SystemColors.Aqua);
                d.DrawLine(x2, y2, x1, y2, SystemColors.Aqua);
                d.DrawLine(x2, y2, x2, y1, SystemColors.Aqua);
            }
        }

        public override bool isObjectOnScreen(DevicePanel d, Classes.Scene.EditorEntity e, int x, int y, int Transparency)
        {
            var widthPixels = (int)(e.attributesMap["size"].ValueVector2.X.High);
            var heightPixels = (int)(e.attributesMap["size"].ValueVector2.Y.High);
            return d.IsObjectOnScreen(x - widthPixels / 2, y - heightPixels / 2, widthPixels, heightPixels);
        }

        public override string GetObjectName()
        {
            return "Water";
        }
    }
}
