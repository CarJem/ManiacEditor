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
    public partial class ChunksToolbar : UserControl
    {
        private ChunksList chunksList;
        public Action<int, bool> TileOptionChanged;

        CheckBox[] selectTileOptionsCheckboxes = new CheckBox[6];
        CheckBox[] tileOptionsCheckboxes = new CheckBox[4];


       

        public ChunksToolbar(StageTiles tiles, String data_directory, String Colors, Editor instance)
        {
            InitializeComponent();
            SetupChunksList(instance);


            RSDKv5.GIF tileGridImage = new GIF((data_directory + "\\16x16Tiles.gif"), Colors);
            chunksList.TilesImage = tileGridImage;
            chunksList.TileScale = 1;
        }

        public void SetupChunksList(Editor instance)
        {
            this.chunksList = new ManiacEditor.ChunksList(instance);
            this.chunksList.Dock = DockStyle.Fill;
            this.chunksList.BackColor = System.Drawing.SystemColors.Window;
            this.chunksList.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.chunksList.FlipHorizontal = false;
            this.chunksList.FlipVertical = false;
            this.chunksList.ForeColor = System.Drawing.SystemColors.ControlText;
            this.chunksList.Location = new System.Drawing.Point(3, 73);
            this.chunksList.Name = "tilesList";
            //this.chunksList.Size = new System.Drawing.Size(249, 267);
            //this.chunksList.TabIndex = 0;
            this.chunksList.TileScale = 2;
            this.tabPage1.Controls.Add(this.chunksList);
        }

        public new void Dispose()
        {
            
            base.Dispose();
        }

        private void TilesToolbar_Load(object sender, EventArgs e)
        {
            Debug.WriteLine("Send to debug output.");          
        }
    }

}
