using ManiacEditor.Entity_Renders;
using RSDKv5;
using SharpDX.Direct3D9;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices.ComTypes;
using System.Diagnostics;
using MonoGame.UI.Forms;
using MonoGame.Extended;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using ImageMagick;

namespace ManiacEditor
{
    [Serializable]
    public class EditorEntity : IDrawable
	{
        public bool Selected { get => GetSelected(); set => SetSelected(value); }
        private bool isSelected = false;
        public int SelectedIndex = -1;
        public DateTimeOffset? TimeWhenSelected = null;

        public bool GetSelected()
        {
            return isSelected;
        }
        public void SetSelected(bool value)
        {
            if (value == true)
            {
                isSelected = value;
                TimeWhenSelected = DateTimeOffset.Now;
            }
            else
            {
                isSelected = value;
                TimeWhenSelected = null;
                SelectedIndex = -1;
            }

            Editor.Instance.Entities.UpdateSelectedIndexForEntities();
        }

        public bool InTempSelection = false;
        public bool TempSelected = false;

        //public static EditorEntity Instance;
        public EditorAnimations EditorAnimations;
        public AttributeValidater AttributeValidater;
        public EntityRenderer RenderDrawing;
        public LinkedRenderer LinkedRenderDrawing;



        public bool filteredOut;
        public bool filteredOutByParent;
        public string uniqueKey = "";
		public bool useOtherSelectionVisiblityMethod = false; //Not Universal; Only for Renders that need it
		public bool drawSelectionBoxInFront = true;
		public bool renderNotFound = false;
        public bool IsInternalObject = false;

        #region Animation + Child Stuff
        //Rotating/Moving Platforms
        public int platformAngle = 0;
        public int platformpositionX = 0;
        public int platformpositionY = 0;

        //For Drawing Extra Child Objects
        public bool childDraw = false;
        public int childX = 0;
        public int childY = 0;
        public bool childDrawAddMode = true;
        public int previousChildCount = 0; //For Reseting ChildCount States
        public static bool Working = false;
        public DateTime lastFrametime;
        public int index = 0;
        public int layerPriority = 0;
        #endregion

        public SceneEntity Entity { get { return _entity; }}
        private SceneEntity _entity;

        public int PositionX { get => GetPosition(true); set => SetPosition(true, (short)value); }
        public int PositionY { get => GetPosition(false); set => SetPosition(false, (short)value); }
        public string Name { get => GetName(); }

        private int GetPosition(bool useX)
        {
            if (useX) return _entity.Position.X.High;
            else return _entity.Position.Y.High;
        }

        private string GetName()
        {
            return _entity.Object.Name.Name;
        }

        private void SetPosition(bool useX, short value)
        {
            if (useX) _entity.Position.X.High = value;
            else _entity.Position.Y.High = value;
        }

        public EditorEntity(SceneEntity entity)
        {
            this._entity = entity;
            lastFrametime = DateTime.Now;
            EditorAnimations = new EditorAnimations(Editor.Instance);
            AttributeValidater = new AttributeValidater();

            if (Editor.Instance.EntityDrawing.EntityRenderers.Count == 0)
            {
                var types = GetType().Assembly.GetTypes().Where(t => t.BaseType == typeof(EntityRenderer)).ToList();
                foreach (var type in types)
                    Editor.Instance.EntityDrawing.EntityRenderers.Add((EntityRenderer)Activator.CreateInstance(type));
            }

            if (Editor.Instance.EntityDrawing.LinkedEntityRenderers.Count == 0)
            {
                var types = GetType().Assembly.GetTypes().Where(t => t.BaseType == typeof(LinkedRenderer)).ToList();
                foreach (var type in types)
                    Editor.Instance.EntityDrawing.LinkedEntityRenderers.Add((LinkedRenderer)Activator.CreateInstance(type));

                foreach (LinkedRenderer render in Editor.Instance.EntityDrawing.LinkedEntityRenderers)
                {
                    render.EditorInstance = Editor.Instance;
                }
            }


        }

        public EditorEntity(SceneEntity entity, bool IsInternal)
        {
            IsInternalObject = IsInternal;
            this._entity = entity;
        }


        public void Draw(Graphics g)
        {

        }

        public EditorEntity GetSelf()
        {
            return this;
        }

