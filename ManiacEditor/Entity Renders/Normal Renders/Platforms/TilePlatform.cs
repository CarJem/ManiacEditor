using System;
using System.Drawing;
using RSDKv5;
using SFML.Graphics;

namespace ManiacEditor.Entity_Renders
{
    public class NewTilePlatform : EntityRenderer
    {
        #region Definitions
        private Classes.Scene.EditorLayer MoveLayer {  get { return Methods.Solution.CurrentSolution.CurrentScene?.Move; } }
        private Classes.Rendering.TextureExt PlatformImage { get; set; }
        private ushort[][] TileMap { get; set; }
        #endregion

        public override void Draw(Structures.EntityRenderProp Properties)
        {
            DevicePanel d = Properties.Graphics;

            Classes.Scene.EditorEntity e = Properties.EditorObject;
            int x = Properties.DrawX;
            int y = Properties.DrawY;
            int transparency = Properties.Transparency;

            bool fliph = false;
            bool flipv = false;


            int PixelWidth = (int)e.attributesMap["size"].ValueVector2.X.High;
            int PixelHeight = (int)e.attributesMap["size"].ValueVector2.Y.High;

            int PixelPosX = (int)e.attributesMap["targetPos"].ValueVector2.X.High - (PixelWidth / 2);
            int PixelPosY = (int)e.attributesMap["targetPos"].ValueVector2.Y.High - (PixelHeight / 2);

            int Pivot_X = x - (PixelWidth / 2);
            int Pivot_Y = y - (PixelHeight / 2);

            DrawTileMap(d, Pivot_X, Pivot_Y, PixelPosX, PixelPosY, PixelWidth, PixelHeight, transparency);
        }
        public override bool isObjectOnScreen(DevicePanel d, Classes.Scene.EditorEntity entity, int x, int y, int Transparency)
        {
            int width = (int)entity.attributesMap["size"].ValueVector2.X.High;
            int height = (int)entity.attributesMap["size"].ValueVector2.Y.High;

            int boundsX = width;
            int boundsY = height;

            return d.IsObjectOnScreen(x, y, boundsX, boundsY);
        }
        public override string GetObjectName()
        {
            return "TilePlatform";
        }
        private void DrawTileMap(DevicePanel d, int x, int y, int platform_x, int platform_y, int platform_width, int platform_height, int transparency)
        {
            if (MoveLayer != null)
            {
                if (TileMap == null || TileMap != MoveLayer.Tiles || PlatformImage == null)
                {
                    if (PlatformImage != null)
                    {
                        PlatformImage.Dispose();
                        PlatformImage = null;
                    }

                    TileMap = MoveLayer.Tiles;

                    var bitmap = new System.Drawing.Bitmap(MoveLayer.Width * Methods.Solution.SolutionConstants.TILE_SIZE, MoveLayer.Height * Methods.Solution.SolutionConstants.TILE_SIZE);
                    System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);
                    MoveLayer.Draw(g);
                    PlatformImage = Methods.Drawing.TextureCreator.FromBitmap(d._device, bitmap);
                    g.Clear(System.Drawing.Color.Transparent);
                    bitmap.Dispose();
                    g.Dispose();
                }

                d.DrawBitmap(PlatformImage, x, y, platform_x, platform_y, platform_width, platform_height, false, transparency);
            }

        }
    }
}
