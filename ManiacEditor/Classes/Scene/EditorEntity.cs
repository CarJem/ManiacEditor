using ManiacEditor.Entity_Renders;
using RSDKv5;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ManiacEditor.Enums;

namespace ManiacEditor.Classes.Scene
{
    [Serializable]
    public class EditorEntity : Xe.Tools.Wpf.BaseNotifyPropertyChanged, IDrawable
    {
        #region Definitions

        #region Toolbox Data
        public string ItemContent
        {
            get
            {
                string Name = this.Object.Name.Name;
                string SlotID = this.FilterSlotID.ToString();
                return string.Format("{0} - {1}", Name, SlotID);
            }
        }
        public object Tag
        {
            get
            {
                return this.SlotID.ToString();
            }
        }
        public System.Windows.Media.Brush ItemForeground
        {
            get
            {
                return Methods.Internal.Theming.GetObjectFilterColorBrush(this);
            }
        }
        public System.Windows.Visibility Visibility
        {
            get
            {
                return Classes.Internal.EntitiesToolboxCore.GetObjectListItemVisiblity(this.Name, this.SlotID, FilteredOut);
            }
        }
        public void RefreshToolboxData()
        {
            OnPropertyChanged(nameof(ItemContent));
            OnPropertyChanged(nameof(Tag));
            OnPropertyChanged(nameof(ItemForeground));
            OnPropertyChanged(nameof(Visibility));
        }

        #endregion

        #region Render Data

        public bool IsVisible { get; set; }
        public EntityRenderer CurrentRender { get; set; }
        public bool DoesRenderHaveErrors { get; set; } = false;
        public bool DoesLinkedRenderHaveErrors { get; set; } = false;
        public LinkedRenderer CurrentLinkedRender { get; set; }

        #endregion

        #region Selection Data
        private bool IsSelected { get; set; } = false;
        public bool Selected
        {
            get
            {
                return IsSelected;
            }
            set
            {
                if (value != IsSelected)
                {
                    OnUpdate?.Invoke(null, null);
                    if (value == true)
                    {
                        if (_entity.Object.Name.Name == "Spline" && IsInternalObject) ManiacEditor.Methods.Internal.UserInterface.EditorToolbars.ChangeSplineSelectedID(_entity.attributesMap["SplineID"].ValueInt32);
                        IsSelected = value;
                        TimeWhenSelected = DateTimeOffset.Now;
                    }
                    else
                    {
                        IsSelected = value;
                        TimeWhenSelected = null;
                        SelectedIndex = -1;
                    }

                    Methods.Solution.CurrentSolution.Entities.UpdateSelectedIndexForEntities();
                }
                else if (value == true) TimeWhenSelected = DateTimeOffset.Now;
            }
        }
        public bool InTempSelection { get; set; } = false;
        public bool TempSelected { get; set; } = false;
        public int SelectedIndex { get; set; } = -1;
        #endregion

        #region Original Object
        public DictionaryWithDefault<string, AttributeValue> attributesMap
        {
            get
            {
                return _entity.attributesMap;
            }
            set
            {
                _entity.attributesMap = value;
            }
        }
        public List<AttributeValue> Attributes
        {
            get
            {
                return _entity.Attributes;
            }
            set
            {
                _entity.Attributes = value;
            }
        }
        public SceneObject Object
        {
            get
            {
                return new SceneObject(_entity.Object.Name, _entity.Object.Attributes);
            }
            set
            {
                _entity.Object = new SceneObject(value.Name, value.Attributes);
            }
        }
        public SceneEntity Entity
        {
            get
            {
                return _entity;
            }
        }

        private SceneEntity _entity;

        #endregion

