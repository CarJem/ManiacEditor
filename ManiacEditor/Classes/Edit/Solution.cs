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

namespace ManiacEditor.Classes.Edit
{
    public static class Solution
    {
        public static Tileconfig TileConfig;
        public static Scene.EditorTiles CurrentTiles;
        public static Scene.EditorScene CurrentScene;
        public static EditorEntities Entities;
        public static StageConfig StageConfig;
        public static Gameconfig GameConfig;

        public static void UnloadScene()
        {
            Classes.Edit.Solution.CurrentScene?.Dispose();
            Classes.Edit.Solution.CurrentScene = null;
            Classes.Edit.Solution.StageConfig = null;
            Editor.Instance.EditorStatusBar._levelIDLabel.Content = "Level ID: NULL";
            Classes.Edit.SolutionState.LevelID = -1;
            Classes.Edit.SolutionState.EncorePaletteExists = false;
            Classes.Edit.SolutionState.EncoreSetupType = 0;
            Editor.Instance.ManiacINI.ClearSettings();
            Editor.Instance.userDefinedEntityRenderSwaps = new Dictionary<string, string>();
            Editor.Instance.userDefinedSpritePaths = new List<string>();
            Editor.Instance.EditorToolbar.EncorePaletteButton.IsChecked = false;
            Editor.Instance.Paths.UnloadScene();
            Classes.Edit.SolutionState.QuitWithoutSavingWarningRequired = false;

            if (Classes.Edit.Solution.CurrentTiles != null) Classes.Edit.Solution.CurrentTiles.Dispose();
            Classes.Edit.Solution.CurrentTiles = null;

            Editor.Instance.TearDownExtraLayerButtons();

            Editor.Instance.Background = null;

            Editor.Instance.Chunks = null;

            EditorAnimations.AnimationTiming.Clear();


            /*if (entitiesClipboard != null)
            {
                foreach (EditorEntity entity in entitiesClipboard)
                    entity.PrepareForExternalCopy();
            }*/


            // Clear local clipboards
            //TilesClipboard = null;
            Editor.Instance.entitiesClipboard = null;

            Classes.Edit.Solution.Entities = null;

            Classes.Edit.SolutionState.Zoom = 1;
            Classes.Edit.SolutionState.ZoomLevel = 0;

            Editor.Instance.UndoStack.Clear();
            Editor.Instance.RedoStack.Clear();

            Editor.Instance.EditorToolbar.EditFGLow.ClearCheckedItems();
            Editor.Instance.EditorToolbar.EditFGHigh.ClearCheckedItems();
            Editor.Instance.EditorToolbar.EditFGLower.ClearCheckedItems();
            Editor.Instance.EditorToolbar.EditFGHigher.ClearCheckedItems();
            Editor.Instance.EditorToolbar.EditEntities.ClearCheckedItems();

            Editor.Instance.ZoomModel.SetViewSize();

            Editor.Instance.UI.UpdateControls();

            // clear memory a little more aggressively 
            Editor.Instance.EntityDrawing.ReleaseResources();
            GC.Collect();
            Classes.Edit.Solution.TileConfig = null;

            Classes.Edit.SolutionState.MenuChar = Classes.Edit.SolutionState.MenuCharS.ToCharArray();
            Classes.Edit.SolutionState.MenuChar_Small = Classes.Edit.SolutionState.MenuCharS_Small.ToCharArray();
            Classes.Edit.SolutionState.LevelSelectChar = Classes.Edit.SolutionState.LevelSelectCharS.ToCharArray();

            Editor.Instance.UpdateStartScreen(true);
        }



    }
}
