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
using Microsoft.Scripting.Utils;
using RSDKv5;
using System.Collections.ObjectModel;
using ManiacEditor.Extensions;

namespace ManiacEditor.Controls.Utility.Object_Manager
{
    /// <summary>
    /// Interaction logic for ObjectManager.xaml
    /// </summary>
    public partial class ObjectManager : Window
	{
		private IList<SceneObject> _sourceSceneObjects;
		private IList<SceneObject> _targetSceneObjects;
		private IList<int> _sourceSceneObjectUID;
		private StageConfig _stageConfig;
		public List<String> objectCheckMemory = new List<string>();

		public ListViewItem SelectedAttributeItem;

		public Controls.Editor.MainEditor EditorInstance;

		public ObservableCollection<CheckBox> lvObjects;

		bool fullRefreshNeeded = false;

		public ObjectManager(IList<SceneObject> targetSceneObjects, StageConfig stageConfig, Controls.Editor.MainEditor instance)
		{
			EditorInstance = instance;
            InitializeComponent();

            if (Classes.Editor.SolutionState.RemoveStageConfigEntriesAllowed)
			{
				rmvStgCfgCheckbox.IsChecked = true;
			}


			_sourceSceneObjects = targetSceneObjects;
			_sourceSceneObjectUID = new List<int>();
			lvObjects = new ObservableCollection<CheckBox>();
			_targetSceneObjects = targetSceneObjects;
			_stageConfig = stageConfig;
			lvObjectsViewer.ItemsSource = lvObjects;

			var targetNames = _targetSceneObjects.Select(tso => tso.Name.ToString());
			var importableObjects = _targetSceneObjects.Where(sso => targetNames.Contains(sso.Name.ToString()))
														.OrderBy(sso => sso.Name.ToString());

			updateSelectedText();
			int PersonalID = 0;
			foreach (var io in importableObjects)
			{
				var lvc = new CheckBox()
				{
					Content = io.Name.ToString(),
					IsChecked = false,
					Tag = PersonalID.ToString()				
				};
                if (!_stageConfig.ObjectsNames.Contains(io.Name.ToString()))
                {
                    if (!Classes.Editor.Solution.GameConfig.ObjectsNames.Contains(io.Name.ToString())) {
                        lvc.Foreground = Brushes.Red;
                    }
                    else
                    {
                        lvc.Foreground = Brushes.GreenYellow;
                    }
                }
                lvc.Checked += lvObjects_ItemChecked;

				lvObjects.Add(lvc);
				PersonalID++;
			}

			updateSelectedText();
		}


		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void FilterText_TextChanged(object sender, TextChangedEventArgs e)
		{
			ReloadList();
			for (int n = lvObjects.Count - 1; n >= 0; --n)
			{
				string removelistitem = FilterText.Text;
				if (!lvObjects[n].Content.ToString().Contains(removelistitem))
				{
					lvObjects.RemoveAt(n);
				}
			}
			updateSelectedText();

		}

		public void addCheckedItems()
		{
			String lvc;
			CheckBox lvi;
			for (int i = 0; i < lvObjects.Count; i++)
			{
				lvi = lvObjects[i];
				lvc = lvi.Content.ToString(); //Get the current Object's Name
				if (objectCheckMemory.Contains(lvc) == false) //See if the memory does not have our current object
				{
					bool checkStatus = lvi.IsChecked.Value; //Grab the Value of the Checkbox for that Object
					if (checkStatus == true)
					{ //If it returns true, add it to memory
						objectCheckMemory.Add(lvc);
					}
				}

				else
				{


				}

			}

		}

		public void removeUncheckedItems()
		{
			String lvc;
			CheckBox lvi;
			for (int i = 0; i < lvObjects.Count; i++)
			{
				lvi = lvObjects[i] as CheckBox;
				lvc = lvi.Content.ToString(); //Get the current Object's Name
				if (objectCheckMemory.Contains(lvc) == false) //See if the memory does not have our current object
				{
					bool checkStatus = lvi.IsChecked.Value; //Grab the Value of the Checkbox for that Object
					if (checkStatus == false)
					{ //If it returns false, grab it's index and remove the range
						int index = objectCheckMemory.IndexOf(lvc);
						objectCheckMemory.RemoveRange(index, 1);
					}
				}
			}
		}

