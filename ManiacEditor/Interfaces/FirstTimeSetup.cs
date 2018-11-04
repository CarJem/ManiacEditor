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
    public partial class FirstTimeSetup : Form
    {
        public FirstTimeSetup()
        {
            InitializeComponent();
        }

        private void DevWarningBox_Load(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void radioButton1_MouseHover(object sender, EventArgs e)
        {
            modeLabel.Text = "Minimal - The very lowest you can go if you are don't have enough power.";
        }

        private void radioButton2_MouseHover(object sender, EventArgs e)
        {
            modeLabel.Text = "Basic - A step up from minimal. Take caution x86 users!";
        }

        private void radioButton3_MouseHover(object sender, EventArgs e)
        {
            modeLabel.Text = "Super - The best feature set without killing it! NOT FOR x86 USERS!";
        }

        private void radioButton4_MouseHover(object sender, EventArgs e)
        {
            modeLabel.Text = "Hyper - Kicking things into OVERDRIVE! NOT FOR x86 USERS";
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
