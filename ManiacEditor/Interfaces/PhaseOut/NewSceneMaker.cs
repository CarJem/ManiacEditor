using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ManiacEditor.Interfaces
{
    public partial class NewSceneMaker : Form
    {
        public int Scene_Width = 128;
        public int Scene_Height = 128;
        public int BG_Width = 256;
        public int BG_Height = 256;
        public string SceneFolder;
        public NewSceneMaker()
        {
            InitializeComponent();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            Scene_Width = (int)numericUpDown1.Value;
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            Scene_Height = (int)numericUpDown2.Value;
        }

        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            BG_Width = (int)numericUpDown4.Value;
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            BG_Height = (int)numericUpDown3.Value;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FolderSelectDialog path = new FolderSelectDialog();
            path.ShowDialog();
            if (path.FileName != null)
            {
                SceneFolder = path.FileName;
                textBox1.Text = SceneFolder;
            }
        }
    }
}
