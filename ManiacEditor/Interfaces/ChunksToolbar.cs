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

        public Action<int, bool> TileOptionChanged;

        CheckBox[] selectTileOptionsCheckboxes = new CheckBox[6];
        CheckBox[] tileOptionsCheckboxes = new CheckBox[4];


       

        public ChunksToolbar(StageTiles tiles, String data_directory, String Colors)
        {
            InitializeComponent();
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
