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
using ManiacEditor.Methods.Internal;

namespace ManiacEditor.Classes.Scene
{
    public class EditorEntities : IDrawable
    {
        #region Definitions

        #region Object Properties Update / Filters

        public static bool ObjectRefreshNeeded { get; set; } = false;
        public int CurrentDefaultFilter { get; set; } = -1;

        #endregion

        #region Entity Collections

        public List<RSDKv5.SceneObject> SceneObjects { get; set; } = new List<SceneObject>();
        public List<Classes.Scene.EditorEntity> Entities { get; set; } = new List<EditorEntity>();
        public List<Classes.Scene.EditorEntity> SelectedEntities { get => GetSelectedEntities(); set => SetSelectedEntities(value); }
        public List<Classes.Scene.EditorEntity> TemporarySelection { get; set; } = new List<Classes.Scene.EditorEntity>();

        public List<Classes.Scene.EditorEntity> InternalEntities { get; set; } = new List<EditorEntity>();
        public List<Classes.Scene.EditorEntity> SelectedInternalEntities { get => GetSelectedInternalEntities(); set => SetSelectedInternalEntities(value); }
        public List<Classes.Scene.EditorEntity> InternalTempSelection { get; set; } = new List<Classes.Scene.EditorEntity>();

        #region Accessors

        private void SetSelectedInternalEntities(IList<Classes.Scene.EditorEntity> SelectedObj)
        {
            List<Classes.Scene.EditorEntity> SortedList = InternalEntities.OrderBy(x => x.SelectedIndex).ToList();
            foreach (var entity in SortedList.Where(X => SelectedObj.Contains(X)).ToList())
            {
                InternalEntities.Where(x => x == entity).FirstOrDefault().Selected = entity.Selected;
            }
        }
        private List<Classes.Scene.EditorEntity> GetSelectedInternalEntities()
        {
            return InternalEntities.Where(x => x.Selected == true).ToList();
        }
        private void SetSelectedEntities(IList<Classes.Scene.EditorEntity> SelectedObj)
        {
            for (int i = 0; i < Entities.Count; i++)
            {
               if (SelectedObj.Contains(Entities[i])) Entities[i].Selected = true;
               else Entities[i].Selected = false;
            }
        }
        private List<Classes.Scene.EditorEntity> GetSelectedEntities()
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
        public string SetupObject { get => GetSetupObject(SourceScene); }
        private RSDKv5.Scene SourceScene { get; set; }

