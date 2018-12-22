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
using Cyotek.Windows.Forms;
using ManiacEditor.Interfaces;

namespace ManiacEditor
{
    
    public partial class OptionBox : Form
    {
        bool preRenderRadioGroupCheckChangeAllowed = true;
        bool collisionColorsRadioGroupCheckChangeAllowed = true;
        public OptionBox()
        {
            InitializeComponent();

            preRenderRadioGroupsUpdate(Properties.Settings.Default.preRenderSceneOption);
            collisionColorsRadioGroupUpdate(Properties.Settings.Default.CollisionColorsDefault);
            preRenderRadioGroupCheckChangeAllowed = true;
            collisionColorsRadioGroupCheckChangeAllowed = true;
            this.label21.Text = Properties.Settings.Default.FasterNudgeValue.ToString();
            this.gridSizeValueLabel.Text = Properties.Settings.Default.CustomGridSizeValue.ToString();
            if (Properties.Settings.Default.x16Default) uncheckOtherGridDefaults(1);
            if (Properties.Settings.Default.x128Default) uncheckOtherGridDefaults(2);
            if (Properties.Settings.Default.x256Default) uncheckOtherGridDefaults(3);
            if (Properties.Settings.Default.CustomGridDefault) uncheckOtherGridDefaults(4);
            initilizeKeyDefaults();

            if (Properties.Settings.Default.NightMode)
            {
                checkBox15.Checked = true;
                tileToolbarZoomDefault.BackColor = Editor.Instance.darkTheme1;
                tabControl1.BackColor = System.Drawing.Color.Bisque;
            }
            tabPage1.UseVisualStyleBackColor = false;
            tabPage2.UseVisualStyleBackColor = false;
            tabPage4.UseVisualStyleBackColor = false;
            tabPage5.UseVisualStyleBackColor = false;
            tabPage6.UseVisualStyleBackColor = false;

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


        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            preRenderRadioGroupsUpdate(1);
            Properties.Settings.Default.CollisionColorsDefault = 0;
            collisionColorsRadioGroupCheckChangeAllowed = true;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            preRenderRadioGroupsUpdate(2);
            Properties.Settings.Default.CollisionColorsDefault = 1;
            collisionColorsRadioGroupCheckChangeAllowed = true;
        }

        private void radioButton1_CheckedChanged_1(object sender, EventArgs e)
        {
            preRenderRadioGroupsUpdate(3);
            Properties.Settings.Default.CollisionColorsDefault = 2;
            collisionColorsRadioGroupCheckChangeAllowed = true;
        }

        private void collisionColorsRadioGroupUpdate(int type)
        {
            bool[] groups = new[] { false, false, false };
            for (int i = 0; i < 3; i++) if (type == i) groups[i] = true;
            if (collisionColorsRadioGroupCheckChangeAllowed == true)
            {
                collisionColorsRadioGroupCheckChangeAllowed = false;
                radioButton4.Checked = false || groups[0];
                radioButton3.Checked = false || groups[1];
                radioButton1.Checked = false || groups[2];
            }

        }

        private void comboBox8_DropDown(object sender, EventArgs e)
        {
            //Grid Default Color
            ColorPickerDialog colorSelect = new ColorPickerDialog();
            DialogResult result = colorSelect.ShowDialog();
            if (result == DialogResult.OK)
            {
                Properties.Settings.Default.GridColorDefault = colorSelect.Color;
            }
        }

        private void comboBox7_DropDown(object sender, EventArgs e)
        {
            //Water Color
            ColorPickerDialog colorSelect = new ColorPickerDialog();
            DialogResult result = colorSelect.ShowDialog();
            if (result == DialogResult.OK)
            {
                Properties.Settings.Default.WaterColorDefault = colorSelect.Color;
                Editor.Instance.waterColor = colorSelect.Color;
            }
        }

        private void comboBox6_DropDown(object sender, EventArgs e)
        {
            //Collision Solid(Top Only) Color
            ColorPickerDialog colorSelect = new ColorPickerDialog();
            DialogResult result = colorSelect.ShowDialog();
            if (result == DialogResult.OK)
            {
                Properties.Settings.Default.CollisionTOColour = colorSelect.Color;
            }
        }

        private void comboBox5_DropDown(object sender, EventArgs e)
        {
            //Collision Solid(LRD) Color
            ColorPickerDialog colorSelect = new ColorPickerDialog();
            DialogResult result = colorSelect.ShowDialog();
            if (result == DialogResult.OK)
            {
                Properties.Settings.Default.CollisionLRDColour = colorSelect.Color;
            }
        }

        private void comboBox4_DropDown(object sender, EventArgs e)
        {
            //Collision Solid(All) Color
            ColorPickerDialog colorSelect = new ColorPickerDialog();
            DialogResult result = colorSelect.ShowDialog();
            if (result == DialogResult.OK)
            {
                Properties.Settings.Default.CollisionSAColour = colorSelect.Color;
            }
        }

        private void OptionBox_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (checkBox15.Checked && !Properties.Settings.Default.NightMode)
            {
                DialogResult result = MessageBox.Show("To apply this setting correctly, you will have to restart the editor, would you like to that now?", "Restart to Apply", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                if (result == DialogResult.OK)
                {
                    Properties.Settings.Default.NightMode = true;
                    Application.Restart();
                    Environment.Exit(0);
                }
            }
            else if (!checkBox15.Checked && Properties.Settings.Default.NightMode)
            {
                DialogResult result = MessageBox.Show("To apply this setting correctly, you will have to restart the editor, would you like to that now?", "Restart to Apply", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                if (result == DialogResult.OK)
                {
                    Properties.Settings.Default.NightMode = false;
                    Application.Restart();
                    Environment.Exit(0);
                }
            }
        }

        private void textBox43_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            bool state = true;
            if (state)
            {
                KeybindTool keybinder = new KeybindTool();
                if (keybinder.ShowDialog() == DialogResult.OK)
                {
                    string name = sender.ToString();
                }
            }


        }

        private void checkBox24_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox25_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox11_CheckedChanged_1(object sender, EventArgs e)
        {

        }

        private void checkBox50_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox48_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox35_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void tabPage4_Click(object sender, EventArgs e)
        {

        }
    }
}
