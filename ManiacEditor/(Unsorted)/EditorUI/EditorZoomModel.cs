using System;
using System.Windows;
using ManiacEditor.Enums;

namespace ManiacEditor
{
    public class EditorZoomModel
    {
        private bool AllowScrollUpdate = true;
        private Controls.Base.MainEditor Editor;
        public EditorZoomModel(Controls.Base.MainEditor instance)
        {
            Editor = instance;
        }

        public void VScrollBar1_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {
            if (AllowScrollUpdate)
            {
                Classes.Core.SolutionState.ViewPositionY = (int)e.NewValue;
                UpdateScrollBars();
            }
            Editor.DeviceModel.GraphicPanel.Render();
        }

        public void HScrollBar1_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {
            if (AllowScrollUpdate)
            {
                Classes.Core.SolutionState.ViewPositionX = (int)e.NewValue;
                UpdateScrollBars();
            }
            Editor.DeviceModel.GraphicPanel.Render();
        }

        public void VScrollBar1_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (AllowScrollUpdate)
            {
                Classes.Core.SolutionState.ViewPositionY = (int)Editor.DeviceModel.vScrollBar1.Value;
                UpdateScrollBars();
            }
            //TODO: Determine if we still need this
            //if (!(Classes.Edit.SolutionState.Zooming || Classes.Edit.SolutionState.DraggingSelection || Classes.Edit.SolutionState.Dragged || Classes.Edit.SolutionState.Scrolling)) Editor.FormsModel.GraphicPanel.Render();
            if (Classes.Core.SolutionState.DraggingSelection)
            {
                Editor.DeviceModel.GraphicPanel.OnMouseMoveEventCreate();
            }

        }

        public void HScrollBar1_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (AllowScrollUpdate)
            {
                Classes.Core.SolutionState.ViewPositionX = (int)Editor.DeviceModel.hScrollBar1.Value;
                UpdateScrollBars();
            }
            //TODO: Determine if we still need this
            //if (!(Classes.Edit.SolutionState.Zooming || Classes.Edit.SolutionState.DraggingSelection || Classes.Edit.SolutionState.Dragged || Classes.Edit.SolutionState.Scrolling)) Editor.FormsModel.GraphicPanel.Render();
            if (Classes.Core.SolutionState.DraggingSelection)
            {
                Editor.DeviceModel.GraphicPanel.OnMouseMoveEventCreate();
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
            if (!Classes.Core.SolutionState.ScrollLocked)
            {
                Classes.Core.SolutionState.ScrollDirection = (int)ScrollDir.Y;
            }
        }

        public void HScrollBar1_Entered(object sender, EventArgs e)
        {
            if (!Classes.Core.SolutionState.ScrollLocked)
            {
                Classes.Core.SolutionState.ScrollDirection = (int)ScrollDir.X;
            }
        }

