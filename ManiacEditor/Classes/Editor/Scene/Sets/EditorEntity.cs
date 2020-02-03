using ManiacEditor.Entity_Renders;
using RSDKv5;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ManiacEditor.Enums;

namespace ManiacEditor.Classes.Editor.Scene.Sets
{
    [Serializable]
    public class EditorEntity : IDrawable
	{
        #region Original
        public bool Selected { get => GetSelected(); set => SetSelected(value); }
        private bool isSelected = false;
        public int SelectedIndex = -1;
        public DateTimeOffset? TimeWhenSelected = null;
        public Action<ManiacEditor.Actions.IAction> ValueChanged = new Action<ManiacEditor.Actions.IAction>(x =>
        {
            ManiacEditor.Controls.Base.MainEditor.Instance.UndoStack.Push(x);
            ManiacEditor.Controls.Base.MainEditor.Instance.RedoStack.Clear();      
        });


        public bool GetSelected()
        {
            return isSelected;
        }
        public void SetSelected(bool value)
        {
            if (value == true)
            {
                if (_entity.Object.Name.Name == "Spline" && IsInternalObject) ManiacEditor.Controls.Base.MainEditor.Instance.EditorToolbar.SelectedSplineIDChangedEvent(_entity.attributesMap["SplineID"].ValueInt32);
                isSelected = value;
                TimeWhenSelected = DateTimeOffset.Now;
            }
            else
            {
                isSelected = value;
                TimeWhenSelected = null;
                SelectedIndex = -1;
            }

            Classes.Editor.Solution.Entities.UpdateSelectedIndexForEntities();
        }

        public bool InTempSelection = false;
        public bool TempSelected = false;

        //public static Classes.Edit.Scene.Sets.EditorEntity Instance;
        public Methods.Entities.EntityAnimator EditorAnimations;
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
            EditorAnimations = new Methods.Entities.EntityAnimator(ManiacEditor.Controls.Base.MainEditor.Instance);

            if (ManiacEditor.Controls.Base.MainEditor.Instance.EntityDrawing.EntityRenderers.Count == 0)
            {
                var types = GetType().Assembly.GetTypes().Where(t => t.BaseType == typeof(EntityRenderer)).ToList();
                foreach (var type in types)
                    ManiacEditor.Controls.Base.MainEditor.Instance.EntityDrawing.EntityRenderers.Add((EntityRenderer)Activator.CreateInstance(type));
            }

            if (ManiacEditor.Controls.Base.MainEditor.Instance.EntityDrawing.LinkedEntityRenderers.Count == 0)
            {
                var types = GetType().Assembly.GetTypes().Where(t => t.BaseType == typeof(LinkedRenderer)).ToList();
                foreach (var type in types)
                    ManiacEditor.Controls.Base.MainEditor.Instance.EntityDrawing.LinkedEntityRenderers.Add((LinkedRenderer)Activator.CreateInstance(type));

                foreach (LinkedRenderer render in ManiacEditor.Controls.Base.MainEditor.Instance.EntityDrawing.LinkedEntityRenderers)
                {
                    render.EditorInstance = ManiacEditor.Controls.Base.MainEditor.Instance;
                }
            }


        }

        public EditorEntity(SceneEntity entity, bool IsInternal)
        {
            IsInternalObject = IsInternal;
            this._entity = entity;
        }

        public void ExportDraw(Graphics g, bool Editor = true)
        {
            DrawBase(new Classes.Editor.Draw.GraphicsHandler(g), Editor);
        }

        public void Draw(Graphics g)
        {
            DrawBase(new Classes.Editor.Draw.GraphicsHandler(g));
        }

        public Classes.Editor.Scene.Sets.EditorEntity GetSelf()
        {
            return this;
        }

        public bool ContainsPoint(Point point)
        {
            if (filteredOut) return false;

            return GetDimensions().Contains(point);
        }

