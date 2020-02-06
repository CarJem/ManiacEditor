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

namespace ManiacEditor.Controls.Base.Elements
{
    /// <summary>
    /// Interaction logic for MenuBar.xaml
    /// </summary>
    public partial class Menu : UserControl
    {
        public Menu()
        {
            InitializeComponent();
        }

        private MainEditor Instance { get; set; }

        public void UpdateInstance(MainEditor instance)
        {
            Instance = instance;
        }

        public void SetSceneOnlyButtonsState(bool enabled, bool stageLoad = false)
        {
            saveToolStripMenuItem.IsEnabled = enabled;
            saveAsToolStripMenuItem.IsEnabled = enabled;
            backupToolStripMenuItem.IsEnabled = enabled;
            unloadSceneToolStripMenuItem.IsEnabled = enabled;
            goToToolStripMenuItem.IsEnabled = enabled;
            findUnusedTilesToolStripMenuItem.IsEnabled = enabled;
            maniacinieditorToolStripMenuItem.IsEnabled = enabled;
            exportToolStripMenuItem.IsEnabled = enabled;

            newShortcutToolStripMenuItem.IsEnabled = System.IO.Directory.Exists(ManiacEditor.Controls.Base.MainEditor.Instance.DataDirectory);
            withoutCurrentCoordinatesToolStripMenuItem.IsEnabled = Classes.Editor.Solution.CurrentScene != null;
            withCurrentCoordinatesToolStripMenuItem.IsEnabled = Classes.Editor.Solution.CurrentScene != null;
            changeEncorePaleteToolStripMenuItem.IsEnabled = enabled;
        }

        public void UpdateMenuItems()
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
            CopyAirLabel.Text = string.Format("Copy Air Tiles {1} ({0})", KeyBindPraser("CopyAirTiles", false, true), Environment.NewLine);
        }

        public string KeyBindPraser(string keyRefrence, bool tooltip = false, bool nonRequiredBinding = false)
        {
            string nullString = (nonRequiredBinding ? "" : "N/A");
            if (nonRequiredBinding && tooltip) nullString = "None";
            List<string> keyBindList = new List<string>();
            List<string> keyBindModList = new List<string>();

            if (!Extensions.Extensions.KeyBindsSettingExists(keyRefrence)) return nullString;

            if (Properties.KeyBinds.Default == null) return nullString;

            var keybindDict = Properties.KeyBinds.Default[keyRefrence] as StringCollection;
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
            entityManagerToolStripMenuItem.IsEnabled = enabled && Classes.Editor.Solution.StageConfig != null;
            importSoundsToolStripMenuItem.IsEnabled = enabled && Classes.Editor.Solution.StageConfig != null;
            layerManagerToolStripMenuItem.IsEnabled = enabled;
            editBackgroundColorsToolStripMenuItem.IsEnabled = enabled;

            undoToolStripMenuItem.IsEnabled = enabled && ManiacEditor.Controls.Base.MainEditor.Instance.UndoStack.Count > 0;
            redoToolStripMenuItem.IsEnabled = enabled && ManiacEditor.Controls.Base.MainEditor.Instance.RedoStack.Count > 0;

            findAndReplaceToolStripMenuItem.IsEnabled = enabled && Classes.Editor.Solution.EditLayerA != null;

            if (Classes.Editor.Solution.Entities != null && Classes.Editor.Solution.Entities.SelectedEntities != null && Classes.Editor.Solution.Entities.SelectedEntities.Count > 1)
            {
                SortSelectedSlotIDs.IsEnabled = true;
                SortSelectedSlotIDsOptimized.IsEnabled = true;
                SortSelectedSlotIDsOrdered.IsEnabled = true;
            }
            else
            {
                SortSelectedSlotIDs.IsEnabled = false;
                SortSelectedSlotIDsOptimized.IsEnabled = false;
                SortSelectedSlotIDsOrdered.IsEnabled = false;
            }
        }

