using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using RSDKv5;
using ManiacEditor.Actions;
using System.Drawing;
using ManiacEditor.Classes.Rendering;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Scene = RSDKv5.Scene;
using ManiacEditor.Enums;
using ManiacEditor.Extensions;
using SFML.System;
using SFML.Graphics;

namespace ManiacEditor.Classes.Rendering
{

    public class ConnectedTexturesLogic
    {

        public class Tile
        {
            public int ID { get; set; }
            public bool isTop { get; set; } = false;
            public bool isBottom { get; set; } = false;
            public bool isLeft { get; set; } = false;
            public bool isRight { get; set; } = false;

            public Tile(int _ID, bool _isTop, bool _isBottom, bool _isLeft, bool _isRight)
            {
                ID = _ID;
                isTop = _isTop;
                isBottom = _isBottom;
                isLeft = _isLeft;
                isRight = _isRight;
            }

            public Tile(int _ID, byte[] b)
            {
                ID = _ID;
                isTop = b[0] == 1;
                isBottom = b[1] == 1;
                isLeft = b[2] == 1;
                isRight = b[3] == 1;
            }

            public Tile(int _ID, string c)
            {
                ID = _ID;
                isTop = c.Contains("↑");
                isBottom = c.Contains("↓");
                isLeft = c.Contains("←");
                isRight = c.Contains("→");
            }

            public bool Matches(bool[] array)
            {
                bool _isTop = array[0];
                bool _isBottom = array[1];
                bool _isLeft = array[2];
                bool _isRight = array[3];

                if (isTop != _isTop) return false;
                if (isBottom != _isBottom) return false;
                if (isLeft != _isLeft) return false;
                if (isRight != _isRight) return false;


                return true;
            }

        }

        public Tile[] Map = new Tile[16];

        private LayerTileProvider Parent { get; set; }

        public ConnectedTexturesLogic(LayerTileProvider _Parent)
        {
            Parent = _Parent;
            InitTileMap();
        }

        private void InitTileMap()
        {
            // ↑  ↓  ←  →  ↗  ↙  ↘  ↖

            //Base
            Map[00] = new Tile(00, "");

            //Horizontal
            Map[01] = new Tile(01, "→");
            Map[02] = new Tile(02, "← →");
            Map[03] = new Tile(03, "←");

            //Vertical
            Map[04] = new Tile(12, "↓");
            Map[05] = new Tile(24, "↓ ↑");
            Map[06] = new Tile(36, "↑");

            //Box A
            Map[07] = new Tile(13, "↓ →");
            Map[08] = new Tile(15, "← ↓");
            Map[09] = new Tile(37, "→ ↑");
            Map[10] = new Tile(39, "← ↑");

            //Box  B 
            Map[11] = new Tile(25, "→ ↑ ↓");
            Map[12] = new Tile(14, "← → ↓");
            Map[13] = new Tile(38, "← → ↑");
            Map[14] = new Tile(27, "← ↑ ↓");

            //All Sides
            Map[15] = new Tile(26, "← → ↑ ↓");
        }

        public int GetConnectionID(Point point)
        {
            bool _isTop = IsTop(point);
            bool _isBottom = IsBottom(point);
            bool _isLeft = IsLeft(point);
            bool _isRight = IsRight(point);

            var array = new bool[] { _isTop, _isBottom, _isLeft, _isRight };

            var entry = Map.Where(x => x.Matches(array)).FirstOrDefault();

            if (entry != null) return entry.ID;
            else return 0;


        }

