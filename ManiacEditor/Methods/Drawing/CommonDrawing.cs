using IWshRuntimeLibrary;
using ManiacEditor.Actions;
using ManiacEditor.Controls;
using ManiacEditor.Controls.Editor;
using ManiacEditor.Controls.Global;
using ManiacEditor.Entity_Renders;
using ManiacEditor.Enums;
using ManiacEditor.Enums;
using ManiacEditor.Events;
using ManiacEditor.Extensions;
using Microsoft.Win32;
using RSDKv5;
using SharpDX.Direct3D9;
using System;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ManiacEditor.Methods.Drawing
{
    public static class CommonDrawing
    {
        #region Definitions

        private static ManiacEditor.Controls.Editor.MainEditor Instance { get; set; }
        public static void UpdateInstance(ManiacEditor.Controls.Editor.MainEditor _Instance)
        {
            Instance = _Instance;
        }

        #endregion

        #region Rendering Definitions

        private static SFML.Graphics.Texture OverlayImage { get; set; }
        private static System.Drawing.Size OverlayImageSize { get; set; }

        public static int GetOverlayImageOpacity(ManiacEditor.Controls.Editor.MainEditor Instance)
        {
            if (Instance != null && Instance.MenuBar != null)
            {
                return (int)Instance.MenuBar.OverlayImageOpacitySlider.Value;
            }
            return 255;
        }
        public static bool CanOverlayImage(ManiacEditor.Controls.Editor.MainEditor Instance)
        {
            if (Instance != null && Instance.MenuBar != null)
            {
                if (OverlayImage != null && Instance.MenuBar.OverlayImageEnabled.IsChecked) return true;
            }
            return false;
        }

        #endregion

        #region Overlay Image


        public static void DisposeOverlayImage()
        {
            if (OverlayImage != null)
            {
                OverlayImage.Dispose();
                OverlayImage = null;
            }
        }

        public static void LoadOverlayImage(string filename)
        {
            try
            {
                DisposeOverlayImage();
                var image = new SFML.Graphics.Image(filename);
                OverlayImageSize = new System.Drawing.Size((int)image.Size.X, (int)image.Size.Y);
                OverlayImage = new SFML.Graphics.Texture(image);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error occured while loading image: " + ex.Message);
                ClearOverlayImage();
            }
        }

        public static void SelectOverlayImage()
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Filter = "PNG File|*.png";
            ofd.Multiselect = false;
            if (ofd.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
            {
                LoadOverlayImage(ofd.FileName);
            }
        }

        public static void ClearOverlayImage()
        {
            DisposeOverlayImage();
        }

        #endregion

        #region Render Loop Methods

        public static void DrawOverlayImage(DevicePanel GraphicPanel)
        {
            int x = 0;
            int y = 0;
            int rec_x = 0;
            int rec_y = 0;
            int width = OverlayImageSize.Width;
            int height = OverlayImageSize.Height;
            GraphicPanel.DrawTexture(OverlayImage, x, y, rec_x, rec_y, width, height, false, GetOverlayImageOpacity(Instance));
        }

        public static void DrawBackground(DevicePanel GraphicPanel)
        {
            if (!ManiacEditor.Methods.Solution.SolutionState.Main.IsTilesEdit()) if (ManiacEditor.Properties.Settings.MyPerformance.HideNormalBackground == false) Instance.EditBackground.Draw(GraphicPanel);
            if (ManiacEditor.Methods.Solution.SolutionState.Main.IsTilesEdit()) if (ManiacEditor.Properties.Settings.MyPerformance.ShowEditLayerBackground == true) Instance.EditBackground.Draw(GraphicPanel, true);
        }

        public static void DrawExtraLayers(DevicePanel GraphicPanel)
        {
            for (int i = 0; i < Instance.EditorToolbar.ExtraLayerEditViewButtons.Count; i++)
            {
                var elb = Instance.EditorToolbar.ExtraLayerEditViewButtons.ElementAt(i);
                if (elb.Value.IsCheckedAll || elb.Key.IsCheckedAll)
                {
                    int index = Instance.EditorToolbar.ExtraLayerEditViewButtons.IndexOf(elb);
                    var _extraViewLayer = Methods.Solution.CurrentSolution.CurrentScene.OtherLayers.ElementAt(index);
                    _extraViewLayer.Draw(GraphicPanel);
                }
            }
        }

        public static void DrawSceneBounds(DevicePanel GraphicPanel)
        {
            int x1 = 0;
            int x2 = Methods.Solution.CurrentSolution.SceneWidth;
            int y1 = 0;
            int y2 = Methods.Solution.CurrentSolution.SceneHeight;


            GraphicPanel.DrawLine(x1, y1, x1, y2, System.Drawing.Color.White);
            GraphicPanel.DrawLine(x1, y1, x2, y1, System.Drawing.Color.White);
            GraphicPanel.DrawLine(x2, y2, x1, y2, System.Drawing.Color.White);
            GraphicPanel.DrawLine(x2, y2, x2, y1, System.Drawing.Color.White);
        }

        public static void DrawSelectionBox(DevicePanel GraphicPanel, bool resetSelection = false)
        {
            if (!resetSelection)
            {
                int bound_x1 = (int)(Methods.Solution.SolutionState.Main.RegionX2); int bound_x2 = (int)(Methods.Solution.SolutionState.Main.LastX);
                int bound_y1 = (int)(Methods.Solution.SolutionState.Main.RegionY2); int bound_y2 = (int)(Methods.Solution.SolutionState.Main.LastY);
                if (bound_x1 != bound_x2 && bound_y1 != bound_y2)
                {
                    if (bound_x1 > bound_x2)
                    {
                        bound_x1 = (int)(Methods.Solution.SolutionState.Main.LastX);
                        bound_x2 = (int)(Methods.Solution.SolutionState.Main.RegionX2);
                    }
                    if (bound_y1 > bound_y2)
                    {
                        bound_y1 = (int)(Methods.Solution.SolutionState.Main.LastY);
                        bound_y2 = (int)(Methods.Solution.SolutionState.Main.RegionY2);
                    }
                    if (ManiacEditor.Methods.Solution.SolutionState.Main.IsChunksEdit())
                    {
                        bound_x1 = Classes.Scene.EditorLayer.GetChunkCoordinatesTopEdge(bound_x1, bound_y1).X;
                        bound_y1 = Classes.Scene.EditorLayer.GetChunkCoordinatesTopEdge(bound_x1, bound_y1).Y;
                        bound_x2 = Classes.Scene.EditorLayer.GetChunkCoordinatesBottomEdge(bound_x2, bound_y2).X;
                        bound_y2 = Classes.Scene.EditorLayer.GetChunkCoordinatesBottomEdge(bound_x2, bound_y2).Y;
                    }


                }

                GraphicPanel.DrawRectangle(bound_x1, bound_y1, bound_x2, bound_y2, System.Drawing.Color.FromArgb(100, System.Drawing.Color.Purple));
                GraphicPanel.DrawLine(bound_x1, bound_y1, bound_x2, bound_y1, System.Drawing.Color.Purple);
                GraphicPanel.DrawLine(bound_x1, bound_y1, bound_x1, bound_y2, System.Drawing.Color.Purple);
                GraphicPanel.DrawLine(bound_x2, bound_y2, bound_x2, bound_y1, System.Drawing.Color.Purple);
                GraphicPanel.DrawLine(bound_x2, bound_y2, bound_x1, bound_y2, System.Drawing.Color.Purple);
            }
            else
            {
                Methods.Solution.SolutionState.Main.TempSelectX1 = 0; Methods.Solution.SolutionState.Main.TempSelectX2 = 0; Methods.Solution.SolutionState.Main.TempSelectY1 = 0; Methods.Solution.SolutionState.Main.TempSelectY2 = 0;
            }
        }

        public static void DrawBrushBox(DevicePanel GraphicPanel, bool Inactive = false)
        {
            int lastX = Methods.Solution.SolutionState.Main.LastX / Solution.SolutionConstants.TILE_SIZE * Solution.SolutionConstants.TILE_SIZE;
            int lastY = Methods.Solution.SolutionState.Main.LastY / Solution.SolutionConstants.TILE_SIZE * Solution.SolutionConstants.TILE_SIZE;


            int oddMod = (Methods.Solution.SolutionState.Main.DrawBrushSize % 2) * Methods.Solution.SolutionConstants.TILE_SIZE;
            int offset = (Methods.Solution.SolutionState.Main.DrawBrushSize / 2) * Methods.Solution.SolutionConstants.TILE_SIZE;
            int x1 = (int)(lastX) - offset;
            int x2 = (int)(lastX) + offset + oddMod;
            int y1 = (int)(lastY) - offset;
            int y2 = (int)(lastY) + offset + oddMod;


            int bound_x1 = (int)(x1); int bound_x2 = (int)(x2);
            int bound_y1 = (int)(y1); int bound_y2 = (int)(y2);
            if (bound_x1 != bound_x2 && bound_y1 != bound_y2)
            {
                if (bound_x1 > bound_x2)
                {
                    bound_x1 = (int)(x2);
                    bound_x2 = (int)(x1);
                }
                if (bound_y1 > bound_y2)
                {
                    bound_y1 = (int)(y2);
                    bound_y2 = (int)(y1);
                }
            }

            if (!Inactive) GraphicPanel.DrawRectangle(bound_x1, bound_y1, bound_x2, bound_y2, System.Drawing.Color.FromArgb(100, System.Drawing.Color.Purple));
            GraphicPanel.DrawLine(bound_x1, bound_y1, bound_x2, bound_y1, System.Drawing.Color.Purple);
            GraphicPanel.DrawLine(bound_x1, bound_y1, bound_x1, bound_y2, System.Drawing.Color.Purple);
            GraphicPanel.DrawLine(bound_x2, bound_y2, bound_x2, bound_y1, System.Drawing.Color.Purple);
            GraphicPanel.DrawLine(bound_x2, bound_y2, bound_x1, bound_y2, System.Drawing.Color.Purple);
        }

        public static void DrawLayer(DevicePanel GraphicPanel, bool ShowLayer, bool EditLayer, Classes.Scene.EditorLayer layer)
        {
            if (layer != null)
            {
                if (ShowLayer || EditLayer) layer.Draw(GraphicPanel);
            }
        }

        public static void DrawGameElements(DevicePanel GraphicPanel)
        {
            Methods.Runtime.GameHandler.DrawGameElements(GraphicPanel);

            if (Methods.Runtime.GameHandler.PlayerSelected) Methods.Runtime.GameHandler.MovePlayer(new System.Drawing.Point(Methods.Solution.SolutionState.Main.LastX, Methods.Solution.SolutionState.Main.LastY), Methods.Runtime.GameHandler.SelectedPlayer);
            if (Methods.Runtime.GameHandler.CheckpointSelected)
            {
                System.Drawing.Point clicked_point = new System.Drawing.Point((int)(Methods.Solution.SolutionState.Main.LastX), (int)(Methods.Solution.SolutionState.Main.LastY));
                Methods.Runtime.GameHandler.UpdateCheckpoint(clicked_point);
            }
        }

        #endregion

        #region Extensions

        public static SFML.Graphics.Texture FromBitmap(Bitmap bitmap)
        {
            MemoryStream stm = new MemoryStream();
            bitmap.Save(stm, System.Drawing.Imaging.ImageFormat.Png);
            return new SFML.Graphics.Texture(stm);
        }

        #endregion

        #region Tile Drawning (System.Drawing.Graphics)

        private static int TILE_SIZE
        {
            get
            {
                return Methods.Solution.SolutionConstants.TILE_SIZE;
            }
        }



        public static void DrawTile(Graphics g, ushort tile, int x, int y, bool SemiTransparent = false)
        {
            DrawTile(g, tile, x, y, false, SemiTransparent);
        }

        public static void DrawTile(Graphics g, ushort tile, int x, int y, bool ChunkDraw, bool SemiTransparent)
        {
            ushort TileIndex = (ushort)(tile & 0x3ff);
            int TileIndexInt = (int)TileIndex;
            bool flipX = ((tile >> 10) & 1) == 1;
            bool flipY = ((tile >> 11) & 1) == 1;
            bool SolidTopA = ((tile >> 12) & 1) == 1;
            bool SolidLrbA = ((tile >> 13) & 1) == 1;
            bool SolidTopB = ((tile >> 14) & 1) == 1;
            bool SolidLrbB = ((tile >> 15) & 1) == 1;


            int CollisionOpacity = (int)Instance.EditorToolbar.CollisionOpacitySlider.Value;
            var CollisionAllSolidColor = Methods.Solution.SolutionState.Main.CollisionAllSolid_Color;
            var CollisionLRDSolidColor = Methods.Solution.SolutionState.Main.CollisionLRDSolid_Color;
            var CollisionTopOnlyColor = Methods.Solution.SolutionState.Main.CollisionTopOnlySolid_Color;


            System.Drawing.Color AllSolid = System.Drawing.Color.FromArgb(CollisionOpacity, CollisionAllSolidColor.R, CollisionAllSolidColor.G, CollisionAllSolidColor.B);
            System.Drawing.Color LRDSolid = System.Drawing.Color.FromArgb(CollisionOpacity, CollisionLRDSolidColor.R, CollisionLRDSolidColor.G, CollisionLRDSolidColor.B);
            System.Drawing.Color TopOnlySolid = System.Drawing.Color.FromArgb(CollisionOpacity, CollisionTopOnlyColor.R, CollisionTopOnlyColor.G, CollisionTopOnlyColor.B);

            g.DrawImage(Methods.Solution.CurrentSolution.CurrentTiles.Image.GetBitmap(new Rectangle(0, TileIndex * TILE_SIZE, TILE_SIZE, TILE_SIZE), flipX, flipY, SemiTransparent), new Rectangle(x * TILE_SIZE, y * TILE_SIZE, TILE_SIZE, TILE_SIZE));

            if (ChunkDraw) return;

            if (Methods.Solution.SolutionState.Main.ShowCollisionA)
            {
                if (SolidLrbA || SolidTopA)
                {
                    if (SolidTopA && SolidLrbA) DrawCollision(true, AllSolid);
                    if (SolidTopA && !SolidLrbA) DrawCollision(true, TopOnlySolid);
                    if (SolidLrbA && !SolidTopA) DrawCollision(true, LRDSolid);
                }
            }
            if (Methods.Solution.SolutionState.Main.ShowCollisionB)
            {
                if (SolidLrbB || SolidTopB)
                {
                    if (SolidTopB && SolidLrbB) DrawCollision(false, AllSolid);
                    if (SolidTopB && !SolidLrbB) DrawCollision(false, TopOnlySolid);
                    if (SolidLrbB && !SolidTopB) DrawCollision(false, LRDSolid);
                }
            }

            if (Methods.Solution.SolutionState.Main.ShowFlippedTileHelper == true)
            {
                g.DrawImage(Methods.Solution.CurrentSolution.CurrentTiles.EditorImage.GetBitmap(new Rectangle(0, 3 * TILE_SIZE, TILE_SIZE, TILE_SIZE), false, false, SemiTransparent),
                            new Rectangle(x * TILE_SIZE, y * TILE_SIZE, TILE_SIZE, TILE_SIZE));
            }
            if (Methods.Solution.SolutionState.Main.ShowTileID == true)
            {
                g.DrawImage(Methods.Solution.CurrentSolution.CurrentTiles.IDImage.GetBitmap(new Rectangle(0, TileIndex * TILE_SIZE, TILE_SIZE, TILE_SIZE), false, false, SemiTransparent),
                            new Rectangle(x * TILE_SIZE, y * TILE_SIZE, TILE_SIZE, TILE_SIZE));
            }

            void DrawCollision(bool drawA, System.Drawing.Color colur)
            {
                Bitmap Map;
                if (drawA) Map = Methods.Solution.CurrentSolution.CurrentTiles.CollisionMaskA.GetBitmap(new Rectangle(0, (tile & 0x3ff) * TILE_SIZE, TILE_SIZE, TILE_SIZE), flipX, flipY, SemiTransparent);
                else Map = Methods.Solution.CurrentSolution.CurrentTiles.CollisionMaskB.GetBitmap(new Rectangle(0, (tile & 0x3ff) * TILE_SIZE, TILE_SIZE, TILE_SIZE), flipX, flipY, SemiTransparent);

                Map = Extensions.Extensions.ChangeImageColor(Map, System.Drawing.Color.White, colur);

                g.DrawImage(Map, x * TILE_SIZE, y * TILE_SIZE, TILE_SIZE, TILE_SIZE);
            }


        }

        #endregion
    }
}
