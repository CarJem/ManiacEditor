using System;
using System.Windows;
using System.Windows.Controls;
using RSDKv5;
using System.Diagnostics;

namespace ManiacEditor
{
	/// <summary>
	/// Interaction logic for TilesToolbar.xaml
	/// </summary>
	public partial class TilesToolbar : UserControl
	{
		public Editor EditorInstance;

		private TilesList tilesList;
		bool disposing = false;
		bool updateList = false;

		public System.Windows.Forms.Panel tilePanel;
		public Interfaces.RetroEDTileList retroEDTileList1;

		public System.Windows.Forms.Integration.WindowsFormsHost host;
		public System.Windows.Forms.Integration.WindowsFormsHost host3;
		public Action<int> TileDoubleClick
		{
			get
			{
				return tilesList.TileDoubleClick;
			}
			set
			{
				tilesList.TileDoubleClick = value;
			}
		}

		public Action<int, bool> TileOptionChanged;

		CheckBox[] selectTileOptionsCheckboxes = new CheckBox[6];
		CheckBox[] tileOptionsCheckboxes = new CheckBox[4];
		bool setCheckboxes;

		public int SelectedTile
		{
			get
			{
				if (disposing) return -1;
				int res = tilesList.SelectedTile;
				for (int i = 0; i < selectTileOptionsCheckboxes.Length; ++i)
					if (selectTileOptionsCheckboxes[i].IsChecked.Value) res |= 1 << (10 + i);
				return res;

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

		public void SetSelectTileOption(int option, bool state)
		{
			selectTileOptionsCheckboxes[option].IsChecked = state;
		}

		public void SetTileOptionState(int option, TileOptionState state)
		{
			setCheckboxes = true;
			switch (state)
			{
				case TileOptionState.Disabled:
					tileOptionsCheckboxes[option].IsEnabled = false;
					tileOptionsCheckboxes[option].IsChecked = false;
					break;
				case TileOptionState.Checked:
					tileOptionsCheckboxes[option].IsEnabled = true;
					tileOptionsCheckboxes[option].IsChecked = true;
					break;
				case TileOptionState.Unchcked:
					tileOptionsCheckboxes[option].IsEnabled = true;
					tileOptionsCheckboxes[option].IsChecked = false;
					break;
				case TileOptionState.Indeterminate:
					tileOptionsCheckboxes[option].IsEnabled = true;
					tileOptionsCheckboxes[option].IsChecked = null;
					break;
			}
			setCheckboxes = false;
		}

		public TilesToolbar(StageTiles tiles, String data_directory, String Colors, Editor instance)
		{
			InitializeComponent();
			SetupTilesList(instance);

			EditorInstance = instance;

			tileOptionsCheckboxes[0] = tileOption1;
			tileOptionsCheckboxes[1] = tileOption2;
			tileOptionsCheckboxes[2] = tileOption3;
			tileOptionsCheckboxes[3] = tileOption4;

			selectTileOptionsCheckboxes[0] = option1CheckBox;
			selectTileOptionsCheckboxes[1] = option2CheckBox;
			selectTileOptionsCheckboxes[2] = option3CheckBox;
			selectTileOptionsCheckboxes[3] = option4CheckBox;
			selectTileOptionsCheckboxes[4] = option5CheckBox;
			selectTileOptionsCheckboxes[5] = option6CheckBox;

			RSDKv5.GIF tileGridImage = new GIF((data_directory + "\\16x16Tiles.gif"), Colors);
			tilesList.TilesImage = tileGridImage;
			tilesList.TileScale = 1 << (int)trackBar1.Value;

			UpdateShortcuts();

			ChunksReload();
		}

		public void SetupTilesList(Editor instance)
		{
			host = new System.Windows.Forms.Integration.WindowsFormsHost();
			host3 = new System.Windows.Forms.Integration.WindowsFormsHost();
			this.retroEDTileList1 = new ManiacEditor.Interfaces.RetroEDTileList();
			this.tilesList = new ManiacEditor.TilesList(instance);
			this.tilePanel = new System.Windows.Forms.Panel();
			// 
			// retroEDTileList1
			// 
			this.retroEDTileList1.BackColor = System.Drawing.SystemColors.Window;
			this.retroEDTileList1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.retroEDTileList1.ImageHeight = 128;
			this.retroEDTileList1.ImageSize = 128;
			this.retroEDTileList1.ImageWidth = 128;
			this.retroEDTileList1.Location = new System.Drawing.Point(3, 3);
			this.retroEDTileList1.Name = "retroEDTileList1";
			this.retroEDTileList1.ScrollValue = 0;
			this.retroEDTileList1.SelectedIndex = -1;
			this.retroEDTileList1.Size = new System.Drawing.Size(234, 290);
			this.retroEDTileList1.TabIndex = 1;

			// 
			// tilePanel
			// 
			this.tilePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
			| System.Windows.Forms.AnchorStyles.Left)
			| System.Windows.Forms.AnchorStyles.Right)));
			this.tilePanel.BackColor = System.Drawing.SystemColors.Control;
			this.tilePanel.Location = new System.Drawing.Point(0, 45);
			this.tilePanel.Name = "tilePanel";
			this.tilePanel.Size = new System.Drawing.Size(241, 253);
			this.tilePanel.TabIndex = 2;
			this.tilePanel.Dock = System.Windows.Forms.DockStyle.Fill;


			host.Child = tilePanel;
			host3.Child = retroEDTileList1;
			TileViewer.Children.Add(host);
			ChunksPage.Children.Add(host3);


