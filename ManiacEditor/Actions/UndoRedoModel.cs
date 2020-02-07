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
        public static void UpdateUndoRedo()
        {
            if (ManiacEditor.Classes.Editor.SolutionState.IsEntitiesEdit())
            {
                if (Classes.Editor.Solution.Entities.SelectedEntities.Count > 0)
                {
                    IAction action = new ActionMoveEntities(Classes.Editor.Solution.Entities.SelectedEntities.ToList(), new Point(Classes.Editor.SolutionState.DraggedX, Classes.Editor.SolutionState.DraggedY));
                    if (Classes.Editor.Solution.Entities.LastAction != null)
                    {
                        // If it is move & duplicate, merge them together
                        var taction = new ActionsGroup();
                        taction.AddAction(Classes.Editor.Solution.Entities.LastAction);
                        Classes.Editor.Solution.Entities.LastAction = null;
                        taction.AddAction(action);
                        taction.Close();
                        action = taction;
                    }
                    Controls.Editor.MainEditor.Instance.UndoStack.Push(action);
                    Controls.Editor.MainEditor.Instance.RedoStack.Clear();
                    Methods.Internal.UserInterface.UpdateControls();
                }
                if (Classes.Editor.Solution.Entities.SelectedInternalEntities.Count > 0)
                {
                    IAction action = new ActionMoveEntities(Classes.Editor.Solution.Entities.SelectedInternalEntities.ToList(), new Point(Classes.Editor.SolutionState.DraggedX, Classes.Editor.SolutionState.DraggedY));
                    if (Classes.Editor.Solution.Entities.LastActionInternal != null)
                    {
                        // If it is move & duplicate, merge them together
                        var taction = new ActionsGroup();
                        taction.AddAction(Classes.Editor.Solution.Entities.LastActionInternal);
                        Classes.Editor.Solution.Entities.LastActionInternal = null;
                        taction.AddAction(action);
                        taction.Close();
                        action = taction;
                    }
                    Controls.Editor.MainEditor.Instance.UndoStack.Push(action);
                    Controls.Editor.MainEditor.Instance.RedoStack.Clear();
                    Methods.Internal.UserInterface.UpdateControls();
                }





            }
        }
    }
}
