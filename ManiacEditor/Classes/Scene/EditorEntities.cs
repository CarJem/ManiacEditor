using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using GenerationsLib.Core;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using RSDKv5;
using ManiacEditor.Actions;
using ManiacEditor.Enums;
using ManiacEditor.Extensions;
using System.Collections;

namespace ManiacEditor.Classes.Scene
{
    public class EditorEntities : IDrawable
    {
        #region Definitions

        #region Filters

        public bool FilterRefreshNeeded { get; set; } = false;
        public int CurrentDefaultFilter { get; set; } = -1;

        #endregion

        #region Entity Collections

        public EventList<Classes.Scene.Sets.EditorEntity> Entities { get; set; } = new EventList<Sets.EditorEntity>();
        public List<Classes.Scene.Sets.EditorEntity> SelectedEntities { get => GetSelectedEntities(); set => SetSelectedEntities(value); }
        public List<Classes.Scene.Sets.EditorEntity> TemporarySelection { get; set; } = new List<Classes.Scene.Sets.EditorEntity>();

        public EventList<Classes.Scene.Sets.EditorEntity> InternalEntities { get; set; } = new EventList<Sets.EditorEntity>();
        public List<Classes.Scene.Sets.EditorEntity> SelectedInternalEntities { get => GetSelectedInternalEntities(); set => SetSelectedInternalEntities(value); }
        public List<Classes.Scene.Sets.EditorEntity> InternalTempSelection { get; set; } = new List<Classes.Scene.Sets.EditorEntity>();

        #region Accessors

        private void SetSelectedInternalEntities(IList<Classes.Scene.Sets.EditorEntity> SelectedObj)
        {
            List<Classes.Scene.Sets.EditorEntity> SortedList = InternalEntities.OrderBy(x => x.SelectedIndex).ToList();
            foreach (var entity in SortedList.Where(X => SelectedObj.Contains(X)).ToList())
            {
                InternalEntities.Where(x => x == entity).FirstOrDefault().Selected = entity.Selected;
            }
        }
        private List<Classes.Scene.Sets.EditorEntity> GetSelectedInternalEntities()
        {
            return InternalEntities.Where(x => x.Selected == true).ToList();
        }
        private void SetSelectedEntities(IList<Classes.Scene.Sets.EditorEntity> SelectedObj)
        {
            foreach (var entity in SelectedObj)
            {
                Entities[entity.SlotID].Selected = entity.Selected;
            }
        }
        private List<Classes.Scene.Sets.EditorEntity> GetSelectedEntities()
        {
            return Entities.Where(x => x.Selected == true).ToList();
        }

        #endregion

        #endregion

        #region Actions

        public Actions.IAction LastAction;
        public Actions.IAction LastActionInternal;
        public Action<IAction> SlotIDSwapped;

        #endregion

        #region Scene Status
        ushort NextFreeSlot { get; set; } = 0;
        public string SetupObject { get => GetSetupObject(SourceScene); }
        private RSDKv5.Scene SourceScene { get; set; }

        #region Accesors
        public string GetSetupObject(RSDKv5.Scene scene)
        {
            try
            {
                var objectList = GetObjects(Methods.Editor.Solution.CurrentScene.Objects);
                string setupObject = objectList.FirstOrDefault(x => x.Contains("Setup"));
                return setupObject;
            }
            catch
            {
                return "";
            }
        }

        #endregion

        #endregion

        #region Exceptions

        public class TooManyEntitiesException : Exception { }

        #endregion

        #region Splines

        private Dictionary<int, List<float>> SplineXPos { get; set; } = new Dictionary<int, List<float>>();
        private Dictionary<int, List<float>> SplineYPos { get; set; } = new Dictionary<int, List<float>>();

        #endregion

        #endregion

        #region Init

        public EditorEntities(RSDKv5.Scene scene)
        {
            Entities.ItemAdded += UpdateSlotIDOrder;
            Entities.ItemRemoved += UpdateSlotIDOrder;

            InternalEntities.ItemAdded += UpdateSlotIDOrder;
            InternalEntities.ItemRemoved += UpdateSlotIDOrder;

            foreach (var obj in scene.Objects)
            {
                foreach (var entity in obj.Entities.Select(x => GenerateEditorEntity(x)))
                {
                    Entities.Add(entity);
                }
            }
            SourceScene = scene;
        }

        #endregion

