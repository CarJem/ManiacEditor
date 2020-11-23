using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.ComponentModel;
using System.Data;
using RSDKv5;
using System.Collections.ObjectModel;
using ManiacEditor.Extensions;
using System.IO;
using System.Windows.Markup;
using System.Xml;

namespace ManiacEditor.Controls.Object_Manager
{
    /// <summary>
    /// Interaction logic for ObjectManager.xaml
    /// </summary>
    public partial class ObjectManager : Window
    {
        #region Object Collections
        private List<SceneObject> _SceneObjects
        {
            get
            {
                return Methods.Solution.CurrentSolution.Entities.SceneObjects;
            }
        }
        private List<string> _StageConfigObjects
        {
            get
            {
                return _SourceStageConfig.ObjectsNames;
            }
        }
        private List<string> _GameConfigObjects
        {
            get
            {
                return Methods.Solution.CurrentSolution.GameConfig.ObjectsNames;
            }
        }
        private StageConfig _SourceStageConfig
        {
            get
            {
                return Methods.Solution.CurrentSolution.StageConfig;
            }
        }

        #endregion

        #region List Collections

        private ObservableCollection<ObjectViewItem> ObjectsList { get; set; } = new ObservableCollection<ObjectViewItem>();
        private ObservableCollection<ObjectViewItem> VisibleObjectsList
        {
            get
            {
                string Filter = FilterText.Text;
                if (Filter != string.Empty)
                {
                    return new ObservableCollection<ObjectViewItem>(ObjectsList.Where(x => x.ObjectName.Contains(Filter)).OrderBy(sso => sso.ToString()));
                }
                else return ObjectsList;
            }
        }
        private List<ObjectViewItem> GetGlobalSelectedObjects()
        {
            return ObjectsList.Where(item => item.IsChecked.Value == true).ToList();
        }
        private List<ObjectViewItem> GetSceneSelectedObjects()
        {
            return ObjectsList.Where(item => item.IsChecked.Value == true && item.Obj_IsScene).ToList();
        }
        private List<ObjectViewItem> GetStageConfigSelectedObjects()
        {
            return ObjectsList.Where(item => item.IsChecked.Value == true && item.Obj_IsStageConfig).ToList();
        }

        #endregion

        #region Variables

        private Controls.Editor.MainEditor Instance;
        private bool ClearUndoHistory = false;

        #endregion

        #region Init
        public ObjectManager(Controls.Editor.MainEditor Instance)
        {
            InitializeComponent();
            this.Instance = Instance;
            ReloadList();
        }

        #endregion

        #region Classes

        public class AttributeItem
        {
            public string Name { get; set; }
            public string Type { get; set; }
        }
        public class ObjectViewItem : System.Windows.Controls.CheckBox
        {
            public string ObjectName { get; set; } = "NULL";
            public bool Obj_IsScene { get; set; } = false;
            public bool Obj_IsStageConfig { get; set; } = false;
            public bool Obj_IsGameConfig { get; set; } = false;
            public bool? Entry_IsChecked
            {
                get
                {
                    return IsChecked;
                }
                set
                {
                    IsChecked = value;

                }
            }
        }

        #endregion

        #region Closing Events/MEthods

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void ObjectManager_FormClosed(object sender, CancelEventArgs e)
        {
            if (ClearUndoHistory == true)
            {
                Actions.UndoRedoModel.ClearStacks();
            }
        }

        #endregion

        #region Object Viewer Methods/Events
        private void LvObjectsViewer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSelectionControls();
            ReloadAttributeTable();
        }
        private void ObjectsList_ItemChecked(object sender, RoutedEventArgs e)
        {
            UpdateSelectionControls();
        }
        private ObjectViewItem GenerateListEntry(string name, int index, IOrderedEnumerable<string> sceneObjects, IOrderedEnumerable<string> gameConfigObjects, IOrderedEnumerable<string> stageConfigObjects)
        {
            bool _isScene = sceneObjects.Contains(name);
            bool _isGameConfig = gameConfigObjects.Contains(name);
            bool _isStageConfig = stageConfigObjects.Contains(name);
            var lvc = new ObjectViewItem()
            {
                Content = name,
                ObjectName = name,
                IsChecked = false,
                Tag = index.ToString(),
                Obj_IsScene = _isScene,
                Obj_IsGameConfig = _isGameConfig,
                Obj_IsStageConfig = _isStageConfig
            };
            lvc.Checked += ObjectsList_ItemChecked;

            //if (!lvc.Obj_IsStageConfig) lvc.Foreground = Methods.Internal.Theming.GetSCBResource("Maniac_ObjectManager_RedTextColor");
            //else if (lvc.Obj_IsScene) lvc.Foreground = Methods.Internal.Theming.GetSCBResource("Maniac_ObjectManager_GreenTextColor");
            //else lvc.Foreground = Methods.Internal.Theming.GetSCBResource("NormalText");

            return lvc;
        }


