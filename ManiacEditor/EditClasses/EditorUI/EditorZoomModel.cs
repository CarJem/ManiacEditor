using EditClasses.Solution;
using System;
using System.Windows;

namespace ManiacEditor
{
    public class EditorZoomModel
    {
        private bool AllowScrollUpdate = true;
        private Editor Editor;
        public EditorZoomModel(Editor instance)
        {
            Editor = instance;
        }

        public void VScrollBar1_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {
            if (AllowScrollUpdate)
            {
                EditClasses.EditorState.ViewPositionY = (int)e.NewValue;
                UpdateScrollBars();
            }
            Editor.FormsModel.GraphicPanel.Render();
        }

        public void HScrollBar1_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {
            if (AllowScrollUpdate)
            {
                EditClasses.EditorState.ViewPositionX = (int)e.NewValue;
                UpdateScrollBars();
            }
            Editor.FormsModel.GraphicPanel.Render();
        }

        public void VScrollBar1_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (AllowScrollUpdate)
            {
                EditClasses.EditorState.ViewPositionY = (int)Editor.FormsModel.vScrollBar1.Value;
                UpdateScrollBars();
            }
            //TODO: Determine if we still need this
            //if (!(EditorStateModel.Zooming || EditorStateModel.DraggingSelection || EditorStateModel.Dragged || EditorStateModel.Scrolling)) Editor.FormsModel.GraphicPanel.Render();
            if (EditClasses.EditorState.DraggingSelection)
            {
                Editor.FormsModel.GraphicPanel.OnMouseMoveEventCreate();
            }

        }