        public void Resize(object sender, RoutedEventArgs e)
        {
            
            // TODO: It hides right now few pixels at the edge

            Visibility nvscrollbar = Visibility.Visible;
            Visibility nhscrollbar = Visibility.Visible;

            if (Editor.DeviceModel.hScrollBar1.Maximum == 0) nhscrollbar = Visibility.Hidden;
            if (Editor.DeviceModel.vScrollBar1.Maximum == 0) nvscrollbar = Visibility.Hidden;

            Editor.DeviceModel.vScrollBar1.Visibility = nvscrollbar;
            Editor.DeviceModel.vScrollBar1Host.Child.Visibility = nvscrollbar;
            Editor.DeviceModel.hScrollBar1Host.Child.Visibility = nhscrollbar;
            Editor.DeviceModel.hScrollBar1.Visibility = nhscrollbar;

            if (Editor.DeviceModel.vScrollBar1.IsVisible)
            {
                Editor.DeviceModel.vScrollBar1.LargeChange = Editor.DeviceModel.vScrollBar1Host.Height;
                Editor.DeviceModel.vScrollBar1.SmallChange = Editor.DeviceModel.vScrollBar1Host.Height / 8;
                Classes.Core.SolutionState.ScreenHeight = (int)Editor.DeviceModel.vScrollBar1Host.Height;
                Editor.DeviceModel.vScrollBar1.Value = Math.Max(0, Math.Min(Editor.DeviceModel.vScrollBar1.Value, Editor.DeviceModel.vScrollBar1.Maximum));
                if (Editor.DeviceModel.vScrollBar1.Track.ViewportSize != Classes.Core.Solution.SceneHeight) Editor.DeviceModel.vScrollBar1.Track.ViewportSize = Classes.Core.Solution.SceneHeight;
            }
            else
            {
                Classes.Core.SolutionState.ScreenHeight = Editor.DeviceModel.GraphicPanel.Height;
                Classes.Core.SolutionState.ViewPositionY = 0;
                Editor.DeviceModel.vScrollBar1.Value = 0;
            }
            if (Editor.DeviceModel.hScrollBar1.IsVisible)
            {
                Editor.DeviceModel.hScrollBar1.LargeChange = Editor.DeviceModel.hScrollBar1Host.Width;
                Editor.DeviceModel.hScrollBar1.SmallChange = Editor.DeviceModel.hScrollBar1Host.Width / 8;
                Classes.Core.SolutionState.ScreenWidth = (int)Editor.DeviceModel.hScrollBar1Host.Width;
                Editor.DeviceModel.hScrollBar1.Value = Math.Max(0, Math.Min(Editor.DeviceModel.hScrollBar1.Value, Editor.DeviceModel.hScrollBar1.Maximum));
                if (Editor.DeviceModel.hScrollBar1.Track.ViewportSize != Classes.Core.Solution.SceneWidth) Editor.DeviceModel.hScrollBar1.Track.ViewportSize = Classes.Core.Solution.SceneWidth;
            }
            else
            {
                Classes.Core.SolutionState.ScreenWidth = Editor.DeviceModel.GraphicPanel.Width;
                Classes.Core.SolutionState.ViewPositionX = 0;
                Editor.DeviceModel.hScrollBar1.Value = 0;
            }

            while (Classes.Core.SolutionState.ScreenWidth > Editor.DeviceModel.GraphicPanel.Width)
                ResizeGraphicPanel(Editor.DeviceModel.GraphicPanel.Width * 2, Editor.DeviceModel.GraphicPanel.Height);
            while (Classes.Core.SolutionState.ScreenHeight > Editor.DeviceModel.GraphicPanel.Height)
                ResizeGraphicPanel(Editor.DeviceModel.GraphicPanel.Width, Editor.DeviceModel.GraphicPanel.Height * 2);
        }

        public void SetViewSize(int width = 0, int height = 0, bool resizeForm = true)
        {
            if (Core.Settings.MySettings.EntityFreeCam)
            {
                width = 10000000;
                height = 10000000;
            }

            Editor.DeviceModel.vScrollBar1.Maximum = height - Editor.DeviceModel.vScrollBar1Host.Height;
            Editor.DeviceModel.hScrollBar1.Maximum = width - Editor.DeviceModel.hScrollBar1Host.Width;

            Editor.DeviceModel.GraphicPanel.DrawWidth = Math.Min((int)width, Editor.DeviceModel.GraphicPanel.Width);
            Editor.DeviceModel.GraphicPanel.DrawHeight = Math.Min((int)height, Editor.DeviceModel.GraphicPanel.Height);

            if (resizeForm) Resize(null, null);

            if (!Core.Settings.MySettings.EntityFreeCam)
            {
                Editor.DeviceModel.hScrollBar1.Value = Math.Max(0, Math.Min(Editor.DeviceModel.hScrollBar1.Value, Editor.DeviceModel.hScrollBar1.Maximum));
                Editor.DeviceModel.vScrollBar1.Value = Math.Max(0, Math.Min(Editor.DeviceModel.vScrollBar1.Value, Editor.DeviceModel.vScrollBar1.Maximum));
            }

        }

        #region Zooming/Resizing Related Methods

