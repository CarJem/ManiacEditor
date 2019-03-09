using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ManiacEditor
{
    public class EditorViewModel
    {
        private bool AllowScrollUpdate = true;
        private Editor Editor;
        public EditorViewModel(Editor instance)
        {
            Editor = instance;
        }

        public void VScrollBar1_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {
            if (AllowScrollUpdate)
            {
                Editor.EditorState.ShiftY = (int)e.NewValue;
                UpdateScrollBars();
            }
            Editor.GraphicsModel.GraphicPanel.Render();
        }

        public void HScrollBar1_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {
            if (AllowScrollUpdate)
            {
                Editor.EditorState.ShiftX = (int)e.NewValue;
                UpdateScrollBars();
            }
            Editor.GraphicsModel.GraphicPanel.Render();
        }

        public void VScrollBar1_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (AllowScrollUpdate)
            {
                Editor.EditorState.ShiftY = (int)Editor.GraphicsModel.vScrollBar1.Value;
                UpdateScrollBars();
            }
            if (!(Editor.EditorState.zooming || Editor.EditorState.draggingSelection || Editor.EditorState.dragged || Editor.EditorState.scrolling)) Editor.GraphicsModel.GraphicPanel.Render();
            if (Editor.EditorState.draggingSelection)
            {
                Editor.GraphicsModel.GraphicPanel.OnMouseMoveEventCreate();
            }

        }

        public void HScrollBar1_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (AllowScrollUpdate)
            {
                Editor.EditorState.ShiftX = (int)Editor.GraphicsModel.hScrollBar1.Value;
                UpdateScrollBars();
            }
            if (!(Editor.EditorState.zooming || Editor.EditorState.draggingSelection || Editor.EditorState.dragged || Editor.EditorState.scrolling)) Editor.GraphicsModel.GraphicPanel.Render();
            if (Editor.EditorState.draggingSelection)
            {
                Editor.GraphicsModel.GraphicPanel.OnMouseMoveEventCreate();
            }

        }

        public void UpdateScrollBars()
        {
            /*AllowScrollUpdate = false;
            Editor.editorView.hScrollBar1.Value = (int)Editor.editorView.hScrollBar1.Value;
            Editor.editorView.vScrollBar1.Value = (int)Editor.editorView.vScrollBar1.Value;
            AllowScrollUpdate = true;*/
        }

        public void VScrollBar1_Entered(object sender, EventArgs e)
        {
            if (!Editor.ScrollLocked)
            {
                Editor.ScrollDirection = (int)ScrollDir.Y;
            }
        }

        public void HScrollBar1_Entered(object sender, EventArgs e)
        {
            if (!Editor.ScrollLocked)
            {
                Editor.ScrollDirection = (int)ScrollDir.X;
            }
        }

        public void Form1_Resize(object sender, RoutedEventArgs e)
        {
            
            // TODO: It hides right now few pixels at the edge

            Visibility nvscrollbar = Visibility.Visible;
            Visibility nhscrollbar = Visibility.Visible;

            if (Editor.GraphicsModel.hScrollBar1.Maximum == 0) nhscrollbar = Visibility.Hidden;
            if (Editor.GraphicsModel.vScrollBar1.Maximum == 0) nvscrollbar = Visibility.Hidden;

            Editor.GraphicsModel.vScrollBar1.Visibility = nvscrollbar;
            Editor.GraphicsModel.vScrollBar1Host.Child.Visibility = nvscrollbar;
            Editor.GraphicsModel.hScrollBar1Host.Child.Visibility = nhscrollbar;
            Editor.GraphicsModel.hScrollBar1.Visibility = nhscrollbar;

            if (Editor.GraphicsModel.vScrollBar1.IsVisible)
            {
                Editor.GraphicsModel.vScrollBar1.LargeChange = Editor.GraphicsModel.vScrollBar1Host.Height;
                Editor.GraphicsModel.vScrollBar1.SmallChange = Editor.GraphicsModel.vScrollBar1Host.Height / 8;
                Editor.EditorState.ScreenHeight = (int)Editor.GraphicsModel.vScrollBar1Host.Height;
                Editor.GraphicsModel.vScrollBar1.Value = Math.Max(0, Math.Min(Editor.GraphicsModel.vScrollBar1.Value, Editor.GraphicsModel.vScrollBar1.Maximum));
                if (Editor.GraphicsModel.vScrollBar1.Track.ViewportSize != Editor.SceneHeight) Editor.GraphicsModel.vScrollBar1.Track.ViewportSize = Editor.SceneHeight;
            }
            else
            {
                Editor.EditorState.ScreenHeight = Editor.GraphicsModel.GraphicPanel.Height;
                Editor.EditorState.ShiftY = 0;
                Editor.GraphicsModel.vScrollBar1.Value = 0;
            }
            if (Editor.GraphicsModel.hScrollBar1.IsVisible)
            {
                Editor.GraphicsModel.hScrollBar1.LargeChange = Editor.GraphicsModel.hScrollBar1Host.Width;
                Editor.GraphicsModel.hScrollBar1.SmallChange = Editor.GraphicsModel.hScrollBar1Host.Width / 8;
                Editor.EditorState.ScreenWidth = (int)Editor.GraphicsModel.hScrollBar1Host.Width;
                Editor.GraphicsModel.hScrollBar1.Value = Math.Max(0, Math.Min(Editor.GraphicsModel.hScrollBar1.Value, Editor.GraphicsModel.hScrollBar1.Maximum));
                if (Editor.GraphicsModel.hScrollBar1.Track.ViewportSize != Editor.SceneWidth) Editor.GraphicsModel.hScrollBar1.Track.ViewportSize = Editor.SceneWidth;
            }
            else
            {
                Editor.EditorState.ScreenWidth = Editor.GraphicsModel.GraphicPanel.Width;
                Editor.EditorState.ShiftX = 0;
                Editor.GraphicsModel.hScrollBar1.Value = 0;
            }

            while (Editor.EditorState.ScreenWidth > Editor.GraphicsModel.GraphicPanel.Width)
                Editor.ResizeGraphicPanel(Editor.GraphicsModel.GraphicPanel.Width * 2, Editor.GraphicsModel.GraphicPanel.Height);
            while (Editor.EditorState.ScreenHeight > Editor.GraphicsModel.GraphicPanel.Height)
                Editor.ResizeGraphicPanel(Editor.GraphicsModel.GraphicPanel.Width, Editor.GraphicsModel.GraphicPanel.Height * 2);
        }

        public void SetViewSize(int width = 0, int height = 0, bool resizeForm = true)
        {
            if (Settings.mySettings.EntityFreeCam)
            {
                width = 10000000;
                height = 10000000;
            }

            Editor.GraphicsModel.vScrollBar1.Maximum = height - Editor.GraphicsModel.vScrollBar1Host.Height;
            Editor.GraphicsModel.hScrollBar1.Maximum = width - Editor.GraphicsModel.hScrollBar1Host.Width;

            Editor.GraphicsModel.GraphicPanel.DrawWidth = Math.Min((int)width, Editor.GraphicsModel.GraphicPanel.Width);
            Editor.GraphicsModel.GraphicPanel.DrawHeight = Math.Min((int)height, Editor.GraphicsModel.GraphicPanel.Height);

            if (resizeForm) Form1_Resize(null, null);

            if (!Settings.mySettings.EntityFreeCam)
            {
                Editor.GraphicsModel.hScrollBar1.Value = Math.Max(0, Math.Min(Editor.GraphicsModel.hScrollBar1.Value, Editor.GraphicsModel.hScrollBar1.Maximum));
                Editor.GraphicsModel.vScrollBar1.Value = Math.Max(0, Math.Min(Editor.GraphicsModel.vScrollBar1.Value, Editor.GraphicsModel.vScrollBar1.Maximum));
            }

        }

        #region Zooming/Resizing Related Methods

        public void SetZoomLevel(int zoom_level, System.Drawing.Point zoom_point, double zoom_level_d = 0.0, bool updateControls = true)
        {
            double old_zoom = Editor.EditorState.Zoom;



            if (zoom_level_d == 0.0)
            {
                Editor.EditorState.ZoomLevel = zoom_level;
                switch (Editor.EditorState.ZoomLevel)
                {
                    case 5: Editor.EditorState.Zoom = 4; break;
                    case 4: Editor.EditorState.Zoom = 3; break;
                    case 3: Editor.EditorState.Zoom = 2; break;
                    case 2: Editor.EditorState.Zoom = 3 / 2.0; break;
                    case 1: Editor.EditorState.Zoom = 5 / 4.0; break;
                    case 0: Editor.EditorState.Zoom = 1; break;
                    case -1: Editor.EditorState.Zoom = 2 / 3.0; break;
                    case -2: Editor.EditorState.Zoom = 1 / 2.0; break;
                    case -3: Editor.EditorState.Zoom = 1 / 3.0; break;
                    case -4: Editor.EditorState.Zoom = 1 / 4.0; break;
                    case -5: Editor.EditorState.Zoom = 1 / 8.0; break;
                }
            }
            else
            {
                Editor.EditorState.ZoomLevel = (int)zoom_level_d;
                Editor.EditorState.Zoom = zoom_level_d;
            }


            Editor.EditorState.zooming = true;

            int oldShiftX = Editor.EditorState.ShiftX;
            int oldShiftY = Editor.EditorState.ShiftY;

            if (Editor.EditorScene != null)
                SetViewSize((int)(Editor.SceneWidth * Editor.EditorState.Zoom), (int)(Editor.SceneHeight * Editor.EditorState.Zoom), updateControls);


            if (Editor.GraphicsModel.hScrollBar1.IsVisible)
            {
                Editor.EditorState.ShiftX = (int)((zoom_point.X + oldShiftX) / old_zoom * Editor.EditorState.Zoom - zoom_point.X);
                Editor.EditorState.ShiftX = (int)Math.Min((Editor.GraphicsModel.hScrollBar1.Maximum), Math.Max(0, Editor.EditorState.ShiftX));
                Editor.GraphicsModel.hScrollBar1.Value = Editor.EditorState.ShiftX;
            }
            if (Editor.GraphicsModel.vScrollBar1.IsVisible)
            {
                Editor.EditorState.ShiftY = (int)((zoom_point.Y + oldShiftY) / old_zoom * Editor.EditorState.Zoom - zoom_point.Y);
                Editor.EditorState.ShiftY = (int)Math.Min((Editor.GraphicsModel.vScrollBar1.Maximum), Math.Max(0, Editor.EditorState.ShiftY));
                Editor.GraphicsModel.vScrollBar1.Value = Editor.EditorState.ShiftY;
            }


            Editor.EditorState.zooming = false;

            if (updateControls) Editor.UI.UpdateControls();
        }

        public void ResetViewSize()
        {
            Editor.EditorView.SetViewSize((int)(Editor.SceneWidth * Editor.EditorState.Zoom), (int)(Editor.SceneHeight * Editor.EditorState.Zoom));
        }
        public void ResizeGraphicPanel(int width = 0, int height = 0)
        {
            if (Settings.mySettings.EntityFreeCam)
            {
                width = Editor.SceneWidth;
                height = Editor.SceneHeight;
            }

            Editor.GraphicsModel.GraphicPanel.Width = width;
            Editor.GraphicsModel.GraphicPanel.Height = height;

            Editor.GraphicsModel.GraphicPanel.ResetDevice();

            Editor.GraphicsModel.GraphicPanel.DrawWidth = Math.Min((int)Editor.GraphicsModel.hScrollBar1.Maximum, Editor.GraphicsModel.GraphicPanel.Width);
            Editor.GraphicsModel.GraphicPanel.DrawHeight = Math.Min((int)Editor.GraphicsModel.vScrollBar1.Maximum, Editor.GraphicsModel.GraphicPanel.Height);
        }
        #endregion
    }
}
