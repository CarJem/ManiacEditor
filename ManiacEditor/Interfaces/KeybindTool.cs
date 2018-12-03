using SharpDX;
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
    public partial class KeybindTool : Form
    {
        Keys CurrentBindingKey = Keys.None;
        bool ctrlChecked = false;
        bool altChecked = false;
        bool tabChecked = false;
        bool shiftChecked = false;

        const string ctrl = "CTRL";
        const string shift = "SHIFT";
        const string alt = "ALT";
        const string tab = "TAB";
        const string plus = " + ";

        public KeybindTool()
        {
            InitializeComponent();
            UpdateResultLabel();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (!(e.Alt || e.Control || e.Shift))
            {
                Keys keyData = e.KeyCode;
                CurrentBindingKey = keyData;
                textBox1.Text = "";
            }
            UpdateResultLabel();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox1.Text = CurrentBindingKey.ToString();
        }

        private void UpdateResultLabel()
        {
            resultLabel.Text = "Result: " + (ctrlChecked ? ctrl + plus : "") + (altChecked ? alt + plus : "") + (shiftChecked ? shift + plus : "") + (tabChecked ? tab + plus : "") + CurrentBindingKey.ToString();
        }

        private void ctrlBox_CheckedChanged(object sender, EventArgs e)
        {
            ctrlChecked = ctrlBox.Checked;
            UpdateResultLabel();
        }

        private void altBox_CheckedChanged(object sender, EventArgs e)
        {
            altChecked = altBox.Checked;
            UpdateResultLabel();
        }

        private void tabBOX_CheckedChanged(object sender, EventArgs e)
        {
            tabChecked = tabBox.Checked;
            UpdateResultLabel();
        }

        private void shiftBox_CheckedChanged(object sender, EventArgs e)
        {
            shiftChecked = shiftBox.Checked;
            UpdateResultLabel();
        }
    }
}