        public void DrawUIButtonBack(Classes.Editor.Draw.GraphicsHandler d, int x, int y, int width, int height, int frameH, int frameW, int Transparency)
        {
            width += 1;
            height += 1;



            bool wEven = width % 2 == 0;
            bool hEven = height % 2 == 0;

            int zoomOffset = (Classes.Editor.SolutionState.Zoom % 1 == 0 ? 0 : 1);

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
        public void DrawTriangle(Classes.Editor.Draw.GraphicsHandler d, int x, int y, int width, int height, int frameH, int frameW, int state = 0, int Transparency = 0xFF)
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

            if (Methods.GameHandler.GameRunning && ManiacEditor.Core.Settings.MyGameOptions.RealTimeObjectMovementMode && !IsInternalObject)
            {

                int ObjectStart = Methods.GameHandler.ObjectStart[Methods.GameHandler.GameVersion.IndexOf(Methods.GameHandler.SelectedGameVersion)];
                int ObjectSize = Methods.GameHandler.ObjectSize[Methods.GameHandler.GameVersion.IndexOf(Methods.GameHandler.SelectedGameVersion)];

                int ObjectAddress = ObjectStart + (ObjectSize * Classes.Editor.Solution.Entities.GetRealSlotID(_entity));
                ManiacEditor.Controls.Base.MainEditor.Instance.GameMemory.WriteInt16(ObjectAddress + 2, _entity.Position.X.High);
                ManiacEditor.Controls.Base.MainEditor.Instance.GameMemory.WriteInt16(ObjectAddress + 6, _entity.Position.Y.High);
            }


        }

        public Rectangle GetDimensions(bool GridAlignment = false)
        {

            if (GridAlignment)
            {
                if (Classes.Editor.SolutionState.UseMagnetMode)
                {
                    int x = Classes.Editor.SolutionState.MagnetSize * (_entity.Position.X.High / Classes.Editor.SolutionState.MagnetSize);
                    int y = Classes.Editor.SolutionState.MagnetSize * (_entity.Position.Y.High / Classes.Editor.SolutionState.MagnetSize);
                    return new Rectangle(x, y, Classes.Editor.SolutionState.MagnetSize, Classes.Editor.SolutionState.MagnetSize);
                }
                else
                {
                    return new Rectangle(_entity.Position.X.High, _entity.Position.Y.High, Classes.Editor.Constants.ENTITY_NAME_BOX_WIDTH, Classes.Editor.Constants.ENTITY_NAME_BOX_HEIGHT);
                }

            }
            else return new Rectangle(_entity.Position.X.High, _entity.Position.Y.High, Classes.Editor.Constants.ENTITY_NAME_BOX_WIDTH, Classes.Editor.Constants.ENTITY_NAME_BOX_HEIGHT);
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
					((filter == 1 || filter == 5) && !ManiacEditor.Core.Settings.MyDefaults.ShowBothEntities) ||
					(filter == 2 && !ManiacEditor.Core.Settings.MyDefaults.ShowManiaEntities) ||
					(filter == 4 && !ManiacEditor.Core.Settings.MyDefaults.ShowEncoreEntities) ||
					(filter == 255 && !ManiacEditor.Core.Settings.MyDefaults.ShowPinballEntities) ||
					((filter < 1 || filter == 3 || filter > 5 && filter != 255) && !ManiacEditor.Core.Settings.MyDefaults.ShowOtherEntities);
			}
			else
			{
                filteredOut = !ManiacEditor.Core.Settings.MyDefaults.ShowFilterlessEntities;
			}


            if (Classes.Editor.SolutionState.entitiesTextFilter != "" && !_entity.Object.Name.Name.Contains(Classes.Editor.SolutionState.entitiesTextFilter))
            {
                filteredOut = true;
            }

