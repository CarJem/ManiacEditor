using IronPython.Modules;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ManiacEditor
{
    public partial class GoToPositionBox : Form
    {
        public bool tilesMode = false;
        public int goTo_X = 0;
        public int goTo_Y = 0;
        public GoToPositionBox()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                tilesMode = true;
            }
            goTo_X = (int)numericUpDown1.Value;
            goTo_Y = (int)numericUpDown2.Value;
        }
    }
}