        #region Accesors
        public string GetSetupObject(RSDKv5.Scene scene)
        {
            try
            {
                var objectList = GetObjects(Methods.Solution.CurrentSolution.CurrentScene.Objects);
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
            SourceScene = scene;
            Load(scene);
        }

        #endregion

        #region Selection
        public bool IsAnythingSelected()
        {
            return SelectedEntities.Count > 0 || TemporarySelection.Count > 0;
        }
        private void SelectEverything()
        {
            for (int i = 0; i < Entities.Count; i++)
            {
                Entities[i].Selected = true;
            }

            for (int i = 0; i < InternalEntities.Count; i++)
            {
                InternalEntities[i].Selected = true;
            }
        }
        public void Select(Rectangle area, bool addSelection = false, bool deselectIfSelected = false)
        {
            if (!addSelection) ClearSelection();
            SelectNormal();
            SelectInternal();

            void SelectNormal()
            {
                for (int i = 0; i < Entities.Count; i++)
                {
                    var entity = Entities[i];
                    if (entity.IsInArea(area))
                    {
                        if (deselectIfSelected)
                        {
                            if (entity.Selected)
                            {
                                //SelectedEntities.Remove(entity);
                                entity.Selected = false;
                            }
                            else
                            {
                                entity.Selected = true;
                            }
                        }
                        else
                        {
                            //SelectedEntities.Add(entity);
                            entity.Selected = true;
                        }
                    }
                }
            }
            void SelectInternal()
            {
                for (int i = 0; i < InternalEntities.Count; i++)
                {
                    var entity = InternalEntities[i];
                    if (entity.IsInArea(area))
                    {
                        if (deselectIfSelected)
                        {
                            if (entity.Selected)
                            {
                                //SelectedInternalEntities.Remove(entity);
                                entity.Selected = false;
                            }
                            else
                            {
                                entity.Selected = true;
                            }
                        }
                        else
                        {
                            //SelectedInternalEntities.Add(entity);
                            entity.Selected = true;
                        }
                    }
                }
            }

        }
        public void Select(Point point, bool addSelection = false, bool deselectIfSelected = false)
        {
            if (!addSelection) ClearSelection();
            // In reverse because we want to select the top one
            SelectNormal();
            SelectInternal();


            void SelectNormal()
            {
                foreach (Classes.Scene.EditorEntity entity in Entities.ToList().Reverse<Classes.Scene.EditorEntity>())
                {
                    if (entity.ContainsPoint(point))
                    {
                        if (deselectIfSelected && SelectedEntities.Contains(entity))
                        {
                            //SelectedEntities.Remove(entity);
                            entity.Selected = false;
                        }
                        else
                        {
                            //SelectedEntities.Add(entity);
                            entity.Selected = true;
                        }
                        // Only the top
                        break;
                    }
                }
            }
            void SelectInternal()
            {
                foreach (Classes.Scene.EditorEntity entity in InternalEntities.ToList().Reverse<Classes.Scene.EditorEntity>())
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

            void TempSelectionUsingList(List<Classes.Scene.EditorEntity> list)
            {
                List<Classes.Scene.EditorEntity> newSelection = (from entity in list where entity.IsInArea(area) select entity).ToList();
                List<Classes.Scene.EditorEntity> outsideSelection = (from entity in list where (!entity.IsInArea(area) && entity.InTempSelection) select entity).ToList();

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
            ClearSelection();
            if (Entities.Exists(x => x.SlotID == (ushort)slot))
            {
                //SelectedEntities.Add(Entities[(ushort)slot]);
                Entities[(ushort)slot].Selected = true;
            }
            ObjectRefreshNeeded = true;
        }
        public void ClearSelection()
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
                //SelectedEntities.Clear();
            }

            void DeselectInternal()
            {
                foreach (var entity in InternalEntities)
                {
                    entity.Selected = false;
                    entity.InTempSelection = false;
                    entity.TempSelected = false;
                }
                //SelectedInternalEntities.Clear();
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
            var orderedList = SelectedEntities.OrderBy(x => x.TimeWhenSelected).ToList();
            for (int i = 0; i < orderedList.Count; i++)
            {
                orderedList[i].SelectedIndex = index;
                index++;
            }
        }

        #endregion

        #region Add Entities
        private void AddEntity(Classes.Scene.EditorEntity entity, bool isInternal = false)
        {
            if (isInternal) this.InternalEntities.Add(entity);
            else this.Entities.Add(entity);

            EvaluteSlotIDOrder();

            ObjectRefreshNeeded = true;

        }
        private void AddEntities(IEnumerable<Classes.Scene.EditorEntity> entities, bool isInternal = false)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            foreach (var entity in entities)
            {
                AddEntity(entity, isInternal);
            }

            EvaluteSlotIDOrder();
        }
        public void Spawn(RSDKv5.SceneObject sceneObject, RSDKv5.Position position)
        {
            var editorEntity = GenerateEditorEntity(new RSDKv5.SceneEntity(sceneObject, GetFreeSlot(0)));
            editorEntity.Position = position;
            var newEntities = new List<Classes.Scene.EditorEntity> { editorEntity };
            LastAction = new Actions.ActionAddDeleteEntities(newEntities, true, x => AddEntities(x), x => DeleteEntities(x));
            AddEntities(newEntities);
            ClearSelection();
            editorEntity.Selected = true;
            //SelectedEntities.Add(editorEntity);
        }
        public void SpawnMultiple(List<KeyValuePair<RSDKv5.SceneObject, RSDKv5.Position>> sceneObjects)
        {
            var newEntities = new List<Classes.Scene.EditorEntity> { };
            foreach (var keyPair in sceneObjects)
            {
                var sceneObject = keyPair.Key;
                var position = keyPair.Value;
                var editorEntity = GenerateEditorEntity(new RSDKv5.SceneEntity(sceneObject, GetFreeSlot(0)));
                editorEntity.Position = position;
                newEntities.Add(editorEntity);
            }


            LastAction = new Actions.ActionAddDeleteEntities(newEntities, true, x => AddEntities(x), x => DeleteEntities(x));
            AddEntities(newEntities);
            ClearSelection();
            foreach (var editorEntity in newEntities)
            {
                editorEntity.Selected = true;
                //SelectedEntities.Add(editorEntity);
            }

        }

        #endregion

        #region Remove Entities
        private void DeleteEntity(Classes.Scene.EditorEntity entity, bool isInternal = false)
        {
            if (isInternal) this.InternalEntities.Remove(entity);
            else this.Entities.Remove(entity);

            EvaluteSlotIDOrder();

            ObjectRefreshNeeded = true;
        }
        public void DeleteEntities(List<Classes.Scene.EditorEntity> entities, bool updateActions = false, bool isInternal = false)
        {
            if (updateActions) LastActionInternal = new Actions.ActionAddDeleteEntities(entities.ToList(), false, x => AddEntities(x, true), x => DeleteEntities(x, false, isInternal));
            foreach (var entity in entities) DeleteEntity(entity, isInternal);

            EvaluteSlotIDOrder();
            Methods.Drawing.ObjectDrawing.RequestEntityVisiblityRefresh(true);
        }
        public void DeleteSelected(bool isInternal = false)
        {
            if ((isInternal ? SelectedInternalEntities.Count : SelectedEntities.Count) > 0) LastAction = new Actions.ActionAddDeleteEntities((isInternal ? SelectedInternalEntities.ToList() : SelectedEntities.ToList()), false, x => AddEntities(x, isInternal), x => DeleteEntities(x, isInternal));
            DeleteEntities(SelectedEntities.ToList(), false, isInternal);
            ClearSelection();
            ManiacEditor.Actions.UndoRedoModel.UpdateEditEntityActions();
        }

        #endregion

        #region Pinpoint Entity

        public Classes.Scene.EditorEntity GetEntityAt(Point point)
        {
            Classes.Scene.EditorEntity result1 = GetEntityAtNormal();
            Classes.Scene.EditorEntity result2 = GetEntityAtInternal();
            return (result1 != null ? result1 : result2);



            Classes.Scene.EditorEntity GetEntityAtNormal()
            {
                foreach (Classes.Scene.EditorEntity entity in Entities.ToList().Reverse<Classes.Scene.EditorEntity>())
                {
                    if (entity.ContainsPoint(point))
                        return entity;
                }
                return null;
            }

            Classes.Scene.EditorEntity GetEntityAtInternal()
            {
                foreach (Classes.Scene.EditorEntity entity in InternalEntities.ToList().Reverse<Classes.Scene.EditorEntity>())
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

            bool GetIsEntityAtWithList(List<Classes.Scene.EditorEntity> list)
            {
                foreach (Classes.Scene.EditorEntity entity in list.Reverse<Classes.Scene.EditorEntity>())
                {
                    if (entity.ContainsPoint(point))
                        return true;
                }
                return false;
            }
        }

        #endregion

        #region Object Generation

        public Classes.Scene.EditorEntity GenerateSplineObject(int value)
        {
            NameIdentifier Name = new NameIdentifier("Spline");
            List<AttributeInfo> Attributes = new List<AttributeInfo>();
            AttributeInfo SplineID = new AttributeInfo("SplineID", AttributeTypes.INT32);
            Attributes.Add(SplineID);
            ushort Slot = 0;
            SceneEntity Entity = new SceneEntity(new SceneObject(Name, Attributes), Slot);
            Entity.attributesMap["SplineID"].ValueInt32 = value;

            var entity = new Classes.Scene.EditorEntity(Entity, true);
            return entity;
        }
        public void SpawnInternalSplineObject(RSDKv5.Position position, int value)
        {
            var editorEntity = GenerateSplineObject(value);
            editorEntity.SlotID = GetFreeSlot(0, true);
            editorEntity.Position = position;
            var newEntities = new List<Classes.Scene.EditorEntity> { editorEntity };
            LastActionInternal = new Actions.ActionAddDeleteEntities(newEntities, true, x => AddEntities(x, true), x => DeleteEntities(x, false, true));
            AddEntities(newEntities, true);
            ClearSelection();
            editorEntity.Selected = true;
            SelectedInternalEntities.Add(editorEntity);
        }
        public Classes.Scene.EditorEntity GenerateEditorEntity(RSDKv5.SceneEntity sceneEntity)
        {
            Classes.Scene.EditorEntity entity = new Classes.Scene.EditorEntity(sceneEntity);

            if (entity.HasFilter() && CurrentDefaultFilter > -1)
            {
                entity.GetAttribute("filter").ValueUInt8 = (byte)CurrentDefaultFilter;
                CurrentDefaultFilter = -1;
            }

            entity.SetFilter();

            return entity;
        }

        #endregion

        #region Set/Get Methods

        public Classes.Scene.EditorEntity GetSelectedEntity()
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
        private ushort GetFreeSlot(ushort SlotID, bool isInternal = false)
        {
            if (isInternal)
            {
                if (InternalEntities.Count > 2048)
                {
                    throw new TooManyEntitiesException();
                }
                for (ushort i = 0; i < InternalEntities.Count; i++)
                {
                    if (!InternalEntities.Exists(x => x.SlotID == (ushort)i)) return i;
                }
                return (ushort)(InternalEntities.Count + 1);
            }
            else
            {
                if (Entities.Count > 2048)
                {
                    throw new TooManyEntitiesException();
                }
                for (ushort i = 0; i < Entities.Count; i++)
                {
                    if (!Entities.Exists(x => x.SlotID == (ushort)i)) return i;
                }
                return (ushort)(Entities.Count + 1);
            }

        }


        #endregion

        #region Duplication Entities
        private void DuplicateEntities(List<Classes.Scene.EditorEntity> entities, bool isInternal = false)
        {
            if (null == entities || !entities.Any()) return;

            // work out a slot for each entity, and add it in turn
            // this should prevent generating the same ID for each member of the list
            var new_entities = new List<Classes.Scene.EditorEntity>();
            foreach (var entity in entities)
            {
                ushort slot = GetFreeSlot(entity.SlotID, isInternal);

                SceneEntity sceneEntity;
                // If this is pasted from another Scene, we need to reassign its Object
                if (entity.IsExternal)
                    sceneEntity = SceneEntity.FromExternal(entity.Entity, Methods.Solution.CurrentSolution.CurrentScene.Entities.SceneObjects, slot);
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
                LastAction = new Actions.ActionAddDeleteEntities(new_entities.ToList(), true, x => AddEntities(x, isInternal), x => DeleteEntities(x, false, isInternal));
            }

            ClearSelection();
            /*
            if (isInternal)
            {
                foreach (var entity in new_entities)
                {
                    entity.Selected = true;
                    SelectedInternalEntities.Add(entity);
                }
            }
            else
            {
                foreach (var entity in new_entities)
                {
                    entity.Selected = true;
                    SelectedEntities.Add(entity);
                }
            }*/
            foreach (var entity in new_entities)
                entity.Selected = true;
        }

        #endregion

        #region Clipboard
        public Methods.Solution.SolutionClipboard.ObjectsClipboardEntry GetClipboardData(bool KeepPosition = false)
        {
            if (SelectedEntities.Count == 0) return null;
            short minX = 0, minY = 0;

            List<Classes.Scene.EditorEntity> copiedEntities = SelectedEntities.Select(x => GenerateEditorEntity(new RSDKv5.SceneEntity(x.Entity, x.SlotID))).ToList();
            if (!KeepPosition)
            {
                minX = copiedEntities.Min(x => x.Position.X.High);
                minY = copiedEntities.Min(x => x.Position.Y.High);
                copiedEntities.ForEach(x => x.Move(new Point(-minX, -minY)));
            }

            return new Methods.Solution.SolutionClipboard.ObjectsClipboardEntry(copiedEntities);
        }
        public void PasteClipboardData(Point NewPos, Methods.Solution.SolutionClipboard.ObjectsClipboardEntry data)
        {
            var entities = data.GetData();
            DuplicateEntities(entities);
            foreach (var entity in SelectedEntities)
            {
                // Move them
                entity.Move(NewPos);
            }
            ObjectRefreshNeeded = true;
        }

        #endregion

        #region Actions
        public static void Cut()
        {
            Copy();
            Delete();
        }
        public static void Paste()
        {
            bool IsWindowsClipboard = false;
            // check if there are Classes.Edit.Scene.EditorSolution.Entities on the Windows clipboard; if so, use those
            if (System.Windows.Clipboard.ContainsData("ManiacEntities")) IsWindowsClipboard = WindowsPaste();
            // if there's none, use the internal clipboard
            if (Methods.Solution.SolutionClipboard.ObjectsClipboard != null && !IsWindowsClipboard) ManiacPaste();
        }
        public static void ManiacPaste()
        {
            Methods.Solution.CurrentSolution.Entities.PasteClipboardData(Methods.Solution.SolutionState.Main.GetLastXY(), Methods.Solution.SolutionClipboard.ObjectsClipboard);
            Actions.UndoRedoModel.UpdateEditEntityActions();
        }
        public static bool WindowsPaste()
        {
            try
            {

                Methods.Solution.CurrentSolution.Entities.PasteClipboardData(Methods.Solution.SolutionState.Main.GetLastXY(), GetClipboardData());
                Actions.UndoRedoModel.UpdateEditEntityActions();
                return true;
            }
            catch (Classes.Scene.EditorEntities.TooManyEntitiesException)
            {
                System.Windows.MessageBox.Show("Too many entities! (limit: 2048)");
                return true;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("There was a problem with pasting the content provided: " + Environment.NewLine + ex.Message);
                return false;
            }

            Methods.Solution.SolutionClipboard.ObjectsClipboardEntry GetClipboardData()
            {
                var data = System.Windows.Clipboard.GetDataObject().GetData("ManiacEntities");
                if (data != null) return (Methods.Solution.SolutionClipboard.ObjectsClipboardEntry)data;
                else return null;
            }
        }
        public static void Copy()
        {
            var copyData = Methods.Solution.CurrentSolution.Entities.GetClipboardData();
            Methods.Solution.SolutionClipboard.SetObjectClipboard(copyData);
        }
        public static void Duplicate()
        {
            try
            {
                Copy();
                Methods.Solution.CurrentSolution.Entities.PasteClipboardData(new Point(16, 16), Methods.Solution.CurrentSolution.Entities.GetClipboardData(true));
                ManiacEditor.Actions.UndoRedoModel.UpdateEditEntityActions();
            }
            catch (Classes.Scene.EditorEntities.TooManyEntitiesException)
            {
                System.Windows.MessageBox.Show("Too many entities! (limit: 2048)");
                return;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("There was a problem with duplicating the content provided: " + Environment.NewLine + ex.Message);
                return;
            }

        }
        public static void Delete()
        {
            Methods.Solution.CurrentSolution.Entities.DeleteSelected();
        }
        public static void Deselect()
        {
            Methods.Solution.CurrentSolution.Entities.ClearSelection();
        }
        public static void SelectAll()
        {
            Methods.Solution.CurrentSolution.Entities.SelectEverything();
        }
        public static void FlipEntities(FlipDirection direction)
        {
            Dictionary<Classes.Scene.EditorEntity, Point> initalPos = new Dictionary<Classes.Scene.EditorEntity, Point>();
            Dictionary<Classes.Scene.EditorEntity, Point> postPos = new Dictionary<Classes.Scene.EditorEntity, Point>();
            foreach (Classes.Scene.EditorEntity e in Methods.Solution.CurrentSolution.Entities.SelectedEntities)
            {
                initalPos.Add(e, new Point(e.PositionX, e.PositionY));
            }
            Methods.Solution.CurrentSolution.Entities.Flip(direction);
            foreach (Classes.Scene.EditorEntity e in Methods.Solution.CurrentSolution.Entities.SelectedEntities)
            {
                postPos.Add(e, new Point(e.PositionX, e.PositionY));
            }
            IAction action = new ActionMultipleMoveEntities(initalPos, postPos);
            Actions.UndoRedoModel.UndoStack.Push(action);
            Actions.UndoRedoModel.RedoStack.Clear();

        }
        public static void MoveEntities(System.Windows.Forms.KeyEventArgs e)
        {
            int x = 0, y = 0;
            int modifier = 1;
            if (Methods.Solution.SolutionState.Main.UseMagnetMode)
            {
                switch (e.KeyData)
                {
                    case Keys.Up: y = (Methods.Solution.SolutionState.Main.UseMagnetYAxis ? -Methods.Solution.SolutionState.Main.MagnetSize : -1); break;
                    case Keys.Down: y = (Methods.Solution.SolutionState.Main.UseMagnetYAxis ? Methods.Solution.SolutionState.Main.MagnetSize : 1); break;
                    case Keys.Left: x = (Methods.Solution.SolutionState.Main.UseMagnetXAxis ? -Methods.Solution.SolutionState.Main.MagnetSize : -1); break;
                    case Keys.Right: x = (Methods.Solution.SolutionState.Main.UseMagnetXAxis ? Methods.Solution.SolutionState.Main.MagnetSize : 1); break;
                }
            }
            if (Methods.Solution.SolutionState.Main.EnableFasterNudge)
            {
                if (Methods.Solution.SolutionState.Main.UseMagnetMode)
                {
                    switch (e.KeyData)
                    {
                        case Keys.Up: y = (Methods.Solution.SolutionState.Main.UseMagnetYAxis ? -Methods.Solution.SolutionState.Main.MagnetSize * Methods.Solution.SolutionState.Main.FasterNudgeAmount : -1 - Methods.Solution.SolutionState.Main.FasterNudgeAmount); break;
                        case Keys.Down: y = (Methods.Solution.SolutionState.Main.UseMagnetYAxis ? Methods.Solution.SolutionState.Main.MagnetSize * Methods.Solution.SolutionState.Main.FasterNudgeAmount : 1 + Methods.Solution.SolutionState.Main.FasterNudgeAmount); break;
                        case Keys.Left: x = (Methods.Solution.SolutionState.Main.UseMagnetXAxis ? -Methods.Solution.SolutionState.Main.MagnetSize * Methods.Solution.SolutionState.Main.FasterNudgeAmount : -1 - Methods.Solution.SolutionState.Main.FasterNudgeAmount); break;
                        case Keys.Right: x = (Methods.Solution.SolutionState.Main.UseMagnetXAxis ? Methods.Solution.SolutionState.Main.MagnetSize * Methods.Solution.SolutionState.Main.FasterNudgeAmount : 1 + Methods.Solution.SolutionState.Main.FasterNudgeAmount); break;
                    }
                }
                else
                {
                    switch (e.KeyData)
                    {
                        case Keys.Up: y = (-1 - Methods.Solution.SolutionState.Main.FasterNudgeAmount) * modifier; break;
                        case Keys.Down: y = (1 + Methods.Solution.SolutionState.Main.FasterNudgeAmount) * modifier; break;
                        case Keys.Left: x = (-1 - Methods.Solution.SolutionState.Main.FasterNudgeAmount) * modifier; break;
                        case Keys.Right: x = (1 + Methods.Solution.SolutionState.Main.FasterNudgeAmount) * modifier; break;
                    }

                }

            }
            if (Methods.Solution.SolutionState.Main.UseMagnetMode == false && Methods.Solution.SolutionState.Main.EnableFasterNudge == false)
            {
                switch (e.KeyData)
                {
                    case Keys.Up: y = -1 * modifier; break;
                    case Keys.Down: y = 1 * modifier; break;
                    case Keys.Left: x = -1 * modifier; break;
                    case Keys.Right: x = 1 * modifier; break;
                }

            }
            if (Methods.Solution.SolutionState.Main.UseMagnetMode)
            {
                int xE = Methods.Solution.CurrentSolution.Entities.SelectedEntities[0].Position.X.High;
                int yE = Methods.Solution.CurrentSolution.Entities.SelectedEntities[0].Position.Y.High;

                if (xE % Methods.Solution.SolutionState.Main.MagnetSize != 0 && Methods.Solution.SolutionState.Main.UseMagnetXAxis)
                {
                    int offsetX = x % Methods.Solution.SolutionState.Main.MagnetSize;
                    x -= offsetX;
                }
                if (yE % Methods.Solution.SolutionState.Main.MagnetSize != 0 && Methods.Solution.SolutionState.Main.UseMagnetYAxis)
                {
                    int offsetY = y % Methods.Solution.SolutionState.Main.MagnetSize;
                    y -= offsetY;
                }
            }

            Methods.Solution.CurrentSolution.Entities.MoveSelected(new Point(0, 0), new Point(x, y), false);


            // Try to merge with last move
            List<Classes.Scene.EditorEntity> SelectedList = Methods.Solution.CurrentSolution.Entities.SelectedEntities.ToList();
            List<Classes.Scene.EditorEntity> SelectedInternalList = Methods.Solution.CurrentSolution.Entities.SelectedInternalEntities.ToList();
            bool selectedActionsState = Actions.UndoRedoModel.UndoStack.Count > 0 && Actions.UndoRedoModel.UndoStack.Peek() is ActionMoveEntities && (Actions.UndoRedoModel.UndoStack.Peek() as ActionMoveEntities).UpdateFromKey(SelectedList, new Point(x, y));
            bool selectedInternalActionsState = Actions.UndoRedoModel.UndoStack.Count > 0 && Actions.UndoRedoModel.UndoStack.Peek() is ActionMoveEntities && (Actions.UndoRedoModel.UndoStack.Peek() as ActionMoveEntities).UpdateFromKey(SelectedInternalList, new Point(x, y));

            if (selectedActionsState || selectedInternalActionsState) { }
            else
            {
                if (SelectedList.Count != 0) Actions.UndoRedoModel.UndoStack.Push(new ActionMoveEntities(SelectedList, new Point(x, y), true));
                if (SelectedInternalList.Count != 0) Actions.UndoRedoModel.UndoStack.Push(new ActionMoveEntities(SelectedInternalList, new Point(x, y), true));

                Actions.UndoRedoModel.RedoStack.Clear();
            }
        }

        #endregion

        #region Movement + Interactions

        public void MoveSelected(Point oldPos, Point newPos, bool duplicate, bool addAction = false)
        {
            Point diff = new Point(newPos.X - oldPos.X, newPos.Y - oldPos.Y);

            if (duplicate)
            {
                if (SelectedEntities.Count > 0) DuplicateEntities(SelectedEntities.ToList());
                if (SelectedInternalEntities.Count > 0) DuplicateEntities(SelectedInternalEntities.ToList(), true);
            }

            if (SelectedEntities.Count > 0)
            {
                foreach (var entity in SelectedEntities) entity.Move(diff);
            }
            if (SelectedInternalEntities.Count > 0)
            {
                foreach (var entity in SelectedInternalEntities) entity.Move(diff);
            }

            ManiacEditor.Methods.Drawing.ObjectDrawing.RequestEntityVisiblityRefresh(true);
        }

        private Dictionary<Classes.Scene.EditorEntity, Point> GetSelectedMovePositions()
        {
            Dictionary<Classes.Scene.EditorEntity, Point> positions = new Dictionary<Classes.Scene.EditorEntity, Point>();
            foreach (Classes.Scene.EditorEntity e in SelectedEntities) positions.Add(e, new Point(e.PositionX, e.PositionY));
            return positions;
        }

        private void AddMoveAction(Dictionary<Classes.Scene.EditorEntity, Point> initalPos, Dictionary<Classes.Scene.EditorEntity, Point> postPos)
        {
            IAction action = new ActionMultipleMoveEntities(initalPos, postPos);
            Actions.UndoRedoModel.UndoStack.Push(action);
            Actions.UndoRedoModel.RedoStack.Clear();
        }

        internal void Flip(FlipDirection direction)
        {
            var positions = SelectedEntities.Select(se => se.Position);
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
                    int xPos = entity.PositionX;
                    int fromLeft = xPos - min;
                    int fromRight = max - xPos;

                    int newX = fromLeft < fromRight ? max - fromLeft : min + fromRight;
                    entity.PositionX = newX;
                }
                else
                {
                    int yPos = entity.PositionY;
                    int fromBottom = yPos - min;
                    int fromTop = max - yPos;

                    int newY = fromBottom < fromTop ? max - fromBottom : min + fromTop;
                    entity.PositionY = newY;
                }

                entity.Flip(direction);
            }
        }

