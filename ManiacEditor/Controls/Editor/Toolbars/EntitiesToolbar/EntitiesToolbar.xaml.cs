using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using RSDKv5;
using System.ComponentModel;
using System.Data;
using ManiacEditor.Controls.Editor.Toolbars.EntitiesToolbar.PropertyGrid;
using ManiacEditor.Extensions;
using ManiacEditor.Controls.Global.Controls.PropertyGrid;
using GenerationsLib.Core;

namespace ManiacEditor.Controls.Editor.Toolbars.EntitiesToolbar
{
    /// <summary>
    /// Interaction logic for TilesToolbar.xaml
    /// </summary>
    public partial class EntitiesToolbar : UserControl
	{
        #region Definitions
        public Action<int> SelectedEntity { get; set; }
		public Action<Actions.IAction> AddAction { get; set; }
		public Action<RSDKv5.SceneObject> Spawn { get; set; }
        public Action<RSDKv5.SceneObject> SpawnInternal { get; set; }
		public bool MultipleObjectsSelected { get; set; } = false;
		List<int> SelectedObjectListIndexes { get; set; } = new List<int>();
		private ManiacEditor.Controls.Editor.MainEditor Instance { get; set; }
		private List<RSDKv5.SceneEntity> _Entities { get; set; }
		private Button[] ObjectList { get; set; } = new Button[2301];
		private List<int> _SelectedEntitySlots { get; set; } = new List<int>();
		private BindingList<TextBlock> _BindingSceneObjectsSource { get; set; } = new BindingList<TextBlock>();
		private RSDKv5.SceneEntity CurrentEntity { get; set; }
		public List<RSDKv5.SceneEntity> Entities
		{
			set
			{
				_Entities = value.ToList();
				_Entities.Sort((x, y) => x.SlotID.CompareTo(y.SlotID));
				UpdateEntitiesList();
			}
		}
		public List<RSDKv5.SceneEntity> SelectedEntities
		{
			get
			{
				return _SelectedEntities;
			}
			set
			{
                int splineID = Methods.Editor.SolutionState.SelectedSplineID;
                if (ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.SplineToolButton.IsChecked.Value && Methods.Editor.SolutionState.SplineOptionsGroup.ContainsKey(splineID) && Methods.Editor.SolutionState.SplineOptionsGroup[splineID].SplineObjectRenderingTemplate != null)
                {
					UpdatePropertiesGrid(new List<SceneEntity>() { Methods.Editor.SolutionState.SplineOptionsGroup[splineID].SplineObjectRenderingTemplate.Entity });
                }
                else
                {
					UpdatePropertiesGrid(value);
                }
            }

		}

		private List<RSDKv5.SceneEntity> _SelectedEntities { get; set; }

		public bool NeedRefresh { get; set; }
		#endregion

		#region Init
		public EntitiesToolbar(List<RSDKv5.SceneObject> sceneObjects, ManiacEditor.Controls.Editor.MainEditor instance)
		{
			Instance = instance;

			InitializeComponent();

			SetupWinFormsPropertyGrid();

			RefreshSpawningObjects(sceneObjects);

			UpdateDefaultFilter(true);

			UpdateEntitiesList(true);

			UpdateFilterNames(true);



		}
		public void SetupWinFormsPropertyGrid()
		{
			UpdatePropertyGridTheme();
		}
		#endregion