        #region Selection
        public bool IsAnythingSelected()
        {
            return SelectedEntities.Count > 0 || TemporarySelection.Count > 0;
        }
        public void SelectAll()
        {
            foreach (var entity in Entities)
            {
                entity.Selected = true;
            }

            foreach (var entity in InternalEntities)
            {
                entity.Selected = true;
            }

        }
        public void Select(Rectangle area, bool addSelection = false, bool deselectIfSelected = false)
        {
            if (!addSelection) Deselect();
            SelectNormal();
            SelectInternal();

            void SelectNormal()
            {
                foreach (var entity in Entities)
                {
                    if (entity.IsInArea(area))
                    {
                        if (deselectIfSelected)
                        {
                            if (entity.Selected)
                            {
                                SelectedEntities.Remove(entity);
                                entity.Selected = false;
                            }
                            else
                            {
                                entity.Selected = true;
                            }
                        }
                        else
                        {
                            SelectedEntities.Add(entity);
                            entity.Selected = true;
                        }
                    }
                }
            }
            void SelectInternal()
            {
                foreach (var entity in InternalEntities)
                {
                    if (entity.IsInArea(area))
                    {
                        if (deselectIfSelected)
                        {
                            if (entity.Selected)
                            {
                                SelectedInternalEntities.Remove(entity);
                                entity.Selected = false;
                            }
                            else
                            {
                                entity.Selected = true;
                            }
                        }
                        else
                        {
                            SelectedInternalEntities.Add(entity);
                            entity.Selected = true;
                        }
                    }
                }
            }

        }
        public void Select(Point point, bool addSelection = false, bool deselectIfSelected = false)
        {
            if (!addSelection) Deselect();
            // In reverse because we want to select the top one
            SelectNormal();
            SelectInternal();


            void SelectNormal()
            {
                foreach (Classes.Scene.Sets.EditorEntity entity in Entities.ToList().Reverse<Classes.Scene.Sets.EditorEntity>())
                {
                    if (entity.ContainsPoint(point))
                    {
                        if (deselectIfSelected && SelectedEntities.Contains(entity))
                        {
                            SelectedEntities.Remove(entity);
                            entity.Selected = false;
                        }
                        else
                        {
                            SelectedEntities.Add(entity);
                            entity.Selected = true;
                        }
                        // Only the top
                        break;
                    }
                }
            }
            void SelectInternal()
            {
                foreach (Classes.Scene.Sets.EditorEntity entity in InternalEntities.ToList().Reverse<Classes.Scene.Sets.EditorEntity>())
                {
                    if (entity.ContainsPoint(point))
                    {
                        if (deselectIfSelected && SelectedInternalEntities.Contains(entity))
                        {
                            SelectedInternalEntities.Remove(entity);
                            entity.Selected = false;
                        }
                        else
                        {
                            SelectedInternalEntities.Add(entity);
                            entity.Selected = true;
                        }
                        // Only the top
                        break;
                    }
                }
            }
        }
        public void TempSelection(Rectangle area, bool deselectIfSelected)
        {
            TempSelectionUsingList(Entities.ToList());
            TempSelectionUsingList(InternalEntities.ToList());

            void TempSelectionUsingList(List<Classes.Scene.Sets.EditorEntity> list)
            {
                List<Classes.Scene.Sets.EditorEntity> newSelection = (from entity in list where entity.IsInArea(area) select entity).ToList();
                List<Classes.Scene.Sets.EditorEntity> outsideSelection = (from entity in list where (!entity.IsInArea(area) && entity.InTempSelection) select entity).ToList();

                foreach (var entity in outsideSelection)
                {
                    entity.InTempSelection = false;
                }

                foreach (var entity in newSelection)
                {
                    entity.InTempSelection = true;
                    entity.TempSelected = !entity.Selected || !deselectIfSelected;
                }

                TemporarySelection = newSelection.Where(x => x.InTempSelection == true).ToList();
            }
        }
        public void SelectSlot(int slot)
        {
            Deselect();
            if (Entities.Exists(x => x.SlotID == (ushort)slot))
            {
                SelectedEntities.Add(Entities[(ushort)slot]);
                Entities[(ushort)slot].Selected = true;
            }
        }
        public void Deselect()
        {
            DeselectNormal();
            DeselectInternal();

            void DeselectNormal()
            {
                foreach (var entity in Entities)
                {
                    entity.Selected = false;
                    entity.InTempSelection = false;
                    entity.TempSelected = false;
                }
                SelectedEntities.Clear();
            }

            void DeselectInternal()
            {
                foreach (var entity in InternalEntities)
                {
                    entity.Selected = false;
                    entity.InTempSelection = false;
                    entity.TempSelected = false;
                }
                SelectedInternalEntities.Clear();
            }
        }
        public void EndTempSelection()
        {
            EndTempNormal();
            EndTempInternal();

            void EndTempNormal()
            {
                foreach (var entity in TemporarySelection)
                {
                    entity.Selected = entity.TempSelected;
                    entity.InTempSelection = false;
                    entity.TempSelected = false;

                }
                TemporarySelection.Clear();
            }

            void EndTempInternal()
            {
                foreach (var entity in InternalTempSelection)
                {
                    entity.Selected = entity.TempSelected;
                    entity.InTempSelection = false;
                    entity.TempSelected = false;

                }
                InternalTempSelection.Clear();
            }
        }
        public void UpdateSelectedIndexForEntities()
        {
            int index = 0;
            foreach (var entity in SelectedEntities.OrderBy(x => x.TimeWhenSelected))
            {
                entity.SelectedIndex = index;
                index++;
            }
        }

