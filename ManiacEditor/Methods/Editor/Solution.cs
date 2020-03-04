using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using RSDKv5;
using ManiacEditor.Actions;
using SharpDX.Direct3D9;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Scene = RSDKv5.Scene;

namespace ManiacEditor.Methods.Editor
{
    public static class Solution
    {
        #region Definitions

        public static Classes.Scene.EditorTiles CurrentTiles { get; set; }
        public static Classes.Scene.EditorScene CurrentScene { get; set; }
        public static Classes.Scene.EditorEntities Entities { get; set; }
        public static StageConfig StageConfig { get; set; }
        public static Gameconfig GameConfig { get; set; }
        public static Tileconfig TileConfig
        {
            get
            {
                if (CurrentTiles != null) return CurrentTiles.TileConfig;
                else return null;
            }
            set
            {
                if (CurrentTiles != null) CurrentTiles.TileConfig = value;
            }
        }
        #endregion

        #region Layers
        public static Classes.Scene.EditorLayer FGHigher => CurrentScene?.HighDetails;
        public static Classes.Scene.EditorLayer FGHigh => CurrentScene?.ForegroundHigh;
        public static Classes.Scene.EditorLayer FGLow => CurrentScene?.ForegroundLow;
        public static Classes.Scene.EditorLayer FGLower => CurrentScene?.LowDetails;
        public static Classes.Scene.EditorLayer ScratchLayer => CurrentScene?.Scratch;
        public static Classes.Scene.EditorLayer EditLayerA { get; set; }
        public static Classes.Scene.EditorLayer EditLayerB { get; set; }
        #endregion

        #region Screen Size
        public static int SceneWidth => (CurrentScene != null ? CurrentScene.Layers.Max(sl => sl.Width) * 16 : 0);
        public static int SceneHeight => (CurrentScene != null ? CurrentScene.Layers.Max(sl => sl.Height) * 16 : 0);
		#endregion

        #region Other Methods
        public static void UnloadScene()
        {
            Methods.Editor.Solution.CurrentScene?.Dispose();
            Methods.Editor.Solution.CurrentScene = null;
            Methods.Editor.Solution.StageConfig = null;
            ManiacEditor.Controls.Editor.MainEditor.Instance.EditorStatusBar._levelIDLabel.Content = "Level ID: NULL";
            Methods.Editor.SolutionState.LevelID = -1;
            Methods.Editor.SolutionState.EncorePaletteExists = false;
            Methods.Editor.SolutionState.EncoreSetupType = 0;
            Classes.Prefrences.SceneCurrentSettings.ClearSettings();
            ManiacEditor.Controls.Editor.MainEditor.Instance.userDefinedEntityRenderSwaps = new Dictionary<string, string>();
            ManiacEditor.Controls.Editor.MainEditor.Instance.userDefinedSpritePaths = new List<string>();
            ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.EncorePaletteButton.IsChecked = false;
			SolutionPaths.UnloadScene();
            Methods.Editor.SolutionState.QuitWithoutSavingWarningRequired = false;

            if (Methods.Editor.Solution.CurrentTiles != null) Methods.Editor.Solution.CurrentTiles.Dispose();
            Methods.Editor.Solution.CurrentTiles = null;

            ManiacEditor.Controls.Editor.MainEditor.Instance.TearDownExtraLayerButtons();

            ManiacEditor.Controls.Editor.MainEditor.Instance.Background = null;

            ManiacEditor.Controls.Editor.MainEditor.Instance.Chunks = null;

            Methods.Entities.EntityAnimator.AnimationTiming.Clear();


            /*if (entitiesClipboard != null)
            {
                foreach (Classes.Edit.Scene.Sets.EditorEntity entity in entitiesClipboard)
                    entity.PrepareForExternalCopy();
            }*/


            // Clear local clipboards
            //TilesClipboard = null;
            ManiacEditor.Controls.Editor.MainEditor.Instance.entitiesClipboard = null;

            Methods.Editor.Solution.Entities = null;

            Methods.Editor.SolutionState.Zoom = 1;
            Methods.Editor.SolutionState.ZoomLevel = 0;

            ManiacEditor.Controls.Editor.MainEditor.Instance.UndoStack.Clear();
            ManiacEditor.Controls.Editor.MainEditor.Instance.RedoStack.Clear();

            ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.EditFGLow.ClearCheckedItems();
            ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.EditFGHigh.ClearCheckedItems();
            ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.EditFGLower.ClearCheckedItems();
            ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.EditFGHigher.ClearCheckedItems();
            ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.EditEntities.ClearCheckedItems();

            ManiacEditor.Controls.Editor.MainEditor.Instance.ViewPanel.SharpPanel.ResizeGraphicsPanel();

            Methods.Internal.UserInterface.UpdateControls();

            // clear memory a little more aggressively 
            Methods.Entities.EntityDrawing.ReleaseResources();
            GC.Collect();
            Methods.Editor.Solution.TileConfig = null;

            ManiacEditor.Controls.Editor.MainEditor.Instance.UpdateStartScreen(true);
        }
        #endregion
    }
}
