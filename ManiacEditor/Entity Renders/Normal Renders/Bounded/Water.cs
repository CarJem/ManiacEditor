using RSDKv5;
using SystemColors = System.Drawing.Color;
using ManiacEditor.Methods.Drawing;
using ManiacEditor.Classes.Scene;
using System.Collections.Generic;
using ManiacEditor.Methods.Solution;

namespace ManiacEditor.Entity_Renders
{
    public class Water : EntityRenderer
    {
        public static bool TilesNeedUpdate { get; set; } = false;
        private static List<TileObject> WaterLayerRenderer = new List<TileObject>();
        private void DrawWaterTileMap(DevicePanel d, int x, int y, int platform_x, int platform_y, int platform_width, int platform_height, SystemColors colors, int Transparency)
        {
            if (CurrentSolution.CurrentScene?.AllLayers != null)
            {
                if (WaterLayerRenderer.Count != CurrentSolution.CurrentScene?.AllLayers.Count)
                {
                    WaterLayerRenderer.Clear();
                    for (int i = 0; i < CurrentSolution.CurrentScene.AllLayers.Count; i++)
                    {
                        WaterLayerRenderer.Add(new TileObject(i));
                    }
                }

                foreach (var layer in WaterLayerRenderer)
                {
                    int pivotX = (platform_width != 0 ? platform_width / 2 : 0);
                    int pivotY = (platform_height != 0 ? platform_height / 2 : 0);

                    layer.DrawTileMap(d, x - pivotX, y - pivotY, platform_x, platform_y, platform_width, platform_height, colors, Transparency, TilesNeedUpdate);
                }
            }
        }
        private void DrawGHZWater(DevicePanel d, ObjectDrawing.EditorAnimation editorAnim, int x, int y, int Transparency, int animID, int type, int widthPixels, int heightPixels, int heightX, int r, int g, int b, bool Selected)
        {
            //Draw Icon
            editorAnim = LoadAnimation("EditorIcons2", d, 0, 8);
            DrawTexturePivotNormal(d, editorAnim, editorAnim.RequestedAnimID, editorAnim.RequestedFrameID, x, y, Transparency);

            if (widthPixels != 0 && heightPixels != 0)
            {
                int x1 = x + widthPixels / -2;
                int x2 = x + widthPixels / 2 - 1;
                int y1 = y + heightPixels / -2;
                int y2 = y + heightPixels / 2 - 1;

                if (Selected)
                {
                    //DrawWaterTileMap(d, x, y, x1, y1, widthPixels, heightPixels, GetWaterColors(255, r, g, b), Transparency);
                    d.DrawRectangle(x1, y1, x2, y2, GetWaterColors(255, r, g, b));
                    if (TilesNeedUpdate) TilesNeedUpdate = false;
                }


                DrawBounds2(d, x2, y2, x1, y1, Transparency, SystemColors.Aqua, SystemColors.Transparent);
            }

            SystemColors GetWaterColors(int _a, int _r, int _g, int _b)
            {

                int red = _r;
                int green = _g;
                int blue = _b;
                if (red > 255) red = 255;
                if (blue > 255) blue = 255;
                if (green > 255) green = 255;
                if (red < 0) red = 0;
                if (blue < 0) blue = 0;
                if (green < 0) green = 0;

                return SystemColors.FromArgb(_a, red, green, blue);
            }

        }
        private void DrawWaterBounds(DevicePanel d, ObjectDrawing.EditorAnimation editorAnim, int x, int y, int Transparency, int animID, int type, int widthPixels, int heightPixels, int heightX, int r, int g, int b, bool Selected)
        {
            //Draw Icon
            editorAnim = LoadAnimation("EditorIcons2", d, 0, 8);
            DrawTexturePivotNormal(d, editorAnim, editorAnim.RequestedAnimID, editorAnim.RequestedFrameID, x, y, Transparency);


            if (widthPixels != 0 && heightPixels != 0 && Selected)
            {
                int x1 = x + widthPixels / -2;
                int x2 = x + widthPixels / 2 - 1;
                int y1 = y + heightPixels / -2;
                int y2 = y + heightPixels / 2 - 1;

                if (Methods.Solution.SolutionState.Main.ShowWaterLevel)
                {
                    if (!ManiacEditor.Properties.Settings.MyPerformance.UseSimplifedWaterRendering)
                    {
                        if (Methods.Solution.SolutionState.Main.AlwaysShowWaterLevel)
                        {
                            int startX = (Methods.Solution.SolutionState.Main.SizeWaterLevelwithBounds ? x1 : 0);
                            int endX = (Methods.Solution.SolutionState.Main.SizeWaterLevelwithBounds ? x2 : Methods.Solution.CurrentSolution.SceneWidth);

                            editorAnim = LoadAnimation("Global/Water.bin", d, 0, 0);
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
                        if (Methods.Solution.SolutionState.Main.AlwaysShowWaterLevel || Selected)
                        {
                            int startX = (Methods.Solution.SolutionState.Main.SizeWaterLevelwithBounds ? x1 : 0);
                            int endX = (Methods.Solution.SolutionState.Main.SizeWaterLevelwithBounds ? x2 : Methods.Solution.CurrentSolution.SceneWidth);
                            d.DrawRectangle(startX, heightX, endX, Methods.Solution.CurrentSolution.SceneHeight, Methods.Solution.SolutionState.Main.waterColor);
                            d.DrawLine(startX, heightX, endX, heightX, SystemColors.White);
                        }
                    }
                }

                DrawBounds2(d, x2, y2, x1, y1, Transparency, SystemColors.Aqua, SystemColors.Transparent);
            }
        }
        private void DrawWaterBase(DevicePanel d, ObjectDrawing.EditorAnimation editorAnim, int x, int y, int Transparency, int animID, int type)
        {
            DrawTexturePivotNormal(d, editorAnim, editorAnim.RequestedAnimID, editorAnim.RequestedFrameID, x, y, Transparency);
        }
        private void DrawBigBubble(DevicePanel d, ObjectDrawing.EditorAnimation editorAnim, int x, int y, int Transparency, int animID, int type)
        {
            editorAnim = LoadAnimation("HCZ/BigBubble.bin", d, 7, 0);
            DrawTexturePivotNormal(d, editorAnim, editorAnim.RequestedAnimID, editorAnim.RequestedFrameID, x, y, Transparency);
        }
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


            var editorAnim = LoadAnimation("Global/Water.bin", d, animID, 0);
            // Base Water + Bubble Source
            if (animID >= 0 && (type == 2 || type == 0) && !showBounds) DrawWaterBase(d, editorAnim, x, y, Transparency, animID, type);
            // HCZ Big Bubbles
            if (type == 5) DrawBigBubble(d, editorAnim, x, y, Transparency, animID, type);
            // Bounded Water
            if (showBounds == true && HCZBubbles == false && type != 1) DrawWaterBounds(d, editorAnim, x, y, Transparency, animID, type, widthPixels, heightPixels, heightX, r, g, b, e.Selected);
            if (type == 1) DrawGHZWater(d, editorAnim, x, y, Transparency, animID, type, widthPixels, heightPixels, heightX, r, g, b, e.Selected);

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
