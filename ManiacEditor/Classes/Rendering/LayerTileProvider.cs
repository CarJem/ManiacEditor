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
    public class LayerTileProvider
    {
        #region Layer Renders

        private Classes.Scene.EditorLayer ParentLayer { get; set; }
        public LayerRenderer MapRender { get; set; }
        public LayerRenderer MapRenderEditor { get; set; }
        public LayerRenderer MapRenderTileID { get; set; }
        public LayerRenderer MapRenderCollisionMapA { get; set; }
        public LayerRenderer MapRenderCollisionMapB { get; set; }

        #endregion

        #region Init

        public LayerTileProvider(Classes.Scene.EditorLayer parentLayer)
        {
            ParentLayer = parentLayer;
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
            if (IsTileSelected(point)) return new SFML.Graphics.Color(255, 0, 0, (byte)ParentLayer.RenderingTransparency);
            else return new SFML.Graphics.Color(255, 255, 255, (byte)ParentLayer.RenderingTransparency);
        }
        public SFML.Graphics.Color GetNullTileProviderColor()
        {
            return new SFML.Graphics.Color(0, 0, 0, 0);
        }
        private SFML.Graphics.Color GetCollisionColors(Point point, ushort value, bool isPathA = true)
        {
            RSDKv5.Tile tile = new Tile(value);

            System.Drawing.Color AllSolid = ManiacEditor.Controls.Editor.MainEditor.Instance.CollisionAllSolid;
            System.Drawing.Color LRDSolid = ManiacEditor.Controls.Editor.MainEditor.Instance.CollisionLRDSolid;
            System.Drawing.Color TopOnlySolid = ManiacEditor.Controls.Editor.MainEditor.Instance.CollisionTopOnlySolid;

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
        private bool IsTileSelected(Point point)
        {
            return ParentLayer.SelectedTiles.Contains(point) || ParentLayer.TempSelectionTiles.Contains(point) && !ParentLayer.TempSelectionDeselectTiles.Contains(point);
        }

        #endregion
    }
}
