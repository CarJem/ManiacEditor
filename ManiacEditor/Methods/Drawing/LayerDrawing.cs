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
using EditorLayer = ManiacEditor.Classes.Scene.EditorLayer;
using ManiacEditor.Enums;
using ManiacEditor.Extensions;
using SFML.System;
using SFML.Graphics;
using ManiacEditor.Controls.Editor;
using EditorScene = ManiacEditor.Classes.Scene.EditorScene;
using ManiacEditor.Methods.Solution;

namespace ManiacEditor.Methods.Drawing
{

    public class LayerDrawing
    {

        #region Definitions
        private MainEditor Instance { get; set; }
        private EditorScene Scene { get; set; }
        public LayerRenderer MapRender { get; set; }
        #endregion

        #region Init
        public LayerDrawing(EditorScene scene, MainEditor instance)
        {
            Instance = instance;
            Scene = scene;
            
            MapRender = new LayerRenderer(Methods.Solution.CurrentSolution.CurrentTiles.CollectiveImage.GetTexture(), TileProvider, 16, Scene.AllLayers.Count * 2);
        }

        #endregion

        #region Layer Visibility Checks

        public void ApplyLayerRules(EditorLayer layer)
        {
            if (!AreWeAnEditLayer() && Methods.Solution.SolutionState.Main.IsTilesEdit())
                layer.RenderingTransparency = 0x52;
            else if (Instance.EditorToolbar.EditEntities.IsCheckedAll && Methods.Solution.SolutionState.Main.ApplyEditEntitiesTransparency)
                layer.RenderingTransparency = 0x32;
            else
                layer.RenderingTransparency = 0xFF;

            bool AreWeAnEditLayer()
            {
                bool isEditA = layer == Methods.Solution.CurrentSolution.EditLayerA;
                bool isEditB = layer == Methods.Solution.CurrentSolution.EditLayerB;
                bool isEditC = layer == Methods.Solution.CurrentSolution.EditLayerC;
                bool isEditD = layer == Methods.Solution.CurrentSolution.EditLayerD;

                return (isEditA || isEditB || isEditC || isEditD);
            }
        }
        public void UpdateLayerVisibility()
        {
            UpdateLayer(Instance.EditorToolbar.ShowFGLower.IsChecked.Value, Instance.EditorToolbar.EditFGLower.IsCheckedAll, Methods.Solution.CurrentSolution.FGLower);
            UpdateLayer(Instance.EditorToolbar.ShowFGLow.IsChecked.Value, Instance.EditorToolbar.EditFGLow.IsCheckedAll, Methods.Solution.CurrentSolution.FGLow);
            UpdateLayer(Instance.EditorToolbar.ShowFGHigh.IsChecked.Value, Instance.EditorToolbar.EditFGHigh.IsCheckedAll, Methods.Solution.CurrentSolution.FGHigh);
            UpdateLayer(Instance.EditorToolbar.ShowFGHigher.IsChecked.Value, Instance.EditorToolbar.EditFGHigher.IsCheckedAll, Methods.Solution.CurrentSolution.FGHigher);

            for (int i = 0; i < Instance.EditorToolbar.ExtraLayerEditViewButtons.Count; i++)
            {
                var elb = Instance.EditorToolbar.ExtraLayerEditViewButtons.ElementAt(i);

                int index = Instance.EditorToolbar.ExtraLayerEditViewButtons.IndexOf(elb);
                var _extraViewLayer = Methods.Solution.CurrentSolution.CurrentScene.OtherLayers.ElementAt(index);

                if (elb.Value.IsCheckedAll || elb.Key.IsCheckedAll) _extraViewLayer.Visible = true;
                else _extraViewLayer.Visible = false;
            }

            void UpdateLayer(bool ShowLayer, bool EditLayer, Classes.Scene.EditorLayer layer)
            {
                if (layer != null)
                {
                    if (ShowLayer || EditLayer) layer.Visible = true;
                    else layer.Visible = false;
                }
            }
        }

        #endregion

