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
using System.Windows;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ManiacEditor.Methods.Draw
{
    public static class CommonGraphics
    {
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

        public static void DrawOverlayImage(ManiacEditor.Controls.Editor.MainEditor Instance, DevicePanel GraphicPanel)
        {
            int x = 0;
            int y = 0;
            int rec_x = 0;
            int rec_y = 0;
            int width = OverlayImageSize.Width;
            int height = OverlayImageSize.Height;
            GraphicPanel.DrawTexture(OverlayImage, x, y, rec_x, rec_y, width, height, false, GetOverlayImageOpacity(Instance));
        }

        public static void DrawBackground(ManiacEditor.Controls.Editor.MainEditor Instance, DevicePanel GraphicPanel)
        {
            if (!ManiacEditor.Methods.Solution.SolutionState.Main.IsTilesEdit()) if (ManiacEditor.Properties.Settings.MyPerformance.HideNormalBackground == false) Instance.EditBackground.Draw(GraphicPanel);
            if (ManiacEditor.Methods.Solution.SolutionState.Main.IsTilesEdit()) if (ManiacEditor.Properties.Settings.MyPerformance.ShowEditLayerBackground == true) Instance.EditBackground.Draw(GraphicPanel, true);
        }

        public static void DrawExtraLayers(ManiacEditor.Controls.Editor.MainEditor Instance, DevicePanel GraphicPanel)
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
    }
}
