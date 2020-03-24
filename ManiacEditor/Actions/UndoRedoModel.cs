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
    }
}