		private void ReloadList()
		{
			if (!fullRefreshNeeded)
			{

				addCheckedItems();
				removeUncheckedItems();
				fullRefreshNeeded = false;
			}
			lvObjects.Clear();
			var targetNames = _targetSceneObjects.Select(tso => tso.Name.ToString());
			var importableObjects = _targetSceneObjects.Where(sso => targetNames.Contains(sso.Name.ToString()))
														.OrderBy(sso => sso.Name.ToString());

			int InstanceID = 0;
			foreach (var io in importableObjects)
			{
				var lvc = new CheckBox()
				{
					Content = io.Name.ToString(),
					IsChecked = false,
					Tag = InstanceID.ToString()
				};
                if (!_stageConfig.ObjectsNames.Contains(io.Name.ToString()))
                {
                    if (!Classes.Editor.Solution.GameConfig.ObjectsNames.Contains(io.Name.ToString()))
                    {
                        lvc.Foreground = Brushes.Red;
                    }
                    else
                    {
                        lvc.Foreground = Brushes.GreenYellow;
                    }
                }
                InstanceID++;

				bool alreadyChecked = false;
				foreach (string str in objectCheckMemory)
				{
					if (objectCheckMemory.Contains(lvc.Content.ToString()) == true)
					{
						lvc.IsChecked = true;
						lvObjects.Add(lvc);

						alreadyChecked = true;
						break;
					}
				}
				if (alreadyChecked == false)
				{
					lvc.IsChecked = false;
					lvObjects.Add(lvc);

				}


			}
			lvObjectsViewer.Refresh();
		}

		public void RefreshList()
		{
			CommonReset();
			var targetNames = _targetSceneObjects.Select(tso => tso.Name.ToString());
			var importableObjects = _targetSceneObjects.Where(sso => targetNames.Contains(sso.Name.ToString()))
														.OrderBy(sso => sso.Name.ToString());

			updateSelectedText();
			foreach (var io in importableObjects)
			{
				var lvc = new CheckBox()
				{
					Content = io.Name.ToString(),
					IsChecked = false
				};

				var lvi = new ListViewItem()
				{
					Content = lvc
				};

			}
			updateSelectedText();
		}

		private void CommonReset()
		{
			FilterText.Text = "";
			objectCheckMemory.Clear();
			lvObjects.Clear();
		}

		private void lvObjects_ItemCheck(object sender, RoutedEventArgs e)
		{
			// this method is not being called for some reason
			// TODO: call this properly and update selected object count
			Console.WriteLine("TEST");
			updateSelectedText();
		}

		private void updateSelectedText()
		{
			//label1.Text = "Amount of Objects Selected (Memory): " + objectCheckMemory.Count + " (Current): " + lvObjects.CheckedItems.Count;
			label1.Content = "Amount of Objects Selected (Memory): " + objectCheckMemory.Count;
		}

		private void btnRemoveEntries_Click(object sender, RoutedEventArgs e)
		{
			var CheckedItems = lvObjects.Where(item => item.IsChecked.Value == true).ToList().Count;
			IList<CheckBox> lvObjects_CheckedItems = lvObjects.Where(item => item.IsChecked.Value == true).ToList();
			if (CheckedItems > 0)
			{
				const int MAX = 10;
				string itemNames = "";
				bool overMax = CheckedItems > MAX;
				int max = (overMax ? MAX - 1 : CheckedItems);
				for (int i = 0; i < max; i++)
					itemNames += "  -" + lvObjects_CheckedItems[i].Content + "(" + lvObjects_CheckedItems[i].Tag + ")" + "\n";
				if (overMax)
					itemNames += "(+" + (CheckedItems - (MAX - 1)) + " more)\n";

				if (MessageBox.Show("Are you sure you want to remove the following objects from this Scene?" + Environment.NewLine + itemNames + "This will also remove all entities of them!","Remove Objects and Entities?", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
				{
                    for (int i = 0; i < CheckedItems; i++)
                    {
                        var item = lvObjects_CheckedItems[i] as CheckBox;
                        int.TryParse(item.Tag.ToString(), out int ID);
                        List<SceneObject> AllInstancesOfThisObject = _targetSceneObjects.Where(x => x.Name.Name == item.Content.ToString()).ToList();
                        SceneObject objectToRemove;

                        if (AllInstancesOfThisObject.Count >= 1)
                        {
                            objectToRemove = AllInstancesOfThisObject.ElementAtOrDefault(ID);
                        }
                        else
                        {
                            objectToRemove = AllInstancesOfThisObject[0];
                        }

                        if (AllInstancesOfThisObject != null && objectToRemove != null)
                        {
                            objectToRemove.Entities.Clear(); // ditch instances of the object from the imported level
                            _targetSceneObjects.Remove(objectToRemove);
                        }
                        else
                        {
                            if (_targetSceneObjects.Where(x => x.Name.Name == item.Content.ToString()) != null)
                            {
                                foreach (var sceneObj in _targetSceneObjects.Where(x => x.Name.Name == item.Content.ToString()).ToList()) 
                                {
                                    sceneObj.Entities.Clear();
                                    _targetSceneObjects.Remove(sceneObj); // ditch instances of the object from the imported level
                                }
                            }
                        }

						if (Classes.Editor.SolutionState.RemoveStageConfigEntriesAllowed)
						{
							if (_stageConfig != null
								&& !_stageConfig.ObjectsNames.Contains(item.Content.ToString()))
							{
								_stageConfig.ObjectsNames.Remove(item.Content.ToString());
							}
						}
					}
                    fullRefreshNeeded = true;
                    ReloadList();
                }
			}
		}

		private void ObjectManager_FormClosed(object sender, CancelEventArgs e)
		{
			objectCheckMemory.Clear();
		}

		private void ImportObjectsUsingExistingEvent(object sender, RoutedEventArgs e)
		{
			Methods.ProgramLauncher.ImportObjectsToolStripMenuItem_Click(GetWindow(this));
			fullRefreshNeeded = true;
			ReloadList();
			// Blanks the list for some reason should consider fixing badly
		}


        private void ImportObjectsUsingMegalistEvent(object sender, RoutedEventArgs e)
        {
			Methods.ProgramLauncher.ImportObjectsWithMegaList(GetWindow(this));
			fullRefreshNeeded = true;
            ReloadList();
            // Blanks the list for some reason should consider fixing badly
        }

        private void LvObjectsViewer_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			updateSelectedText();
			if (lvObjectsViewer.SelectedItem != null)
			{
				var lvc = lvObjectsViewer.SelectedItem as CheckBox;
				if (lvc != null)
				{
					//Debug.Print(lvc.Content.ToString());
					int.TryParse(lvc.Tag.ToString(), out int ID);
					SceneObject obj = _targetSceneObjects[ID];

					attributesTable.Items.Clear();

					var gridView = new GridView();
					this.attributesTable.View = gridView;
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
						this.attributesTable.Items.Add(new AttributeItem { Type = att.Type.ToString(), Name = att.Name.ToString() });
					}
					attributesTable.Items.Refresh();
				}

			}

		}

