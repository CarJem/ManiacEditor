using System;
using System.Windows;
using System.Windows.Controls;
using RSDKv5;
using System.Diagnostics;
using GenerationsLib.WPF;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace ManiacEditor.Controls.Editor.Toolbars
{
	public partial class TilesToolbar : UserControl
	{
		#region Definitions
		public ManiacEditor.Controls.Editor.MainEditor Instance;

		public Methods.Draw.GIF TileGridImage
		{
			get
			{
				return Methods.Editor.Solution.CurrentTiles?.Image;
			}
		}

		bool isDisposing = false;
		public Action<ushort> TileDoubleClick { get; set; }

		private bool _TilesFlipHorizontal = false;
		private bool _TilesFlipVertical = false;

		public bool TilesFlipHorizontal
		{
			get
			{
				return _TilesFlipHorizontal;
			}
			set
			{
				_TilesFlipHorizontal = value;
				TilesList.FlipX = value;
				ChunkList.FlipX = value;
			}
		}
		public bool TilesFlipVertical
		{
			get
			{
				return _TilesFlipVertical;
			}
			set
			{
				_TilesFlipVertical = value;
				TilesList.FlipY = value;
				ChunkList.FlipY = value;
			}
		}
		private bool CurrentMultiLayerState { get; set; } = false;

		private bool AwaitTileContextMenuOpen { get; set; } = false;
		private bool AwaitChunkContextMenuOpen { get; set; } = false;

		public ManiacEditor.Controls.Global.Controls.ManiacTileList ChunkList;
		public ManiacEditor.Controls.Global.Controls.ManiacTileList TilesList;

		public Action<int, bool> TileOptionChanged;

		CheckBox[] CurrentTileCheckboxOptions = new CheckBox[6];
		CheckBox[] SelectedTilesCheckboxOptions = new CheckBox[4];
		bool setCheckboxes;

		public int SelectedTileIndex { get; set; }

		public int SelectedChunkIndex { get; set; }

		public ushort SelectedTile
		{
			get
			{
				ushort Tile = (ushort)TilesList.SelectedIndex;

				bool FlipX = CurrentTileCheckboxOptions[0].IsChecked.Value;
				bool FlipY = CurrentTileCheckboxOptions[1].IsChecked.Value;
				bool SolidTopA = CurrentTileCheckboxOptions[2].IsChecked.Value;
				bool SolidLrbA = CurrentTileCheckboxOptions[3].IsChecked.Value;
				bool SolidTopB = CurrentTileCheckboxOptions[4].IsChecked.Value;
				bool SolidLrbB = CurrentTileCheckboxOptions[5].IsChecked.Value;

				Tile = (ushort)ManiacEditor.Methods.Layers.TileFindReplace.SetBit(10, FlipX, Tile);
				Tile = (ushort)ManiacEditor.Methods.Layers.TileFindReplace.SetBit(11, FlipY, Tile);
				Tile = (ushort)ManiacEditor.Methods.Layers.TileFindReplace.SetBit(12, SolidTopA, Tile);
				Tile = (ushort)ManiacEditor.Methods.Layers.TileFindReplace.SetBit(13, SolidLrbA, Tile);
				Tile = (ushort)ManiacEditor.Methods.Layers.TileFindReplace.SetBit(14, SolidTopB, Tile);
				Tile = (ushort)ManiacEditor.Methods.Layers.TileFindReplace.SetBit(15, SolidLrbB, Tile);

				return Tile;
			}
		}

		public bool ShowShortcuts { set { UpdateShortcuts(value); } }

		public enum TileOptionState
		{
			Disabled,
			Checked,
			Unchcked,
			Indeterminate
		}
		#endregion

		#region Init
		public TilesToolbar(ManiacEditor.Controls.Editor.MainEditor _Instance)
		{
			try
			{
				InitializeComponent();
				SetupTilesList(_Instance);

				Instance = _Instance;

				SetCheckboxDefaults();
				UpdateShortcuts();
				ReloadLists();

			}
			catch (Exception ex)
			{
				Debug.Print(ex.ToString());
			}

		}


		private void SetCheckboxDefaults()
		{
			SelectedTilesCheckboxOptions[0] = tileOption1;
			SelectedTilesCheckboxOptions[1] = tileOption2;
			SelectedTilesCheckboxOptions[2] = tileOption3;
			SelectedTilesCheckboxOptions[3] = tileOption4;

			CurrentTileCheckboxOptions[0] = option1CheckBox;
			CurrentTileCheckboxOptions[1] = option2CheckBox;
			CurrentTileCheckboxOptions[2] = SolidTopACheckBox;
			CurrentTileCheckboxOptions[3] = SolidAllButTopACheckBox;
			CurrentTileCheckboxOptions[4] = SolidTopBCheckBox;
			CurrentTileCheckboxOptions[5] = SolidAllButTopBCheckBox;
		}

		#endregion

		#region UI Refresh
		public void RefreshThemeColors()
		{
			System.Drawing.Color ListBackColor = Methods.Internal.Theming.ThemeBrush1;
			this.ChunkList.Background = new System.Windows.Media.SolidColorBrush(ListBackColor.ToSWMColor());
			this.TilesList.Background = new System.Windows.Media.SolidColorBrush(ListBackColor.ToSWMColor());
		}
		public void SetupTilesList(ManiacEditor.Controls.Editor.MainEditor Instance)
		{
			this.ChunkList = new ManiacEditor.Controls.Global.Controls.ManiacTileList();
			this.TilesList = new ManiacEditor.Controls.Global.Controls.ManiacTileList();

			this.ChunkList.ContextMenuRequestClick += ChunkList_ContextMenuRequestClick;
			this.TilesList.ContextMenuRequestClick += TilesList_ContextMenuRequestClick;

			this.ChunkList.SelectedIndexChanged += ChunkList_SelectedIndexChanged;
			this.TilesList.SelectedIndexChanged += TilesListList_SelectedIndexChanged;

			this.TilesList.MouseMove += TilesList_MouseMove;
			this.TilesList.MouseDoubleClick += TilesList_MouseDoubleClick;

			RefreshThemeColors();

			TileViewer.Children.Add(TilesList);
			ChunksPage.Children.Add(ChunkList);
		}

		private void UpdateShortcuts(bool Show = false)
		{
			if (Show)
			{
				option1CheckBox.Content = "Flip Horizontal " + Environment.NewLine + Extensions.KeyEventExts.KeyBindPraser("FlipHTiles", true);
				option2CheckBox.Content = "Flip Vertical " + Environment.NewLine + Extensions.KeyEventExts.KeyBindPraser("FlipVTiles", true);
			}
			else
			{
				option1CheckBox.Content = "Flip Horizontal" + Environment.NewLine + string.Format("({0} - Selected Only)", Extensions.KeyEventExts.KeyBindPraser("FlipH"));
				option2CheckBox.Content = "Flip Vertical" + Environment.NewLine + string.Format("({0} - Selected Only)", Extensions.KeyEventExts.KeyBindPraser("FlipV"));
			}
		}
		private void SetDropdownItemsState(bool enabled)
		{
			AutoGenerateChunksSingle.IsEnabled = enabled;
			AutoGenerateChunks.IsEnabled = enabled;
			SaveChunksManually.IsEnabled = enabled;
		}

		public void UpdateModeSpecifics(SelectionChangedEventArgs sender = null)
		{
			this.Dispatcher.BeginInvoke(new Action(() =>
			{
				if (sender != null && sender.Source is TabControl)
				{
					if (TabControl.SelectedIndex == 0) Instance.EditorToolbar.ChunksToolButton.IsChecked = false;
					else Instance.EditorToolbar.ChunksToolButton.IsChecked = true;
				}

				if (ManiacEditor.Methods.Editor.SolutionState.IsChunksEdit() && this.ChunkList != null)
				{
					SelectedTileLabel.Content = "Selected Chunk: " + ChunkList.SelectedIndex.ToString();
					if (TabControl.SelectedIndex != 1) TabControl.SelectedIndex = 1;
				}
				else if (this.TilesList != null)
				{
					SelectedTileLabel.Content = "Selected Tile: " + TilesList.SelectedIndex.ToString();
					if (TabControl.SelectedIndex != 0) TabControl.SelectedIndex = 0;
				}

				SelectedChunkIndex = ChunkList.SelectedIndex;
				SelectedTileIndex = TilesList.SelectedIndex;

				if (AwaitChunkContextMenuOpen)
				{
					UpdateChunksContextMenu();
				}
				
				if (AwaitTileContextMenuOpen)
				{
					UpdateTilesContextMenu();
				}



			}));
		}
		private void RefreshLists()
		{
			ChunkList.Invalidate();
			TilesList.Invalidate();
		}


		#endregion

		#region List Reload Methods

		public void UpdateChunksListIfNeeded()
		{
			if (CurrentMultiLayerState == false && Methods.Editor.Solution.EditLayerB != null)
			{
				CurrentMultiLayerState = true;
				ChunksReload();
			}
			else if (CurrentMultiLayerState == true && Methods.Editor.Solution.EditLayerB == null)
			{
				CurrentMultiLayerState = false;
				ChunksReload();
			}
		}
		public void ChunksReload()
		{
			if (isDisposing) return;
			if (Instance.Chunks != null)
			{
				int LastIndex = ChunkList.SelectedIndex;
				ChunkList.SelectedIndex = -1;
				var chunksList = Instance.Chunks.GetChunkCurrentImages();
				ChunkList.Images.Clear();
				ChunkList.Images = new List<System.Windows.Media.ImageSource>(chunksList);
				ChunkList.SelectedIndex = LastIndex;
				TilesList.Invalidate(true);
			}

		}

		private List<System.Windows.Media.ImageSource> GetCurrentTileImages()
		{
			List<System.Windows.Media.ImageSource> TilesList = new List<System.Windows.Media.ImageSource>();
			for (int i = 0; i < 1024; i++)
			{
				TilesList.Add(TileGridImage.GetImageSource(new System.Drawing.Rectangle(0, 16 * i, 16, 16), false, false));
			}
			return TilesList;
		}


		public void TilesReload()
		{
			if (isDisposing) return;
			TilesList.Images.Clear();

			var tilesList = GetCurrentTileImages();
			TilesList.Images.Clear();
			TilesList.Images = new List<System.Windows.Media.ImageSource>(tilesList);

			//TilesFlipHorizontal
			//TilesFlipVertical

			int indexStorage = TilesList.SelectedIndex;
			TilesList.SelectedIndex = -1;
			TilesList.SelectedIndex = indexStorage;
			TilesList.Invalidate(true);

		}
		public void ReloadLists()
		{
			TilesReload();
			ChunksReload();
		}

		#endregion

		#region Events
		private void TilesList_MouseDoubleClick(object sender, System.Windows.Input.MouseEventArgs e)
		{
			if (isDisposing) return;
			if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
			{
				if (SelectedTileIndex != -1 && TileDoubleClick != null)
				{
					TileDoubleClick(SelectedTile);
				}
			}
		}
		private void TilesList_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
		{

		}
		private void TilePanel_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			Methods.Internal.Controls.GraphicPanel_OnKeyDown(sender, e);
		}
		private void TilePanel_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			Methods.Internal.Controls.GraphicPanel_OnKeyUp(sender, e);
		}
		private void ChunkList_SelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateModeSpecifics();
		}
		private void TilesListList_SelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateModeSpecifics();
		}

		private void TileZoomTrackbar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (isDisposing) return;
			int scale = 16 << (int)TileZoomTrackbar.Value;
			TilesList.ImageSize = scale;
		}
		private void TilesToolbar_Load(object sender, RoutedEventArgs e)
		{
			SetDefaults();
			RefreshLists();
		}
		private void TabControl_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
		{
			UpdateModeSpecifics(e);
		}
		private void UserControl_Unloaded(object sender, RoutedEventArgs e)
		{
			//this.Dispose();
		}
		private void TabItem_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{

		}
		private void SaveChunksManually_Click(object sender, RoutedEventArgs e)
		{
			Instance.Chunks?.Save();
		}
		private void AutoGenerateChunks_Click(object sender, RoutedEventArgs e)
		{
			if (Methods.Editor.Solution.EditLayerA != null && Methods.Editor.Solution.EditLayerB != null)
			{
				Instance.Chunks.AutoGenerateChunks(Methods.Editor.Solution.EditLayerA, Methods.Editor.Solution.EditLayerB);
				ChunksReload();
			}
		}
		private void ChunkZoomTrackbar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (ChunkList != null && ChunkZoomTrackbar != null)
			{
				if (isDisposing) return;
				int scale = 32 << (int)ChunkZoomTrackbar.Value;
				ChunkList.ImageSize = scale;
			}

		}
		private void AutoGenerateChunksSingle_Click(object sender, RoutedEventArgs e)
		{
			if (Methods.Editor.Solution.EditLayerA != null)
			{
				Instance.Chunks.AutoGenerateChunks(Methods.Editor.Solution.EditLayerA);
				ChunksReload();
			}
		}
		private void ExtraOptionsButton_Click(object sender, RoutedEventArgs e)
		{
			ExtraOptionsButton.ContextMenu.IsOpen = true;

			if (!ManiacEditor.Methods.Editor.SolutionState.IsChunksEdit()) SetDropdownItemsState(false);
			else SetDropdownItemsState(true);
		}
		#endregion

		#region Common Methods

		private void SetDefaults()
		{
			TileZoomTrackbar.Value = Properties.Settings.MyDefaults.TilesDefaultZoom;
			ChunkZoomTrackbar.Value = Properties.Settings.MyDefaults.ChunksDefaultZoom;

			SolidTopACheckBox.IsChecked = Properties.Settings.MyDefaults.SolidTopADefault;
			SolidAllButTopACheckBox.IsChecked = Properties.Settings.MyDefaults.SolidAllButTopADefault;
			SolidTopBCheckBox.IsChecked = Properties.Settings.MyDefaults.SolidTopBDefault;
			SolidAllButTopBCheckBox.IsChecked = Properties.Settings.MyDefaults.SolidAllButTopBDefault;
		}
		public void Reload()
		{
			if (isDisposing) return;
			ReloadLists();
		}
		public void Dispose()
		{
			isDisposing = true;
		}

		#endregion

		#region Set Tile Option Methods
		public void SetSelectTileOption(int option, bool state)
		{
			CurrentTileCheckboxOptions[option].IsChecked = state;
		}
		public void SetTileOptionState(int option, TileOptionState state)
		{
			setCheckboxes = true;
			switch (state)
			{
				case TileOptionState.Disabled:
					SelectedTilesCheckboxOptions[option].IsEnabled = false;
					SelectedTilesCheckboxOptions[option].IsChecked = false;
					break;
				case TileOptionState.Checked:
					SelectedTilesCheckboxOptions[option].IsEnabled = true;
					SelectedTilesCheckboxOptions[option].IsChecked = true;
					break;
				case TileOptionState.Unchcked:
					SelectedTilesCheckboxOptions[option].IsEnabled = true;
					SelectedTilesCheckboxOptions[option].IsChecked = false;
					break;
				case TileOptionState.Indeterminate:
					SelectedTilesCheckboxOptions[option].IsEnabled = true;
					SelectedTilesCheckboxOptions[option].IsChecked = null;
					break;
			}
			setCheckboxes = false;
		}
		#endregion

		#region Tile Option Events

		private void option1CheckBox_CheckedChanged(object sender, RoutedEventArgs e)
		{
			TilesFlipHorizontal = option1CheckBox.IsChecked.Value;
		}
		private void option2CheckBox_CheckedChanged(object sender, RoutedEventArgs e)
		{
			TilesFlipVertical = option2CheckBox.IsChecked.Value;
		}
		private void tileOption1_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (!setCheckboxes) TileOptionChanged?.Invoke(0, tileOption1.IsChecked.Value);
		}
		private void tileOption2_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (!setCheckboxes) TileOptionChanged?.Invoke(1, tileOption2.IsChecked.Value);
		}
		private void tileOption3_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (!setCheckboxes) TileOptionChanged?.Invoke(2, tileOption3.IsChecked.Value);
		}
		private void tileOption4_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (!setCheckboxes) TileOptionChanged?.Invoke(3, tileOption4.IsChecked.Value);
		}

		#endregion

		#region Chunk Management
		public void RemoveChunk(int ChunkIndex)
		{
			Instance.Chunks.RemoveChunk(ChunkIndex);
			ChunkList.Images.Clear();
			if (ChunkIndex != 0) ChunkList.SelectedIndex = ChunkIndex--;
			else ChunkList.SelectedIndex = -1;
			ChunksReload();
		}
		public void DuplicateChunk(int ChunkIndex)
		{
			Instance.Chunks.DuplicateChunk(ChunkIndex);
			ChunksReload();
		}



		#endregion

		#region Context Menu Items

		private void TilesList_ContextMenuRequestClick(object sender, System.Windows.Input.MouseEventArgs e)
		{
			if (e.RightButton == System.Windows.Input.MouseButtonState.Pressed)
			{
				TileContextMenu.IsOpen = true;
			}
		}
		private void ChunkList_ContextMenuRequestClick(object sender, System.Windows.Input.MouseEventArgs e)
		{
			if (e.RightButton == System.Windows.Input.MouseButtonState.Pressed)
			{
				ChunksContextMenu.IsOpen = true;
			}
		}

		private void UpdateTilesContextMenu()
		{
			EditTileCollisionMenuItem.IsEnabled = false;

			if (SelectedTileIndex != -1)
			{
				EditTileCollisionMenuItem.IsEnabled = true;
				EditTileCollisionMenuItem.Header = string.Format(EditTileCollisionMenuItem.Tag.ToString(), "#" + SelectedTileIndex);
			}
			else
			{
				EditTileCollisionMenuItem.IsEnabled = false;
				EditTileCollisionMenuItem.Header = string.Format(EditTileCollisionMenuItem.Tag.ToString(), "N/A");
			}
		}

		private void UpdateChunksContextMenu()
		{
			MoveChunkDownMenuItem.IsEnabled = false;
			MoveChunkUpMenuItem.IsEnabled = false;
			DuplicateChunkMenuItem.IsEnabled = false;
			RemoveChunkMenuItem.IsEnabled = false;
			PasteChunkMenuItem.IsEnabled = false;

			PasteChunkMenuItem.IsEnabled = true;

			if (SelectedChunkIndex != -1)
			{
				//MoveChunkDownMenuItem.IsEnabled = true;
				//MoveChunkUpMenuItem.IsEnabled = true;
				DuplicateChunkMenuItem.IsEnabled = true;
				RemoveChunkMenuItem.IsEnabled = true;

				MoveChunkDownMenuItem.Header = string.Format(MoveChunkDownMenuItem.Tag.ToString(), "#" + SelectedChunkIndex);
				MoveChunkUpMenuItem.Header = string.Format(MoveChunkUpMenuItem.Tag.ToString(), "#" + SelectedChunkIndex);
				DuplicateChunkMenuItem.Header = string.Format(DuplicateChunkMenuItem.Tag.ToString(), "#" + SelectedChunkIndex);
				RemoveChunkMenuItem.Header = string.Format(RemoveChunkMenuItem.Tag.ToString(), "#" + SelectedChunkIndex);
			}
			else
			{
				//MoveChunkDownMenuItem.IsEnabled = false;
				//MoveChunkUpMenuItem.IsEnabled = false;
				DuplicateChunkMenuItem.IsEnabled = false;
				RemoveChunkMenuItem.IsEnabled = false;

				MoveChunkDownMenuItem.Header = string.Format(MoveChunkDownMenuItem.Tag.ToString(), "N/A");
				MoveChunkUpMenuItem.Header = string.Format(MoveChunkUpMenuItem.Tag.ToString(), "N/A");
				DuplicateChunkMenuItem.Header = string.Format(DuplicateChunkMenuItem.Tag.ToString(), "N/A");
				RemoveChunkMenuItem.Header = string.Format(RemoveChunkMenuItem.Tag.ToString(), "N/A");
			}
		}

		private void RemoveChunkMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (SelectedChunkIndex != -1)
			{
				Instance.TilesToolbar.RemoveChunk(SelectedChunkIndex);
			}
		}

		private void DuplicateChunkMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (SelectedChunkIndex != -1)
			{
				Instance.TilesToolbar.DuplicateChunk(SelectedChunkIndex);
			}
		}

		private void PasteChunkFromClipboardMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (Instance.TilesClipboard != null)
			{
				Instance.Chunks.ConvertClipboardtoMultiLayerChunk(Instance.TilesClipboard.Item1, Instance.TilesClipboard.Item2);

				Instance.TilesToolbar?.ChunksReload();
			}
		}

		private void EditTileCollisionMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (Instance.TileManiacInstance == null || Instance.TileManiacInstance.IsEditorClosed) Instance.TileManiacInstance = new ManiacEditor.Controls.TileManiac.CollisionEditor();
			if (Instance.TileManiacInstance.Visibility != System.Windows.Visibility.Visible)
			{
				Instance.TileManiacInstance.Show();
			}
			if (Methods.Editor.Solution.TileConfig != null && Methods.Editor.Solution.CurrentTiles != null)
			{
				if (Instance.TileManiacInstance.Visibility != System.Windows.Visibility.Visible || Instance.TileManiacInstance.TileConfig == null)
				{
					Instance.TileManiacInstance.LoadTileConfigViaIntergration(Methods.Editor.Solution.TileConfig, ManiacEditor.Methods.Editor.SolutionPaths.TileConfig_Source.ToString(), SelectedTileIndex);
				}
				else
				{
					Instance.TileManiacInstance.SetCollisionIndex(SelectedTileIndex);
					Instance.TileManiacInstance.Activate();
				}

			}
		}

		private void EditChunkMenuItem_Click(object sender, RoutedEventArgs e)
		{

		}

		#endregion

		private void TileContextMenu_ContextMenuOpening(object sender, ContextMenuEventArgs e)
		{
			UpdateTilesContextMenu();
		}

		private void ChunksContextMenu_ContextMenuOpening(object sender, ContextMenuEventArgs e)
		{
			UpdateChunksContextMenu();
		}
	}
}
