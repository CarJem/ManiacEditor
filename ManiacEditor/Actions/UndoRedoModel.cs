using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ManiacEditor.Actions;
using RSDKv5;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using ManiacEditor.Controls.TileManiac;
using ManiacEditor.Enums;
using ManiacEditor.Extensions;

namespace ManiacEditor.Actions
{
    public static class UndoRedoModel
    {
        public static Stack<IAction> UndoStack { get; set; } = new Stack<IAction>(); //Undo Actions Stack
        public static Stack<IAction> RedoStack { get; set; } = new Stack<IAction>(); //Redo Actions Stack

        public static void Undo()
        {
            if (Actions.UndoRedoModel.UndoStack.Count > 0)
            {
                if (ManiacEditor.Methods.Solution.SolutionState.IsTilesEdit()) Methods.Solution.SolutionActions.Deselect(false);
                else if (ManiacEditor.Methods.Solution.SolutionState.IsEntitiesEdit())
                {
                    // deselect only if delete/create
                    if (Actions.UndoRedoModel.UndoStack.Peek() is ActionAddDeleteEntities) Methods.Solution.SolutionActions.Deselect(false);
                }
                IAction act = Actions.UndoRedoModel.UndoStack.Pop();
                if (act != null)
                {
                    act.Undo();
                    Actions.UndoRedoModel.RedoStack.Push(act.Redo());
                }
            }
            Methods.Internal.UserInterface.UpdateControls();
        }
        public static void Redo()
        {
            if (Actions.UndoRedoModel.RedoStack.Count > 0)
            {
                IAction act = Actions.UndoRedoModel.RedoStack.Pop();
                act.Undo();
                Actions.UndoRedoModel.UndoStack.Push(act.Redo());
            }
            Methods.Internal.UserInterface.UpdateControls();
        }
        public static void ClearStacks()
        {
            Actions.UndoRedoModel.UndoStack.Clear();
            Actions.UndoRedoModel.RedoStack.Clear();
        }
        public static void UpdateEditEntityActions()
        {
            if (Methods.Solution.CurrentSolution.Entities.LastAction != null || Methods.Solution.CurrentSolution.Entities.LastActionInternal != null) Actions.UndoRedoModel.RedoStack.Clear();
            if (Methods.Solution.CurrentSolution.Entities.LastAction != null)
            {
                Actions.UndoRedoModel.UndoStack.Push(Methods.Solution.CurrentSolution.Entities.LastAction);
                Methods.Solution.CurrentSolution.Entities.LastAction = null;
            }
            if (Methods.Solution.CurrentSolution.Entities.LastActionInternal != null)
            {
                Actions.UndoRedoModel.UndoStack.Push(Methods.Solution.CurrentSolution.Entities.LastActionInternal);
                Methods.Solution.CurrentSolution.Entities.LastActionInternal = null;
            }

        }
        public static void UpdateEditEntitiesActions()
        {
            if (ManiacEditor.Methods.Solution.SolutionState.IsEntitiesEdit())
            {
                if (Methods.Solution.CurrentSolution.Entities.SelectedEntities.Count > 0)
                {
                    IAction action = new ActionMoveEntities(Methods.Solution.CurrentSolution.Entities.SelectedEntities.ToList(), new System.Drawing.Point(Methods.Solution.SolutionState.DraggedX, Methods.Solution.SolutionState.DraggedY));
                    if (Methods.Solution.CurrentSolution.Entities.LastAction != null)
                    {
                        // If it is move & duplicate, merge them together
                        var taction = new ActionsGroup();
                        taction.AddAction(Methods.Solution.CurrentSolution.Entities.LastAction);
                        Methods.Solution.CurrentSolution.Entities.LastAction = null;
                        taction.AddAction(action);
                        taction.Close();
                        action = taction;
                    }
                    Actions.UndoRedoModel.UndoStack.Push(action);
                    Actions.UndoRedoModel.RedoStack.Clear();
                }
                if (Methods.Solution.CurrentSolution.Entities.SelectedInternalEntities.Count > 0)
                {
                    IAction action = new ActionMoveEntities(Methods.Solution.CurrentSolution.Entities.SelectedInternalEntities.ToList(), new System.Drawing.Point(Methods.Solution.SolutionState.DraggedX, Methods.Solution.SolutionState.DraggedY));
                    if (Methods.Solution.CurrentSolution.Entities.LastActionInternal != null)
                    {
                        // If it is move & duplicate, merge them together
                        var taction = new ActionsGroup();
                        taction.AddAction(Methods.Solution.CurrentSolution.Entities.LastActionInternal);
                        Methods.Solution.CurrentSolution.Entities.LastActionInternal = null;
                        taction.AddAction(action);
                        taction.Close();
                        action = taction;
                    }
                    Actions.UndoRedoModel.UndoStack.Push(action);
                    Actions.UndoRedoModel.RedoStack.Clear();
                }
            }
        }
        public static void UpdateEditLayerActions()
        {
            if (Methods.Solution.CurrentSolution.EditLayerA != null)
            {
                List<IAction> actions = Methods.Solution.CurrentSolution.EditLayerA?.Actions;
                if (actions.Count > 0) Actions.UndoRedoModel.RedoStack.Clear();
                while (actions.Count > 0)
                {
                    bool create_new = false;
                    if (Actions.UndoRedoModel.UndoStack.Count == 0 || !(Actions.UndoRedoModel.UndoStack.Peek() is ActionsGroup))
                    {
                        create_new = true;
                    }
                    else
                    {
                        create_new = (Actions.UndoRedoModel.UndoStack.Peek() as ActionsGroup).IsClosed;
                    }
                    if (create_new)
                    {
                        Actions.UndoRedoModel.UndoStack.Push(new ActionsGroup());
                    }
                    (Actions.UndoRedoModel.UndoStack.Peek() as ActionsGroup).AddAction(actions[0]);
                    actions.RemoveAt(0);
                }
            }
            if (Methods.Solution.CurrentSolution.EditLayerB != null)
            {
                List<IAction> actions = Methods.Solution.CurrentSolution.EditLayerB?.Actions;
                if (actions.Count > 0) Actions.UndoRedoModel.RedoStack.Clear();
                while (actions.Count > 0)
                {
                    bool create_new = false;
                    if (Actions.UndoRedoModel.UndoStack.Count == 0 || !(Actions.UndoRedoModel.UndoStack.Peek() is ActionsGroup))
                    {
                        create_new = true;
                    }
                    else
                    {
                        create_new = (Actions.UndoRedoModel.UndoStack.Peek() as ActionsGroup).IsClosed;
                    }
                    if (create_new)
                    {
                        Actions.UndoRedoModel.UndoStack.Push(new ActionsGroup());
                    }
                    (Actions.UndoRedoModel.UndoStack.Peek() as ActionsGroup).AddAction(actions[0]);
                    actions.RemoveAt(0);
                }
            }
        }
    }
}