        public bool ContainsPoint(Point point)
        {
            if (filteredOut) return false;

            return GetDimensions().Contains(point);
        }

        public void DrawUIButtonBack(DevicePanel d, int x, int y, int width, int height, int frameH, int frameW, int Transparency)
        {
            width += 1;
            height += 1;



            bool wEven = width % 2 == 0;
            bool hEven = height % 2 == 0;

            int zoomOffset = (Editor.Instance.GetZoom() % 1 == 0 ? 0 : 1);

            int x2 = x;
            int y2 = y;
            if (width != 0) x2 -= width / 2;
            if (height != 0) y2 -= height / 2;

            
            d.DrawRectangle(x2, y2, x2 + width + (!wEven ? zoomOffset : 0), y2 + height + (!hEven ? zoomOffset : 0), System.Drawing.Color.FromArgb(Transparency, 0, 0, 0));

            System.Drawing.Color colur = System.Drawing.Color.FromArgb(Transparency, 0, 0, 0);
            //Left Triangle         
            for (int i = 1; i <= (height); i++)
            {
                d.DrawLine(x2 - height + i, (int)(y + (!hEven ? 1 : 0) + (height / 2) - i), x2,(int)(y + (!hEven ? 1 : 0) + (height / 2) - i), colur, true);
            }

            int x3 = x2 + width;
            int y3 = y2 + height;

            //Right Triangle
            for (int i = 1; i <= height; i++)
            {
                d.DrawLine(x3, y + (!hEven ? 1 : 0) + (height / 2) - i, x3 + height + i, y + (!hEven ? 1 : 0) + (height / 2) - i, colur, true);
            }
        }
        public void DrawTriangle(DevicePanel d, int x, int y, int width, int height, int frameH, int frameW, int state = 0, int Transparency = 0xFF)
        {

            bool wEven = width % 2 == 0;
            bool hEven = height % 2 == 0;

            System.Drawing.Color colur = System.Drawing.Color.FromArgb(Transparency, 0, 0, 0);
            if (state == 0)
            {
                //Left Triangle         
                for (int i = 1; i <= (height); i++)
                {
                    d.DrawLine(x - height + i, y + (!hEven ? 1 : 0) + (height / 2) - i, x, y + (!hEven ? 1 : 0) + (height / 2) - i, colur, true);
                }
            }
            else if (state == 1)
            {
                //Right Triangle
                for (int i = 1; i <= height; i++)
                {
                    d.DrawLine(x, y + (!hEven ? 1 : 0) + (height / 2) - i, x + height + i, y + (!hEven ? 1 : 0) + (height / 2) - i, colur, true);
                }
            }



        }

        public bool IsInArea(Rectangle area)
        {
            if (filteredOut) return false;

            return GetDimensions().IntersectsWith(area);
        }


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

            if (Editor.Instance.InGame.GameRunning && Settings.MyGameOptions.RealTimeObjectMovementMode && !IsInternalObject)
            {

                int ObjectStart = EditorInGame.ObjectStart[EditorInGame.GameVersion.IndexOf(EditorInGame.SelectedGameVersion)];
                int ObjectSize =  EditorInGame.ObjectSize[EditorInGame.GameVersion.IndexOf(EditorInGame.SelectedGameVersion)];

                int ObjectAddress = ObjectStart + (ObjectSize * Editor.Instance.Entities.GetRealSlotID(_entity));
                Editor.Instance.GameMemory.WriteInt16(ObjectAddress + 2, _entity.Position.X.High);
                Editor.Instance.GameMemory.WriteInt16(ObjectAddress + 6, _entity.Position.Y.High);
            }


        }