        #endregion

        #region Add Entities

        /// <summary>
        /// Adds an entity to the Scene, and consumes the specified ID Slot.
        /// </summary>
        /// <param name="entity">Entity to add to the scene.</param>
        private void AddEntity(Classes.Scene.Sets.EditorEntity entity)
        {
            entity.Entity.Object.Entities.Add(entity.Entity);
            this.Entities.Add(entity);
        }
        private void AddInternalEntity(Classes.Scene.Sets.EditorEntity entity)
        {
            entity.Entity.Object.Entities.Add(entity.Entity);
            this.InternalEntities.Add(entity);
        }

        /// <summary>
        /// Adds an entity to the editor's own memory.
        /// </summary>
        /// <param name="entity">Entity to add to the editor's memory.</param>
        /// <summary>
        /// Adds a set of entities to the Scene, and consumes the ID Slot specified for each.
        /// </summary>
        /// <param name="entities">Set of entities.</param>
        private void AddEntities(IEnumerable<Classes.Scene.Sets.EditorEntity> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            foreach (var entity in entities)
            {
                AddEntity(entity);
            }
        }
        private void AddEntitiesInternal(IEnumerable<Classes.Scene.Sets.EditorEntity> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            foreach (var entity in entities)
            {
                AddInternalEntity(entity);
            }
        }

        /// <summary>
        /// Creates a new instance of the given SceneObject at the indicated position.
        /// </summary>
        /// <param name="sceneObject">Type of SceneObject to create an instance of.</param>
        /// <param name="position">Location to insert into the scene.</param>
        public void Add(RSDKv5.SceneObject sceneObject, RSDKv5.Position position)
        {
            var editorEntity = GenerateEditorEntity(new RSDKv5.SceneEntity(sceneObject, GetFreeSlot(null)));
            editorEntity.Entity.Position = position;
            var newEntities = new List<Classes.Scene.Sets.EditorEntity> { editorEntity };
            LastAction = new Actions.ActionAddDeleteEntities(newEntities, true, x => AddEntities(x), x => DeleteEntities(x));
            AddEntities(newEntities);
            Deselect();
            editorEntity.Selected = true;
            SelectedEntities.Add(editorEntity);
        }

        /// <summary>
        /// Creates new instances of the given SceneObject at the indicated positions.
        /// </summary>
        /// <param name="sceneObjects">A Dictonary containing the type of SceneObjects to create instances of & their locations to insert them into the scene.</param>
        public void Add(List<KeyValuePair<RSDKv5.SceneObject, RSDKv5.Position>> sceneObjects)
        {
            var newEntities = new List<Classes.Scene.Sets.EditorEntity> { };
            foreach (var keyPair in sceneObjects)
            {
                var sceneObject = keyPair.Key;
                var position = keyPair.Value;
                var editorEntity = GenerateEditorEntity(new RSDKv5.SceneEntity(sceneObject, GetFreeSlot(null)));
                editorEntity.Entity.Position = position;
                newEntities.Add(editorEntity);
            }


            LastAction = new Actions.ActionAddDeleteEntities(newEntities, true, x => AddEntities(x), x => DeleteEntities(x));
            AddEntities(newEntities);
            Deselect();
            foreach (var editorEntity in newEntities)
            {
                editorEntity.Selected = true;
                SelectedEntities.Add(editorEntity);
            }

        }

        #endregion

        #region Remove Entities
        private void DeleteEntities(List<Classes.Scene.Sets.EditorEntity> entities, bool updateActions = false)
        {
            if (updateActions) LastAction = new Actions.ActionAddDeleteEntities(entities.ToList(), false, x => AddEntities(x), x => DeleteEntities(x));
            foreach (var entity in entities)
            {
                entity.Entity.Object.Entities.Remove(entity.Entity);
                this.Entities.Remove(entity);
                if (entity.SlotID < NextFreeSlot) NextFreeSlot = entity.SlotID;

            }
        }
        public void DeleteInternalEntities(List<Classes.Scene.Sets.EditorEntity> entities, bool updateActions = false)
        {
            if (updateActions) LastActionInternal = new Actions.ActionAddDeleteEntities(entities.ToList(), false, x => AddEntitiesInternal(x), x => DeleteInternalEntities(x));
            foreach (var entity in entities)
            {
                entity.Entity.Object.Entities.Remove(entity.Entity);
                this.InternalEntities.Remove(entity);

            }
        }
        public void DeleteSelected()
        {
            if (SelectedEntities.Count > 0)
                LastAction = new Actions.ActionAddDeleteEntities(SelectedEntities.ToList(), false, x => AddEntities(x), x => DeleteEntities(x));
            DeleteEntities(SelectedEntities.ToList());
            Deselect();
        }
        public void DeleteInternallySelected()
        {
            if (SelectedInternalEntities.Count > 0)
                LastActionInternal = new Actions.ActionAddDeleteEntities(SelectedInternalEntities.ToList(), false, x => AddEntitiesInternal(x), x => DeleteInternalEntities(x));
            DeleteInternalEntities(SelectedInternalEntities.ToList());
            Deselect();
        }
        #endregion