		#region Property Grid 
		public void UpdatePropertiesGrid(List<RSDKv5.SceneEntity> selectedEntities)
		{
			// Reset the List Item if the Current Entity is nothing or if it's a multi-selection
			if (CurrentEntity == null)
			{
				entitiesList.Content = null;
				TabControl.SelectedIndex = 0;
			}
			UpdateSelectedEntitiesList();

			MultipleObjectsSelected = false;
			bool isCommonObjects = false;
			SelectedObjectListIndexes.Clear();

			EntityEditor.Header = "Entity Editor";

			if (selectedEntities.Count != 1)
			{
				PropertiesGrid.SelectedObject = null;
				CurrentEntity = null;
				_SelectedEntitySlots.Clear();
				if (selectedEntities.Count > 1)
				{

					// Then we are selecting multiple objects				
					isCommonObjects = true;
					EntityEditor.Header = "Entity Editor | " + String.Format("{0} entities selected", selectedEntities.Count);
					MultipleObjectsSelected = true;
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
				else
				{
					SetSingleSelect();
				}


				if (selectedEntities == SelectedEntities)
				{
					UpdateSelectedProperties();
				}
				else
				{
					_SelectedEntities = selectedEntities;
					CreateMultiSelectedProperties(_SelectedEntities);
				}

			}

			if (!MultipleObjectsSelected) SetSingleSelect();


			void SetSingleSelect()
			{
				_SelectedEntities = selectedEntities;
				if (selectedEntities.Count == 0) return;
				RSDKv5.SceneEntity entity = selectedEntities[0];

				if (entity == CurrentEntity)
				{
					UpdateSelectedProperties();
					return;
				}
				CurrentEntity = entity;

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



				CreateSelectedProperties(entity);
			}

		}

        #region Single Select
        private void SetSelectedProperties(RSDKv5.SceneEntity entity, Global.Controls.PropertyGrid.PropertyControl.PropertyChangedEventArgs e)
		{
			SetSelectedProperties(entity, e.Property, e.NewValue, e.OldValue);
		}
		private void SetSelectedProperties(RSDKv5.SceneEntity entity, string Property, object NewValue, object OldValue)
		{
			string Category = Property.Split(',')[0];
			string Name = Property.Split(',')[1];


			if (Category == "position") UpdatePosition();
			else if (Category == "object")
			{
				if (Name == "name" && OldValue != NewValue) UpdateName();
				else if (Name == "entitySlot" && OldValue != NewValue) UpdateEntitySlot();
				UpdateEntityProperties();
			}
			else UpdateAttribute();

			void UpdatePosition()
			{
				float fvalue = (float)NewValue;
				if (fvalue < Int16.MinValue || fvalue > Int16.MaxValue)
				{
					// Invalid

					return;
				}
				var pos = entity.Position;
				if (Name == "x")
				{
					pos.X.High = (short)fvalue;
					pos.X.Low = (ushort)(fvalue * 0x10000);
				}
				else if (Name == "y")
				{
					pos.Y.High = (short)fvalue;
					pos.Y.Low = (ushort)(fvalue * 0x10000);
				}
				entity.Position = pos;
				if (entity == CurrentEntity)
					UpdateSelectedProperties();
			}

			void UpdateName()
			{
				var info = RSDKv5.Objects.GetObjectName(new RSDKv5.NameIdentifier(NewValue as string));
				if (info == null)
				{
					MessageBox.Show("Unknown Object", "", MessageBoxButton.OK, MessageBoxImage.Error);
					return;
				}
				var objectsList = ((BindingList<TextBlock>)_BindingSceneObjectsSource).ToList();
				var objects = objectsList.Select(x => x.Tag as RSDKv5.SceneObject).ToList();
				var obj = objects.FirstOrDefault(t => t.Name.Name == NewValue as string);
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
					_BindingSceneObjectsSource.Add(newItem);
				}
			}

			void UpdateEntitySlot()
			{
				ushort newSlot = (ushort)NewValue;
				// Check if slot has been used
				var objectsList = ((BindingList<TextBlock>)_BindingSceneObjectsSource).ToList();
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

			void UpdateAttribute()
			{
				var attribute = entity.GetAttribute(Category);
				switch (attribute.Type)
				{
					case RSDKv5.AttributeTypes.UINT8:
						attribute.ValueUInt8 = (byte)NewValue;
						break;
					case RSDKv5.AttributeTypes.UINT16:
						attribute.ValueUInt16 = (ushort)NewValue;
						break;
					case RSDKv5.AttributeTypes.UINT32:
						attribute.ValueUInt32 = (uint)NewValue;
						break;
					case RSDKv5.AttributeTypes.INT8:
						attribute.ValueInt8 = (sbyte)NewValue;
						break;
					case RSDKv5.AttributeTypes.INT16:
						attribute.ValueInt16 = (short)NewValue;
						break;
					case RSDKv5.AttributeTypes.INT32:
						attribute.ValueInt32 = (int)NewValue;
						break;
					case RSDKv5.AttributeTypes.ENUM:
						attribute.ValueEnum = (int)NewValue;
						break;
					case RSDKv5.AttributeTypes.BOOL:
						attribute.ValueBool = (bool)NewValue;
						break;
					case RSDKv5.AttributeTypes.STRING:
						attribute.ValueString = (string)NewValue;
						break;
					case RSDKv5.AttributeTypes.VECTOR2:
						float fvalue = (float)NewValue;
						if (fvalue < Int16.MinValue || fvalue > Int16.MaxValue) return; // Invalid
						var pos = attribute.ValueVector2;
						if (Name == "x")
						{
							pos.X.High = (short)fvalue;
							pos.X.Low = (ushort)(fvalue * 0x10000);
						}
						else if (Name == "y")
						{
							pos.Y.High = (short)fvalue;
							pos.Y.Low = (ushort)(fvalue * 0x10000);
						}
						attribute.ValueVector2 = pos;
						if (entity == CurrentEntity)
							UpdateSelectedProperties();
						break;
					case RSDKv5.AttributeTypes.COLOR:
						System.Drawing.Color c = (System.Drawing.Color)NewValue;
						attribute.ValueColor = new RSDKv5.Color(c.R, c.G, c.B, c.A);
						break;
				}
				UpdatePropertiesGrid(new List<RSDKv5.SceneEntity>() { entity });
			}

			void UpdateEntityProperties()
			{
				if (entity == CurrentEntity)
				{
					UpdateSelectedProperties();
				}
				else
				{
					CurrentEntity = null;
					UpdatePropertiesGrid(new List<RSDKv5.SceneEntity>() { entity });
				}
				UpdateEntitiesList();
			}
		}
		public void CreateSelectedProperties(SceneEntity entity)
		{
			PropertyControl.PropertyGridObject objProperties = new PropertyControl.PropertyGridObject();

			objProperties.AddProperty("object", "name", "string", typeof(string), entity.Object.Name.ToString());
			objProperties.AddProperty("object", "entitySlot", "ushort", typeof(ushort), entity.SlotID);

			objProperties.AddProperty("position", "x", "float", typeof(float), entity.Position.X.High + ((float)entity.Position.X.Low / 0x10000));
			objProperties.AddProperty("position", "y", "float", typeof(float), entity.Position.Y.High + ((float)entity.Position.Y.Low / 0x10000));


			foreach (var attribute in entity.Object.Attributes)
			{
				string attribute_name = attribute.Name.ToString();
				var attribute_value = CurrentEntity.GetAttribute(attribute_name);
				switch (attribute.Type)
				{
					case RSDKv5.AttributeTypes.UINT8:
						objProperties.AddProperty(attribute_name, "uint8", "byte", typeof(byte), attribute_value.ValueUInt8);
						break;
					case RSDKv5.AttributeTypes.UINT16:
						objProperties.AddProperty(attribute_name, "uint16", "ushort", typeof(ushort), attribute_value.ValueUInt16);
						break;
					case RSDKv5.AttributeTypes.UINT32:
						objProperties.AddProperty(attribute_name, "uint32", "uint", typeof(uint), attribute_value.ValueUInt32);
						break;
					case RSDKv5.AttributeTypes.INT8:
						objProperties.AddProperty(attribute_name, "int8", "sbyte", typeof(sbyte), attribute_value.ValueInt8);
						break;
					case RSDKv5.AttributeTypes.INT16:
						objProperties.AddProperty(attribute_name, "int16", "short", typeof(short), attribute_value.ValueInt16);
						break;
					case RSDKv5.AttributeTypes.INT32:
						objProperties.AddProperty(attribute_name, "int32", "int", typeof(int), attribute_value.ValueInt32);
						break;
					case RSDKv5.AttributeTypes.ENUM:
						objProperties.AddProperty(attribute_name, "enum (var)", "uint", typeof(uint), attribute_value.ValueEnum);
						break;
					case RSDKv5.AttributeTypes.BOOL:
						objProperties.AddProperty(attribute_name, "bool", "bool", typeof(bool), attribute_value.ValueBool);
						break;
					case RSDKv5.AttributeTypes.STRING:
						objProperties.AddProperty(attribute_name, "string", "string", typeof(string), attribute_value.ValueString);
						break;
					case RSDKv5.AttributeTypes.VECTOR2:
						objProperties.AddProperty(attribute_name, "x", "float", typeof(float), attribute_value.ValueVector2.X.High + ((float)attribute_value.ValueVector2.X.Low / 0x10000));
						objProperties.AddProperty(attribute_name, "y", "float", typeof(float), attribute_value.ValueVector2.Y.High + ((float)attribute_value.ValueVector2.Y.Low / 0x10000));
						break;
					case RSDKv5.AttributeTypes.COLOR:
						var color = attribute_value.ValueColor;
						objProperties.AddProperty(attribute_name, "color", "color", typeof(System.Drawing.Color), System.Drawing.Color.FromArgb(255 /* color.A */, color.R, color.G, color.B));
						break;
				}
			}

			PropertiesGrid.SelectedObject = objProperties;
		}
		public void UpdateSingleSelectedProperties()
		{
			object selectedObject = PropertiesGrid.SelectedObject;
			if (selectedObject is PropertyControl.PropertyGridObject obj)
			{
				obj.UpdateProperty("position", "x", CurrentEntity.Position.X.High + ((float)CurrentEntity.Position.X.Low / 0x10000));
				obj.UpdateProperty("position", "y", CurrentEntity.Position.Y.High + ((float)CurrentEntity.Position.Y.Low / 0x10000));
				foreach (var attribute in CurrentEntity.Object.Attributes)
				{
					string attribute_name = attribute.Name.ToString();
					var attribute_value = CurrentEntity.GetAttribute(attribute_name);
					switch (attribute.Type)
					{
						case RSDKv5.AttributeTypes.UINT8:
							obj.UpdateProperty(attribute_name, "uint8", attribute_value.ValueUInt8);
							break;
						case RSDKv5.AttributeTypes.UINT16:
							obj.UpdateProperty(attribute_name, "uint16", attribute_value.ValueUInt16);
							break;
						case RSDKv5.AttributeTypes.UINT32:
							obj.UpdateProperty(attribute_name, "uint32", attribute_value.ValueUInt32);
							break;
						case RSDKv5.AttributeTypes.INT8:
							obj.UpdateProperty(attribute_name, "int8", attribute_value.ValueInt8);
							break;
						case RSDKv5.AttributeTypes.INT16:
							obj.UpdateProperty(attribute_name, "int16", attribute_value.ValueInt16);
							break;
						case RSDKv5.AttributeTypes.INT32:
							obj.UpdateProperty(attribute_name, "int32", attribute_value.ValueInt32);
							break;
						case RSDKv5.AttributeTypes.ENUM:
							obj.UpdateProperty(attribute_name, "enum (var)", attribute_value.ValueEnum);
							break;
						case RSDKv5.AttributeTypes.BOOL:
							obj.UpdateProperty(attribute_name, "bool", attribute_value.ValueBool);
							break;
						case RSDKv5.AttributeTypes.STRING:
							obj.UpdateProperty(attribute_name, "string", attribute_value.ValueString);
							break;
						case RSDKv5.AttributeTypes.VECTOR2:
							obj.UpdateProperty(attribute_name, "x", attribute_value.ValueVector2.X.High + ((float)attribute_value.ValueVector2.X.Low / 0x10000));
							obj.UpdateProperty(attribute_name, "y", attribute_value.ValueVector2.Y.High + ((float)attribute_value.ValueVector2.Y.Low / 0x10000));
							break;
						case RSDKv5.AttributeTypes.COLOR:
							var color = attribute_value.ValueColor;
							obj.UpdateProperty(attribute_name, "color", System.Drawing.Color.FromArgb(255 /* color.A */, color.R, color.G, color.B));
							break;
					}
				}
				NeedRefresh = true;

			}
		}
		#endregion

		#region Multi Select
		public void UpdateMultiSelectedProperties()
		{
			object selectedObject = PropertiesGrid.SelectedObject;
			if (selectedObject is PropertyControl.PropertyGridObject obj)
			{
				foreach (var attribute in GetMultiAttributes(SelectedEntities))
				{
					string attribute_name = attribute.Key.Name.ToString();
					var attribute_value = attribute.Value;
					switch (attribute.Key.Type)
					{
						case RSDKv5.AttributeTypes.UINT8:
							obj.UpdateProperty(attribute_name, "uint8", attribute_value.ValueUInt8);
							break;
						case RSDKv5.AttributeTypes.UINT16:
							obj.UpdateProperty(attribute_name, "uint16", attribute_value.ValueUInt16);
							break;
						case RSDKv5.AttributeTypes.UINT32:
							obj.UpdateProperty(attribute_name, "uint32", attribute_value.ValueUInt32);
							break;
						case RSDKv5.AttributeTypes.INT8:
							obj.UpdateProperty(attribute_name, "int8", attribute_value.ValueInt8);
							break;
						case RSDKv5.AttributeTypes.INT16:
							obj.UpdateProperty(attribute_name, "int16", attribute_value.ValueInt16);
							break;
						case RSDKv5.AttributeTypes.INT32:
							obj.UpdateProperty(attribute_name, "int32", attribute_value.ValueInt32);
							break;
						case RSDKv5.AttributeTypes.ENUM:
							obj.UpdateProperty(attribute_name, "enum (var)", attribute_value.ValueEnum);
							break;
						case RSDKv5.AttributeTypes.BOOL:
							obj.UpdateProperty(attribute_name, "bool", attribute_value.ValueBool);
							break;
						case RSDKv5.AttributeTypes.STRING:
							obj.UpdateProperty(attribute_name, "string", attribute_value.ValueString);
							break;
						case RSDKv5.AttributeTypes.VECTOR2:
							obj.UpdateProperty(attribute_name, "x", attribute_value.ValueVector2.X.High + ((float)attribute_value.ValueVector2.X.Low / 0x10000));
							obj.UpdateProperty(attribute_name, "y", attribute_value.ValueVector2.Y.High + ((float)attribute_value.ValueVector2.Y.Low / 0x10000));
							break;
						case RSDKv5.AttributeTypes.COLOR:
							var color = attribute_value.ValueColor;
							obj.UpdateProperty(attribute_name, "color", System.Drawing.Color.FromArgb(255 /* color.A */, color.R, color.G, color.B));
							break;
					}
				}
				NeedRefresh = true;

			}
		}
		private void SetMultiSelectedProperties(List<RSDKv5.SceneEntity> entities, Global.Controls.PropertyGrid.PropertyControl.PropertyChangedEventArgs e)
		{
			var ActionList = SetMultiSelectedProperties(entities, e.Property, e.NewValue, e.OldValue);
			AddAction?.Invoke(new Actions.ActionEntityMultiplePropertyChange(SelectedEntities, e.Property, ActionList));
		}
		private List<Actions.ActionEntityPropertyChange> SetMultiSelectedProperties(List<RSDKv5.SceneEntity> entities, string Property, object NewValue, object OldValue)
		{
			string Category = Property.Split(',')[0];
			string Name = Property.Split(',')[1];

			List<Actions.ActionEntityPropertyChange> ActionList = new List<Actions.ActionEntityPropertyChange>();

			UpdateAttribute();

			return ActionList;

			void UpdateAttribute()
			{
				foreach (var entity in entities)
				{
					if (entity.AttributeExists(Category, GetAttributeTypeFromName(Name)))
					{
						var attribute = entity.GetAttribute(Category);
						switch (attribute.Type)
						{
							case RSDKv5.AttributeTypes.UINT8:
								OldValue = attribute.ValueUInt8;
								attribute.ValueUInt8 = (byte)NewValue;
								break;
							case RSDKv5.AttributeTypes.UINT16:
								OldValue = attribute.ValueUInt16;
								attribute.ValueUInt16 = (ushort)NewValue;
								break;
							case RSDKv5.AttributeTypes.UINT32:
								OldValue = attribute.ValueUInt32;
								attribute.ValueUInt32 = (uint)NewValue;
								break;
							case RSDKv5.AttributeTypes.INT8:
								OldValue = attribute.ValueInt8;
								attribute.ValueInt8 = (sbyte)NewValue;
								break;
							case RSDKv5.AttributeTypes.INT16:
								OldValue = attribute.ValueInt16;
								attribute.ValueInt16 = (short)NewValue;
								break;
							case RSDKv5.AttributeTypes.INT32:
								OldValue = attribute.ValueInt32;
								attribute.ValueInt32 = (int)NewValue;
								break;
							case RSDKv5.AttributeTypes.ENUM:
								OldValue = attribute.ValueEnum;
								attribute.ValueEnum = (int)NewValue;
								break;
							case RSDKv5.AttributeTypes.BOOL:
								OldValue = attribute.ValueBool;
								attribute.ValueBool = (bool)NewValue;
								break;
							case RSDKv5.AttributeTypes.STRING:
								OldValue = attribute.ValueString;
								attribute.ValueString = (string)NewValue;
								break;
							case RSDKv5.AttributeTypes.VECTOR2:
								float fvalue = (float)NewValue;
								if (fvalue < Int16.MinValue || fvalue > Int16.MaxValue) return; // Invalid
								var pos = attribute.ValueVector2;
								OldValue = pos;
								if (Name == "x")
								{
									pos.X.High = (short)fvalue;
									pos.X.Low = (ushort)(fvalue * 0x10000);
								}
								else if (Name == "y")
								{
									pos.Y.High = (short)fvalue;
									pos.Y.Low = (ushort)(fvalue * 0x10000);
								}
								attribute.ValueVector2 = pos;
								break;
							case RSDKv5.AttributeTypes.COLOR:
								System.Drawing.Color c = (System.Drawing.Color)NewValue;
								attribute.ValueColor = new RSDKv5.Color(c.R, c.G, c.B, c.A);
								break;
						}
						ActionList.Add(new Actions.ActionEntityPropertyChange(entity, Property, OldValue, NewValue, new Action<RSDKv5.SceneEntity, string, object, object>(SetSelectedProperties)));
					}
				}
				UpdatePropertiesGrid(entities);

			}
		}
		public void CreateMultiSelectedProperties(List<RSDKv5.SceneEntity> entities)
		{
			PropertyControl.PropertyGridObject objProperties = new PropertyControl.PropertyGridObject();
			foreach (var attribute in GetMultiAttributes(entities))
			{
				string attribute_name = attribute.Key.Name.ToString();
				var attribute_value = attribute.Value;
				switch (attribute.Key.Type)
				{
					case RSDKv5.AttributeTypes.UINT8:
						objProperties.AddProperty(attribute_name, "uint8", "byte", typeof(byte), attribute_value.ValueUInt8);
						break;
					case RSDKv5.AttributeTypes.UINT16:
						objProperties.AddProperty(attribute_name, "uint16", "ushort", typeof(ushort), attribute_value.ValueUInt16);
						break;
					case RSDKv5.AttributeTypes.UINT32:
						objProperties.AddProperty(attribute_name, "uint32", "uint", typeof(uint), attribute_value.ValueUInt32);
						break;
					case RSDKv5.AttributeTypes.INT8:
						objProperties.AddProperty(attribute_name, "int8", "sbyte", typeof(sbyte), attribute_value.ValueInt8);
						break;
					case RSDKv5.AttributeTypes.INT16:
						objProperties.AddProperty(attribute_name, "int16", "short", typeof(short), attribute_value.ValueInt16);
						break;
					case RSDKv5.AttributeTypes.INT32:
						objProperties.AddProperty(attribute_name, "int32", "int", typeof(int), attribute_value.ValueInt32);
						break;
					case RSDKv5.AttributeTypes.ENUM:
						objProperties.AddProperty(attribute_name, "enum (var)", "uint", typeof(uint), attribute_value.ValueEnum);
						break;
					case RSDKv5.AttributeTypes.BOOL:
						objProperties.AddProperty(attribute_name, "bool", "bool", typeof(bool), attribute_value.ValueBool);
						break;
					case RSDKv5.AttributeTypes.STRING:
						objProperties.AddProperty(attribute_name, "string", "string", typeof(string), attribute_value.ValueString);
						break;
					case RSDKv5.AttributeTypes.VECTOR2:
						objProperties.AddProperty(attribute_name, "x", "float", typeof(float), attribute_value.ValueVector2.X.High + ((float)attribute_value.ValueVector2.X.Low / 0x10000));
						objProperties.AddProperty(attribute_name, "y", "float", typeof(float), attribute_value.ValueVector2.Y.High + ((float)attribute_value.ValueVector2.Y.Low / 0x10000));
						break;
					case RSDKv5.AttributeTypes.COLOR:
						var color = attribute_value.ValueColor;
						objProperties.AddProperty(attribute_name, "color", "color", typeof(System.Drawing.Color), System.Drawing.Color.FromArgb(255 /* color.A */, color.R, color.G, color.B));
						break;
				}
			}

			PropertiesGrid.SelectedObject = objProperties;
		}
		public Dictionary<RSDKv5.AttributeInfo, RSDKv5.AttributeValue> GetMultiAttributes(List<RSDKv5.SceneEntity> entities)
		{
			Dictionary<RSDKv5.AttributeInfo, RSDKv5.AttributeValue> MultiAttributes = new Dictionary<RSDKv5.AttributeInfo, RSDKv5.AttributeValue>();
			foreach (var entity in entities)
			{
				foreach (var attribute in entity.Object.Attributes)
				{
					if (MultiAttributes.ToList().Exists(x => x.Key.Name == attribute.Name && x.Key.Type == attribute.Type))
					{
						AttributeValue child = entity.GetAttribute(attribute.Name);
						AttributeValue parent = MultiAttributes[attribute];

						if (!CompareValueRaw(child, parent)) MultiAttributes[attribute] = new AttributeValue(attribute.Type);
					}
					else MultiAttributes.Add(attribute, entity.GetAttribute(attribute.Name));
				}

			}
			return MultiAttributes;
		}

		#endregion

		private void PropertiesGrid_PropertyValueChanged(object sender, Global.Controls.PropertyGrid.PropertyControl.PropertyChangedEventArgs e)
		{
			if (MultipleObjectsSelected)
			{
				SetMultiSelectedProperties(SelectedEntities, e);
			}
			else
			{
				AddAction?.Invoke(new Actions.ActionEntityPropertyChange(CurrentEntity, e.Property, e.OldValue, e.NewValue, new Action<RSDKv5.SceneEntity, string, object, object>(SetSelectedProperties)));
				SetSelectedProperties(CurrentEntity, e);
			}

		}
		public void UpdateSelectedProperties()
		{
			if (MultipleObjectsSelected) UpdateMultiSelectedProperties();
			else if (CurrentEntity != null) UpdateSingleSelectedProperties();
		}

		#endregion

		#region UI Refresh

		public void UpdatePropertyGridTheme(bool ForceRefresh = false)
		{
			/*
			if (App.Skin == Skin.Dark)
			{
				this.PropertiesGrid.BackColor = Methods.Internal.Theming.darkTheme0;
				this.PropertiesGrid.CategoryForeColor = Methods.Internal.Theming.darkTheme3;
				this.PropertiesGrid.CommandsBorderColor = System.Drawing.Color.DarkGray;
				this.PropertiesGrid.CommandsForeColor = System.Drawing.Color.Black;
				this.PropertiesGrid.HelpBackColor = System.Drawing.Color.White;
				this.PropertiesGrid.HelpBorderColor = System.Drawing.Color.DarkGray;
				this.PropertiesGrid.HelpForeColor = System.Drawing.Color.Black;
				this.PropertiesGrid.LineColor = Methods.Internal.Theming.darkTheme5;
				this.PropertiesGrid.SelectedItemWithFocusBackColor = System.Drawing.Color.Blue;
				this.PropertiesGrid.SelectedItemWithFocusForeColor = System.Drawing.Color.White;
				this.PropertiesGrid.ViewBackColor = Methods.Internal.Theming.darkTheme0;
				this.PropertiesGrid.ViewBorderColor = Methods.Internal.Theming.darkTheme1;
				this.PropertiesGrid.ViewForeColor = Methods.Internal.Theming.darkTheme3;
			}
			else
			{
				this.PropertiesGrid.BackColor = System.Drawing.Color.White;
				this.PropertiesGrid.CategoryForeColor = System.Drawing.Color.Black;
				this.PropertiesGrid.CommandsBorderColor = System.Drawing.Color.DarkGray;
				this.PropertiesGrid.CommandsForeColor = System.Drawing.Color.Black;
				this.PropertiesGrid.HelpBackColor = System.Drawing.Color.White;
				this.PropertiesGrid.HelpBorderColor = System.Drawing.Color.DarkGray;
				this.PropertiesGrid.HelpForeColor = System.Drawing.Color.Black;
				this.PropertiesGrid.LineColor = System.Drawing.Color.Silver;
				this.PropertiesGrid.SelectedItemWithFocusBackColor = System.Drawing.Color.DodgerBlue;
				this.PropertiesGrid.SelectedItemWithFocusForeColor = System.Drawing.Color.White;
				this.PropertiesGrid.ViewBackColor = System.Drawing.Color.White;
				this.PropertiesGrid.ViewBorderColor = System.Drawing.Color.DarkGray;
				this.PropertiesGrid.ViewForeColor = System.Drawing.Color.Black;
			}
			*/
			if (ForceRefresh) this.PropertiesGrid.Update();
		}
		public void UpdateFilterNames(bool startup = false)
		{
			if (startup)
			{
				maniaFilter.Foreground = Methods.Internal.Theming.GetColorBrush(2);
				encoreFilter.Foreground = Methods.Internal.Theming.GetColorBrush(4);
				pinballFilter.Foreground = Methods.Internal.Theming.GetColorBrush(255);
				bothFilter.Foreground = Methods.Internal.Theming.GetColorBrush(1);
				otherFilter.Foreground = Methods.Internal.Theming.GetColorBrush(0);
			}
			if (Properties.Settings.MySettings.UseBitOperators)
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
		public void Dispose()
		{
			if (this.PropertiesGrid != null)
			{
				this.PropertiesGrid.Dispose();
				this.PropertiesGrid = null;
			}
		}
		public void PropertiesRefresh()
		{
			PropertiesGrid.Refresh();
			NeedRefresh = false;
		}

		#endregion

		#region AttributeValues 

		public AttributeTypes GetAttributeTypeFromName(string Name)
		{
			switch(Name)
			{
				case "uint8":
					return AttributeTypes.UINT8;
				case "uint16":
					return AttributeTypes.UINT16;
				case "uint32":
					return AttributeTypes.UINT32;
				case "int8":
					return AttributeTypes.INT8;
				case "int16":
					return AttributeTypes.INT16;
				case "int32":
					return AttributeTypes.INT32;
				case "enum (var)":
					return AttributeTypes.ENUM;
				case "bool":
					return AttributeTypes.BOOL;
				case "string":
					return AttributeTypes.STRING;
				case "x":
					return AttributeTypes.VECTOR2;
				case "y":
					return AttributeTypes.VECTOR2;
				case "color":
					return AttributeTypes.COLOR;
				default:
					return AttributeTypes.ENUM;
			}
		}
		public bool CompareValueRaw(AttributeValue itemA, AttributeValue itemB)
		{
			switch (itemA.Type)
			{
				case AttributeTypes.UINT8:
					return itemA.ValueUInt8 == itemB.ValueUInt8;
				case AttributeTypes.UINT16:
					return itemA.ValueUInt16 == itemB.ValueUInt16;
				case AttributeTypes.UINT32:
					return itemA.ValueUInt32 == itemB.ValueUInt32;
				case AttributeTypes.INT8:
					return itemA.ValueInt8 == itemB.ValueInt8;
				case AttributeTypes.INT16:
					return itemA.ValueInt16 == itemB.ValueInt16;
				case AttributeTypes.INT32:
					return itemA.ValueInt32 == itemB.ValueInt32;
				case AttributeTypes.ENUM:
					return itemA.ValueEnum == itemB.ValueEnum;
				case AttributeTypes.BOOL:
					return itemA.ValueBool == itemB.ValueBool;
				case AttributeTypes.STRING:
					return itemA.ValueString == itemB.ValueString;
				case AttributeTypes.VECTOR2:
					return itemA.ValueVector2.Equals(itemB.ValueVector2);
				case AttributeTypes.VECTOR3:
					return itemA.ValueVector3.Equals(itemB.ValueVector3);
				case AttributeTypes.COLOR:
					return itemA.ValueColor.Equals(itemB.ValueColor);
				default:
					return false;
			}

		}

		#endregion

		#region Old
		public void UpdateEntitiesList(bool FirstLoad = false)
		{

			//This if statement Triggers when the toolbar opens for the first time
			if (FirstLoad) _Entities = Methods.Editor.Solution.Entities.Entities.Select(x => x.Entity).ToList();
            SceneEntitiesList.Items.Clear();

            int count = (2301 > _Entities.Count() ? _Entities.Count() : 2031);

            for (int i = 0; i < count; i++)
            {
                var entity = _Entities[i];
                if (entity != null)
                {
                    Visibility VisibilityStatus = GetObjectListItemVisiblity(entity.Object.Name.Name, entity.SlotID);
                    if (ObjectList[i] == null)
                    {
						ObjectList[i] = new System.Windows.Controls.Button()
                        {
                            Content = string.Format("{0} - {1}", entity.Object.Name.Name, entity.SlotID),
                            Foreground = Methods.Internal.Theming.GetColorBrush(entity),
                            Tag = entity.SlotID.ToString(),
                            Visibility = VisibilityStatus
                        };
                        ObjectList[i].Click += EntitiesListEntryClicked;
                    }
                    else
                    {
                        ObjectList[i].Content = String.Format("{0} - {1}", entity.Object.Name.Name, entity.SlotID);
						ObjectList[i].Foreground = Methods.Internal.Theming.GetColorBrush(entity);
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
                            Content = string.Format("{0} - {1}", "UNUSED", i),
                            Foreground = Methods.Internal.Theming.GetColorBrush(256),
                            Height = 0,
                            Visibility = Visibility.Collapsed,
                            Tag = "NULL"

                        };
                        ObjectList[i].Click += EntitiesListEntryClicked;
                    }
                    else
                    {
                        ObjectList[i].Content = String.Format("{0} - {1}", "UNUSED", i);
						ObjectList[i].Foreground = Methods.Internal.Theming.GetColorBrush(256);
                        ObjectList[i].Height = 0;
                        ObjectList[i].Visibility = Visibility.Collapsed;
                        ObjectList[i].Tag = "NULL";
                    }

                }
                if (ObjectList[i].Visibility != Visibility.Collapsed) SceneEntitiesList.Items.Add(ObjectList[i]);
            }

            if (CurrentEntity != null) GoToObjectButton.IsEnabled = true;
			else GoToObjectButton.IsEnabled = false;
        }
		public Visibility GetObjectListItemVisiblity(string name, ushort slotID)
		{
			if (MultipleObjectsSelected == true)
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
				switch(Properties.Settings.MyDefaults.DefaultFilter[0])
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
						Properties.Settings.MyDefaults.DefaultFilter = "M";
						break;
					case 1:
						Properties.Settings.MyDefaults.DefaultFilter = "E";
						break;
					case 2:
						Properties.Settings.MyDefaults.DefaultFilter = "B";
						break;
					case 3:
						Properties.Settings.MyDefaults.DefaultFilter = "P";
						break;
					case 4:
						Properties.Settings.MyDefaults.DefaultFilter = "O";
						break;
					default:
						Properties.Settings.MyDefaults.DefaultFilter = "M";
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


				_BindingSceneObjectsSource.Clear();
				foreach (var _object in bindingSceneObjectsList)
				{
					TextBlock item = new TextBlock()
					{
						Tag = _object,
						Text = _object.Name.Name
					};
					_BindingSceneObjectsSource.Add(item);
				}

				if (_BindingSceneObjectsSource != null && _BindingSceneObjectsSource.Count > 1)
				{
					cbSpawn.ItemsSource = _BindingSceneObjectsSource;
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
			for (int i = 0; i < _Entities.Count; i++)
			{
				if (_Entities[i].SlotID == (ushort)slotID)
				{
					index = i;
				}
			}
			return index;
		}
        private void UpdateSelectedEntitiesList()
        {
            SelectionViewer.Children.Clear();
            if (Methods.Editor.Solution.Entities.SelectedEntities != null)
            {
                foreach (var entity in Methods.Editor.Solution.Entities.SelectedEntities.OrderBy(x => x.TimeWhenSelected))
                {
                    TextBlock entry = new TextBlock();
                    entry.Text = string.Format("{0} | {1} | ID:{2} | X:{3},Y:{4}", string.Format("{0}", entity.SelectedIndex + 1), entity.Name, entity.SlotID, entity.Entity.Position.X.High, entity.Entity.Position.Y.High);
                    SelectionViewer.Children.Add(entry);
                }
            }

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
                    switch (Properties.Settings.MyDefaults.DefaultFilter[0])
                    {
                        case 'M':
                            Methods.Editor.Solution.Entities.CurrentDefaultFilter = 2;
                            break;
                        case 'E':
                            Methods.Editor.Solution.Entities.CurrentDefaultFilter = 4;
                            break;
                        case 'B':
                            Methods.Editor.Solution.Entities.CurrentDefaultFilter = 1;
                            break;
                        case 'P':
                            Methods.Editor.Solution.Entities.CurrentDefaultFilter = 255;
                            break;
                        default:
                            Methods.Editor.Solution.Entities.CurrentDefaultFilter = 0;
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
			GoToObjectButton.ContextMenu.IsOpen = true;
		}
		private void goToThisEntityToolStripMenuItem_Click(object sender, RoutedEventArgs e)
		{
            if (CurrentEntity != null)
			{
				int x = CurrentEntity.Position.X.High;
				int y = CurrentEntity.Position.Y.High;
				Methods.Editor.EditorActions.GoToPosition(x, y);
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
			if (CurrentEntity != null)
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
        private void EntitiesList_Click(object sender, RoutedEventArgs e)
        {
            if (TabControl.SelectedIndex != 2)
            {
                TabControl.SelectedIndex = 2;
                if (CurrentEntity != null)
                {
                    var currObject = ObjectList.Where(x => x.Tag.ToString() == CurrentEntity.SlotID.ToString()).FirstOrDefault();
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
            Methods.Editor.Solution.Entities.Deselect();
            Methods.Editor.Solution.Entities.Entities.Where(x => x.SlotID.ToString() == button.Tag.ToString()).FirstOrDefault().Selected = true;
            TabControl.SelectedIndex = 0;
            SelectedEntities = Methods.Editor.Solution.Entities.SelectedEntities.Select(x => x.Entity).ToList();
        }
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void ButtonSpinner_Spin(object sender, Xceed.Wpf.Toolkit.SpinEventArgs e)
        {
            if (CurrentEntity != null)
            {
                int index = ObjectList.IndexOf(ObjectList.Where(x => x.Tag.ToString() == CurrentEntity.SlotID.ToString()).FirstOrDefault());

                if (e.Direction == Xceed.Wpf.Toolkit.SpinDirection.Decrease) index--;
                else index++;

                if (ObjectList.Length <= index || index < 0) return;

                if (ObjectList[index] != null)
                {
                    Methods.Editor.Solution.Entities.Deselect();
                    Methods.Editor.Solution.Entities.Entities.Where(x => x.SlotID.ToString() == ObjectList[index].Tag.ToString()).FirstOrDefault().Selected = true;
                    TabControl.SelectedIndex = 0;
                    SelectedEntities = Methods.Editor.Solution.Entities.SelectedEntities.Select(x => x.Entity).ToList();
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
        #endregion
    }
}