        #region Object Data
        public ushort SlotID
        {
            get
            {
                return _entity.SlotID;
            }
            set
            {
                _entity.SlotID = value;
                RefreshToolboxData();
            }
        }
        private int _FilterSlotID = 0;
        public int FilterSlotID
        {
            get
            {
                return _FilterSlotID;
            }
            set
            {
                _FilterSlotID = value;
                RefreshToolboxData();
            }
        }
        public Position Position
        {
            get
            {
                return _entity.Position;
            }
            set
            {
                _entity.Position = value;
                RefreshToolboxData();
            }
        }
        public int PositionX
        {
            get
            {
                return _entity.Position.X.High;
            }
            set
            {
                _entity.Position.X.High = (short)value;
                RefreshToolboxData();
            }
        }
        public int PositionY
        {
            get
            {
                return _entity.Position.Y.High;
            }
            set
            {
                _entity.Position.Y.High = (short)value;
                RefreshToolboxData();
            }
        }
        public string Name
        {
            get
            {
                return _entity.Object.Name.ToString();
            }
        }
        public void PrepareForExternalCopy()
        {
            _entity.PrepareForExternalCopy();
        }
        public bool IsExternal
        {
            get
            {
                return _entity.IsExternal();
            }
        }
        private bool _FilteredOut { get; set; }
        public bool FilteredOut
        {
            get
            {
                return _FilteredOut;
            }
            set
            {
                _FilteredOut = value;
                OnPropertyChanged(nameof(FilteredOut));
                RefreshToolboxData();
            }
        }
        public bool ManuallyFilteredOut
        {
            get
            {
                return (Methods.Solution.SolutionState.Main.ObjectFilter != "" && !Object.Name.Name.Contains(Methods.Solution.SolutionState.Main.ObjectFilter));
            }
        }
        public bool RenderNotFound { get; set; } = false;
        public bool IsInternalObject { get; set; } = false;
        public DateTimeOffset? TimeWhenSelected { get; set; } = null;

        #endregion

        #region Animation + Child Stuff
        public DateTime LastFrametime { get; set; }
        public int CurrentAnimIndex { get; set; } = 0;
        #endregion

        #region Undo + Redo

        public Action<ManiacEditor.Actions.IAction> ValueChanged = new Action<ManiacEditor.Actions.IAction>(x =>
        {
            Actions.UndoRedoModel.UndoStack.Push(x);
            Actions.UndoRedoModel.RedoStack.Clear();           
        });

        #endregion

        #endregion

        #region Events

        public static event EventHandler OnUpdate;

        #endregion

        #region Init

        public EditorEntity(SceneEntity entity)
        {
            this._entity = entity;
            LastFrametime = DateTime.Now;
        }
        public EditorEntity(SceneEntity entity, bool IsInternal)
        {
            IsInternalObject = IsInternal;
            this._entity = entity;
        }

        #endregion

        #region Original Methods


        public bool AttributeExists(string name, AttributeTypes type)
        {
            return _entity.AttributeExists(name, type);
        }
        public AttributeValue GetAttribute(string name)
        {
            return _entity.GetAttribute(name);
        }
        public AttributeValue GetAttribute(NameIdentifier name)
        {
            return _entity.GetAttribute(name);
        }

        #endregion

        #region Methods
        public static int GetSlotIndex(EditorEntity e)
        {
            return Methods.Solution.CurrentSolution.Entities.Entities.IndexOf(e);
        }
        public bool ContainsPoint(Point point)
        {
            if (FilteredOut) return false;

            if (ManuallyFilteredOut) return false;

            return GetDimensions().Contains(point);
        }
        public bool IsInArea(Rectangle area)
        {
            if (FilteredOut) return false;

            if (ManuallyFilteredOut) return false;

            return GetDimensions().IntersectsWith(area);
        }
        public Rectangle GetDimensions(bool GridAlignment = false)
        {

            if (GridAlignment)
            {
                if (Methods.Solution.SolutionState.Main.UseMagnetMode)
                {
                    int x = Methods.Solution.SolutionState.Main.MagnetSize * (PositionX / Methods.Solution.SolutionState.Main.MagnetSize);
                    int y = Methods.Solution.SolutionState.Main.MagnetSize * (PositionY / Methods.Solution.SolutionState.Main.MagnetSize);
                    return new Rectangle(x, y, Methods.Solution.SolutionState.Main.MagnetSize, Methods.Solution.SolutionState.Main.MagnetSize);
                }
                else
                {
                    return new Rectangle(PositionX, PositionY, Methods.Solution.SolutionConstants.ENTITY_NAME_BOX_WIDTH, Methods.Solution.SolutionConstants.ENTITY_NAME_BOX_HEIGHT);
                }

            }
            else return new Rectangle(PositionX, PositionY, Methods.Solution.SolutionConstants.ENTITY_NAME_BOX_WIDTH, Methods.Solution.SolutionConstants.ENTITY_NAME_BOX_HEIGHT);
        }

        #endregion

        #region Filter Management

