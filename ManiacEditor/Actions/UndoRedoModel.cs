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

        public static void UpdateLastEntityAction()
        {
            if (Methods.Editor.Solution.Entities.LastAction != null || Methods.Editor.Solution.Entities.LastActionInternal != null) Actions.UndoRedoModel.RedoStack.Clear();
            if (Methods.Editor.Solution.Entities.LastAction != null)
            {
                Actions.UndoRedoModel.UndoStack.Push(Methods.Editor.Solution.Entities.LastAction);
                Methods.Editor.Solution.Entities.LastAction = null;
            }
            if (Methods.Editor.Solution.Entities.LastActionInternal != null)
            {
                Actions.UndoRedoModel.UndoStack.Push(Methods.Editor.Solution.Entities.LastActionInternal);
                Methods.Editor.Solution.Entities.LastActionInternal = null;
            }
            if (Methods.Editor.Solution.Entities.LastAction != null || Methods.Editor.Solution.Entities.LastActionInternal != null) Methods.Internal.UserInterface.UpdateControls();

        }
        public static void UpdateEditEntitiesActions()
        {
            if (ManiacEditor.Methods.Editor.SolutionState.IsEntitiesEdit())
            {
                if (Methods.Editor.Solution.Entities.SelectedEntities.Count > 0)
                {
                    IAction action = new ActionMoveEntities(Methods.Editor.Solution.Entities.SelectedEntities.ToList(), new System.Drawing.Point(Methods.Editor.SolutionState.DraggedX, Methods.Editor.SolutionState.DraggedY));
                    if (Methods.Editor.Solution.Entities.LastAction != null)
                    {
                        // If it is move & duplicate, merge them together
                        var taction = new ActionsGroup();
                        taction.AddAction(Methods.Editor.Solution.Entities.LastAction);
                        Methods.Editor.Solution.Entities.LastAction = null;
                        taction.AddAction(action);
                        taction.Close();
                        action = taction;
                    }
                    Actions.UndoRedoModel.UndoStack.Push(action);
                    Actions.UndoRedoModel.RedoStack.Clear();
                    Methods.Internal.UserInterface.UpdateControls();
                }
                if (Methods.Editor.Solution.Entities.SelectedInternalEntities.Count > 0)
                {
                    IAction action = new ActionMoveEntities(Methods.Editor.Solution.Entities.SelectedInternalEntities.ToList(), new System.Drawing.Point(Methods.Editor.SolutionState.DraggedX, Methods.Editor.SolutionState.DraggedY));
                    if (Methods.Editor.Solution.Entities.LastActionInternal != null)
                    {
                        // If it is move & duplicate, merge them together
                        var taction = new ActionsGroup();
                        taction.AddAction(Methods.Editor.Solution.Entities.LastActionInternal);
                        Methods.Editor.Solution.Entities.LastActionInternal = null;
                        taction.AddAction(action);
                        taction.Close();
                        action = taction;
                    }
                    Actions.UndoRedoModel.UndoStack.Push(action);
                    Actions.UndoRedoModel.RedoStack.Clear();
                    Methods.Internal.UserInterface.UpdateControls();
                }
            }
        }
        public static void UpdateEditLayerActions()
        {
            if (Methods.Editor.Solution.EditLayerA != null)
            {
                List<IAction> actions = Methods.Editor.Solution.EditLayerA?.Actions;
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
            if (Methods.Editor.Solution.EditLayerB != null)
            {
                List<IAction> actions = Methods.Editor.Solution.EditLayerB?.Actions;
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
