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

namespace ManiacEditor.Controls.Editor_Elements
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
            }));

        }

        public void UpdateMenuItems()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                newToolStripMenuItem.InputGestureText = KeyBindPraser("New");
                openToolStripMenuItem.InputGestureText = KeyBindPraser("Open");
                openDataDirectoryToolStripMenuItem.InputGestureText = KeyBindPraser("OpenDataDir");
                saveToolStripMenuItem.InputGestureText = KeyBindPraser("_Save");
                saveAsToolStripMenuItem.InputGestureText = KeyBindPraser("SaveAs");
                undoToolStripMenuItem.InputGestureText = KeyBindPraser("Undo");
                redoToolStripMenuItem.InputGestureText = KeyBindPraser("Redo");
                cutToolStripMenuItem.InputGestureText = KeyBindPraser("Cut");
                copyToolStripMenuItem.InputGestureText = KeyBindPraser("Copy");
                pasteToolStripMenuItem.InputGestureText = KeyBindPraser("Paste");
                duplicateToolStripMenuItem.InputGestureText = KeyBindPraser("Duplicate");
                selectAllToolStripMenuItem.InputGestureText = KeyBindPraser("SelectAll");
                deleteToolStripMenuItem.InputGestureText = KeyBindPraser("Delete");
                statusNAToolStripMenuItem.InputGestureText = KeyBindPraser("ScrollLock");
                nudgeSelectionFasterToolStripMenuItem.InputGestureText = KeyBindPraser("NudgeFaster", false, true);
                swapScrollLockDirMenuToolstripButton.InputGestureText = KeyBindPraser("ScrollLockTypeSwitch", false, true);
                resetZoomLevelToolstripMenuItem.InputGestureText = KeyBindPraser("ResetZoomLevel");
                unloadSceneToolStripMenuItem.InputGestureText = KeyBindPraser("UnloadScene", false, true);
                flipVerticalIndvidualToolStripMenuItem.InputGestureText = KeyBindPraser("FlipVIndv");
                flipHorizontalIndvidualToolStripMenuItem.InputGestureText = KeyBindPraser("FlipHIndv");
                flipHorizontalToolStripMenuItem.InputGestureText = KeyBindPraser("FlipH");
                flipVerticalToolStripMenuItem.InputGestureText = KeyBindPraser("FlipV");
                pasteTochunkToolStripMenuItem.InputGestureText = KeyBindPraser("PasteToChunk", false, true);
                developerInterfaceToolStripMenuItem.InputGestureText = KeyBindPraser("DeveloperInterface", false, true);
                saveForForceOpenOnStartupToolStripMenuItem.InputGestureText = KeyBindPraser("ForceOpenOnStartup", false, true);
                copyAirToggle.InputGestureText = KeyBindPraser("CopyAirTiles", false, true);
            }));

        }

        public string KeyBindPraser(string keyRefrence, bool tooltip = false, bool nonRequiredBinding = false)
        {
            string nullString = (nonRequiredBinding ? "" : "N/A");
            if (nonRequiredBinding && tooltip) nullString = "None";
            List<string> keyBindList = new List<string>();
            List<string> keyBindModList = new List<string>();

            if (!Extensions.Extensions.KeyBindsSettingExists(keyRefrence)) return nullString;

            if (Properties.Settings.MyKeyBinds == null) return nullString;

            var keybindDict = Properties.Settings.MyKeyBinds.GetInput(keyRefrence) as List<string>;
            if (keybindDict != null)
            {
                keyBindList = keybindDict.Cast<string>().ToList();
            }
            else
            {
                return nullString;
            }

            if (keyBindList == null)
            {
                return nullString;
            }

            if (keyBindList.Count > 1)
            {
                string keyBindLister = "";
                foreach (string key in keyBindList)
                {
                    keyBindLister += String.Format("({0}) ", key);
                }
                if (tooltip) return String.Format(" ({0})", keyBindLister);
                else return keyBindLister;
            }
            else if ((keyBindList.Count == 1) && keyBindList[0] != "None")
            {
                if (tooltip) return String.Format(" ({0})", keyBindList[0]);
                else return keyBindList[0];
            }
            else
            {
                return nullString;
            }


        }

        public void SetEditButtonsState(bool enabled)
        {
            entityManagerToolStripMenuItem.IsEnabled = enabled && Methods.Solution.CurrentSolution.StageConfig != null;
            importSoundsToolStripMenuItem.IsEnabled = enabled && Methods.Solution.CurrentSolution.StageConfig != null;
            layerManagerToolStripMenuItem.IsEnabled = enabled;
            editBackgroundColorsToolStripMenuItem.IsEnabled = enabled;

            undoToolStripMenuItem.IsEnabled = enabled && Actions.UndoRedoModel.UndoStack.Count > 0;
            redoToolStripMenuItem.IsEnabled = enabled && Actions.UndoRedoModel.RedoStack.Count > 0;

            //findAndReplaceToolStripMenuItem.IsEnabled = enabled && Methods.Editor.Solution.EditLayerA != null;
        }

        public void SetPasteButtonsState(bool enabled)
        {
            bool windowsClipboardState;
            bool windowsEntityClipboardState;
            //Doing this too often seems to cause a lot of grief for the app, should be relocated and stored as a bool
            try
            {
                if (ManiacEditor.Methods.Solution.SolutionState.IsTilesEdit()) windowsClipboardState = Clipboard.ContainsData("ManiacTiles");
                else windowsClipboardState = false;
                if (ManiacEditor.Methods.Solution.SolutionState.IsEntitiesEdit()) windowsEntityClipboardState = Clipboard.ContainsData("ManiacEntities");
                else windowsEntityClipboardState = false;
            }
            catch
            {
                windowsClipboardState = false;
                windowsEntityClipboardState = false;
            }


            if (ManiacEditor.Methods.Solution.SolutionState.IsTilesEdit())
            {
                if (enabled && HasCopyDataTiles()) SetPasteEnabledButtons(true);
                else SetPasteEnabledButtons(false);
            }
            else if (ManiacEditor.Methods.Solution.SolutionState.IsEntitiesEdit())
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
                pasteTochunkToolStripMenuItem.IsEnabled = pasteEnabled && ManiacEditor.Methods.Solution.SolutionState.IsTilesEdit();
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

            selectAllToolStripMenuItem.IsEnabled = (ManiacEditor.Methods.Solution.SolutionState.IsTilesEdit() && !ManiacEditor.Methods.Solution.SolutionState.IsChunksEdit()) || ManiacEditor.Methods.Solution.SolutionState.IsEntitiesEdit();

            bool CanFlip(int option)
            {
                switch (option)
                {
                    case 0:
                        if (ManiacEditor.Methods.Solution.SolutionState.IsEntitiesEdit() && ManiacEditor.Methods.Solution.SolutionState.IsSelected()) return true;
                        else if (ManiacEditor.Methods.Solution.SolutionState.IsTilesEdit()) return true;
                        break;
                    case 1:
                        return ManiacEditor.Methods.Solution.SolutionState.IsTilesEdit();
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
            Methods.Solution.SolutionState.UnlockCamera ^= true;
        }

        private void TogglePixelModeEvent(object sender, RoutedEventArgs e)
        {
            Methods.Solution.SolutionState.CountTilesSelectedInPixels ^= true;
        }

        public void ToggleScrollLockEvent(object sender, RoutedEventArgs e)
        {
            Methods.Solution.SolutionState.ScrollLocked ^= true;
        }

        public void ToggleFasterNudgeEvent(object sender, RoutedEventArgs e)
        {
            Methods.Solution.SolutionState.EnableFasterNudge ^= true;
        }

        public void ShowDebugStatsEvent(object sender, RoutedEventArgs e) 
        {
            Methods.Solution.SolutionState.DebugStatsVisibleOnPanel ^= true; 
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
            Instance.ViewPanel.SharpPanel.SelectOverlayImage();
        }

        private void ClearImageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Instance.ViewPanel.SharpPanel.ClearOverlayImage();
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

        private void TileManiacNormal(object sender, RoutedEventArgs e)
        {
            Methods.ProgramLauncher.TileManiacNormal();
        }

        public void ImportSoundsEvent(object sender, RoutedEventArgs e) 
        { 
            Methods.ProgramLauncher.ImportSoundsToolStripMenuItem_Click(sender, e); 
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
            Methods.Solution.SolutionState.ExtraLayersMoveToFront ^= true; 
        }

        private void ToggleCopyAirEvent(object sender, RoutedEventArgs e) 
        { 
            Methods.Solution.SolutionState.CopyAir ^= true; 
        }

        #endregion

        #region Entities Events
        public void ApplyEditEntitiesTransparencyEvent(object sender, RoutedEventArgs e)
        {
            Methods.Solution.SolutionState.ApplyEditEntitiesTransparency ^= true;
        }
        public void MenuButtonChangedEvent(object sender, RoutedEventArgs e)
        {
            ManiacEditor.Methods.Solution.SolutionActions.SetManiaMenuInputType(sender, e);
        }
        private void ShowEntitiesAboveAllOtherLayersToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Methods.Solution.SolutionState.EntitiesVisibileAboveAllLayers ^= true;
        }

        private void EntityFilterTextChangedEvent(object sender, TextChangedEventArgs e)
        {
            ManiacEditor.Methods.Solution.SolutionActions.UpdateEntityFilterFromTextBox(sender, e);
        }
        private void ShowEntitySelectionBoxesEvent(object sender, RoutedEventArgs e)
        {
            Methods.Solution.SolutionState.ShowEntitySelectionBoxes ^= true;
        }
        private void ShowWaterLevelEvent(object sender, RoutedEventArgs e)
        {
            Methods.Solution.SolutionState.ShowWaterLevel ^= true;
        }
        private void AlwaysShowWaterLevelEvent(object sender, RoutedEventArgs e)
        {
            Methods.Solution.SolutionState.AlwaysShowWaterLevel ^= true;
        }
        private void WaterSizeWithBoundsEvent(object sender, RoutedEventArgs e)
        {
            Methods.Solution.SolutionState.SizeWaterLevelwithBounds ^= true;
        }
        private void SwapEncoreManiaEntityVisibilityEvent(object sender, RoutedEventArgs e)
        {
            ManiacEditor.Methods.Solution.SolutionActions.SwapEncoreManiaEntityVisibility();
        }
        private void ShowParallaxSpritesEvent(object sender, RoutedEventArgs e)
        {
            Methods.Solution.SolutionState.ShowParallaxSprites ^= true;
        }

        private void ShowEntityPathArrowsEvent(object sender, RoutedEventArgs e)
        {
            Methods.Solution.SolutionState.ShowEntityPathArrows ^= true;
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
        private void SoundLooperToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ManiacEditor.Methods.Solution.SolutionActions.SoundLooperToolStripMenuItem_Click(sender, e);
        }
        private void TestingDeveloperMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }










        #endregion
    }
}
