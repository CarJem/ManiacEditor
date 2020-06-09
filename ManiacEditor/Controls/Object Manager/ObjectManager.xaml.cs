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

namespace ManiacEditor.Controls.Object_Manager
{
    /// <summary>
    /// Interaction logic for ObjectManager.xaml
    /// </summary>
    public partial class ObjectManager : Window
	{
        #region Variables
        private IList<SceneObject> _SourceSceneObjects;
		private IList<SceneObject> _TargetSceneObjects;
		private IList<int> _SourceSceneObjectUID;
		private StageConfig _SourceStageConfig;

		private Controls.Editor.MainEditor Instance;

		private List<String> ObjectCheckMemory = new List<string>();
		private ObservableCollection<CheckBox> ListEntries;
		private bool FullRefreshNeeded = false;
        #endregion

        #region Init
        public ObjectManager(IList<SceneObject> targetSceneObjects, StageConfig stageConfig, Controls.Editor.MainEditor instance)
		{
			Instance = instance;
            InitializeComponent();

            if (rmvStgCfgCheckbox.IsChecked.Value)
			{
				rmvStgCfgCheckbox.IsChecked = true;
			}


			_SourceSceneObjects = targetSceneObjects;
			_SourceSceneObjectUID = new List<int>();
			ListEntries = new ObservableCollection<CheckBox>();
			_TargetSceneObjects = targetSceneObjects;
			_SourceStageConfig = stageConfig;
			lvObjectsViewer.ItemsSource = ListEntries;

			var targetNames = _TargetSceneObjects.Select(tso => tso.Name.ToString());
			var importableObjects = _TargetSceneObjects.Where(sso => targetNames.Contains(sso.Name.ToString()))
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
                if (!_SourceStageConfig.ObjectsNames.Contains(io.Name.ToString()))
                {
					if (Methods.Solution.CurrentSolution.GameConfig != null)
					{
						if (!Methods.Solution.CurrentSolution.GameConfig.ObjectsNames.Contains(io.Name.ToString()))
						{
							lvc.Foreground = Methods.Internal.Theming.GetSCBResource("Maniac_ObjectManager_RedTextColor");
						}
						else
						{
							lvc.Foreground = Methods.Internal.Theming.GetSCBResource("Maniac_ObjectManager_GreenTextColor");
						}
					}
					else
					{
						lvc.Foreground = Methods.Internal.Theming.GetSCBResource("Maniac_ObjectManager_GreenTextColor");
					}

                }
                lvc.Checked += lvObjects_ItemChecked;

				ListEntries.Add(lvc);
				PersonalID++;
			}

			updateSelectedText();
		}
		#endregion

