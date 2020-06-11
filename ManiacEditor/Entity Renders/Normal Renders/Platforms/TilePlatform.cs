using System;
using System.Drawing;
using RSDKv5;
using SFML.Graphics;

namespace ManiacEditor.Entity_Renders
{
    public class NewTilePlatform : EntityRenderer
    {
        #region Definitions
        private Classes.Scene.EditorLayer ScatchLayer
        {
            get
            {
                return Methods.Solution.CurrentSolution.CurrentScene?.Move;
            }
        }
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

            int initalSizeX = (int)e.attributesMap["size"].ValueVector2.X.High / 16;
            int initalSizeY = (int)e.attributesMap["size"].ValueVector2.Y.High / 16;

            Width = initalSizeX + 1;
            Height = initalSizeY + 1;

            int offsetX = (int)e.attributesMap["targetPos"].ValueVector2.X.High / 16;
            int offsetY = (int)e.attributesMap["targetPos"].ValueVector2.Y.High / 16;

            int RealOffsetX = (offsetX - initalSizeX < 0 ? 0 : offsetX - initalSizeX);
            int RealOffsetY = (offsetY - initalSizeY < 0 ? 0 : offsetY - initalSizeY);

            DrawTileMap(d, x, y, RealOffsetX, RealOffsetY, Width, Height);

            var Animation = Methods.Drawing.ObjectDrawing.LoadAnimation(Properties.Graphics, "EditorIcons2", 0, 7);
            DrawTexturePivotNormal(Properties.Graphics, Animation, Animation.RequestedAnimID, Animation.RequestedFrameID, x, y, RenderingTransparency);
        }
        private void DrawTileMap(DevicePanel d, int x, int y, int offsetX, int offsetY, int width, int height)
        {
            if (LayerRenderer == null) LayerRenderer = new Classes.Rendering.LayerRenderer(Methods.Solution.CurrentSolution.CurrentTiles.Image.GetTexture(), TileProvider, 16, 1);
            TileMap = GetTileMap(offsetX, offsetY, width, height);
            UpdateTileMap(d, x, y);
        }
        public override bool isObjectOnScreen(DevicePanel d, Classes.Scene.EditorEntity entity, int x, int y, int Transparency)
        {
            int width = (int)entity.attributesMap["size"].ValueVector2.X.High;
            int height = (int)entity.attributesMap["size"].ValueVector2.Y.High;
            int x2 = (int)entity.attributesMap["offset"].ValueVector2.X.High;
            int y2 = (int)entity.attributesMap["offset"].ValueVector2.Y.High;

            int boundsX = width / 16;
            int boundsY = height / 16;

            return d.IsObjectOnScreen(x, y, boundsX, boundsY);
        }
        public override string GetObjectName()
        {
            return "TilePlatform";
        }
        #endregion

