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
    public class EditorEntity : IDrawable
	{
        #region Definitions

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
                if (value == true)
                {
                    if (_entity.Object.Name.Name == "Spline" && IsInternalObject) ManiacEditor.Methods.Internal.UserInterface.ChangeSplineSelectedID(_entity.attributesMap["SplineID"].ValueInt32);
                    IsSelected = value;
                    TimeWhenSelected = DateTimeOffset.Now;
                }
                else
                {
                    IsSelected = value;
                    TimeWhenSelected = null;
                    SelectedIndex = -1;
                }

                Methods.Editor.Solution.Entities.UpdateSelectedIndexForEntities();
            }
        }
        public bool InTempSelection { get; set; } = false;
        public bool TempSelected { get; set; } = false;
        public int SelectedIndex { get; set; } = -1;
        #endregion

        #region Original Object

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
                _entity.SlotID = SlotID;
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
            }
        }
        public string Name
        {
            get
            {
                return _entity.Object.Name.Name;
            }
        }


        public bool IsExternal
        {
            get
            {
                return _entity.IsExternal();
            }
        }
        public bool FilteredOut { get; set; }
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

        #region Methods
        public bool ContainsPoint(Point point)
        {
            if (FilteredOut) return false;

            return GetDimensions().Contains(point);
        }
        public bool IsInArea(Rectangle area)
        {
            if (FilteredOut) return false;

            return GetDimensions().IntersectsWith(area);
        }
        public Rectangle GetDimensions(bool GridAlignment = false)
        {

            if (GridAlignment)
            {
                if (Methods.Editor.SolutionState.UseMagnetMode)
                {
                    int x = Methods.Editor.SolutionState.MagnetSize * (_entity.Position.X.High / Methods.Editor.SolutionState.MagnetSize);
                    int y = Methods.Editor.SolutionState.MagnetSize * (_entity.Position.Y.High / Methods.Editor.SolutionState.MagnetSize);
                    return new Rectangle(x, y, Methods.Editor.SolutionState.MagnetSize, Methods.Editor.SolutionState.MagnetSize);
                }
                else
                {
                    return new Rectangle(_entity.Position.X.High, _entity.Position.Y.High, Methods.Editor.EditorConstants.ENTITY_NAME_BOX_WIDTH, Methods.Editor.EditorConstants.ENTITY_NAME_BOX_HEIGHT);
                }

            }
            else return new Rectangle(_entity.Position.X.High, _entity.Position.Y.High, Methods.Editor.EditorConstants.ENTITY_NAME_BOX_WIDTH, Methods.Editor.EditorConstants.ENTITY_NAME_BOX_HEIGHT);
        }

        #endregion

        #region Filter Management

        public bool SetFilter()
        {
            if (HasFilter())
            {
                int filter = _entity.GetAttribute("filter").ValueUInt8;

                /**
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
                    ((filter < 1 || filter == 3 || filter > 5 && filter != 255) && !ManiacEditor.Properties.Settings.MyDefaults.ShowOtherEntities);
            }
            else
            {
                FilteredOut = !ManiacEditor.Properties.Settings.MyDefaults.ShowFilterlessEntities;
            }


            if (Methods.Editor.SolutionState.ObjectFilter != "" && !_entity.Object.Name.Name.Contains(Methods.Editor.SolutionState.ObjectFilter))
            {
                FilteredOut = true;
            }

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
            if (Methods.Entities.EntityDrawing.CanDrawLinked(_entity.Object.Name.Name)) Methods.Entities.EntityDrawing.DrawLinked(d, this);
            else Methods.Entities.EntityDrawing.DrawNormal(d, this);
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
                _entity.Position.X.High += (short)diff.X;
                _entity.Position.Y.High += (short)diff.Y;
            }
            else
            {
                _entity.Position.X.High = (short)diff.X;
                _entity.Position.Y.High = (short)diff.Y;
            }

            if (Methods.Runtime.GameHandler.GameRunning && ManiacEditor.Properties.Settings.MyGameOptions.RealTimeObjectMovementMode && !IsInternalObject)
            {

                int ObjectStart = Methods.Runtime.GameHandler.ObjectStart[Methods.Runtime.GameHandler.GameVersion.IndexOf(Methods.Runtime.GameHandler.SelectedGameVersion)];
                int ObjectSize = Methods.Runtime.GameHandler.ObjectSize[Methods.Runtime.GameHandler.GameVersion.IndexOf(Methods.Runtime.GameHandler.SelectedGameVersion)];

                int ObjectAddress = ObjectStart + (ObjectSize * _entity.SlotID);
                Methods.Runtime.GameHandler.GameMemory.WriteInt16(ObjectAddress + 2, _entity.Position.X.High);
                Methods.Runtime.GameHandler.GameMemory.WriteInt16(ObjectAddress + 6, _entity.Position.Y.High);
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