			this.tilesList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
| System.Windows.Forms.AnchorStyles.Left)
| System.Windows.Forms.AnchorStyles.Right)));
			this.tilesList.BackColor = System.Drawing.SystemColors.Window;
			this.tilesList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tilesList.FlipHorizontal = false;
			this.tilesList.FlipVertical = false;
			this.tilesList.ForeColor = System.Drawing.SystemColors.ControlText;
			this.tilesList.Location = new System.Drawing.Point(0, 0);
			this.tilesList.Name = "tilesList";
			this.tilesList.Size = new System.Drawing.Size(241, 253);
			this.tilesList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tilesList.TabIndex = 0;
			this.tilesList.TileScale = 2;
			this.tilePanel.Controls.Add(this.tilesList);
		}

		private void trackBar1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (disposing) return;
			tilesList.TileScale = 1 << (int)trackBar1.Value;
		}

		public void Reload(string colors = null)
		{
			if (disposing) return;
			tilesList.Reload(colors);
			ChunksReload();
		}

		public void ChunksReload()
		{
			if (disposing) return;
			retroEDTileList1.Images.Clear();
			EditorInstance.EditorChunk.DisposeTextures();

			for (int i = 0; i < EditorInstance.EditorChunk.StageStamps.StampList.Count; i++)
			{
				retroEDTileList1.Images.Add(EditorInstance.EditorChunk.GetChunkTexture(i));
			}
			retroEDTileList1.Refresh();
		}

		public void Dispose()
		{
			disposing = true;
			if (tilesList != null)
			{
				if (tilesList.graphicPanel != null)
				{
					tilesList.graphicPanel.Dispose();
					tilesList.graphicPanel = null;
				}
				tilesList.Controls.Clear();
				tilesList.Dispose();
				tilesList = null;
			}
			if (tilePanel != null)
			{
				tilePanel.Controls.Clear();
				tilePanel.Dispose();
				tilePanel = null;
			}
			if (retroEDTileList1 != null)
			{
				retroEDTileList1.Dispose();
				retroEDTileList1 = null;
			}
			if (host != null)
			{
				host.Child.Dispose();
				host = null;
			}
			if (host3 != null)
			{
				host3.Child.Dispose();
				host3 = null;
			}


		}

		private void TilesToolbar_Load(object sender, RoutedEventArgs e)
		{
			Debug.WriteLine("Send to debug output.");
			trackBar1.Value = Properties.Settings.Default.tileToolbarDefaultZoomLevel;

		}

		private void option1CheckBox_CheckedChanged(object sender, RoutedEventArgs e)
		{
			tilesList.FlipHorizontal = option1CheckBox.IsChecked.Value;
		}

		private void option2CheckBox_CheckedChanged(object sender, RoutedEventArgs e)
		{
			tilesList.FlipVertical = option2CheckBox.IsChecked.Value;
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

		private void UpdateShortcuts(bool show = false)
		{
			if (show)
			{
				option1CheckBox.Content = "Flip Horizontal " + Environment.NewLine + EditorInstance.EditorControls.KeyBindPraser("FlipHTiles", true);
				option2CheckBox.Content = "Flip Vertical " + Environment.NewLine + EditorInstance.EditorControls.KeyBindPraser("FlipVTiles", true);
			}
			else
			{
				option1CheckBox.Content = "Flip Horizontal" + Environment.NewLine + string.Format("({0} - Selected Only)", EditorInstance.EditorControls.KeyBindPraser("FlipH"));
				option2CheckBox.Content = "Flip Vertical" + Environment.NewLine + string.Format("({0} - Selected Only)", EditorInstance.EditorControls.KeyBindPraser("FlipV"));
			}
		}

		private void trackBar1_Scroll(object sender, EventArgs e)
		{

		}
		public void RefreshTileSelected()
		{
			EditorInstance.TilesToolbar.SelectedTileLabel.Content = "Selected Tile: " + EditorInstance.ToolbarSelectedTile;
		}

		private void editTileInTileManiacToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (EditorInstance.mainform == null || EditorInstance.mainform.IsClosed) EditorInstance.mainform = new TileManiacWPF.MainWindow();
			if (EditorInstance.mainform.Visibility != Visibility.Visible)
			{
				EditorInstance.mainform.Show();
			}
			EditorInstance.mainform.SetIntergrationNightMode(Properties.Settings.Default.NightMode);
			if (EditorInstance.TilesConfig != null && EditorInstance.StageTiles != null)
			{
				if (EditorInstance.mainform.Visibility != Visibility.Visible || EditorInstance.mainform.tcf == null)
				{
					EditorInstance.mainform.LoadTileConfigViaIntergration(EditorInstance.TilesConfig, EditorInstance.SceneFilepath, SelectedTile);
				}
				else
				{
					EditorInstance.mainform.SetCollisionIndex(SelectedTile);
					EditorInstance.mainform.Activate();
				}

			}

		}

		private void tabControl1_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.Source is TabControl)
			{
				if (TabControl.SelectedIndex == 0) EditorInstance.ChunksToolButton.IsChecked = false;
				else EditorInstance.ChunksToolButton.IsChecked = true;
			}
			if (TabControl.SelectedIndex == 0)
			{
				tilesList.graphicPanel.Render();
			}

		}

		private void TilesToolbar_Resize(object sender, EventArgs e)
		{


		}

		private void selectedTileLabel_Click(object sender, EventArgs e)
		{

		}

		private void UserControl_Unloaded(object sender, RoutedEventArgs e)
		{
			//this.Dispose();
		}

		private void TabItem_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			if (TabControl.SelectedIndex == 0)
			{
				tilesList.graphicPanel.Render();
			}
		}
	}
}
