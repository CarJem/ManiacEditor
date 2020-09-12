using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.Specialized;
using ManiacEditor.Controls.Editor;
using System.Management.Instrumentation;

namespace ManiacEditor.Controls.Editor
{
    /// <summary>
    /// Interaction logic for MenuBar.xaml
    /// </summary>
    public partial class Menu : UserControl
    {
        #region Init
        public Menu()
        {
            InitializeComponent();
        }

        private MainEditor Instance { get; set; }

        public void UpdateInstance(MainEditor instance)
        {
            Instance = instance;
        }
        #endregion

        #region UI Refresh
        public void SetSceneOnlyButtonsState(bool enabled)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                saveToolStripMenuItem.IsEnabled = enabled;
                saveAsToolStripMenuItem.IsEnabled = enabled;
                unloadSceneToolStripMenuItem.IsEnabled = enabled;
                goToToolStripMenuItem.IsEnabled = enabled;
                findUnusedTilesToolStripMenuItem.IsEnabled = enabled;
                maniacinieditorToolStripMenuItem.IsEnabled = enabled;
                exportToolStripMenuItem.IsEnabled = enabled;
                changeEncorePaleteToolStripMenuItem.IsEnabled = enabled;
                ImageOverlayGroupMenuItem.IsEnabled = enabled;
                BackupMenuItem.IsEnabled = enabled;
                tilemanagerToolStripMenuItem.IsEnabled = enabled;
            }));

        }

        public void UpdateMenuItems()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                newToolStripMenuItem.InputGestureText = "Ctrl + N";
                openToolStripMenuItem.InputGestureText = "Ctrl + O";
                openDataDirectoryToolStripMenuItem.InputGestureText = "Ctrl + Alt + O";
                saveToolStripMenuItem.InputGestureText = "Ctrl + S";
                saveAsToolStripMenuItem.InputGestureText = "Ctrl + Alt + S";
                undoToolStripMenuItem.InputGestureText = "Ctrl + Z";
                redoToolStripMenuItem.InputGestureText = "Ctrl + Y";
                cutToolStripMenuItem.InputGestureText = "Ctrl + X";
                copyToolStripMenuItem.InputGestureText = "Ctrl + C";
                pasteToolStripMenuItem.InputGestureText = "Ctrl + V";
                duplicateToolStripMenuItem.InputGestureText = "Ctrl + D";
                selectAllToolStripMenuItem.InputGestureText = "Ctrl + A";
                deleteToolStripMenuItem.InputGestureText = "Delete";
                statusNAToolStripMenuItem.InputGestureText = "F3";
                nudgeSelectionFasterToolStripMenuItem.InputGestureText = "Ctrl + F1";
                swapScrollLockDirMenuToolstripButton.InputGestureText = "Ctrl + F3";
                resetZoomLevelToolstripMenuItem.InputGestureText = "Ctrl + 0";
                unloadSceneToolStripMenuItem.InputGestureText = "Ctrl + U";
                flipVerticalIndvidualToolStripMenuItem.InputGestureText = "Ctrl + F";
                flipHorizontalIndvidualToolStripMenuItem.InputGestureText = "Ctrl + M";
                flipHorizontalToolStripMenuItem.InputGestureText = "M";
                flipVerticalToolStripMenuItem.InputGestureText = "F";
                pasteTochunkToolStripMenuItem.InputGestureText = "Ctrl + Shift + V";
            }));

        }

        public void UpdateUndoRedoButtons(bool enabled)
        {
            undoToolStripMenuItem.IsEnabled = enabled && Actions.UndoRedoModel.UndoStack.Count > 0;
            redoToolStripMenuItem.IsEnabled = enabled && Actions.UndoRedoModel.RedoStack.Count > 0;

            if (undoToolStripMenuItem.IsEnabled) undoToolStripMenuItem.Foreground = Methods.Internal.Theming.NormalText;
            else undoToolStripMenuItem.Foreground = Methods.Internal.Theming.DisabledText;
            if (redoToolStripMenuItem.IsEnabled) redoToolStripMenuItem.Foreground = Methods.Internal.Theming.NormalText;
            else redoToolStripMenuItem.Foreground = Methods.Internal.Theming.DisabledText;
        }

        public void SetEditButtonsState(bool enabled)
        {
            entityManagerToolStripMenuItem.IsEnabled = enabled && Methods.Solution.CurrentSolution.StageConfig != null;
            importSoundsToolStripMenuItem.IsEnabled = enabled && Methods.Solution.CurrentSolution.StageConfig != null;
            layerManagerToolStripMenuItem.IsEnabled = enabled;
            editBackgroundColorsToolStripMenuItem.IsEnabled = enabled;

            UpdateUndoRedoButtons(enabled);

            //findAndReplaceToolStripMenuItem.IsEnabled = enabled && Methods.Editor.Solution.EditLayerA != null;
        }

        public void SetPasteButtonsState(bool enabled)
        {
            bool windowsClipboardState;
            bool windowsEntityClipboardState;
            //Doing this too often seems to cause a lot of grief for the app, should be relocated and stored as a bool
            try
            {
                if (ManiacEditor.Methods.Solution.SolutionState.Main.IsTilesEdit()) windowsClipboardState = Clipboard.ContainsData("ManiacTiles");
                else windowsClipboardState = false;
                if (ManiacEditor.Methods.Solution.SolutionState.Main.IsEntitiesEdit()) windowsEntityClipboardState = Clipboard.ContainsData("ManiacEntities");
                else windowsEntityClipboardState = false;
            }
            catch
            {
                windowsClipboardState = false;
                windowsEntityClipboardState = false;
            }


            if (ManiacEditor.Methods.Solution.SolutionState.Main.IsTilesEdit())
            {
                if (enabled && HasCopyDataTiles()) SetPasteEnabledButtons(true);
                else SetPasteEnabledButtons(false);
            }
            else if (ManiacEditor.Methods.Solution.SolutionState.Main.IsEntitiesEdit())
            {
                if (enabled && HasCopyDataEntities()) SetPasteEnabledButtons(true);
                else SetPasteEnabledButtons(false);
            }
            else
            {
                SetPasteEnabledButtons(false);
            }

            void SetPasteEnabledButtons(bool pasteEnabled)
            {
                pasteToolStripMenuItem.IsEnabled = pasteEnabled;
                pasteTochunkToolStripMenuItem.IsEnabled = pasteEnabled && ManiacEditor.Methods.Solution.SolutionState.Main.IsTilesEdit();
            }

            bool HasCopyDataTiles() { return Methods.Solution.SolutionClipboard.TilesClipboard != null || windowsClipboardState == true; }
            bool HasCopyDataEntities() { return Methods.Solution.SolutionClipboard.ObjectsClipboard != null || windowsEntityClipboardState == true; }
        }

        public void SetSelectOnlyButtonsState(bool enabled = true)
        {
            deleteToolStripMenuItem.IsEnabled = enabled;
            copyToolStripMenuItem.IsEnabled = enabled;
            cutToolStripMenuItem.IsEnabled = enabled;
            duplicateToolStripMenuItem.IsEnabled = enabled;


            flipHorizontalToolStripMenuItem.IsEnabled = enabled && CanFlip(0);
            flipVerticalToolStripMenuItem.IsEnabled = enabled && CanFlip(0);
            flipHorizontalIndvidualToolStripMenuItem.IsEnabled = enabled && CanFlip(1);
            flipVerticalIndvidualToolStripMenuItem.IsEnabled = enabled && CanFlip(1);

            selectAllToolStripMenuItem.IsEnabled = (ManiacEditor.Methods.Solution.SolutionState.Main.IsTilesEdit() && !ManiacEditor.Methods.Solution.SolutionState.Main.IsChunksEdit()) || ManiacEditor.Methods.Solution.SolutionState.Main.IsEntitiesEdit();

            bool CanFlip(int option)
            {
                switch (option)
                {
                    case 0:
                        if (ManiacEditor.Methods.Solution.SolutionState.Main.IsEntitiesEdit() && ManiacEditor.Methods.Solution.SolutionState.Main.IsSelected()) return true;
                        else if (ManiacEditor.Methods.Solution.SolutionState.Main.IsTilesEdit()) return true;
                        break;
                    case 1:
                        return ManiacEditor.Methods.Solution.SolutionState.Main.IsTilesEdit();
                }
                return false;
            }
        }

        #endregion

        #region File Events
        private void NewSceneEvent(object sender, RoutedEventArgs e) 
        { 
            ManiacEditor.Methods.Solution.SolutionLoader.NewScene(); 
        }

        public void OpenSceneEvent(object sender, RoutedEventArgs e) 
        { 
            ManiacEditor.Methods.Solution.SolutionLoader.OpenScene(); 
        }

        private void OpenSceneSelectEvent(object sender, RoutedEventArgs e) 
        { 
            ManiacEditor.Methods.Solution.SolutionLoader.OpenSceneSelect();
        }

        public void OpenDataDirectoryEvent(object sender, RoutedEventArgs e) 
        { 
            ManiacEditor.Methods.Solution.SolutionLoader.OpenDataDirectory(); 
        }

        public void SaveSceneEvent(object sender, RoutedEventArgs e) 
        { 
            ManiacEditor.Methods.Solution.SolutionLoader.Save(); 
        }

        private void ExitEditorEvent(object sender, RoutedEventArgs e) 
        { 
            Instance.Close(); 
        }

        private void ExportToolStripMenuItem_Click(object sender, RoutedEventArgs e) 
        { 
            Methods.ProgramLauncher.ExportGUI(sender, e); 
        }

        public void SaveSceneAsEvent(object sender, RoutedEventArgs e) 
        { 
            ManiacEditor.Methods.Solution.SolutionLoader.SaveAs(); 
        }

        public void UnloadSceneEvent(object sender, RoutedEventArgs e) 
        { 
            ManiacEditor.Methods.Solution.SolutionLoader.UnloadScene(); 
        }

        private void BackupStageConfigMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ManiacEditor.Methods.Solution.SolutionLoader.BackupStageConfig();
        }

        private void BackupTileConfigMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ManiacEditor.Methods.Solution.SolutionLoader.BackupTileConfig();
        }

        private void BackupSceneMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ManiacEditor.Methods.Solution.SolutionLoader.BackupSceneFile();
        }

        private void BackupStageTilesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ManiacEditor.Methods.Solution.SolutionLoader.BackupStageTiles();
        }

        private void RecentScenes_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            Classes.Prefrences.RecentsRefrenceState.RefreshRecentScenes();
        }

        private void RecentDataSources_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            Classes.Prefrences.RecentsRefrenceState.RefreshDataSources();
        }

        #endregion

        #region Edit Events
        private void UndoEvent(object sender, RoutedEventArgs e)
        {
            Actions.UndoRedoModel.Undo();
        }
        private void RedoEvent(object sender, RoutedEventArgs e)
        {
            Actions.UndoRedoModel.Redo();
        }
        public void PasteToChunksEvent(object sender, RoutedEventArgs e) 
        { 
            Methods.Solution.SolutionActions.PasteToChunks(); 
        }
        public void SelectAllEvent(object sender, RoutedEventArgs e) 
        { 
            Methods.Solution.SolutionActions.SelectAll(); 
        }
        public void CutEvent(object sender, RoutedEventArgs e) 
        { 
            Methods.Solution.SolutionActions.Cut(); 
        }
        public void CopyEvent(object sender, RoutedEventArgs e) 
        { 
            Methods.Solution.SolutionActions.Copy(); 
        }
        public void PasteEvent(object sender, RoutedEventArgs e) 
        { 
            Methods.Solution.SolutionActions.Paste(); 
        }
        public void DuplicateEvent(object sender, RoutedEventArgs e) 
        { 
            Methods.Solution.SolutionActions.Duplicate(); 
        }
        private void DeleteEvent(object sender, RoutedEventArgs e) 
        { 
            Methods.Solution.SolutionActions.Delete(); 
        }
        public void FlipVerticalEvent(object sender, RoutedEventArgs e) 
        { 
            Methods.Solution.SolutionActions.FlipVertical(); 
        }
        public void FlipHorizontalEvent(object sender, RoutedEventArgs e) 
        { 
            Methods.Solution.SolutionActions.FlipHorizontal(); 
        }
        public void FlipVerticalIndividualEvent(object sender, RoutedEventArgs e) 
        { 
            Methods.Solution.SolutionActions.FlipVerticalIndividual(); 
        }
        public void FlipHorizontalIndividualEvent(object sender, RoutedEventArgs e) 
        { 
            Methods.Solution.SolutionActions.FlipHorizontalIndividual(); 
        }

        private void ClipboardManagerMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Methods.ProgramLauncher.ClipboardManager();
        }
        #endregion

        #region Tool Events

        public void GoToPositionEvent(object sender, RoutedEventArgs e) 
        { 
            ManiacEditor.Methods.Solution.SolutionActions.GoToPosition(sender, e); 
        }

        private void FindAndReplaceToolEvent(object sender, RoutedEventArgs e) 
        { 
            ManiacEditor.Methods.Solution.SolutionActions.FindAndReplaceTool(sender, e);
        }

        private void OffsetTileIndexesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ManiacEditor.Methods.Solution.SolutionActions.OffsetTileIndexesTool();
        }

        private void FindUnusedTiles(object sender, RoutedEventArgs e) 
        { 
            Methods.Layers.TileFindReplace.FindUnusedTiles(); 
        }

        #endregion

        #region View Events
        private void UnlockCameraToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Methods.Solution.SolutionState.Main.UnlockCamera ^= true;
        }

        private void TogglePixelModeEvent(object sender, RoutedEventArgs e)
        {
            Methods.Solution.SolutionState.Main.CountTilesSelectedInPixels ^= true;
        }

        public void ToggleScrollLockEvent(object sender, RoutedEventArgs e)
        {
            Methods.Solution.SolutionState.Main.ScrollLocked ^= true;
        }

        public void ToggleFasterNudgeEvent(object sender, RoutedEventArgs e)
        {
            Methods.Solution.SolutionState.Main.EnableFasterNudge ^= true;
        }

        public void ShowDebugStatsEvent(object sender, RoutedEventArgs e) 
        {
            Methods.Solution.SolutionState.Main.DebugStatsVisibleOnPanel ^= true; 
        }

        private void ResetZoomLevelEvent(object sender, RoutedEventArgs e) 
        { 
            Instance.ViewPanel.SharpPanel.UpdateZoomLevel(0, new System.Drawing.Point(0, 0)); 
        }

        private void SetScrollDirectionEvent(object sender, RoutedEventArgs e) 
        { 
            ManiacEditor.Methods.Solution.SolutionActions.SetScrollLockDirection(); 
        }

        private void OverlayImageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Methods.Drawing.CommonDrawing.SelectOverlayImage(Instance.ViewPanel.SharpPanel.GraphicPanel._device);
        }

        private void ClearImageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Methods.Drawing.CommonDrawing.ClearOverlayImage();
        }

        #endregion

        #region Scene Events

        private void SetEncorePalleteEvent(object sender, RoutedEventArgs e) 
        { 
            ManiacEditor.Methods.Solution.SolutionActions.SetEncorePallete(sender); 
        }
        private void LayerManagerEvent(object sender, RoutedEventArgs e) 
        { 
            Methods.ProgramLauncher.LayerManager(sender, e); 
        }

        private void TileManagerEvent(object sender, RoutedEventArgs e)
        {
            Methods.ProgramLauncher.TileManager();
        }

        private void TileManiacNormal(object sender, RoutedEventArgs e)
        {
            Methods.ProgramLauncher.TileManiacNormal();
        }

        public void ImportSoundsEvent(object sender, RoutedEventArgs e) 
        { 
            Methods.ProgramLauncher.ImportSounds(sender, e); 
        }

        private void ManiacINIEditorEvent(object sender, RoutedEventArgs e) 
        { 
            Methods.ProgramLauncher.ManiacINIEditor(sender, e); 
        }

        private void ChangeEditorBGColorEvent(object sender, RoutedEventArgs e)
        {
            Methods.ProgramLauncher.ChangeEditorBackgroundColors();
        }

        public void ObjectManagerEvent(object sender, RoutedEventArgs e) 
        { 
            Methods.ProgramLauncher.ObjectManager(); 
        }

        private void ChangeLevelIDEvent(object sender, RoutedEventArgs e) 
        { 
            ManiacEditor.Methods.Solution.SolutionActions.ChangeLevelID(sender, e); 
        }
        #endregion

        #region Layer Events

        private void MoveExtraLayersToFrontEvent(object sender, RoutedEventArgs e) 
        { 
            Methods.Solution.SolutionState.Main.ExtraLayersMoveToFront ^= true; 
        }

        private void ToggleCopyAirEvent(object sender, RoutedEventArgs e) 
        { 
            Methods.Solution.SolutionState.Main.CopyAir ^= true; 
        }

        #endregion

        #region Entities Events
        public void ApplyEditEntitiesTransparencyEvent(object sender, RoutedEventArgs e)
        {
            Methods.Solution.SolutionState.Main.ApplyEditEntitiesTransparency ^= true;
        }
        public void MenuButtonChangedEvent(object sender, RoutedEventArgs e)
        {
            ManiacEditor.Methods.Solution.SolutionActions.SetManiaMenuInputType(sender, e);
        }
        private void ShowEntitiesAboveAllOtherLayersToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Methods.Solution.SolutionState.Main.EntitiesVisibileAboveAllLayers ^= true;
        }

        private void EntityFilterTextChangedEvent(object sender, TextChangedEventArgs e)
        {
            ManiacEditor.Methods.Solution.SolutionActions.UpdateEntityFilterFromTextBox(sender, e);
        }
        private void ShowEntitySelectionBoxesEvent(object sender, RoutedEventArgs e)
        {
            Methods.Solution.SolutionState.Main.ShowEntitySelectionBoxes ^= true;
        }
        private void ShowWaterLevelEvent(object sender, RoutedEventArgs e)
        {
            Methods.Solution.SolutionState.Main.ShowWaterLevel ^= true;
        }
        private void AlwaysShowWaterLevelEvent(object sender, RoutedEventArgs e)
        {
            Methods.Solution.SolutionState.Main.AlwaysShowWaterLevel ^= true;
        }
        private void WaterSizeWithBoundsEvent(object sender, RoutedEventArgs e)
        {
            Methods.Solution.SolutionState.Main.SizeWaterLevelwithBounds ^= true;
        }
        private void SwapEncoreManiaEntityVisibilityEvent(object sender, RoutedEventArgs e)
        {
            ManiacEditor.Methods.Solution.SolutionActions.SwapEncoreManiaEntityVisibility();
        }
        private void ShowParallaxSpritesEvent(object sender, RoutedEventArgs e)
        {
            Methods.Solution.SolutionState.Main.ShowParallaxSprites ^= true;
        }

        private void ShowEntityPathArrowsEvent(object sender, RoutedEventArgs e)
        {
            Methods.Solution.SolutionState.Main.ShowEntityPathArrows ^= true;
        }
        private void MenuLanguageChangedEvent(object sender, RoutedEventArgs e)
        {
            ManiacEditor.Methods.Solution.SolutionActions.ManiaMenuLanguageChanged(sender, e);
        }

        #endregion

        #region App Events
        private void SonicManiaHeadless(object sender, RoutedEventArgs e) 
        { 
            Methods.ProgramLauncher.SonicManiaHeadless(); 
        }
        private void MenuAppsCheatEngine_Click(object sender, RoutedEventArgs e) 
        { 
            Methods.ProgramLauncher.CheatEngine(); 
        }
        private void ModManager(object sender, RoutedEventArgs e) 
        { 
            Methods.ProgramLauncher.ManiaModManager(); 
        }
        private void InsanicManiacToolStripMenuItem_Click(object sender, RoutedEventArgs e) 
        { 

        }
        private void RSDKAnnimationEditorToolStripMenuItem_Click(object sender, RoutedEventArgs e) 
        { 
            Methods.ProgramLauncher.RSDKAnimationEditor(); 
        }
        private void ColorPaletteEditorToolStripMenuItem_Click(object sender, RoutedEventArgs e) 
        { 
            Methods.ProgramLauncher.ManiaPal(sender, e); 
        }
        private void ManiaPalMenuItem_SubmenuOpened(object sender, RoutedEventArgs e) 
        { 
            Methods.ProgramLauncher.ManiaPalSubmenuOpened(sender, e); 
        }
        #endregion

        #region Folder Events
        private void OpenSceneFolderToolStripMenuItem_Click(object sender, RoutedEventArgs e) 
        { 
            Methods.ProgramLauncher.OpenSceneFolder(); 
        }
        private void OpenManiacEditorFolderToolStripMenuItem_Click(object sender, RoutedEventArgs e) 
        { 
            Methods.ProgramLauncher.OpenManiacEditorFolder();
        }
        private void OpenManiacEditorFixedSettingsFolderToolStripMenuItem_Click(object sender, RoutedEventArgs e) 
        { 
            Methods.ProgramLauncher.OpenManiacEditorFixedSettingsFolder(); 
        }
        private void OpenManiacEditorPortableSettingsFolderToolStripMenuItem_Click(object sender, RoutedEventArgs e) 
        { 
            Methods.ProgramLauncher.OpenManiacEditorPortableSettingsFolder(); 
        }
        private void OpenDataDirectoryFolderToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        { 
            Methods.ProgramLauncher.OpenDataDirectory(); 
        }
        private void openMasterDataDirectoryFolderToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Methods.ProgramLauncher.OpenMasterDataDirectory();
        }
        private void OpenTilesGIFMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Methods.ProgramLauncher.OpenStageTiles();
        }
        private void OpenSonicManiaFolderToolStripMenuItem_Click(object sender, RoutedEventArgs e) 
        {
            Methods.ProgramLauncher.OpenSonicManiaFolder(); 
        }
        private void OpenASavedPlaceToolStripMenuItem_DropDownOpening(object sender, RoutedEventArgs e)
        { 
            Methods.ProgramLauncher.OpenASavedPlaceDropDownOpening(sender, e);
        }
        private void OpenASavedPlaceToolStripMenuItem_DropDownClosed(object sender, RoutedEventArgs e) 
        {
            Methods.ProgramLauncher.OpenASavedPlaceDropDownClosed(sender, e);
        }
        #endregion

        #region Other Events

        public void AboutScreenEvent(object sender, RoutedEventArgs e) 
        { 
            Methods.ProgramLauncher.AboutScreen(); 
        }

        private void WikiLinkEvent(object sender, RoutedEventArgs e) 
        { 
            Methods.ProgramLauncher.WikiLink(); 
        }

        public void OptionsMenuEvent(object sender, RoutedEventArgs e) 
        { 
            Methods.ProgramLauncher.OptionsMenu();
        }

        private void ControlsMenuEvent(object sender, RoutedEventArgs e) 
        { 
            Methods.ProgramLauncher.ControlMenu(); 
        }

        #endregion

        #region Developer Only Events
        private void DeveloperTerminalEvent(object sender, RoutedEventArgs e)
        {
            Methods.ProgramLauncher.DevTerm();
        }
        private void MD5GeneratorToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ManiacEditor.Methods.Solution.SolutionActions.MD5GeneratorToolStripMenuItem_Click(sender, e);
        }
        private void ConsoleWindowToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ManiacEditor.Methods.Solution.SolutionActions.ConsoleWindowToolStripMenuItem_Click(sender, e);
        }
        private void SaveForForceOpenOnStartupToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ManiacEditor.Methods.Solution.SolutionActions.SaveForForceOpenOnStartupToolStripMenuItem_Click(sender, e);
        }
        private void LeftToolbarToggleDev_Click(object sender, RoutedEventArgs e)
        {
            if (Instance != null) Instance.ViewPanel.SplitContainer.UpdateToolbars(false, true);
        }
        private void RightToolbarToggleDev_Click(object sender, RoutedEventArgs e)
        {
            if (Instance != null) Instance.ViewPanel.SplitContainer.UpdateToolbars(true, true);
        }
        private void EnableAllButtonsToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ManiacEditor.Methods.Solution.SolutionActions.EnableAllButtonsToolStripMenuItem_Click(sender, e);
        }
        private void TestingDeveloperMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }










        #endregion


    }
}
