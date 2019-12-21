﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using RSDKv5;
using ManiacEditor.Actions;

namespace ManiacEditor
{
    public class EditorEntities : IDrawable
    {
        #region Definitions
        #region Filters
        public bool FilterRefreshNeeded = false;
        public int DefaultFilter = -1;
        #endregion

        #region Scene Entities
        private List<EditorEntity> _Entities = new List<EditorEntity>();
        private SortedDictionary<ushort, EditorEntity> _EntitiesBySlot = new SortedDictionary<ushort, EditorEntity>();
        public List<EditorEntity> Entities { get => GetEntities(); set => SetEntities(value); }
        public IList<EditorEntity> SelectedEntities { get => GetSelectedEntities(); set => SetSelectedEntities(value); }
        public SortedDictionary<ushort, EditorEntity> EntitiesBySlot { get => GetSortedEntities(); }
        public IList<EditorEntity> TemporarySelection = new List<EditorEntity>();
        #endregion

        #region Internal Entities
        private SortedDictionary<ushort, EditorEntity> _InternalEntitiesBySlot = new SortedDictionary<ushort, EditorEntity>();

        private List<EditorEntity> _InternalEntities = new List<EditorEntity>();
        public List<EditorEntity> InternalEntities { get => GetInternalEntities(); set => SetInternalEntities(value); }
        public IList<EditorEntity> SelectedInternalEntities { get => GetSelectedInternalEntities(); set => SetSelectedInternalEntities(value); }

        public IList<EditorEntity> InternalTempSelection = new List<EditorEntity>();
        public SortedDictionary<ushort, EditorEntity> InternalEntitiesBySlot { get => GetSortedEntities(); }
        #endregion

        #region Other

        ushort nextFreeSlot = 0;
        public class TooManyEntitiesException : Exception { }
        public Actions.IAction LastAction;
        public Actions.IAction LastActionInternal;

        public Action<IAction> SlotIDSwapped;
        public string SetupObject { get => GetSetupObject(SourceScene); }
        private Scene SourceScene;

        #endregion
        #endregion

        public EditorEntities(Scene scene)
        {
            foreach (var obj in scene.Objects)
            {
                Entities.AddRange(obj.Entities.Select(x => GenerateEditorEntity(x)));
            }
            FindDuplicateIds();
            GetSortedEntities();
            SourceScene = scene;
        }

        #region Internal Objects

        public EditorEntity GenerateSplineObject()
        {
            NameIdentifier Name = new NameIdentifier("Spline");
            List<AttributeInfo> Attributes = new List<AttributeInfo>();
            AttributeInfo SplineID = new AttributeInfo("SplineID", AttributeTypes.INT32);
            Attributes.Add(SplineID);
            ushort Slot = 0;
            SceneEntity Entity = new SceneEntity(new SceneObject(Name, Attributes), Slot);
            Entity.attributesMap["SplineID"].ValueInt32 = Editor.Instance.SplineSpawnID.Value.Value;

            return new EditorEntity(Entity, true);
        }

        #endregion

        #region Set/Get Methods
        private void SetInternalEntities(List<EditorEntity> Entities)
        {
            _InternalEntities = Entities;
        }

        private List<EditorEntity> GetInternalEntities()
        {
            return _InternalEntities;
        }

        private void SetEntities(List<EditorEntity> Entities)
        {
            _Entities = Entities;
            GetSortedEntities();
        }

        private List<EditorEntity> GetEntities()
        {
            return _Entities;
        }

        private SortedDictionary<ushort, EditorEntity> GetInternalSortedEntities()
        {
            ushort SlotID = 0;
            foreach (var entity in InternalEntities)
            {
                entity.Entity.SlotID = SlotID;
                SlotID++;
            }
            var keyValuePairs = InternalEntities.ToDictionary(x => x.Entity.SlotID);
            _EntitiesBySlot = new SortedDictionary<ushort, EditorEntity>(keyValuePairs);
            return new SortedDictionary<ushort, EditorEntity>(keyValuePairs);
        }