        #endregion

        #region Slot ID Manipulation

        private void EvaluteSlotIDOrder()
        {   
            /*
            for (int i = 0; i < Entities.Count; i++)
            {
                Entities[i].SlotID = (ushort)i;
            }

            for (int i = 0; i < InternalEntities.Count; i++)
            {
                InternalEntities[i].SlotID = (ushort)i;
            }
            */
        }

        #endregion

        #region Object Properties

        public void UpdateObjectProperties(DevicePanel d = null)
        {
            ObjectRefreshNeeded = false;
            foreach (var Entity in Entities)
            {
                Entity.SetFilter();
            }

            if (d != null)
            {
                Methods.Drawing.ObjectDrawing.UpdateVisibleEntities(d, Entities);
                Methods.Drawing.ObjectDrawing.UpdateVisibleEntities(d, InternalEntities);
            }
        }
        #endregion

        #region Drawing
        public void Draw(Graphics g)
        {

        }
        public void Draw(DevicePanel d)
        {
            if (ObjectRefreshNeeded)
                UpdateObjectProperties(d);
            foreach (var entity in Entities.Where(x => x.IsVisible == true))
            {
                entity.Draw(d);
            }
            foreach (var entity in Entities.Where(x => x.IsVisible == true))
            {
                Methods.Drawing.ObjectDrawing.DrawSelectionBox(d, entity);
            }
        }
        public void DrawInternal(DevicePanel d)
        {
            if (ObjectRefreshNeeded)
                UpdateObjectProperties(d);


            foreach (var entity in InternalEntities.Where(x => x.IsVisible == true))
            {
                Methods.Drawing.ObjectDrawing.DrawInternal(d, entity);
                if (entity.Name == "Spline")
                {
                    int id = entity.attributesMap["SplineID"].ValueInt32;
                    if (!Methods.Solution.SolutionState.Main.SplineOptionsGroup.ContainsKey(id)) Methods.Solution.SolutionState.Main.AddSplineOptionsGroup(id);
                    if (SplineXPos.ContainsKey(id))
                    {
                        SplineXPos[id].Add(entity.PositionX);
                        SplineYPos[id].Add(entity.PositionY);
                    }
                    else
                    {
                        SplineXPos.Add(id, new List<float>() { entity.PositionX });
                        SplineYPos.Add(id, new List<float>() { entity.PositionY });
                    }
                }
            }

            foreach (var path in SplineXPos)
            {
                int CurrentSplineTotalNumberOfObjects = 0;
                int CurrentNumberOfObjectsRendered = 0;
                int splineID = path.Key;
                Methods.Solution.SolutionState.StateModel.SplineOptions selectedOptions = Methods.Solution.SolutionState.Main.SplineOptionsGroup[splineID];
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
                            if (Methods.Drawing.ObjectDrawing.CanDraw(selectedOptions.SplineObjectRenderingTemplate.Object.Name.Name))
                            {
                                Methods.Drawing.ObjectDrawing.DrawDedicatedRender(d, selectedOptions.SplineObjectRenderingTemplate);
                            }
                        }
                        lastPoint = new Point(p.X, p.Y);
                    }
                }
                SplineXPos[path.Key].Clear();
                SplineYPos[path.Key].Clear();