        public void SetPasteButtonsState(bool enabled)
        {
            bool windowsClipboardState;
            bool windowsEntityClipboardState;
            //Doing this too often seems to cause a lot of grief for the app, should be relocated and stored as a bool
            try
            {
                if (ManiacEditor.Classes.Editor.SolutionState.IsTilesEdit()) windowsClipboardState = Clipboard.ContainsData("ManiacTiles");
                else windowsClipboardState = false;
                if (ManiacEditor.Classes.Editor.SolutionState.IsEntitiesEdit()) windowsEntityClipboardState = Clipboard.ContainsData("ManiacEntities");
                else windowsEntityClipboardState = false;
            }
            catch
            {
                windowsClipboardState = false;
                windowsEntityClipboardState = false;
            }


            if (ManiacEditor.Classes.Editor.SolutionState.IsTilesEdit())
            {
                if (enabled && HasCopyDataTiles()) SetPasteEnabledButtons(true);
                else SetPasteEnabledButtons(false);
            }
            else if (ManiacEditor.Classes.Editor.SolutionState.IsEntitiesEdit())
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
                pasteToToolStripMenuItem.IsEnabled = pasteEnabled;
                pasteTochunkToolStripMenuItem.IsEnabled = pasteEnabled && ManiacEditor.Classes.Editor.SolutionState.IsTilesEdit();
            }

