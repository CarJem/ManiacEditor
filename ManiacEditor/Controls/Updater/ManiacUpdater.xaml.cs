using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Net;
using System.IO;
using ManiacEditor.Classes;
using System.Reflection;

namespace ManiacEditor.Controls.Updater
{
    /// <summary>
    /// Interaction logic for ManiacUpdater.xaml
    /// </summary>
    public partial class ManiacUpdater : UserControl
    {
        public ManiacUpdater()
        {
            InitializeComponent();
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                CheckForUpdates();
            }
        }


        private bool AwaitingAsync { get; set; } = false;
        public bool isOnline { get; private set; }
        public string VersionCheckFileName { get; private set; }
        public string UpdateFileName { get; private set; }
        public string InstallerDownloadURL { get; private set; }



        public UpdateResult UpdateResults { get; private set; }
        public enum UpdateResult : int
        {
            NoNetwork = 0,
            Unknown = 1,
            Outdated = 2,
            UpToDate = 3
        }

        #region Network Checking

        /// <summary>
        /// Indicates whether any network connection is available
        /// Filter connections below a specified speed, as well as virtual network cards.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if a network connection is available; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNetworkAvailable()
        {
            return IsNetworkAvailable(100);
        }

        /// <summary>
        /// Indicates whether any network connection is available.
        /// Filter connections below a specified speed, as well as virtual network cards.
        /// </summary>
        /// <param name="minimumSpeed">The minimum speed required. Passing 0 will not filter connection using speed.</param>
        /// <returns>
        ///     <c>true</c> if a network connection is available; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNetworkAvailable(long minimumSpeed)
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
                return false;

            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                // discard because of standard reasons
                if ((ni.OperationalStatus != OperationalStatus.Up) ||
                    (ni.NetworkInterfaceType == NetworkInterfaceType.Loopback) ||
                    (ni.NetworkInterfaceType == NetworkInterfaceType.Tunnel))
                    continue;

                // this allow to filter modems, serial, etc.
                // I use 10000000 as a minimum speed for most cases
                if (ni.Speed < minimumSpeed)
                    continue;

                // discard virtual cards (virtual box, virtual pc, etc.)
                if ((ni.Description.IndexOf("virtual", StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (ni.Name.IndexOf("virtual", StringComparison.OrdinalIgnoreCase) >= 0))
                    continue;

                // discard "Microsoft Loopback Adapter", it will not show as NetworkInterfaceType.Loopback but as Ethernet Card.
                if (ni.Description.Equals("Microsoft Loopback Adapter", StringComparison.OrdinalIgnoreCase))
                    continue;

                return true;
            }
            return false;
        }

        #endregion

        #region Update Checking
        private void CheckForUpdates()
        {
            if (AwaitingAsync) return;


            AwaitingAsync = true;
            CleanCache();            
            try
            {
                UpdateUpdaterText(3);
                isOnline = IsNetworkAvailable();

                if (isOnline)
                {
                    string download_path = Methods.ProgramPaths.DownloadRequestsFolder;
                    string url = @"https://raw.githubusercontent.com/CarJem/GenerationsLib.Updates/master/UpdateMetadata/ManiacEditor_Updates.json";
                    VersionCheckFileName = DownloadFromURL(url, download_path, DownloadVersionCheckFileComplete);
                }
                else
                {
                    UpdateResults = UpdateResult.NoNetwork;
                    UpdateUpdaterText(2);
                }
            }
            catch (Exception ex)
            {
                UpdateUpdaterText(2);
            }
            AwaitingAsync = false;
        }

        private void CleanCache()
        {
            foreach (var file in Directory.EnumerateFiles(Methods.ProgramPaths.DownloadRequestsFolder, "*.*"))
            {
                File.Delete(file);
            }
        }