        public void HScrollBar1_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (AllowScrollUpdate)
            {
                EditClasses.EditorState.ViewPositionX = (int)Editor.FormsModel.hScrollBar1.Value;
                UpdateScrollBars();
            }
            //TODO: Determine if we still need this
            //if (!(EditorStateModel.Zooming || EditorStateModel.DraggingSelection || EditorStateModel.Dragged || EditorStateModel.Scrolling)) Editor.FormsModel.GraphicPanel.Render();
            if (EditClasses.EditorState.DraggingSelection)
            {
                Editor.FormsModel.GraphicPanel.OnMouseMoveEventCreate();
            }

        }

        public void UpdateScrollBars()
        {
            //TODO: Determine if we still need this
            /*AllowScrollUpdate = false;
            Editor.editorView.hScrollBar1.Value = (int)Editor.editorView.hScrollBar1.Value;
            Editor.editorView.vScrollBar1.Value = (int)Editor.editorView.vScrollBar1.Value;
            AllowScrollUpdate = true;*/
        }

        public void VScrollBar1_Entered(object sender, EventArgs e)
        {
            if (!EditClasses.EditorState.ScrollLocked)
            {
                EditClasses.EditorState.ScrollDirection = (int)ScrollDir.Y;
            }
        }

        public void HScrollBar1_Entered(object sender, EventArgs e)
        {
            if (!EditClasses.EditorState.ScrollLocked)
            {
                EditClasses.EditorState.ScrollDirection = (int)ScrollDir.X;
            }
        }

        public void Resize(object sender, RoutedEventArgs e)
        {
            
            // TODO: It hides right now few pixels at the edge

            Visibility nvscrollbar = Visibility.Visible;
            Visibility nhscrollbar = Visibility.Visible;

            if (Editor.FormsModel.hScrollBar1.Maximum == 0) nhscrollbar = Visibility.Hidden;
            if (Editor.FormsModel.vScrollBar1.Maximum == 0) nvscrollbar = Visibility.Hidden;

            Editor.FormsModel.vScrollBar1.Visibility = nvscrollbar;
            Editor.FormsModel.vScrollBar1Host.Child.Visibility = nvscrollbar;
            Editor.FormsModel.hScrollBar1Host.Child.Visibility = nhscrollbar;
            Editor.FormsModel.hScrollBar1.Visibility = nhscrollbar;

            if (Editor.FormsModel.vScrollBar1.IsVisible)
            {
                Editor.FormsModel.vScrollBar1.LargeChange = Editor.FormsModel.vScrollBar1Host.Height;
                Editor.FormsModel.vScrollBar1.SmallChange = Editor.FormsModel.vScrollBar1Host.Height / 8;
                EditClasses.EditorState.ScreenHeight = (int)Editor.FormsModel.vScrollBar1Host.Height;
                Editor.FormsModel.vScrollBar1.Value = Math.Max(0, Math.Min(Editor.FormsModel.vScrollBar1.Value, Editor.FormsModel.vScrollBar1.Maximum));
                if (Editor.FormsModel.vScrollBar1.Track.ViewportSize != Editor.SceneHeight) Editor.FormsModel.vScrollBar1.Track.ViewportSize = Editor.SceneHeight;
            }
            else
            {
                EditClasses.EditorState.ScreenHeight = Editor.FormsModel.GraphicPanel.Height;
                EditClasses.EditorState.ViewPositionY = 0;
                Editor.FormsModel.vScrollBar1.Value = 0;
            }
            if (Editor.FormsModel.hScrollBar1.IsVisible)
            {
                Editor.FormsModel.hScrollBar1.LargeChange = Editor.FormsModel.hScrollBar1Host.Width;
                Editor.FormsModel.hScrollBar1.SmallChange = Editor.FormsModel.hScrollBar1Host.Width / 8;
                EditClasses.EditorState.ScreenWidth = (int)Editor.FormsModel.hScrollBar1Host.Width;
                Editor.FormsModel.hScrollBar1.Value = Math.Max(0, Math.Min(Editor.FormsModel.hScrollBar1.Value, Editor.FormsModel.hScrollBar1.Maximum));
                if (Editor.FormsModel.hScrollBar1.Track.ViewportSize != Editor.SceneWidth) Editor.FormsModel.hScrollBar1.Track.ViewportSize = Editor.SceneWidth;
            }
            else
            {
                EditClasses.EditorState.ScreenWidth = Editor.FormsModel.GraphicPanel.Width;
                EditClasses.EditorState.ViewPositionX = 0;
                Editor.FormsModel.hScrollBar1.Value = 0;
            }

            while (EditClasses.EditorState.ScreenWidth > Editor.FormsModel.GraphicPanel.Width)
                ResizeGraphicPanel(Editor.FormsModel.GraphicPanel.Width * 2, Editor.FormsModel.GraphicPanel.Height);
            while (EditClasses.EditorState.ScreenHeight > Editor.FormsModel.GraphicPanel.Height)
                ResizeGraphicPanel(Editor.FormsModel.GraphicPanel.Width, Editor.FormsModel.GraphicPanel.Height * 2);
        }

        public void SetViewSize(int width = 0, int height = 0, bool resizeForm = true)
        {
            if (Settings.MySettings.EntityFreeCam)
            {
                width = 10000000;
                height = 10000000;
            }

            Editor.FormsModel.vScrollBar1.Maximum = height - Editor.FormsModel.vScrollBar1Host.Height;
            Editor.FormsModel.hScrollBar1.Maximum = width - Editor.FormsModel.hScrollBar1Host.Width;

            Editor.FormsModel.GraphicPanel.DrawWidth = Math.Min((int)width, Editor.FormsModel.GraphicPanel.Width);
            Editor.FormsModel.GraphicPanel.DrawHeight = Math.Min((int)height, Editor.FormsModel.GraphicPanel.Height);

            if (resizeForm) Resize(null, null);

            if (!Settings.MySettings.EntityFreeCam)
            {
                Editor.FormsModel.hScrollBar1.Value = Math.Max(0, Math.Min(Editor.FormsModel.hScrollBar1.Value, Editor.FormsModel.hScrollBar1.Maximum));
                Editor.FormsModel.vScrollBar1.Value = Math.Max(0, Math.Min(Editor.FormsModel.vScrollBar1.Value, Editor.FormsModel.vScrollBar1.Maximum));
            }

        }

        #region Zooming/Resizing Related Methods

        public void SetZoomLevel(int zoom_level, System.Drawing.Point zoom_point, double zoom_level_d = 0.0, bool updateControls = true)
        {
            double old_zoom = EditClasses.EditorState.Zoom;



            if (zoom_level_d == 0.0)
            {
                EditClasses.EditorState.ZoomLevel = zoom_level;
                switch (EditClasses.EditorState.ZoomLevel)
                {
                    case 5: EditClasses.EditorState.Zoom = 4; break;
                    case 4: EditClasses.EditorState.Zoom = 3; break;
                    case 3: EditClasses.EditorState.Zoom = 2; break;
                    case 2: EditClasses.EditorState.Zoom = 3 / 2.0; break;
                    case 1: EditClasses.EditorState.Zoom = 5 / 4.0; break;
                    case 0: EditClasses.EditorState.Zoom = 1; break;
                    case -1: EditClasses.EditorState.Zoom = 2 / 3.0; break;
                    case -2: EditClasses.EditorState.Zoom = 1 / 2.0; break;
                    case -3: EditClasses.EditorState.Zoom = 1 / 3.0; break;
                    case -4: EditClasses.EditorState.Zoom = 1 / 4.0; break;
                    case -5: EditClasses.EditorState.Zoom = 1 / 8.0; break;
                }
            }
            else
            {
                EditClasses.EditorState.ZoomLevel = (int)zoom_level_d;
                EditClasses.EditorState.Zoom = zoom_level_d;
            }


            EditClasses.EditorState.Zooming = true;

            int oldShiftX = EditClasses.EditorState.ViewPositionX;
            int oldShiftY = EditClasses.EditorState.ViewPositionY;

            if (CurrentSolution.CurrentScene != null)
                SetViewSize((int)(Editor.SceneWidth * EditClasses.EditorState.Zoom), (int)(Editor.SceneHeight * EditClasses.EditorState.Zoom), updateControls);


            if (Editor.FormsModel.hScrollBar1.IsVisible)
            {
                EditClasses.EditorState.ViewPositionX = (int)((zoom_point.X + oldShiftX) / old_zoom * EditClasses.EditorState.Zoom - zoom_point.X);
                EditClasses.EditorState.ViewPositionX = (int)Math.Min((Editor.FormsModel.hScrollBar1.Maximum), Math.Max(0, EditClasses.EditorState.ViewPositionX));
                Editor.FormsModel.hScrollBar1.Value = EditClasses.EditorState.ViewPositionX;
            }
            if (Editor.FormsModel.vScrollBar1.IsVisible)
            {
                EditClasses.EditorState.ViewPositionY = (int)((zoom_point.Y + oldShiftY) / old_zoom * EditClasses.EditorState.Zoom - zoom_point.Y);
                EditClasses.EditorState.ViewPositionY = (int)Math.Min((Editor.FormsModel.vScrollBar1.Maximum), Math.Max(0, EditClasses.EditorState.ViewPositionY));
                Editor.FormsModel.vScrollBar1.Value = EditClasses.EditorState.ViewPositionY;
            }


            EditClasses.EditorState.Zooming = false;

            if (updateControls) Editor.UI.UpdateControls();
        }

        public void ResetViewSize()
        {
            Editor.ZoomModel.SetViewSize((int)(Editor.SceneWidth * EditClasses.EditorState.Zoom), (int)(Editor.SceneHeight * EditClasses.EditorState.Zoom));
        }
        public void ResizeGraphicPanel(int width = 0, int height = 0)
        {
            if (Settings.MySettings.EntityFreeCam)
            {
                width = Editor.SceneWidth;
                height = Editor.SceneHeight;
            }

            Editor.FormsModel.GraphicPanel.Width = width;
            Editor.FormsModel.GraphicPanel.Height = height;

            Editor.FormsModel.GraphicPanel.ResetDevice();

            Editor.FormsModel.GraphicPanel.DrawWidth = Math.Min((int)Editor.FormsModel.hScrollBar1.Maximum, Editor.FormsModel.GraphicPanel.Width);
            Editor.FormsModel.GraphicPanel.DrawHeight = Math.Min((int)Editor.FormsModel.vScrollBar1.Maximum, Editor.FormsModel.GraphicPanel.Height);
        }
        #endregion
    }
}
