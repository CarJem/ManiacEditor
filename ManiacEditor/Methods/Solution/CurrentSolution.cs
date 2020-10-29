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
using ManiacEditor.Structures;

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

        public static InfinityConfig InfinityConfig { get; set; }
        public static InfinityUnlocks InfinityUnlocks { get; set; }
        public static IZStage IZ_Stage { get; set; }
        #endregion

        #region Internal Definitions

        public static int LevelID { get; set; } = -1;

        #endregion

        #region Init

        public static Controls.Editor.MainEditor UI_Instance { get; set; }
        public static void UpdateInstance(Controls.Editor.MainEditor _Instance)
        {
            UI_Instance = _Instance;
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

        #region Infinity Zone API Intergration

        public static void GetIZStage()
        {
            if (InfinityConfig == null) IZ_Stage = null;
            IZ_Stage = InfinityConfig.Stages.Where(x => x.StageKey == SolutionPaths.CurrentSceneData.IZ_StageKey).FirstOrDefault();
        }

        public static void SetIZStage(IZStage value)
        {
            if (InfinityConfig == null) return;
            var item = InfinityConfig.Stages.Where(x => x.StageKey == SolutionPaths.CurrentSceneData.IZ_StageKey).FirstOrDefault();
            int index = InfinityConfig.Stages.IndexOf(item);
            InfinityConfig.Stages[index] = value;
        }

        #endregion

        #region Other Methods
        public static void UnloadScene()
        {
            InfinityConfig = null;
            InfinityUnlocks = null;
            IZ_Stage = null;

            Methods.Solution.CurrentSolution.CurrentScene?.Dispose();
            Methods.Solution.CurrentSolution.CurrentScene = null;
            Methods.Solution.CurrentSolution.StageConfig = null;
            UI_Instance.EditorStatusBar.LevelIdentifierLabel.Content = "Level ID: NULL";
            Methods.Solution.CurrentSolution.LevelID = -1;
            Methods.Solution.SolutionState.Main.EncorePaletteExists = false;
            Methods.Solution.SolutionState.Main.EncoreSetupType = 0;
            Classes.Prefrences.SceneCurrentSettings.ClearSettings();
            UI_Instance.EditorToolbar.EncorePaletteButton.IsChecked = false;
			SolutionPaths.UnloadScene();

            if (Methods.Solution.CurrentSolution.CurrentTiles != null) Methods.Solution.CurrentSolution.CurrentTiles.Dispose();
            Methods.Solution.CurrentSolution.CurrentTiles = null;

            UI_Instance.EditorToolbar.TearDownExtraLayerButtons();

            UI_Instance.Background = null;
            UI_Instance.Chunks = null;

            UI_Instance.TilesToolbar = null;
            UI_Instance.EntitiesToolbar = null;

            Methods.Solution.SolutionState.Main.Zoom = 1;
            Methods.Solution.SolutionState.Main.ZoomLevel = 0;

            Actions.UndoRedoModel.ClearStacks();

            UI_Instance.EditorToolbar.EditFGLow.ClearCheckedItems();
            UI_Instance.EditorToolbar.EditFGHigh.ClearCheckedItems();
            UI_Instance.EditorToolbar.EditFGLower.ClearCheckedItems();
            UI_Instance.EditorToolbar.EditFGHigher.ClearCheckedItems();
            UI_Instance.EditorToolbar.EditEntities.ClearCheckedItems();

            UI_Instance.ViewPanel.SharpPanel.UpdateGraphicsPanelControls();

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
