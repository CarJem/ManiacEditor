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
                    tileOptionsCheckboxes[option].Enabled = false;
                    tileOptionsCheckboxes[option].Checked = false;
                    break;
                case TileOptionState.Checked:
                    tileOptionsCheckboxes[option].Enabled = true;
                    tileOptionsCheckboxes[option].CheckState = CheckState.Checked;
                    break;
                case TileOptionState.Unchcked:
                    tileOptionsCheckboxes[option].Enabled = true;
                    tileOptionsCheckboxes[option].CheckState = CheckState.Unchecked;
                    break;
                case TileOptionState.Indeterminate:
                    tileOptionsCheckboxes[option].Enabled = true;
                    tileOptionsCheckboxes[option].CheckState = CheckState.Indeterminate;
                    break;
            }
            setCheckboxes = false;
        }

        public TilesToolbar(StageTiles tiles, String data_directory, String Colors, Editor instance)
        {
            InitializeComponent(instance);
            EditorInstance = instance;

            if (Properties.Settings.Default.NightMode)
            {
                trackBar1.BackColor = EditorInstance.darkTheme1;
                tilesList.BackColor = EditorInstance.darkTheme1;
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
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            tilesList.TileScale = 1 << trackBar1.Value;
        }

        public void Reload(string colors = null)
        {
            tilesList.Reload(colors);
        }

        public new void Dispose()
        {
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
                option1CheckBox.Text = "Flip Horizontal (Ctrl)";
                option2CheckBox.Text = "Flip Vertical (Shift)";
            }
            else
            {
                option1CheckBox.Text = "Flip Horizontal";
                option2CheckBox.Text = "Flip Vertical";
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
    }

}
