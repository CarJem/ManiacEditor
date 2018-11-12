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
    public partial class UpdateStatusBox : Form
    {
        bool updateAvaliable = false;
        bool sourceBuild = false;
        public UpdateStatusBox(int condition, EditorUpdater updater)
        {
            InitializeComponent();
            switch (condition)
            {
                case 0:
                    updateAvaliable = false;
                    break;
                case 1:
                    updateAvaliable = true;
                    break;
                case 2:
                    updateAvaliable = false;
                    sourceBuild = true;
                    break;
                default:
                    updateAvaliable = false;
                    break;
            }
            if (updateAvaliable)
            {
                this.updateInfoLabel.Text = "An Update is Avaliable!" + Environment.NewLine + Environment.NewLine + string.Format("Local Version: {0}", updater.GetVersion()) + Environment.NewLine + string.Format("Current Version: {0}", updater.GetCurrentVersion());
            }
            else
            {
                if (sourceBuild)
                {
                    this.updateInfoLabel.Text = "Your using the source. No need to update!" + Environment.NewLine + Environment.NewLine + string.Format("Local Version: {0}", updater.GetVersion()) + Environment.NewLine + string.Format("Current Version: {0}", updater.GetCurrentVersion());
                }
                else
                {
                    this.updateInfoLabel.Text = "No Updates Found!" + Environment.NewLine + Environment.NewLine + string.Format("Local Version: {0}", updater.GetVersion()) + Environment.NewLine + string.Format("Current Version: {0}", updater.GetCurrentVersion());
                }
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo("https://ci.appveyor.com/project/CarJem/maniaceditor-generationsedition");
            Process.Start(sInfo);
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo("https://github.com/CarJem/ManiacEditor-GenerationsEdition/releases");
            Process.Start(sInfo);
        }
    }
}
