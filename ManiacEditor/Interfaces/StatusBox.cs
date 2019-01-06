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
        public Editor EditorInstance;
        public StatusBox(Editor instance)
        {
            EditorInstance = instance;
            InitializeComponent();
            t = new System.Windows.Forms.Timer();
            t.Interval = 10;
            t.Tick += new EventHandler(setText);
            t.Start();
        }

        private void setText(object sender, EventArgs e)
        {
            string newLine = Environment.NewLine;
            informationLabel.Text = "Data Directory: " + EditorInstance.DataDirectory;
            informationLabel.Text = informationLabel.Text + newLine + "Mod Data Directory: " + EditorInstance.ModDataDirectory;
            informationLabel.Text = informationLabel.Text + newLine + "Master Data Directory: " + EditorInstance.MasterDataDirectory;
            informationLabel.Text = informationLabel.Text + newLine + "Zoom Level: " + EditorInstance.GetZoom();
            informationLabel.Text = informationLabel.Text + newLine + "Scene Filepath: " + EditorInstance.SceneFilepath;
            informationLabel.Text = informationLabel.Text + newLine + "Scene Path: " + EditorInstance.ScenePath;
            informationLabel.Text = informationLabel.Text + newLine + "Selected Zone: " + EditorInstance.SelectedZone;
            informationLabel.Text = informationLabel.Text + newLine + "Scene TileConfig Path: " + Path.Combine(EditorInstance.SceneFilepath, "TileConfig.bin").ToString(); 
        }


        private void controlBox_Quit(object sender, EventArgs e)
        {
            EditorInstance.controlWindowOpen = false;
        }
    }
}