            return filteredOut;
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
                return (TempSelected && ManiacEditor.Classes.Editor.SolutionState.IsEntitiesEdit()) ? System.Drawing.Color.MediumPurple : System.Drawing.Color.MediumTurquoise;
            }
            else
            {
                return (Selected && ManiacEditor.Classes.Editor.SolutionState.IsEntitiesEdit()) ? System.Drawing.Color.MediumPurple : System.Drawing.Color.MediumTurquoise;
            }
        }
        public int GetTransparencyLevel()
        {
            return (Classes.Editor.Solution.EditLayerA == null) ? 0xff : 0x32;
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
			if (priority != 0) validPlane = Methods.Entities.AttributeHandler.PlaneFilterCheck(_entity, priority);
			else validPlane = true;
			
			return validPlane;
		}
		public virtual void DrawBoxOnly(DevicePanel d)
		{
			int Transparency = (Classes.Editor.Solution.EditLayerA == null) ? 0xff : 0x32;
			int x = _entity.Position.X.High;
			int y = _entity.Position.Y.High;

			if (filteredOut) return;

            System.Drawing.Color color = GetBoxInsideColor();
            System.Drawing.Color color2 = GetFilterBoxColor();

            DrawSelectionBox(new Classes.Editor.Draw.GraphicsHandler(d), x, y, Transparency, color, color2);

		}
        public virtual void Draw(DevicePanel d)
        {
            Draw(new Classes.Editor.Draw.GraphicsHandler(d));
        }

        public virtual void Draw(Classes.Editor.Draw.GraphicsHandler d)
        {
            if (filteredOut) return;
            if (Methods.Entities.EntityDrawing.RenderingSettings.LinkedObjectsToRender.Contains(_entity.Object.Name.Name) && Classes.Editor.SolutionState.ShowEntityPathArrows)
            {
                try
                {
                    var structure = new Structures.LinkedEntityRenderProp(d, _entity, this);
                    LinkedRenderer renderer = ManiacEditor.Controls.Base.MainEditor.Instance.EntityDrawing.LinkedEntityRenderers.Where(t => t.GetObjectName() == _entity.Object.Name.Name.ToString()).FirstOrDefault();
                    if (renderer != null) renderer.Draw(structure);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Unable to load the linked render for " + _entity.Object.Name.Name + "! " + ex.ToString());
                    ManiacEditor.Controls.Base.MainEditor.Instance.EntityDrawing.linkedrendersWithErrors.Add(_entity.Object.Name.Name);

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
            DrawSelectionBox(new Classes.Editor.Draw.GraphicsHandler(d), x, y, Transparency, System.Drawing.Color.Transparent, System.Drawing.Color.Red);
        }

        public virtual void DrawBase(Classes.Editor.Draw.GraphicsHandler d, bool drawSelectionBox = true)
        {
            List<string> entityRenderList = Methods.Entities.EntityDrawing.RenderingSettings.ObjectToRender;
            List<string> onScreenExlusionList = (ManiacEditor.Core.Settings.MyPerformance.DisableRendererExclusions ? new List<string>() : Methods.Entities.EntityDrawing.RenderingSettings.ObjectCullingExclusions);
         
            if (!onScreenExlusionList.Contains(_entity.Object.Name.Name)) if (!this.IsObjectOnScreen(d)) return;
            System.Drawing.Color color = GetBoxInsideColor();
            System.Drawing.Color color2 = GetFilterBoxColor();
            int Transparency = GetTransparencyLevel();
            ManiacEditor.Controls.Base.MainEditor.Instance.EntityDrawing.LoadNextAnimation(this);

            int x = _entity.Position.X.High;
            int y = _entity.Position.Y.High;
            int _ChildX = GetChildX();
            int _ChildY = GetChildY();
            bool fliph = false;
            bool flipv = false;
            bool rotate = false;
            var offset = GetRotationFromAttributes(ref fliph, ref flipv, ref rotate);
            string name = _entity.Object.Name.Name;

			if (!drawSelectionBoxInFront && !Classes.Editor.SolutionState.EntitySelectionBoxesAlwaysPrioritized && drawSelectionBox) DrawSelectionBox(d, x, y, Transparency, color, color2);

            if (!ManiacEditor.Core.Settings.MyPerformance.NeverLoadEntityTextures)
            {
                if (entityRenderList.Contains(name)) PrimaryDraw(d, onScreenExlusionList);
                else FallbackDraw(d, x, y, _ChildX, _ChildY, Transparency, color);
            }

            if (drawSelectionBoxInFront && !Classes.Editor.SolutionState.EntitySelectionBoxesAlwaysPrioritized && drawSelectionBox) DrawSelectionBox(d, x, y, Transparency, color, color2);
		}
        public virtual void PrimaryDraw(Classes.Editor.Draw.GraphicsHandler d, List<string> onScreenExlusionList)
        {
            if ((this.IsObjectOnScreen(d) || onScreenExlusionList.Contains(_entity.Object.Name.Name)) && ManiacEditor.Core.Settings.MyPerformance.UseAlternativeRenderingMode)
            {
                ManiacEditor.Controls.Base.MainEditor.Instance.EntityDrawing.DrawOthers(d, _entity, this, childX, childY, index, previousChildCount, platformAngle, EditorAnimations, Selected, childDrawAddMode);
            }
            else if (!ManiacEditor.Core.Settings.MyPerformance.UseAlternativeRenderingMode)
            {
                ManiacEditor.Controls.Base.MainEditor.Instance.EntityDrawing.DrawOthers(d, _entity, this, childX, childY, index, previousChildCount, platformAngle, EditorAnimations, Selected, childDrawAddMode);
            }
        }
        public virtual void FallbackDraw(Classes.Editor.Draw.GraphicsHandler d, int x, int y, int _ChildX, int _ChildY, int Transparency, System.Drawing.Color color, bool overridePosition = false)
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
            var editorAnim = ManiacEditor.Controls.Base.MainEditor.Instance.EntityDrawing.LoadAnimation2(name, d.DevicePanel, -1, -1, fliph, flipv, rotate);
            if (editorAnim != null && editorAnim.Frames.Count > 0)
            {
                renderNotFound = false;
                // Special cases that always display a set frame(?)
                if (ManiacEditor.Controls.Base.MainEditor.Instance.EditorToolbar.ShowAnimations.IsEnabled == true)
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
                    d.DrawBitmap(new Classes.Editor.Draw.GraphicsHandler.GraphicsInfo(frame.Texture), __X + frame.Frame.PivotX + ((int)offset.X * frame.Frame.Width), __Y + frame.Frame.PivotY + ((int)offset.Y * frame.Frame.Height),
                        frame.Frame.Width, frame.Frame.Height, false, Transparency);
                }
                else
                { // No frame to render
                    if (Classes.Editor.SolutionState.ShowEntitySelectionBoxes) d.DrawRectangle(x, y, x + Classes.Editor.Constants.ENTITY_NAME_BOX_WIDTH, y + Classes.Editor.Constants.ENTITY_NAME_BOX_HEIGHT, System.Drawing.Color.FromArgb(Transparency, color));
                }
                //Failsafe?
                //DrawOthers(d);

            }
            else
            {
               renderNotFound = true;
            }
        }
        public void DrawSelectionBox(Classes.Editor.Draw.GraphicsHandler d, int x, int y, int Transparency, System.Drawing.Color color, System.Drawing.Color color2)
        {
            if (Classes.Editor.SolutionState.ShowEntitySelectionBoxes && !useOtherSelectionVisiblityMethod && this.IsObjectOnScreen(d))
            {
                if (renderNotFound)
                {
                    d.DrawRectangle(x, y, x + Classes.Editor.Constants.ENTITY_NAME_BOX_WIDTH, y + Classes.Editor.Constants.ENTITY_NAME_BOX_HEIGHT, System.Drawing.Color.FromArgb(Transparency, color));
                }
                else
                {
                    d.DrawRectangle(x, y, x + Classes.Editor.Constants.ENTITY_NAME_BOX_WIDTH, y + Classes.Editor.Constants.ENTITY_NAME_BOX_HEIGHT, GetSelectedColor(color2));
                }
                d.DrawLine(x, y, x + Classes.Editor.Constants.ENTITY_NAME_BOX_WIDTH, y, System.Drawing.Color.FromArgb(Transparency, color2));
                d.DrawLine(x, y, x, y + Classes.Editor.Constants.ENTITY_NAME_BOX_HEIGHT, System.Drawing.Color.FromArgb(Transparency, color2));
                d.DrawLine(x, y + Classes.Editor.Constants.ENTITY_NAME_BOX_HEIGHT, x + Classes.Editor.Constants.ENTITY_NAME_BOX_WIDTH, y + Classes.Editor.Constants.ENTITY_NAME_BOX_HEIGHT, System.Drawing.Color.FromArgb(Transparency, color2));
                d.DrawLine(x + Classes.Editor.Constants.ENTITY_NAME_BOX_WIDTH, y, x + Classes.Editor.Constants.ENTITY_NAME_BOX_WIDTH, y + Classes.Editor.Constants.ENTITY_NAME_BOX_HEIGHT, System.Drawing.Color.FromArgb(Transparency, color2));
                if (ManiacEditor.Core.Settings.MyPerformance.DisableEntitySelectionBoxText == false)
                {
                    if (Classes.Editor.SolutionState.Zoom >= 1)
                    {
                        d.DrawTextSmall(string.Format("{0} (ID: {1})", _entity.Object.Name, _entity.SlotID), x + 2, y + 2, Classes.Editor.Constants.ENTITY_NAME_BOX_WIDTH - 4, System.Drawing.Color.FromArgb(Transparency, System.Drawing.Color.Black), true);
                    }
                }

                if (SelectedIndex != -1)
                { 
                    d.DrawText(string.Format("{0}", SelectedIndex + 1), x + 1, y + 1, Classes.Editor.Constants.ENTITY_NAME_BOX_WIDTH, System.Drawing.Color.Black, true);
                    d.DrawText(string.Format("{0}", SelectedIndex + 1), x, y, Classes.Editor.Constants.ENTITY_NAME_BOX_WIDTH, System.Drawing.Color.Red, true);
                }
            }
        }

        public System.Drawing.Color GetSelectedColor(System.Drawing.Color color)
        {
            if (InTempSelection)
            {
                return System.Drawing.Color.FromArgb(TempSelected && ManiacEditor.Classes.Editor.SolutionState.IsEntitiesEdit() ? 0x60 : 0x00, color);
            }
            else
            {
                return System.Drawing.Color.FromArgb(Selected && ManiacEditor.Classes.Editor.SolutionState.IsEntitiesEdit() ? 0x60 : 0x00, color);
            }
        }

		public Methods.Entities.EntityDrawing.EditorAnimation.EditorFrame GetFrameFromAttribute(Methods.Entities.EntityDrawing.EditorAnimation anim, AttributeValue attribute, string key = "frameID")
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
                case AttributeTypes.ENUM:
                    frameID = (int)_entity.attributesMap[key].ValueEnum;
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
                    case AttributeTypes.ENUM:
                        dir = (int) attribute.ValueEnum;
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
        public bool IsObjectOnScreen(Classes.Editor.Draw.GraphicsHandler d)
        {
            
            int x = _entity.Position.X.High + childX;
            int y = _entity.Position.Y.High + childY;
            if (childDrawAddMode == false)
            {
                x = childX;
                y = childY;
            }
            int Transparency = (Classes.Editor.Solution.EditLayerA == null) ? 0xff : 0x32;

            bool isObjectVisibile = false;

            
			if (!filteredOut)
			{
                if (RenderDrawing == null || RenderDrawing.GetObjectName() != _entity.Object.Name.Name) RenderDrawing = ManiacEditor.Controls.Base.MainEditor.Instance.EntityDrawing.EntityRenderers.Where(t => t.GetObjectName() == _entity.Object.Name.Name).FirstOrDefault();
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

        public void Dispose()
        {

        }
        #endregion

        #region Animations

        public void ProcessAnimation(int speed, int frameCount, int duration, int startFrame = 0)
        {
            // Playback
            if (Classes.Editor.SolutionState.AllowSpriteAnimations)
            {
                if (speed > 0)
                {
                    int speed1 = speed * 64 / (duration == 0 ? 256 : duration);
                    if (speed1 == 0)
                        speed1 = 1;
                    if ((DateTime.Now - lastFrametime).TotalMilliseconds > 1024 / speed1)
                    {
                        index++;
                        lastFrametime = DateTime.Now;
                    }
                }
            }
            else index = 0 + startFrame;
            if (index >= frameCount)
                index = 0;

        }

        #endregion
    }
}
