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
        public static Classes.Editor.Scene.EditorEntities Entities;
        public static StageConfig StageConfig;
        public static Gameconfig GameConfig;

        #region Layers
        public static Classes.Editor.Scene.Sets.EditorLayer FGHigher => CurrentScene?.HighDetails;
        public static Classes.Editor.Scene.Sets.EditorLayer FGHigh => CurrentScene?.ForegroundHigh;
        public static Classes.Editor.Scene.Sets.EditorLayer FGLow => CurrentScene?.ForegroundLow;
        public static Classes.Editor.Scene.Sets.EditorLayer FGLower => CurrentScene?.LowDetails;
        public static Classes.Editor.Scene.Sets.EditorLayer ScratchLayer => CurrentScene?.Scratch;
        public static Classes.Editor.Scene.Sets.EditorLayer EditLayerA { get; set; }
        public static Classes.Editor.Scene.Sets.EditorLayer EditLayerB { get; set; }
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
            ManiacEditor.Interfaces.Base.MapEditor.Instance.EditorStatusBar._levelIDLabel.Content = "Level ID: NULL";
            Classes.Editor.SolutionState.LevelID = -1;
            Classes.Editor.SolutionState.EncorePaletteExists = false;
            Classes.Editor.SolutionState.EncoreSetupType = 0;
            ManiacEditor.Interfaces.Base.MapEditor.Instance.ManiacINI.ClearSettings();
            ManiacEditor.Interfaces.Base.MapEditor.Instance.userDefinedEntityRenderSwaps = new Dictionary<string, string>();
            ManiacEditor.Interfaces.Base.MapEditor.Instance.userDefinedSpritePaths = new List<string>();
            ManiacEditor.Interfaces.Base.MapEditor.Instance.EditorToolbar.EncorePaletteButton.IsChecked = false;
            ManiacEditor.Interfaces.Base.MapEditor.Instance.Paths.UnloadScene();
            Classes.Editor.SolutionState.QuitWithoutSavingWarningRequired = false;

            if (Classes.Editor.Solution.CurrentTiles != null) Classes.Editor.Solution.CurrentTiles.Dispose();
            Classes.Editor.Solution.CurrentTiles = null;

            ManiacEditor.Interfaces.Base.MapEditor.Instance.TearDownExtraLayerButtons();

            ManiacEditor.Interfaces.Base.MapEditor.Instance.Background = null;

            ManiacEditor.Interfaces.Base.MapEditor.Instance.Chunks = null;

            EditorAnimations.AnimationTiming.Clear();


            /*if (entitiesClipboard != null)
            {
                foreach (Classes.Edit.Scene.Sets.EditorEntity entity in entitiesClipboard)
                    entity.PrepareForExternalCopy();
            }*/


            // Clear local clipboards
            //TilesClipboard = null;
            ManiacEditor.Interfaces.Base.MapEditor.Instance.entitiesClipboard = null;

            Classes.Editor.Solution.Entities = null;

            Classes.Editor.SolutionState.Zoom = 1;
            Classes.Editor.SolutionState.ZoomLevel = 0;

            ManiacEditor.Interfaces.Base.MapEditor.Instance.UndoStack.Clear();
            ManiacEditor.Interfaces.Base.MapEditor.Instance.RedoStack.Clear();

            ManiacEditor.Interfaces.Base.MapEditor.Instance.EditorToolbar.EditFGLow.ClearCheckedItems();
            ManiacEditor.Interfaces.Base.MapEditor.Instance.EditorToolbar.EditFGHigh.ClearCheckedItems();
            ManiacEditor.Interfaces.Base.MapEditor.Instance.EditorToolbar.EditFGLower.ClearCheckedItems();
            ManiacEditor.Interfaces.Base.MapEditor.Instance.EditorToolbar.EditFGHigher.ClearCheckedItems();
            ManiacEditor.Interfaces.Base.MapEditor.Instance.EditorToolbar.EditEntities.ClearCheckedItems();

            ManiacEditor.Interfaces.Base.MapEditor.Instance.ZoomModel.SetViewSize();

            ManiacEditor.Interfaces.Base.MapEditor.Instance.UI.UpdateControls();

            // clear memory a little more aggressively 
            ManiacEditor.Interfaces.Base.MapEditor.Instance.EntityDrawing.ReleaseResources();
            GC.Collect();
            Classes.Editor.Solution.TileConfig = null;

            ManiacEditor.Interfaces.Base.MapEditor.Instance.UpdateStartScreen(true);
        }



    }
}
