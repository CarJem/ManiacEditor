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

namespace ManiacEditor.Interfaces.EditorElements
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

            newShortcutToolStripMenuItem.IsEnabled = System.IO.Directory.Exists(Editor.Instance.DataDirectory);
            withoutCurrentCoordinatesToolStripMenuItem.IsEnabled = EditorSolution.CurrentScene != null;
            withCurrentCoordinatesToolStripMenuItem.IsEnabled = EditorSolution.CurrentScene != null;
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

            if (!Extensions.KeyBindsSettingExists(keyRefrence)) return nullString;

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
            entityManagerToolStripMenuItem.IsEnabled = enabled && EditorSolution.StageConfig != null;
            importSoundsToolStripMenuItem.IsEnabled = enabled && EditorSolution.StageConfig != null;
            layerManagerToolStripMenuItem.IsEnabled = enabled;
            editBackgroundColorsToolStripMenuItem.IsEnabled = enabled;

            undoToolStripMenuItem.IsEnabled = enabled && Editor.Instance.UndoStack.Count > 0;
            redoToolStripMenuItem.IsEnabled = enabled && Editor.Instance.RedoStack.Count > 0;

            findAndReplaceToolStripMenuItem.IsEnabled = enabled && Editor.Instance.EditLayerA != null;

            if (EditorSolution.Entities != null && EditorSolution.Entities.SelectedEntities != null && EditorSolution.Entities.SelectedEntities.Count > 1)
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
                if (Editor.Instance.IsTilesEdit()) windowsClipboardState = Clipboard.ContainsData("ManiacTiles");
                else windowsClipboardState = false;
                if (Editor.Instance.IsEntitiesEdit()) windowsEntityClipboardState = Clipboard.ContainsData("ManiacEntities");
                else windowsEntityClipboardState = false;
            }
            catch
            {
                windowsClipboardState = false;
                windowsEntityClipboardState = false;
            }


            if (Editor.Instance.IsTilesEdit())
            {
                if (enabled && HasCopyDataTiles()) SetPasteEnabledButtons(true);
                else SetPasteEnabledButtons(false);
            }
            else if (Editor.Instance.IsEntitiesEdit())
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
                pasteTochunkToolStripMenuItem.IsEnabled = pasteEnabled && Editor.Instance.IsTilesEdit();
            }

            bool HasCopyDataTiles() { return Editor.Instance.TilesClipboard != null || windowsClipboardState == true; }
            bool HasCopyDataEntities() { return Editor.Instance.entitiesClipboard != null || windowsEntityClipboardState == true; }
        }

        public void SetSelectOnlyButtonsState(bool enabled = true)
        {
            enabled &= Editor.Instance.IsSelected();
            deleteToolStripMenuItem.IsEnabled = enabled;
            copyToolStripMenuItem.IsEnabled = enabled;
            cutToolStripMenuItem.IsEnabled = enabled;
            duplicateToolStripMenuItem.IsEnabled = enabled;


            flipHorizontalToolStripMenuItem.IsEnabled = enabled && CanFlip(0);
            flipVerticalToolStripMenuItem.IsEnabled = enabled && CanFlip(0);
            flipHorizontalIndvidualToolStripMenuItem.IsEnabled = enabled && CanFlip(1);
            flipVerticalIndvidualToolStripMenuItem.IsEnabled = enabled && CanFlip(1);

            selectAllToolStripMenuItem.IsEnabled = (Editor.Instance.IsTilesEdit() && !Editor.Instance.IsChunksEdit()) || Editor.Instance.IsEntitiesEdit();

            bool CanFlip(int option)
            {
                switch (option)
                {
                    case 0:
                        if (Editor.Instance.IsEntitiesEdit() && Editor.Instance.IsSelected()) return true;
                        else if (Editor.Instance.IsTilesEdit()) return true;
                        break;
                    case 1:
                        return Editor.Instance.IsTilesEdit();
                }
                return false;
            }
        }

        #region Action Events (MenuItems, Clicks, etc.)
        #region File Events
        private void NewSceneEvent(object sender, RoutedEventArgs e) { Editor.Instance.FileHandler.NewScene(); }
        public void OpenSceneEvent(object sender, RoutedEventArgs e) { Editor.Instance.FileHandler.OpenScene(); }
        public void OpenDataDirectoryEvent(object sender, RoutedEventArgs e) { Editor.Instance.FileHandler.OpenDataDirectory(); }
        public void SaveSceneEvent(object sender, RoutedEventArgs e) { Editor.Instance.FileHandler.Save(); }
        private void ExitEditorEvent(object sender, RoutedEventArgs e) { Editor.Instance.Close(); }
        private void ExportAsPNGEvent(object sender, RoutedEventArgs e) { Editor.Instance.FileHandler.ExportAsPNG(); }
        private void ExportLayersAsPNGEvent(object sender, RoutedEventArgs e) { Editor.Instance.FileHandler.ExportLayersAsPNG(); }
        private void ExportObjLayoutAsPNGEvent(object sender, RoutedEventArgs e) { Editor.Instance.FileHandler.ExportObjLayoutAsPNG(); }
        private void ExportToolStripMenuItem_Click(object sender, RoutedEventArgs e) { EditorLaunch.ExportGUI(sender, e); }
        public void SaveSceneAsEvent(object sender, RoutedEventArgs e) { Editor.Instance.FileHandler.SaveAs(); }
        private void RecoverEvent(object sender, RoutedEventArgs e) { Editor.Instance.UIEvents.BackupRecoverButton_Click(sender, e); }
        public void UnloadSceneEvent(object sender, RoutedEventArgs e) { Editor.Instance.FileHandler.UnloadScene(); }
        private void BackupStageConfigEvent(object sender, RoutedEventArgs e) { Editor.Instance.UIEvents.StageConfigBackup(sender, e); }
        private void BackupSceneEvent(object sender, RoutedEventArgs e) { Editor.Instance.UIEvents.SceneBackup(sender, e); }
        #endregion
        #region Edit Events
        public void PasteToChunksEvent(object sender, RoutedEventArgs e) { Editor.Instance.UIEvents.PasteToChunks(); }
        public void SelectAllEvent(object sender, RoutedEventArgs e) { Editor.Instance.UIEvents.SelectAll(); }
        public void CutEvent(object sender, RoutedEventArgs e) { Editor.Instance.UIEvents.Cut(); }
        public void CopyEvent(object sender, RoutedEventArgs e) { Editor.Instance.UIEvents.Copy(); }
        public void PasteEvent(object sender, RoutedEventArgs e) { Editor.Instance.UIEvents.Paste(); }
        public void DuplicateEvent(object sender, RoutedEventArgs e) { Editor.Instance.UIEvents.Duplicate(); }
        private void DeleteEvent(object sender, RoutedEventArgs e) { Editor.Instance.UIEvents.Delete(); }
        public void FlipVerticalEvent(object sender, RoutedEventArgs e) { Editor.Instance.UIEvents.FlipVertical(); }
        public void FlipHorizontalEvent(object sender, RoutedEventArgs e) { Editor.Instance.UIEvents.FlipHorizontal(); }
        public void FlipVerticalIndividualEvent(object sender, RoutedEventArgs e) { Editor.Instance.UIEvents.FlipVerticalIndividual(); }
        public void FlipHorizontalIndividualEvent(object sender, RoutedEventArgs e) { Editor.Instance.UIEvents.FlipHorizontalIndividual(); }
        #endregion
        #region Developer Stuff (WIP)
        private void DeveloperTerminalEvent(object sender, RoutedEventArgs e) { EditorLaunch.DevTerm(); }
        private void MD5GeneratorToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Editor.Instance.UIEvents.MD5GeneratorToolStripMenuItem_Click(sender, e); }
        private void FindAndReplaceToolEvent(object sender, RoutedEventArgs e) { Editor.Instance.UIEvents.FindAndReplaceTool(sender, e); }
        private void ConsoleWindowToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Editor.Instance.UIEvents.ConsoleWindowToolStripMenuItem_Click(sender, e); }
        private void SaveForForceOpenOnStartupToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Editor.Instance.UIEvents.SaveForForceOpenOnStartupToolStripMenuItem_Click(sender, e); }
        private void LeftToolbarToggleDev_Click(object sender, RoutedEventArgs e) { Editor.Instance.UI.UpdateToolbars(false, true); }
        private void RightToolbarToggleDev_Click(object sender, RoutedEventArgs e) { Editor.Instance.UI.UpdateToolbars(true, true); }
        private void EnableAllButtonsToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Editor.Instance.UIEvents.EnableAllButtonsToolStripMenuItem_Click(sender, e); }
        #endregion
        public void GoToPositionEvent(object sender, RoutedEventArgs e) { Editor.Instance.UIEvents.GoToPosition(sender, e); }
        private void UndoEvent(object sender, RoutedEventArgs e) { Editor.Instance.EditorUndo(); }
        private void RedoEvent(object sender, RoutedEventArgs e) { Editor.Instance.EditorRedo(); }
        private void TogglePixelModeEvent(object sender, RoutedEventArgs e) { Editor.Instance.Options.CountTilesSelectedInPixels ^= true; }
        public void ToggleScrollLockEvent(object sender, RoutedEventArgs e) { Editor.Instance.Options.ScrollLocked ^= true; }
        public void ToggleFasterNudgeEvent(object sender, RoutedEventArgs e) { Editor.Instance.Options.EnableFasterNudge ^= true; }
        public void ApplyEditEntitiesTransparencyEvent(object sender, RoutedEventArgs e) { Editor.Instance.Options.ApplyEditEntitiesTransparency ^= true; }
        public void ToggleDebugHUDEvent(object sender, RoutedEventArgs e) { Editor.Instance.Options.DebugStatsVisibleOnPanel ^= true; }
        private void ResetZoomLevelEvent(object sender, RoutedEventArgs e) { Editor.Instance.ZoomModel.SetZoomLevel(0, new System.Drawing.Point(0, 0)); }
        private void UseLargeDebugHUDText(object sender, RoutedEventArgs e) { Editor.Instance.Options.UseLargeDebugStats ^= true; }
        public void MenuButtonChangedEvent(object sender, RoutedEventArgs e) { Editor.Instance.UIEvents.SetMenuButtonType(sender, e); }
        public void MenuButtonChangedEvent(string tag) { Editor.Instance.UIEvents.SetMenuButtonType(tag); }
        private void ShowEntitiesAboveAllOtherLayersToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Editor.Instance.Options.EntitiesVisibileAboveAllLayers ^= true; }
        private void EntitySelectionBoxesAlwaysPrioritizedEvent(object sender, RoutedEventArgs e) { Editor.Instance.Options.EntitySelectionBoxesAlwaysPrioritized ^= true; }
        private void PrioritizedEntityViewingEvent(object sender, RoutedEventArgs e) { Editor.Instance.Options.PrioritizedEntityViewing ^= true; }
        private void SetEncorePalleteEvent(object sender, RoutedEventArgs e) { Editor.Instance.UIEvents.SetEncorePallete(sender); }
        private void MoveExtraLayersToFrontEvent(object sender, RoutedEventArgs e) { Editor.Instance.Options.ExtraLayersMoveToFront ^= true; }
        private void EntityFilterTextChangedEvent(object sender, TextChangedEventArgs e) { Editor.Instance.UIEvents.EntityFilterTextChanged(sender, e); }
        private void ShowEntitySelectionBoxesEvent(object sender, RoutedEventArgs e) { Editor.Instance.Options.ShowEntitySelectionBoxes ^= true; }
        private void ShowWaterLevelEvent(object sender, RoutedEventArgs e) { Editor.Instance.Options.ShowWaterLevel ^= true; }
        private void AlwaysShowWaterLevelEvent(object sender, RoutedEventArgs e) { Editor.Instance.Options.AlwaysShowWaterLevel ^= true; }
        private void SortSelectedSlotIDsEvent(object sender, RoutedEventArgs e) { EditorSolution.Entities.OrderSelectedSlotIDs(); }
        private void SortSelectedSlotIDsOptimizedEvent(object sender, RoutedEventArgs e) { EditorSolution.Entities.OrderSelectedSlotIDs(true); }
        private void SortSelectedSlotIDsOrderedEvent(object sender, RoutedEventArgs e) { EditorSolution.Entities.OrderSelectedSlotIDs(false, true); }
        private void WaterSizeWithBoundsEvent(object sender, RoutedEventArgs e) { Editor.Instance.Options.SizeWaterLevelwithBounds ^= true; }
        private void SwapEncoreManiaEntityVisibilityEvent(object sender, RoutedEventArgs e) { Editor.Instance.UIEvents.SwapEncoreManiaEntityVisibility(); }
        private void ShowParallaxSpritesEvent(object sender, RoutedEventArgs e) { Editor.Instance.Options.ShowParallaxSprites ^= true; }
        private void SetScrollDirectionEvent(object sender, RoutedEventArgs e) { Editor.Instance.UIEvents.SetScrollLockDirection(); }
        private void ShowEntityPathArrowsEvent(object sender, RoutedEventArgs e) { Editor.Instance.Options.ShowEntityPathArrows ^= true; }
        private void MenuLanguageChangedEvent(object sender, RoutedEventArgs e) { Editor.Instance.UIEvents.MenuLanguageChanged(sender, e); }

        private void OptimizeEntitySlotIDsEvent(object sender, RoutedEventArgs e) { if (EditorSolution.CurrentScene != null) EditorSolution.Entities.OptimizeAllSlotIDs(); }
        private void ToggleRightClickSlotIDSwapEvent(object sender, RoutedEventArgs e) { Editor.Instance.Options.RightClicktoSwapSlotID ^= true; }
        private void ToggleCopyAirEvent(object sender, RoutedEventArgs e) { Editor.Instance.Options.CopyAir ^= true; }
        private void ChangeLevelIDEvent(object sender, RoutedEventArgs e) { Editor.Instance.UIEvents.ChangeLevelID(sender, e); }
        private void ToggleMultiLayerSelectEvent(object sender, RoutedEventArgs e) { Editor.Instance.Options.MultiLayerEditMode ^= true; }
        private void MakeDataFolderShortcutEvent(object sender, RoutedEventArgs e) { Editor.Instance.UIEvents.MakeShortcutForDataFolderOnly(sender, e); }
        private void MakeShortcutWithCurrentCoordinatesEvent(object sender, RoutedEventArgs e) { Editor.Instance.UIEvents.MakeShortcutWithCurrentCoordinatesToolStripMenuItem_Click(sender, e); }
        private void MakeShortcutWithoutCurrentCoordinatesEvent(object sender, RoutedEventArgs e) { Editor.Instance.UIEvents.MakeShortcutWithoutCurrentCoordinatesToolStripMenuItem_Click(sender, e); }
        private void SoundLooperToolStripMenuItem_Click(object sender, RoutedEventArgs e) { Editor.Instance.UIEvents.SoundLooperToolStripMenuItem_Click(sender, e); }
        private void FindUnusedTiles(object sender, RoutedEventArgs e) { Editor.Instance.FindAndReplace.FindUnusedTiles(); }


        #region Collision Slider Events
        private void CollisionOpacitySliderValueChangedEvent(object sender, RoutedPropertyChangedEventArgs<double> e) { Editor.Instance.UIEvents?.CollisionOpacitySliderValueChanged(sender, e); }
        #endregion

        #region Apps
        private void RSDKUnpackerEvent(object sender, RoutedEventArgs e) { EditorLaunch.RSDKUnpacker(); }
        private void SonicManiaHeadless(object sender, RoutedEventArgs e) { EditorLaunch.SonicManiaHeadless(); }
        private void MenuAppsCheatEngine_Click(object sender, RoutedEventArgs e) { EditorLaunch.CheatEngine(); }
        private void ModManager(object sender, RoutedEventArgs e) { EditorLaunch.ManiaModManager(); }
        private void TileManiacNormal(object sender, RoutedEventArgs e) { EditorLaunch.TileManiacNormal(); }
        private void InsanicManiacToolStripMenuItem_Click(object sender, RoutedEventArgs e) { EditorLaunch.InsanicManiac(); }
        private void RSDKAnnimationEditorToolStripMenuItem_Click(object sender, RoutedEventArgs e) { EditorLaunch.RSDKAnnimationEditor(); }
        private void RenderListManagerToolstripMenuItem_Click(object sender, RoutedEventArgs e) { EditorLaunch.RenderListManager(); }
        private void ColorPaletteEditorToolStripMenuItem_Click(object sender, RoutedEventArgs e) { EditorLaunch.ManiaPal(sender, e); }
        private void ManiaPalMenuItem_SubmenuOpened(object sender, RoutedEventArgs e) { EditorLaunch.ManiaPalSubmenuOpened(sender, e); }
        private void DuplicateObjectIDHealerToolStripMenuItem_Click(object sender, RoutedEventArgs e) { EditorLaunch.DuplicateObjectIDHealer(); }
        #endregion

        #region Folders
        private void OpenSceneFolderToolStripMenuItem_Click(object sender, RoutedEventArgs e) { EditorLaunch.OpenSceneFolder(); }
        private void OpenManiacEditorFolderToolStripMenuItem_Click(object sender, RoutedEventArgs e) { EditorLaunch.OpenManiacEditorFolder(); }
        private void OpenManiacEditorFixedSettingsFolderToolStripMenuItem_Click(object sender, RoutedEventArgs e) { EditorLaunch.OpenManiacEditorFixedSettingsFolder(); }
        private void OpenManiacEditorPortableSettingsFolderToolStripMenuItem_Click(object sender, RoutedEventArgs e) { EditorLaunch.OpenManiacEditorPortableSettingsFolder(); }
        private void OpenDataDirectoryFolderToolStripMenuItem_Click(object sender, RoutedEventArgs e) { EditorLaunch.OpenDataDirectory(); }
        private void OpenSonicManiaFolderToolStripMenuItem_Click(object sender, RoutedEventArgs e) { EditorLaunch.OpenSonicManiaFolder(); }
        private void OpenASavedPlaceToolStripMenuItem_DropDownOpening(object sender, RoutedEventArgs e) { EditorLaunch.OpenASavedPlaceDropDownOpening(sender, e); }
        private void OpenASavedPlaceToolStripMenuItem_DropDownClosed(object sender, RoutedEventArgs e) { EditorLaunch.OpenASavedPlaceDropDownClosed(sender, e); }
        private void OpenAResourcePackFolderToolStripMenuItem_DropDownOpening(object sender, RoutedEventArgs e) { EditorLaunch.OpenAResourcePackFolderDropDownOpening(sender, e); }
        private void OpenAResourcePackFolderToolStripMenuItem_DropDownClosed(object sender, RoutedEventArgs e) { EditorLaunch.OpenAResourcePackFolderDropDownClosed(sender, e); }
        #endregion

        #region Settings and Other Menu Events
        public void AboutScreenEvent(object sender, RoutedEventArgs e) { EditorLaunch.AboutScreen(); }
        public void ImportObjectsToolStripMenuItem_Click(Window window = null) { EditorLaunch.ImportObjectsToolStripMenuItem_Click(window); }
        public void ImportObjectsWithMegaList(Window window = null) { EditorLaunch.ImportObjectsWithMegaList(window); }
        public void ImportSoundsEvent(object sender, RoutedEventArgs e) { EditorLaunch.ImportSoundsToolStripMenuItem_Click(sender, e); }
        public void ImportSoundsEvent(Window window = null) { EditorLaunch.ImportSounds(window); }
        private void LayerManagerEvent(object sender, RoutedEventArgs e) { EditorLaunch.LayerManager(sender, e); }
        private void ManiacINIEditorEvent(object sender, RoutedEventArgs e) { EditorLaunch.ManiacINIEditor(sender, e); }
        private void ChangePrimaryBackgroundColorEvent(object sender, RoutedEventArgs e) { EditorLaunch.ChangePrimaryBackgroundColor(sender, e); }
        private void ChangeSecondaryBackgroundColorEvent(object sender, RoutedEventArgs e) { EditorLaunch.ChangeSecondaryBackgroundColor(sender, e); }
        public void ObjectManagerEvent(object sender, RoutedEventArgs e) { EditorLaunch.ObjectManager(); }
        private void InGameOptionsMenuEvent(object sender, RoutedEventArgs e) { EditorLaunch.InGameSettings(); }
        private void WikiLinkEvent(object sender, RoutedEventArgs e) { EditorLaunch.WikiLink(); }
        public void OptionsMenuEvent(object sender, RoutedEventArgs e) { EditorLaunch.OptionsMenu(); }
        private void ControlsMenuEvent(object sender, RoutedEventArgs e) { EditorLaunch.ControlMenu(); }
        #endregion

        private void RecentScenes_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            Editor.Instance.RefreshRecentScenes();
        }

        private void RecentDataSources_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            Editor.Instance.RefreshDataSources();
        }

        #endregion
    }
}