                Methods.Solution.SolutionState.Main.SplineOptionsGroup[splineID].SplineTotalNumberOfObjects = CurrentSplineTotalNumberOfObjects - 1;
                Methods.Solution.SolutionState.Main.SplineOptionsGroup[splineID].SplineNumberOfObjectsRendered = CurrentNumberOfObjectsRendered;
            }

            Methods.Internal.UserInterface.SplineControls.UpdateSplineToolbox();



        }
        #endregion

        #region Read/Write

        public void Load(RSDKv5.Scene Scene)
        {
            foreach (var SceneObject in Scene.Objects)
            {
                SceneObjects.Add(new SceneObject(SceneObject.Name, SceneObject.Attributes));
                Entities.AddRange(SceneObject.Entities.ConvertAll(x => GenerateEditorEntity(x)));
            }
            Entities = Entities.OrderBy(x => x.SlotID).ToList();
        }
        public List<SceneObject> Save()
        {
            List<SceneObject> Objects = new List<SceneObject>(SceneObjects);
            Objects.ForEach(x => x.Entities = new List<SceneEntity>());
            foreach (var entry in Entities)
            {
                SceneObject currentObject = entry.Object;
                if (!Objects.Exists(x => x.Name.ToString() == currentObject.Name.ToString())) Objects.Add(currentObject);
                int index = Objects.IndexOf(Objects.Where(x => x.Name.ToString() == currentObject.Name.ToString()).FirstOrDefault());
                /*if (!Objects[index].Entities.Exists(x => x.SlotID == entry.SlotID))*/ 
                Objects[index].Entities.Add(entry.Entity);
            }
            return Objects;
        }

        #endregion


    }
}
