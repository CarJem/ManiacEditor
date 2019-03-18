using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using RSDKv5;
using System.Diagnostics;
using Microsoft.Scripting.Utils;
using WPFGrid = Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid;
using System.ComponentModel;
using System.Drawing;
using System.Data;

namespace ManiacEditor
{
	/// <summary>
	/// Interaction logic for TilesToolbar.xaml
	/// </summary>
	public partial class EntitiesToolbar2 : UserControl
	{
		public Action<int> SelectedEntity;
		public Action<Actions.IAction> AddAction;
		public Action<RSDKv5.SceneObject> Spawn;

		public bool multipleObjects = false;
		public bool IndexChangeLock = false;

		List<int> SelectedObjectListIndexes = new List<int>();

		public Editor EditorInstance;

		public System.Windows.Forms.PropertyGrid entityProperties;

		private List<RSDKv5.SceneEntity> _entities;

		private TextBlock[] ObjectList= new TextBlock[2048];


		private List<int> _selectedEntitySlots = new List<int>();
		private BindingList<TextBlock> _bindingSceneObjectsSource = new BindingList<TextBlock>();

		private RSDKv5.SceneEntity currentEntity;

		

		public List<RSDKv5.SceneEntity> Entities
		{
			set
			{
				_entities = value.ToList();
				_entities.Sort((x, y) => x.SlotID.CompareTo(y.SlotID));
				UpdateEntitiesList();
			}
		}

		public List<RSDKv5.SceneEntity> SelectedEntities
		{
			set
			{
				UpdateEntitiesProperties(value);
			}

		}

		public bool NeedRefresh;

		public EntitiesToolbar2(List<RSDKv5.SceneObject> sceneObjects, Editor instance)
		{
			EditorInstance = instance;

			InitializeComponent();




			if (!Settings.mySettings.ExperimentalPropertyGridView)
			{
				entityPropertiesLegacy.Visibility = Visibility.Visible;
				entityProperties2.Visibility = Visibility.Hidden;
				entityPropertiesLegacy.IsEnabled = true;
				entityProperties2.IsEnabled = false;

				entityProperties = new System.Windows.Forms.PropertyGrid();
				this.entityProperties.Dock = System.Windows.Forms.DockStyle.Fill;
				this.entityProperties.BackColor = System.Drawing.Color.White;
				this.entityProperties.CategoryForeColor = System.Drawing.Color.Black;
				this.entityProperties.CommandsBorderColor = System.Drawing.Color.DarkGray;
				this.entityProperties.CommandsForeColor = System.Drawing.Color.Black;
				this.entityProperties.HelpBackColor = System.Drawing.Color.White;
				this.entityProperties.HelpBorderColor = System.Drawing.Color.DarkGray;
				this.entityProperties.HelpForeColor = System.Drawing.Color.Black;
				this.entityProperties.HelpVisible = false;
				this.entityProperties.LineColor = System.Drawing.Color.Silver;
				this.entityProperties.Location = new System.Drawing.Point(7, 46);
				this.entityProperties.Name = "entityProperties";
				this.entityProperties.PropertySort = System.Windows.Forms.PropertySort.Categorized;
				this.entityProperties.SelectedItemWithFocusBackColor = System.Drawing.Color.DodgerBlue;
				this.entityProperties.SelectedItemWithFocusForeColor = System.Drawing.Color.White;
				this.entityProperties.Size = new System.Drawing.Size(234, 236);
				this.entityProperties.TabIndex = 1;
				this.entityProperties.ToolbarVisible = false;
				this.entityProperties.ViewBackColor = System.Drawing.Color.White;
				this.entityProperties.ViewBorderColor = System.Drawing.Color.DarkGray;
				this.entityProperties.ViewForeColor = System.Drawing.Color.Black;
				this.entityProperties.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.entityProperties_PropertyValueChanged);
				entityPropertiesLegacy.Child = entityProperties;
				entityProperties.Show();
			}
			else
			{
				entityPropertiesLegacy.Visibility = Visibility.Hidden;
				entityProperties2.Visibility = Visibility.Visible;
				entityPropertiesLegacy.IsEnabled = false;
				entityProperties2.IsEnabled = true;
			}

			RefreshSpawningObjects(sceneObjects);

			UpdateDefaultFilter(true);

			UpdateFilterNames(true);

			UpdateEntitiesList(true);

			entitiesList.ItemsSource = ObjectList;


		}

