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

        private static Texture CurrentTexture { get; set; }
        private Classes.Rendering.LayerRenderer LayerRenderer { get; set; }
        private ushort[][] TileMap { get; set; }
        private int Width { get; set; }
        private int Height { get; set; }
        private int RenderingTransparency { get; set; }

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

            DrawTileMap(d, x, y, offsetX, offsetY, Width, Height);

            var Animation = Methods.Drawing.ObjectDrawing.LoadAnimation(Properties.Graphics, "EditorIcons2", 0, 7);
            DrawTexturePivotNormal(Properties.Graphics, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, RenderingTransparency);
        }
        private void DrawTileMap(DevicePanel d, int x, int y, int offsetX, int offsetY, int width, int height)
        {
            if (CurrentTexture == null || CurrentTexture != Methods.Solution.CurrentSolution.CurrentTiles.BaseImage.GetTexture()) CurrentTexture = Methods.Solution.CurrentSolution.CurrentTiles.BaseImage.GetTexture();
            if (LayerRenderer == null) LayerRenderer = new Classes.Rendering.LayerRenderer(CurrentTexture, TileProvider, 16, 1);
            TileMap = GetTileMap(offsetX, offsetY, width, height);
            UpdateTileMap(d, x, y);
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

        #region Tile Provider
        private void UpdateTileMap(DevicePanel d, int x, int y)
        {
            //TODO: Make Working Again
            /*
            int Aligned_X = (x / 16) * 16;
            int Aligned_Y = (y / 16) * 16;
            int Aligned_W = (int)((Width * 16));
            int Aligned_H = (int)((Height * 16));

            if (LayerRenderer == null) return;
            LayerRenderer.Position = new SFML.System.Vector2f(Aligned_X, Aligned_Y);
            LayerRenderer.Size = new SFML.System.Vector2f(Aligned_W, Aligned_H);
            LayerRenderer.Refresh();
            d.RenderWindow.Draw(LayerRenderer);
            */
        }
        private ushort[][] GetTileMap(int offsetX, int offsetY, int width, int height)
        {
            ushort[][] map = new ushort[height][];
            for (int y = 0; y < height; y++)
            {
                map[y] = new ushort[width];
                for (int x = 0; x < width; x++)
                {
                    map[y][x] = GetTile(offsetX + x, offsetY + y);
                }
            }
            return map;
        }
        private ushort GetTile(int x, int y)
        {
            if (ScatchLayer == null) return 0xffff;
            int layerWidth = ScatchLayer.Width;
            int layerHeight = ScatchLayer.Height;
            if (x > layerWidth || y > layerHeight) return 0xffff;
            else return ScatchLayer.Tiles[y][x];
        }
        private void TileProvider(int x, int y, int layer, out SFML.Graphics.Color color, out IntRect rec)
        {
            if (IsTileWithinRange(x, y))
            {
                Point point = new Point(x, y);
                var tile = TileMap[point.Y][point.X];

                bool NotAir = (tile != 0xffff);

                if (NotAir)
                {
                    rec = GetTileRect(tile);
                    color = GetNormalColors(new Point(x, y), tile);
                }
                else
                {
                    rec = GetNullTileRect();
                    color = GetNullTileProviderColor();
                }
            }
            else
            {
                rec = GetNullTileRect();
                color = GetNullTileProviderColor();
            }
        }

        #endregion

        #region Tile Drawing Methods

        SFML.Graphics.Color GetNormalColors(Point point, ushort value)
        {
            System.Drawing.Color NormalColor = System.Drawing.Color.White;
            return new SFML.Graphics.Color(NormalColor.R, NormalColor.G, NormalColor.B, (byte)RenderingTransparency);
        }
        SFML.Graphics.Color GetNullTileProviderColor()
        {
            return new SFML.Graphics.Color(0, 0, 0, 0);
        }
        IntRect GetTileRect(ushort tile)
        {
            int index = (tile & 0x3ff);

            bool flipX = ((tile >> 10) & 1) == 1;
            bool flipY = ((tile >> 11) & 1) == 1;
            bool SolidTopA = ((tile >> 12) & 1) == 1;
            bool SolidLrbA = ((tile >> 13) & 1) == 1;
            bool SolidTopB = ((tile >> 14) & 1) == 1;
            bool SolidLrbB = ((tile >> 15) & 1) == 1;

            int tile_size = Methods.Solution.SolutionConstants.TILE_SIZE;
            int tile_texture_y = index * tile_size;
            int rect_x;
            int rect_y;
            int rect_width;
            int rect_height;


            if (flipX && flipY)
            {
                rect_x = tile_size;
                rect_y = tile_texture_y + tile_size;
                rect_width = -tile_size;
                rect_height = -tile_size;
            }
            else if (flipY)
            {
                rect_x = 0;
                rect_y = tile_texture_y + tile_size;
                rect_width = tile_size;
                rect_height = -tile_size;
            }
            else if (flipX)
            {
                rect_x = tile_size;
                rect_y = tile_texture_y;
                rect_width = -tile_size;
                rect_height = tile_size;
            }
            else
            {
                rect_x = 0;
                rect_y = tile_texture_y;
                rect_width = tile_size;
                rect_height = tile_size;
            }

            return new IntRect(rect_x, rect_y, rect_width, rect_height);
        }
        IntRect GetNullTileRect()
        {
            return new IntRect(0, 0, 16, 16);
        }
        bool IsTileWithinRange(int _x, int _y)
        {
            return (Height > _y && 0 <= _y && Width > _x && 0 <= _x);
        }

        #endregion
    }
}