        private void UpdateUpdaterText(int state, Classes.General.VersionCheck versionCheck = null)
        {
            if (state == 0)
            {
                InstallerDownloadURL = "";
                UpdateDetails.Text = "";
                UpdateStatus.Text = "Unable to Prase Update Data";
                UpdateDetails.Visibility = Visibility.Collapsed;
                UpdateHyperlink.Visibility = Visibility.Collapsed;
                UpdateSeperator.Visibility = Visibility.Collapsed;
            }
            else if (state == 1)
            {
                InstallerDownloadURL = versionCheck.DownloadURL;
                UpdateDetails.Text = versionCheck.Details;
                UpdateStatus.Text = "Update Avaliable!";
                UpdateDetails.Visibility = Visibility.Visible;
                UpdateHyperlink.Visibility = Visibility.Visible;
                UpdateSeperator.Visibility = Visibility.Visible;
            }
            else if (state == 2)
            {
                InstallerDownloadURL = "";
                UpdateDetails.Text = "";
                UpdateStatus.Text = "Unable to Check for Updates";
                UpdateDetails.Visibility = Visibility.Collapsed;
                UpdateHyperlink.Visibility = Visibility.Collapsed;
                UpdateSeperator.Visibility = Visibility.Collapsed;
            }
            else if (state == 3)
            {
                InstallerDownloadURL = "";
                UpdateDetails.Text = "";
                UpdateStatus.Text = "Checking for Updates...";
                UpdateDetails.Visibility = Visibility.Collapsed;
                UpdateHyperlink.Visibility = Visibility.Collapsed;
                UpdateSeperator.Visibility = Visibility.Collapsed;
            }
            else if (state == 4)
            {
                InstallerDownloadURL = versionCheck.DownloadURL;
                UpdateDetails.Text = versionCheck.Details;
                UpdateStatus.Text = "Up to Date! Click \"Update\" to Redownload!";
                UpdateDetails.Visibility = Visibility.Visible;
                UpdateHyperlink.Visibility = Visibility.Visible;
                UpdateSeperator.Visibility = Visibility.Visible;
            }
        }

        private void DownloadVersionCheckFileComplete()
        {
            try
            {
                Classes.General.VersionCheck versionCheck = new Classes.General.VersionCheck(new FileInfo(System.IO.Path.Combine(Methods.ProgramPaths.DownloadRequestsFolder, VersionCheckFileName)));

                var current = Methods.ProgramBase.GetVersion();
                var remote = versionCheck.Version;

                int offset = current.CompareTo(remote);
                
                if (offset == 0)
                {
                    UpdateUpdaterText(4, versionCheck);
                }
                else if (offset >= 1)
                {
                    UpdateUpdaterText(4, versionCheck);
                }
                else if (offset <= -1)
                {
                    UpdateUpdaterText(1, versionCheck);
                }

            }
            catch (Exception ex)
            {
                UpdateUpdaterText(0);
            }
        }

        #endregion

        #region Update Downloading

        private string GetRemoteFileName(string baseURL)
        {
            Uri uri = new Uri(baseURL);
            return System.IO.Path.GetFileName(uri.LocalPath);
        }

        private string DownloadFromURL(string url, string destination, Action finishAction, bool backgroundDownload = true)
        {
            if (!Directory.Exists(destination)) Directory.CreateDirectory(destination);
            string baseURL = GetBaseURL(url);
            if (baseURL != "") url = baseURL;

            string remote_filename = "";
            if (url != "") remote_filename = GetRemoteFileName(url);
            string filename = "temp.zip";
            if (remote_filename != "") filename = remote_filename;

            DownloadWindow downloadWindow = new DownloadWindow($"Downloading \"{filename}\"", url, $"{destination}\\{filename}");
            downloadWindow.DownloadCompleted = finishAction;
            if (backgroundDownload) downloadWindow.StartBackground();
            else downloadWindow.Start();
            return filename;

        }

        private string GetBaseURL(string url)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.AllowAutoRedirect = false;  // IMPORTANT

            webRequest.Timeout = 10000;           // timeout 10s
            webRequest.Method = "HEAD";
            // Get the response ...
            HttpWebResponse webResponse;
            using (webResponse = (HttpWebResponse)webRequest.GetResponse())
            {
                // Now look to see if it's a redirect
                if ((int)webResponse.StatusCode >= 300 && (int)webResponse.StatusCode <= 399)
                {
                    string uriString = webResponse.Headers["Location"];
                    return uriString;
                }
            }
            return "";
        }

        #endregion

        #region Old Link Button

        private void linkLabel2_LinkClicked(object sender, RoutedEventArgs e)
        {
            CheckForUpdates();
        }
        #endregion

        #region Install Update Methods
        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                isOnline = IsNetworkAvailable();

                if (isOnline)
                {
                    string download_path = Methods.ProgramPaths.DownloadRequestsFolder;
                    string url = InstallerDownloadURL;
                    UpdateFileName = DownloadFromURL(url, download_path, DownloadUpdateFileComplete, false);
                }
                else
                {
                    MessageBox.Show("Unable to download update package! Your not online!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failure grabbing latest update package: " + ex.Message);
            }
        }

        private void DownloadUpdateFileComplete()
        {
            try
            {
                string updateLocation = System.IO.Path.Combine(Methods.ProgramPaths.DownloadRequestsFolder, UpdateFileName);
                Process.Start(updateLocation);
                Environment.Exit(0);
            }
            catch (Exception ex)
            {

            }
        }

        #endregion
    }
}