        public void SetZoomLevel(int zoom_level, System.Drawing.Point zoom_point, double zoom_level_d = 0.0, bool updateControls = true)
        {
            double old_zoom = Classes.Core.SolutionState.Zoom;



            if (zoom_level_d == 0.0)
            {
                Classes.Core.SolutionState.ZoomLevel = zoom_level;
                switch (Classes.Core.SolutionState.ZoomLevel)
                {
                    case 5: Classes.Core.SolutionState.Zoom = 4; break;
                    case 4: Classes.Core.SolutionState.Zoom = 3; break;
                    case 3: Classes.Core.SolutionState.Zoom = 2; break;
                    case 2: Classes.Core.SolutionState.Zoom = 3 / 2.0; break;
                    case 1: Classes.Core.SolutionState.Zoom = 5 / 4.0; break;
                    case 0: Classes.Core.SolutionState.Zoom = 1; break;
                    case -1: Classes.Core.SolutionState.Zoom = 2 / 3.0; break;
                    case -2: Classes.Core.SolutionState.Zoom = 1 / 2.0; break;
                    case -3: Classes.Core.SolutionState.Zoom = 1 / 3.0; break;
                    case -4: Classes.Core.SolutionState.Zoom = 1 / 4.0; break;
                    case -5: Classes.Core.SolutionState.Zoom = 1 / 8.0; break;
                }
            }
            else
            {
                Classes.Core.SolutionState.ZoomLevel = (int)zoom_level_d;
                Classes.Core.SolutionState.Zoom = zoom_level_d;
            }


            Classes.Core.SolutionState.Zooming = true;

            int oldShiftX = Classes.Core.SolutionState.ViewPositionX;
            int oldShiftY = Classes.Core.SolutionState.ViewPositionY;

            if (Classes.Core.Solution.CurrentScene != null)
                SetViewSize((int)(Classes.Core.Solution.SceneWidth * Classes.Core.SolutionState.Zoom), (int)(Classes.Core.Solution.SceneHeight * Classes.Core.SolutionState.Zoom), updateControls);


            if (Editor.DeviceModel.hScrollBar1.IsVisible)
            {
                Classes.Core.SolutionState.ViewPositionX = (int)((zoom_point.X + oldShiftX) / old_zoom * Classes.Core.SolutionState.Zoom - zoom_point.X);
                Classes.Core.SolutionState.ViewPositionX = (int)Math.Min((Editor.DeviceModel.hScrollBar1.Maximum), Math.Max(0, Classes.Core.SolutionState.ViewPositionX));
                Editor.DeviceModel.hScrollBar1.Value = Classes.Core.SolutionState.ViewPositionX;
            }
            if (Editor.DeviceModel.vScrollBar1.IsVisible)
            {
                Classes.Core.SolutionState.ViewPositionY = (int)((zoom_point.Y + oldShiftY) / old_zoom * Classes.Core.SolutionState.Zoom - zoom_point.Y);
                Classes.Core.SolutionState.ViewPositionY = (int)Math.Min((Editor.DeviceModel.vScrollBar1.Maximum), Math.Max(0, Classes.Core.SolutionState.ViewPositionY));
                Editor.DeviceModel.vScrollBar1.Value = Classes.Core.SolutionState.ViewPositionY;
            }


            Classes.Core.SolutionState.Zooming = false;

            if (updateControls) Editor.UI.UpdateControls();
        }

        public void ResetViewSize()
        {
            Editor.ZoomModel.SetViewSize((int)(Classes.Core.Solution.SceneWidth * Classes.Core.SolutionState.Zoom), (int)(Classes.Core.Solution.SceneHeight * Classes.Core.SolutionState.Zoom));
        }
        public void ResizeGraphicPanel(int width = 0, int height = 0)
        {
            if (Core.Settings.MySettings.EntityFreeCam)
            {
                width = Classes.Core.Solution.SceneWidth;
                height = Classes.Core.Solution.SceneHeight;
            }

            Editor.DeviceModel.GraphicPanel.Width = width;
            Editor.DeviceModel.GraphicPanel.Height = height;

            Editor.DeviceModel.GraphicPanel.ResetDevice();

            Editor.DeviceModel.GraphicPanel.DrawWidth = Math.Min((int)Editor.DeviceModel.hScrollBar1.Maximum, Editor.DeviceModel.GraphicPanel.Width);
            Editor.DeviceModel.GraphicPanel.DrawHeight = Math.Min((int)Editor.DeviceModel.vScrollBar1.Maximum, Editor.DeviceModel.GraphicPanel.Height);
        }
        #endregion
    }
}