		private void addAttributeBtn_Click(object sender, RoutedEventArgs e)
		{
			if (lvObjectsViewer.SelectedItems.Count > 0)
			{
				var target = lvObjectsViewer.SelectedItem as CheckBox;
				if (target == null) return;
 				string targetName = target.Content.ToString();
				SceneObject obj = _targetSceneObjects.First(sso => sso.Name.ToString().Equals(targetName));
				SceneObject[] objs = { obj };
				addAttributeToObjects(objs);
			}
		}

		private void addAttributeToAllObjectsToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			SceneObject[] objs = _targetSceneObjects.ToArray();
			addAttributeToObjects(objs);
		}

		private void addAttributeToObjects(SceneObject[] objs)
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

		private void removeAttributeBtn_Click(object sender, RoutedEventArgs e)
		{
			if (attributesTable.SelectedItem != null)
			{
				var att = attributesTable.SelectedItem as AttributeItem;
				var target = lvObjectsViewer.SelectedItem as CheckBox;

				if (att == null || target == null) return;

				string attName = att.Name.ToString();
				string targetName = target.Content.ToString();
				SceneObject obj = _targetSceneObjects.Single(sso => sso.Name.ToString().Equals(targetName));

				if (MessageBox.Show("Removing an attribute can cause serious problems and cannot be undone.\nI highly recommend making a backup first.\nAre you sure you want to remove the attribute \"" + attName + "\" from the object \"" + obj.Name.Name + "\" and all entities of it?", "Caution! This way lies madness!", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
				{
					obj.RemoveAttribute(attName);
					LvObjectsViewer_SelectionChanged(null, null);
				}
			}
		}

		private void backupStageConfigToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			ManiacEditor.Classes.Editor.SolutionLoader.StageConfigBackup(null, null);
		}

		private void lvObjects_ItemChecked(object sender, RoutedEventArgs e)
		{
			updateSelectedText();
		}

		private void Checkbox_CheckChanged(object sender, RoutedEventArgs e)
		{
			if (rmvStgCfgCheckbox.IsChecked.Value)
			{
				Classes.Editor.SolutionState.RemoveStageConfigEntriesAllowed = true;
			}
			else
			{
				Classes.Editor.SolutionState.RemoveStageConfigEntriesAllowed = false;
			}
		}

		private void attributesTable_KeyUp(object sender, KeyEventArgs e)
		{
			if (sender != attributesTable) return;

			if (e.Key == Key.LeftCtrl && e.Key == Key.C)
				CopySelectedValuesToClipboard();


		}
		private void CopySelectedValuesToClipboard()
		{
			var builder = new StringBuilder();
			foreach (var item in attributesTable.SelectedItems)
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

		private void mD5GeneratorToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			ManiacEditor.Controls.Utility.Editor.Dev.MD5HashGen hashmap = new ManiacEditor.Controls.Utility.Editor.Dev.MD5HashGen(EditorInstance);
			hashmap.Show();
		}

		public class AttributeItem
		{
			public string Name { get; set; }

			public string Type { get; set; }
		}

		private void importSoundsToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			Methods.ProgramLauncher.ImportSounds(GetWindow(this));
			ReloadList();
		}
	}
}