        private SortedDictionary<ushort, EditorEntity> GetSortedEntities()
        {
            var keyValuePairs = Entities.ToDictionary(x => x.Entity.SlotID);
            _EntitiesBySlot = new SortedDictionary<ushort, EditorEntity>(keyValuePairs);
            return new SortedDictionary<ushort, EditorEntity>(keyValuePairs);
        }

        private void SetSelectedInternalEntities(IList<EditorEntity> SelectedObj)
        {
            List<EditorEntity> SortedList = InternalEntities.OrderBy(x => x.SelectedIndex).ToList();
            foreach (var entity in SortedList.Where(X => SelectedObj.Contains(X)).ToList())
            {
                InternalEntities.Where(X => X == entity).FirstOrDefault().Selected = entity.Selected;
            }
        }

        private List<EditorEntity> GetSelectedInternalEntities()
        {
            return InternalEntities.Where(x => x.Selected == true).ToList();
        }

        private void SetSelectedEntities(IList<EditorEntity> SelectedObj)
        {
            List<EditorEntity> SortedList = Entities.OrderBy(x => x.SelectedIndex).ToList();
            foreach (var entity in SortedList.Where(X => SelectedObj.Contains(X)).ToList())
            {
                Entities.Where(X => X == entity).FirstOrDefault().Selected = entity.Selected;
            }
        }

        private List<EditorEntity> GetSelectedEntities()
        {
            return Entities.Where(x => x.Selected == true).ToList();
        }

        public EditorEntity GetSelectedEntity()
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

        public string GetSetupObject(Scene scene)
        {
            try
            {
                var objectList = GetObjects(EditorSolution.CurrentScene.Objects);
                string setupObject = objectList.FirstOrDefault(x => x.Contains("Setup"));
                return setupObject;
            }
            catch
            {
                return "";
            }
        }

        private ushort GetFreeSlot(RSDKv5.SceneEntity preferred)
        {
            if (preferred != null && !EntitiesBySlot.ContainsKey(preferred.SlotID)) return preferred.SlotID;
            while (EntitiesBySlot.ContainsKey(nextFreeSlot))
            {
                ++nextFreeSlot;
            }
            if (nextFreeSlot == 2048)
            {
                if (EntitiesBySlot.Count < 2048)
                {
                    // Next time search from beggining
                    nextFreeSlot = 0;
                }
                throw new TooManyEntitiesException();
            }
            return nextFreeSlot++;
        }

        private ushort GetFreeSlotInternal(RSDKv5.SceneEntity preferred)
        {
            if (preferred != null && !InternalEntitiesBySlot.ContainsKey(preferred.SlotID)) return preferred.SlotID;
            while (InternalEntitiesBySlot.ContainsKey(nextFreeSlot))
            {
                ++nextFreeSlot;
            }
            if (nextFreeSlot == 2048)
            {
                if (InternalEntitiesBySlot.Count < 2048)
                {
                    // Next time search from beggining
                    nextFreeSlot = 0;
                }
                throw new TooManyEntitiesException();
            }
            return nextFreeSlot++;
        }

        public ushort GetRealSlotID(RSDKv5.SceneEntity CurrentObject)
        {
            GetSortedEntities();
            return (ushort)EntitiesBySlot.IndexOf(EntitiesBySlot.Where(x => x.Value.Entity == CurrentObject).FirstOrDefault());
        }

        #endregion

        #region Entity Generation / Removal / Selection

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
                foreach (EditorEntity entity in Entities.Reverse<EditorEntity>())
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
                foreach (EditorEntity entity in InternalEntities.Reverse<EditorEntity>())
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
            TempSelectionUsingList(Entities);
            TempSelectionUsingList(InternalEntities);

