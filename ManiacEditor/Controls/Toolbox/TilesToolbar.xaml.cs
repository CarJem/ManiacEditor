using System;
using System.Windows;
using System.Windows.Controls;
using RSDKv5;
using System.Diagnostics;
using GenerationsLib.WPF;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;

namespace ManiacEditor.Controls.Toolbox
{
	public partial class TilesToolbar : UserControl
	{
		#region Definitions
		public ManiacEditor.Controls.Editor.MainEditor Instance;

		public Classes.Rendering.GIF TileGridImage
		{
			get
			{
				return Methods.Solution.CurrentSolution.CurrentTiles?.BaseImage;
			}
		}

		bool isDisposing = false;
		public Action<ushort> TileDoubleClick { get; set; }
		public Action<Tuple<List<ushort>, int[]>> MultiTileDoubleClick { get; set; }

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
				UpdateListFlips();
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
				UpdateListFlips();
			}
		}


		private void UpdateListFlips()
		{
			Application.Current.Dispatcher.Invoke(new Action(() =>
			{
				TilesList.FlipX = TilesFlipHorizontal;
				ChunkList.FlipX = TilesFlipHorizontal;
				TilesList.FlipY = TilesFlipVertical;
				ChunkList.FlipY = TilesFlipVertical;
			}), System.Windows.Threading.DispatcherPriority.Background);
		}

		private bool CurrentMultiLayerState { get; set; } = false;

		private bool AwaitTileContextMenuOpen { get; set; } = false;
		private bool AwaitChunkContextMenuOpen { get; set; } = false;

		public ManiacEditor.Controls.Global.Controls.ManiacTileList ChunkList;
		public ManiacEditor.Controls.Global.Controls.ManiacTileList TilesList;

		public Action<int, bool> TileOptionChanged { get; set; }

		CheckBox[] PlacedTileCheckboxOptions = new CheckBox[6];
		CheckBox[] SelectedTilesCheckboxOptions = new CheckBox[6];
		bool setCheckboxes;

		public int SelectedTileIndex { get; set; }

		public int SelectedChunkIndex { get; set; }
		public List<Tile> SelectedTiles
		{
			get
			{
				List<Tile> Tiles = new List<Tile>();

				foreach (var tile in TilesList.TileList.SelectedItems)
				{
					ushort Tile = (ushort)TilesList.Images.IndexOf(tile as System.Windows.Media.ImageSource);

					bool FlipX = PlacedTileCheckboxOptions[0].IsChecked.Value;
					bool FlipY = PlacedTileCheckboxOptions[1].IsChecked.Value;
					bool SolidTopA = PlacedTileCheckboxOptions[2].IsChecked.Value;
					bool SolidLrbA = PlacedTileCheckboxOptions[3].IsChecked.Value;
					bool SolidTopB = PlacedTileCheckboxOptions[4].IsChecked.Value;
					bool SolidLrbB = PlacedTileCheckboxOptions[5].IsChecked.Value;

					Tile = (ushort)ManiacEditor.Methods.Layers.TileFindReplace.SetBit(10, FlipX, Tile);
					Tile = (ushort)ManiacEditor.Methods.Layers.TileFindReplace.SetBit(11, FlipY, Tile);
					Tile = (ushort)ManiacEditor.Methods.Layers.TileFindReplace.SetBit(12, SolidTopA, Tile);
					Tile = (ushort)ManiacEditor.Methods.Layers.TileFindReplace.SetBit(13, SolidLrbA, Tile);
					Tile = (ushort)ManiacEditor.Methods.Layers.TileFindReplace.SetBit(14, SolidTopB, Tile);
					Tile = (ushort)ManiacEditor.Methods.Layers.TileFindReplace.SetBit(15, SolidLrbB, Tile);

					Tiles.Add(new Tile(Tile));
				}

				return Tiles.OrderBy(x => x.Index).ToList();
			}
		}
		public ushort SelectedTile
		{
			get
			{
				ushort Tile = (ushort)TilesList.SelectedIndex;

				bool FlipX = PlacedTileCheckboxOptions[0].IsChecked.Value;
				bool FlipY = PlacedTileCheckboxOptions[1].IsChecked.Value;
				bool SolidTopA = PlacedTileCheckboxOptions[2].IsChecked.Value;
				bool SolidLrbA = PlacedTileCheckboxOptions[3].IsChecked.Value;
				bool SolidTopB = PlacedTileCheckboxOptions[4].IsChecked.Value;
				bool SolidLrbB = PlacedTileCheckboxOptions[5].IsChecked.Value;

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
				ManiacEditor.Extensions.ConsoleExtensions.Print(ex.ToString());
			}

		}


		private void SetCheckboxDefaults()
		{
			SelectedTilesCheckboxOptions[0] = SelectedTileOptionFlipX;
			SelectedTilesCheckboxOptions[1] = SelectedTileOptionFlipY;
			SelectedTilesCheckboxOptions[2] = SelectedTileOptionSolidTop_A;
			SelectedTilesCheckboxOptions[3] = SelectedTileOptionSolidLRB_A;
			SelectedTilesCheckboxOptions[4] = SelectedTileOptionSolidTop_B;
			SelectedTilesCheckboxOptions[5] = SelectedTileOptionSolidLRB_B;

			PlacedTileCheckboxOptions[0] = PlacedTileOptionFlipX;
			PlacedTileCheckboxOptions[1] = PlacedTileOptionFlipY;
			PlacedTileCheckboxOptions[2] = PlacedTileOptionSolidTop_A;
			PlacedTileCheckboxOptions[3] = PlacedTileOptionSolidLRB_A;
			PlacedTileCheckboxOptions[4] = PlacedTileOptionSolidTop_B;
			PlacedTileCheckboxOptions[5] = PlacedTileOptionSolidLRB_B;
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

			this.TilesList.TileList.AllowDrop = true;
            this.TilesList.TileList.MouseMove += TileList_MouseMove;
            this.TilesList.TileList.PreviewMouseLeftButtonDown += TileList_PreviewMouseLeftButtonDown;

			RefreshThemeColors();

			TileViewer.Children.Add(TilesList);
			ChunksPage.Children.Add(ChunkList);
		}



        private void UpdateShortcuts(bool Show = false)
		{
			if (Show)
			{
				PlacedTileOptionFlipX.Content = "Flip X - " + "ControlKey";
				PlacedTileOptionFlipY.Content = "Flip Y - " + "ShiftKey";
			}
			else
			{
				PlacedTileOptionFlipX.Content = "Flip X";
				PlacedTileOptionFlipY.Content = "Flip Y";
			}
		}
		private void SetDropdownItemsState(bool enabled)
		{
			AutoGenerateChunksSingle.IsEnabled = enabled;
			AutoGenerateChunks.IsEnabled = enabled;
			SaveChunksManually.IsEnabled = enabled;
		}

		private void SetTilesDropdownItemsState(bool enabled)
		{
			AllowMultiTileSelectMenuItem.IsEnabled = enabled;
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

				if (ManiacEditor.Methods.Solution.SolutionState.Main.IsChunksEdit() && this.ChunkList != null)
				{
					ExtraOptionsButton.Visibility = Visibility.Visible;
					ExtraTileOptionsButton.Visibility = Visibility.Collapsed;
					SelectedTileLabel.Text = "Selected Chunk: " + ChunkList.SelectedIndex.ToString();
					if (TabControl.SelectedIndex != 1) TabControl.SelectedIndex = 1;
				}
				else if (this.TilesList != null)
				{
					ExtraOptionsButton.Visibility = Visibility.Collapsed;
					ExtraTileOptionsButton.Visibility = Visibility.Visible;
					SelectedTileLabel.Text = "Selected Tile: " + TilesList.SelectedIndex.ToString();
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

		#region Drag/Drop

		private void TileList_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			DragStartPoint = e.GetPosition(null);
		}

		private void TileList_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
		{
			if (AllowMultiTileSelectMenuItem.IsChecked) return;

			// Get the current mouse position
			Point mousePos = e.GetPosition(null);
			Vector diff = DragStartPoint - mousePos;

			if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed && (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
			{
				// Get the dragged ListViewItem
				ListView listView = sender as ListView;
				ListViewItem listViewItem = FindAnchestor<ListViewItem>((DependencyObject)e.OriginalSource);
				if (listViewItem == null) return;
				DataObject dragData = new DataObject(typeof(Int32), this.TilesList.TileList.Items.IndexOf(listViewItem));
				DragDrop.DoDragDrop(listViewItem, dragData, DragDropEffects.Copy | DragDropEffects.Move);
			}
		}

		// Helper to search up the VisualTree
		private static T FindAnchestor<T>(DependencyObject current) where T : DependencyObject 
		{
			do
			{
				if (current is T)
				{
					return (T)current;
				}
				current = System.Windows.Media.VisualTreeHelper.GetParent(current);
			}
			while (current != null);
			return null;
		}

		private Point DragStartPoint { get; set; } = new Point();

		#endregion

		#region List Reload Methods

		public void UpdateChunksListIfNeeded()
		{
			if (CurrentMultiLayerState == false && Methods.Solution.CurrentSolution.EditLayerB != null)
			{
				CurrentMultiLayerState = true;
				ChunksReload();
			}
			else if (CurrentMultiLayerState == true && Methods.Solution.CurrentSolution.EditLayerB == null)
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
				ChunkList.Invalidate(true);
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

		private void PlaceTileMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (TilesList.TileList.SelectedItems.Count > 1 && MultiTileDoubleClick != null)
			{
				var SelectedItems = SelectedTiles;
				int StartIndex = GetStartIndex((ushort)(SelectedItems[0].Index & 0x3ff), TilesList.ModelView.ItemColumns);
				MultiTileDoubleClick(new Tuple<List<ushort>, int[]>(SelectedItems.ConvertAll(x => x.RawData), new int[] { TilesList.ModelView.ItemColumns, StartIndex }));
			}
			else if (SelectedTileIndex != -1 && TileDoubleClick != null)
			{
				TileDoubleClick(SelectedTile);
			}


			int GetStartIndex(int Index, int Columns)
			{
				return Index % Columns;
			}
		}
		private void TilesList_MouseDoubleClick(object sender, System.Windows.Input.MouseEventArgs e)
		{
			if (isDisposing) return;
			if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
			{
				PlaceTileMenuItem_Click(sender, e);
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
		private void ExtraTileOptionsButton_Click(object sender, RoutedEventArgs e)
		{
			ExtraTileOptionsButton.ContextMenu.IsOpen = true;

			if (!ManiacEditor.Methods.Solution.SolutionState.Main.IsTilesEdit()) SetTilesDropdownItemsState(false);
			else SetTilesDropdownItemsState(true);
		}

		private void AllowMultiTileSelectMenuItem_Checked(object sender, RoutedEventArgs e)
		{
			if (TilesList != null)
			{
				if (AllowMultiTileSelectMenuItem.IsChecked && TilesList.TileList.SelectionMode != SelectionMode.Extended)
				{
					TilesList.TileList.SelectionMode = SelectionMode.Extended;
					TilesList.SelectedIndex = 0;
				}
				else if (TilesList.TileList.SelectionMode != SelectionMode.Single)
				{
					TilesList.TileList.SelectionMode = SelectionMode.Single;
					TilesList.SelectedIndex = 0;
				}
			}
		}
		private void SaveChunksManually_Click(object sender, RoutedEventArgs e)
		{
			Instance.Chunks?.Save();
		}
		private void AutoGenerateChunks_Click(object sender, RoutedEventArgs e)
		{
			if (Methods.Solution.CurrentSolution.EditLayerA != null && Methods.Solution.CurrentSolution.EditLayerB != null)
			{
				Instance.Chunks.AutoGenerateChunks(Methods.Solution.CurrentSolution.EditLayerA, Methods.Solution.CurrentSolution.EditLayerB);
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
			if (Methods.Solution.CurrentSolution.EditLayerA != null)
			{
				Instance.Chunks.AutoGenerateChunks(Methods.Solution.CurrentSolution.EditLayerA);
				ChunksReload();
			}
		}
		private void ExtraOptionsButton_Click(object sender, RoutedEventArgs e)
		{
			ExtraOptionsButton.ContextMenu.IsOpen = true;

			if (!ManiacEditor.Methods.Solution.SolutionState.Main.IsChunksEdit()) SetDropdownItemsState(false);
			else SetDropdownItemsState(true);
		}
		#endregion

		#region Common Methods

		private void SetDefaults()
		{
			TileZoomTrackbar.Value = Properties.Settings.MyDefaults.TilesDefaultZoom;
			ChunkZoomTrackbar.Value = Properties.Settings.MyDefaults.ChunksDefaultZoom;

			PlacedTileOptionSolidTop_A.IsChecked = Properties.Settings.MyDefaults.SolidTopADefault;
			PlacedTileOptionSolidLRB_A.IsChecked = Properties.Settings.MyDefaults.SolidAllButTopADefault;
			PlacedTileOptionSolidTop_B.IsChecked = Properties.Settings.MyDefaults.SolidTopBDefault;
			PlacedTileOptionSolidLRB_B.IsChecked = Properties.Settings.MyDefaults.SolidAllButTopBDefault;
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
			PlacedTileCheckboxOptions[option].IsChecked = state;
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

		private void PlaceOptionFlipX(object sender, RoutedEventArgs e)
		{
			TilesFlipHorizontal = PlacedTileOptionFlipX.IsChecked.Value;
		}
		private void PlaceOptionFlipY(object sender, RoutedEventArgs e)
		{
			TilesFlipVertical = PlacedTileOptionFlipY.IsChecked.Value;
		}
		private void SelectTileOptionFlipX(object sender, RoutedEventArgs e)
		{
			if (!setCheckboxes) TileOptionChanged?.Invoke(0, SelectedTileOptionFlipX.IsChecked.Value);
		}

		private void SelectTileOptionFlipY(object sender, RoutedEventArgs e)
		{
			if (!setCheckboxes) TileOptionChanged?.Invoke(1, SelectedTileOptionFlipY.IsChecked.Value);
		}
		private void SelectOptionSolidTopA(object sender, RoutedEventArgs e)
		{
			if (!setCheckboxes) TileOptionChanged?.Invoke(2, SelectedTileOptionSolidTop_A.IsChecked.Value);
		}
		private void SelectOptionSolidLRB_A(object sender, RoutedEventArgs e)
		{
			if (!setCheckboxes) TileOptionChanged?.Invoke(3, SelectedTileOptionSolidLRB_A.IsChecked.Value);
		}
		private void SelectOptionSolidTopB(object sender, RoutedEventArgs e)
		{
			if (!setCheckboxes) TileOptionChanged?.Invoke(4, SelectedTileOptionSolidTop_B.IsChecked.Value);
		}
		private void SelectOptionSolidLRB_B(object sender, RoutedEventArgs e)
		{
			if (!setCheckboxes) TileOptionChanged?.Invoke(5, SelectedTileOptionSolidLRB_B.IsChecked.Value);
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
		private void TileContextMenu_ContextMenuOpening(object sender, ContextMenuEventArgs e)
		{
			UpdateTilesContextMenu();
		}

		private void ChunksContextMenu_ContextMenuOpening(object sender, ContextMenuEventArgs e)
		{
			UpdateChunksContextMenu();
		}

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
				PlaceTileMenuItem.IsEnabled = true;
				if (TilesList.TileList.SelectedItems.Count > 1)
				{
					PlaceTileMenuItem.Header = "Place Tiles...";
					EditTileCollisionMenuItem.IsEnabled = false;
					EditTileCollisionMenuItem.Header = string.Format(EditTileCollisionMenuItem.Tag.ToString(), "N/A");
				}
				else
				{
					PlaceTileMenuItem.Header = "Place Tile...";
					EditTileCollisionMenuItem.IsEnabled = true;
					EditTileCollisionMenuItem.Header = string.Format(EditTileCollisionMenuItem.Tag.ToString(), "#" + SelectedTileIndex);
				}


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
			if (Methods.Solution.SolutionClipboard.TilesClipboard != null)
			{
				Instance.Chunks.ConvertClipboardtoMultiLayerChunk(Methods.Solution.SolutionClipboard.TilesClipboard);

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
			if (Methods.Solution.CurrentSolution.TileConfig != null && Methods.Solution.CurrentSolution.CurrentTiles != null)
			{
				if (Instance.TileManiacInstance.Visibility != System.Windows.Visibility.Visible || Instance.TileManiacInstance.TileConfig == null)
				{
					Instance.TileManiacInstance.LoadTileConfigViaIntergration(Methods.Solution.CurrentSolution.TileConfig, ManiacEditor.Methods.Solution.SolutionPaths.TileConfig_Source.ToString(), SelectedTileIndex);
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


	}
}
