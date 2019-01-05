using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.ComponentModel;
using ManiacEditor.Actions;
using ManiacEditor.Enums;
using ManiacEditor.Properties;
using RSDKv5;
using SharpDX.Direct3D9;
using Color = System.Drawing.Color;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Reflection;

namespace ManiacEditor
{
    public class EditorControls
    {
        private Editor Editor = Editor.Instance;

        //Shorthanding Settings
        readonly Properties.Settings mySettings = Properties.Settings.Default;
        Properties.KeyBinds myKeyBinds = Properties.KeyBinds.Default;
        public void GraphicPanel_OnKeyDown(object sender, KeyEventArgs e)
        {
            // Faster Nudge Toggle
            if (e.Control && e.KeyCode == myKeyBinds.NudgeFaster)
            {
                Editor.NudgeFasterButton_Click(sender, e);
            }
            // Scroll Lock Toggle
            else if (e.Control && e.KeyCode == myKeyBinds.ScrollLock)
            {
                Editor.ScrollLockButton_Click(sender, e);
            }
            // Switch Scroll Lock Type
            else if (e.Control && e.KeyCode == myKeyBinds.ScrollLockTypeSwitch)
            {
                Editor.SwapScrollLockDirectionToolStripMenuItem_Click(sender, e);

            }
            // Tiles Toolbar Flip Vertical
            else if (e.KeyCode == Keys.ShiftKey)
            {
                if (Editor.IsTilesEdit() && Editor.PlaceTilesButton.Checked)
                    Editor.TilesToolbar.SetSelectTileOption(1, true);
            }
            // Tiles Toolbar Flip Horizontal
            else if (e.KeyCode == Keys.ControlKey)
            {
                if (Editor.IsTilesEdit() && Editor.PlaceTilesButton.Checked)
                    Editor.TilesToolbar.SetSelectTileOption(0, true);
            }
            // Open Click (Alt: Open Data Dir)
            else if (e.Control && e.KeyCode == Keys.O)
            {
                if (e.Alt)
                {
                    Editor.OpenDataDirectoryToolStripMenuItem_Click(null, null);
                }
                else
                {
                    Editor.Open_Click(null, null);
                }
            }
            // New Click
            else if (e.Control && e.KeyCode == Keys.N)
            {
                //Editor.New_Click(null, null);
            }
            // Save Click (Alt: Save As)
            else if (e.Control && e.KeyCode == Keys.S)
            {
                if (e.Alt)
                {
                    Editor.SaveAsToolStripMenuItem_Click(null, null);
                }
                else
                {
                    Editor.Save_Click(null, null);
                }
            }
            // Reset Zoom Level
            else if (e.Control && (e.KeyCode == Keys.D0 || e.KeyCode == Keys.NumPad0))
            {
                Editor.SetZoomLevel(0, new Point(0, 0));
            }
            // Undo
            else if (e.KeyCode == Keys.Z)
            {
                Editor.EditorUndo();
            }
            // Redo
            else if (e.KeyCode == Keys.Y)
            {
                Editor.EditorRedo();
            }
            /*else if (e.KeyData == Keys.Up || e.KeyData == Keys.Down || e.KeyData == Keys.Left || e.KeyData == Keys.Right)
            {
                Editor.MoveCameraFreely(sender, e);
            }*/
            // Editing Key Shortcuts
            if (Editor.IsEditing())
            {
                GraphicPanel_OnKeyDownEditing(sender, e);
            }
        }

        public void GraphicPanel_OnKeyDownEditing(object sender, KeyEventArgs e)
        {
            //Paste
            if (e.Control && e.KeyCode == Keys.V)
            {
                Editor.PasteToolStripMenuItem_Click(sender, e);
            }
            //Select All
            if (e.Control && e.KeyCode == Keys.A)
            {
                Editor.SelectAllToolStripMenuItem_Click(sender, e);
            }
            // Selected Key Shortcuts   
            if (Editor.IsSelected())
            {
                GraphicPanel_OnKeyDownSelectedEditing(sender, e);
            }
        }

        public void GraphicPanel_OnKeyDownSelectedEditing(object sender, KeyEventArgs e)
        {
            // Delete
            if (e.KeyData == Keys.Delete)
            {
                Editor.DeleteSelected();
            }

            // Moving
            else if (e.KeyData == Keys.Up || e.KeyData == Keys.Down || e.KeyData == Keys.Left || e.KeyData == Keys.Right)
            {
                Editor.MoveEntityOrTiles(sender, e);
            }
            // (Ctrl Key)
            if (e.Control)
            {
                //Cut 
                if (e.KeyCode == Keys.X)
                {
                    Editor.CutToolStripMenuItem_Click(sender, e);
                }
                //Copy
                else if (e.KeyCode == Keys.C)
                {
                    Editor.CopyToolStripMenuItem_Click(sender, e);
                }
                //Duplicate
                else if (e.KeyCode == Keys.D)
                {
                    Editor.DuplicateToolStripMenuItem_Click(sender, e);
                }
                // Flip Vertical Individual
                else if (e.KeyCode == Keys.F)
                {
                    if (Editor.IsTilesEdit())
                        Editor.FlipVerticalIndividualToolStripMenuItem_Click(sender, e);
                }
                // Flip Horizontal Individual
                else if (e.KeyCode == Keys.M)
                {
                    if (Editor.IsTilesEdit())
                        Editor.FlipHorizontalIndividualToolStripMenuItem_Click(sender, e);
                }
            }
            else
            {
                // Flip Vertical
                if (e.KeyCode == Keys.F)
                {
                    if (Editor.IsTilesEdit())
                        Editor.FlipVerticalToolStripMenuItem_Click(sender, e);
                    else if (Editor.IsEntitiesEdit())
                        Editor.FlipEntities(FlipDirection.Veritcal);
                }

                // Flip Horizontal
                else if (e.KeyCode == Keys.M)
                {
                    if (Editor.IsTilesEdit())
                        Editor.FlipHorizontalToolStripMenuItem_Click(sender, e);
                    else if (Editor.IsEntitiesEdit())
                        Editor.FlipEntities(FlipDirection.Horizontal);
                }
            }
        }

        public void GraphicPanel_OnKeyUp(object sender, KeyEventArgs e)
        {
            // Tiles Toolbar Flip Horizontal
            if (e.KeyCode == Keys.ControlKey)
            {
                if (Editor.IsTilesEdit() && Editor.PlaceTilesButton.Checked)
                    Editor.TilesToolbar.SetSelectTileOption(0, false);
            }
            // Tiles Toolbar Flip Vertical
            else if (e.KeyCode == Keys.ShiftKey)
            {
                if (Editor.IsTilesEdit() && Editor.PlaceTilesButton.Checked)
                    Editor.TilesToolbar.SetSelectTileOption(1, false);
            }
        }
    }
}