        #region Pinpoint Entity

        public Classes.Scene.Sets.EditorEntity GetEntityAt(Point point)
        {
            Classes.Scene.Sets.EditorEntity result1 = GetEntityAtNormal();
            Classes.Scene.Sets.EditorEntity result2 = GetEntityAtInternal();
            return (result1 != null ? result1 : result2);



            Classes.Scene.Sets.EditorEntity GetEntityAtNormal()
            {
                foreach (Classes.Scene.Sets.EditorEntity entity in Entities.ToList().Reverse<Classes.Scene.Sets.EditorEntity>())
                {
                    if (entity.ContainsPoint(point))
                        return entity;
                }
                return null;
            }

            Classes.Scene.Sets.EditorEntity GetEntityAtInternal()
            {
                foreach (Classes.Scene.Sets.EditorEntity entity in InternalEntities.ToList().Reverse<Classes.Scene.Sets.EditorEntity>())
                {
                    if (entity.ContainsPoint(point))
                        return entity;
                }
                return null;
            }
        }
        public bool IsEntityAt(Point point, bool GridMode = false)
        {
            bool result1 = GetIsEntityAtWithList(Entities.ToList());
            bool result2 = GetIsEntityAtWithList(InternalEntities.ToList());
            return (result1 != false ? result1 : result2);

            bool GetIsEntityAtWithList(List<Classes.Scene.Sets.EditorEntity> list)
            {
                foreach (Classes.Scene.Sets.EditorEntity entity in list.Reverse<Classes.Scene.Sets.EditorEntity>())
                {
                    if (entity.ContainsPoint(point))
                        return true;
                }
                return false;
            }
        }

        #endregion

        #region Object Generation

        public Classes.Scene.Sets.EditorEntity GenerateSplineObject()
        {
            NameIdentifier Name = new NameIdentifier("Spline");
            List<AttributeInfo> Attributes = new List<AttributeInfo>();
            AttributeInfo SplineID = new AttributeInfo("SplineID", AttributeTypes.INT32);
            Attributes.Add(SplineID);
            ushort Slot = 0;
            SceneEntity Entity = new SceneEntity(new SceneObject(Name, Attributes), Slot);
            Entity.attributesMap["SplineID"].ValueInt32 = ManiacEditor.Controls.Editor.MainEditor.Instance.EditorToolbar.SplineSpawnID.Value.Value;

            return new Classes.Scene.Sets.EditorEntity(Entity, true);
        }
        public void SpawnInternalSplineObject(RSDKv5.Position position)
        {
            var editorEntity = GenerateSplineObject();
            editorEntity.SlotID = GetFreeSlotInternal(null);
            editorEntity.Entity.Position = position;
            var newEntities = new List<Classes.Scene.Sets.EditorEntity> { editorEntity };
            LastActionInternal = new Actions.ActionAddDeleteEntities(newEntities, true, x => AddEntitiesInternal(x), x => DeleteInternalEntities(x));
            AddEntitiesInternal(newEntities);
            Deselect();
            editorEntity.Selected = true;
            SelectedInternalEntities.Add(editorEntity);
        }
        public Classes.Scene.Sets.EditorEntity GenerateEditorEntity(RSDKv5.SceneEntity sceneEntity)
        {
            Classes.Scene.Sets.EditorEntity entity = new Classes.Scene.Sets.EditorEntity(sceneEntity);

            if (entity.HasFilter() && CurrentDefaultFilter > -1)
            {
                entity.Entity.GetAttribute("filter").ValueUInt8 = (byte)CurrentDefaultFilter;
                CurrentDefaultFilter = -1;
            }

            entity.SetFilter();

            return entity;
        }

        #endregion

        #region Set/Get Methods

        public Classes.Scene.Sets.EditorEntity GetSelectedEntity()
        {
            if (SelectedEntities.Count > 0 && SelectedEntities[0] != null) return SelectedEntities[0];
            else return SelectedInternalEntities[0];
        }

