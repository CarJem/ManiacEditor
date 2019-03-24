using ManiacEditor.Properties;
using RSDKv5;
using System;
using System.IO;
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
using System.Windows.Shapes;
using MessageBox = RSDKrU.MessageBox;

namespace ManiacEditor.Interfaces
{
	/// <summary>
	/// Interaction logic for ObjectImporter.xaml
	/// </summary>
	public partial class SoundImporter : Window
	{
		private IList<WAVConfiguration> _sourceSceneSounds;
		private IList<WAVConfiguration> _targetSceneSounds;
		private StageConfig _stageConfig;
		public IList<CheckBox> lvObjects = new List<CheckBox>();

		public SoundImporter(StageConfig sourceSceneSounds, StageConfig stageConfig)
		{
			InitializeComponent();
			if (Settings.MySettings.NightMode)
				SetRTFText(ManiacEditor.Properties.Resources.SoundWarningDarkTheme);
			else
				SetRTFText(ManiacEditor.Properties.Resources.SoundWarning);

			_sourceSceneSounds = sourceSceneSounds.WAVs;
			_targetSceneSounds = stageConfig.WAVs;
			_stageConfig = stageConfig;

			var targetSounds = _targetSceneSounds.Select(tso => tso.Name);
			var importableSounds = _sourceSceneSounds.Where(sso => !targetSounds.Contains(sso.Name))
													 .OrderBy(sso => sso.Name);

			foreach (var io in importableSounds)
			{
				var lvi = new CheckBox()
				{
					IsChecked = false,
					Content = io.Name.ToString(),
					FontSize = 8.5
				};

				lvObjects.Add(lvi);
			}
			lvObjectsView.ItemsSource = lvObjects;
		}

		public void SetRTFText(string text)
		{
			MemoryStream stream = new MemoryStream(ASCIIEncoding.Default.GetBytes(text));
			this.rtbWarning.Selection.Load(stream, DataFormats.Rtf);
		}

		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}

		private void btnImport_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;

			var CheckedItems = lvObjects.Where(item => item.IsChecked.Value == true).ToList().Count;
			IList<CheckBox> lvObjects_CheckedItems = lvObjects.Where(item => item.IsChecked.Value == true).ToList();

			foreach (var lvci in lvObjects_CheckedItems)
			{
				var item = lvci as CheckBox;
				WAVConfiguration soundToImport = _sourceSceneSounds.Single(sso => sso.Name.Equals(item.Content));
				_targetSceneSounds.Add(_sourceSceneSounds.Single(sso => sso.Name.Equals(item.Content)));

				if (_stageConfig != null
					&& !_stageConfig.WAVs.Select(w => w.Name).Contains(soundToImport.Name))
				{
					_stageConfig.WAVs.Add(soundToImport);
				}
			}

			Close();
		}
	}

}
