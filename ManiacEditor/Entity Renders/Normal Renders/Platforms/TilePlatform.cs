using System;
using System.Drawing;
using ManiacEditor.Methods.Solution;
using RSDKv5;
using SFML.Graphics;

namespace ManiacEditor.Entity_Renders
{
    public class NewTilePlatform : EntityRenderer
    {
        #region Definitions
        private Classes.Scene.EditorLayer MoveLayer {  get { return Methods.Solution.CurrentSolution.CurrentScene?.Move; } }
        private Classes.Rendering.TextureExt PlatformImage { get; set; }
        private Platform TrueSelf { get; set; } = new Platform();
        private ushort[][] TileMap { get; set; }
        public static bool TilesNeedUpdate { get; set; } = false;
        #endregion

        private int platform_x;
        private int platform_y;
        private int platform_width;
        private int platform_height;

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

            StoreTileMapData(PixelPosX, PixelPosY, PixelWidth, PixelHeight);
            TrueSelf.DrawSubType(Properties, DrawTileMap);
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

        private void StoreTileMapData(int _PixelPosX, int _PixelPosY, int _PixelWidth, int _PixelHeight)
        {
            platform_x = _PixelPosX;
            platform_y = _PixelPosY;
            platform_width = _PixelWidth;
            platform_height = _PixelHeight;
        }
        private bool DrawTileMap(DevicePanel d, int _x, int _y, int transparency, System.Drawing.Color color)
        {

            int x = _x - (platform_width / 2);
            int y = _y - (platform_height / 2);

            if (MoveLayer != null)
            {
                if (TileMap == null || TileMap != MoveLayer.Tiles || TilesNeedUpdate || PlatformImage == null)
                {
                    TilesNeedUpdate = false;
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

                d.DrawBitmap(PlatformImage, x, y, platform_x, platform_y, platform_width, platform_height, false, transparency, false, false, 0, color);
            }
            return true;
        }
    }
}