        public List<string> GetObjects(List<RSDKv5.SceneObject> sceneObjects)
        {
            sceneObjects.Sort((x, y) => x.Name.ToString().CompareTo(y.Name.ToString()));
            List<string> strings = sceneObjects.Select(s => s.Name.Name).ToList();
            return strings;
        }

        private ushort GetFreeSlot(RSDKv5.SceneEntity preferred)
        {
            if (preferred != null && !Entities.Exists(x => x.SlotID == preferred.SlotID)) return preferred.SlotID;
            while (Entities.Exists(x => x.SlotID == NextFreeSlot))
            {
                ++NextFreeSlot;
            }
            if (NextFreeSlot == 2048)
            {
                if (Entities.Count < 2048)
                {
                    // Next time search from beggining
                    NextFreeSlot = 0;
                }
                throw new TooManyEntitiesException();
            }
            return NextFreeSlot++;
        }

        private ushort GetFreeSlotInternal(RSDKv5.SceneEntity preferred)
        {
            if (preferred != null && !InternalEntities.Exists(x => x.SlotID == preferred.SlotID)) return preferred.SlotID;
            while (InternalEntities.Exists(x => x.SlotID == NextFreeSlot))
            {
                ++NextFreeSlot;
            }
            if (NextFreeSlot == 2048)
            {
                if (InternalEntities.Count < 2048)
                {
                    // Next time search from beggining
                    NextFreeSlot = 0;
                }
                throw new TooManyEntitiesException();
            }
            return NextFreeSlot++;
        }

        public ushort GetRealSlotID(RSDKv5.SceneEntity CurrentObject)
        {
            return (ushort)Entities.IndexOf(Entities.Where(x => x.Entity == CurrentObject).FirstOrDefault());
        }

        #endregion

        #region Duplication Entities
        private void DuplicateEntities(List<Classes.Scene.Sets.EditorEntity> entities)
        {
            if (null == entities || !entities.Any()) return;

            // work out a slot for each entity, and add it in turn
            // this should prevent generating the same ID for each member of the list
            var new_entities = new List<Classes.Scene.Sets.EditorEntity>();
            foreach (var entity in entities)
            {
                ushort slot = GetFreeSlot(entity.Entity);

                SceneEntity sceneEntity;
                // If this is pasted from another Scene, we need to reassign its Object
                if (entity.IsExternal())
                    sceneEntity = SceneEntity.FromExternal(entity.Entity, Methods.Editor.Solution.CurrentScene.Objects, slot);
                // If it's from this Scene, we can use the existing Object
                else
                    sceneEntity = new SceneEntity(entity.Entity, slot);

                // Make sure it was created properly
                if (sceneEntity != null)
                {
                    var editorEntity = GenerateEditorEntity(sceneEntity);
                    AddEntity(editorEntity);
                    new_entities.Add(editorEntity);
                }
            }
            if (new_entities.Count > 0)
            {
                LastAction = new Actions.ActionAddDeleteEntities(new_entities.ToList(), true, x => AddEntities(x), x => DeleteEntities(x));
            }

            Deselect();
            foreach (var entity in new_entities)
            {
                SelectedEntities.Add(entity);
            }
            foreach (var entity in new_entities)
                entity.Selected = true;
        }
        private void DuplicateEntitiesInternal(List<Classes.Scene.Sets.EditorEntity> entities)
        {
            if (null == entities || !entities.Any()) return;

            // work out a slot for each entity, and add it in turn
            // this should prevent generating the same ID for each member of the list
            var new_entities = new List<Classes.Scene.Sets.EditorEntity>();
            foreach (var entity in entities)
            {
                ushort slot = GetFreeSlotInternal(entity.Entity);

                SceneEntity sceneEntity;
                // If this is pasted from another Scene, we need to reassign its Object
                if (entity.IsExternal())
                    sceneEntity = SceneEntity.FromExternal(entity.Entity, Methods.Editor.Solution.CurrentScene.Objects, slot);
                // If it's from this Scene, we can use the existing Object
                else
                    sceneEntity = new SceneEntity(entity.Entity, slot);

                // Make sure it was created properly
                if (sceneEntity != null)
                {
                    var editorEntity = GenerateEditorEntity(sceneEntity);
                    AddInternalEntity(editorEntity);
                    new_entities.Add(editorEntity);
                }
            }
            if (new_entities.Count > 0)
            {
                LastActionInternal = new Actions.ActionAddDeleteEntities(new_entities.ToList(), true, x => AddEntitiesInternal(x), x => DeleteInternalEntities(x));
            }

            Deselect();
            foreach (var entity in new_entities)
            {
                SelectedInternalEntities.Add(entity);
            }
            foreach (var entity in new_entities)
                entity.Selected = true;
        }

        #endregion

