using ManiacEditor.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ManiacEditor
{
    public class EditorUpdater
    {
        string AppveyorVersion = "";
        bool badBuild = false;
        bool unkownError = false;
        public void CheckforUpdates(bool manuallyTriggered = false) {
            // Appveyor Update Check
            int buildNumber = -1;
            string versionNum = GetVersion();
            string versionNum2 = "0.0.0.0";
            //Debug.Print(versionNum);
            using (WebClient client = new WebClient())
            {
                try
                {
                    string appveyorDetails = client.DownloadString("https://ci.appveyor.com/api/projects/CarJem/maniaceditor-generationsedition");
                    if (appveyorDetails.Contains("buildNumber"))
                    {
                        string regex = "[0-9]*";
                        string stuff = Regex.Match(appveyorDetails, "\"buildNumber\":" + regex).ToString();
                        string buildNumberString = new String(stuff.Where(Char.IsDigit).ToArray());
                        buildNumber = Int32.Parse(buildNumberString);
                        //Debug.Print(buildNumber.ToString());


                        // Unable to retrive version number at the moment so disable this stuff
                        string regex2 = "\"version\":";
                        versionNum2 = Regex.Match(appveyorDetails, regex2 + "\"[^\"]*\"").Value.ToString();
                        versionNum2 = versionNum2.Replace(regex2, "");
                        versionNum2 = versionNum2.Replace("\"", "");
                        //Debug.Print(versionNum2);
                        AppveyorVersion = versionNum2;

                        if (appveyorDetails.Contains("\"status\":\"success\""))
                        {
                            badBuild = false;
                        }
                        else if (appveyorDetails.Contains("\"status\":\"failed\""))
                        {
                            badBuild = true;
                        }
                        else
                        {
                            unkownError = true;
                        }
                    }
                }
                catch
                {
                    //Debug.Print("Unable to get version from Appveyor, skiping update check.");
                }

            }
            if (buildNumber != -1 && !versionNum.Contains("DEV"))
            {
                string v1 = versionNum;
                string v2 = versionNum2;

                var version1 = new Version(v1);
                var version2 = new Version(v2);

                var result = version1.CompareTo(version2);
                if (result < 0)
                {
                        UpdateStatusBox box = new UpdateStatusBox(1, this);
                        box.ShowDialog();
                }
            }
            else
            {
                if (manuallyTriggered)
                {
                    if (versionNum.Contains("DEV"))
                    {
                        UpdateStatusBox box = new UpdateStatusBox(2, this);
                        box.ShowDialog();
                    }
                    else
                    {
                        UpdateStatusBox box = new UpdateStatusBox(0, this);
                        box.ShowDialog();
                    }

                }
                else if (versionNum.Contains("DEV"))
                {
                    //Debug.Print("DEV Build is being used! Don't get updates");
                }
            }
        }

        public string GetVersion()
        {
            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            //Adjust this after major and minor versions
            if (version == "1.0.0.0")
            {
                string devVersion = version.TrimEnd(version[version.Length - 1]) + "DEV";
                return devVersion;
            }
            return version;
        }

        public string GetCurrentVersion()
        {
            if (badBuild)
            {
                return AppveyorVersion + ("FAILED");
            }
            else if (unkownError)
            {
                return AppveyorVersion + ("UNKOWN");
            }
            else
            {
                return AppveyorVersion;
            }

        }

    }
}
