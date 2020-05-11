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

namespace ManiacEditor.Methods.Solution
{
    public static class CurrentSolution
    {
        #region Scene Definitions

        public static Classes.Scene.EditorTiles CurrentTiles { get; set; }
        public static Classes.Scene.EditorScene CurrentScene { get; set; }
        public static StageConfig StageConfig { get; set; }
        public static GameConfig GameConfig { get; set; }
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
        public static Classes.Scene.EditorEntities Entities
        {
            get
            {
                if (CurrentScene == null) return null;
                return CurrentScene.Entities;
            }
            set
            {
                if (CurrentScene != null) CurrentScene.Entities = value;
            }
        }
        #endregion

        #region Internal Definitions

        public static int LevelID { get; set; } = -1;

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

        #endregion

        #region Edit Layers
        public static List<Classes.Scene.EditorLayer> EditLayers
        {
            get
            {
                var layersInEditMode = new List<Classes.Scene.EditorLayer>();
                if (EditLayerA != null) layersInEditMode.Add(EditLayerA);
                if (EditLayerB != null) layersInEditMode.Add(EditLayerB);
                if (EditLayerC != null) layersInEditMode.Add(EditLayerC);
                if (EditLayerD != null) layersInEditMode.Add(EditLayerD);
                return layersInEditMode;
            }
        }


        private static Classes.Scene.EditorLayer _EditLayerA;
        private static Classes.Scene.EditorLayer _EditLayerB;
        private static Classes.Scene.EditorLayer _EditLayerC;
        private static Classes.Scene.EditorLayer _EditLayerD;

        public static Classes.Scene.EditorLayer EditLayerA
        {
            get
            {
                return _EditLayerA;
            }
            set
            {
                _EditLayerA = value;
                Methods.Internal.UserInterface.EditorToolbars.ValidateEditorToolbars();
            }
        }
        public static Classes.Scene.EditorLayer EditLayerB
        {
            get
            {
                return _EditLayerB;
            }
            set
            {
                _EditLayerB = value;
                Methods.Internal.UserInterface.EditorToolbars.ValidateEditorToolbars();
            }
        }
        public static Classes.Scene.EditorLayer EditLayerC
        {
            get
            {
                return _EditLayerC;
            }
            set
            {
                _EditLayerC = value;
                Methods.Internal.UserInterface.EditorToolbars.ValidateEditorToolbars();
            }
        }
        public static Classes.Scene.EditorLayer EditLayerD
        {
            get
            {
                return _EditLayerD;
            }
            set
            {
                _EditLayerD = value;
                Methods.Internal.UserInterface.EditorToolbars.ValidateEditorToolbars();
            }
        }

        #endregion

        #region Screen Size
        public static int SceneWidth => (CurrentScene != null ? CurrentScene.Layers.Max(sl => sl.Width) * 16 : 0);
        public static int SceneHeight => (CurrentScene != null ? CurrentScene.Layers.Max(sl => sl.Height) * 16 : 0);
		#endregion

        #region Other Methods
        public static void UnloadScene()
        {
            Methods.Solution.CurrentSolution.CurrentScene?.Dispose();
            Methods.Solution.CurrentSolution.CurrentScene = null;
            Methods.Solution.CurrentSolution.StageConfig = null;
            Instance.EditorStatusBar.LevelIdentifierLabel.Content = "Level ID: NULL";
            Methods.Solution.CurrentSolution.LevelID = -1;
            Methods.Solution.SolutionState.Main.EncorePaletteExists = false;
            Methods.Solution.SolutionState.Main.EncoreSetupType = 0;
            Classes.Prefrences.SceneCurrentSettings.ClearSettings();
            Instance.userDefinedSpritePaths = new List<string>();
            Instance.EditorToolbar.EncorePaletteButton.IsChecked = false;
			SolutionPaths.UnloadScene();
            Methods.Solution.SolutionState.Main.QuitWithoutSavingWarningRequired = false;

            if (Methods.Solution.CurrentSolution.CurrentTiles != null) Methods.Solution.CurrentSolution.CurrentTiles.Dispose();
            Methods.Solution.CurrentSolution.CurrentTiles = null;

            Instance.EditorToolbar.TearDownExtraLayerButtons();

            Instance.Background = null;
            Instance.Chunks = null;

            Instance.TilesToolbar = null;
            Instance.EntitiesToolbar = null;

            Methods.Solution.SolutionState.Main.Zoom = 1;
            Methods.Solution.SolutionState.Main.ZoomLevel = 0;

            Actions.UndoRedoModel.ClearStacks();

            Instance.EditorToolbar.EditFGLow.ClearCheckedItems();
            Instance.EditorToolbar.EditFGHigh.ClearCheckedItems();
            Instance.EditorToolbar.EditFGLower.ClearCheckedItems();
            Instance.EditorToolbar.EditFGHigher.ClearCheckedItems();
            Instance.EditorToolbar.EditEntities.ClearCheckedItems();

            Instance.ViewPanel.SharpPanel.UpdateGraphicsPanelControls();

            Methods.Internal.UserInterface.UpdateControls();

            // clear memory a little more aggressively 
            Methods.Drawing.ObjectDrawing.ReleaseResources();
            GC.Collect();
            Methods.Solution.CurrentSolution.TileConfig = null;

            Methods.Internal.UserInterface.Misc.UpdateStartScreen(true);
        }
        #endregion
    }
}
