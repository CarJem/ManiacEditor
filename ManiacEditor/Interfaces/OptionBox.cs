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
using System.Windows.Forms.PropertyGridInternal;

namespace ManiacEditor
{
    
    public partial class OptionBox : Form
    {
        bool preRenderRadioGroupCheckChangeAllowed = true;
        public OptionBox()
        {
            InitializeComponent();

            preRenderRadioGroupsUpdate(Properties.Settings.Default.preRenderSceneOption);
            preRenderRadioGroupCheckChangeAllowed = true;
            this.label21.Text = Properties.Settings.Default.FasterNudgeValue.ToString();
            this.gridSizeValueLabel.Text = Properties.Settings.Default.CustomGridSizeValue.ToString();
            if (Properties.Settings.Default.x16Default) uncheckOtherGridDefaults(1);
            if (Properties.Settings.Default.x128Default) uncheckOtherGridDefaults(2);
            if (Properties.Settings.Default.x256Default) uncheckOtherGridDefaults(3);
            if (Properties.Settings.Default.CustomGridDefault) uncheckOtherGridDefaults(4);
            initilizeKeyDefaults();

            CASANUD.Value = Properties.Settings.Default.CollisionSAColour.A;
            CASRNUD.Value = Properties.Settings.Default.CollisionSAColour.R;
            CASGNUD.Value = Properties.Settings.Default.CollisionSAColour.G;
            CASBNUD.Value = Properties.Settings.Default.CollisionSAColour.B;
            CTOSANUD.Value = Properties.Settings.Default.CollisionTOColour.A;
            CTOSRNUD.Value = Properties.Settings.Default.CollisionTOColour.R;
            CTOSGNUD.Value = Properties.Settings.Default.CollisionTOColour.G;
            CTOSBNUD.Value = Properties.Settings.Default.CollisionTOColour.B;
            CLRDSANUD.Value = Properties.Settings.Default.CollisionLRDColour.A;
            CLRDSRNUD.Value = Properties.Settings.Default.CollisionLRDColour.R;
            CLRDSGNUD.Value = Properties.Settings.Default.CollisionLRDColour.G;
            CLRDSBNUD.Value = Properties.Settings.Default.CollisionLRDColour.B;

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (SceneSelectRadio2.Checked == true)
            {
                Properties.Settings.Default.IsFilesViewDefault = true;
                Properties.Settings.Default.SceneSelectRadioButton1On = false;
                Properties.Settings.Default.SceneSelectRadioButton2On = true;
            }
            else
            {
                Properties.Settings.Default.IsFilesViewDefault = false;
                Properties.Settings.Default.SceneSelectRadioButton1On = true;
                Properties.Settings.Default.SceneSelectRadioButton2On = false;

            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (SceneSelectRadio2.Checked == true)
            {
                Properties.Settings.Default.IsFilesViewDefault = true;
                Properties.Settings.Default.SceneSelectRadioButton1On = false;
                Properties.Settings.Default.SceneSelectRadioButton2On = true;
            }
            else
            {
                Properties.Settings.Default.IsFilesViewDefault = false;
                Properties.Settings.Default.SceneSelectRadioButton1On = true;
                Properties.Settings.Default.SceneSelectRadioButton2On = false;

            }
        }

        private void radioButtonY_CheckedChanged_1(object sender, EventArgs e)
        {
            if (radioButtonY.Checked == true)
            {
                Properties.Settings.Default.ScrollLockDirection = true;
                Properties.Settings.Default.ScrollLockX = false;
                Properties.Settings.Default.ScrollLockY = true;
            }
            else
            {
                Properties.Settings.Default.ScrollLockDirection = false;
                Properties.Settings.Default.ScrollLockX = true;
                Properties.Settings.Default.ScrollLockY = false;

            }
        }

        private void radioButtonX_CheckedChanged_1(object sender, EventArgs e)
        {
            if (radioButtonY.Checked == true)
            {
                Properties.Settings.Default.ScrollLockDirection = true;
                Properties.Settings.Default.ScrollLockX = false;
                Properties.Settings.Default.ScrollLockY = true;
            }
            else
            {
                Properties.Settings.Default.ScrollLockDirection = false;
                Properties.Settings.Default.ScrollLockX = true;
                Properties.Settings.Default.ScrollLockY = false;

            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to wipe your settings?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Properties.Settings.Default.Reset();
            }
            else
            {
                
            }
        }

        private void nudgeValueDownMoreButton_Click(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.FasterNudgeValue <= 100 && Properties.Settings.Default.FasterNudgeValue >= 0)
                Properties.Settings.Default.FasterNudgeValue -= 5;
            if (Properties.Settings.Default.FasterNudgeValue <= 0)
                Properties.Settings.Default.FasterNudgeValue = 1;
            if (Properties.Settings.Default.FasterNudgeValue > 100)
                Properties.Settings.Default.FasterNudgeValue = 100;
            this.label21.Text = Properties.Settings.Default.FasterNudgeValue.ToString();
        }