        #region Tile Provider
        private void UpdateTileMap(DevicePanel d, int x, int y)
        {
            int Aligned_X = x;
            int Aligned_Y = y;
            int Aligned_W = (int)((Width * 16));
            int Aligned_H = (int)((Height * 16));

            if (LayerRenderer == null) return;
            LayerRenderer.Position = new SFML.System.Vector2f(Aligned_X, Aligned_Y);
            LayerRenderer.Size = new SFML.System.Vector2f(Aligned_W, Aligned_H);
            LayerRenderer.Refresh();
            d.RenderWindow.Draw(LayerRenderer);
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
            if (x >= layerWidth || y >= layerHeight) return 0xffff;
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

    /*
    public class TilePlatform : EntityRenderer
    {

        public override void Draw(Structures.EntityRenderProp properties)
        {
            Methods.Draw.GraphicsHandler d = properties.Graphics;
            SceneEntity entity = properties.Object; 
            Classes.Scene.Sets.EditorEntity e = properties.EditorObject;
            int x = properties.X;
            int y = properties.Y;
            int Transparency = properties.Transparency;
            int index = properties.Index;
            int previousChildCount = properties.PreviousChildCount;
            int platformAngle = properties.PlatformAngle;
            Methods.Entities.EntityAnimator Animation = properties.Animations;
            bool selected  = properties.isSelected;
            bool fliph = false;
            bool flipv = false;



            int width = (int)entity.attributesMap["size"].ValueVector2.X.High;
            int height = (int)entity.attributesMap["size"].ValueVector2.Y.High;
            int TargetX = (int)entity.attributesMap["targetPos"].ValueVector2.X.High;
            int TargetY = (int)entity.attributesMap["targetPos"].ValueVector2.Y.High;
            var editorAnim = Controls.Editor.MainEditor.Instance.EntityDrawing.LoadAnimation2("EditorIcons2", d.DevicePanel, 0, 7, fliph, flipv, false);

            width = RoundNum(width, Methods.Editor.EditorConstants.TILE_SIZE) / Methods.Editor.EditorConstants.TILE_SIZE;
            height = RoundNum(height, Methods.Editor.EditorConstants.TILE_SIZE) / Methods.Editor.EditorConstants.TILE_SIZE;
            TargetX = RoundNum(TargetX, Methods.Editor.EditorConstants.TILE_SIZE) / Methods.Editor.EditorConstants.TILE_SIZE;
            TargetY = RoundNum(TargetY, Methods.Editor.EditorConstants.TILE_SIZE) / Methods.Editor.EditorConstants.TILE_SIZE;

            //d.DrawRectangle(x - width / 2, y - height / 2, x + width/2, y + height/2, System.Drawing.Color.White);
            // The position for some platforms are still off a bit (but it's very decent)
            if (Methods.Editor.Solution.CurrentScene?.Move != null)
            {
                SceneLayer Move = Methods.Editor.Solution.CurrentScene?.Move.Layer;

                DrawPlatformTiles(d, Move, x, y, TargetX, TargetY, width, height, Transparency, e.Selected);

            }
        }

        private void DrawPlatformTiles(Methods.Draw.GraphicsHandler d, SceneLayer _layer, int ObjectX, int ObjectY, int CenterX, int CenterY, int width, int height, int Transperncy, bool selected = false)
        {
            //Rectangle rect = GetTileArea(x2, y2, width, height, _layer);

            if (width == 0 || height == 0) return;

            int x = ObjectX - ((width * Methods.Editor.EditorConstants.TILE_SIZE) / 2);
            int y = ObjectY - ((height * Methods.Editor.EditorConstants.TILE_SIZE) / 2);

            int TargetX = CenterX - width / 2;
            int TargetY = CenterY - height / 2;

            if (TargetY < 0 || TargetX < 0 || CenterX + width > GetColumnLength(_layer.Tiles, 1) || CenterY + height > GetColumnLength(_layer.Tiles, 0)) return;
            for (int ty = 0; ty < height; ++ty)
            {
                for (int tx = 0; tx < width; ++tx)
                {
                    int TileX = TargetX + tx;
                    int TileY = TargetY + ty;

                    // We will draw those later
                    if (_layer.Tiles?[TileY][TileX] != 0xffff)
                    {
                        DrawTile(d, _layer.Tiles[TileY][TileX], x + (tx * Methods.Editor.EditorConstants.TILE_SIZE), y + (ty * Methods.Editor.EditorConstants.TILE_SIZE), selected, Transperncy);
                    }
                }
            }
        }

        private int GetColumnLength(ushort[][] jaggedArray, int columnIndex)
        {
            int count = 0;
            foreach (ushort[] row in jaggedArray)
            {
                if (columnIndex < row.Length) count++;
            }
            return count;
        }

        private Rectangle GetTileArea(int x, int y, int width, int height, SceneLayer _layer)
        {
            int startX = x - width;
            int endX = x + width;

            int startY = y - height;
            int endY = y + height;

            return new Rectangle(startX, startY, endX, endY);
        }


        public void DrawTile(Methods.Draw.GraphicsHandler d, ushort tile, int x, int y, bool selected, int Transperncy)
        {
            bool flipX = ((tile >> 10) & 1) == 1;
            bool flipY = ((tile >> 11) & 1) == 1;
            d.DrawBitmap(Methods.Editor.Solution.CurrentTiles.Image.GetTexture(d.DevicePanel._device, new Rectangle(0, (tile & 0x3ff) * Methods.Editor.EditorConstants.TILE_SIZE, Methods.Editor.EditorConstants.TILE_SIZE, Methods.Editor.EditorConstants.TILE_SIZE), flipX, flipY),
            x, y, Methods.Editor.EditorConstants.TILE_SIZE, Methods.Editor.EditorConstants.TILE_SIZE, selected, Transperncy);
        }

        private int RoundNum(int num, int step)
        {
            if (num >= 0)
                return ((num + (step / 2)) / step) * step;
            else
                return ((num - (step / 2)) / step) * step;
        }




        public override string GetObjectName()
        {
            return "TilePlatform";
        }
    }
    */
}
