using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Reflection;
using System.IO;



namespace ManiacED_ManiaPal
{
    public class Connector
    {
		string StageConfigFilePath;
		string GameConfigFilePath;
		bool GameConfigNull = false;
		bool StageConfigNull = false;
		static ManiaPal.MainWindow ManiaPalInstance;
		public Connector()
		{
			AppDomain.CurrentDomain.AssemblyResolve += (s, e) =>
			{
				return AppDomain.CurrentDomain.GetAssemblies()
				   .SingleOrDefault(asm => asm.FullName == e.Name);
			};
			/*
			string exeLocationUrl = System.Reflection.Assembly.GetEntryAssembly().GetName().CodeBase;
			string exeLocation = new Uri(exeLocationUrl).LocalPath;
			string directory = new System.IO.FileInfo(exeLocation).Directory.FullName;

			Assembly.LoadFile(System.IO.Path.Combine(directory, "ManiaPal", "Cyotek.Windows.Forms.ColorPicker.dll"));
			Assembly.LoadFile(System.IO.Path.Combine(directory, "ManiaPal", "RSDKv5.dll"));
			*/
		}

		void ResetInformation()
		{
			StageConfigFilePath = "";
			GameConfigFilePath = "";
			GameConfigNull = false;
			StageConfigNull = false;
		}

		public void SetLoadingInformation(string GC_Path, string SC_Path, bool SC_NULL, bool GC_NULL)
		{
			ResetInformation();
			StageConfigFilePath = SC_Path;
			GameConfigFilePath = GC_Path;
			GameConfigNull = GC_NULL;
			StageConfigNull = SC_NULL;
		}

		public void Kill()
		{
			if (ManiaPalInstance != null)
			{
				var MP = ManiaPal.MainWindow.Instance;
				MP.Close();
			}
		}

		public void LoadConfig(bool UseGameConfig)
		{
			if (UseGameConfig && !GameConfigNull)
			{
				if (GameConfigFilePath != null) ManiaPal.MainWindow.Instance.LoadFile(GameConfigFilePath);
			}
			else if (!StageConfigNull)
			{
				if (StageConfigFilePath != null) ManiaPal.MainWindow.Instance.LoadFile(StageConfigFilePath);
			}
		}

		public void RefreshConfig(bool UseGameConfig)
		{
			if (UseGameConfig && !GameConfigNull)
			{
				if (StageConfigFilePath != null) ManiaPal.MainWindow.Instance.LoadFile(StageConfigFilePath);
				ManiaPal.MainWindow.Instance.RefreshPalette(ManiaPal.MainWindow.Instance.CurrentPaletteSet);
			}
			if (!StageConfigNull)
			{
				if (GameConfigFilePath != null) ManiaPal.MainWindow.Instance.LoadFile(GameConfigFilePath);
				ManiaPal.MainWindow.Instance.RefreshPalette(ManiaPal.MainWindow.Instance.CurrentPaletteSet);
			}
		}

		public void Activate(bool UseGameConfig)
		{
			if (ManiaPalInstance == null)
			{
				ManiaPalInstance = new ManiaPal.MainWindow();
				ManiaPalInstance.Closing += ManiaPalInstance_Closing;
				ManiaPalInstance.Show();

				while (ManiaPal.MainWindow.Instance == null)
					Thread.Sleep(100);

				LoadConfig(UseGameConfig);

				ManiaPal.MainWindow.Instance.RefreshPalette(ManiaPal.MainWindow.Instance.CurrentPaletteSet);
				ManiaPal.MainWindow.Instance.Activate();
			}
			else
			{
				var MP = ManiaPal.MainWindow.Instance;
				MP.Activate();
				MP.Visibility = System.Windows.Visibility.Visible;

				RefreshConfig(UseGameConfig);
			}
		}

		private void ManiaPalInstance_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			ManiaPalInstance = null;
		}
	}
}