		#region Events

		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
		private void FilterText_TextChanged(object sender, TextChangedEventArgs e)
		{
			ReloadList();
			for (int n = ListEntries.Count - 1; n >= 0; --n)
			{
				string removelistitem = FilterText.Text;
				if (!ListEntries[n].Content.ToString().Contains(removelistitem))
				{
					ListEntries.RemoveAt(n);
				}
			}
			updateSelectedText();

		}
		private void lvObjects_ItemCheck(object sender, RoutedEventArgs e)
		{
			// this method is not being called for some reason
			// TODO: call this properly and update selected object count
			ManiacEditor.Extensions.ConsoleExtensions.Print("TEST");
			updateSelectedText();
		}
		private void btnRemoveEntries_Click(object sender, RoutedEventArgs e)
		{
			var CheckedItems = ListEntries.Where(item => item.IsChecked.Value == true).ToList().Count;
			IList<CheckBox> lvObjects_CheckedItems = ListEntries.Where(item => item.IsChecked.Value == true).ToList();
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

				if (MessageBox.Show("Are you sure you want to remove the following objects from this Scene?" + Environment.NewLine + itemNames + "This will also remove all entities of them!", "Remove Objects and Entities?", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
				{
					for (int i = 0; i < CheckedItems; i++)
					{
						var item = lvObjects_CheckedItems[i] as CheckBox;
						int.TryParse(item.Tag.ToString(), out int ID);
						List<SceneObject> AllInstancesOfThisObject = _TargetSceneObjects.Where(x => x.Name.Name == item.Content.ToString()).ToList();
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
							_TargetSceneObjects.Remove(objectToRemove);
						}
						else
						{
							if (_TargetSceneObjects.Where(x => x.Name.Name == item.Content.ToString()) != null)
							{
								foreach (var sceneObj in _TargetSceneObjects.Where(x => x.Name.Name == item.Content.ToString()).ToList())
								{
									sceneObj.Entities.Clear();
									_TargetSceneObjects.Remove(sceneObj); // ditch instances of the object from the imported level
								}
							}
						}

						if (rmvStgCfgCheckbox.IsChecked.Value)
						{
							if (_SourceStageConfig != null
								&& !_SourceStageConfig.ObjectsNames.Contains(item.Content.ToString()))
							{
								_SourceStageConfig.ObjectsNames.Remove(item.Content.ToString());
							}
						}
					}
					FullRefreshNeeded = true;
					ReloadList();
				}
			}
		}
		private void ObjectManager_FormClosed(object sender, CancelEventArgs e)
		{
			ObjectCheckMemory.Clear();
		}
		private void ImportObjectsUsingExistingEvent(object sender, RoutedEventArgs e)
		{
			Methods.ProgramLauncher.ImportObjectsFromScene(GetWindow(this));
			FullRefreshNeeded = true;
			ReloadList();
			// TODO: Blanks the list for some reason should consider fixing badly
		}
		private void ImportObjectsUsingMegalistEvent(object sender, RoutedEventArgs e)
		{
			Methods.ProgramLauncher.ImportObjectsWithMegaList(GetWindow(this));
			FullRefreshNeeded = true;
			ReloadList();
			// TODO: Blanks the list for some reason should consider fixing badly
		}
		private void ExportObjectsUsingExistingEvent(object sender, RoutedEventArgs e)
		{
			Methods.ProgramLauncher.ExportObjectsFromScene(GetWindow(this));
		}
		private void ExportObjectsUsingMegalistEvent(object sender, RoutedEventArgs e)
		{
			Methods.ProgramLauncher.ExportObjectsWithMegaList();
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
					SceneObject obj = _TargetSceneObjects[ID];

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
				SceneObject obj = _TargetSceneObjects.First(sso => sso.Name.ToString().Equals(targetName));
				SceneObject[] objs = { obj };
				addAttributeToObjects(objs);
			}
		}
		private void addAttributeToAllObjectsToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			SceneObject[] objs = _TargetSceneObjects.ToArray();
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
				SceneObject obj = _TargetSceneObjects.Single(sso => sso.Name.ToString().Equals(targetName));

				if (MessageBox.Show("Removing an attribute can cause serious problems and cannot be undone.\nI highly recommend making a backup first.\nAre you sure you want to remove the attribute \"" + attName + "\" from the object \"" + obj.Name.Name + "\" and all entities of it?", "Caution! This way lies madness!", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
				{
					obj.RemoveAttribute(attName);
					LvObjectsViewer_SelectionChanged(null, null);
				}
			}
		}
		private void backupStageConfigToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			ManiacEditor.Methods.Solution.SolutionLoader.BackupStageConfig();
		}
		private void lvObjects_ItemChecked(object sender, RoutedEventArgs e)
		{
			updateSelectedText();
		}
		private void Checkbox_CheckChanged(object sender, RoutedEventArgs e)
		{

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
			ManiacEditor.Controls.Misc.Dev.MD5HashGen hashmap = new ManiacEditor.Controls.Misc.Dev.MD5HashGen(Instance);
			hashmap.Show();
		}
		private void importSoundsToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
			Methods.ProgramLauncher.ImportSounds(null);
			ReloadList();
		}

		#endregion

		#region Methods
		public void addCheckedItems()
		{
			String lvc;
			CheckBox lvi;
			for (int i = 0; i < ListEntries.Count; i++)
			{
				lvi = ListEntries[i];
				lvc = lvi.Content.ToString(); //Get the current Object's Name
				if (ObjectCheckMemory.Contains(lvc) == false) //See if the memory does not have our current object
				{
					bool checkStatus = lvi.IsChecked.Value; //Grab the Value of the Checkbox for that Object
					if (checkStatus == true)
					{ //If it returns true, add it to memory
						ObjectCheckMemory.Add(lvc);
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
			for (int i = 0; i < ListEntries.Count; i++)
			{
				lvi = ListEntries[i] as CheckBox;
				lvc = lvi.Content.ToString(); //Get the current Object's Name
				if (ObjectCheckMemory.Contains(lvc) == false) //See if the memory does not have our current object
				{
					bool checkStatus = lvi.IsChecked.Value; //Grab the Value of the Checkbox for that Object
					if (checkStatus == false)
					{ //If it returns false, grab it's index and remove the range
						int index = ObjectCheckMemory.IndexOf(lvc);
						ObjectCheckMemory.RemoveRange(index, 1);
					}
				}
			}
		}
		private void ReloadList()
		{
			if (!FullRefreshNeeded)
			{

				addCheckedItems();
				removeUncheckedItems();
				FullRefreshNeeded = false;
			}
			ListEntries.Clear();
			var targetNames = _TargetSceneObjects.Select(tso => tso.Name.ToString());
			var importableObjects = _TargetSceneObjects.Where(sso => targetNames.Contains(sso.Name.ToString()))
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
				if (!_SourceStageConfig.ObjectsNames.Contains(io.Name.ToString()))
				{
					if (Methods.Solution.CurrentSolution.GameConfig != null)
					{
						if (!Methods.Solution.CurrentSolution.GameConfig.ObjectsNames.Contains(io.Name.ToString()))
						{
							lvc.Foreground = Methods.Internal.Theming.GetSCBResource("Maniac_ObjectManager_RedTextColor");
						}
						else
						{
							lvc.Foreground = Methods.Internal.Theming.GetSCBResource("Maniac_ObjectManager_GreenTextColor");
						}
					}
					else
					{
						lvc.Foreground = Methods.Internal.Theming.GetSCBResource("Maniac_ObjectManager_GreenTextColor");
					}
				}
				InstanceID++;

				bool alreadyChecked = false;
				foreach (string str in ObjectCheckMemory)
				{
					if (ObjectCheckMemory.Contains(lvc.Content.ToString()) == true)
					{
						lvc.IsChecked = true;
						ListEntries.Add(lvc);

						alreadyChecked = true;
						break;
					}
				}
				if (alreadyChecked == false)
				{
					lvc.IsChecked = false;
					ListEntries.Add(lvc);

				}


			}
			lvObjectsViewer.Refresh();
		}
		public void RefreshList()
		{
			CommonReset();
			var targetNames = _TargetSceneObjects.Select(tso => tso.Name.ToString());
			var importableObjects = _TargetSceneObjects.Where(sso => targetNames.Contains(sso.Name.ToString()))
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
			ObjectCheckMemory.Clear();
			ListEntries.Clear();
		}
		private void updateSelectedText()
		{
			//label1.Text = "Amount of Objects Selected (Memory): " + objectCheckMemory.Count + " (Current): " + lvObjects.CheckedItems.Count;
			label1.Content = "Amount of Objects Selected (Memory): " + ObjectCheckMemory.Count;
		}

		#endregion

		#region Classes

		public class AttributeItem
		{
			public string Name { get; set; }

			public string Type { get; set; }
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
