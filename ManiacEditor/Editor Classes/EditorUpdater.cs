using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Net.NetworkInformation;

namespace ManiacEditor
{
    public class EditorUpdater
    {
		System.Windows.Window Window;

        public void CheckforUpdates(bool manuallyTriggered = false, bool dontShowUpdaterBox = false)
        {

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

    }
}