            void TempSelectionUsingList(List<EditorEntity> list)
            {
                List<EditorEntity> newSelection = (from entity in list where entity.IsInArea(area) select entity).ToList();
                List<EditorEntity> outsideSelection = (from entity in list where (!entity.IsInArea(area) && entity.InTempSelection) select entity).ToList();

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
            if (EntitiesBySlot.ContainsKey((ushort)slot))
            {
                SelectedEntities.Add(EntitiesBySlot[(ushort)slot]);
                EntitiesBySlot[(ushort)slot].Selected = true;
            }
        }
        /// <summary>
        /// Adds an entity to the Scene, and consumes the specified ID Slot.
        /// </summary>
        /// <param name="entity">Entity to add to the scene.</param>
        private void AddEntity(EditorEntity entity)
        {
            entity.Entity.Object.Entities.Add(entity.Entity);
            this.Entities.Add(entity);
            GetSortedEntities();

        }
        private void AddInternalEntity(EditorEntity entity)
        {
            entity.Entity.Object.Entities.Add(entity.Entity);
            this.InternalEntities.Add(entity);
            GetInternalSortedEntities();
        }

        /// <summary>
        /// Adds an entity to the editor's own memory.
        /// </summary>
        /// <param name="entity">Entity to add to the editor's memory.</param>
        /// <summary>
        /// Adds a set of entities to the Scene, and consumes the ID Slot specified for each.
        /// </summary>
        /// <param name="entities">Set of entities.</param>
        private void AddEntities(IEnumerable<EditorEntity> entities)
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

        private void AddEntitiesInternal(IEnumerable<EditorEntity> entities)
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

        private void DeleteEntities(List<EditorEntity> entities)
        {
            foreach (var entity in entities)
            {
                entity.Entity.Object.Entities.Remove(entity.Entity);
                this.Entities.Remove(entity);
                EntitiesBySlot.Remove(entity.Entity.SlotID);
                if (entity.Entity.SlotID < nextFreeSlot) nextFreeSlot = entity.Entity.SlotID;

            }
        }

