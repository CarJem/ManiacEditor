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
        public static Stageconfig StageConfig { get; set; }
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

        #region Init

        private static Controls.Editor.MainEditor Instance { get; set; }
        public static void UpdateInstance(Controls.Editor.MainEditor instance)
        {
            Instance = instance;
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
            Instance.EditorStatusBar._levelIDLabel.Content = "Level ID: NULL";
            Methods.Editor.SolutionState.LevelID = -1;
            Methods.Editor.SolutionState.EncorePaletteExists = false;
            Methods.Editor.SolutionState.EncoreSetupType = 0;
            Classes.Prefrences.SceneCurrentSettings.ClearSettings();
            Instance.userDefinedEntityRenderSwaps = new Dictionary<string, string>();
            Instance.userDefinedSpritePaths = new List<string>();
            Instance.EditorToolbar.EncorePaletteButton.IsChecked = false;
			SolutionPaths.UnloadScene();
            Methods.Editor.SolutionState.QuitWithoutSavingWarningRequired = false;

            if (Methods.Editor.Solution.CurrentTiles != null) Methods.Editor.Solution.CurrentTiles.Dispose();
            Methods.Editor.Solution.CurrentTiles = null;

            Instance.EditorToolbar.TearDownExtraLayerButtons();

            Instance.Background = null;

            Instance.Chunks = null;

            Methods.Entities.EntityAnimator.AnimationTiming.Clear();

            Instance.TilesClipboard = null;
            Instance.ObjectsClipboard = null;

            Methods.Editor.Solution.Entities = null;

            Methods.Editor.SolutionState.OldZoom = 1;
            Methods.Editor.SolutionState.ZoomLevel = 0;

            Actions.UndoRedoModel.UndoStack.Clear();
            Actions.UndoRedoModel.RedoStack.Clear();

            Instance.EditorToolbar.EditFGLow.ClearCheckedItems();
            Instance.EditorToolbar.EditFGHigh.ClearCheckedItems();
            Instance.EditorToolbar.EditFGLower.ClearCheckedItems();
            Instance.EditorToolbar.EditFGHigher.ClearCheckedItems();
            Instance.EditorToolbar.EditEntities.ClearCheckedItems();

            Instance.ViewPanel.SharpPanel.ResizeGraphicsPanel();

            Methods.Internal.UserInterface.UpdateControls();

            // clear memory a little more aggressively 
            Methods.Entities.EntityDrawing.ReleaseResources();
            GC.Collect();
            Methods.Editor.Solution.TileConfig = null;

            Instance.UpdateStartScreen(true);
        }
        #endregion
    }
}
