using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace ManiacEditor
{
    partial class AboutBox : Form
    {
        public EditorUpdater Updater = new EditorUpdater();
        public AboutBox()
        {
            InitializeComponent();
            Text = String.Format("About {0}", AssemblyTitle);
            labelProductName.Text = AssemblyProduct;
            labelVersion.Text = String.Format("Version {0}", Updater.GetVersion());
            buildDateLabel.Text = String.Format("Build Date: {0}", GetBuildTime) + Environment.NewLine + String.Format("Architecture: {0}", GetProgramType);
            labelCopyright.Text = AssemblyCopyright;
            llAbout.Links.Clear();

            if (Settings.mySettings.NightMode)
            {
                linkLabel3.LinkColor = Editor.darkTheme4;
                llAbout.LinkColor = Editor.darkTheme4;
            }

            AddClickableLink("koolkdev", "https://github.com/koolkdev/ManiacEditor");
            AddClickableLink("Axanery", "https://www.youtube.com/channel/UCIsXoOHibP8wpjcha3bSbMQ");
            AddClickableLink("OtherworldBob", "https://github.com/OtherworldBob/ManiacEditor");
            AddClickableLink("SuperSonic16", "https://github.com/thesupersonic16");
            AddClickableLink("CarJem Generations", "https://github.com/CarJem");
            AddClickableLink("Campbellsonic", "https://github.com/campbellsonic/ManiacEditor");
            AddClickableLink("Rubberduckycooly", "https://github.com/Rubberduckycooly");
        }

        #region Assembly Attribute Accessors

        public string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public string GetProgramType
        {
            get
            {
                if (Environment.Is64BitProcess)
                {
                    return "x64";
                }
                else
                {
                    return "x86";
                }
            }
        }

        public string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        private string GetBuildTime
        {
            get
            {
                DateTime buildDate = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).LastWriteTime;
                String buildTimeString = buildDate.ToString();
                return buildTimeString;
            }

        }
        #endregion



        private void AddClickableLink(string sourceText, string linkTargetUrl)
        {
            var link = new LinkLabel.Link(llAbout.Text.IndexOf(sourceText),
                                          sourceText.Length,
                                          linkTargetUrl);
            if (Settings.mySettings.NightMode) llAbout.LinkColor = Editor.darkTheme4;
            llAbout.Links.Add(link);
        }

        private void llAbout_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start(e.Link.LinkData.ToString());
                e.Link.Visited = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to open the link. " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Updater.CheckforUpdates(true);
        }
    }
}