            bool HasCopyDataTiles() { return ManiacEditor.Controls.Base.MainEditor.Instance.TilesClipboard != null || windowsClipboardState == true; }
            bool HasCopyDataEntities() { return ManiacEditor.Controls.Base.MainEditor.Instance.entitiesClipboard != null || windowsEntityClipboardState == true; }
        }

        public void SetSelectOnlyButtonsState(bool enabled = true)
        {
            enabled &= ManiacEditor.Classes.Editor.SolutionState.IsSelected();
            deleteToolStripMenuItem.IsEnabled = enabled;
            copyToolStripMenuItem.IsEnabled = enabled;
            cutToolStripMenuItem.IsEnabled = enabled;
            duplicateToolStripMenuItem.IsEnabled = enabled;


            flipHorizontalToolStripMenuItem.IsEnabled = enabled && CanFlip(0);
            flipVerticalToolStripMenuItem.IsEnabled = enabled && CanFlip(0);
            flipHorizontalIndvidualToolStripMenuItem.IsEnabled = enabled && CanFlip(1);
            flipVerticalIndvidualToolStripMenuItem.IsEnabled = enabled && CanFlip(1);

            selectAllToolStripMenuItem.IsEnabled = (ManiacEditor.Classes.Editor.SolutionState.IsTilesEdit() && !ManiacEditor.Classes.Editor.SolutionState.IsChunksEdit()) || ManiacEditor.Classes.Editor.SolutionState.IsEntitiesEdit();

            bool CanFlip(int option)
            {
                switch (option)
                {
                    case 0:
                        if (ManiacEditor.Classes.Editor.SolutionState.IsEntitiesEdit() && ManiacEditor.Classes.Editor.SolutionState.IsSelected()) return true;
                        else if (ManiacEditor.Classes.Editor.SolutionState.IsTilesEdit()) return true;
                        break;
                    case 1:
                        return ManiacEditor.Classes.Editor.SolutionState.IsTilesEdit();
                }
                return false;
            }
        }

        #region Action Events (MenuItems, Clicks, etc.)
        #region File Events
        private void NewSceneEvent(object sender, RoutedEventArgs e) { ManiacEditor.Classes.Editor.SolutionLoader.NewScene(); }
        public void OpenSceneEvent(object sender, RoutedEventArgs e) { ManiacEditor.Classes.Editor.SolutionLoader.OpenScene(); }
        public void OpenDataDirectoryEvent(object sender, RoutedEventArgs e) { ManiacEditor.Classes.Editor.SolutionLoader.OpenDataDirectory(); }
        public void SaveSceneEvent(object sender, RoutedEventArgs e) { ManiacEditor.Classes.Editor.SolutionLoader.Save(); }
        private void ExitEditorEvent(object sender, RoutedEventArgs e) { ManiacEditor.Controls.Base.MainEditor.Instance.Close(); }
        private void ExportAsPNGEvent(object sender, RoutedEventArgs e) { ManiacEditor.Classes.Editor.SolutionLoader.ExportAsPNG(); }
        private void ExportLayersAsPNGEvent(object sender, RoutedEventArgs e) { ManiacEditor.Classes.Editor.SolutionLoader.ExportLayersAsPNG(); }
        private void ExportObjLayoutAsPNGEvent(object sender, RoutedEventArgs e) { ManiacEditor.Classes.Editor.SolutionLoader.ExportObjLayoutAsPNG(); }
        private void ExportToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.ExportGUI(sender, e); }
        public void SaveSceneAsEvent(object sender, RoutedEventArgs e) { ManiacEditor.Classes.Editor.SolutionLoader.SaveAs(); }
        private void RecoverEvent(object sender, RoutedEventArgs e) { ManiacEditor.Classes.Editor.SolutionLoader.BackupRecoverButton_Click(sender, e); }
        public void UnloadSceneEvent(object sender, RoutedEventArgs e) { ManiacEditor.Classes.Editor.SolutionLoader.UnloadScene(); }
        private void BackupStageConfigEvent(object sender, RoutedEventArgs e) { ManiacEditor.Classes.Editor.SolutionLoader.StageConfigBackup(sender, e); }
        private void BackupSceneEvent(object sender, RoutedEventArgs e) { ManiacEditor.Classes.Editor.SolutionLoader.SceneBackup(sender, e); }
        #endregion
        #region Edit Events
        public void PasteToChunksEvent(object sender, RoutedEventArgs e) { Classes.Editor.EditorActions.PasteToChunks(); }
        public void SelectAllEvent(object sender, RoutedEventArgs e) { Classes.Editor.EditorActions.SelectAll(); }
        public void CutEvent(object sender, RoutedEventArgs e) { Classes.Editor.EditorActions.Cut(); }
        public void CopyEvent(object sender, RoutedEventArgs e) { Classes.Editor.EditorActions.Copy(); }
        public void PasteEvent(object sender, RoutedEventArgs e) { Classes.Editor.EditorActions.Paste(); }
        public void DuplicateEvent(object sender, RoutedEventArgs e) { Classes.Editor.EditorActions.Duplicate(); }
        private void DeleteEvent(object sender, RoutedEventArgs e) { Classes.Editor.EditorActions.Delete(); }
        public void FlipVerticalEvent(object sender, RoutedEventArgs e) { Classes.Editor.EditorActions.FlipVertical(); }
        public void FlipHorizontalEvent(object sender, RoutedEventArgs e) { Classes.Editor.EditorActions.FlipHorizontal(); }
        public void FlipVerticalIndividualEvent(object sender, RoutedEventArgs e) { Classes.Editor.EditorActions.FlipVerticalIndividual(); }
        public void FlipHorizontalIndividualEvent(object sender, RoutedEventArgs e) { Classes.Editor.EditorActions.FlipHorizontalIndividual(); }
        #endregion
        #region Developer Stuff (WIP)
        private void DeveloperTerminalEvent(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.DevTerm(); }
        private void MD5GeneratorToolStripMenuItem_Click(object sender, RoutedEventArgs e) { ManiacEditor.Classes.Editor.EditorActions.MD5GeneratorToolStripMenuItem_Click(sender, e); }
        private void FindAndReplaceToolEvent(object sender, RoutedEventArgs e) { ManiacEditor.Classes.Editor.EditorActions.FindAndReplaceTool(sender, e); }
        private void ConsoleWindowToolStripMenuItem_Click(object sender, RoutedEventArgs e) { ManiacEditor.Classes.Editor.EditorActions.ConsoleWindowToolStripMenuItem_Click(sender, e); }
        private void SaveForForceOpenOnStartupToolStripMenuItem_Click(object sender, RoutedEventArgs e) { ManiacEditor.Classes.Editor.EditorActions.SaveForForceOpenOnStartupToolStripMenuItem_Click(sender, e); }
        private void LeftToolbarToggleDev_Click(object sender, RoutedEventArgs e) { if (Instance != null) Instance.ViewPanel.SplitContainer.UpdateToolbars(false, true); }
        private void RightToolbarToggleDev_Click(object sender, RoutedEventArgs e) { if (Instance != null) Instance.ViewPanel.SplitContainer.UpdateToolbars(true, true); }
        private void EnableAllButtonsToolStripMenuItem_Click(object sender, RoutedEventArgs e) { ManiacEditor.Classes.Editor.EditorActions.EnableAllButtonsToolStripMenuItem_Click(sender, e); }
        #endregion
        public void GoToPositionEvent(object sender, RoutedEventArgs e) { ManiacEditor.Classes.Editor.EditorActions.GoToPosition(sender, e); }
        private void UndoEvent(object sender, RoutedEventArgs e) { Classes.Editor.EditorActions.EditorUndo(); }
        private void RedoEvent(object sender, RoutedEventArgs e) { Classes.Editor.EditorActions.EditorRedo(); }
        private void TogglePixelModeEvent(object sender, RoutedEventArgs e) { Classes.Editor.SolutionState.CountTilesSelectedInPixels ^= true; }
        public void ToggleScrollLockEvent(object sender, RoutedEventArgs e) { Classes.Editor.SolutionState.ScrollLocked ^= true; }
        public void ToggleFasterNudgeEvent(object sender, RoutedEventArgs e) { Classes.Editor.SolutionState.EnableFasterNudge ^= true; }
        public void ApplyEditEntitiesTransparencyEvent(object sender, RoutedEventArgs e) { Classes.Editor.SolutionState.ApplyEditEntitiesTransparency ^= true; }
        public void ToggleDebugHUDEvent(object sender, RoutedEventArgs e) { Classes.Editor.SolutionState.DebugStatsVisibleOnPanel ^= true; }
        private void ResetZoomLevelEvent(object sender, RoutedEventArgs e) { ManiacEditor.Controls.Base.MainEditor.Instance.DeviceModel.SetZoomLevel(0, new System.Drawing.Point(0, 0)); }
        private void UseLargeDebugHUDText(object sender, RoutedEventArgs e) { Classes.Editor.SolutionState.UseLargeDebugStats ^= true; }
        public void MenuButtonChangedEvent(object sender, RoutedEventArgs e) { ManiacEditor.Classes.Editor.EditorActions.SetManiaMenuInputType(sender, e); }
        public void MenuButtonChangedEvent(string tag) { ManiacEditor.Classes.Editor.EditorActions.SetManiaMenuInputType(tag); }
        private void ShowEntitiesAboveAllOtherLayersToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Classes.Editor.SolutionState.EntitiesVisibileAboveAllLayers ^= true; }
        private void EntitySelectionBoxesAlwaysPrioritizedEvent(object sender, RoutedEventArgs e) { Classes.Editor.SolutionState.EntitySelectionBoxesAlwaysPrioritized ^= true; }
        private void PrioritizedEntityViewingEvent(object sender, RoutedEventArgs e) { Classes.Editor.SolutionState.PrioritizedEntityViewing ^= true; }
        private void SetEncorePalleteEvent(object sender, RoutedEventArgs e) { ManiacEditor.Classes.Editor.EditorActions.SetEncorePallete(sender); }
        private void MoveExtraLayersToFrontEvent(object sender, RoutedEventArgs e) { Classes.Editor.SolutionState.ExtraLayersMoveToFront ^= true; }
        private void EntityFilterTextChangedEvent(object sender, TextChangedEventArgs e) { ManiacEditor.Classes.Editor.EditorActions.UpdateEntityFilterFromTextBox(sender, e); }
        private void ShowEntitySelectionBoxesEvent(object sender, RoutedEventArgs e) { Classes.Editor.SolutionState.ShowEntitySelectionBoxes ^= true; }
        private void ShowWaterLevelEvent(object sender, RoutedEventArgs e) { Classes.Editor.SolutionState.ShowWaterLevel ^= true; }
        private void AlwaysShowWaterLevelEvent(object sender, RoutedEventArgs e) { Classes.Editor.SolutionState.AlwaysShowWaterLevel ^= true; }
        private void SortSelectedSlotIDsEvent(object sender, RoutedEventArgs e) { Classes.Editor.Solution.Entities.OrderSelectedSlotIDs(); }
        private void SortSelectedSlotIDsOptimizedEvent(object sender, RoutedEventArgs e) { Classes.Editor.Solution.Entities.OrderSelectedSlotIDs(true); }
        private void SortSelectedSlotIDsOrderedEvent(object sender, RoutedEventArgs e) { Classes.Editor.Solution.Entities.OrderSelectedSlotIDs(false, true); }
        private void WaterSizeWithBoundsEvent(object sender, RoutedEventArgs e) { Classes.Editor.SolutionState.SizeWaterLevelwithBounds ^= true; }
        private void SwapEncoreManiaEntityVisibilityEvent(object sender, RoutedEventArgs e) { ManiacEditor.Classes.Editor.EditorActions.SwapEncoreManiaEntityVisibility(); }
        private void ShowParallaxSpritesEvent(object sender, RoutedEventArgs e) { Classes.Editor.SolutionState.ShowParallaxSprites ^= true; }
        private void SetScrollDirectionEvent(object sender, RoutedEventArgs e) { ManiacEditor.Classes.Editor.EditorActions.SetScrollLockDirection(); }
        private void ShowEntityPathArrowsEvent(object sender, RoutedEventArgs e) { Classes.Editor.SolutionState.ShowEntityPathArrows ^= true; }
        private void MenuLanguageChangedEvent(object sender, RoutedEventArgs e) { ManiacEditor.Classes.Editor.EditorActions.ManiaMenuLanguageChanged(sender, e); }
        private void OptimizeEntitySlotIDsEvent(object sender, RoutedEventArgs e) { if (Classes.Editor.Solution.CurrentScene != null) Classes.Editor.Solution.Entities.OptimizeAllSlotIDs(); }
        private void ToggleRightClickSlotIDSwapEvent(object sender, RoutedEventArgs e) { Classes.Editor.SolutionState.RightClicktoSwapSlotID ^= true; }
        private void ToggleCopyAirEvent(object sender, RoutedEventArgs e) { Classes.Editor.SolutionState.CopyAir ^= true; }
        private void ChangeLevelIDEvent(object sender, RoutedEventArgs e) { ManiacEditor.Classes.Editor.EditorActions.ChangeLevelID(sender, e); }
        private void ToggleMultiLayerSelectEvent(object sender, RoutedEventArgs e) { Classes.Editor.SolutionState.MultiLayerEditMode ^= true; }
        private void MakeDataFolderShortcutEvent(object sender, RoutedEventArgs e) { ManiacEditor.Classes.Editor.EditorActions.MakeShortcutForDataFolderOnly(sender, e); }
        private void MakeShortcutWithCurrentCoordinatesEvent(object sender, RoutedEventArgs e) { ManiacEditor.Classes.Editor.EditorActions.MakeShortcutWithCurrentCoordinatesToolStripMenuItem_Click(sender, e); }
        private void MakeShortcutWithoutCurrentCoordinatesEvent(object sender, RoutedEventArgs e) { ManiacEditor.Classes.Editor.EditorActions.MakeShortcutWithoutCurrentCoordinatesToolStripMenuItem_Click(sender, e); }
        private void SoundLooperToolStripMenuItem_Click(object sender, RoutedEventArgs e) { ManiacEditor.Classes.Editor.EditorActions.SoundLooperToolStripMenuItem_Click(sender, e); }
        private void FindUnusedTiles(object sender, RoutedEventArgs e) { ManiacEditor.Controls.Base.MainEditor.Instance.FindAndReplace.FindUnusedTiles(); }

        #region Apps
        private void RSDKUnpackerEvent(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.RSDKUnpacker(); }
        private void SonicManiaHeadless(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.SonicManiaHeadless(); }
        private void MenuAppsCheatEngine_Click(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.CheatEngine(); }
        private void ModManager(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.ManiaModManager(); }
        private void TileManiacNormal(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.TileManiacNormal(); }
        private void InsanicManiacToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.InsanicManiac(); }
        private void RSDKAnnimationEditorToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.RSDKAnnimationEditor(); }
        private void RenderListManagerToolstripMenuItem_Click(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.RenderListManager(); }
        private void ColorPaletteEditorToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.ManiaPal(sender, e); }
        private void ManiaPalMenuItem_SubmenuOpened(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.ManiaPalSubmenuOpened(sender, e); }
        private void DuplicateObjectIDHealerToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.DuplicateObjectIDHealer(); }
        #endregion

        #region Folders
        private void OpenSceneFolderToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.OpenSceneFolder(); }
        private void OpenManiacEditorFolderToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.OpenManiacEditorFolder(); }
        private void OpenManiacEditorFixedSettingsFolderToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.OpenManiacEditorFixedSettingsFolder(); }
        private void OpenManiacEditorPortableSettingsFolderToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.OpenManiacEditorPortableSettingsFolder(); }
        private void OpenDataDirectoryFolderToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.OpenDataDirectory(); }
        private void OpenSonicManiaFolderToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.OpenSonicManiaFolder(); }
        private void OpenASavedPlaceToolStripMenuItem_DropDownOpening(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.OpenASavedPlaceDropDownOpening(sender, e); }
        private void OpenASavedPlaceToolStripMenuItem_DropDownClosed(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.OpenASavedPlaceDropDownClosed(sender, e); }
        private void OpenAResourcePackFolderToolStripMenuItem_DropDownOpening(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.OpenAResourcePackFolderDropDownOpening(sender, e); }
        private void OpenAResourcePackFolderToolStripMenuItem_DropDownClosed(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.OpenAResourcePackFolderDropDownClosed(sender, e); }
        #endregion

        #region Settings and Other Menu Events
        public void AboutScreenEvent(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.AboutScreen(); }
        public void ImportObjectsToolStripMenuItem_Click(Window window = null) { Methods.ProgramLauncher.ImportObjectsToolStripMenuItem_Click(window); }
        public void ImportObjectsWithMegaList(Window window = null) { Methods.ProgramLauncher.ImportObjectsWithMegaList(window); }
        public void ImportSoundsEvent(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.ImportSoundsToolStripMenuItem_Click(sender, e); }
        public void ImportSoundsEvent(Window window = null) { Methods.ProgramLauncher.ImportSounds(window); }
        private void LayerManagerEvent(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.LayerManager(sender, e); }
        private void ManiacINIEditorEvent(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.ManiacINIEditor(sender, e); }
        private void ChangePrimaryBackgroundColorEvent(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.ChangePrimaryBackgroundColor(sender, e); }
        private void ChangeSecondaryBackgroundColorEvent(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.ChangeSecondaryBackgroundColor(sender, e); }
        public void ObjectManagerEvent(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.ObjectManager(); }
        private void InGameOptionsMenuEvent(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.InGameSettings(); }
        private void WikiLinkEvent(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.WikiLink(); }
        public void OptionsMenuEvent(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.OptionsMenu(); }
        private void ControlsMenuEvent(object sender, RoutedEventArgs e) { Methods.ProgramLauncher.ControlMenu(); }
        #endregion

        private void RecentScenes_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            ManiacEditor.Controls.Base.MainEditor.Instance.RefreshRecentScenes();
        }

        private void RecentDataSources_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            ManiacEditor.Controls.Base.MainEditor.Instance.RefreshDataSources();
        }

        #endregion

        private void TestingDeveloperMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