        #region Clipboard
        public List<Classes.Scene.Sets.EditorEntity> CopyToClipboard(bool keepPosition = false)
        {
            if (SelectedEntities.Count == 0) return null;
            short minX = 0, minY = 0;

            List<Classes.Scene.Sets.EditorEntity> copiedEntities = SelectedEntities.Select(x => GenerateEditorEntity(new RSDKv5.SceneEntity(x.Entity, x.SlotID))).ToList();
            if (!keepPosition)
            {
                minX = copiedEntities.Min(x => x.Entity.Position.X.High);
                minY = copiedEntities.Min(x => x.Entity.Position.Y.High);
                copiedEntities.ForEach(x => x.Move(new Point(-minX, -minY)));
            }

            return copiedEntities;
        }
        public void PasteFromClipboard(Point newPos, List<Classes.Scene.Sets.EditorEntity> entities)
        {
            DuplicateEntities(entities);
            foreach (var entity in SelectedEntities)
            {
                // Move them
                entity.Move(newPos);
            }
        }
        #endregion

        #region Movement + Interactions

        public void MoveSelected(Point oldPos, Point newPos, bool duplicate)
        {
            Point diff = new Point(newPos.X - oldPos.X, newPos.Y - oldPos.Y);

            if (duplicate)
            {
                if (SelectedEntities.Count > 0) DuplicateEntities(SelectedEntities.ToList());
                if (SelectedInternalEntities.Count > 0) DuplicateEntitiesInternal(SelectedInternalEntities.ToList());
            }

            if (SelectedEntities.Count > 0)
            {
                foreach (var entity in SelectedEntities)
                {
                    entity.Move(diff);
                }
            }
            if (SelectedInternalEntities.Count > 0)
            {
                foreach (var entity in SelectedInternalEntities)
                {
                    entity.Move(diff);
                }
            }




        }
        internal void Flip(FlipDirection direction)
        {
            var positions = SelectedEntities.Select(se => se.Entity.Position);
            IEnumerable<Position.Value> monoCoordinatePositions;
            if (direction == FlipDirection.Horizontal)
            {
                monoCoordinatePositions = positions.Select(p => p.X);
            }
            else
            {
                monoCoordinatePositions = positions.Select(p => p.Y);
            }

            short min = monoCoordinatePositions.Min(m => m.High);
            short max = monoCoordinatePositions.Max(m => m.High);
            int diff = max - min;

            foreach (var entity in SelectedEntities)
            {
                if (direction == FlipDirection.Horizontal)
                {
                    short xPos = entity.Entity.Position.X.High;
                    int fromLeft = xPos - min;
                    int fromRight = max - xPos;

                    int newX = fromLeft < fromRight ? max - fromLeft : min + fromRight;
                    entity.Entity.Position.X.High = (short)newX;
                }
                else
                {
                    short yPos = entity.Entity.Position.Y.High;
                    int fromBottom = yPos - min;
                    int fromTop = max - yPos;

                    int newY = fromBottom < fromTop ? max - fromBottom : min + fromTop;
                    entity.Entity.Position.Y.High = (short)newY;
                }

                entity.Flip(direction);
            }
        }

        #endregion

