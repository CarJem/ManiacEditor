using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ManiacEditor
{
    public partial class StatusBox : Form
    {
        System.Windows.Forms.Timer t;
        public StatusBox()
        {
            InitializeComponent();
            t = new System.Windows.Forms.Timer();
            t.Interval = 10;
            t.Tick += new EventHandler(setText);
            t.Start();
        }

        private void setText(object sender, EventArgs e)
        {
            string newLine = Environment.NewLine;
            informationLabel.Text = "Data Directory: " + Editor.DataDirectory;
            informationLabel.Text = informationLabel.Text + newLine + "Mod Data Directory: " + Editor.ModDataDirectory;
            informationLabel.Text = informationLabel.Text + newLine + "Master Data Directory: " + Editor.MasterDataDirectory;
            informationLabel.Text = informationLabel.Text + newLine + "Zoom Level: " + Editor.Instance.GetZoom();
            informationLabel.Text = informationLabel.Text + newLine + "Scene Filepath: " + Editor.Instance.SceneFilepath;
            informationLabel.Text = informationLabel.Text + newLine + "Scene Path: " + Editor.Instance.ScenePath;
            informationLabel.Text = informationLabel.Text + newLine + "Selected Zone: " + Editor.Instance.SelectedZone;
            informationLabel.Text = informationLabel.Text + newLine + "Scene TileConfig Path: " + Path.Combine(Editor.Instance.SceneFilepath, "TileConfig.bin").ToString(); 
        }


        private void controlBox_Quit(object sender, EventArgs e)
        {
            Editor.Instance.controlWindowOpen = false;
        }
    }
}