        public bool IsTop(Point point)
        {
            return Parent.IsTileSelected(new Point(point.X, point.Y - 1));
        }
        public bool IsBottom(Point point)
        {
            return Parent.IsTileSelected(new Point(point.X, point.Y + 1));
        }
        public bool IsLeft(Point point)
        {
            return Parent.IsTileSelected(new Point(point.X - 1, point.Y));
        }
        public bool IsRight(Point point)
        {
            return Parent.IsTileSelected(new Point(point.X + 1, point.Y));
        }
        public bool IsNorth(Point point)
        {
            return Parent.IsTileSelected(new Point(point.X + 1, point.Y - 1));
        }
        public bool IsEast(Point point)
        {
            return Parent.IsTileSelected(new Point(point.X + 1, point.Y + 1));
        }
        public bool IsSouth(Point point)
        {
            return Parent.IsTileSelected(new Point(point.X - 1, point.Y + 1));
        }
        public bool IsWest(Point point)
        {
            return Parent.IsTileSelected(new Point(point.X - 1, point.Y - 1));
        }

    }

    public class LayerTileProvider
    {

        private ConnectedTexturesLogic ConnectedTexturesLogic { get; set; }


        #region Layer Renders

        private Classes.Scene.EditorLayer ParentLayer { get; set; }
        public LayerRenderer MapRender { get; set; }
        public LayerRenderer MapRenderEditor { get; set; }
        public LayerRenderer MapRenderTileID { get; set; }
        public LayerRenderer MapRenderCollisionMapA { get; set; }
        public LayerRenderer MapRenderCollisionMapB { get; set; }
        public LayerRenderer MapRenderSelected { get; set; }

        #endregion

        #region Init

        public LayerTileProvider(Classes.Scene.EditorLayer parentLayer)
        {
            ParentLayer = parentLayer;
            ConnectedTexturesLogic = new ConnectedTexturesLogic(this);
        }

        #endregion

        #region Tile Providers

        public void TileIDProvider(int x, int y, int layer, double Zoom, out SFML.Graphics.Color color, out IntRect rec)
        {
            if (IsTileWithinRange(x, y))
            {
                Point point = new Point(x, y);
                var tile = GetTileToDraw(point);

                bool NotAir = (tile != 0xffff);

                if (NotAir)
                {
                    rec = GetTileRect(tile, Zoom);
                    color = GetNormalColors(new Point(x, y), tile);
                }
                else
                {
                    rec = GetNullTileProviderRect();
                    color = GetNullTileProviderColor();
                }
            }
            else
            {
                rec = GetNullTileProviderRect();
                color = GetNullTileProviderColor();
            }
        }
        public void FlippedTileProvider(int x, int y, int layer, double Zoom, out SFML.Graphics.Color color, out IntRect rec)
        {
            if (IsTileWithinRange(x, y))
            {
                Point point = new Point(x, y);
                var tile = GetTileToDraw(point);

                bool NotAir = (tile != 0xffff);

                if (NotAir)
                {
                    rec = GetTileRect(tile, Zoom, true);
                    color = GetNormalColors(new Point(x, y), tile);
                }
                else
                {
                    rec = GetNullTileProviderRect();
                    color = GetNullTileProviderColor();
                }
            }
            else
            {
                rec = GetNullTileProviderRect();
                color = GetNullTileProviderColor();
            }

        }
        public void TileProvider(int x, int y, int layer, double Zoom, out SFML.Graphics.Color color, out IntRect rec)
        {
            if (IsTileWithinRange(x, y))
            {
                Point point = new Point(x, y);
                var tile = GetTileToDraw(point);

                bool NotAir = (tile != 0xffff);

                if (NotAir)
                {
                    rec = GetTileRect(tile, Zoom);
                    color = GetNormalColors(new Point(x, y), tile);
                }
                else
                {
                    rec = GetNullTileProviderRect();
                    color = GetNullTileProviderColor();
                }
            }
            else
            {
                rec = GetNullTileProviderRect();
                color = GetNullTileProviderColor();
            }




        }
        public void TileSelectedProvider(int x, int y, int layer, double Zoom, out SFML.Graphics.Color color, out IntRect rec)
        {
            if (IsTileWithinRange(x, y))
            {
                Point point = new Point(x, y);
                if (IsTileSelected(point))
                {
                    rec = GetTileSelectedRect(point);
                    color = new SFML.Graphics.Color(255, 0, 0, (byte)ParentLayer.RenderingTransparency);
                }
                else
                {
                    rec = GetNullTileProviderRect();
                    color = GetNullTileProviderColor();
                }

            }
            else
            {
                rec = GetNullTileProviderRect();
                color = GetNullTileProviderColor();
            }




        }
        public void TileCollisionProviderA(int x, int y, int layer, double Zoom, out SFML.Graphics.Color color, out IntRect rec)
        {
            if (IsTileWithinRange(x, y))
            {
                var tile = GetTileToDraw(new Point(x, y));
                bool NotAir = (tile != 0xffff);

                if (NotAir)
                {
                    rec = GetTileRect(tile, Zoom);
                    color = GetCollisionColors(new Point(x, y), tile, true);
                }
                else
                {
                    rec = GetNullTileProviderRect();
                    color = GetNullTileProviderColor();
                }
            }
            else
            {
                rec = GetNullTileProviderRect();
                color = GetNullTileProviderColor();
            }

        }
        public void TileCollisionProviderB(int x, int y, int layer, double Zoom, out SFML.Graphics.Color color, out IntRect rec)
        {
            if (IsTileWithinRange(x, y))
            {
                var tile = GetTileToDraw(new Point(x, y));
                bool NotAir = (tile != 0xffff);

                if (NotAir)
                {
                    rec = GetTileRect(tile, Zoom);
                    color = GetCollisionColors(new Point(x, y), tile, false);
                }
                else
                {
                    rec = GetNullTileProviderRect();
                    color = GetNullTileProviderColor();
                }
            }
            else
            {
                rec = GetNullTileProviderRect();
                color = GetNullTileProviderColor();
            }

        }

