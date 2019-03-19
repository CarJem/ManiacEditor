using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using IronPython.Modules;
using RSDKv5;
using SharpDX.Direct3D9;
using ManiacEditor.Entity_Renders;
using ManiacEditor.Actions;

namespace ManiacEditor
{
    public class EditorEntities : IDrawable
    {
        public bool FilterRefreshNeeded = false;
        public int DefaultFilter = -1;

        public bool OptimizeAssets = false;

        private List<EditorEntity> _Entities = new List<EditorEntity>();
        private SortedDictionary<ushort, EditorEntity> _EntitiesBySlot = new SortedDictionary<ushort, EditorEntity>();

        public List<EditorEntity> Entities { get =>  GetEntities(); set => SetEntities(value); }
        public IList<EditorEntity> SelectedEntities { get => GetSelectedEntities(); set => SetSelectedEntities(value); }
        SortedDictionary<ushort, EditorEntity> EntitiesBySlot { get => GetSortedEntities(); }

        public IList<EditorEntity> tempSelection = new List<EditorEntity>();




        ushort nextFreeSlot = 0;

        public class TooManyEntitiesException : Exception { }

        public Actions.IAction LastAction;
        public Action<IAction> SlotIDSwapped;

        public int layerPrority = 0;

        public string SetupObject = "";

        public Editor EditorInstance;