		public void UpdateFilterNames(bool startup = false)
		{
			if (startup)
			{
				maniaFilter.Foreground = GetColorBrush(2);
				encoreFilter.Foreground = GetColorBrush(4);
				pinballFilter.Foreground = GetColorBrush(255);
				bothFilter.Foreground = GetColorBrush(1);
				otherFilter.Foreground = GetColorBrush(0);

				maniaFilterCheck.Foreground = GetColorBrush(2);
				encoreFilterCheck.Foreground = GetColorBrush(4);
				otherFilterCheck.Foreground = GetColorBrush(0);
				bothFilterCheck.Foreground = GetColorBrush(1);
				pinballFilterCheck.Foreground = GetColorBrush(255);
			}
			if (Properties.Settings.Default.useBitOperators)
			{
				maniaFilterCheck.Content = "Mania (0b0010)";
				encoreFilterCheck.Content = "Encore (0b0100)";
				otherFilterCheck.Content = "Other (0b0000)";
				bothFilterCheck.Content = "Both (0b0001)";
				pinballFilterCheck.Content = "All (0b11111111)";

				maniaFilter.Content = "Mania (0b0010)";
				encoreFilter.Content = "Encore (0b0100)";
				otherFilter.Content = "Other (0b0000)";
				bothFilter.Content = "Both (0b0001)";
				pinballFilter.Content = "All (0b11111111)";
			}
			else
			{
				maniaFilterCheck.Content = "Mania (2)";
				encoreFilterCheck.Content = "Encore (4)";
				otherFilterCheck.Content = "Other (0)";
				bothFilterCheck.Content = "Both (1 & 5)";
				pinballFilterCheck.Content = "All (255)";

				maniaFilter.Content = "Mania (2)";
				encoreFilter.Content = "Encore (4)";
				otherFilter.Content = "Other (0)";
				bothFilter.Content = "Both (1 & 5)";
				pinballFilter.Content = "All (255)";
			}

		}

		public void UpdateEntitiesList(bool FirstLoad = false)
		{
			//This if statement Triggers when the toolbar opens for the first time
			if (FirstLoad) _entities = EditorInstance.entities.Entities.Select(x => x.Entity).ToList();
			int j = 0;
			for (int i = 0; i < 2048; i++)
			{
				SceneEntity entity;
				if (j < _entities.Count)
				{
					entity = _entities[j];
				}
				else
				{
					entity = null;
				}

				if (entity != null && entity.SlotID == i)
				{
					Visibility VisibilityStatus = GetObjectListItemVisiblity(entity.Object.Name.Name, entity.SlotID);
					ObjectList[i] = new TextBlock()
					{
						Text = String.Format("{0} - {1}", entity.Object.Name.Name, entity.SlotID),
						Foreground = GetColorBrush(entity),
						Tag = entity.SlotID.ToString(),
						Visibility = VisibilityStatus
					};
					j++;
				}
				else
				{

					ObjectList[i] = new TextBlock()
					{
						Text = String.Format("{0} - {1}", "UNUSED", i),
						Foreground = GetColorBrush(256),
						Height = 0,
						Visibility = Visibility.Collapsed,
						Tag = "NULL"
						
					};					
				}
			}
			if (currentEntity != null) GoToObject.IsEnabled = true;
			else GoToObject.IsEnabled = false;

            if (EditorInstance.entities.SelectedEntities != null && EditorInstance.entities.SelectedEntities.Count > 1) SortSelectedSlotIDs.IsEnabled = true;
            else SortSelectedSlotIDs.IsEnabled = false;
        }

		public Visibility GetObjectListItemVisiblity(string name, ushort slotID)
		{
			if (multipleObjects == true)
			{
				if (SelectedObjectListIndexes.Contains((int)slotID))
				{
					return Visibility.Visible;
				}
				else
				{
					return Visibility.Collapsed;
				}
			} 
			else
			{
				if (searchBox.Text != "")
				{
					if (name.Contains(searchBox.Text))
					{
						return Visibility.Visible;
					}
					else
					{
						return Visibility.Collapsed;
					}
				}
				else
				{
					return Visibility.Visible;
				}
			}


		}

		public void UpdateDefaultFilter(bool startup)
		{
			if (startup)
			{
				switch(Properties.Settings.Default.DefaultFilter[0])
				{
					case 'M':
						defaultFilter.SelectedIndex = 0;
						break;
					case 'E':
						defaultFilter.SelectedIndex = 1;
						break;
					case 'B':
						defaultFilter.SelectedIndex = 2;
						break;
					case 'P':
						defaultFilter.SelectedIndex = 3;
						break;
					case 'O':
						defaultFilter.SelectedIndex = 4;
						break;
					default:
						defaultFilter.SelectedIndex = 0;
						break;
				}
			}
			else
			{
				switch (defaultFilter.SelectedIndex)
				{
					case 0:
						Properties.Settings.Default.DefaultFilter = "M";
						break;
					case 1:
						Properties.Settings.Default.DefaultFilter = "E";
						break;
					case 2:
						Properties.Settings.Default.DefaultFilter = "B";
						break;
					case 3:
						Properties.Settings.Default.DefaultFilter = "P";
						break;
					case 4:
						Properties.Settings.Default.DefaultFilter = "O";
						break;
					default:
						Properties.Settings.Default.DefaultFilter = "M";
						break;
				}
			}
		}

