using MonoGame.Extended.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace ManiacEditor
{
    public partial class ModConfigCreator : Form
    {
        bool isEditing = false;
        string originalName = "";
        public ModConfigCreator(bool edit = false, int editIndex = 0)
        {
            InitializeComponent();
            if (edit == true)
            {
                richTextBox1.Text = Properties.Settings.Default.modConfigs[editIndex];
                textBox1.Text = Properties.Settings.Default.modConfigsNames[editIndex];
                textBox1.Text = textBox1.Text.Remove(textBox1.Text.Length - 4);
                originalName = textBox1.Text;
                isEditing = true;
            }
        }

        private void ModConfigCreator_Load(object sender, EventArgs e)
        {

        }

        private void create_Click(object sender, EventArgs e)
        {
            bool cancel = false;
            if (textBox1.Text == "" || textBox1.Text.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                MessageBox.Show("You must type a valid file name!");
                cancel = true;
            }
            if (richTextBox1.Text == "")
            {
                MessageBox.Show("You must have some content in your config!");
                cancel = true;
            }
            if (cancel == false)
            {
                if(isEditing == false)
                {
                    Properties.EditorState.Default.addConfigContents = this.richTextBox1.Text;
                    Properties.EditorState.Default.addConfigName = this.textBox1.Text;
                    Properties.EditorState.Default.addConfig = true;
                    string file = Environment.CurrentDirectory + "\\Config\\" + Properties.EditorState.Default.addConfigName + ".ini";
                    string directory = Environment.CurrentDirectory + "\\Config";
                    DirectoryInfo di = Directory.CreateDirectory(directory);
                    File.Create(file).Close();
                    File.WriteAllText(file, Properties.EditorState.Default.addConfigContents);
                    this.DialogResult = DialogResult.OK;
                }
                else
                {
                    Properties.EditorState.Default.addConfigContents = this.richTextBox1.Text;
                    Properties.EditorState.Default.addConfigName = this.textBox1.Text;
                    Properties.EditorState.Default.addConfigName = this.textBox1.Text;
                    Properties.EditorState.Default.addConfig = true;
                    String fileNew = Environment.CurrentDirectory + "\\Config\\" + Properties.EditorState.Default.addConfigName + ".ini";
                    if (Properties.EditorState.Default.addConfigName != originalName)
                    {
                        String fileOld = Environment.CurrentDirectory + "\\Config\\" + originalName + ".ini";
                        File.Move(fileOld, fileNew);
                    }
                    File.WriteAllText(fileNew, Properties.EditorState.Default.addConfigContents);
                    this.DialogResult = DialogResult.OK;
                }

            }
        }

        private void cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ModConfigCreator_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void ModConfigCreator_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            MessageBox.Show("This config file will replace ManiaModLoader.ini when it is used, so look to that if you want to know what to input!");
        }

        private void import_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Title = "Select ManiaModLoader.ini";
            ofd.Filter = "Windows PE Executable|*.ini";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string fileName = ofd.FileName;
                string fileContents = File.ReadAllText(fileName);
                richTextBox1.Clear();
                richTextBox1.Text = fileContents;
            }
        }
    }
}
