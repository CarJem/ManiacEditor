using RSDKv5;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ManiacEditor.Controls.Options;
using ManiacEditor.Methods.Entities;

namespace ManiacEditor.Controls.Object_Manager
{
    /// <summary>
    /// Interaction logic for ObjectImporter.xaml
    /// </summary>
    public partial class ObjectImporter : Window
    {
        #region Variables
        private IList<SceneObject> SourceObjects;
        private StageConfig TargetStageConfig;
        private IList<SceneObject> TargetObjects;

        private IList<CheckBox> ListEntries = new List<CheckBox>();
        private Controls.Editor.MainEditor Instance;
        #endregion

        #region Init
        public ObjectImporter(IList<SceneObject> _SourceObjects, IList<SceneObject> _TargetObjects, StageConfig _TargetStageConfig, Controls.Editor.MainEditor Instance)
        {
            InitializeComponent();
            SetupWindow(Instance);

            SourceObjects = _SourceObjects;
            TargetObjects = _TargetObjects;
            TargetStageConfig = _TargetStageConfig;

            GenerateNormalList();
        }
        public ObjectImporter(string DataFolder, GameConfig SourceConfig, IList<SceneObject> _TargetObjects, StageConfig _TargetStageConfig, Controls.Editor.MainEditor Instance)
        {
            InitializeComponent();
            SetupWindow(Instance);

            TargetObjects = _TargetObjects;
            TargetStageConfig = _TargetStageConfig;

            GenerateMegaList(DataFolder, SourceConfig);

        }
        public void SetupWindow(Controls.Editor.MainEditor instance)
        {
            Instance = instance;
            if (Methods.Solution.SolutionState.Main.AddStageConfigEntriesAllowed) AddToStageConfigCheckbox.IsChecked = true;
        }

        #endregion

        #region List Generation
        public void GenerateMegaList(string DataFolder, GameConfig SourceConfig)
        {
            MultiZoneObjectSearchPair MegaList = Methods.Entities.ObjectCollection.GetObjectsFromDataFolder(DataFolder, SourceConfig, TargetObjects);

            foreach (var Entry in MegaList.SourceObjects)
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

                    ListEntries.Add(zoneCheck);

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

                        if (MegaList.TargetNames.Contains(obj.Name.ToString())) objCheck.Foreground = new SolidColorBrush(System.Windows.Media.Colors.Red);

                        ListEntries.Add(objCheck);
                    }
                }
            }
            foreach (var item in ListEntries) ImportObjectList.Children.Add(item);

        }
        public void GenerateNormalList()
        {
            ObjectSearchPair ImportList = Methods.Entities.ObjectCollection.GetObjectsFromScene(SourceObjects, TargetObjects);

            foreach (var io in ImportList.SourceObjects)
            {
                var lvi = new CheckBox()
                {
                    IsChecked = false,
                    Content = io.Name.ToString(),
                    FontSize = 8.5
                };

                if (ImportList.TargetNames.Contains(io.Name.ToString())) lvi.Foreground = new SolidColorBrush(System.Windows.Media.Colors.Red);

                ListEntries.Add(lvi);
            }
            foreach (var item in ListEntries) ImportObjectList.Children.Add(item);
        }
        #endregion

        #region Methods

        private void ImportSelected()
        {
            try
            {
                var CheckedItems = ListEntries.Where(item => item.IsChecked.Value == true).ToList().Count;
                IList<CheckBox> lvObjects_CheckedItems = ListEntries.Where(item => item.IsChecked.Value == true).ToList();

                foreach (var lvci in lvObjects_CheckedItems)
                {
                    var item = lvci as CheckBox;
                    if (!(item.Content is SettingsHeader))
                    {
                        SceneObject objectToImport = SourceObjects.Where(sso => sso.Name.ToString().Equals(item.Content.ToString())).FirstOrDefault();
                        if (objectToImport != null)
                        {
                            objectToImport.Entities.Clear(); // ditch instances of the object from the imported level
                            TargetObjects.Add(objectToImport);

                            if (Methods.Solution.SolutionState.Main.AddStageConfigEntriesAllowed)
                            {
                                if (TargetStageConfig != null && !TargetStageConfig.ObjectsNames.Contains(item.Content.ToString()))
                                {
                                    TargetStageConfig.ObjectsNames.Add(item.Content.ToString());
                                }
                            }
                        }
                    }
                }


                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Unable to import Objects. " + ex.Message);
                DialogResult = false;
                Close();
            }
        }
        private void CancelImport()
        {
            DialogResult = false;
            Close();
        }

        #endregion

        #region Events
        private void ZoneCheck_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox item = sender as CheckBox;
            if (item != null)
            {
                bool Collapsed = (item.IsChecked.Value ? false : true);
                string Zone = item.Tag.ToString();
                foreach (var listItem in ListEntries)
                {
                    if (listItem != sender && listItem.Tag.ToString() == Zone)
                    {
                        listItem.Visibility = (Collapsed ? Visibility.Collapsed : Visibility.Visible);
                        listItem.Height = (Collapsed ? 0 : double.NaN);
                    }
                }
            }
        }
		private void CancelButton_Clicked(object sender, RoutedEventArgs e)
		{
            CancelImport();
        }
		private void ImportButton_Clicked(object sender, RoutedEventArgs e)
		{
            ImportSelected();
        }
		private void AddToStageConfigCheckbox_CheckChanged(object sender, RoutedEventArgs e)
		{
			if (AddToStageConfigCheckbox.IsChecked.Value) Methods.Solution.SolutionState.Main.AddStageConfigEntriesAllowed = true;
            else Methods.Solution.SolutionState.Main.AddStageConfigEntriesAllowed = false;
        }
        #endregion
    }

}
