using System;
using System.Windows;
using System.Windows.Controls;
using RSDKv5;
using System.Diagnostics;

namespace ManiacEditor.Controls.Editor.Toolbars.TilesToolbar
{
	public partial class TilesToolbar : UserControl
	{
        #region Definitions
        public ManiacEditor.Controls.Editor.MainEditor Instance;

		bool isDisposing = false;
		ManiacEditor.Methods.Draw.GIF TileGridImage;
		string TilesImagePath;
		public Action<ushort> TileDoubleClick { get; set; } 

		bool TilesFlipHorizontal = false;
		bool TilesFlipVertical = false;

		private bool ChunkRefreshNeeded { get; set; } = false;
		private bool TileRefreshNeeded { get; set; } = false;
		private bool CurrentMultiLayerState { get; set; } = false;

		public System.Windows.Forms.Panel TilePanel;
		public string CurrentColorPalette;

		public ManiacEditor.Controls.Global.Controls.RetroEDTileList ChunkList;
        public ManiacEditor.Controls.Global.Controls.RetroEDTileList TilesList;

		public System.Windows.Forms.Integration.WindowsFormsHost TileHost;
		public System.Windows.Forms.Integration.WindowsFormsHost ChunkHost;

		public Action<int, bool> TileOptionChanged;

		CheckBox[] CurrentTileCheckboxOptions = new CheckBox[6];
		CheckBox[] SelectedTilesCheckboxOptions = new CheckBox[4];
		bool setCheckboxes;

		public int SelectedTileIndex
		{
			get
			{
				return TilesList.SelectedIndex;
			}
		}

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
        public TilesToolbar(Classes.Scene.EditorTiles tiles, String data_directory, String Colors, ManiacEditor.Controls.Editor.MainEditor instance)
		{
            try
            {
                InitializeComponent();
                SetupTilesList(instance);

                Instance = instance;

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

                TilesImagePath = data_directory + "\\16x16Tiles.gif";
                TileGridImage = new Methods.Draw.GIF((TilesImagePath), Colors);

                UpdateShortcuts();

				ReloadLists(Colors);

			}
            catch (Exception ex)
            {
                Debug.Print(ex.ToString());
            }

		}
		#endregion