        public EditorEntities(Scene scene, Editor instance)
        {
            EditorInstance = instance;
            foreach (var obj in scene.Objects)
            {
                Entities.AddRange(obj.Entities.Select(x => GenerateEditorEntity(x)));
            }
            FindDuplicateIds();
            SetupObject = GetSetupObject(scene);
            GetSortedEntities();
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

        private SortedDictionary<ushort,EditorEntity> GetSortedEntities()
        {
            var keyValuePairs = Entities.ToDictionary(x => x.Entity.SlotID);
            _EntitiesBySlot = new SortedDictionary<ushort, EditorEntity>(keyValuePairs);
            return new SortedDictionary<ushort, EditorEntity>(keyValuePairs);
        }

        private void SetSelectedEntities(IList<EditorEntity> SelectedObj)
        {
            foreach (var entity in Entities.Where(X => SelectedObj.Contains(X)).ToList())
            {
                Entities.Where(X =>  X == entity).FirstOrDefault().Selected = entity.Selected;
            }
        }

        private List<EditorEntity> GetSelectedEntities()
        {
            return Entities.Where(x => x.Selected == true).ToList();
        }

        private void FindDuplicateIds()
        {
            var groupedById = Entities.GroupBy(e => e.Entity.SlotID).Where(g => g.Count() > 1);
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
                var objectList = GetObjects(EditorInstance.EditorScene.Objects);
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

        public ushort GetRealSlotID(RSDKv5.SceneEntity CurrentObject)
        {
            GetSortedEntities();
            return (ushort)EntitiesBySlot.IndexOf(EntitiesBySlot.Where(x => x.Value.Entity == CurrentObject).FirstOrDefault());
        }

		public void Select(Rectangle area, bool addSelection = false, bool deselectIfSelected = false)
        {
            if (!addSelection) Deselect();
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

        public void Select(Point point, bool addSelection = false, bool deselectIfSelected = false)
        {
            if (!addSelection) Deselect();
            // In reverse because we want to select the top one
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

        public void TempSelection(Rectangle area, bool deselectIfSelected)
        {
            List<EditorEntity> newSelection = (from entity in Entities where entity.IsInArea(area) select entity).ToList();
            List<EditorEntity> outsideSelection = (from entity in Entities where (!entity.IsInArea(area) && entity.InTempSelection) select entity).ToList();

            foreach (var entity in outsideSelection)
            {
                entity.InTempSelection = false;
            }

            foreach (var entity in newSelection)
            {
                entity.InTempSelection = true;
                entity.TempSelected = !entity.Selected || !deselectIfSelected;
            }

            tempSelection = newSelection.Where(x => x.InTempSelection == true).ToList();


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



        public void OrderSelectedSlotIDs(bool optimize = false)
        {
            if (SelectedEntities == null || SelectedEntities.Count < 0) return;
            IList<SceneEntity> OrderedEntities = new List<SceneEntity>();
            IList<ushort> OrderedSlotIDs = new List<ushort>();
            IList<ushort> UnorderedSlotIDs = new List<ushort>();
            foreach (var entity in SelectedEntities.OrderBy(x => x.SelectedIndex))
            {
                OrderedEntities.Add(entity.Entity);
            }
            foreach (var entity in SelectedEntities.OrderBy(x => x.Entity.SlotID))
            {
                if (optimize) OrderedSlotIDs.Add(GetRealSlotID(entity.Entity));
                else OrderedSlotIDs.Add(entity.Entity.SlotID);

            }
            foreach (var entity in OrderedEntities)
            {
                UnorderedSlotIDs.Add(entity.SlotID);
            }
            IAction action = new Actions.ActionSortSlotIDs(OrderedEntities, OrderedSlotIDs, UnorderedSlotIDs, new Action<IList<SceneEntity>, IList<ushort>>(ChangeSeveralSlotIDs));
            SlotIDSwapped?.Invoke(action);
            ChangeSeveralSlotIDs(OrderedEntities, OrderedSlotIDs);

            EditorInstance.UndoStack.Push(action);
            EditorInstance.RedoStack.Clear();
            EditorInstance.UI.UpdateControls();

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

            EditorInstance.UndoStack.Push(action);
            EditorInstance.RedoStack.Clear();
            EditorInstance.UI.UpdateControls();
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

            EditorInstance.UndoStack.Push(action);
            EditorInstance.RedoStack.Clear();
            EditorInstance.UI.UpdateControls();
        }

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
                    sceneEntity = SceneEntity.FromExternal(entity.Entity, EditorInstance.EditorScene.Objects, slot);
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
                LastAction = new Actions.ActionAddDeleteEntities(new_entities.ToList(), true, x => AddEntities(x), x => DeleteEntities(x));
            Deselect();
            foreach (var entity in new_entities)
            {
                SelectedEntities.Add(entity);
            }
            foreach (var entity in new_entities)
                entity.Selected = true;
        }

        public void MoveSelected(Point oldPos, Point newPos, bool duplicate)
        {
            Point diff = new Point(newPos.X - oldPos.X, newPos.Y - oldPos.Y);
            if (duplicate) DuplicateEntities(SelectedEntities.ToList());
            foreach (var entity in SelectedEntities)
            {
                entity.Move(diff);
            }
        }

        public bool IsSelected()
        {
            return SelectedEntities.Count > 0 || tempSelection.Count > 0;
        }

        public void DeleteSelected()
        {
            if (SelectedEntities.Count > 0)
                LastAction = new Actions.ActionAddDeleteEntities(SelectedEntities.ToList(), false, x => AddEntities(x), x => DeleteEntities(x));
            DeleteEntities(SelectedEntities.ToList());
            Deselect();
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
            foreach (EditorEntity entity in Entities.Reverse<EditorEntity>())
                if (entity.ContainsPoint(point))
                    return entity;
            return null;
        }
        public void Deselect()
        {
            foreach (var entity in Entities)
            {
                entity.Selected = false;
            }
            SelectedEntities.Clear();
            GetSortedEntities();
        }

        public void EndTempSelection()
        {
            foreach (var entity in tempSelection)
            {
                entity.InTempSelection = false;
                entity.TempSelected = false;
            }
            tempSelection.Clear();
        }

        public void Draw(Graphics g)
        {

        }

        public void Draw(DevicePanel d)
        {
            if (FilterRefreshNeeded)
                UpdateViewFilters();
            foreach (var entity in Entities.OrderBy(e => e.Entity.SlotID))
            {
                if (entity.IsObjectOnScreen(d)) entity.Draw(d);
            }
        }

		public void DrawPriority(DevicePanel d, int prority)
        {
            if (FilterRefreshNeeded)
                UpdateViewFilters();
            foreach (var entity in Entities.OrderBy(e => e.Entity.SlotID))
            {
                if (entity.ValidPriorityPlane(prority) && entity.IsObjectOnScreen(d)) entity.Draw(d);
            }
        }

        public void DrawSelectionBoxes(DevicePanel d)
        {
            foreach (var entity in Entities.OrderBy(e => e.Entity.SlotID))
            {
                if (entity.IsObjectOnScreen(d)) entity.DrawBoxOnly(d);
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

        private EditorEntity GenerateEditorEntity(RSDKv5.SceneEntity sceneEntity)
        {
            /*
            try
            {
                string objectName = sceneEntity.Object.Name.Name;
                if (LinkedRendersNames.Contains(objectName)) return new LinkedEditorEntity(sceneEntity, EditorInstance);
            }
            catch
            {
                Debug.WriteLine("Failed to generate a LinkedEditorEntity, will create a basic one instead.");
            }*/

            EditorEntity entity = new EditorEntity(sceneEntity, EditorInstance);

            if (entity.HasFilter() && DefaultFilter > -1)
            {
                entity.Entity.GetAttribute("filter").ValueUInt8 = (byte)DefaultFilter;
                DefaultFilter = -1;
            }

            entity.SetFilter();

            return entity;
        }
        public void UpdateViewFilters()
        {
            FilterRefreshNeeded = false;
            foreach (EditorEntity entity in Entities)
			{
				entity.SetFilter();
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
    }
}
