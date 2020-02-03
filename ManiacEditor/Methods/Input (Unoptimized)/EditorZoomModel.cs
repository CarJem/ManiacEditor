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
                Classes.Editor.SolutionState.ViewPositionY = (int)e.NewValue;
                UpdateScrollBars();
            }
            Editor.DeviceModel.GraphicPanel.Render();
        }

        public void HScrollBar1_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {
            if (AllowScrollUpdate)
            {
                Classes.Editor.SolutionState.ViewPositionX = (int)e.NewValue;
                UpdateScrollBars();
            }
            Editor.DeviceModel.GraphicPanel.Render();
        }

        public void VScrollBar1_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (AllowScrollUpdate)
            {
                Classes.Editor.SolutionState.ViewPositionY = (int)Editor.DeviceModel.vScrollBar1.Value;
                UpdateScrollBars();
            }
            //TODO: Determine if we still need this
            //if (!(Classes.Edit.SolutionState.Zooming || Classes.Edit.SolutionState.DraggingSelection || Classes.Edit.SolutionState.Dragged || Classes.Edit.SolutionState.Scrolling)) Editor.FormsModel.GraphicPanel.Render();
            if (Classes.Editor.SolutionState.DraggingSelection)
            {
                Editor.DeviceModel.GraphicPanel.OnMouseMoveEventCreate();
            }

        }

        public void HScrollBar1_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (AllowScrollUpdate)
            {
                Classes.Editor.SolutionState.ViewPositionX = (int)Editor.DeviceModel.hScrollBar1.Value;
                UpdateScrollBars();
            }
            //TODO: Determine if we still need this
            //if (!(Classes.Edit.SolutionState.Zooming || Classes.Edit.SolutionState.DraggingSelection || Classes.Edit.SolutionState.Dragged || Classes.Edit.SolutionState.Scrolling)) Editor.FormsModel.GraphicPanel.Render();
            if (Classes.Editor.SolutionState.DraggingSelection)
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
            if (!Classes.Editor.SolutionState.ScrollLocked)
            {
                Classes.Editor.SolutionState.ScrollDirection = (int)ScrollDir.Y;
            }
        }

        public void HScrollBar1_Entered(object sender, EventArgs e)
        {
            if (!Classes.Editor.SolutionState.ScrollLocked)
            {
                Classes.Editor.SolutionState.ScrollDirection = (int)ScrollDir.X;
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
                Classes.Editor.SolutionState.ScreenHeight = (int)Editor.DeviceModel.vScrollBar1Host.Height;
                Editor.DeviceModel.vScrollBar1.Value = Math.Max(0, Math.Min(Editor.DeviceModel.vScrollBar1.Value, Editor.DeviceModel.vScrollBar1.Maximum));
                if (Editor.DeviceModel.vScrollBar1.Track.ViewportSize != Classes.Editor.Solution.SceneHeight) Editor.DeviceModel.vScrollBar1.Track.ViewportSize = Classes.Editor.Solution.SceneHeight;
            }
            else
            {
                Classes.Editor.SolutionState.ScreenHeight = Editor.DeviceModel.GraphicPanel.Height;
                Classes.Editor.SolutionState.ViewPositionY = 0;
                Editor.DeviceModel.vScrollBar1.Value = 0;
            }
            if (Editor.DeviceModel.hScrollBar1.IsVisible)
            {
                Editor.DeviceModel.hScrollBar1.LargeChange = Editor.DeviceModel.hScrollBar1Host.Width;
                Editor.DeviceModel.hScrollBar1.SmallChange = Editor.DeviceModel.hScrollBar1Host.Width / 8;
                Classes.Editor.SolutionState.ScreenWidth = (int)Editor.DeviceModel.hScrollBar1Host.Width;
                Editor.DeviceModel.hScrollBar1.Value = Math.Max(0, Math.Min(Editor.DeviceModel.hScrollBar1.Value, Editor.DeviceModel.hScrollBar1.Maximum));
                if (Editor.DeviceModel.hScrollBar1.Track.ViewportSize != Classes.Editor.Solution.SceneWidth) Editor.DeviceModel.hScrollBar1.Track.ViewportSize = Classes.Editor.Solution.SceneWidth;
            }
            else
            {
                Classes.Editor.SolutionState.ScreenWidth = Editor.DeviceModel.GraphicPanel.Width;
                Classes.Editor.SolutionState.ViewPositionX = 0;
                Editor.DeviceModel.hScrollBar1.Value = 0;
            }

            while (Classes.Editor.SolutionState.ScreenWidth > Editor.DeviceModel.GraphicPanel.Width)
                ResizeGraphicPanel(Editor.DeviceModel.GraphicPanel.Width * 2, Editor.DeviceModel.GraphicPanel.Height);
            while (Classes.Editor.SolutionState.ScreenHeight > Editor.DeviceModel.GraphicPanel.Height)
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
            double old_zoom = Classes.Editor.SolutionState.Zoom;



            if (zoom_level_d == 0.0)
            {
                Classes.Editor.SolutionState.ZoomLevel = zoom_level;
                switch (Classes.Editor.SolutionState.ZoomLevel)
                {
                    case 5: Classes.Editor.SolutionState.Zoom = 4; break;
                    case 4: Classes.Editor.SolutionState.Zoom = 3; break;
                    case 3: Classes.Editor.SolutionState.Zoom = 2; break;
                    case 2: Classes.Editor.SolutionState.Zoom = 3 / 2.0; break;
                    case 1: Classes.Editor.SolutionState.Zoom = 5 / 4.0; break;
                    case 0: Classes.Editor.SolutionState.Zoom = 1; break;
                    case -1: Classes.Editor.SolutionState.Zoom = 2 / 3.0; break;
                    case -2: Classes.Editor.SolutionState.Zoom = 1 / 2.0; break;
                    case -3: Classes.Editor.SolutionState.Zoom = 1 / 3.0; break;
                    case -4: Classes.Editor.SolutionState.Zoom = 1 / 4.0; break;
                    case -5: Classes.Editor.SolutionState.Zoom = 1 / 8.0; break;
                }
            }
            else
            {
                Classes.Editor.SolutionState.ZoomLevel = (int)zoom_level_d;
                Classes.Editor.SolutionState.Zoom = zoom_level_d;
            }


            Classes.Editor.SolutionState.Zooming = true;

            int oldShiftX = Classes.Editor.SolutionState.ViewPositionX;
            int oldShiftY = Classes.Editor.SolutionState.ViewPositionY;

            if (Classes.Editor.Solution.CurrentScene != null)
                SetViewSize((int)(Classes.Editor.Solution.SceneWidth * Classes.Editor.SolutionState.Zoom), (int)(Classes.Editor.Solution.SceneHeight * Classes.Editor.SolutionState.Zoom), updateControls);


            if (Editor.DeviceModel.hScrollBar1.IsVisible)
            {
                Classes.Editor.SolutionState.ViewPositionX = (int)((zoom_point.X + oldShiftX) / old_zoom * Classes.Editor.SolutionState.Zoom - zoom_point.X);
                Classes.Editor.SolutionState.ViewPositionX = (int)Math.Min((Editor.DeviceModel.hScrollBar1.Maximum), Math.Max(0, Classes.Editor.SolutionState.ViewPositionX));
                Editor.DeviceModel.hScrollBar1.Value = Classes.Editor.SolutionState.ViewPositionX;
            }
            if (Editor.DeviceModel.vScrollBar1.IsVisible)
            {
                Classes.Editor.SolutionState.ViewPositionY = (int)((zoom_point.Y + oldShiftY) / old_zoom * Classes.Editor.SolutionState.Zoom - zoom_point.Y);
                Classes.Editor.SolutionState.ViewPositionY = (int)Math.Min((Editor.DeviceModel.vScrollBar1.Maximum), Math.Max(0, Classes.Editor.SolutionState.ViewPositionY));
                Editor.DeviceModel.vScrollBar1.Value = Classes.Editor.SolutionState.ViewPositionY;
            }


            Classes.Editor.SolutionState.Zooming = false;

            if (updateControls) Methods.Internal.UserInterface.UpdateControls();
        }

        public void ResetViewSize()
        {
            Editor.ZoomModel.SetViewSize((int)(Classes.Editor.Solution.SceneWidth * Classes.Editor.SolutionState.Zoom), (int)(Classes.Editor.Solution.SceneHeight * Classes.Editor.SolutionState.Zoom));
        }
        public void ResizeGraphicPanel(int width = 0, int height = 0)
        {
            if (Core.Settings.MySettings.EntityFreeCam)
            {
                width = Classes.Editor.Solution.SceneWidth;
                height = Classes.Editor.Solution.SceneHeight;
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