		#region UI Refresh
		public void RefreshThemeColors()
		{
			System.Drawing.Color ListBackColor = Methods.Internal.Theming.ThemeBrush1;
			this.ChunkList.BackColor = ListBackColor;
			this.TilePanel.BackColor = ListBackColor;
			this.TilesList.BackColor = ListBackColor;
		}
		public void SetupTilesList(ManiacEditor.Controls.Editor.MainEditor Instance)
		{
			TileHost = new System.Windows.Forms.Integration.WindowsFormsHost();
			ChunkHost = new System.Windows.Forms.Integration.WindowsFormsHost();
			this.ChunkList = new ManiacEditor.Controls.Global.Controls.RetroEDTileList();

			this.TilePanel = new System.Windows.Forms.Panel();



			// 
			// ChunkList
			// 
			this.ChunkList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ChunkList.ImageHeight = 16 * 8;
			this.ChunkList.ImageSize = 16 * 8;
			this.ChunkList.ImageWidth = 16 * 8;
			this.ChunkList.Location = new System.Drawing.Point(3, 3);
			this.ChunkList.Name = "ChunkList";
			this.ChunkList.ScrollValue = 0;
			this.ChunkList.SelectedIndex = -1;
			this.ChunkList.Size = new System.Drawing.Size(234, 290);
			this.ChunkList.TabIndex = 1;
			this.ChunkList.MouseClick += ChunkList_Click;
			this.ChunkList.SelectedIndexChanged += ChunkList_SelectedIndexChanged;

			// 
			// TileList
			//
			this.TilesList = new ManiacEditor.Controls.Global.Controls.RetroEDTileList();
			this.TilesList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.TilesList.ImageHeight = 16 * 1;
			this.TilesList.ImageSize = 16 * 1;
			this.TilesList.ImageWidth = 16 * 1;
			this.TilesList.Location = new System.Drawing.Point(3, 3);
			this.TilesList.Name = "TilesList";
			this.TilesList.ScrollValue = 0;
			this.TilesList.SelectedIndex = -1;
			this.TilesList.Size = new System.Drawing.Size(234, 290);
			this.TilesList.TabIndex = 1;
			this.TilesList.MouseClick += TilesList_MouseClick; ;
			this.TilesList.SelectedIndexChanged += TilesListList_SelectedIndexChanged;
			this.TilesList.MouseMove += TilesList_MouseMove;
			this.TilesList.MouseDoubleClick += TilesList_MouseDoubleClick;
			this.TilePanel.Controls.Add(this.TilesList);

			// 
			// tilePanel
			// 
			this.TilePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
			| System.Windows.Forms.AnchorStyles.Left)
			| System.Windows.Forms.AnchorStyles.Right)));
			this.TilePanel.Location = new System.Drawing.Point(0, 45);
			this.TilePanel.Name = "tilePanel";
			this.TilePanel.Size = new System.Drawing.Size(241, 253);
			this.TilePanel.TabIndex = 2;
			this.TilePanel.Dock = System.Windows.Forms.DockStyle.Fill;

			RefreshThemeColors();


			TileHost.Child = TilePanel;
			ChunkHost.Child = ChunkList;
			TileViewer.Children.Add(TileHost);
			ChunksPage.Children.Add(ChunkHost);
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
			}));
		}
		private void RefreshLists()
		{
			if (ChunkRefreshNeeded)
			{
				ChunkList.Refresh();
				ChunkRefreshNeeded = false;
			}
			if (TileRefreshNeeded)
			{
				TilesList.Refresh();
				TileRefreshNeeded = false;
			}
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
			ChunkList.Images.Clear();
			if (Instance.Chunks != null)
			{
				int LastIndex = ChunkList.SelectedIndex;
				ChunkList.SelectedIndex = -1;
				ChunkList.Images = Instance.Chunks.GetChunkCurrentImages();


				ChunkRefreshNeeded = true;
				ChunkList.SelectedIndex = LastIndex;
			}

		}
		public void TilesReload(string colors = "")
		{
			if (colors != "") CurrentColorPalette = colors;
			else if (CurrentColorPalette != null) colors = CurrentColorPalette;

			if (isDisposing) return;
			TilesList.Images.Clear();
			TileGridImage = new Methods.Draw.GIF((TilesImagePath), colors);

			for (int i = 0; i < 1024; i++)
			{
				TilesList.Images.Add(TileGridImage.GetBitmap(new System.Drawing.Rectangle(0, 16 * i, 16, 16), TilesFlipHorizontal, TilesFlipVertical));
			}
			int indexStorage = TilesList.SelectedIndex;
			TilesList.SelectedIndex = -1;

			TileRefreshNeeded = true;
			TilesList.SelectedIndex = indexStorage;

		}
		public void ReloadLists(String Colors = "")
		{
			ChunksReload();
			TilesReload(Colors);
		}

		#endregion

		#region Events
		private void TilesList_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (isDisposing) return;
			if (e.Button == System.Windows.Forms.MouseButtons.Left)
			{
				if (SelectedTileIndex != -1 && TileDoubleClick != null)
				{
					TileDoubleClick(SelectedTile);
				}
			}
		}
		private void TilesList_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			TilesList.EditCollisionMenuItem.Enabled = false;
			if (e.Button == System.Windows.Forms.MouseButtons.Right)
			{
				if (TilesList.SelectedIndex != -1)
				{
					TilesList.EditCollisionMenuItem.Enabled = true;
					TilesList.EditCollisionMenuItem.Text = string.Format("Edit Collision of Tile {0} with Tile Maniac", SelectedTileIndex);

					TilesList.Tiles16ToolStrip.Show(TilesList, e.Location);
				}
				else
				{
					TilesList.EditCollisionMenuItem.Enabled = false;
					TilesList.EditCollisionMenuItem.Text = string.Format("Edit Collision of Tile {0} with Tile Maniac", "N/A");

					TilesList.Tiles16ToolStrip.Show(TilesList, e.Location);
				}
			}
		}
		private void TilesList_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (isDisposing) return;
			if (e.Button == System.Windows.Forms.MouseButtons.Left)
			{
				if (SelectedTileIndex != -1)
				{
					Int32 val = SelectedTileIndex;
					TilesList.DoDragDrop(val, System.Windows.Forms.DragDropEffects.Move);
				}
			}
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
		private void ChunkList_Click(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			ChunkList.moveChunkDownToolStripMenuItem.Enabled = false;
			ChunkList.moveChunkUpToolStripMenuItem.Enabled = false;
			if (e.Button == System.Windows.Forms.MouseButtons.Right)
			{
				if (ChunkList.SelectedIndex != -1)
				{
					ChunkList.removeChunkToolStripMenuItem.Enabled = true;
					ChunkList.duplicateChunkToolStripMenuItem.Enabled = true;

					ChunkList.Chunks128ToolStrip.Show(ChunkList, e.Location);
				}
				else
				{
					ChunkList.removeChunkToolStripMenuItem.Enabled = false;
					ChunkList.duplicateChunkToolStripMenuItem.Enabled = false;

					ChunkList.Chunks128ToolStrip.Show(ChunkList, e.Location);
				}
			}
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
			RefreshLists();
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
		private void MenuItem_SubmenuOpened(object sender, RoutedEventArgs e)
		{
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
		public void Reload(string colors = null)
		{
			if (isDisposing) return;
			ReloadLists(colors);
		}
		public void Dispose()
		{
			isDisposing = true;
			if (TilePanel != null)
			{
				TilePanel.Controls.Clear();
				TilePanel.Dispose();
				TilePanel = null;
			}
			if (TilesList != null)
			{
				TilesList.Controls.Clear();
				TilesList.Dispose();
				TilesList = null;
			}
			if (ChunkList != null)
			{
				ChunkList.Controls.Clear();
				ChunkList.Dispose();
				ChunkList = null;
			}
			if (TileHost != null)
			{
				TileHost.Child.Dispose();
				TileHost = null;
			}
			if (ChunkHost != null)
			{
				ChunkHost.Child.Dispose();
				ChunkHost = null;
			}


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
			TilesReload();
		}
		private void option2CheckBox_CheckedChanged(object sender, RoutedEventArgs e)
		{
			TilesFlipVertical = option2CheckBox.IsChecked.Value;
			TilesReload();
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



    }
}
