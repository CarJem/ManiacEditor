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
        public AboutBox()
        {
            InitializeComponent();
            Text = String.Format("About {0}", AssemblyTitle);
            labelProductName.Text = AssemblyProduct;
            labelVersion.Text = String.Format("Version {0}", AssemblyVersion);
            buildDateLabel.Text = String.Format("Build Date: {0}", GetBuildTime) + Environment.NewLine + String.Format("Architecture: {0}", GetProgramType);
            labelCopyright.Text = AssemblyCopyright;
            llAbout.Links.Clear();

            AddClickableLink("koolkdev", "https://github.com/koolkdev/ManiacEditor");
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

        public string AssemblyVersion
        {
            get
            {
                string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                if (Regex.IsMatch(version, "[0 - 9]*.[0 - 9]*.[0 - 9]*.0"))
                {
                    string devVersion = version.TrimEnd(version[version.Length - 1]) + "DEV";
                    return devVersion;
                }
                return version;
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

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo("https://ci.appveyor.com/project/CarJem/maniaceditor-generationsedition");
            Process.Start(sInfo);
        }

        private void linkLabel1_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo("https://github.com/CarJem/ManiacEditor-GenerationsEdition/releases");
            Process.Start(sInfo);
        }
    }
}