        private void DeleteInternalEntities(List<EditorEntity> entities)
        {
            foreach (var entity in entities)
            {
                entity.Entity.Object.Entities.Remove(entity.Entity);
                this.InternalEntities.Remove(entity);
                InternalEntitiesBySlot.Remove(entity.Entity.SlotID);

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
                GetSortedEntities();
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
                GetInternalSortedEntities();
            }
        }
        private void DuplicateEntities(List<EditorEntity> entities)
        {
            if (null == entities || !entities.Any()) return;

            // work out a slot for each entity, and add it in turn
            // this should prevent generating the same ID for each member of the list
            var new_entities = new List<EditorEntity>();
            foreach (var entity in entities)
            {
                ushort slot = GetFreeSlot(entity.Entity);

                SceneEntity sceneEntity;
                // If this is pasted from another Scene, we need to reassign its Object
                if (entity.IsExternal())
                    sceneEntity = SceneEntity.FromExternal(entity.Entity, EditorSolution.CurrentScene.Objects, slot);
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
        private void DuplicateEntitiesInternal(List<EditorEntity> entities)
        {
            if (null == entities || !entities.Any()) return;

            // work out a slot for each entity, and add it in turn
            // this should prevent generating the same ID for each member of the list
            var new_entities = new List<EditorEntity>();
            foreach (var entity in entities)
            {
                ushort slot = GetFreeSlotInternal(entity.Entity);

                SceneEntity sceneEntity;
                // If this is pasted from another Scene, we need to reassign its Object
                if (entity.IsExternal())
                    sceneEntity = SceneEntity.FromExternal(entity.Entity, EditorSolution.CurrentScene.Objects, slot);
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
        /// <summary>
        /// Creates a new instance of the given SceneObject at the indicated position.
        /// </summary>
        /// <param name="sceneObject">Type of SceneObject to create an instance of.</param>
        /// <param name="position">Location to insert into the scene.</param>
        public void Add(RSDKv5.SceneObject sceneObject, RSDKv5.Position position)
        {
            var editorEntity = GenerateEditorEntity(new RSDKv5.SceneEntity(sceneObject, GetFreeSlot(null)));
            editorEntity.Entity.Position = position;
            var newEntities = new List<EditorEntity> { editorEntity };
            LastAction = new Actions.ActionAddDeleteEntities(newEntities, true, x => AddEntities(x), x => DeleteEntities(x));
            AddEntities(newEntities);
            Deselect();
            editorEntity.Selected = true;
            SelectedEntities.Add(editorEntity);
        }

        public void SpawnInternalSplineObject(RSDKv5.Position position)
        {

            var editorEntity = GenerateSplineObject();
            editorEntity.Entity.SlotID = GetFreeSlotInternal(null);
            editorEntity.Entity.Position = position;
            var newEntities = new List<EditorEntity> { editorEntity };
            LastActionInternal = new Actions.ActionAddDeleteEntities(newEntities, true, x => AddEntitiesInternal(x), x => DeleteInternalEntities(x));
            AddEntitiesInternal(newEntities);
            Deselect();
            editorEntity.Selected = true;
            SelectedInternalEntities.Add(editorEntity);
            GetInternalSortedEntities();
        }

        public EditorEntity GenerateEditorEntity(RSDKv5.SceneEntity sceneEntity)
        {
            EditorEntity entity = new EditorEntity(sceneEntity);

            if (entity.HasFilter() && DefaultFilter > -1)
            {
                entity.Entity.GetAttribute("filter").ValueUInt8 = (byte)DefaultFilter;
                DefaultFilter = -1;
            }

            entity.SetFilter();

            return entity;
        }
        public bool IsSelected()
        {
            return SelectedEntities.Count > 0 || TemporarySelection.Count > 0;
        }
        public List<EditorEntity> CopyToClipboard(bool keepPosition = false)
        {
            if (SelectedEntities.Count == 0) return null;
            short minX = 0, minY = 0;

            List<EditorEntity> copiedEntities = SelectedEntities.Select(x => GenerateEditorEntity(new RSDKv5.SceneEntity(x.Entity, x.Entity.SlotID))).ToList();
            if (!keepPosition)
            {
                minX = copiedEntities.Min(x => x.Entity.Position.X.High);
                minY = copiedEntities.Min(x => x.Entity.Position.Y.High);
                copiedEntities.ForEach(x => x.Move(new Point(-minX, -minY)));
            }

            return copiedEntities;
        }
        public void PasteFromClipboard(Point newPos, List<EditorEntity> entities)
        {
            DuplicateEntities(entities);
            foreach (var entity in SelectedEntities)
            {
                // Move them
                entity.Move(newPos);
            }
        }
        public EditorEntity GetEntityAt(Point point)
        {
            EditorEntity result1 = GetEntityAtNormal();
            EditorEntity result2 = GetEntityAtInternal();
            return (result1 != null ? result1 : result2);



            EditorEntity GetEntityAtNormal()
            {
                foreach (EditorEntity entity in Entities.Reverse<EditorEntity>())
                {
                    if (entity.ContainsPoint(point))
                        return entity;
                }
                return null;
            }

            EditorEntity GetEntityAtInternal()
            {
                foreach (EditorEntity entity in InternalEntities.Reverse<EditorEntity>())
                {
                    if (entity.ContainsPoint(point))
                        return entity;
                }
                return null;
            }
        }



        public bool IsEntityAt(Point point, bool GridMode = false)
        {
            bool result1 = GetIsEntityAtWithList(Entities);
            bool result2 = GetIsEntityAtWithList(InternalEntities);
            return (result1 != false ? result1 : result2);

            bool GetIsEntityAtWithList(List<EditorEntity> list)
            {
                foreach (EditorEntity entity in list.Reverse<EditorEntity>())
                {
                    if (entity.ContainsPoint(point))
                        return true;
                }
                return false;
            }
        }

        #endregion

        #region Slot ID Stuff
        public List<int> GetAllUsedSlotIDs()
        {
            return Entities.Select(x => (int)x.Entity.SlotID).ToList();
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
                var b = Extensions.GroupConsecutive(a);
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

            foreach (var entity in SelectedEntities.OrderBy(x => x.Entity.SlotID))
            {
                if (optimize) OrderedSlotIDs.Add(GetRealSlotID(entity.Entity));
                else if (ordered)
                {
                    OrderedSlotIDs.Add(RangedSlotIDs[RangedIndex]);
                    RangedIndex++;
                }
                else OrderedSlotIDs.Add(entity.Entity.SlotID);
            }

            foreach (var entity in OrderedEntities)
            {
                UnorderedSlotIDs.Add(entity.SlotID);
            }
            IAction action = new Actions.ActionSortSlotIDs(OrderedEntities, OrderedSlotIDs, UnorderedSlotIDs, new Action<IList<SceneEntity>, IList<ushort>>(ChangeSeveralSlotIDs));
            SlotIDSwapped?.Invoke(action);
            ChangeSeveralSlotIDs(OrderedEntities, OrderedSlotIDs);

            Editor.Instance.UndoStack.Push(action);
            Editor.Instance.RedoStack.Clear();
            Editor.Instance.UI.UpdateControls();

        }
        public void SwapSlotIDsFromPair()
        {
            EditorEntity entity1 = SelectedEntities[0];
            EditorEntity entity2 = SelectedEntities[1];
            ushort slotID_A = entity1.Entity.SlotID;
            ushort slotID_B = entity2.Entity.SlotID;
            IAction action = new Actions.ActionSwapSlotIDs(entity1.Entity, entity2.Entity, slotID_A, slotID_B, new Action<SceneEntity, SceneEntity, ushort, ushort>(SwapSlotIDs));
            SlotIDSwapped?.Invoke(action);
            SwapSlotIDs(entity1.Entity, entity2.Entity, slotID_A, slotID_B);

            Editor.Instance.UndoStack.Push(action);
            Editor.Instance.RedoStack.Clear();
            Editor.Instance.UI.UpdateControls();
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
            foreach (var entity in Entities.OrderBy(x => x.Entity.SlotID))
            {
                OrderedEntities.Add(entity.Entity);
                OrderedSlotIDs.Add(GetRealSlotID(entity.Entity));
                UnorderedSlotIDs.Add(entity.Entity.SlotID);
            }

            IAction action = new Actions.ActionSortSlotIDs(OrderedEntities, OrderedSlotIDs, UnorderedSlotIDs, new Action<IList<SceneEntity>, IList<ushort>>(ChangeSeveralSlotIDs));
            SlotIDSwapped?.Invoke(action);
            ChangeSeveralSlotIDs(OrderedEntities, OrderedSlotIDs);

            Editor.Instance.UndoStack.Push(action);
            Editor.Instance.RedoStack.Clear();
            Editor.Instance.UI.UpdateControls();
        }
        private void FindDuplicateIds()
        {
            var groupedById = Entities.GroupBy(e => e.Entity.SlotID).Where(g => g.Count() > 1);
        }
        #endregion

        #region Updating
        public void UpdateViewFilters()
        {
            FilterRefreshNeeded = false;
            foreach (EditorEntity entity in Entities)
            {
                entity.SetFilter();
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

        #region Drawing
        public void Draw(Graphics g)
        {
            foreach (var entity in Entities.OrderBy(e => e.Entity.SlotID))
            {
                entity.Draw(new GraphicsHandler(g));
            }
        }
        public void Draw(DevicePanel d)
        {
            if (FilterRefreshNeeded)
                UpdateViewFilters();
            foreach (var entity in Entities.OrderBy(e => e.Entity.SlotID))
            {
                if (entity.IsObjectOnScreen(new GraphicsHandler(d))) entity.Draw(d);
            }
        }

        Dictionary<int, List<float>> SplineXPos = new Dictionary<int, List<float>>();
        Dictionary<int, List<float>> SplineYPos = new Dictionary<int, List<float>>();

        public void DrawInternalObjects(DevicePanel d)
        {
            if (FilterRefreshNeeded)
                UpdateViewFilters();


            foreach (var entity in InternalEntities)
            {
                if (entity.IsObjectOnScreen(new GraphicsHandler(d))) entity.DrawInternal(d);
                if (entity.Name == "Spline")
                {
                    int id = entity.Entity.attributesMap["SplineID"].ValueInt32;
                    if (!Editor.Instance.UIModes.SplineOptionsGroup.ContainsKey(id)) Editor.Instance.UIModes.AddSplineOptionsGroup(id);
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
                int splineID = path.Key;
                EditorUIModes.SplineOptions selectedOptions = Editor.Instance.UIModes.SplineOptionsGroup[splineID];
                if (SplineXPos[splineID].Count > 1)
                {
                    float[] xs, ys;
                    Functions.Extensions.Spline.CubicSpline.FitParametric(SplineXPos[splineID].ToArray(), SplineYPos[splineID].ToArray(), (selectedOptions.SplineSize > 0 ? selectedOptions.SplineSize : 1), out xs, out ys);
                    Point lastPoint = new Point(-1, -1);
                    foreach (var p in Extensions.CreateDataPoints(xs, ys))
                    {
                        if (lastPoint.X != -1)
                        {
                            if (selectedOptions.SplineToolShowLines) d.DrawLine(p.X, p.Y, lastPoint.X, lastPoint.Y, System.Drawing.Color.Red);
                        }
                        if (selectedOptions.SplineToolShowPoints) d.DrawRectangle(p.X, p.Y, p.X + 2, p.Y + 2, System.Drawing.Color.Red);
                        if (selectedOptions.SplineToolShowObject && selectedOptions.SplineObjectRenderingTemplate != null)
                        {
                            if (EditorEntityDrawing.RenderingSettings.ObjectToRender.Contains(selectedOptions.SplineObjectRenderingTemplate.Entity.Object.Name.Name))
                            {
                                Editor.Instance.EntityDrawing.DrawOthers(new GraphicsHandler(d), selectedOptions.SplineObjectRenderingTemplate.Entity, selectedOptions.SplineObjectRenderingTemplate, p.X, p.Y, 0, 0, 0, selectedOptions.SplineObjectRenderingTemplate.EditorAnimations, selectedOptions.SplineObjectRenderingTemplate.Selected, selectedOptions.SplineObjectRenderingTemplate.AttributeValidater, true);
                            }
                            else
                            {
                                selectedOptions.SplineObjectRenderingTemplate.FallbackDraw(new GraphicsHandler(d), p.X, p.Y, 0, 0, 255, System.Drawing.Color.Transparent, true);
                            }
                        }
                        lastPoint = new Point(p.X, p.Y);
                    }
                }
                SplineXPos[path.Key].Clear();
                SplineYPos[path.Key].Clear();
            }





        }


        public void DrawPriority(DevicePanel d, int prority)
        {
            if (FilterRefreshNeeded)
                UpdateViewFilters();
            foreach (var entity in Entities.OrderBy(e => e.Entity.SlotID))
            {
                if (entity.ValidPriorityPlane(prority) && entity.IsObjectOnScreen(new GraphicsHandler(d))) entity.Draw(d);
            }
        }
        public void DrawSelectionBoxes(DevicePanel d)
        {
            foreach (var entity in Entities.OrderBy(e => e.Entity.SlotID))
            {
                if (entity.IsObjectOnScreen(new GraphicsHandler(d))) entity.DrawBoxOnly(d);
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





    }
}