        public bool SetFilter()
        {
            int filter = (HasFilter() ? _entity.GetAttribute("filter").ValueUInt8 : 0);

            /*
                * 1 or 5 = Both
                * 2 = Mania
                * 4 = Encore
				* 255 = Pinball
                * 
            */
            FilteredOut =
                ((filter == 1 || filter == 5) && !ManiacEditor.Properties.Settings.MyDefaults.ShowBothEntities) ||
                (filter == 2 && !ManiacEditor.Properties.Settings.MyDefaults.ShowManiaEntities) ||
                (filter == 4 && !ManiacEditor.Properties.Settings.MyDefaults.ShowEncoreEntities) ||
                (filter == 255 && !ManiacEditor.Properties.Settings.MyDefaults.ShowPinballEntities) ||
                (filter == 0 && !ManiacEditor.Properties.Settings.MyDefaults.ShowFilterlessEntities) ||
                ((filter < 1 || filter == 3 || filter > 5 && filter != 255) && !ManiacEditor.Properties.Settings.MyDefaults.ShowOtherEntities);

            return FilteredOut;
        }
        public bool HasFilter()
        {
            return _entity.attributesMap.ContainsKey("filter") && _entity.attributesMap["filter"].Type == AttributeTypes.UINT8;
        }
        public bool HasSpecificFilter(uint input, bool checkHigher = false)
        {
            if (_entity.attributesMap.ContainsKey("filter") && _entity.attributesMap["filter"].Type == AttributeTypes.UINT8)
            {
                if (_entity.attributesMap["filter"].ValueUInt8 == input && checkHigher == false)
                {
                    return true;
                }
                else if (_entity.attributesMap["filter"].ValueUInt8 >= input && checkHigher)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public bool HasFilterOther()
        {
            if (_entity.attributesMap.ContainsKey("filter") && _entity.attributesMap["filter"].Type == AttributeTypes.UINT8)
            {
                int filter = _entity.attributesMap["filter"].ValueUInt8;
                if (filter < 1 || filter == 3 || filter > 5)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region Drawing
        public void Draw(Graphics g)
        {

        }
        public virtual void Draw(DevicePanel d)
        {
            if (FilteredOut) return;
            if (ManuallyFilteredOut) return;
            if (Methods.Drawing.ObjectDrawing.CanDrawLinked(_entity.Object.Name.Name)) Methods.Drawing.ObjectDrawing.DrawLinked(d, this);
            else Methods.Drawing.ObjectDrawing.DrawNormal(d, this);
        }
        public virtual void DrawBase(DevicePanel d)
        {
            Methods.Drawing.ObjectDrawing.DrawNormal(d, this);
        }

        public void Dispose()
        {

        }
        #endregion

        #region Manipulation
        public void Move(Point diff, bool relative = true)
        {
            if (relative)
            {
                PositionX += (short)diff.X;
                PositionY += (short)diff.Y;
            }
            else
            {
                PositionX = (short)diff.X;
                PositionY = (short)diff.Y;
            }

            if (Methods.Runtime.GameHandler.GameRunning && ManiacEditor.Properties.Settings.MyGameOptions.RealTimeObjectMovementMode && !IsInternalObject)
            {

                int ObjectStart = Methods.Runtime.GameHandler.ObjectStart[Methods.Runtime.GameHandler.GameVersion.IndexOf(Methods.Runtime.GameHandler.SelectedGameVersion)];
                int ObjectSize = Methods.Runtime.GameHandler.ObjectSize[Methods.Runtime.GameHandler.GameVersion.IndexOf(Methods.Runtime.GameHandler.SelectedGameVersion)];

                int ObjectAddress = ObjectStart + (ObjectSize * _entity.SlotID);
                Methods.Runtime.GameHandler.GameMemory.WriteInt16(ObjectAddress + 2, (short)PositionX);
                Methods.Runtime.GameHandler.GameMemory.WriteInt16(ObjectAddress + 6, (short)PositionY);
            }


        }
        public SharpDX.Vector2 GetRotationFromAttributes(ref bool fliph, ref bool flipv, ref bool rotate)
        {
            AttributeValue attribute = null;
            var attributes = _entity.attributesMap;
            int dir = 0;
            var offset = new SharpDX.Vector2();
            if (attributes.ContainsKey("orientation"))
            {
                attribute = attributes["orientation"];
                switch (attribute.Type)
                {
                    case AttributeTypes.UINT8:
                        dir = attribute.ValueUInt8;
                        break;
                    case AttributeTypes.INT8:
                        dir = attribute.ValueInt8;
                        break;
                    case AttributeTypes.ENUM:
                        dir = (int)attribute.ValueEnum;
                        break;
                }
                if (dir == 0) // Up
                {
                }
                else if (dir == 1) // Down
                {
                    fliph = true;
                    offset.X = 1;
                    flipv = true;
                    offset.Y = 1;
                }
                else if (dir == 2) // Right
                {
                    flipv = true;
                    rotate = true;
                    offset.Y = 1;
                    //animID = 1;
                }
                else if (dir == 3) // Left
                {
                    flipv = true;
                    rotate = true;
                    offset.Y = 1;
                    //animID = 1;
                }
            }
            if (attributes.ContainsKey("direction") && dir == 0)
            {
                attribute = attributes["direction"];
                switch (attribute.Type)
                {
                    case AttributeTypes.UINT8:
                        dir = attribute.ValueUInt8;
                        break;
                    case AttributeTypes.INT8:
                        dir = attribute.ValueInt8;
                        break;
                    case AttributeTypes.ENUM:
                        dir = (int)attribute.ValueEnum;
                        break;
                }
                if (dir == 0) // Right
                {
                }
                else if (dir == 1) // left
                {
                    fliph = true;
                    offset.X = 0;
                    //rotate = true;
                }
                else if (dir == 2) // Up
                {
                    flipv = true;
                }
                else if (dir == 3) // Down
                {
                    flipv = true;
                    //offset.Y = 1;
                }
            }
            return offset;
        }
        private void SetObjectProperties(SceneEntity entity, string name, object oldValue, object newValue, AttributeTypes Type)
        {
            switch (Type)
            {
                case AttributeTypes.UINT8:
                    entity.attributesMap[name].ValueUInt8 = (byte)newValue;
                    break;
                case AttributeTypes.UINT16:
                    entity.attributesMap[name].ValueUInt16 = (ushort)newValue;
                    break;
                case AttributeTypes.UINT32:
                    entity.attributesMap[name].ValueUInt32 = (uint)newValue;
                    break;
                case AttributeTypes.INT8:
                    entity.attributesMap[name].ValueInt8 = (sbyte)newValue;
                    break;
                case AttributeTypes.INT16:
                    entity.attributesMap[name].ValueInt16 = (short)newValue;
                    break;
                case AttributeTypes.INT32:
                    entity.attributesMap[name].ValueInt32 = (int)newValue;
                    break;
                case AttributeTypes.ENUM:
                    entity.attributesMap[name].ValueEnum = (int)newValue;
                    break;
                case AttributeTypes.BOOL:
                    entity.attributesMap[name].ValueBool = (bool)newValue;
                    break;
                case AttributeTypes.COLOR:
                    entity.attributesMap[name].ValueColor = (RSDKv5.Color)newValue;
                    break;
                case AttributeTypes.VECTOR2:
                    entity.attributesMap[name].ValueVector2 = (Position)newValue;
                    break;
                case AttributeTypes.VECTOR3:
                    entity.attributesMap[name].ValueVector3 = (Position)newValue;
                    break;
                case AttributeTypes.STRING:
                    entity.attributesMap[name].ValueString = (string)newValue;
                    break;

            }
        }
        internal void Flip(FlipDirection flipDirection)
        {
            if (_entity.AttributeExists("flipFlag", AttributeTypes.ENUM))
            {
                int oldValue = _entity.attributesMap["flipFlag"].ValueEnum;
                int newValue = oldValue;
                if (flipDirection == FlipDirection.Horizontal) newValue ^= 0x01;
                else newValue ^= 0x02;
                InvokeAction("flipFlag", oldValue, newValue, AttributeTypes.ENUM);
            }

            if (_entity.AttributeExists("direction", AttributeTypes.UINT8))
            {
                byte oldValue = _entity.attributesMap["direction"].ValueUInt8;
                byte newValue = oldValue;
                if (flipDirection == FlipDirection.Horizontal) newValue ^= 0x01;
                else newValue ^= 0x02;
                InvokeAction("direction", oldValue, newValue, AttributeTypes.UINT8);
            }

            void InvokeAction(string tag, object oldValue, object newValue, AttributeTypes type)
            {
                ValueChanged?.Invoke(new Actions.ActionEntityAttributeChange(_entity, tag, oldValue, newValue, type, new Action<RSDKv5.SceneEntity, string, object, object, AttributeTypes>(SetObjectProperties)));
                SetObjectProperties(_entity, tag, oldValue, newValue, type);
            }
        }

        #endregion
    }
}
