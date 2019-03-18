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
	public partial class ObjectImporter : Window
	{
		private IList<SceneObject> _sourceSceneObjects;
		private IList<SceneObject> _targetSceneObjects;
		private StageConfig _stageConfig;
		public Editor EditorInstance;
		public IList<CheckBox> lvObjects = new List<CheckBox>();

		public ObjectImporter(IList<SceneObject> sourceSceneObjects, IList<SceneObject> targetSceneObjects, StageConfig stageConfig, Editor instance)
		{
			InitializeComponent();
			EditorInstance = instance;
			if (EditorInstance.AddStageConfigEntriesAllowed)
			{
				checkBox1.IsChecked = true;
			}
			if (Settings.mySettings.NightMode)
				SetRTFText(ManiacEditor.Properties.Resources.ObjectWarningDarkTheme);
			else
				SetRTFText(ManiacEditor.Properties.Resources.ObjectWarning);

			_sourceSceneObjects = sourceSceneObjects;
			_targetSceneObjects = targetSceneObjects;
			_stageConfig = stageConfig;

			var targetNames = targetSceneObjects.Select(tso => tso.Name.ToString());
			var importableObjects = sourceSceneObjects.Where(sso => !targetNames.Contains(sso.Name.ToString()))
														.OrderBy(sso => sso.Name.ToString());
			if (Properties.Settings.Default.RemoveObjectImportLock == true)
			{
				importableObjects = _sourceSceneObjects.OrderBy(sso => sso.Name.ToString());
			}
			else
			{
				importableObjects = sourceSceneObjects.Where(sso => !targetNames.Contains(sso.Name.ToString()))
										.OrderBy(sso => sso.Name.ToString());
			}

			foreach (var io in importableObjects)
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

			try
			{
				var CheckedItems = lvObjects.Where(item => item.IsChecked.Value == true).ToList().Count;
				IList<CheckBox> lvObjects_CheckedItems = lvObjects.Where(item => item.IsChecked.Value == true).ToList();

				foreach (var lvci in lvObjects_CheckedItems)
				{
					var item = lvci as CheckBox;
					SceneObject objectToImport = _sourceSceneObjects.Single(sso => sso.Name.ToString().Equals(item.Content.ToString()));
					objectToImport.Entities.Clear(); // ditch instances of the object from the imported level
					_targetSceneObjects.Add(objectToImport);

					if (EditorInstance.AddStageConfigEntriesAllowed)
					{
						if (_stageConfig != null && !_stageConfig.ObjectsNames.Contains(item.Content.ToString()))
						{
							_stageConfig.ObjectsNames.Add(item.Content.ToString());
						}
					}

				}

				
				DialogResult = true;
				Close();
			}
			catch (Exception ex)
			{
				RSDKrU.MessageBox.Show("Unable to import Objects. " + ex.Message);
				DialogResult = false;
				Close();
			}


		}

		private void checkBox1_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (checkBox1.IsChecked.Value)
			{
				EditorInstance.AddStageConfigEntriesAllowed = true;
			}
			else
			{
				EditorInstance.AddStageConfigEntriesAllowed = false;
			}
		}
	}

}