        #endregion

        #region Refresh Methods/Events
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            UpdateSelectionControls();
        }
        private void UpdateSelectionControls()
        {
            SelectedObjectCountLabel.Content = "Amount of Objects Selected: " + GetGlobalSelectedObjects().Count;
            bool isObjectSelected = ObjectListBox.SelectedItem != null;
            bool isObjectSelectedInScene = (isObjectSelected ? (ObjectListBox.SelectedItem as ObjectViewItem).Obj_IsScene : false);
            bool isObjectChecked = GetGlobalSelectedObjects().Any();
            bool isAttributeSelected = AttributesTable.SelectedItem != null;

            GlobalRemoveButton.IsEnabled = isObjectChecked;
            StageConfigRemoveButton.IsEnabled = isObjectChecked;
            SceneRemoveButton.IsEnabled = isObjectChecked;

            AddAttributeButton.IsEnabled = isObjectSelectedInScene;
            RemoveAttributeButton.IsEnabled = isAttributeSelected;

        }
        private void FilterText_TextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshList();
        }
        private void ReloadAttributeTable()
        {
            if (ObjectListBox.SelectedItem != null)
            {
                var lvc = ObjectListBox.SelectedItem as CheckBox;
                if (lvc != null)
                {
                    //Debug.Print(lvc.Content.ToString());
                    int.TryParse(lvc.Tag.ToString(), out int ID);
                    AttributesTable.Items.Clear();
                    if (ID != -1)
                    {
                        SceneObject obj = _SceneObjects[ID];
                        var gridView = new GridView();
                        this.AttributesTable.View = gridView;
                        gridView.Columns.Add(new GridViewColumn
                        {
                            Header = "Name",
                            DisplayMemberBinding = new Binding("Name")
                        });
                        gridView.Columns.Add(new GridViewColumn
                        {
                            Header = "Type",
                            DisplayMemberBinding = new Binding("Type")
                        });

                        foreach (AttributeInfo att in obj.Attributes)
                        {
                            this.AttributesTable.Items.Add(new AttributeItem { Type = att.Type.ToString(), Name = att.Name.ToString() });
                        }
                    }
                    AttributesTable.Items.Refresh();
                }

            }
        }
        private void RefreshObjectColumns(ListView listView)
        {
            int padding = 0;
            int autoFillColumnIndex = (listView.View as GridView).Columns.Count - 1;
            if (listView.ActualWidth == Double.NaN)
                listView.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            double remainingSpace = listView.ActualWidth - SystemParameters.VerticalScrollBarWidth;
            for (int i = 0; i < (listView.View as GridView).Columns.Count; i++)
                if (i != autoFillColumnIndex)
                    remainingSpace -= (listView.View as GridView).Columns[i].ActualWidth;
            (listView.View as GridView).Columns[autoFillColumnIndex].Width = (remainingSpace >= 0 ? remainingSpace : 0) + padding;
        }
        private void ReloadList()
        {
            ObjectsList.Clear();
            ObjectListBox.ItemsSource = null;
            RefreshObjectColumns(ObjectListBox);

            var SceneObjects = _SceneObjects.Select(y => y.Name.ToString()).OrderBy(x => x.ToString());
            var StageConfigObjects = _StageConfigObjects.OrderBy(x => x.ToString());
            var GameConfigObjects = _GameConfigObjects.OrderBy(sso => sso.ToString());

            var AllObjects = SceneObjects.Union(StageConfigObjects).OrderBy(sso => sso.ToString());

            int index = 0;
            foreach (var name in AllObjects)
            {

                if (SceneObjects.Contains(name))
                {
                    var lvc = GenerateListEntry(name, index, SceneObjects, GameConfigObjects, StageConfigObjects);
                    ObjectsList.Add(lvc);
                    index++;
                }
                else
                {
                    var lvc = GenerateListEntry(name, -1, SceneObjects, GameConfigObjects, StageConfigObjects);
                    ObjectsList.Add(lvc);
                }
            }

            ObjectListBox.ItemsSource = VisibleObjectsList;
            UpdateSelectionControls();
        }
        private void RefreshList()
        {
            ObjectListBox.ItemsSource = null;
            RefreshObjectColumns(ObjectListBox);
            ObjectListBox.ItemsSource = VisibleObjectsList;
            UpdateSelectionControls();
        }

        #endregion

        #region Object Management Methods/Events
        private void AddNewObjectEvent(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Adding objects is still experimental and could be dangerous.\nI highly recommend making a backup first.",
                "Danger! Experimental territory!",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            string title = "Add Object";
            string message = "Enter your new object's name:";
            string newObjectName = GenerationsLib.WPF.TextPrompt2.ShowDialog(title, message);

            if (!_SceneObjects.Exists(x => x.Name.Name == newObjectName)) Methods.Solution.CurrentSolution.Entities.SceneObjects.Add(new SceneObject(new NameIdentifier(newObjectName), new List<AttributeInfo>()));
            if (!_StageConfigObjects.Contains(newObjectName)) _StageConfigObjects.Add(newObjectName);

            Classes.Prefrences.SceneCurrentSettings.AddCustomObjectHashNames(newObjectName);

            Methods.Internal.UserInterface.UpdateControls();
            Instance.EntitiesToolbar?.RefreshSpawningObjects(Methods.Solution.CurrentSolution.CurrentScene.Entities.SceneObjects);
            Methods.Internal.UserInterface.SplineControls.UpdateSplineSpawnObjectsList(Methods.Solution.CurrentSolution.CurrentScene.Entities.SceneObjects);

            ReloadList();
        }

        private void GlobalRemoveButton_Click(object sender, RoutedEventArgs e)
        {
            var SelectedObjects = GetGlobalSelectedObjects();

            if (RemovePrompt(GeneratePromptList(SelectedObjects), "This will also remove all Entities of them within this scene!"))
            {
                var ObjectsToRemove = SelectedObjects.ConvertAll(x => x.ObjectName);
                RemoveStageConfigObjects(ObjectsToRemove);
                RemoveSceneObjects(ObjectsToRemove);
                ReloadList();
            }
        }
        private void StageConfigRemoveButton_Click(object sender, RoutedEventArgs e)
        {
            var SelectedObjects = GetStageConfigSelectedObjects();
            if (RemovePrompt(GeneratePromptList(SelectedObjects), "This will prevent any entities with these names from loading in this scene!"))
            {
                var ObjectsToRemove = SelectedObjects.ConvertAll(x => x.ObjectName);
                RemoveStageConfigObjects(ObjectsToRemove);
                ReloadList();
            }
        }
        private void SceneRemoveButton_Click(object sender, RoutedEventArgs e)
        {
            var SelectedObjects = GetSceneSelectedObjects();
            if (RemovePrompt(GeneratePromptList(SelectedObjects), "This will also remove all Entities of them within this scene!"))
            {
                var ObjectsToRemove = SelectedObjects.ConvertAll(x => x.ObjectName);
                RemoveSceneObjects(ObjectsToRemove);
                ReloadList();
            }
        }
        private void RemoveSceneObjects(List<string> Entries)
        {
            Methods.Solution.CurrentSolution.Entities.DeleteEntities(Methods.Solution.CurrentSolution.Entities.Entities.Where(x => Entries.Contains(x.Entity.Object.Name.ToString()) == true).ToList());
            _SceneObjects.RemoveAll(x => Entries.Contains(x.Name.ToString()) == true);
            ClearUndoHistory = true;
        }
        private void RemoveStageConfigObjects(List<string> Entries)
        {
            _StageConfigObjects.RemoveAll(x => Entries.Contains(x.ToString()) == true);
        }
        private List<string> GeneratePromptList(List<ObjectViewItem> Items)
        {
            return Items.ConvertAll<string>(x => string.Format("{0} - ({1})", x.ObjectName, x.Tag));
        }
        private bool RemovePrompt(List<string> ItemNames, string ExtraWarning = "")
        {
            var NL = Environment.NewLine;
            if (MessageBox.Show("Remove the Following Objects? (This cannot be undone)" + NL + String.Join(NL, ItemNames.ToArray()) + NL + ExtraWarning, "Remove Objects?", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                return true;
            }
            else return false;
        }

        #endregion

        #region Attribute Table Methods/Events
        private void AttributesTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSelectionControls();
        }
        private void AttributesTable_KeyUp(object sender, KeyEventArgs e)
        {
            if (sender != AttributesTable) return;
            if (e.Key == Key.LeftCtrl && e.Key == Key.C) CopySelectedAttributeValuesToClipboard();
        }
        private void AddAttributeButton_Click(object sender, RoutedEventArgs e)
        {
            if (ObjectListBox.SelectedItems.Count > 0)
            {
                var target = ObjectListBox.SelectedItem as CheckBox;
                if (target == null) return;
                string targetName = target.Content.ToString();
                SceneObject obj = _SceneObjects.First(sso => sso.Name.ToString().Equals(targetName));
                SceneObject[] objs = { obj };
                AddAttributeToObjects(objs);
            }
        }
        private void RemoveAttributeButton_Click(object sender, RoutedEventArgs e)
        {
            if (AttributesTable.SelectedItem != null)
            {
                var att = AttributesTable.SelectedItem as AttributeItem;
                var target = ObjectListBox.SelectedItem as CheckBox;

                if (att == null || target == null) return;

                string attName = att.Name.ToString();
                string targetName = target.Content.ToString();
                SceneObject obj = _SceneObjects.Single(sso => sso.Name.ToString().Equals(targetName));

                if (MessageBox.Show("Removing an attribute can cause serious problems and cannot be undone.\nI highly recommend making a backup first.\nAre you sure you want to remove the attribute \"" + attName + "\" from the object \"" + obj.Name.Name + "\" and all entities of it?", "Caution! This way lies madness!", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    obj.RemoveAttribute(attName);
                    LvObjectsViewer_SelectionChanged(null, null);
                }
            }
        }
        private void AddAttributeToAllObjectsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SceneObject[] objs = _SceneObjects.ToArray();
            AddAttributeToObjects(objs);
        }
        private void AddAttributeToObjects(SceneObject[] objs)
        {
            MessageBox.Show("Adding attributes is still experimental and could be dangerous.\nI highly recommend making a backup first.",
                "Danger! Experimental territory!",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            var dialog = new AddAttributeWindow(objs);
            dialog.Owner = Window.GetWindow(this);

            dialog.ShowDialog();
            if (dialog.DialogResult != true)
                return; // nothing to do

            // added, now refresh
            LvObjectsViewer_SelectionChanged(null, null);
        }
        private void CopySelectedAttributeValuesToClipboard()
        {
            var builder = new StringBuilder();
            foreach (var item in AttributesTable.SelectedItems)
            {
                var itemBX = item as CheckBox;
                if (itemBX != null)
                {
                    string valueString = itemBX.Content.ToString();
                    builder.AppendLine(valueString);
                }

            }


            Clipboard.SetText(builder.ToString());
        }

        #endregion

        #region Import/Export Methods/Events

        private void ImportObjectsUsingExistingEvent(object sender, RoutedEventArgs e)
        {
            Methods.ProgramLauncher.ImportObjectsFromScene(GetWindow(this));
            ReloadList();
        }
        private void ImportObjectsUsingMegalistEvent(object sender, RoutedEventArgs e)
        {
            Methods.ProgramLauncher.ImportObjectsWithMegaList(GetWindow(this));
            ReloadList();
        }
        private void ExportObjectsUsingExistingEvent(object sender, RoutedEventArgs e)
        {
            Methods.ProgramLauncher.ExportObjectsFromScene(GetWindow(this));
        }
        private void ExportObjectsUsingMegalistEvent(object sender, RoutedEventArgs e)
        {
            Methods.ProgramLauncher.ExportObjectsWithMegaList();
        }
        private void ImportSoundsToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Methods.ProgramLauncher.ImportSounds(null);
            ReloadList();
        }

        #endregion

        #region Misc Shortcut Methods/Events 

        private void ContextMenuButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = (sender as Button);
            if (!btn.ContextMenu.IsOpen)
            {
                //Open the Context menu when Button is clicked
                btn.ContextMenu.IsEnabled = true;
                btn.ContextMenu.PlacementTarget = (sender as Button);
                btn.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                btn.ContextMenu.IsOpen = true;

            }
        }
        private void BackupStageConfig_ToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ManiacEditor.Methods.Solution.SolutionLoader.BackupStageConfig();
        }
        private void MD5_Generator_ToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ManiacEditor.Controls.Misc.Dev.MD5HashGen hashmap = new ManiacEditor.Controls.Misc.Dev.MD5HashGen(Instance);
            hashmap.Show();
        }

        #endregion
    }

    public class AutoSizedGridView : GridView
	{
		protected override void PrepareItem(ListViewItem item)
		{
			foreach (GridViewColumn column in Columns)
			{
				// Setting NaN for the column width automatically determines the required
				// width enough to hold the content completely.

				// If the width is NaN, first set it to ActualWidth temporarily.
				if (double.IsNaN(column.Width))
					column.Width = column.ActualWidth;

				// Finally, set the column with to NaN. This raises the property change
				// event and re computes the width.
				column.Width = double.NaN;
			}
			base.PrepareItem(item);
		}
	}
}
