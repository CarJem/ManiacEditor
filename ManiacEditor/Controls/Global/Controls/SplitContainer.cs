using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Controls.Primitives;
using System.Windows.Shapes;


namespace ManiacEditor.Controls.Global.Controls
{
    public static class Commands
    {
        public static RoutedCommand SizeChanged = new RoutedCommand("SizeChanged", typeof(Commands));
        public static RoutedCommand DragDelta = new RoutedCommand("DragDelta", typeof(Commands));
    }
    /// <summary>
    /// Interaction logic for SplitContainer.xaml
    /// </summary>
    public class SplitContainer : Control
    {

        public SplitContainer()
        {
            this.DefaultStyleKey = typeof(SplitContainer);

            this.CommandBindings.Add(new CommandBinding(Commands.SizeChanged, Spliter_SizeChanged));
            this.CommandBindings.Add(new CommandBinding(Commands.DragDelta, Spliter_DragDelta));
        }

        public event EventHandler MyEvent;

        private void OnEvent()
        {
            if (MyEvent != null)
            {
                MyEvent(this, EventArgs.Empty);
            }
        }

        #region LeftContainer
        public object LeftContainer
        {
            get { return (object)GetValue(LeftContainerProperty); }
            set { SetValue(LeftContainerProperty, value); }
        }
        public static readonly DependencyProperty LeftContainerProperty = DependencyProperty.Register("LeftContainer", typeof(object), typeof(SplitContainer), null);

        #endregion

        #region RightContainer
        public object RightContainer
        {
            get { return (object)GetValue(RightContainerProperty); }
            set { SetValue(RightContainerProperty, value); }
        }
        public static readonly DependencyProperty RightContainerProperty = DependencyProperty.Register("RightContainer", typeof(object), typeof(SplitContainer),
              null);
        #endregion

        #region CenterContainer
        public object CenterContainer
        {
            get { return (object)GetValue(CenterContainerProperty); }
            set { SetValue(CenterContainerProperty, value); }
        }
        public static readonly DependencyProperty CenterContainerProperty =
            DependencyProperty.Register("CenterContainer", typeof(object), typeof(SplitContainer),
              null);
        #endregion

        private ManiacEditor.Controls.Editor.MainEditor Instance { get; set; }

        public void UpdateInstance(ManiacEditor.Controls.Editor.MainEditor instance)
        {
            Instance = instance;
        }

        #region Splitter Events
        private void Spliter_DragDelta(object sender, ExecutedRoutedEventArgs e)
        {

        }
        private void Spliter_SizeChanged(object sender, ExecutedRoutedEventArgs e)
        {

        }
        #endregion

        #region Accessors

        public ColumnDefinition ToolbarRight
        {
            get
            {
                var template = this.Template;
                return (ColumnDefinition)template.FindName("ToolbarRight", this);
            }
        }

        public ColumnDefinition SplitterRight
        {
            get
            {
                var template = this.Template;
                return (ColumnDefinition)template.FindName("SplitterRight", this);
            }
        }

        public GridSplitter SpliterRight
        {
            get
            {
                var template = this.Template;
                return (GridSplitter)template.FindName("SpliterRight", this);
            }
        }

        public ColumnDefinition ToolbarLeft
        {
            get
            {
                var template = this.Template;
                return (ColumnDefinition)template.FindName("ToolbarLeft", this);
            }
        }

        public ColumnDefinition SplitterLeft
        {
            get
            {
                var template = this.Template;
                return (ColumnDefinition)template.FindName("SplitterLeft", this);
            }
        }

        public GridSplitter SpliterLeft
        {
            get
            {
                var template = this.Template;
                return (GridSplitter)template.FindName("SpliterLeft", this);
            }
        }

        public ContentPresenter CenterPanel
        {
            get
            {
                var template = this.Template;
                return (ContentPresenter)template.FindName("CenterPanel", this);
            }
        }

        public ContentPresenter PanelLeft
        {
            get
            {
                var template = this.Template;
                return (ContentPresenter)template.FindName("PanelLeft", this);
            }
        }

        public ContentPresenter PanelRight
        {
            get
            {
                var template = this.Template;
                return (ContentPresenter)template.FindName("PanelRight", this);
            }
        }

        #endregion

        public void UpdateToolbars(bool rightToolbar = true, bool visible = false)
        {
            if (ToolbarRight == null || SplitterRight == null || ToolbarLeft == null || SplitterLeft == null) return;
            if (rightToolbar)
            {
                if (visible)
                {
                    ToolbarRight.Width = new GridLength(300);
                    ToolbarRight.MinWidth = 300;
                    ToolbarRight.MaxWidth = CenterPanel.ActualWidth / 3;
                    SplitterRight.Width = new GridLength(6);
                    SplitterRight.MinWidth = 6;
                }
                else
                {
                    ToolbarRight.Width = new GridLength(0);
                    ToolbarRight.MinWidth = 0;
                    ToolbarRight.MaxWidth = 0;
                    SplitterRight.Width = new GridLength(0);
                    SplitterRight.MinWidth = 0;
                }
            }

            else
            {
                if (visible)
                {
                    ToolbarLeft.Width = new GridLength(200);
                    ToolbarLeft.MinWidth = 200;
                    ToolbarLeft.MaxWidth = CenterPanel.ActualWidth / 3;
                    SplitterLeft.Width = new GridLength(3);
                    SplitterLeft.MinWidth = 3;
                }
                else
                {
                    ToolbarLeft.Width = new GridLength(0);
                    ToolbarLeft.MinWidth = 0;
                    ToolbarLeft.MaxWidth = 0;
                    SplitterLeft.Width = new GridLength(0);
                    SplitterLeft.MinWidth = 0;
                }
            }
            OnEvent();

        }
    }
}
