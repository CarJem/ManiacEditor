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
            SetupWindow();

            GenerateNormalList(sourceSceneObjects, targetSceneObjects, stageConfig);
        }

        public void SetupWindow()
        {
            if (EditorInstance.UIModes.AddStageConfigEntriesAllowed) checkBox1.IsChecked = true;
            if (Settings.MySettings.NightMode) SetRTFText(ManiacEditor.Properties.Resources.ObjectWarningDarkTheme);
            else SetRTFText(ManiacEditor.Properties.Resources.ObjectWarning);
        }

        public ObjectImporter(string dataFolderBase, GameConfig SourceConfig, IList<SceneObject> targetSceneObjects, StageConfig stageConfig, Editor instance)
        {
            InitializeComponent();
            EditorInstance = instance;
            SetupWindow();

            _targetSceneObjects = targetSceneObjects;
            _stageConfig = stageConfig;

            GenerateMegaList(dataFolderBase, SourceConfig, targetSceneObjects);

        }

        public class ImportableZoneObject
        {

            public List<SceneObject> Objects = new List<SceneObject>();
            public string Zone = "";

            public ImportableZoneObject(string zone)
            {
                Zone = zone;
            }

            public ImportableZoneObject(string zone, List<SceneObject> objects)
            {
                Zone = zone;
                Objects = objects;
            }
        }

        public void GenerateMegaList(string dataFolderBase, GameConfig SourceConfig, IList<SceneObject> targetSceneObjects)
        {
            var targetNames = targetSceneObjects.Select(tso => tso.Name.ToString());
            _sourceSceneObjects = new List<SceneObject>();

            List<ImportableZoneObject> MegaList = new List<ImportableZoneObject>();
            List<SceneObject> GlobalObjects = new List<SceneObject>();

            foreach (var category in SourceConfig.Categories)
            {
                foreach (var scene in category.Scenes)
                {
                    string sceneLocation = scene.GetFilePath(dataFolderBase);
                    if (File.Exists(sceneLocation))
                    {
                        Scene sourceScene = new Scene(sceneLocation);
                        foreach (var sceneObj in sourceScene.Objects.Distinct())
                        {
                            if (!MegaList.Exists(x => x.Zone == scene.Zone))
                            {
                                ImportableZoneObject newZone = new ImportableZoneObject(scene.Zone);
                                MegaList.Add(newZone);
                            }
                            var zoneEntry = MegaList.Where(x => x.Zone == scene.Zone).First();
                            if (!zoneEntry.Objects.Exists(x => x.Name.Name == sceneObj.Name.Name))
                            {
                                if (!SourceConfig.ObjectsNames.Contains(sceneObj.Name.Name))
                                {
                                    zoneEntry.Objects.Add(sceneObj);
                                    if (!_sourceSceneObjects.Contains(sceneObj)) _sourceSceneObjects.Add(sceneObj);
                                }
                                else
                                {
                                    if (!GlobalObjects.Exists(x => x.Name.Name == sceneObj.Name.Name))
                                    {
                                        GlobalObjects.Add(sceneObj);
                                    }
                                }

                            }

                        }
                    }
                }
            }

            MegaList.ForEach(x => x.Objects = x.Objects.Where(sso => !GlobalObjects.Contains(sso)).ToList());
            MegaList.Insert(0, new ImportableZoneObject("Global", GlobalObjects));

            if (Properties.Settings.Default.RemoveObjectImportLock == false)
            {
                MegaList.ForEach(x => x.Objects = x.Objects.Where(sso => !targetNames.Contains(sso.Name.Name)).ToList());
            }

            foreach (var Entry in MegaList)
            {
                if (Entry.Objects.Count != 0)
                {
                    var header = new SettingsHeader()
                    {
                        HeaderName = Entry.Zone
                    };
                    var zoneCheck = new CheckBox()
                    {
                        IsChecked = false,
                        Content = header,
                        Tag = Entry.Zone
                    };

                    zoneCheck.Checked += ZoneCheck_Checked;
                    zoneCheck.Unchecked += ZoneCheck_Checked;

                    lvObjects.Add(zoneCheck);

                    foreach (var obj in Entry.Objects)
                    {
                        var objCheck = new CheckBox()
                        {
                            IsChecked = false,
                            Content = obj.Name.ToString(),
                            FontSize = 8.5,
                            Tag = Entry.Zone,
                            Visibility = Visibility.Collapsed,
                            Height = 0,
                            Margin = new Thickness(20, 0, 0, 0)
                        };

                        if (targetNames.Contains(obj.Name.ToString())) objCheck.Foreground = new SolidColorBrush(System.Windows.Media.Colors.Red);

                        lvObjects.Add(objCheck);
                    }
                }
            }
            foreach (var item in lvObjects)
            {
                lvObjectsView.Children.Add(item);
            }

        }

        public void GenerateNormalList(IList<SceneObject> sourceSceneObjects, IList<SceneObject> targetSceneObjects, StageConfig stageConfig)
        {
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

                if (targetNames.Contains(io.Name.ToString())) lvi.Foreground = new SolidColorBrush(System.Windows.Media.Colors.Red);

                lvObjects.Add(lvi);
            }
            foreach (var item in lvObjects)
            {
                lvObjectsView.Children.Add(item);
            }
        }

        private void ZoneCheck_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox item = sender as CheckBox;
            if (item != null)
            {
                bool Collapsed = (item.IsChecked.Value ? false : true);
                string Zone = item.Tag.ToString();
                foreach (var listItem in lvObjects)
                {
                    if (listItem != sender && listItem.Tag.ToString() == Zone)
                    {
                        listItem.Visibility = (Collapsed ? Visibility.Collapsed : Visibility.Visible);
                        listItem.Height = (Collapsed ? 0 : double.NaN);
                    }
                }
            }
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
                    if (!(item.Content is SettingsHeader))
                    {
                        SceneObject objectToImport = _sourceSceneObjects.Where(sso => sso.Name.ToString().Equals(item.Content.ToString())).FirstOrDefault();
                        objectToImport.Entities.Clear(); // ditch instances of the object from the imported level
                        _targetSceneObjects.Add(objectToImport);

                        if (EditorInstance.UIModes.AddStageConfigEntriesAllowed)
                        {
                            if (_stageConfig != null && !_stageConfig.ObjectsNames.Contains(item.Content.ToString()))
                            {
                                _stageConfig.ObjectsNames.Add(item.Content.ToString());
                            }
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
				EditorInstance.UIModes.AddStageConfigEntriesAllowed = true;
			}
			else
			{
				EditorInstance.UIModes.AddStageConfigEntriesAllowed = false;
			}
		}
	}

}
