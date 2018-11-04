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
    public partial class DeviceLostBox : Form
    {
        public DeviceLostBox(String ex, String ex2, String code, String code2, int state = 0)
        {
            var newLine = Environment.NewLine;
            InitializeComponent();

            System.IO.Stream str = Properties.Resources.OhNo;
            System.Media.SoundPlayer snd = new System.Media.SoundPlayer(str);
            snd.Play();
            if (state == 1)
            {
                outOfMemory();
            }
            textBox1.Text = "First Error (This is the one that matters)" + newLine + newLine + ex + newLine + newLine + "Second Error(This one really doesn't matter much)" + newLine + newLine + ex2;
            errorLabel.Text = "Error Code: " + code + Environment.NewLine + "Secondary Error Code(Mostly Unimportant): " + code2; ;
        }

        public void outOfMemory() 
        {
            button2.Enabled = false;
            button3.Enabled = false;
            label1.Text = Properties.EditorState.Default.deviceCrashedString;
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void showDetailsCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (showDetailsCheckbox.Checked == false)
            {
                textBox1.Visible = false;
                this.Height = 210;
            }
            else
            {
                textBox1.Visible = true;
                this.Height = 420;
            }

        }
    }
}
