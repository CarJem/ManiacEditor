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
        //Shorthanding Setting Files
        Properties.Settings mySettings = Properties.Settings.Default;
        Properties.KeyBinds myKeyBinds = Properties.KeyBinds.Default;

        public FirstTimeSetup()
        {
            InitializeComponent();
        }

        private void DevWarningBox_Load(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (minimalOption.Checked)
            {
                mySettings.EditorGeneralConfigMode = "Minimal";
                mySettings.ShowFirstTimeSetup = false;
            }
            else if (basicOption.Checked)
            {
                mySettings.EditorGeneralConfigMode = "Basic";
                mySettings.ShowFirstTimeSetup = false;
            }
            else if (superOption.Checked)
            {
                mySettings.EditorGeneralConfigMode = "Super";
                mySettings.ShowFirstTimeSetup = false;
            }
            else if (hyperOption.Checked)
            {
                mySettings.EditorGeneralConfigMode = "Hyper";
                mySettings.ShowFirstTimeSetup = false;
            }
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

        private void applyMinimalDefaults()
        {
            mySettings.DisableRenderExlusions = true;
            mySettings.NeverLoadEntityTextures = true;
            mySettings.ReduceZoom = true;
            mySettings.SimplifiedWaterLevelRendering = true;
            mySettings.AnimationsDefault = false;
        }

        private void applyBasicDefaults()
        {
            mySettings.DisableRenderExlusions = true;
            mySettings.ReduceZoom = true;
            mySettings.SimplifiedWaterLevelRendering = true;
            mySettings.AnimationsDefault = false;
            mySettings.AllowMoreRenderUpdates = true;
            mySettings.allowForSmoothSelection = true;
            mySettings.PrioritizedObjectRendering = false;
        }

        private void applySuperDefaults()
        {
            mySettings.DisableRenderExlusions = false;
            mySettings.ReduceZoom = false;
            mySettings.SimplifiedWaterLevelRendering = false;
            mySettings.AllowMoreRenderUpdates = true;
            mySettings.allowForSmoothSelection = true;
            mySettings.PrioritizedObjectRendering = true;
            mySettings.preRenderTURBOMode = false;
        }

        private void applyHyperDefaults()
        {
            mySettings.DisableRenderExlusions = false;
            mySettings.ReduceZoom = false;
            mySettings.SimplifiedWaterLevelRendering = false;
            mySettings.AllowMoreRenderUpdates = true;
            mySettings.allowForSmoothSelection = true;
            mySettings.PrioritizedObjectRendering = true;
            mySettings.preRenderTURBOMode = true;
        }
    }
}
