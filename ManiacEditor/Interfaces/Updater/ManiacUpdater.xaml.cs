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

namespace ManiacEditor.Interfaces.Updater
{
    /// <summary>
    /// Interaction logic for ManiacUpdater.xaml
    /// </summary>
    public partial class ManiacUpdater : UserControl
    {
        public ManiacUpdater()
        {
            InitializeComponent();
            CheckForUpdates();
        }


        private bool allowUsage { get; set; } = false;
        public bool isOnline { get; private set; }
        public string VersionCheckFileName { get; private set; }

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
            if (!allowUsage) return;
            try
            {
                isOnline = IsNetworkAvailable();

                if (isOnline)
                {
                    string download_path = "";
                    string url = @"http://sonic3air.org/sonic3air_updateinfo.json";
                    VersionCheckFileName = DownloadFromURL(url, download_path, DownloadCheckComplete);
                }
                else
                {
                    UpdateResults = UpdateResult.NoNetwork;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DownloadCheckComplete()
        {
            try
            {

            }
            catch (Exception ex)
            {
                throw ex;
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
        private void linkLabel1_LinkClicked(object sender, RoutedEventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo("https://github.com/CarJem/GenerationsLib.Updates/raw/master/UpdateFiles/Maniac_Editor/Setup.exe");
            Process.Start(sInfo);
        }

        private void linkLabel2_LinkClicked(object sender, RoutedEventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo("https://github.com/CarJem/ManiacEditor-GenerationsEdition/releases");
            Process.Start(sInfo);
        }
        #endregion
    }
}
