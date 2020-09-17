using System;
using System.Drawing;
using RSDKv5;
using SFML.Graphics;

namespace ManiacEditor.Entity_Renders
{
    public class EncoreRoute : EntityRenderer
    {
        #region Definitions
        private Classes.Scene.EditorLayer ScatchLayer
        {
            get
            {
                return Methods.Solution.CurrentSolution.CurrentScene?.Scratch;
            }
        }

        private Classes.Rendering.TextureExt PlatformImage { get; set; }
        private ushort[][] TileMap { get; set; }
        private int Width { get; set; }
        private int Height { get; set; }
        private int RenderingTransparency { get; set; }
        public static bool TilesNeedUpdate { get; set; } = false;

        #endregion

        #region Main
        public override void Draw(Structures.EntityRenderProp Properties)
        {
            DevicePanel d = Properties.Graphics;

            Classes.Scene.EditorEntity e = Properties.EditorObject;
            int x = Properties.DrawX;
            int y = Properties.DrawY;
            RenderingTransparency = Properties.Transparency;

            bool fliph = false;
            bool flipv = false;

            Width = (int)e.attributesMap["size"].ValueVector2.X.High;
            Height = (int)e.attributesMap["size"].ValueVector2.Y.High;
            int offsetX = (int)e.attributesMap["offset"].ValueVector2.X.High;
            int offsetY = (int)e.attributesMap["offset"].ValueVector2.Y.High;

            int real_x = (x != 0 ? ((x / 16) * 16) : 0);
            int real_y = (y != 0 ? ((y / 16) * 16) : 0);

            DrawTileMap(d, real_x, real_y, offsetX * 16, offsetY * 16, Width * 16, Height * 16, RenderingTransparency);

            var Animation = Methods.Drawing.ObjectDrawing.LoadAnimation(Properties.Graphics, "EditorIcons2", 0, 7);
            DrawTexturePivotNormal(Properties.Graphics, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, RenderingTransparency);
        }
        private void DrawTileMap(DevicePanel d, int x, int y, int platform_x, int platform_y, int platform_width, int platform_height, int transparency)
        {
            if (ScatchLayer != null)
            {
                if (TileMap == null || TileMap != ScatchLayer.Tiles || TilesNeedUpdate || PlatformImage == null)
                {
                    TilesNeedUpdate = false;
                    if (PlatformImage != null)
                    {
                        PlatformImage.Dispose();
                        PlatformImage = null;
                    }

                    TileMap = ScatchLayer.Tiles;

                    var bitmap = new System.Drawing.Bitmap(ScatchLayer.Width * Methods.Solution.SolutionConstants.TILE_SIZE, ScatchLayer.Height * Methods.Solution.SolutionConstants.TILE_SIZE);
                    System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);
                    ScatchLayer.Draw(g);
                    PlatformImage = Methods.Drawing.TextureCreator.FromBitmap(d._device, bitmap);
                    g.Clear(System.Drawing.Color.Transparent);
                    bitmap.Dispose();
                    g.Dispose();
                }

                d.DrawBitmap(PlatformImage, x, y, platform_x, platform_y, platform_width, platform_height, false, transparency);
            }

        }


        public override bool isObjectOnScreen(DevicePanel d, Classes.Scene.EditorEntity entity, int x, int y, int Transparency)
        {
            int width = (int)entity.attributesMap["size"].ValueVector2.X.High;
            int height = (int)entity.attributesMap["size"].ValueVector2.Y.High;
            int x2 = (int)entity.attributesMap["offset"].ValueVector2.X.High;
            int y2 = (int)entity.attributesMap["offset"].ValueVector2.Y.High;

            int boundsX = width * 16;
            int boundsY = height * 16;

            return d.IsObjectOnScreen(x, y, boundsX, boundsY);
        }
        public override string GetObjectName()
        {
            return "EncoreRoute";
        }
        #endregion
    }
}