        #region Slot ID Stuff
        private void UpdateSlotIDOrder(object sender, EventListArgs<Sets.EditorEntity> e)
        {
            foreach (var item in Entities)
            {
                item.SlotID = (ushort)Entities.IndexOf(item);
            }
            foreach (var item in InternalEntities)
            {
                item.SlotID = (ushort)InternalEntities.IndexOf(item);
            }
            Methods.Internal.UserInterface.UpdateEntitiesToolbarList();
        }
        public List<int> GetAllUsedSlotIDs()
        {
            return Entities.Select(x => (int)x.SlotID).ToList();
        }
        public void OrderSelectedSlotIDs(bool optimize = false, bool ordered = false)
        {
            if (SelectedEntities == null || SelectedEntities.Count < 0) return;

            IList<ushort> RangedSlotIDs = new List<ushort>();
            bool RangedSlotIDsFound = false;
            int RangedIndex = 0;

            IList<SceneEntity> OrderedEntities = new List<SceneEntity>();
            IList<ushort> OrderedSlotIDs = new List<ushort>();
            IList<ushort> UnorderedSlotIDs = new List<ushort>();
            if (ordered)
            {
                var range = Enumerable.Range(0, 2048);
                var a = range.Except(GetAllUsedSlotIDs()).ToList();
                var b = Extensions.Extensions.GroupConsecutive(a);
                int amountOfSlotsNeeded = SelectedEntities.Count();
                foreach (var list in b)
                {
                    if (list.Count() >= amountOfSlotsNeeded)
                    {
                        RangedSlotIDsFound = true;
                        RangedSlotIDs = list.Select(i => (ushort)i).ToList().GetRange(0, amountOfSlotsNeeded);
                        break;
                    }
                }


                if (!RangedSlotIDsFound)
                {
                    MessageBox.Show(string.Format("Unable to Find an avaliable range that can fit {0} Entities!", amountOfSlotsNeeded));
                    return;
                }
            }
            foreach (var entity in SelectedEntities.OrderBy(x => x.SelectedIndex))
            {
                OrderedEntities.Add(entity.Entity);
            }

            foreach (var entity in SelectedEntities.OrderBy(x => x.SlotID))
            {
                if (optimize) OrderedSlotIDs.Add(GetRealSlotID(entity.Entity));
                else if (ordered)
                {
                    OrderedSlotIDs.Add(RangedSlotIDs[RangedIndex]);
                    RangedIndex++;
                }
                else OrderedSlotIDs.Add(entity.SlotID);
            }

            foreach (var entity in OrderedEntities)
            {
                UnorderedSlotIDs.Add(entity.SlotID);
            }
            IAction action = new Actions.ActionSortSlotIDs(OrderedEntities, OrderedSlotIDs, UnorderedSlotIDs, new Action<IList<SceneEntity>, IList<ushort>>(ChangeSeveralSlotIDs));
            SlotIDSwapped?.Invoke(action);
            ChangeSeveralSlotIDs(OrderedEntities, OrderedSlotIDs);

            ManiacEditor.Controls.Editor.MainEditor.Instance.UndoStack.Push(action);
            ManiacEditor.Controls.Editor.MainEditor.Instance.RedoStack.Clear();
            Methods.Internal.UserInterface.UpdateControls();

        }
        public void SwapSlotIDsFromPair()
        {
            Classes.Scene.Sets.EditorEntity entity1 = SelectedEntities[0];
            Classes.Scene.Sets.EditorEntity entity2 = SelectedEntities[1];
            ushort slotID_A = entity1.SlotID;
            ushort slotID_B = entity2.SlotID;
            IAction action = new Actions.ActionSwapSlotIDs(entity1.Entity, entity2.Entity, slotID_A, slotID_B, new Action<SceneEntity, SceneEntity, ushort, ushort>(SwapSlotIDs));
            SlotIDSwapped?.Invoke(action);
            SwapSlotIDs(entity1.Entity, entity2.Entity, slotID_A, slotID_B);

            ManiacEditor.Controls.Editor.MainEditor.Instance.UndoStack.Push(action);
            ManiacEditor.Controls.Editor.MainEditor.Instance.RedoStack.Clear();
            Methods.Internal.UserInterface.UpdateControls();
        }
        public void ChangeSeveralSlotIDs(IList<SceneEntity> entities, IList<ushort> slots)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                entities[i].SlotID = slots[i];
            }
        }
        public void SwapSlotIDs(SceneEntity A, SceneEntity B, ushort slotA, ushort slotB)
        {
            A.SlotID = slotB;
            B.SlotID = slotA;
        }
        public void OptimizeAllSlotIDs()
        {
            IList<SceneEntity> OrderedEntities = new List<SceneEntity>();
            IList<ushort> OrderedSlotIDs = new List<ushort>();
            IList<ushort> UnorderedSlotIDs = new List<ushort>();
            foreach (var entity in Entities)
            {
                OrderedEntities.Add(entity.Entity);
                OrderedSlotIDs.Add(GetRealSlotID(entity.Entity));
                UnorderedSlotIDs.Add(entity.SlotID);
            }

            IAction action = new Actions.ActionSortSlotIDs(OrderedEntities, OrderedSlotIDs, UnorderedSlotIDs, new Action<IList<SceneEntity>, IList<ushort>>(ChangeSeveralSlotIDs));
            SlotIDSwapped?.Invoke(action);
            ChangeSeveralSlotIDs(OrderedEntities, OrderedSlotIDs);

            ManiacEditor.Controls.Editor.MainEditor.Instance.UndoStack.Push(action);
            ManiacEditor.Controls.Editor.MainEditor.Instance.RedoStack.Clear();
            Methods.Internal.UserInterface.UpdateControls();
        }
        #endregion

        #region Filters
        public void UpdateViewFilters()
        {
            FilterRefreshNeeded = false;
            foreach (var obj in Entities)
            {
                obj.SetFilter();
            }
        }
        #endregion

        #region Drawing
        public void Draw(Graphics g)
        {
            foreach (var entity in Entities)
            {
                entity.Draw(new Methods.Draw.GraphicsHandler(g));
            }
        }
        public void Draw(DevicePanel d)
        {
            if (FilterRefreshNeeded)
                UpdateViewFilters();
            foreach (var entity in Entities)
            {
                if (entity.IsObjectOnScreen(new Methods.Draw.GraphicsHandler(d))) entity.Draw(d);
            }
        }
        public void DrawInternalObjects(DevicePanel d)
        {
            if (FilterRefreshNeeded)
                UpdateViewFilters();


            foreach (var entity in InternalEntities)
            {
                if (entity.IsObjectOnScreen(new Methods.Draw.GraphicsHandler(d))) entity.DrawInternal(d);
                if (entity.Name == "Spline")
                {
                    int id = entity.Entity.attributesMap["SplineID"].ValueInt32;
                    if (!Methods.Editor.SolutionState.SplineOptionsGroup.ContainsKey(id)) Methods.Editor.SolutionState.AddSplineOptionsGroup(id);
                    if (SplineXPos.ContainsKey(id))
                    {
                        SplineXPos[id].Add(entity.Entity.Position.X.High);
                        SplineYPos[id].Add(entity.Entity.Position.Y.High);
                    }
                    else
                    {
                        SplineXPos.Add(id, new List<float>() { entity.Entity.Position.X.High });
                        SplineYPos.Add(id, new List<float>() { entity.Entity.Position.Y.High });
                    }
                }
            }

            foreach (var path in SplineXPos)
            {
                int CurrentSplineTotalNumberOfObjects = 0;
                int CurrentNumberOfObjectsRendered = 0;
                int splineID = path.Key;
                Methods.Editor.SolutionState.SplineOptions selectedOptions = Methods.Editor.SolutionState.SplineOptionsGroup[splineID];
                CurrentSplineTotalNumberOfObjects = SplineXPos[splineID].Count;
                if (SplineXPos[splineID].Count > 1)
                {
                    CurrentSplineTotalNumberOfObjects++;
                    float[] xs, ys;
                    Extensions.Spline.CubicSpline.FitParametric(SplineXPos[splineID].ToArray(), SplineYPos[splineID].ToArray(), (selectedOptions.SplineSize > 0 ? selectedOptions.SplineSize : 1), out xs, out ys);
                    Point lastPoint = new Point(-1, -1);
                    foreach (var p in Extensions.Extensions.CreateDataPoints(xs, ys))
                    {
                        CurrentNumberOfObjectsRendered++;
                        if (lastPoint.X != -1)
                        {
                            if (selectedOptions.SplineToolShowLines) d.DrawLine(p.X, p.Y, lastPoint.X, lastPoint.Y, System.Drawing.Color.Red);
                        }
                        if (selectedOptions.SplineToolShowPoints) d.DrawRectangle(p.X, p.Y, p.X + 2, p.Y + 2, System.Drawing.Color.Red);
                        if (selectedOptions.SplineToolShowObject && selectedOptions.SplineObjectRenderingTemplate != null)
                        {
                            if (Methods.Entities.EntityDrawing.RenderingSettings.ObjectToRender.Contains(selectedOptions.SplineObjectRenderingTemplate.Entity.Object.Name.Name))
                            {
                                ManiacEditor.Controls.Editor.MainEditor.Instance.EntityDrawing.DrawOthers(new Methods.Draw.GraphicsHandler(d), selectedOptions.SplineObjectRenderingTemplate.Entity, selectedOptions.SplineObjectRenderingTemplate, p.X, p.Y, 0, 0, 0, selectedOptions.SplineObjectRenderingTemplate.EditorAnimations, selectedOptions.SplineObjectRenderingTemplate.Selected, true);
                            }
                            else
                            {
                                selectedOptions.SplineObjectRenderingTemplate.FallbackDraw(new Methods.Draw.GraphicsHandler(d), p.X, p.Y, 0, 0, 255, System.Drawing.Color.Transparent, true);
                            }
                        }
                        lastPoint = new Point(p.X, p.Y);
                    }
                }
                SplineXPos[path.Key].Clear();
                SplineYPos[path.Key].Clear();

                Methods.Editor.SolutionState.SplineOptionsGroup[splineID].SplineTotalNumberOfObjects = CurrentSplineTotalNumberOfObjects - 1;
                Methods.Editor.SolutionState.SplineOptionsGroup[splineID].SplineNumberOfObjectsRendered = CurrentNumberOfObjectsRendered;
            }

            Methods.Internal.UserInterface.UpdateSplineToolbox();



        }
        public void DrawPriority(DevicePanel d, int prority)
        {
            if (FilterRefreshNeeded)
                UpdateViewFilters();
            foreach (var entity in Entities)
            {
                if (entity.ValidPriorityPlane(prority) && entity.IsObjectOnScreen(new Methods.Draw.GraphicsHandler(d))) entity.Draw(d);
            }
        }
        public void DrawSelectionBoxes(DevicePanel d)
        {
            foreach (var entity in Entities)
            {
                if (entity.IsObjectOnScreen(new Methods.Draw.GraphicsHandler(d))) entity.DrawBoxOnly(d);
            }
        }
        #endregion
    }
}
