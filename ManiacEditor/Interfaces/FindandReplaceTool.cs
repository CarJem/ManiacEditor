using RSDKv5;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ManiacEditor.Interfaces
{
    public partial class FindandReplaceTool : Form
    {
        int value = 0;
        int replaceValue = 0;
        bool replaceMode = false;
        bool copyResults = false;
        int applyToState = 0;
        string specificLayer = "";
        bool readyState = false;
        bool perserveOptions = false;
        public FindandReplaceTool()
        {
           InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Int32.TryParse(textBox1.Text,out value);
            Int32.TryParse(textBox2.Text, out replaceValue);
            if (checkBox1.Checked)
            {
                replaceMode = true;
            }
            if (checkBox2.Checked)
            {
                copyResults = true;
            }
            if (radioButton1.Checked)
            {
                applyToState = 1; //All Layers
            }
            if (radioButton2.Checked)
            {
                applyToState = 2; //Specific Layer
            }
            if (radioButton3.Checked)
            {
                applyToState = 0; //Active Edit Layer Only
            }
            if (textBox3.Text != "")
            {
                textBox3.Text = specificLayer;
            }
            if (checkBox3.Checked)
            {
                perserveOptions = true;
            }
            readyState = true;
        }

        public int GetValue()
        {
            return value;
        }
        public bool GetReadyState()
        {
            return readyState;
        }
        public int GetReplaceValue()
        {
            return replaceValue;
        }

        public string GetSpecificLayer()
        {
            return specificLayer;
        }

        public bool IsReplaceMode()
        {
            return replaceMode;
        }

        public bool CopyResultsOrNot()
        {
            return copyResults;
        }

        public int GetApplyState()
        {
            return applyToState;
        }

        public bool PerservingCollision()
        {
            return perserveOptions;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                textBox3.Enabled = true;
            }
            else
            {
                textBox3.Enabled = false;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                textBox2.Enabled = true;
                label2.Enabled = true;
            }
            else
            {
                textBox2.Enabled = false;
                label2.Enabled = false;
            }
        }

        private void option2CheckBox_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void option1CheckBox_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox6.Checked)
            {
                option1CheckBox.Enabled = true;
                option2CheckBox.Enabled = true;
                option3CheckBox.Enabled = true;
                option4CheckBox.Enabled = true;
                option5CheckBox.Enabled = true;
                option6CheckBox.Enabled = true;
            }
            else
            {
                option1CheckBox.Enabled = false;
                option2CheckBox.Enabled = false;
                option3CheckBox.Enabled = false;
                option4CheckBox.Enabled = false;
                option5CheckBox.Enabled = false;
                option6CheckBox.Enabled = false;
            }
        }

        private void groupBox5_Enter(object sender, EventArgs e)
        {

        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }
    }
}
