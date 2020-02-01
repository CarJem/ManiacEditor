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

namespace ManiacEditor.Classes.Editor
{
    public static class Solution
    {
        public static Tileconfig TileConfig;
        public static Scene.EditorTiles CurrentTiles;
        public static Scene.EditorScene CurrentScene;
        public static Classes.Edit.Scene.EditorEntities Entities;
        public static StageConfig StageConfig;
        public static Gameconfig GameConfig;

        #region Layers
        public static Classes.Editor.Scene.EditorLayer FGHigher => CurrentScene?.HighDetails;
        public static Classes.Editor.Scene.EditorLayer FGHigh => CurrentScene?.ForegroundHigh;
        public static Classes.Editor.Scene.EditorLayer FGLow => CurrentScene?.ForegroundLow;
        public static Classes.Editor.Scene.EditorLayer FGLower => CurrentScene?.LowDetails;
        public static Classes.Editor.Scene.EditorLayer ScratchLayer => CurrentScene?.Scratch;
        public static Classes.Editor.Scene.EditorLayer EditLayerA { get; set; }
        public static Classes.Editor.Scene.EditorLayer EditLayerB { get; set; }
        #endregion

        #region Screen Size
        public static int SceneWidth => (CurrentScene != null ? CurrentScene.Layers.Max(sl => sl.Width) * 16 : 0);
        public static int SceneHeight => (CurrentScene != null ? CurrentScene.Layers.Max(sl => sl.Height) * 16 : 0);
        #endregion

        public static void UnloadScene()
        {
            Classes.Editor.Solution.CurrentScene?.Dispose();
            Classes.Editor.Solution.CurrentScene = null;
            Classes.Editor.Solution.StageConfig = null;
            ManiacEditor.Editor.Instance.EditorStatusBar._levelIDLabel.Content = "Level ID: NULL";
            Classes.Editor.SolutionState.LevelID = -1;
            Classes.Editor.SolutionState.EncorePaletteExists = false;
            Classes.Editor.SolutionState.EncoreSetupType = 0;
            ManiacEditor.Editor.Instance.ManiacINI.ClearSettings();
            ManiacEditor.Editor.Instance.userDefinedEntityRenderSwaps = new Dictionary<string, string>();
            ManiacEditor.Editor.Instance.userDefinedSpritePaths = new List<string>();
            ManiacEditor.Editor.Instance.EditorToolbar.EncorePaletteButton.IsChecked = false;
            ManiacEditor.Editor.Instance.Paths.UnloadScene();
            Classes.Editor.SolutionState.QuitWithoutSavingWarningRequired = false;

            if (Classes.Editor.Solution.CurrentTiles != null) Classes.Editor.Solution.CurrentTiles.Dispose();
            Classes.Editor.Solution.CurrentTiles = null;

            ManiacEditor.Editor.Instance.TearDownExtraLayerButtons();

            ManiacEditor.Editor.Instance.Background = null;

            ManiacEditor.Editor.Instance.Chunks = null;

            EditorAnimations.AnimationTiming.Clear();


            /*if (entitiesClipboard != null)
            {
                foreach (EditorEntity entity in entitiesClipboard)
                    entity.PrepareForExternalCopy();
            }*/


            // Clear local clipboards
            //TilesClipboard = null;
            ManiacEditor.Editor.Instance.entitiesClipboard = null;

            Classes.Editor.Solution.Entities = null;

            Classes.Editor.SolutionState.Zoom = 1;
            Classes.Editor.SolutionState.ZoomLevel = 0;

            ManiacEditor.Editor.Instance.UndoStack.Clear();
            ManiacEditor.Editor.Instance.RedoStack.Clear();

            ManiacEditor.Editor.Instance.EditorToolbar.EditFGLow.ClearCheckedItems();
            ManiacEditor.Editor.Instance.EditorToolbar.EditFGHigh.ClearCheckedItems();
            ManiacEditor.Editor.Instance.EditorToolbar.EditFGLower.ClearCheckedItems();
            ManiacEditor.Editor.Instance.EditorToolbar.EditFGHigher.ClearCheckedItems();
            ManiacEditor.Editor.Instance.EditorToolbar.EditEntities.ClearCheckedItems();

            ManiacEditor.Editor.Instance.ZoomModel.SetViewSize();

            ManiacEditor.Editor.Instance.UI.UpdateControls();

            // clear memory a little more aggressively 
            ManiacEditor.Editor.Instance.EntityDrawing.ReleaseResources();
            GC.Collect();
            Classes.Editor.Solution.TileConfig = null;

            Classes.Editor.SolutionState.MenuChar = Classes.Editor.SolutionState.MenuCharS.ToCharArray();
            Classes.Editor.SolutionState.MenuChar_Small = Classes.Editor.SolutionState.MenuCharS_Small.ToCharArray();
            Classes.Editor.SolutionState.LevelSelectChar = Classes.Editor.SolutionState.LevelSelectCharS.ToCharArray();

            ManiacEditor.Editor.Instance.UpdateStartScreen(true);
        }



    }
}
