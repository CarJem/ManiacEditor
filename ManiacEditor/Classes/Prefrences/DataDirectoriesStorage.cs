using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Specialized;

namespace ManiacEditor.Classes.Prefrences
{
    public static class DataDirectoriesStorage 
    {
        public static void AddSavedDataFolder(string dataDirectory)
        {
            try
            {
                if (ManiacEditor.Properties.Settings.MySettings.SavedDataDirectories == null) ManiacEditor.Properties.Settings.MySettings.SavedDataDirectories = new StringCollection();
                var mySettings = Properties.Settings.MySettings;
                var dataDirectories = ManiacEditor.Properties.Settings.MySettings.SavedDataDirectories;

                if (dataDirectories == null)
                {
                    dataDirectories = new StringCollection();
                    ManiacEditor.Properties.Settings.MySettings.SavedDataDirectories = dataDirectories;
                }

                if (dataDirectories.Contains(dataDirectory)) dataDirectories.Remove(dataDirectory);

                dataDirectories.Insert(0, dataDirectory);

                ManiacEditor.Classes.Options.GeneralSettings.Save();


            }
            catch (Exception ex)
            {
                Debug.Write("Failed to add data folder to saved list: " + ex);
            }
        }
        public static void AddRecentDataFolder(string dataDirectory)
        {
            if (dataDirectory == string.Empty) return;
            try
            {
                if (ManiacEditor.Properties.Settings.MySettings.RecentDataDirectories == null) ManiacEditor.Properties.Settings.MySettings.RecentDataDirectories = new StringCollection();
                var mySettings = Properties.Settings.MySettings;
                var dataDirectories = ManiacEditor.Properties.Settings.MySettings.RecentDataDirectories;

                if (dataDirectories == null)
                {
                    dataDirectories = new StringCollection();
                    ManiacEditor.Properties.Settings.MySettings.RecentDataDirectories = dataDirectories;
                }

                if (dataDirectories.Contains(dataDirectory)) dataDirectories.Remove(dataDirectory);

                if (dataDirectories.Count >= 10)
                {
                    for (int i = 9; i < dataDirectories.Count; i++) dataDirectories.RemoveAt(i);
                }

                dataDirectories.Insert(0, dataDirectory);

                ManiacEditor.Classes.Options.GeneralSettings.Save();


            }
            catch (Exception ex)
            {
                Debug.Write("Failed to add data folder to recent list: " + ex);
            }
        }
    }
}