        public Rectangle GetDimensions(bool GridAlignment = false)
        {

            if (GridAlignment)
            {
                if (Editor.Instance.UIModes.UseMagnetMode)
                {
                    int x = Editor.Instance.UIModes.MagnetSize * (_entity.Position.X.High / Editor.Instance.UIModes.MagnetSize);
                    int y = Editor.Instance.UIModes.MagnetSize * (_entity.Position.Y.High / Editor.Instance.UIModes.MagnetSize);
                    return new Rectangle(x, y, Editor.Instance.UIModes.MagnetSize, Editor.Instance.UIModes.MagnetSize);
                }
                else
                {
                    return new Rectangle(_entity.Position.X.High, _entity.Position.Y.High, EditorConstants.ENTITY_NAME_BOX_WIDTH, EditorConstants.ENTITY_NAME_BOX_HEIGHT);
                }

            }
            else return new Rectangle(_entity.Position.X.High, _entity.Position.Y.High, EditorConstants.ENTITY_NAME_BOX_WIDTH, EditorConstants.ENTITY_NAME_BOX_HEIGHT);
        }

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
				filteredOut =
					((filter == 1 || filter == 5) && !Settings.MyDefaults.ShowBothEntities) ||
					(filter == 2 && !Settings.MyDefaults.ShowManiaEntities) ||
					(filter == 4 && !Settings.MyDefaults.ShowEncoreEntities) ||
					(filter == 255 && !Settings.MyDefaults.ShowPinballEntities) ||
					((filter < 1 || filter == 3 || filter > 5 && filter != 255) && !Settings.MyDefaults.ShowOtherEntities);
			}
			else
			{
				filteredOut = !Settings.MyDefaults.ShowFilterlessEntities;
			}


            if (Editor.Instance.UIModes.entitiesTextFilter != "" && !_entity.Object.Name.Name.Contains(Editor.Instance.UIModes.entitiesTextFilter))
            {
                filteredOut = true;
            }

            return filteredOut;
        }

        public void TestIfPlayerObject()
        {
            if (_entity.Object.Name.Name == "Player" && !Editor.Instance.playerObjectPosition.Contains(_entity))
            {
                Editor.Instance.playerObjectPosition.Add(_entity);
            }
        }
        public System.Drawing.Color GetFilterBoxColor()
        {
            System.Drawing.Color color = System.Drawing.Color.DarkBlue;
            if (HasSpecificFilter(1) || HasSpecificFilter(5))
            {
                color = System.Drawing.Color.DarkBlue;
            }
            else if (HasSpecificFilter(2))
            {
                color = System.Drawing.Color.DarkRed;
            }
            else if (HasSpecificFilter(4))
            {
                color = System.Drawing.Color.DarkGreen;
            }
            else if (HasSpecificFilter(255))
            {
                color = System.Drawing.Color.Purple;
            }
            else if (HasFilterOther())
            {
                color = System.Drawing.Color.Yellow;
            }
            else if (!HasFilter())
            {
                color = System.Drawing.Color.White;
            }
            return color;

        }
        public System.Drawing.Color GetBoxInsideColor()
        {
            if (InTempSelection)
            {
                return (TempSelected && Editor.Instance.IsEntitiesEdit()) ? System.Drawing.Color.MediumPurple : System.Drawing.Color.MediumTurquoise;
            }
            else
            {
                return (Selected && Editor.Instance.IsEntitiesEdit()) ? System.Drawing.Color.MediumPurple : System.Drawing.Color.MediumTurquoise;
            }
        }
        public int GetTransparencyLevel()
        {
            return (Editor.Instance.EditLayerA == null) ? 0xff : 0x32;
        }
        public int GetChildX()
        {
            return  (childDrawAddMode == false ? childX : _entity.Position.X.High + (childDraw ? childX : 0));
        }
        public int GetChildY()
        {
            return (childDrawAddMode == false ? childY : _entity.Position.Y.High + (childDraw ? childY : 0));
        }
        public bool ValidPriorityPlane(int priority)
		{
			bool validPlane = false;
			if (priority != 0) validPlane = AttributeValidater.PlaneFilterCheck(_entity, priority);
			else validPlane = true;
			
			return validPlane;
		}
		public virtual void DrawBoxOnly(DevicePanel d)
		{
			int Transparency = (Editor.Instance.EditLayerA == null) ? 0xff : 0x32;
			int x = _entity.Position.X.High;
			int y = _entity.Position.Y.High;

			if (filteredOut) return;

            System.Drawing.Color color = GetBoxInsideColor();
            System.Drawing.Color color2 = GetFilterBoxColor();

            DrawSelectionBox(d, x, y, Transparency, color, color2);

		}
        public virtual void Draw(DevicePanel d)
        {
            if (filteredOut) return;
            if (EditorEntityDrawing.LinkedRendersNames.Contains(_entity.Object.Name.Name) && Editor.Instance.UIModes.ShowEntityPathArrows)
            {
                try
                {
                    LinkedRenderer renderer = Editor.Instance.EntityDrawing.LinkedEntityRenderers.Where(t => t.GetObjectName() == _entity.Object.Name.Name.ToString()).FirstOrDefault();
                    if (renderer != null) renderer.Draw(d, _entity, this);
                }
                catch (Exception ex)
                {
                    RSDKrU.MessageBox.Show("Unable to load the linked render for " + _entity.Object.Name.Name + "! " + ex.ToString());
                    Editor.Instance.EntityDrawing.linkedrendersWithErrors.Add(_entity.Object.Name.Name);

                }

            }
            else
            {
                DrawBase(d);
            }
        }
        public virtual void DrawInternal(DevicePanel d)
        {
            int Transparency = GetTransparencyLevel();

            int x = _entity.Position.X.High;
            int y = _entity.Position.Y.High;

            DrawSelectionBox(d, x, y, Transparency, System.Drawing.Color.Transparent, System.Drawing.Color.Red);
        }

        public virtual void DrawBase(DevicePanel d)
        {
            TestIfPlayerObject();

            List<string> entityRenderList = Editor.Instance.EntityDrawing.entityRenderingObjects;
            List<string> onScreenExlusionList = (Settings.MyPerformance.DisableRendererExclusions ? new List<string>() : Editor.Instance.EntityDrawing.renderOnScreenExlusions);
         
            if (!onScreenExlusionList.Contains(_entity.Object.Name.Name)) if (!this.IsObjectOnScreen(d)) return;
            System.Drawing.Color color = GetBoxInsideColor();
            System.Drawing.Color color2 = GetFilterBoxColor();
            int Transparency = GetTransparencyLevel();
            Editor.Instance.EntityDrawing.LoadNextAnimation(this);

            int x = _entity.Position.X.High;
            int y = _entity.Position.Y.High;
            int _ChildX = GetChildX();
            int _ChildY = GetChildY();
            bool fliph = false;
            bool flipv = false;
            bool rotate = false;
            var offset = GetRotationFromAttributes(ref fliph, ref flipv, ref rotate);
            string name = _entity.Object.Name.Name;

			if (!drawSelectionBoxInFront && !Editor.Instance.UIModes.EntitySelectionBoxesAlwaysPrioritized) DrawSelectionBox(d, x, y, Transparency, color, color2);

            if (!Settings.MyPerformance.NeverLoadEntityTextures)
            {
                if (entityRenderList.Contains(name)) PrimaryDraw(d, onScreenExlusionList);
                else FallbackDraw(d, x, y, _ChildX, _ChildY, Transparency, color);
            }

            if (drawSelectionBoxInFront && !Editor.Instance.UIModes.EntitySelectionBoxesAlwaysPrioritized) DrawSelectionBox(d, x, y, Transparency, color, color2);
		}
        public virtual void PrimaryDraw(DevicePanel d, List<string> onScreenExlusionList)
        {
            if ((this.IsObjectOnScreen(d) || onScreenExlusionList.Contains(_entity.Object.Name.Name)) && Settings.MyPerformance.UseAlternativeRenderingMode)
            {
                Editor.Instance.EntityDrawing.DrawOthers(d, _entity, this, childX, childY, index, previousChildCount, platformAngle, EditorAnimations, Selected, AttributeValidater, childDrawAddMode);
            }
            else if (!Settings.MyPerformance.UseAlternativeRenderingMode)
            {
                Editor.Instance.EntityDrawing.DrawOthers(d, _entity, this, childX, childY, index, previousChildCount, platformAngle, EditorAnimations, Selected, AttributeValidater, childDrawAddMode);
            }
        }
        public virtual void FallbackDraw(DevicePanel d, int x, int y, int _ChildX, int _ChildY, int Transparency, System.Drawing.Color color, bool overridePosition = false)
        {
            int __X = GetChildX();
            int __Y = GetChildY();
            if (overridePosition)
            {
                __X = x;
                __Y = y;
            }
            bool fliph = false;
            bool flipv = false;
            bool rotate = false;
            var offset = GetRotationFromAttributes(ref fliph, ref flipv, ref rotate);
            string name = _entity.Object.Name.Name;
            var editorAnim = Editor.Instance.EntityDrawing.LoadAnimation2(name, d, -1, -1, fliph, flipv, rotate);
            if (editorAnim != null && editorAnim.Frames.Count > 0)
            {
                renderNotFound = false;
                // Special cases that always display a set frame(?)
                if (Editor.Instance.ShowAnimations.IsEnabled == true)
                {
                    if (_entity.Object.Name.Name == "StarPost")
                        index = 1;
                }
                // Just incase
                if (index >= editorAnim.Frames.Count)
                    index = 0;
                var frame = editorAnim.Frames[index];

                if (_entity.attributesMap.ContainsKey("frameID"))
                    frame = GetFrameFromAttribute(editorAnim, _entity.attributesMap["frameID"]);

                if (frame != null)
                {
                    EditorAnimations.ProcessAnimation(frame.Entry.SpeedMultiplyer, frame.Entry.Frames.Count, frame.Frame.Delay, index);
                    // Draw the normal filled Rectangle but Its visible if you have the entity selected
                    d.DrawBitmap(frame.Texture, __X + frame.Frame.PivotX + ((int)offset.X * frame.Frame.Width), __Y + frame.Frame.PivotY + ((int)offset.Y * frame.Frame.Height),
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
                }
                else
                { // No frame to render
                    if (Editor.Instance.UIModes.ShowEntitySelectionBoxes) d.DrawRectangle(x, y, x + EditorConstants.ENTITY_NAME_BOX_WIDTH, y + EditorConstants.ENTITY_NAME_BOX_HEIGHT, System.Drawing.Color.FromArgb(Transparency, color));
                }
                //Failsafe?
                //DrawOthers(d);

            }
            else
            {
               renderNotFound = true;
            }
        }
        public void DrawSelectionBox(DevicePanel d, int x, int y, int Transparency, System.Drawing.Color color, System.Drawing.Color color2)
        {
            if (Editor.Instance.UIModes.ShowEntitySelectionBoxes && !useOtherSelectionVisiblityMethod && this.IsObjectOnScreen(d))
            {
                if (renderNotFound)
                {
                    d.DrawRectangle(x, y, x + EditorConstants.ENTITY_NAME_BOX_WIDTH, y + EditorConstants.ENTITY_NAME_BOX_HEIGHT, System.Drawing.Color.FromArgb(Transparency, color));
                }
                else
                {
                    d.DrawRectangle(x, y, x + EditorConstants.ENTITY_NAME_BOX_WIDTH, y + EditorConstants.ENTITY_NAME_BOX_HEIGHT, GetSelectedColor(color2));
                }
                d.DrawLine(x, y, x + EditorConstants.ENTITY_NAME_BOX_WIDTH, y, System.Drawing.Color.FromArgb(Transparency, color2));
                d.DrawLine(x, y, x, y + EditorConstants.ENTITY_NAME_BOX_HEIGHT, System.Drawing.Color.FromArgb(Transparency, color2));
                d.DrawLine(x, y + EditorConstants.ENTITY_NAME_BOX_HEIGHT, x + EditorConstants.ENTITY_NAME_BOX_WIDTH, y + EditorConstants.ENTITY_NAME_BOX_HEIGHT, System.Drawing.Color.FromArgb(Transparency, color2));
                d.DrawLine(x + EditorConstants.ENTITY_NAME_BOX_WIDTH, y, x + EditorConstants.ENTITY_NAME_BOX_WIDTH, y + EditorConstants.ENTITY_NAME_BOX_HEIGHT, System.Drawing.Color.FromArgb(Transparency, color2));
                if (Settings.MyPerformance.DisableEntitySelectionBoxText == false)
                {
                    if (Editor.Instance.GetZoom() >= 1)
                    {
                        d.DrawTextSmall(String.Format("{0} (ID: {1})", _entity.Object.Name, _entity.SlotID), x + 2, y + 2, EditorConstants.ENTITY_NAME_BOX_WIDTH - 4, System.Drawing.Color.FromArgb(Transparency, System.Drawing.Color.Black), true);
                    }
                }

                if (SelectedIndex != -1)
                { 
                    d.DrawText(string.Format("{0}", SelectedIndex + 1), x + 1, y + 1, EditorConstants.ENTITY_NAME_BOX_WIDTH, System.Drawing.Color.Black, true);
                    d.DrawText(string.Format("{0}", SelectedIndex + 1), x, y, EditorConstants.ENTITY_NAME_BOX_WIDTH, System.Drawing.Color.Red, true);
                }
            }
        }

        public System.Drawing.Color GetSelectedColor(System.Drawing.Color color)
        {
            if (InTempSelection)
            {
                return System.Drawing.Color.FromArgb(TempSelected && Editor.Instance.IsEntitiesEdit() ? 0x60 : 0x00, color);
            }
            else
            {
                return System.Drawing.Color.FromArgb(Selected && Editor.Instance.IsEntitiesEdit() ? 0x60 : 0x00, color);
            }
        }

		public EditorEntityDrawing.EditorAnimation.EditorFrame GetFrameFromAttribute(EditorEntityDrawing.EditorAnimation anim, AttributeValue attribute, string key = "frameID")
        {
            int frameID = -1;
            switch (attribute.Type)
            {
                case AttributeTypes.UINT8:
                    frameID = _entity.attributesMap[key].ValueUInt8;
                    break;
                case AttributeTypes.INT8:
                    frameID = _entity.attributesMap[key].ValueInt8;
                    break;
                case AttributeTypes.VAR:
                    frameID = (int)_entity.attributesMap[key].ValueVar;
                    break;
            }
            if (frameID >= anim.Frames.Count)
                frameID = (anim.Frames.Count - 1) - (frameID % anim.Frames.Count + 1);
            if (frameID < 0)
                frameID = 0;
            if (frameID >= 0 && frameID < int.MaxValue)
                return anim.Frames[frameID % anim.Frames.Count];
            else
                return null; // Don't Render the Animation
        }
        /// <summary>
        /// Guesses the rotation of an entity
        /// </summary>
        /// <param name="attributes">List of all Attributes</param>
        /// <param name="fliph">Reference to fliph</param>
        /// <param name="flipv">Reference to flipv</param>
        /// <returns>AnimationID Offset</returns>
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
                    case AttributeTypes.VAR:
                        dir = (int) attribute.ValueVar;
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
                    case AttributeTypes.VAR:
                        dir = (int)attribute.ValueVar;
                        break;
                }
                if (dir == 0) // Right
                {
                }
                else if (dir == 1) // left
                {
                    fliph = true;
                    offset.X = 0;
                    rotate = true;
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
        public bool IsObjectOnScreen(DevicePanel d)
        {
            
            int x = _entity.Position.X.High + childX;
            int y = _entity.Position.Y.High + childY;
            if (childDrawAddMode == false)
            {
                x = childX;
                y = childY;
            }
            int Transparency = (Editor.Instance.EditLayerA == null) ? 0xff : 0x32;

            bool isObjectVisibile = false;

            
			if (!filteredOut)
			{
                if (RenderDrawing == null || RenderDrawing.GetObjectName() != _entity.Object.Name.Name) RenderDrawing = Editor.Instance.EntityDrawing.EntityRenderers.Where(t => t.GetObjectName() == _entity.Object.Name.Name).FirstOrDefault();
                if (RenderDrawing != null)
				{
					isObjectVisibile = RenderDrawing.isObjectOnScreen(d, _entity, null, x, y, 0);
				}
				else
				{
					isObjectVisibile = d.IsObjectOnScreen(x, y, 20, 20);
				}
			}

            isObjectVisibile = d.IsObjectOnScreen(x, y, 20, 20);
            return isObjectVisibile;


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

        public void PrepareForExternalCopy()
        {
            _entity.PrepareForExternalCopy();
        }

        public bool IsExternal()
        {
            return _entity.IsExternal();
        }

        internal void Flip(FlipDirection flipDirection)
        {
            if (_entity.attributesMap.ContainsKey("flipFlag"))
            {
                if (flipDirection == FlipDirection.Horizontal)
                {
                    _entity.attributesMap["flipFlag"].ValueVar ^= 0x01;
                }
                else
                {
                    _entity.attributesMap["flipFlag"].ValueVar ^= 0x02;
                }
            }
        }

        public void Dispose()
        {

        }
	}
}