        #region Tile Providers
        public void TileProvider(int x, int y, int index, out SFML.Graphics.Color color, out IntRect rec)
        {

            int actual_index = index / 2;
            int mode_index = index % 2;

            EditorLayer layer = Scene.AllLayers[actual_index];

            if (mode_index == 1)
            {
                if (SolutionState.Main.ShowTileID) TileIDProvider(x, y, layer, out color, out rec);
                else if (SolutionState.Main.ShowFlippedTileHelper) TileFlipProvider(x, y, layer, out color, out rec);
                else if (SolutionState.Main.ShowCollisionA) TileCollisionProviderA(x, y, layer, out color, out rec);
                else if (SolutionState.Main.ShowCollisionB) TileCollisionProviderB(x, y, layer, out color, out rec);
                else
                {
                    rec = GetNullTileProviderRect();
                    color = GetNullTileProviderColor();
                }
            }
            else
            {
                if (IsTileWithinRange(layer, x, y) && layer.Visible)
                {
                    Point point = new Point(x, y);
                    var tile = GetTileToDraw(layer, point);

                    bool NotAir = (tile != 0xffff);

                    if (NotAir)
                    {
                        rec = GetTileRect(0, tile);
                        color = GetNormalColors(layer, new Point(x, y), tile);
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


        }
        public void TileFlipProvider(int x, int y, EditorLayer ParentLayer, out SFML.Graphics.Color color, out IntRect rec)
        {
            if (IsTileWithinRange(ParentLayer, x, y) && ParentLayer.Visible)
            {
                Point point = new Point(x, y);
                var tile = GetTileToDraw(ParentLayer, point);

                bool NotAir = (tile != 0xffff);

                if (NotAir)
                {
                    rec = GetTileRect(64, tile, true);
                    color = GetNormalColors(ParentLayer, new Point(x, y), tile);
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
        public void TileIDProvider(int x, int y, EditorLayer ParentLayer, out SFML.Graphics.Color color, out IntRect rec)
        {
            if (IsTileWithinRange(ParentLayer, x, y) && ParentLayer.Visible)
            {
                Point point = new Point(x, y);
                var tile = GetTileToDraw(ParentLayer, point);

                bool NotAir = (tile != 0xffff);

                if (NotAir)
                {
                    rec = GetTileRectNoFlip(tile);
                    color = GetNormalColors(ParentLayer, new Point(x, y), tile);
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
        public void TileCollisionProviderA(int x, int y, EditorLayer ParentLayer, out SFML.Graphics.Color color, out IntRect rec)
        {

            if (IsTileWithinRange(ParentLayer, x, y) && ParentLayer.Visible)
            {
                var tile = GetTileToDraw(ParentLayer, new Point(x, y));
                bool NotAir = (tile != 0xffff);

                if (NotAir)
                {
                    rec = GetTileRect(32, tile);
                    color = GetCollisionColors(ParentLayer, new Point(x, y), tile, true);
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
        public void TileCollisionProviderB(int x, int y, EditorLayer ParentLayer, out SFML.Graphics.Color color, out IntRect rec)
        {
            if (IsTileWithinRange(ParentLayer, x, y) && ParentLayer.Visible)
            {
                var tile = GetTileToDraw(ParentLayer, new Point(x, y));
                bool NotAir = (tile != 0xffff);

                if (NotAir)
                {
                    rec = GetTileRect(48, tile);
                    color = GetCollisionColors(ParentLayer, new Point(x, y), tile, false);
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

        public  IntRect GetNullTileProviderRect()
        {
            return new IntRect(0, 0, 16, 16);
        }
        private  IntRect GetTileRectNoFlip(ushort tile)
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
            int rect_x = 16;
            int rect_y = tile_texture_y;
            int rect_width = tile_size;
            int rect_height = tile_size;

            return new IntRect(rect_x, rect_y, rect_width, rect_height);
        }
        private  IntRect GetTileRect(int offset, ushort tile, bool FlipGuideMode = false)
        {
            int index = (tile & 0x3ff);

            bool flipX = ((tile >> 10) & 1) == 1;
            bool flipY = ((tile >> 11) & 1) == 1;
            bool SolidTopA = ((tile >> 12) & 1) == 1;
            bool SolidLrbA = ((tile >> 13) & 1) == 1;
            bool SolidTopB = ((tile >> 14) & 1) == 1;
            bool SolidLrbB = ((tile >> 15) & 1) == 1;


            int tile_x_offset = offset;
            int tile_size = Methods.Solution.SolutionConstants.TILE_SIZE;
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
                rect_x = tile_x_offset + tile_size;
                rect_y = tile_texture_y + tile_size;
                rect_width = -tile_size;
                rect_height = -tile_size;
            }
            else if (flipY)
            {
                rect_x = tile_x_offset;
                rect_y = tile_texture_y + tile_size;
                rect_width = tile_size;
                rect_height = -tile_size;
            }
            else if (flipX)
            {
                rect_x = tile_x_offset + tile_size;
                rect_y = tile_texture_y;
                rect_width = -tile_size;
                rect_height = tile_size;
            }
            else
            {
                rect_x = tile_x_offset;
                rect_y = tile_texture_y;
                rect_width = tile_size;
                rect_height = tile_size;
            }

            return new IntRect(rect_x, rect_y, rect_width, rect_height);
        }

        #endregion

        #region Tile Colors

        private SFML.Graphics.Color GetNormalColors(EditorLayer ParentLayer, Point point, ushort value)
        {
            System.Drawing.Color NormalColor = System.Drawing.Color.White;
            if (IsTileSelected(ParentLayer, point))
            {
                NormalColor = Extensions.Extensions.Darken(NormalColor, 0.3);
            }

            return new SFML.Graphics.Color(NormalColor.R, NormalColor.G, NormalColor.B, (byte)ParentLayer.RenderingTransparency);
        }
        public SFML.Graphics.Color GetNullTileProviderColor()
        {
            return new SFML.Graphics.Color(0, 0, 0, 0);
        }

        #endregion

        #region Collision Colors

        private System.Drawing.Color GetCollisionColor(System.Drawing.Color Color, int Opacity, int Opacity2)
        {
            int Diff = Opacity > Opacity2 ? Opacity - Opacity2 : Opacity2 - Opacity;
            int Biggest = Opacity > Opacity2 ? Opacity : Opacity2;

            int RealOpacity = Biggest - (int)Diff;
            return System.Drawing.Color.FromArgb(RealOpacity, Color.R, Color.G, Color.B);
        }
        private SFML.Graphics.Color GetCollisionColors(EditorLayer ParentLayer, Point point, ushort value, bool isPathA = true)
        {
            RSDKv5.Tile tile = new Tile(value);

            int collisionOpacity = (int)ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.CollisionOpacitySlider.Value;
            int layerOpacity = ParentLayer.RenderingTransparency;

            System.Drawing.Color AllSolid = GetCollisionColor(Methods.Solution.SolutionState.Main.CollisionAllSolid_Color, layerOpacity, collisionOpacity);
            System.Drawing.Color LRDSolid = GetCollisionColor(Methods.Solution.SolutionState.Main.CollisionLRDSolid_Color, layerOpacity, collisionOpacity);
            System.Drawing.Color TopOnlySolid = GetCollisionColor(Methods.Solution.SolutionState.Main.CollisionTopOnlySolid_Color, layerOpacity, collisionOpacity);

            int AllSolid_Alpha = AllSolid.A;
            int LRDSolid_Alpha = LRDSolid.A;
            int TopOnlySolid_Alpha = TopOnlySolid.A;

            if (IsTileSelected(ParentLayer, point))
            {
                AllSolid = System.Drawing.Color.FromArgb(AllSolid_Alpha, Extensions.Extensions.Darken(AllSolid, 0.3));
                LRDSolid = System.Drawing.Color.FromArgb(LRDSolid_Alpha, Extensions.Extensions.Darken(LRDSolid, 0.3));
                TopOnlySolid = System.Drawing.Color.FromArgb(TopOnlySolid_Alpha, Extensions.Extensions.Darken(TopOnlySolid, 0.3));
            }

            if (isPathA)
            {
                if (tile.SolidTopA && tile.SolidLrbA) return new SFML.Graphics.Color(AllSolid.R, AllSolid.G, AllSolid.B, AllSolid.A);
                else if (tile.SolidTopA && !tile.SolidLrbA) return new SFML.Graphics.Color(TopOnlySolid.R, TopOnlySolid.G, TopOnlySolid.B, TopOnlySolid.A);
                else if (!tile.SolidTopA && tile.SolidLrbA) return new SFML.Graphics.Color(LRDSolid.R, LRDSolid.G, LRDSolid.B, LRDSolid.A);
                else return new SFML.Graphics.Color(0, 0, 0, 0);
            }
            else
            {
                if (tile.SolidTopB && tile.SolidLrbB) return new SFML.Graphics.Color(AllSolid.R, AllSolid.G, AllSolid.B, AllSolid.A);
                else if (tile.SolidTopB && !tile.SolidLrbB) return new SFML.Graphics.Color(TopOnlySolid.R, TopOnlySolid.G, TopOnlySolid.B, TopOnlySolid.A);
                else if (!tile.SolidTopB && tile.SolidLrbB) return new SFML.Graphics.Color(LRDSolid.R, LRDSolid.G, LRDSolid.B, LRDSolid.A);
                else return new SFML.Graphics.Color(0, 0, 0, 0);
            }

        }

        #endregion

        #region Tile Helper Methods

        public ushort GetTileToDraw(EditorLayer ParentLayer, Point source)
        {
            if (ParentLayer.SelectedTiles.Values.ContainsKey(source)) return ParentLayer.SelectedTiles.Values[source];
            else return ParentLayer.Layer.Tiles[source.Y][source.X];
        }
        private  bool IsTileWithinRange(EditorLayer ParentLayer, int x, int y)
        {
            return (ParentLayer.Height > y && 0 <= y && ParentLayer.Width > x && 0 <= x);
        }
        public  bool IsTileSelected(EditorLayer ParentLayer, Point point)
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