		public void RefreshSpawningObjects(List<RSDKv5.SceneObject> sceneObjects)
		{
			try
			{
				sceneObjects.Sort((x, y) => x.Name.ToString().CompareTo(y.Name.ToString()));
				var bindingSceneObjectsList = new BindingList<RSDKv5.SceneObject>(sceneObjects);


				_bindingSceneObjectsSource.Clear();
				foreach (var _object in bindingSceneObjectsList)
				{
					TextBlock item = new TextBlock()
					{
						Tag = _object,
						Text = _object.Name.Name
					};
					_bindingSceneObjectsSource.Add(item);
				}

				if (_bindingSceneObjectsSource != null && _bindingSceneObjectsSource.Count > 1)
				{
					cbSpawn.ItemsSource = _bindingSceneObjectsSource;
					cbSpawn.SelectedItem = cbSpawn.Items[0];
					var SelectedItem = cbSpawn.SelectedItem as TextBlock;
					if (SelectedItem == null) return;
					SelectedItem.Foreground = (SolidColorBrush)this.FindResource("NormalText");
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}

		}



		public void EntitiesList_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
		{
			if (currentEntity != null)
			{
				if (entitiesList.SelectedIndex == -1) return;
				if (entitiesList.SelectedIndex != currentEntity.SlotID)
				{
					Int32.TryParse(ObjectList[entitiesList.SelectedIndex].Tag.ToString(), out int selectedIndexInt);
					SelectedEntity?.Invoke(_entities.Where(x => x.SlotID == selectedIndexInt).First().SlotID);
				}
			}
			else if (multipleObjects == true)
			{
				if (entitiesList.SelectedIndex == -1) return;

				Int32.TryParse(ObjectList[entitiesList.SelectedIndex].Tag.ToString(), out int selectedIndexInt);
				SelectedEntity?.Invoke(_entities.Where(x => x.SlotID == selectedIndexInt).First().SlotID);
				
			}
			else if (currentEntity == null)
			{
				if (entitiesList.SelectedIndex == -1) return;

				Int32.TryParse(ObjectList[entitiesList.SelectedIndex].Tag.ToString(), out int selectedIndexInt);
				SelectedEntity?.Invoke(_entities.Where(x => x.SlotID == selectedIndexInt).First().SlotID);
			}
            if (currentEntity != null) GoToObject.IsEnabled = true;
            else GoToObject.IsEnabled = false;

            if (EditorInstance.entities.SelectedEntities != null && EditorInstance.entities.SelectedEntities.Count > 0) SortSelectedSlotIDs.IsEnabled = true;
            else SortSelectedSlotIDs.IsEnabled = false;

        }

		public int GetIndexOfSlotID(int slotID)
		{
			int index = 0;
			for (int i = 0; i < _entities.Count; i++)
			{
				if (_entities[i].SlotID == (ushort)slotID)
				{
					index = i;
				}
			}
			return index;
		}

		private void AddProperty(LocalProperties properties, int category_index, string category, string name, string value_type, object value, bool read_only = false)
		{
			properties.Add(String.Format("{0},{1}", category, name), new LocalProperty(name, value_type, category_index, category, name, true, read_only, value, ""));

		}

		public System.Drawing.Color GetSenstiveFilterColors(string colorID)
		{
			if (colorID == "Blue")
			{
				if (Settings.mySettings.NightMode) return System.Drawing.Color.LightBlue;
				else return System.Drawing.Color.Blue;
			}
			else if (colorID == "Green")
			{
				if (Settings.mySettings.NightMode) return System.Drawing.Color.LightGreen;
				else return System.Drawing.Color.Green;
			}
			else if (colorID == "Red")
			{
				if (Settings.mySettings.NightMode) return System.Drawing.Color.FromArgb(211,76,49);
				else return System.Drawing.Color.Red;
			}
			else
			{
				if (Settings.mySettings.NightMode) return System.Drawing.Color.White;
				else return System.Drawing.Color.Black;
			}
		}

		public SolidColorBrush GetColorBrush(SceneEntity entity)
		{
			int filter = GetFilter(entity);
			SolidColorBrush ForeColor = (SolidColorBrush)this.FindResource("NormalText");
			switch (filter)
			{
				case var _ when(filter == 0 || filter >= 5 && filter != 255):
					ForeColor = new SolidColorBrush(Extensions.ColorConvertToMedia(System.Drawing.Color.Gold)); // Other Filter
					break;
				case var _ when (filter == 1 || filter == 5):
					ForeColor = new SolidColorBrush(Extensions.ColorConvertToMedia(GetSenstiveFilterColors("Blue"))); // Both Filter
					break;
				case 2:
					ForeColor = new SolidColorBrush(Extensions.ColorConvertToMedia(GetSenstiveFilterColors("Red"))); // Mania Filter
					break;
				case 4:
					ForeColor = new SolidColorBrush(Extensions.ColorConvertToMedia(GetSenstiveFilterColors("Green"))); //Encore Filter
					break;
				case 255:
					ForeColor = new SolidColorBrush(Extensions.ColorConvertToMedia(System.Drawing.Color.Violet)); // All Filter
                    break;
				default:
					ForeColor = (SolidColorBrush)this.FindResource("NormalText"); // NULL Filter
					break;
			}
			return ForeColor; 
		}

		public SolidColorBrush GetColorBrush(int filter)
		{
			SolidColorBrush ForeColor = (SolidColorBrush)this.FindResource("NormalText");
			switch (filter)
			{
				case var _ when (filter == 0 || filter >= 5 && filter != 255):
					ForeColor = new SolidColorBrush(Extensions.ColorConvertToMedia(System.Drawing.Color.Gold)); // Other Filter
					break;
				case var _ when (filter == 1 || filter == 5):
					ForeColor = new SolidColorBrush(Extensions.ColorConvertToMedia(GetSenstiveFilterColors("Blue"))); // Both Filter
					break;
				case 2:
					ForeColor = new SolidColorBrush(Extensions.ColorConvertToMedia(GetSenstiveFilterColors("Red"))); // Mania Filter
					break;
				case 4:
					ForeColor = new SolidColorBrush(Extensions.ColorConvertToMedia(GetSenstiveFilterColors("Green"))); //Encore Filter
					break;
				case 255:
					ForeColor = new SolidColorBrush(Extensions.ColorConvertToMedia(System.Drawing.Color.Violet)); // All Filter
                    break;
				default:
					ForeColor = (SolidColorBrush)this.FindResource("NormalText"); // NULL Filter
					break;
			}
			return ForeColor;
		}

		public void Dispose()
		{
			if (this.entityProperties != null)
			{
				this.entityProperties.Dispose();
				this.entityProperties = null;
			}

		}


		public int GetFilter(SceneEntity entity)
		{
			if (entity.attributesMap.ContainsKey("filter") && entity.attributesMap["filter"].Type == AttributeTypes.UINT8)
			{
				int filter = entity.attributesMap["filter"].ValueUInt8;
				return filter;
			}
			else
			{
				return -1;
			}
		}

		private void UpdateEntitiesProperties(List<RSDKv5.SceneEntity> selectedEntities)
		{
			// Reset the List Item if the Current Entity is nothing or if it's a multi-selection
			if (selectedEntities.Count !=  1 || currentEntity == null) entitiesList.SelectedItem = null;

			multipleObjects = false;
			bool isCommonObjects = false;
			SelectedObjectListIndexes.Clear();

			EntityEditor.Header = "Entity Editor";

			if (selectedEntities.Count != 1)
			{
				if (Settings.mySettings.ExperimentalPropertyGridView) entityProperties2.SelectedObject = null;
				else entityProperties.SelectedObject = null;
				currentEntity = null;
				_selectedEntitySlots.Clear();
				if (selectedEntities.Count > 1)
				{
					// Then we are selecting multiple objects				
					isCommonObjects = true;
					EntityEditor.Header =  "Entity Editor | " + String.Format("{0} entities selected", selectedEntities.Count);
					multipleObjects = true;
					string commonObject = selectedEntities[0].Object.Name.Name;
					foreach (RSDKv5.SceneEntity selectedEntity in selectedEntities)
					{
						SelectedObjectListIndexes.Add(selectedEntity.SlotID);
						if (selectedEntity.Object.Name.Name != commonObject)
						{
							isCommonObjects = false;
						}
					}

				}
				if (isCommonObjects == true)
				{
					//Keep Going (if Implemented; which it's not)
					return;
				}
				else
				{
					return;
				}

			}

			

			RSDKv5.SceneEntity entity = selectedEntities[0];

			if (entity == currentEntity) return;
			currentEntity = entity;

			if (entitiesList.SelectedIndex >= 0 && entitiesList.SelectedIndex < _entities.Count && _entities[entitiesList.SelectedIndex] == currentEntity)
			{
				// Than it is called from selected item in the menu, so changeing the text will remove it, we don't want that
			}
			else
			{
				//Select the Only Item that is Selected
				entitiesList.SelectedItem = ObjectList[entity.SlotID];
			}
			UpdateEntitiesList();
			AssignEntityProperties(entity);
		}

		public void AssignEntityProperties(SceneEntity entity)
		{
			LocalProperties objProperties = new LocalProperties();
			int category_index = 2 + entity.Attributes.Count;
			AddProperty(objProperties, category_index, "object", "name", "string", entity.Object.Name.ToString(), false);
			AddProperty(objProperties, category_index, "object", "entitySlot", "ushort", entity.SlotID, false);
			--category_index;
			AddProperty(objProperties, category_index, "position", "x", "float", entity.Position.X.High + ((float)entity.Position.X.Low / 0x10000));
			AddProperty(objProperties, category_index, "position", "y", "float", entity.Position.Y.High + ((float)entity.Position.Y.Low / 0x10000));
			--category_index;


			foreach (var attribute in entity.Object.Attributes)
			{
				string attribute_name = attribute.Name.ToString();
				var attribute_value = currentEntity.GetAttribute(attribute_name);
				switch (attribute.Type)
				{
					case RSDKv5.AttributeTypes.UINT8:
						AddProperty(objProperties, category_index, attribute_name, "uint8", "byte", attribute_value.ValueUInt8);
						break;
					case RSDKv5.AttributeTypes.UINT16:
						AddProperty(objProperties, category_index, attribute_name, "uint16", "ushort", attribute_value.ValueUInt16);
						break;
					case RSDKv5.AttributeTypes.UINT32:
						AddProperty(objProperties, category_index, attribute_name, "uint32", "uint", attribute_value.ValueUInt32);
						break;
					case RSDKv5.AttributeTypes.INT8:
						AddProperty(objProperties, category_index, attribute_name, "int8", "sbyte", attribute_value.ValueInt8);
						break;
					case RSDKv5.AttributeTypes.INT16:
						AddProperty(objProperties, category_index, attribute_name, "int16", "short", attribute_value.ValueInt16);
						break;
					case RSDKv5.AttributeTypes.INT32:
						AddProperty(objProperties, category_index, attribute_name, "int32", "int", attribute_value.ValueInt32);
						break;
					case RSDKv5.AttributeTypes.VAR:
						AddProperty(objProperties, category_index, attribute_name, "var", "uint", attribute_value.ValueVar);
						break;
					case RSDKv5.AttributeTypes.BOOL:
						AddProperty(objProperties, category_index, attribute_name, "bool", "bool", attribute_value.ValueBool);
						break;
					case RSDKv5.AttributeTypes.STRING:
						AddProperty(objProperties, category_index, attribute_name, "string", "string", attribute_value.ValueString);
						break;
					case RSDKv5.AttributeTypes.POSITION:
						AddProperty(objProperties, category_index, attribute_name, "x", "float", attribute_value.ValuePosition.X.High + ((float)attribute_value.ValuePosition.X.Low / 0x10000));
						AddProperty(objProperties, category_index, attribute_name, "y", "float", attribute_value.ValuePosition.Y.High + ((float)attribute_value.ValuePosition.Y.Low / 0x10000));
						break;
					case RSDKv5.AttributeTypes.COLOR:
						var color = attribute_value.ValueColor;
						AddProperty(objProperties, category_index, attribute_name, "color", "color", System.Drawing.Color.FromArgb(255 /* color.A */, color.R, color.G, color.B));
						break;
				}
				--category_index;
			}
			if (Settings.mySettings.ExperimentalPropertyGridView) entityProperties2.SelectedObject = new LocalPropertyGridObject(objProperties);
			else entityProperties.SelectedObject = new LocalPropertyGridObject(objProperties);
		}

		public void UpdateCurrentEntityProperites()
		{
			object selectedObject = (Settings.mySettings.ExperimentalPropertyGridView ? entityProperties2.SelectedObject : entityProperties.SelectedObject);
			if (selectedObject is LocalPropertyGridObject obj)
			{
				obj.setValue("position,x", currentEntity.Position.X.High + ((float)currentEntity.Position.X.Low / 0x10000));
				obj.setValue("position,y", currentEntity.Position.Y.High + ((float)currentEntity.Position.Y.Low / 0x10000));
				foreach (var attribute in currentEntity.Object.Attributes)
				{
					string attribute_name = attribute.Name.ToString();
					var attribute_value = currentEntity.GetAttribute(attribute_name);
					switch (attribute.Type)
					{
						case RSDKv5.AttributeTypes.UINT8:
							obj.setValue(String.Format("{0},{1}", attribute_name, "uint8"), attribute_value.ValueUInt8);
							break;
						case RSDKv5.AttributeTypes.UINT16:
							obj.setValue(String.Format("{0},{1}", attribute_name, "uint16"), attribute_value.ValueUInt16);
							break;
						case RSDKv5.AttributeTypes.UINT32:
							obj.setValue(String.Format("{0},{1}", attribute_name, "uint32"), attribute_value.ValueUInt32);
							break;
						case RSDKv5.AttributeTypes.INT8:
							obj.setValue(String.Format("{0},{1}", attribute_name, "int8"), attribute_value.ValueInt8);
							break;
						case RSDKv5.AttributeTypes.INT16:
							obj.setValue(String.Format("{0},{1}", attribute_name, "int16"), attribute_value.ValueInt16);
							break;
						case RSDKv5.AttributeTypes.INT32:
							obj.setValue(String.Format("{0},{1}", attribute_name, "int32"), attribute_value.ValueInt32);
							break;
						case RSDKv5.AttributeTypes.VAR:
							obj.setValue(String.Format("{0},{1}", attribute_name, "var"), attribute_value.ValueVar);
							break;
						case RSDKv5.AttributeTypes.BOOL:
							obj.setValue(String.Format("{0},{1}", attribute_name, "bool"), attribute_value.ValueBool);
							break;
						case RSDKv5.AttributeTypes.STRING:
							obj.setValue(String.Format("{0},{1}", attribute_name, "string"), attribute_value.ValueString);
							break;
						case RSDKv5.AttributeTypes.POSITION:
							obj.setValue(String.Format("{0},{1}", attribute_name, "x"), attribute_value.ValuePosition.X.High + ((float)attribute_value.ValuePosition.X.Low / 0x10000));
							obj.setValue(String.Format("{0},{1}", attribute_name, "y"), attribute_value.ValuePosition.Y.High + ((float)attribute_value.ValuePosition.Y.Low / 0x10000));
							break;
						case RSDKv5.AttributeTypes.COLOR:
							var color = attribute_value.ValueColor;
							obj.setValue(String.Format("{0},{1}", attribute_name, "color"), System.Drawing.Color.FromArgb(255 /* color.A */, color.R, color.G, color.B));
							break;
					}
				}
				NeedRefresh = true;

			}
		}

		public void PropertiesRefresh()
		{
			if (Settings.mySettings.ExperimentalPropertyGridView) entityProperties2.Update();
			else entityProperties.Refresh();
			NeedRefresh = false;
		}
		private void setEntitiyProperty(RSDKv5.SceneEntity entity, string tag, object value, object oldValue)
		{
			string[] parts = tag.Split(',');
			string category = parts[0];
			string name = parts[1];
			if (category == "position")
			{
				float fvalue = (float)value;
				if (fvalue < Int16.MinValue || fvalue > Int16.MaxValue)
				{
					// Invalid
					var obj = (Settings.mySettings.ExperimentalPropertyGridView ? entityProperties2.SelectedObject as LocalPropertyGridObject : entityProperties.SelectedObject as LocalPropertyGridObject);
					obj.setValue(tag, oldValue);
					return;
				}
				var pos = entity.Position;
				if (name == "x")
				{
					pos.X.High = (short)fvalue;
					pos.X.Low = (ushort)(fvalue * 0x10000);
				}
				else if (name == "y")
				{
					pos.Y.High = (short)fvalue;
					pos.Y.Low = (ushort)(fvalue * 0x10000);
				}
				entity.Position = pos;
				if (entity == currentEntity)
					UpdateCurrentEntityProperites();
			}
			else if (category == "object")
			{
				if (name == "name" && oldValue != value)
				{
					var info = RSDKv5.Objects.GetObjectName(new RSDKv5.NameIdentifier(value as string));
					if (info == null)
					{
						MessageBox.Show("Unknown Object", "", MessageBoxButton.OK, MessageBoxImage.Error);
						return;
					}
					var objectsList = ((BindingList<TextBlock>)_bindingSceneObjectsSource).ToList();
					var objects = objectsList.Select(x => x.Tag as RSDKv5.SceneObject).ToList();
					var obj = objects.FirstOrDefault(t => t.Name.Name == value as string);
					if (obj != null)
					{
                        var attribs = entity.Object.Attributes;
                        entity.Attributes.Clear();
						entity.attributesMap.Clear();
						foreach (var attb in attribs)
						{
							var attributeValue = new RSDKv5.AttributeValue(attb.Type);
							entity.Attributes.Add(attributeValue);
							entity.attributesMap.Add(attb.Name.Name, attributeValue);
						}
						entity.Object.Entities.Remove(entity);
						entity.Object = obj;
						obj.Entities.Add(entity);
					}
					else
					{
						// The new object
						var sobj = new RSDKv5.SceneObject(entity.Object.Name, entity.Object.Attributes);

						entity.Attributes.Clear();
						entity.attributesMap.Clear();
						foreach (var attb in entity.Object.Attributes)
						{
							var attributeValue = new RSDKv5.AttributeValue(attb.Type);
							entity.Attributes.Add(attributeValue);
							entity.attributesMap.Add(attb.Name.Name, attributeValue);
						}
						entity.Object.Entities.Remove(entity);
						entity.Object = sobj;
						sobj.Entities.Add(entity);
						TextBlock newItem = new TextBlock()
						{
							Tag = sobj,
							Foreground = (SolidColorBrush)this.FindResource("NormalText"),
							Text = sobj.Name.Name
						};
						_bindingSceneObjectsSource.Add(newItem);
					}
				}
				if (name == "entitySlot" && oldValue != value)
				{
					ushort newSlot = (ushort)value;
					// Check if slot has been used
					var objectsList = ((BindingList<TextBlock>)_bindingSceneObjectsSource).ToList();
					var objects = objectsList.Select(x => x.Tag as RSDKv5.SceneObject).ToList();
					foreach (var obj in objects)
					{
						if (obj.Entities.Any(t => t.SlotID == newSlot))
						{
							MessageBox.Show("Slot " + newSlot + " is currently being used by a " + obj.Name.ToString(),
								"Slot in use!", MessageBoxButton.OK, MessageBoxImage.Error);
							return;
						}
						if (newSlot > 2048)
						{
							MessageBox.Show("Slot " + newSlot + " is bigger than the maximum amount of objects allowed!",
								"Slot is too big!", MessageBoxButton.OK, MessageBoxImage.Error);
							return;
						}
					}
					// Passed
					entity.SlotID = newSlot;
				}
				// Update Properties
				currentEntity = null;
				UpdateEntitiesProperties(new List<RSDKv5.SceneEntity>() { entity });
				//UpdateEntitiesList();
			}
			else
			{
				var attribute = entity.GetAttribute(category);
				switch (attribute.Type)
				{
					case RSDKv5.AttributeTypes.UINT8:
						attribute.ValueUInt8 = (byte)value;
						break;
					case RSDKv5.AttributeTypes.UINT16:
						attribute.ValueUInt16 = (ushort)value;
						break;
					case RSDKv5.AttributeTypes.UINT32:
						attribute.ValueUInt32 = (uint)value;
						break;
					case RSDKv5.AttributeTypes.INT8:
						attribute.ValueInt8 = (sbyte)value;
						break;
					case RSDKv5.AttributeTypes.INT16:
						attribute.ValueInt16 = (short)value;
						break;
					case RSDKv5.AttributeTypes.INT32:
						attribute.ValueInt32 = (int)value;
						break;
					case RSDKv5.AttributeTypes.VAR:
						attribute.ValueVar = (uint)value;
						break;
					case RSDKv5.AttributeTypes.BOOL:
						attribute.ValueBool = (bool)value;
						break;
					case RSDKv5.AttributeTypes.STRING:
						attribute.ValueString = (string)value;
						break;
					case RSDKv5.AttributeTypes.POSITION:
						float fvalue = (float)value;
						if (fvalue < Int16.MinValue || fvalue > Int16.MaxValue)
						{
							// Invalid
							var obj = (Settings.mySettings.ExperimentalPropertyGridView ? entityProperties2.SelectedObject as LocalPropertyGridObject : entityProperties.SelectedObject as LocalPropertyGridObject);
							obj.setValue(tag, oldValue);
							return;
						}
						var pos = attribute.ValuePosition;
						if (name == "x")
						{
							pos.X.High = (short)fvalue;
							pos.X.Low = (ushort)(fvalue * 0x10000);
						}
						else if (name == "y")
						{
							pos.Y.High = (short)fvalue;
							pos.Y.Low = (ushort)(fvalue * 0x10000);
						}
						attribute.ValuePosition = pos;
						if (entity == currentEntity)
							UpdateCurrentEntityProperites();
						break;
					case RSDKv5.AttributeTypes.COLOR:
						System.Drawing.Color c = (System.Drawing.Color)value;
						attribute.ValueColor = new RSDKv5.Color(c.R, c.G, c.B, c.A);
						break;
				}
			}
		}


		private void entityProperties_PropertyValueChanged(object s, System.Windows.Forms.PropertyValueChangedEventArgs e)
		{
			if (!Settings.mySettings.ExperimentalPropertyGridView)
			{
				string tag = e.ChangedItem.PropertyDescriptor.Name;
				AddAction?.Invoke(new Actions.ActionEntityPropertyChange(currentEntity, tag, e.OldValue, e.ChangedItem.Value, new Action<RSDKv5.SceneEntity, string, object, object>(setEntitiyProperty)));
				setEntitiyProperty(currentEntity, tag, e.ChangedItem.Value, e.OldValue);
			}


		}

		private void entityProperties2_PropertyValueChanged(object s, Xceed.Wpf.Toolkit.PropertyGrid.PropertyValueChangedEventArgs e)
		{
			if (Settings.mySettings.ExperimentalPropertyGridView)
			{
				string tag = e.OriginalSource.ToString();
				AddAction?.Invoke(new Actions.ActionEntityPropertyChange(currentEntity, tag, e.OldValue, e.NewValue, new Action<RSDKv5.SceneEntity, string, object, object>(setEntitiyProperty)));
				setEntitiyProperty(currentEntity, tag, e.NewValue, e.OldValue);
			}
		}

		private void entitiesList_DropDown(object sender, EventArgs e)
		{
			entitiesList.ItemsSource = null;
			entitiesList.Items.Clear();
			UpdateEntitiesList();
			entitiesList.ItemsSource = ObjectList;
		}

		private void btnSpawn_Click(object sender, RoutedEventArgs e)
		{
			if (cbSpawn?.SelectedItem != null && cbSpawn.SelectedItem is TextBlock)
			{
				var selectedItem = cbSpawn.SelectedItem as TextBlock;
				if (selectedItem.Tag == null) return;
				if (selectedItem.Tag is RSDKv5.SceneObject)
				{
					var obj = selectedItem.Tag as RSDKv5.SceneObject;
					switch (Properties.Settings.Default.DefaultFilter[0])
					{
						case 'M':
							EditorInstance.entities.DefaultFilter = 2;
							break;
						case 'E':
							EditorInstance.entities.DefaultFilter = 4;
							break;
						case 'B':
							EditorInstance.entities.DefaultFilter = 1;
							break;
						case 'P':
							EditorInstance.entities.DefaultFilter = 255;
							break;
						default:
							EditorInstance.entities.DefaultFilter = 0;
							break;
					}
					Spawn?.Invoke(obj);
				}
			}
		}


		private void maniaFilterCheck_CheckedChanged(object sender, RoutedEventArgs e)
		{
			EditorInstance.entities.FilterRefreshNeeded = true;	
		}

		private void encoreFilterCheck_CheckedChanged(object sender, RoutedEventArgs e)
		{
			EditorInstance.entities.FilterRefreshNeeded = true;
		}

		private void bothFilterCheck_CheckedChanged(object sender, RoutedEventArgs e)
		{
			EditorInstance.entities.FilterRefreshNeeded = true;
		}

		private void PinballFilterCheck_CheckedChanged(object sender, RoutedEventArgs e)
		{
			EditorInstance.entities.FilterRefreshNeeded = true;
		}

		private void otherFilterCheck_CheckedChanged(object sender, RoutedEventArgs e)
		{
			EditorInstance.entities.FilterRefreshNeeded = true;
		}

		private void cbSpawn_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				btnSpawn_Click(sender, e);
			}
		}


		private void button2_Click(object sender, RoutedEventArgs e)
		{
			MoreButton.ContextMenu.IsOpen = true;
		}

		private void goToThisEntityToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
            if (currentEntity != null)
			{
				int x = currentEntity.Position.X.High;
				int y = currentEntity.Position.Y.High;
				EditorInstance.GoToPosition(x, y);
			}
		}

		private void ComboBoxItem_Selected(object sender, RoutedEventArgs e)
		{

		}

		private void FilterBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			UpdateDefaultFilter(false);
		}

		private void EntitiesList_DropDownClosed(object sender, EventArgs e)
		{
			if (currentEntity != null)
			{
				entitiesList.SelectedItem = ObjectList[(int)currentEntity.SlotID];
			}
		}

		private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			UpdateEntitiesList();
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{

		}

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SortSelectedSlotIDs_Click(object sender, RoutedEventArgs e)
        {
            EditorInstance.entities.OrderSelectedSlotIDs();
        }

        private void SortSelectedSlotIDsOptimized_Click(object sender, RoutedEventArgs e)
        {
            EditorInstance.entities.OrderSelectedSlotIDs(true);
        }
    }
}
