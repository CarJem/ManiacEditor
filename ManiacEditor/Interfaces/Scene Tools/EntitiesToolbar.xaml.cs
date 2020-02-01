using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using RSDKv5;
using Microsoft.Scripting.Utils;
using System.ComponentModel;
using System.Data;

namespace ManiacEditor
{
    /// <summary>
    /// Interaction logic for TilesToolbar.xaml
    /// </summary>
    public partial class EntitiesToolbar : UserControl
	{
		public Action<int> SelectedEntity;
		public Action<Actions.IAction> AddAction;
		public Action<RSDKv5.SceneObject> Spawn;
        public Action<RSDKv5.SceneObject> SpawnInternal;


        public bool multipleObjects = false;
		public bool IndexChangeLock = false;

		List<int> SelectedObjectListIndexes = new List<int>();

		public Editor EditorInstance;

		public System.Windows.Forms.PropertyGrid entityProperties;

		private List<RSDKv5.SceneEntity> _entities;

		private Button[] ObjectList = new Button[2301];


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
                int splineID = Classes.Editor.SolutionState.SelectedSplineID;
                if (Editor.Instance.EditorToolbar.SplineToolButton.IsChecked.Value && Classes.Editor.SolutionState.SplineOptionsGroup.ContainsKey(splineID) && Classes.Editor.SolutionState.SplineOptionsGroup[splineID].SplineObjectRenderingTemplate != null)
                {
                    UpdateEntitiesProperties(new List<SceneEntity>() { Classes.Editor.SolutionState.SplineOptionsGroup[splineID].SplineObjectRenderingTemplate.Entity });
                }
                else
                {
                    UpdateEntitiesProperties(value);
                }
            }

		}

        public bool NeedRefresh;

		public EntitiesToolbar(List<RSDKv5.SceneObject> sceneObjects, Editor instance)
		{
			EditorInstance = instance;

			InitializeComponent();

            SetupWinFormsPropertyGrid();

            RefreshSpawningObjects(sceneObjects);

			UpdateDefaultFilter(true);

			UpdateEntitiesList(true);

            UpdateFilterNames(true);



        }

        public void SetupWinFormsPropertyGrid()
        {
            entityProperties = new System.Windows.Forms.PropertyGrid();

            this.entityProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.entityProperties.HelpVisible = false;
            this.entityProperties.Location = new System.Drawing.Point(7, 46);
            this.entityProperties.Name = "entityProperties";
            this.entityProperties.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.entityProperties.Size = new System.Drawing.Size(234, 236);
            this.entityProperties.TabIndex = 1;
            this.entityProperties.ToolbarVisible = false;

            UpdatePropertyGridTheme();

            this.entityProperties.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.entityProperties_PropertyValueChanged);
            entityPropertiesLegacy.Child = entityProperties;
            entityProperties.Show();
        }

        public void UpdatePropertyGridTheme(bool ForceRefresh = false)
        {
            if (App.Skin == Skin.Dark)
            {
                this.entityProperties.BackColor = EditorTheming.darkTheme0;
                this.entityProperties.CategoryForeColor = EditorTheming.darkTheme3;
                this.entityProperties.CommandsBorderColor = System.Drawing.Color.DarkGray;
                this.entityProperties.CommandsForeColor = System.Drawing.Color.Black;
                this.entityProperties.HelpBackColor = System.Drawing.Color.White;
                this.entityProperties.HelpBorderColor = System.Drawing.Color.DarkGray;
                this.entityProperties.HelpForeColor = System.Drawing.Color.Black;
                this.entityProperties.LineColor = EditorTheming.darkTheme5;
                this.entityProperties.SelectedItemWithFocusBackColor = System.Drawing.Color.Blue;
                this.entityProperties.SelectedItemWithFocusForeColor = System.Drawing.Color.White;
                this.entityProperties.ViewBackColor = EditorTheming.darkTheme0;
                this.entityProperties.ViewBorderColor = EditorTheming.darkTheme1;
                this.entityProperties.ViewForeColor = EditorTheming.darkTheme3;
            }
            else
            {
                this.entityProperties.BackColor = System.Drawing.Color.White;
                this.entityProperties.CategoryForeColor = System.Drawing.Color.Black;
                this.entityProperties.CommandsBorderColor = System.Drawing.Color.DarkGray;
                this.entityProperties.CommandsForeColor = System.Drawing.Color.Black;
                this.entityProperties.HelpBackColor = System.Drawing.Color.White;
                this.entityProperties.HelpBorderColor = System.Drawing.Color.DarkGray;
                this.entityProperties.HelpForeColor = System.Drawing.Color.Black;
                this.entityProperties.LineColor = System.Drawing.Color.Silver;
                this.entityProperties.SelectedItemWithFocusBackColor = System.Drawing.Color.DodgerBlue;
                this.entityProperties.SelectedItemWithFocusForeColor = System.Drawing.Color.White;
                this.entityProperties.ViewBackColor = System.Drawing.Color.White;
                this.entityProperties.ViewBorderColor = System.Drawing.Color.DarkGray;
                this.entityProperties.ViewForeColor = System.Drawing.Color.Black;
            }

            if (ForceRefresh) this.entityProperties.Update();
        }

		public void UpdateFilterNames(bool startup = false)
		{
			if (startup)
			{
				maniaFilter.Foreground = Editor.Instance.Theming.GetColorBrush(2);
				encoreFilter.Foreground = Editor.Instance.Theming.GetColorBrush(4);
				pinballFilter.Foreground = Editor.Instance.Theming.GetColorBrush(255);
				bothFilter.Foreground = Editor.Instance.Theming.GetColorBrush(1);
				otherFilter.Foreground = Editor.Instance.Theming.GetColorBrush(0);
			}
			if (Settings.MySettings.UseBitOperators)
			{
				maniaFilter.Content = "Mania (0b0010)";
				encoreFilter.Content = "Encore (0b0100)";
				otherFilter.Content = "Other (0b0000)";
				bothFilter.Content = "Both (0b0001)";
				pinballFilter.Content = "All (0b11111111)";
			}
			else
			{
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
			if (FirstLoad) _entities = Classes.Editor.Solution.Entities.Entities.Select(x => x.Entity).ToList();
            SceneEntitiesList.Items.Clear();

            int count = (2301 > _entities.Count() ? _entities.Count() : 2031);

            for (int i = 0; i < count; i++)
            {
                var entity = _entities[i];
                if (entity != null)
                {
                    Visibility VisibilityStatus = GetObjectListItemVisiblity(entity.Object.Name.Name, entity.SlotID);
                    if (ObjectList[i] == null)
                    {
                        ObjectList[i] = new System.Windows.Controls.Button()
                        {
                            Content = String.Format("{0} - {1}", entity.Object.Name.Name, entity.SlotID),
                            Foreground = Editor.Instance.Theming.GetColorBrush(entity),
                            Tag = entity.SlotID.ToString(),
                            Visibility = VisibilityStatus
                        };
                        ObjectList[i].Click += EntitiesListEntryClicked;
                    }
                    else
                    {
                        ObjectList[i].Content = String.Format("{0} - {1}", entity.Object.Name.Name, entity.SlotID);
                        ObjectList[i].Foreground = Editor.Instance.Theming.GetColorBrush(entity);
                        ObjectList[i].Tag = entity.SlotID.ToString();
                        ObjectList[i].Visibility = VisibilityStatus;
                    }

                }
                else
                {
                    if (ObjectList[i] == null)
                    {
                        ObjectList[i] = new System.Windows.Controls.Button()
                        {
                            Content = String.Format("{0} - {1}", "UNUSED", i),
                            Foreground = Editor.Instance.Theming.GetColorBrush(256),
                            Height = 0,
                            Visibility = Visibility.Collapsed,
                            Tag = "NULL"

                        };
                        ObjectList[i].Click += EntitiesListEntryClicked;
                    }
                    else
                    {
                        ObjectList[i].Content = String.Format("{0} - {1}", "UNUSED", i);
                        ObjectList[i].Foreground = Editor.Instance.Theming.GetColorBrush(256);
                        ObjectList[i].Height = 0;
                        ObjectList[i].Visibility = Visibility.Collapsed;
                        ObjectList[i].Tag = "NULL";
                    }

                }
                if (ObjectList[i].Visibility != Visibility.Collapsed) SceneEntitiesList.Items.Add(ObjectList[i]);
            }

            if (currentEntity != null) GoToObject.IsEnabled = true;
			else GoToObject.IsEnabled = false;

            if (Classes.Editor.Solution.Entities.SelectedEntities != null && Classes.Editor.Solution.Entities.SelectedEntities.Count > 1 && !Classes.Editor.Solution.Entities.SelectedEntities.ToList().Exists(x => x.IsInternalObject))
            {
                SortSelectedSlotIDs.IsEnabled = true;
                SortSelectedSlotIDsOptimized.IsEnabled = true;
                SortSelectedSlotIDsOrdered.IsEnabled = true;
            }
            else
            {
                SortSelectedSlotIDs.IsEnabled = false;
                SortSelectedSlotIDsOptimized.IsEnabled = false;
                SortSelectedSlotIDsOrdered.IsEnabled = false;
            }
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
				switch(Settings.MyDefaults.DefaultFilter[0])
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
						Settings.MyDefaults.DefaultFilter = "M";
						break;
					case 1:
						Settings.MyDefaults.DefaultFilter = "E";
						break;
					case 2:
						Settings.MyDefaults.DefaultFilter = "B";
						break;
					case 3:
						Settings.MyDefaults.DefaultFilter = "P";
						break;
					case 4:
						Settings.MyDefaults.DefaultFilter = "O";
						break;
					default:
						Settings.MyDefaults.DefaultFilter = "M";
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

		public void Dispose()
		{
			if (this.entityProperties != null)
			{
				this.entityProperties.Dispose();
				this.entityProperties = null;
			}

		}

        private void UpdateSelectedEntitiesList()
        {
            SelectionViewer.Children.Clear();
            if (Classes.Editor.Solution.Entities.SelectedEntities != null)
            {
                foreach (var entity in Classes.Editor.Solution.Entities.SelectedEntities.OrderBy(x => x.TimeWhenSelected))
                {
                    TextBlock entry = new TextBlock();
                    entry.Text = string.Format("{0} | {1} | ID:{2} | X:{3},Y:{4}", string.Format("{0}", entity.SelectedIndex + 1), entity.Name, entity.Entity.SlotID, entity.Entity.Position.X.High, entity.Entity.Position.Y.High);
                    SelectionViewer.Children.Add(entry);
                }
            }

        }

        public void UpdateEntityProperties(List<RSDKv5.SceneEntity> selectedEntities)
        {
            UpdateEntitiesProperties(selectedEntities);
        }

		private void UpdateEntitiesProperties(List<RSDKv5.SceneEntity> selectedEntities)
		{
            // Reset the List Item if the Current Entity is nothing or if it's a multi-selection
            if (currentEntity == null)
            {
                entitiesList.Content = null;
                TabControl.SelectedIndex = 0;
            }
            UpdateSelectedEntitiesList();

            multipleObjects = false;
			bool isCommonObjects = false;
			SelectedObjectListIndexes.Clear();

			EntityEditor.Header = "Entity Editor";

			if (selectedEntities.Count != 1)
			{
                entityProperties.SelectedObject = null;
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
                    TabControl.SelectedIndex = 1;
                }
                else
                {
                    TabControl.SelectedIndex = 0;
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
            else
            {
                TabControl.SelectedIndex = 0;
            }

			

			RSDKv5.SceneEntity entity = selectedEntities[0];

			if (entity == currentEntity) return;
			currentEntity = entity;

            UpdateEntitiesList();

            if (entity.Object.Name.Name != "Spline")
            {
                var entry = ObjectList.Where(x => x.Tag.ToString() == entity.SlotID.ToString()).FirstOrDefault();
                if (entry != null)
                {
                    entitiesList.Content = entry.Content;
                    entitiesList.Foreground = entry.Foreground;
                    entitiesList.Tag = entry.Tag;
                }
                else
                {
                    entitiesList.Content = null;
                    entitiesList.Tag = null;
                }
            }
            else
            {
                entitiesList.Content = null;
                entitiesList.Tag = null;
            }



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
					case RSDKv5.AttributeTypes.ENUM:
						AddProperty(objProperties, category_index, attribute_name, "enum (var)", "uint", attribute_value.ValueEnum);
						break;
					case RSDKv5.AttributeTypes.BOOL:
						AddProperty(objProperties, category_index, attribute_name, "bool", "bool", attribute_value.ValueBool);
						break;
					case RSDKv5.AttributeTypes.STRING:
						AddProperty(objProperties, category_index, attribute_name, "string", "string", attribute_value.ValueString);
						break;
					case RSDKv5.AttributeTypes.VECTOR2:
						AddProperty(objProperties, category_index, attribute_name, "x", "float", attribute_value.ValueVector2.X.High + ((float)attribute_value.ValueVector2.X.Low / 0x10000));
						AddProperty(objProperties, category_index, attribute_name, "y", "float", attribute_value.ValueVector2.Y.High + ((float)attribute_value.ValueVector2.Y.Low / 0x10000));
						break;
					case RSDKv5.AttributeTypes.COLOR:
						var color = attribute_value.ValueColor;
						AddProperty(objProperties, category_index, attribute_name, "color", "color", System.Drawing.Color.FromArgb(255 /* color.A */, color.R, color.G, color.B));
						break;
				}
				--category_index;
			}

            entityProperties.SelectedObject = new LocalPropertyGridObject(objProperties);
		}

		public void UpdateCurrentEntityProperites()
		{
			object selectedObject = entityProperties.SelectedObject;
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
						case RSDKv5.AttributeTypes.ENUM:
							obj.setValue(String.Format("{0},{1}", attribute_name, "enum (var)"), attribute_value.ValueEnum);
							break;
						case RSDKv5.AttributeTypes.BOOL:
							obj.setValue(String.Format("{0},{1}", attribute_name, "bool"), attribute_value.ValueBool);
							break;
						case RSDKv5.AttributeTypes.STRING:
							obj.setValue(String.Format("{0},{1}", attribute_name, "string"), attribute_value.ValueString);
							break;
						case RSDKv5.AttributeTypes.VECTOR2:
							obj.setValue(String.Format("{0},{1}", attribute_name, "x"), attribute_value.ValueVector2.X.High + ((float)attribute_value.ValueVector2.X.Low / 0x10000));
							obj.setValue(String.Format("{0},{1}", attribute_name, "y"), attribute_value.ValueVector2.Y.High + ((float)attribute_value.ValueVector2.Y.Low / 0x10000));
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
            entityProperties.Refresh();
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
                    var obj = entityProperties.SelectedObject as LocalPropertyGridObject;
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
					case RSDKv5.AttributeTypes.ENUM:
						attribute.ValueEnum = (int)value;
						break;
					case RSDKv5.AttributeTypes.BOOL:
						attribute.ValueBool = (bool)value;
						break;
					case RSDKv5.AttributeTypes.STRING:
						attribute.ValueString = (string)value;
						break;
					case RSDKv5.AttributeTypes.VECTOR2:
						float fvalue = (float)value;
						if (fvalue < Int16.MinValue || fvalue > Int16.MaxValue)
						{
							// Invalid
							var obj = entityProperties.SelectedObject as LocalPropertyGridObject;
							obj.setValue(tag, oldValue);
							return;
						}
						var pos = attribute.ValueVector2;
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
						attribute.ValueVector2 = pos;
						if (entity == currentEntity)
							UpdateCurrentEntityProperites();
						break;
					case RSDKv5.AttributeTypes.COLOR:
						System.Drawing.Color c = (System.Drawing.Color)value;
						attribute.ValueColor = new RSDKv5.Color(c.R, c.G, c.B, c.A);
						break;
				}
                UpdateEntitiesProperties(new List<RSDKv5.SceneEntity>() { entity });

            }
        }


		private void entityProperties_PropertyValueChanged(object s, System.Windows.Forms.PropertyValueChangedEventArgs e)
		{
            string tag = e.ChangedItem.PropertyDescriptor.Name;
            AddAction?.Invoke(new Actions.ActionEntityPropertyChange(currentEntity, tag, e.OldValue, e.ChangedItem.Value, new Action<RSDKv5.SceneEntity, string, object, object>(setEntitiyProperty)));
            setEntitiyProperty(currentEntity, tag, e.ChangedItem.Value, e.OldValue);
        }

		private void btnSpawn_Click(object sender, RoutedEventArgs e)
		{
            SpawnObject();
        }

        public void SpawnObject()
        {
            if (cbSpawn?.SelectedItem != null && cbSpawn.SelectedItem is TextBlock)
            {
                var selectedItem = cbSpawn.SelectedItem as TextBlock;
                if (selectedItem.Tag == null) return;
                if (selectedItem.Tag is RSDKv5.SceneObject)
                {
                    var obj = selectedItem.Tag as RSDKv5.SceneObject;
                    switch (Settings.MyDefaults.DefaultFilter[0])
                    {
                        case 'M':
                            Classes.Editor.Solution.Entities.DefaultFilter = 2;
                            break;
                        case 'E':
                            Classes.Editor.Solution.Entities.DefaultFilter = 4;
                            break;
                        case 'B':
                            Classes.Editor.Solution.Entities.DefaultFilter = 1;
                            break;
                        case 'P':
                            Classes.Editor.Solution.Entities.DefaultFilter = 255;
                            break;
                        default:
                            Classes.Editor.Solution.Entities.DefaultFilter = 0;
                            break;
                    }
                    Spawn?.Invoke(obj);
                }
            }
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
				//entitiesList.SelectedItem = ObjectList[(int)currentEntity.SlotID];
			}
		}

		private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			UpdateEntitiesList();
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{

		}

        private void SortSelectedSlotIDs_Click(object sender, RoutedEventArgs e)
        {
            Classes.Editor.Solution.Entities.OrderSelectedSlotIDs();
        }

        private void SortSelectedSlotIDsOptimized_Click(object sender, RoutedEventArgs e)
        {
            Classes.Editor.Solution.Entities.OrderSelectedSlotIDs(true);
        }

        private void SortSelectedSlotIDsOrdered_Click(object sender, RoutedEventArgs e)
        {
            Classes.Editor.Solution.Entities.OrderSelectedSlotIDs(false, true);
        }

        private void EntitiesList_Click(object sender, RoutedEventArgs e)
        {
            if (TabControl.SelectedIndex != 2)
            {
                TabControl.SelectedIndex = 2;
                if (currentEntity != null)
                {
                    var currObject = ObjectList.Where(x => x.Tag.ToString() == currentEntity.SlotID.ToString()).FirstOrDefault();
                    if (currObject != null)
                    {
                        SceneEntitiesList.ScrollIntoView(currObject);
                    }
                }

            }
            else
            {
                TabControl.SelectedIndex = 0;
            }
        }

        private void EntitiesListEntryClicked(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button button = sender as System.Windows.Controls.Button;
            Classes.Editor.Solution.Entities.Deselect();
            Classes.Editor.Solution.Entities.Entities.Where(x => x.Entity.SlotID.ToString() == button.Tag.ToString()).FirstOrDefault().Selected = true;
            TabControl.SelectedIndex = 0;
            SelectedEntities = Classes.Editor.Solution.Entities.SelectedEntities.Select(x => x.Entity).ToList();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ButtonSpinner_Spin(object sender, Xceed.Wpf.Toolkit.SpinEventArgs e)
        {
            if (currentEntity != null)
            {
                int index = ObjectList.IndexOf(ObjectList.Where(x => x.Tag.ToString() == currentEntity.SlotID.ToString()).FirstOrDefault());

                if (e.Direction == Xceed.Wpf.Toolkit.SpinDirection.Decrease) index--;
                else index++;

                if (ObjectList.Length <= index || index < 0) return;

                if (ObjectList[index] != null)
                {
                    Classes.Editor.Solution.Entities.Deselect();
                    Classes.Editor.Solution.Entities.Entities.Where(x => x.Entity.SlotID.ToString() == ObjectList[index].Tag.ToString()).FirstOrDefault().Selected = true;
                    TabControl.SelectedIndex = 0;
                    SelectedEntities = Classes.Editor.Solution.Entities.SelectedEntities.Select(x => x.Entity).ToList();
                }
            }

        }

        private void EntitiesList_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 1)
            {
                ButtonSpinner_Spin(null, new Xceed.Wpf.Toolkit.SpinEventArgs(Xceed.Wpf.Toolkit.SpinDirection.Increase));
            }
            else
            {
                ButtonSpinner_Spin(null, new Xceed.Wpf.Toolkit.SpinEventArgs(Xceed.Wpf.Toolkit.SpinDirection.Decrease));
            }
        }

        private void CbSpawn_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
