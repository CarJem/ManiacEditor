using IronPython.Runtime.Operations;
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
        public string AppveyorVersion = "";
        public string AppveyorBuildMessage = "";
        public bool badBuild = false;
		public bool _404StyleError = false;
		public bool DeveloperError = false;
		public bool unkownError = false;
        public bool runningBuild = false;
        public int condition = 0;

		System.Windows.Window Window;

        public void CheckforUpdates(bool manuallyTriggered = false, bool dontShowUpdaterBox = false) {
            // Appveyor Update Check
            int buildNumber = -1;
            string versionNum = GetVersion();
            string versionNum2 = "0.0.0.0";
			//Debug.Print(versionNum);
			if (versionNum.Contains("DEV") && !manuallyTriggered)
			{
				DeveloperError = true;
				condition = 2;
				AppveyorBuildMessage = "Your using the source. No need to update!";
				return;
			}

			using (WebClient client = new WebClient())
            {
                try
                {
					string appveyorDetails = client.DownloadString("https://ci.appveyor.com/api/projects/CarJem/maniaceditor-generationsedition");
					//Debug.Print(appveyorDetails);

					if (appveyorDetails.Contains("buildNumber"))
					{
						string regex = "[0-9]*";
						string stuff = Regex.Match(appveyorDetails, "\"buildNumber\":" + regex).ToString();
						string buildNumberString = new String(stuff.Where(Char.IsDigit).ToArray());
						buildNumber = Int32.Parse(buildNumberString);
						//Debug.Print(buildNumber.ToString());

						string regex3 = "\"message\":.*,\"branch\"";
						string stuff3 = Regex.Match(appveyorDetails, regex3).ToString();
						stuff3 = stuff3.Replace("\"message\":", "");
						stuff3 = stuff3.Replace(",\"branch\"", "");
						AppveyorBuildMessage = stuff3;
						//Debug.Print(stuff3.ToString());

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
						else if (appveyorDetails.Contains("\"status\":\"running\""))
						{
							runningBuild = true;
						}
						else
						{
							unkownError = true;
						}
					}
					else
					{
						_404StyleError = true;
						AppveyorBuildMessage = "Make sure you are connected to the internet or click these links.";
					}
				}
                catch
                {
                    Debug.Print("Unable to get version from Appveyor, skiping update check.");
                }

            }
            if (buildNumber != -1 && !versionNum.Contains("DEV"))
            {
                string v1 = versionNum;
                string v2 = versionNum2;

                Debug.Print(v1);
                Debug.Print(v2);

                var version1 = new Version(v1);
                var version2 = new Version(v2);

                var result = version1.CompareTo(version2);
                string curVer = GetCurrentVersion();
                Debug.Print(result.ToString());
                if (result != 0)
                {
                    if (badBuild == false && !curVer.Contains("RUNNING"))
                    {
                        if (!dontShowUpdaterBox)
                        {
							Updater box = new Updater(1, this);
							box.Owner = System.Windows.Window.GetWindow(Window);
                            box.ShowDialog();
                        }
                        condition = 1;

                    }
                    else
                    {
                        if (!dontShowUpdaterBox)
                        {
							Updater box = new Updater(0, this);
							box.Owner = System.Windows.Window.GetWindow(Window);
							box.ShowDialog();
                        }
                        condition = 0;
                    }
                }

            }
            else
            {
                if (manuallyTriggered)
                {
                    if (versionNum.Contains("DEV"))
                    {
                        if (!dontShowUpdaterBox)
                        {
							Updater box = new Updater(2, this);
							box.Owner = System.Windows.Window.GetWindow(Window);
							box.ShowDialog();
                        }
                        condition = 2;
                    }
                    else
                    {
                        if (!dontShowUpdaterBox)
                        {
							Updater box = new Updater(0, this);
							box.Owner = System.Windows.Window.GetWindow(Window);
							box.ShowDialog();
                        }
                        condition = 0;
                    }

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

		public EditorUpdater(System.Windows.Window window = null)
		{
			Window = window;
		}

        public string GetCurrentVersion()
        {
            if (badBuild)
            {
                return AppveyorVersion + (" (FAILED)");
            }
            else if (runningBuild)
            {
                return AppveyorVersion + (" (BUILDING - May Fail)");
            }
            else if (unkownError)
            {
                return AppveyorVersion + (" (UNKOWN ERROR)");
            }
			else if (AppveyorVersion != "")
            {
                return AppveyorVersion;
            }
			else if (DeveloperError)
			{
				return "N/A";
			}
			else
			{
				return "Unable to Fetch/Prase Updates.";
			}

        }

        public string GetBuildMessage()
        {
            return AppveyorBuildMessage;
        }

    }
}