        #endregion

        #region Tile IntRect

        public IntRect GetNullTileProviderRect()
        {
            return new IntRect(0, 0, 16, 16);
        }
        private IntRect GetTileSelectedRect(Point point)
        {
            int tile_size = Methods.Editor.EditorConstants.TILE_SIZE;
            int tile_texture_y = ConnectedTexturesLogic.GetConnectionID(point) * tile_size;
            int rect_x = 0;
            int rect_y = tile_texture_y;
            int rect_width = tile_size;
            int rect_height = tile_size;

            return new IntRect(rect_x, rect_y, rect_width, rect_height);
        }

        private IntRect GetTileRect(ushort tile, double zoom, bool FlipGuideMode = false)
        {
            int index = (tile & 0x3ff);

            bool flipX = ((tile >> 10) & 1) == 1;
            bool flipY = ((tile >> 11) & 1) == 1;
            bool SolidTopA = ((tile >> 12) & 1) == 1;
            bool SolidLrbA = ((tile >> 13) & 1) == 1;
            bool SolidTopB = ((tile >> 14) & 1) == 1;
            bool SolidLrbB = ((tile >> 15) & 1) == 1;

            int tile_size = Methods.Editor.EditorConstants.TILE_SIZE;
            int tile_texture_y = index * tile_size;
            int rect_x;
            int rect_y;
            int rect_width;
            int rect_height;

            if (FlipGuideMode)
            {
                tile_texture_y = 3 * tile_size;
            }


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

        #endregion

        #region Tile Colors

        private SFML.Graphics.Color GetNormalColors(Point point, ushort value)
        {
            System.Drawing.Color NormalColor = System.Drawing.Color.White;
            if (IsTileSelected(point))
            {
                NormalColor = NormalColor.Blend(System.Drawing.Color.Red, 128);
            }

            return new SFML.Graphics.Color(NormalColor.R, NormalColor.G, NormalColor.B, (byte)ParentLayer.RenderingTransparency);
        }
        public SFML.Graphics.Color GetNullTileProviderColor()
        {
            return new SFML.Graphics.Color(0, 0, 0, 0);
        }

        private System.Drawing.Color GetCollisionColor(System.Drawing.Color Color, int Opacity, int Opacity2)
        {
            double Amount = (Opacity2 / 255) * 100;
            double AmountOfLoss = Opacity / Amount;
            int RealOpacity = Opacity - (int)AmountOfLoss;
            return System.Drawing.Color.FromArgb(RealOpacity, Color.R, Color.G, Color.B);
        }

        private SFML.Graphics.Color GetCollisionColors(Point point, ushort value, bool isPathA = true)
        {
            RSDKv5.Tile tile = new Tile(value);

            System.Drawing.Color AllSolid = GetCollisionColor(ManiacEditor.Controls.Editor.MainEditor.Instance.CollisionAllSolid, ParentLayer.RenderingTransparency, (int)ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.collisionOpacitySlider.Value);
            System.Drawing.Color LRDSolid = GetCollisionColor(ManiacEditor.Controls.Editor.MainEditor.Instance.CollisionLRDSolid, ParentLayer.RenderingTransparency, (int)ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.collisionOpacitySlider.Value);
            System.Drawing.Color TopOnlySolid = GetCollisionColor(ManiacEditor.Controls.Editor.MainEditor.Instance.CollisionTopOnlySolid, ParentLayer.RenderingTransparency, (int)ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.collisionOpacitySlider.Value);

            if (IsTileSelected(point))
            {
                AllSolid = AllSolid.Blend(System.Drawing.Color.Red, 128);
                LRDSolid = LRDSolid.Blend(System.Drawing.Color.Red, 128);
                TopOnlySolid = TopOnlySolid.Blend(System.Drawing.Color.Red, 128);
            }

            if (isPathA)
            {
                if (tile.SolidTopA && tile.SolidLrbA) return new SFML.Graphics.Color(AllSolid.R, AllSolid.G, AllSolid.B, AllSolid.A);
                else if (tile.SolidTopA && !tile.SolidLrbA) return new SFML.Graphics.Color(TopOnlySolid.R, TopOnlySolid.G, TopOnlySolid.B, TopOnlySolid.A);
                else if (!tile.SolidTopA && tile.SolidLrbA) return new SFML.Graphics.Color(LRDSolid.R, LRDSolid.G, LRDSolid.B, LRDSolid.A);
                else return new SFML.Graphics.Color(255, 255, 255, 0);
            }
            else
            {
                if (tile.SolidTopB && tile.SolidLrbB) return new SFML.Graphics.Color(AllSolid.R, AllSolid.G, AllSolid.B, AllSolid.A);
                else if (tile.SolidTopB && !tile.SolidLrbB) return new SFML.Graphics.Color(TopOnlySolid.R, TopOnlySolid.G, TopOnlySolid.B, TopOnlySolid.A);
                else if (!tile.SolidTopB && tile.SolidLrbB) return new SFML.Graphics.Color(LRDSolid.R, LRDSolid.G, LRDSolid.B, LRDSolid.A);
                else return new SFML.Graphics.Color(255, 255, 255, 0);
            }

        }

        #endregion

        #region Tile Helper Methods

        public ushort GetTileToDraw(Point source)
        {
            if (ParentLayer.SelectedTiles.Values.ContainsKey(source)) return ParentLayer.SelectedTiles.Values[source];
            else return ParentLayer.Layer.Tiles[source.Y][source.X];
        }
        private bool IsTileWithinRange(int x, int y)
        {
            return (ParentLayer.Height > y && 0 <= y && ParentLayer.Width > x && 0 <= x);
        }
        public bool IsTileSelected(Point point)
        {
            if (ParentLayer.TempSelectionTiles.Contains(point))
            {
                return !ParentLayer.TempSelectionDeselectTiles.Contains(point);
            }
            else return ParentLayer.SelectedTiles.Contains(point);
        }

        #endregion
    }
}