        private void nudgeValueDownButton_Click(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.FasterNudgeValue <= 100 && Properties.Settings.Default.FasterNudgeValue >= 0)
                Properties.Settings.Default.FasterNudgeValue -= 1;
            if (Properties.Settings.Default.FasterNudgeValue <= 0)
                Properties.Settings.Default.FasterNudgeValue = 1;
            if (Properties.Settings.Default.FasterNudgeValue > 100)
                Properties.Settings.Default.FasterNudgeValue = 100;
            this.label21.Text = Properties.Settings.Default.FasterNudgeValue.ToString();
        }

        private void nudgeValueUpButton_Click(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.FasterNudgeValue != 100 && Properties.Settings.Default.FasterNudgeValue >= 0)
                Properties.Settings.Default.FasterNudgeValue += 1;
            if (Properties.Settings.Default.FasterNudgeValue <= 0)
                Properties.Settings.Default.FasterNudgeValue = 1;
            if (Properties.Settings.Default.FasterNudgeValue > 100)
                Properties.Settings.Default.FasterNudgeValue = 100;
            this.label21.Text = Properties.Settings.Default.FasterNudgeValue.ToString();
        }

        private void nudgeValueUpMoreButton_Click(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.FasterNudgeValue != 100 && Properties.Settings.Default.FasterNudgeValue >= 0)
                Properties.Settings.Default.FasterNudgeValue += 5;
            if (Properties.Settings.Default.FasterNudgeValue <= 0)
                Properties.Settings.Default.FasterNudgeValue = 1;
            if (Properties.Settings.Default.FasterNudgeValue > 100)
                Properties.Settings.Default.FasterNudgeValue = 100;
            this.label21.Text = Properties.Settings.Default.FasterNudgeValue.ToString();
        }

        private void RPCCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.ShowDiscordRPC == false)
            {
                Properties.Settings.Default.ShowDiscordRPC = RPCCheckBox.Checked = true;
                Editor.Instance.UpdateDiscord(Editor.Instance.ScenePath);
            }
            else
            {
                Properties.Settings.Default.ShowDiscordRPC = RPCCheckBox.Checked = false;
                Editor.Instance.UpdateDiscord();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SettingsReader.exportSettings();
        }

        private void importOptionsButton_Click(object sender, EventArgs e)
        {
            SettingsReader.importSettings();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            String title = "Save Settings";
            String details = "Are you sure you want to save your settings, if the editor breaks because of one of these settings, you will have to redownload or manually reset you editor's config file! It's best you use the OK button to 'test' out the features before you save them.";
            if (MessageBox.Show(details, title, MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Properties.Settings.Default.Save();
            }
            else
            {
                return;
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Title = "Select SonicMania.exe";
            ofd.Filter = "Windows PE Executable|*.exe";
            if (ofd.ShowDialog() == DialogResult.OK)
                Properties.Settings.Default.RunGamePath = ofd.FileName;
        }

        private void button14_Click(object sender, EventArgs e)
        {
                var ofd = new OpenFileDialog();
                ofd.Title = "Select ManiaModManager.exe";
                ofd.Filter = "Windows PE Executable|*.exe";
                if (ofd.ShowDialog() == DialogResult.OK)
                    Properties.Settings.Default.RunModLoaderPath = ofd.FileName;
            
        }

        private void button15_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Title = "Select RSDK Animation Editor.exe";
            ofd.Filter = "Windows PE Executable|*.exe";
            if (ofd.ShowDialog() == DialogResult.OK)
                Properties.Settings.Default.RunAniEdPath = ofd.FileName;
        }

        private void button16_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Title = "Select TileManiac.exe";
            ofd.Filter = "Windows PE Executable|*.exe";
            if (ofd.ShowDialog() == DialogResult.OK)
                Properties.Settings.Default.RunTileManiacPath = ofd.FileName;
        }

        private void button17_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Title = "Select Color Palette Program (.exe)";
            ofd.Filter = "Windows PE Executable|*.exe";
            if (ofd.ShowDialog() == DialogResult.OK)
                Properties.Settings.Default.RunPalleteEditorPath = ofd.FileName;
        }

        private void OptionBox_Load(object sender, EventArgs e)
        {

        }

        private void gridSizeDownMore_Click(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.CustomGridSizeValue <= 500 && Properties.Settings.Default.CustomGridSizeValue >= 0)
                Properties.Settings.Default.CustomGridSizeValue -= 5;
            if (Properties.Settings.Default.CustomGridSizeValue <= 0)
                Properties.Settings.Default.CustomGridSizeValue = 1;
            if (Properties.Settings.Default.CustomGridSizeValue > 500)
                Properties.Settings.Default.CustomGridSizeValue = 500;
            this.gridSizeValueLabel.Text = Properties.Settings.Default.CustomGridSizeValue.ToString();
        }

        private void gridSizeDown_Click(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.CustomGridSizeValue <= 500 && Properties.Settings.Default.CustomGridSizeValue >= 0)
                Properties.Settings.Default.CustomGridSizeValue -= 1;
            if (Properties.Settings.Default.CustomGridSizeValue <= 0)
                Properties.Settings.Default.CustomGridSizeValue = 1;
            if (Properties.Settings.Default.CustomGridSizeValue > 500)
                Properties.Settings.Default.CustomGridSizeValue = 500;
            this.gridSizeValueLabel.Text = Properties.Settings.Default.CustomGridSizeValue.ToString();
        }

        private void gridSizeUp_Click(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.CustomGridSizeValue != 500 && Properties.Settings.Default.CustomGridSizeValue >= 0)
                Properties.Settings.Default.CustomGridSizeValue += 1;
            if (Properties.Settings.Default.CustomGridSizeValue <= 0)
                Properties.Settings.Default.CustomGridSizeValue = 1;
            if (Properties.Settings.Default.CustomGridSizeValue > 500)
                Properties.Settings.Default.CustomGridSizeValue = 500;
            this.gridSizeValueLabel.Text = Properties.Settings.Default.CustomGridSizeValue.ToString();
        }

        private void gridSizeUpMore_Click(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.CustomGridSizeValue != 500 && Properties.Settings.Default.CustomGridSizeValue >= 0)
                Properties.Settings.Default.CustomGridSizeValue += 5;
            if (Properties.Settings.Default.CustomGridSizeValue <= 0)
                Properties.Settings.Default.CustomGridSizeValue = 1;
            if (Properties.Settings.Default.CustomGridSizeValue > 500)
                Properties.Settings.Default.CustomGridSizeValue = 500;
            this.label21.Text = Properties.Settings.Default.CustomGridSizeValue.ToString();
        }

        private void checkBox12_CheckStateChanged(object sender, EventArgs e)
        {

        }

        private void checkBox13_CheckStateChanged(object sender, EventArgs e)
        {

        }

        private void checkBox10_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox11_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void uncheckOtherGridDefaults(int i)
        {
            switch (i)
            {
                case 1:
                    Properties.Settings.Default.x16Default = true;
                    Properties.Settings.Default.x128Default = false;
                    Properties.Settings.Default.x256Default = false;
                    Properties.Settings.Default.CustomGridDefault = false;
                    x16checkbox.Checked = true;
                    x128checkbox.Checked = false;
                    x256checkbox.Checked = false;
                    customGridCheckbox.Checked = false;
                    break;
                case 2:
                    Properties.Settings.Default.x16Default = false;
                    Properties.Settings.Default.x128Default = true;
                    Properties.Settings.Default.x256Default = false;
                    Properties.Settings.Default.CustomGridDefault = false;
                    x16checkbox.Checked = false;
                    x128checkbox.Checked = true;
                    x256checkbox.Checked = false;
                    customGridCheckbox.Checked = false;
                    break;
                case 3:
                    Properties.Settings.Default.x16Default = false;
                    Properties.Settings.Default.x128Default = false;
                    Properties.Settings.Default.x256Default = true;
                    Properties.Settings.Default.CustomGridDefault = false;
                    x16checkbox.Checked = false;
                    x128checkbox.Checked = false;
                    x256checkbox.Checked = true;
                    customGridCheckbox.Checked = false;
                    break;
                case 4:
                    Properties.Settings.Default.x16Default = false;
                    Properties.Settings.Default.x128Default = false;
                    Properties.Settings.Default.x256Default = false;
                    Properties.Settings.Default.CustomGridDefault = true;
                    x16checkbox.Checked = false;
                    x128checkbox.Checked = false;
                    x256checkbox.Checked = false;
                    customGridCheckbox.Checked = true;
                    break;
                /*default:
                    //x16checkbox.Checked = true; //Default
                    x128checkbox.Checked = false;
                    x256checkbox.Checked = false;
                    customGridCheckbox.Checked = false;
                    break;*/
            }


        }

        private void x16checkbox_CheckedChanged(object sender, EventArgs e)
        {
            if (x16checkbox.Checked == true)
            {
                uncheckOtherGridDefaults(1);
            }
        }

        private void X128checkbox_CheckedChanged(object sender, EventArgs e)
        {
            if (x128checkbox.Checked == true)
            {
                uncheckOtherGridDefaults(2);
            }
        }

        private void customGridCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (customGridCheckbox.Checked == true)
            {
                uncheckOtherGridDefaults(4);
            }
        }

        private void x256checkbox_CheckedChanged(object sender, EventArgs e)
        {
            if (x256checkbox.Checked == true)
            {
                uncheckOtherGridDefaults(3);
            }
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void initilizeKeyDefaults()
        {
            scrollLockAxisKeyBox.Text = Properties.KeyBinds.Default.ScrollLockTypeSwitch.ToString();
            nudgeFasterKeyBox.Text = Properties.KeyBinds.Default.NudgeFaster.ToString();
            scrollLockKeyBox.Text = Properties.KeyBinds.Default.ScrollLock.ToString();
        }

        private void nudgeFaster_KeyDown(object sender, KeyEventArgs e)
        {
            if (!(e.Alt || e.Control || e.Shift))
            {
                Keys keyData = e.KeyCode;
                Properties.KeyBinds.Default.NudgeFaster = keyData;
                nudgeFasterKeyBox.Text = "";
                nudgeFasterKeyBox.Text = Properties.KeyBinds.Default.NudgeFaster.ToString();
                Properties.KeyBinds.Default.Save();
            }

            
        }

        private void scrollLock_KeyDown(object sender, KeyEventArgs e)
        {
            if (!(e.Alt || e.Control || e.Shift))
            {
                Keys keyData = e.KeyCode;
                Properties.KeyBinds.Default.ScrollLock = keyData;
                scrollLockKeyBox.Text = "";
                scrollLockKeyBox.Text = Properties.KeyBinds.Default.ScrollLock.ToString();
                Properties.KeyBinds.Default.Save();
            }

        }

        private void scrollLockType_KeyDown(object sender, KeyEventArgs e)
        {
                if (!(e.Alt || e.Control || e.Shift))
                {
                    Keys keyData = e.KeyCode;
                    Properties.KeyBinds.Default.ScrollLockTypeSwitch = keyData;
                    scrollLockAxisKeyBox.Text = "";
                    scrollLockAxisKeyBox.Text = Properties.KeyBinds.Default.ScrollLockTypeSwitch.ToString();
                    Properties.KeyBinds.Default.Save();
            }

        }

        private void preRenderAlways_CheckedChanged(object sender, EventArgs e)
        {
            preRenderRadioGroupsUpdate(3);
            Properties.Settings.Default.preRenderSceneOption = 3;
            preRenderRadioGroupCheckChangeAllowed = true;
        }

        private void promptForPreRender_CheckedChanged(object sender, EventArgs e)
        {
            preRenderRadioGroupsUpdate(2);
            Properties.Settings.Default.preRenderSceneOption = 2;
            preRenderRadioGroupCheckChangeAllowed = true;
        }

        private void caseBasedPreRender_CheckedChanged(object sender, EventArgs e)
        {
            preRenderRadioGroupsUpdate(1);
            Properties.Settings.Default.preRenderSceneOption = 1;
            preRenderRadioGroupCheckChangeAllowed = true;
        }

        private void manualPreRender_CheckedChanged(object sender, EventArgs e)
        {
            preRenderRadioGroupsUpdate(0);
            Properties.Settings.Default.preRenderSceneOption = 0;
            preRenderRadioGroupCheckChangeAllowed = true;
        }

        private void preRenderRadioGroupsUpdate(int type)
        {
            bool[] groups = new [] {false,false,false,false};
            for (int i = 0; i < 4; i++) if (type == i) groups[i] = true;
            if (preRenderRadioGroupCheckChangeAllowed == true)
            {
                preRenderRadioGroupCheckChangeAllowed = false;
                manualPreRender.Checked = false || groups[0];
                caseBasedPreRender.Checked = false || groups[1];
                promptForPreRender.Checked = false || groups[2];
                preRenderAlways.Checked = false || groups[3];
            }

        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void CASANUD_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.CollisionSAColour = System.Drawing.Color.FromArgb((int)CASANUD.Value, Properties.Settings.Default.CollisionSAColour.R, Properties.Settings.Default.CollisionSAColour.G, Properties.Settings.Default.CollisionSAColour.B);
            Editor.Instance.RefreshCollisionColours(true);
        }

        private void CASRNUD_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.CollisionSAColour = System.Drawing.Color.FromArgb(Properties.Settings.Default.CollisionSAColour.A, (int)CASRNUD.Value, Properties.Settings.Default.CollisionSAColour.G, Properties.Settings.Default.CollisionSAColour.B);
            Editor.Instance.RefreshCollisionColours(true);
        }

        private void CASGNUD_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.CollisionSAColour = System.Drawing.Color.FromArgb(Properties.Settings.Default.CollisionSAColour.A, Properties.Settings.Default.CollisionSAColour.R, (int)CASGNUD.Value, Properties.Settings.Default.CollisionSAColour.B);
            Editor.Instance.RefreshCollisionColours(true);
        }

        private void CASBNUD_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.CollisionSAColour = System.Drawing.Color.FromArgb(Properties.Settings.Default.CollisionSAColour.A, Properties.Settings.Default.CollisionSAColour.R, Properties.Settings.Default.CollisionSAColour.G, (int)CASBNUD.Value);
            Editor.Instance.RefreshCollisionColours(true);
        }

        private void CTOSANUD_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.CollisionTOColour = System.Drawing.Color.FromArgb((int)CTOSANUD.Value, Properties.Settings.Default.CollisionTOColour.R, Properties.Settings.Default.CollisionTOColour.G, Properties.Settings.Default.CollisionTOColour.B);
            Editor.Instance.RefreshCollisionColours(true);
        }

        private void CTOSRNUD_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.CollisionTOColour = System.Drawing.Color.FromArgb(Properties.Settings.Default.CollisionTOColour.A, (int)CTOSRNUD.Value, Properties.Settings.Default.CollisionTOColour.G, Properties.Settings.Default.CollisionTOColour.B);
            Editor.Instance.RefreshCollisionColours(true);
        }

        private void CTOSGNUD_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.CollisionTOColour = System.Drawing.Color.FromArgb(Properties.Settings.Default.CollisionTOColour.A, Properties.Settings.Default.CollisionTOColour.R, (int)CTOSGNUD.Value, Properties.Settings.Default.CollisionTOColour.B);
            Editor.Instance.RefreshCollisionColours(true);
        }

        private void CTOSBNUD_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.CollisionTOColour = System.Drawing.Color.FromArgb(Properties.Settings.Default.CollisionTOColour.A, Properties.Settings.Default.CollisionTOColour.R, Properties.Settings.Default.CollisionTOColour.G, (int)CTOSBNUD.Value);
            Editor.Instance.RefreshCollisionColours(true);
        }

        private void CLRDSANUD_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.CollisionLRDColour = System.Drawing.Color.FromArgb((int)CLRDSANUD.Value, Properties.Settings.Default.CollisionLRDColour.R, Properties.Settings.Default.CollisionLRDColour.G, Properties.Settings.Default.CollisionLRDColour.B);
            Editor.Instance.RefreshCollisionColours(true);
        }

        private void CLRDSRNUD_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.CollisionLRDColour = System.Drawing.Color.FromArgb(Properties.Settings.Default.CollisionLRDColour.A, (int)CLRDSRNUD.Value, Properties.Settings.Default.CollisionLRDColour.G, Properties.Settings.Default.CollisionLRDColour.B);
            Editor.Instance.RefreshCollisionColours(true);
        }

        private void CLRDSGNUD_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.CollisionLRDColour = System.Drawing.Color.FromArgb(Properties.Settings.Default.CollisionLRDColour.A, Properties.Settings.Default.CollisionLRDColour.R, (int)CLRDSGNUD.Value, Properties.Settings.Default.CollisionLRDColour.B);
            Editor.Instance.RefreshCollisionColours(true);
        }

        private void CLRDSBNUD_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.CollisionLRDColour = System.Drawing.Color.FromArgb(Properties.Settings.Default.CollisionLRDColour.A, Properties.Settings.Default.CollisionLRDColour.R, Properties.Settings.Default.CollisionLRDColour.G, (int)CLRDSBNUD.Value);
            Editor.Instance.RefreshCollisionColours(true);
        }
    }
}
