using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RSDKv5;
using System.Diagnostics;
using TileManiac;

namespace ManiacEditor
{
    public partial class TilesToolbar : UserControl
    {
        public Editor EditorInstance;

        private TilesList tilesList;
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
            get {
                int res = tilesList.SelectedTile;
                for (int i = 0; i < selectTileOptionsCheckboxes.Length; ++i)
                    if (selectTileOptionsCheckboxes[i].Checked) res |= 1 << (10 + i);
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
            selectTileOptionsCheckboxes[option].Checked = state;
        }

        public void SetTileOptionState(int option, TileOptionState state)
        {
            setCheckboxes = true;
            switch (state)
            {
                case TileOptionState.Disabled:
                    tileOptionsCheckboxes[option].AutoCheck = false;
					tileOptionsCheckboxes[option].ForeColor = SystemColors.GrayText;
					tileOptionsCheckboxes[option].Checked = false;
                    break;
                case TileOptionState.Checked:
                    tileOptionsCheckboxes[option].Enabled = true;
					tileOptionsCheckboxes[option].AutoCheck = true;
					tileOptionsCheckboxes[option].ForeColor = SystemColors.ControlText;
					tileOptionsCheckboxes[option].CheckState = CheckState.Checked;
                    break;
                case TileOptionState.Unchcked:
                    tileOptionsCheckboxes[option].Enabled = true;
					tileOptionsCheckboxes[option].AutoCheck = true;
					tileOptionsCheckboxes[option].ForeColor = SystemColors.ControlText;
					tileOptionsCheckboxes[option].CheckState = CheckState.Unchecked;
                    break;
                case TileOptionState.Indeterminate:
                    tileOptionsCheckboxes[option].Enabled = true;
					tileOptionsCheckboxes[option].AutoCheck = true;
					tileOptionsCheckboxes[option].ForeColor = SystemColors.ControlText;
					tileOptionsCheckboxes[option].CheckState = CheckState.Indeterminate;
                    break;
            }
            setCheckboxes = false;
        }

        public TilesToolbar(StageTiles tiles, String data_directory, String Colors, Editor instance)
        {
            InitializeComponent();

            EditorInstance = instance;
            SetupTilesList(instance);

            if (Properties.Settings.Default.NightMode)
            {
                trackBar1.BackColor = Editor.darkTheme1;
                tilesList.BackColor = Editor.darkTheme1;
                groupBox1.ForeColor = Editor.darkTheme3;
            }

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
            tilesList.TileScale = 1 << trackBar1.Value;

            UpdateShortcuts();

            ChunksReload();
        }

        public void SetupTilesList(Editor instance)
        {

            this.tilesList = new ManiacEditor.TilesList(instance);
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
            this.tilesList.Size = tilePanel.Size;
            this.tilesList.Dock = DockStyle.Fill;
            this.tilesList.TabIndex = 0;
            this.tilesList.TileScale = 2;
            this.tilePanel.Controls.Add(this.tilesList);
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            tilesList.TileScale = 1 << trackBar1.Value;
        }

        public void Reload(string colors = null)
        {
            tilesList.Reload(colors);
            ChunksReload();
        }

        public void ChunksReload()
        {
            retroEDTileList1.Images.Clear();
            EditorInstance.EditorChunk.DisposeTextures();

            for (int i = 0; i < EditorInstance.EditorChunk.StageStamps.StampList.Count; i++)
            {
                retroEDTileList1.Images.Add(EditorInstance.EditorChunk.GetChunkTexture(i));
            }
            retroEDTileList1.Refresh();
        }

        public new void Dispose()
        {
            retroEDTileList1.Dispose();
            tilesList.Dispose();
            base.Dispose();
        }

        private void TilesToolbar_Load(object sender, EventArgs e)
        {
            Debug.WriteLine("Send to debug output.");
            trackBar1.Value = Properties.Settings.Default.tileToolbarDefaultZoomLevel;

        }

        private void option1CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            tilesList.FlipHorizontal = option1CheckBox.Checked;
        }

        private void option2CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            tilesList.FlipVertical = option2CheckBox.Checked;
        }

        private void tileOption1_CheckedChanged(object sender, EventArgs e)
        {
            if (!setCheckboxes) TileOptionChanged?.Invoke(0, tileOption1.Checked);
        }

        private void tileOption2_CheckedChanged(object sender, EventArgs e)
        {
            if (!setCheckboxes) TileOptionChanged?.Invoke(1, tileOption2.Checked);
        }

        private void tileOption3_CheckedChanged(object sender, EventArgs e)
        {
            if (!setCheckboxes) TileOptionChanged?.Invoke(2, tileOption3.Checked);
        }

        private void tileOption4_CheckedChanged(object sender, EventArgs e)
        {
            if (!setCheckboxes) TileOptionChanged?.Invoke(3, tileOption4.Checked);
        }

        private void UpdateShortcuts(bool show=false)
        {
            if (show)
            {
                option1CheckBox.Text = "Flip Horizontal " + Environment.NewLine + EditorInstance.EditorControls.KeyBindPraser("FlipHTiles", true);
				option2CheckBox.Text = "Flip Vertical " + Environment.NewLine + EditorInstance.EditorControls.KeyBindPraser("FlipVTiles", true);
			}
            else
            {
                option1CheckBox.Text = "Flip Horizontal" + Environment.NewLine + string.Format("({0} - Selected Only)", EditorInstance.EditorControls.KeyBindPraser("FlipH"));
				option2CheckBox.Text = "Flip Vertical" + Environment.NewLine + string.Format("({0} - Selected Only)", EditorInstance.EditorControls.KeyBindPraser("FlipV"));
			}
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {

        }
        public void RefreshTileSelected()
        {
            EditorInstance.TilesToolbar.selectedTileLabel.Text = "Selected Tile: " + EditorInstance.ToolbarSelectedTile;
        }

        private void editTileInTileManiacToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (EditorInstance.mainform.IsDisposed) EditorInstance.mainform = new TileManiac.Mainform();
            if (!EditorInstance.mainform.Visible)
            {
                EditorInstance.mainform.Show();
            }
            if (EditorInstance.TilesConfig != null && EditorInstance.StageTiles != null)
            {
                if (!EditorInstance.mainform.Visible || EditorInstance.mainform.tcf == null)
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

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tabControl1.TabPages[0]) EditorInstance.ChunksToolButton.IsChecked = false;
            else EditorInstance.ChunksToolButton.IsChecked = true;
        }

        private void TilesToolbar_Resize(object sender, EventArgs e)
        {


        }

        private void selectedTileLabel_Click(object sender, EventArgs e)
        {
		
        }
    }

}
