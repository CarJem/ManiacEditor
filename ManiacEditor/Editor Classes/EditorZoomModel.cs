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
                EditorStateModel.ViewPositionY = (int)e.NewValue;
                UpdateScrollBars();
            }
            Editor.FormsModel.GraphicPanel.Render();
        }

        public void HScrollBar1_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {
            if (AllowScrollUpdate)
            {
                EditorStateModel.ViewPositionX = (int)e.NewValue;
                UpdateScrollBars();
            }
            Editor.FormsModel.GraphicPanel.Render();
        }

        public void VScrollBar1_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (AllowScrollUpdate)
            {
                EditorStateModel.ViewPositionY = (int)Editor.FormsModel.vScrollBar1.Value;
                UpdateScrollBars();
            }
            if (!(Editor.StateModel.Zooming || Editor.StateModel.DraggingSelection || Editor.StateModel.Dragged || Editor.StateModel.Scrolling)) Editor.FormsModel.GraphicPanel.Render();
            if (Editor.StateModel.DraggingSelection)
            {
                Editor.FormsModel.GraphicPanel.OnMouseMoveEventCreate();
            }

        }

        public void HScrollBar1_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (AllowScrollUpdate)
            {
                EditorStateModel.ViewPositionX = (int)Editor.FormsModel.hScrollBar1.Value;
                UpdateScrollBars();
            }
            if (!(Editor.StateModel.Zooming || Editor.StateModel.DraggingSelection || Editor.StateModel.Dragged || Editor.StateModel.Scrolling)) Editor.FormsModel.GraphicPanel.Render();
            if (Editor.StateModel.DraggingSelection)
            {
                Editor.FormsModel.GraphicPanel.OnMouseMoveEventCreate();
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
            if (!Editor.UIModes.ScrollLocked)
            {
                Editor.UIModes.ScrollDirection = (int)ScrollDir.Y;
            }
        }

        public void HScrollBar1_Entered(object sender, EventArgs e)
        {
            if (!Editor.UIModes.ScrollLocked)
            {
                Editor.UIModes.ScrollDirection = (int)ScrollDir.X;
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
                Editor.StateModel.ScreenHeight = (int)Editor.FormsModel.vScrollBar1Host.Height;
                Editor.FormsModel.vScrollBar1.Value = Math.Max(0, Math.Min(Editor.FormsModel.vScrollBar1.Value, Editor.FormsModel.vScrollBar1.Maximum));
                if (Editor.FormsModel.vScrollBar1.Track.ViewportSize != Editor.SceneHeight) Editor.FormsModel.vScrollBar1.Track.ViewportSize = Editor.SceneHeight;
            }
            else
            {
                Editor.StateModel.ScreenHeight = Editor.FormsModel.GraphicPanel.Height;
                EditorStateModel.ViewPositionY = 0;
                Editor.FormsModel.vScrollBar1.Value = 0;
            }
            if (Editor.FormsModel.hScrollBar1.IsVisible)
            {
                Editor.FormsModel.hScrollBar1.LargeChange = Editor.FormsModel.hScrollBar1Host.Width;
                Editor.FormsModel.hScrollBar1.SmallChange = Editor.FormsModel.hScrollBar1Host.Width / 8;
                Editor.StateModel.ScreenWidth = (int)Editor.FormsModel.hScrollBar1Host.Width;
                Editor.FormsModel.hScrollBar1.Value = Math.Max(0, Math.Min(Editor.FormsModel.hScrollBar1.Value, Editor.FormsModel.hScrollBar1.Maximum));
                if (Editor.FormsModel.hScrollBar1.Track.ViewportSize != Editor.SceneWidth) Editor.FormsModel.hScrollBar1.Track.ViewportSize = Editor.SceneWidth;
            }
            else
            {
                Editor.StateModel.ScreenWidth = Editor.FormsModel.GraphicPanel.Width;
                EditorStateModel.ViewPositionX = 0;
                Editor.FormsModel.hScrollBar1.Value = 0;
            }

            while (Editor.StateModel.ScreenWidth > Editor.FormsModel.GraphicPanel.Width)
                ResizeGraphicPanel(Editor.FormsModel.GraphicPanel.Width * 2, Editor.FormsModel.GraphicPanel.Height);
            while (Editor.StateModel.ScreenHeight > Editor.FormsModel.GraphicPanel.Height)
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
            double old_zoom = Editor.StateModel.Zoom;



            if (zoom_level_d == 0.0)
            {
                Editor.StateModel.ZoomLevel = zoom_level;
                switch (Editor.StateModel.ZoomLevel)
                {
                    case 5: Editor.StateModel.Zoom = 4; break;
                    case 4: Editor.StateModel.Zoom = 3; break;
                    case 3: Editor.StateModel.Zoom = 2; break;
                    case 2: Editor.StateModel.Zoom = 3 / 2.0; break;
                    case 1: Editor.StateModel.Zoom = 5 / 4.0; break;
                    case 0: Editor.StateModel.Zoom = 1; break;
                    case -1: Editor.StateModel.Zoom = 2 / 3.0; break;
                    case -2: Editor.StateModel.Zoom = 1 / 2.0; break;
                    case -3: Editor.StateModel.Zoom = 1 / 3.0; break;
                    case -4: Editor.StateModel.Zoom = 1 / 4.0; break;
                    case -5: Editor.StateModel.Zoom = 1 / 8.0; break;
                }
            }
            else
            {
                Editor.StateModel.ZoomLevel = (int)zoom_level_d;
                Editor.StateModel.Zoom = zoom_level_d;
            }


            Editor.StateModel.Zooming = true;

            int oldShiftX = EditorStateModel.ViewPositionX;
            int oldShiftY = EditorStateModel.ViewPositionY;

            if (Editor.EditorScene != null)
                SetViewSize((int)(Editor.SceneWidth * Editor.StateModel.Zoom), (int)(Editor.SceneHeight * Editor.StateModel.Zoom), updateControls);


            if (Editor.FormsModel.hScrollBar1.IsVisible)
            {
                EditorStateModel.ViewPositionX = (int)((zoom_point.X + oldShiftX) / old_zoom * Editor.StateModel.Zoom - zoom_point.X);
                EditorStateModel.ViewPositionX = (int)Math.Min((Editor.FormsModel.hScrollBar1.Maximum), Math.Max(0, EditorStateModel.ViewPositionX));
                Editor.FormsModel.hScrollBar1.Value = EditorStateModel.ViewPositionX;
            }
            if (Editor.FormsModel.vScrollBar1.IsVisible)
            {
                EditorStateModel.ViewPositionY = (int)((zoom_point.Y + oldShiftY) / old_zoom * Editor.StateModel.Zoom - zoom_point.Y);
                EditorStateModel.ViewPositionY = (int)Math.Min((Editor.FormsModel.vScrollBar1.Maximum), Math.Max(0, EditorStateModel.ViewPositionY));
                Editor.FormsModel.vScrollBar1.Value = EditorStateModel.ViewPositionY;
            }


            Editor.StateModel.Zooming = false;

            if (updateControls) Editor.UI.UpdateControls();
        }

        public void ResetViewSize()
        {
            Editor.ZoomModel.SetViewSize((int)(Editor.SceneWidth * Editor.StateModel.Zoom), (int)(Editor.SceneHeight * Editor.StateModel.Zoom));
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
