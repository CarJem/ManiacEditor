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
                Classes.Edit.SolutionState.ViewPositionY = (int)e.NewValue;
                UpdateScrollBars();
            }
            Editor.FormsModel.GraphicPanel.Render();
        }

        public void HScrollBar1_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {
            if (AllowScrollUpdate)
            {
                Classes.Edit.SolutionState.ViewPositionX = (int)e.NewValue;
                UpdateScrollBars();
            }
            Editor.FormsModel.GraphicPanel.Render();
        }

        public void VScrollBar1_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (AllowScrollUpdate)
            {
                Classes.Edit.SolutionState.ViewPositionY = (int)Editor.FormsModel.vScrollBar1.Value;
                UpdateScrollBars();
            }
            //TODO: Determine if we still need this
            //if (!(Classes.Edit.SolutionState.Zooming || Classes.Edit.SolutionState.DraggingSelection || Classes.Edit.SolutionState.Dragged || Classes.Edit.SolutionState.Scrolling)) Editor.FormsModel.GraphicPanel.Render();
            if (Classes.Edit.SolutionState.DraggingSelection)
            {
                Editor.FormsModel.GraphicPanel.OnMouseMoveEventCreate();
            }

        }

        public void HScrollBar1_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (AllowScrollUpdate)
            {
                Classes.Edit.SolutionState.ViewPositionX = (int)Editor.FormsModel.hScrollBar1.Value;
                UpdateScrollBars();
            }
            //TODO: Determine if we still need this
            //if (!(Classes.Edit.SolutionState.Zooming || Classes.Edit.SolutionState.DraggingSelection || Classes.Edit.SolutionState.Dragged || Classes.Edit.SolutionState.Scrolling)) Editor.FormsModel.GraphicPanel.Render();
            if (Classes.Edit.SolutionState.DraggingSelection)
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
            if (!Classes.Edit.SolutionState.ScrollLocked)
            {
                Classes.Edit.SolutionState.ScrollDirection = (int)ScrollDir.Y;
            }
        }

        public void HScrollBar1_Entered(object sender, EventArgs e)
        {
            if (!Classes.Edit.SolutionState.ScrollLocked)
            {
                Classes.Edit.SolutionState.ScrollDirection = (int)ScrollDir.X;
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
                Classes.Edit.SolutionState.ScreenHeight = (int)Editor.FormsModel.vScrollBar1Host.Height;
                Editor.FormsModel.vScrollBar1.Value = Math.Max(0, Math.Min(Editor.FormsModel.vScrollBar1.Value, Editor.FormsModel.vScrollBar1.Maximum));
                if (Editor.FormsModel.vScrollBar1.Track.ViewportSize != Classes.Edit.Solution.SceneHeight) Editor.FormsModel.vScrollBar1.Track.ViewportSize = Classes.Edit.Solution.SceneHeight;
            }
            else
            {
                Classes.Edit.SolutionState.ScreenHeight = Editor.FormsModel.GraphicPanel.Height;
                Classes.Edit.SolutionState.ViewPositionY = 0;
                Editor.FormsModel.vScrollBar1.Value = 0;
            }
            if (Editor.FormsModel.hScrollBar1.IsVisible)
            {
                Editor.FormsModel.hScrollBar1.LargeChange = Editor.FormsModel.hScrollBar1Host.Width;
                Editor.FormsModel.hScrollBar1.SmallChange = Editor.FormsModel.hScrollBar1Host.Width / 8;
                Classes.Edit.SolutionState.ScreenWidth = (int)Editor.FormsModel.hScrollBar1Host.Width;
                Editor.FormsModel.hScrollBar1.Value = Math.Max(0, Math.Min(Editor.FormsModel.hScrollBar1.Value, Editor.FormsModel.hScrollBar1.Maximum));
                if (Editor.FormsModel.hScrollBar1.Track.ViewportSize != Classes.Edit.Solution.SceneWidth) Editor.FormsModel.hScrollBar1.Track.ViewportSize = Classes.Edit.Solution.SceneWidth;
            }
            else
            {
                Classes.Edit.SolutionState.ScreenWidth = Editor.FormsModel.GraphicPanel.Width;
                Classes.Edit.SolutionState.ViewPositionX = 0;
                Editor.FormsModel.hScrollBar1.Value = 0;
            }

            while (Classes.Edit.SolutionState.ScreenWidth > Editor.FormsModel.GraphicPanel.Width)
                ResizeGraphicPanel(Editor.FormsModel.GraphicPanel.Width * 2, Editor.FormsModel.GraphicPanel.Height);
            while (Classes.Edit.SolutionState.ScreenHeight > Editor.FormsModel.GraphicPanel.Height)
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
            double old_zoom = Classes.Edit.SolutionState.Zoom;



            if (zoom_level_d == 0.0)
            {
                Classes.Edit.SolutionState.ZoomLevel = zoom_level;
                switch (Classes.Edit.SolutionState.ZoomLevel)
                {
                    case 5: Classes.Edit.SolutionState.Zoom = 4; break;
                    case 4: Classes.Edit.SolutionState.Zoom = 3; break;
                    case 3: Classes.Edit.SolutionState.Zoom = 2; break;
                    case 2: Classes.Edit.SolutionState.Zoom = 3 / 2.0; break;
                    case 1: Classes.Edit.SolutionState.Zoom = 5 / 4.0; break;
                    case 0: Classes.Edit.SolutionState.Zoom = 1; break;
                    case -1: Classes.Edit.SolutionState.Zoom = 2 / 3.0; break;
                    case -2: Classes.Edit.SolutionState.Zoom = 1 / 2.0; break;
                    case -3: Classes.Edit.SolutionState.Zoom = 1 / 3.0; break;
                    case -4: Classes.Edit.SolutionState.Zoom = 1 / 4.0; break;
                    case -5: Classes.Edit.SolutionState.Zoom = 1 / 8.0; break;
                }
            }
            else
            {
                Classes.Edit.SolutionState.ZoomLevel = (int)zoom_level_d;
                Classes.Edit.SolutionState.Zoom = zoom_level_d;
            }


            Classes.Edit.SolutionState.Zooming = true;

            int oldShiftX = Classes.Edit.SolutionState.ViewPositionX;
            int oldShiftY = Classes.Edit.SolutionState.ViewPositionY;

            if (Classes.Edit.Solution.CurrentScene != null)
                SetViewSize((int)(Classes.Edit.Solution.SceneWidth * Classes.Edit.SolutionState.Zoom), (int)(Classes.Edit.Solution.SceneHeight * Classes.Edit.SolutionState.Zoom), updateControls);


            if (Editor.FormsModel.hScrollBar1.IsVisible)
            {
                Classes.Edit.SolutionState.ViewPositionX = (int)((zoom_point.X + oldShiftX) / old_zoom * Classes.Edit.SolutionState.Zoom - zoom_point.X);
                Classes.Edit.SolutionState.ViewPositionX = (int)Math.Min((Editor.FormsModel.hScrollBar1.Maximum), Math.Max(0, Classes.Edit.SolutionState.ViewPositionX));
                Editor.FormsModel.hScrollBar1.Value = Classes.Edit.SolutionState.ViewPositionX;
            }
            if (Editor.FormsModel.vScrollBar1.IsVisible)
            {
                Classes.Edit.SolutionState.ViewPositionY = (int)((zoom_point.Y + oldShiftY) / old_zoom * Classes.Edit.SolutionState.Zoom - zoom_point.Y);
                Classes.Edit.SolutionState.ViewPositionY = (int)Math.Min((Editor.FormsModel.vScrollBar1.Maximum), Math.Max(0, Classes.Edit.SolutionState.ViewPositionY));
                Editor.FormsModel.vScrollBar1.Value = Classes.Edit.SolutionState.ViewPositionY;
            }


            Classes.Edit.SolutionState.Zooming = false;

            if (updateControls) Editor.UI.UpdateControls();
        }

        public void ResetViewSize()
        {
            Editor.ZoomModel.SetViewSize((int)(Classes.Edit.Solution.SceneWidth * Classes.Edit.SolutionState.Zoom), (int)(Classes.Edit.Solution.SceneHeight * Classes.Edit.SolutionState.Zoom));
        }
        public void ResizeGraphicPanel(int width = 0, int height = 0)
        {
            if (Settings.MySettings.EntityFreeCam)
            {
                width = Classes.Edit.Solution.SceneWidth;
                height = Classes.Edit.Solution.SceneHeight;
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
