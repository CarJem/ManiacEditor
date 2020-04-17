using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using ManiacEditor;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using Point = System.Windows.Point;
using System.Runtime.InteropServices;
using System.Collections.ObjectModel;
using SystemInformation = System.Windows.Forms.SystemInformation;
using GenerationsLib.WPF;
using System.Linq;
using System.Windows.Data;
using System.Globalization;

namespace ManiacEditor.Controls.Global.Controls
{

    [DefaultEvent("SelectedIndexChanged")]
    public partial class ManiacTileList : UserControl
    {

        #region Definitions

        #region Data Model

        public class ViewModel : INotifyPropertyChanged
        {
            private HorizontalAlignment _HorizontalAlignment { get; set; }
            private VerticalAlignment _VerticalAlignment { get; set; }
            private Transform _CurrentTransform { get; set; }
            private int _ImageSize { get; set; }
            private int _ItemColumns { get; set; }
            private int _ItemRows { get; set; }

            private Orientation _Orientation { get; set; }

            private int _ForcedItemHeight { get; set; }

            public Transform CurrentTransform
            {
                get
                {
                    return _CurrentTransform;
                }
                set
                {
                    _CurrentTransform = value;
                    NotifyPropertyChanged("CurrentTransform");
                }
            }
            public Orientation Orientation
            {
                get
                {
                    return _Orientation;
                }
                set
                {
                    _Orientation = value;
                    NotifyPropertyChanged("Orientation");
                }
            }

            public int ImageSize
            {
                get
                {
                    return _ImageSize;
                }
                set
                {
                    _ImageSize = value;
                    NotifyPropertyChanged("ImageSize");
                }
            }
            public int ItemColumns
            {
                get
                {
                    return _ItemColumns;
                }
                set
                {
                    _ItemColumns = value;
                    NotifyPropertyChanged("ItemColumns");
                }
            }
            public HorizontalAlignment HorizontalAlignment
            {
                get
                {
                    return _HorizontalAlignment;
                }
                set
                {
                    _HorizontalAlignment = value;
                    NotifyPropertyChanged("HorizontalAlignment");
                }
            }
            public VerticalAlignment VerticalAlignment
            {
                get
                {
                    return _VerticalAlignment;
                }
                set
                {
                    _VerticalAlignment = value;
                    NotifyPropertyChanged("VerticalAlignment");
                }
            }
            public int ItemRows
            {
                get
                {
                    return _ItemRows;
                }
                set
                {
                    _ItemRows = value;
                    NotifyPropertyChanged("ItemRows");
                }
            }

            #region INotifyPropertyChanged Properties

            public event PropertyChangedEventHandler PropertyChanged;

            protected void NotifyPropertyChanged(String info)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(info));
                }
            }

            #endregion
        }

        public ViewModel ModelView
        {
            get
            {
                return (DataContext as ViewModel);
            }
            set
            {
                DataContext = value;
            }
        }

        #endregion

        #region Instance

        private static ManiacEditor.Controls.Editor.MainEditor Instance;
        public static void UpdateInstance(ManiacEditor.Controls.Editor.MainEditor _instance)
        {
            Instance = _instance;
        }

        #endregion

        #region Selected Index

        [Browsable(false)]
        public int SelectedIndex
        {
            get { return TileList.SelectedIndex; }
            set
            {
                bool changed = value != TileList.SelectedIndex;
                TileList.SelectedIndex = value;
                ScrollToSelected();
                if (changed) SelectedIndexChanged(this, EventArgs.Empty);
            }
        }

        #endregion

        #region Event Handlers

        public event EventHandler SelectedIndexChanged = delegate { };

        public event EventHandler ItemDrag = delegate { };

        public event EventHandler<MouseEventArgs> ContextMenuRequestClick = delegate { };

        #endregion

        #region Image Values

        private int _LastImageSize;
        private int _ImageSize = 16;

        [DefaultValue(16)]
        public int ImageSize
        {
            get { return _ImageSize; }
            set
            {
                ModelView.ImageSize = value;
                _ImageSize = value;
                Invalidate();
            }
        }
        #endregion

        #region List Direction

        private Direction _LastDirection;
        private Direction _Direction = Direction.Vertical;
        [DefaultValue(Direction.Vertical)]
        public Direction Direction
        {
            get { return _Direction; }
            set
            {
                _Direction = value;
                Invalidate();
            }
        }

        #endregion

        #region Flipping
        public bool _LastFlipX;
        public bool _LastFlipY;

        public bool _FlipX = false;
        public bool _FlipY = false;
        public bool FlipX
        {
            get { return _FlipX; }
            set
            {
                _FlipX = value;
                Invalidate();
            }
        }
        public bool FlipY
        {
            get { return _FlipY; }
            set
            {
                _FlipY = value;
                Invalidate();
            }
        }

        #endregion

        #region Images / List Items

        private bool SourceNeedsUpdate { get; set; } = true;

        public List<ImageSource> _Images = new List<ImageSource>();
        public List<ImageSource> Images
        {
            get
            {
                return _Images;
            }
            set
            {
                SourceNeedsUpdate = true;
                _Images = value;
            }
        }

        private void UpdateSource(bool UpdateSource, bool UpdateSize, bool UpdateTransform)
        {

        }

        #endregion

        #endregion

        #region Init

        public ManiacTileList()
        {
            InitializeComponent();
            ModelView = new ViewModel();
        }


        #endregion

        #region General Methods
        public void Invalidate(bool ForceUpdateSource = false)
        {


            Application.Current.Dispatcher.Invoke(new Action(() =>
            {

                if (ForceUpdateSource) SourceNeedsUpdate = true;

                bool FlipChanged = (FlipX != _LastFlipX || (FlipY != _LastFlipY));
                bool SizeChanged = (ImageSize != _LastImageSize);
                bool DirectionChanged = (Direction != _LastDirection);

                if (FlipChanged) FlipItems(FlipX, FlipY);
                if (SizeChanged) ChangeSize(ImageSize);
                if (DirectionChanged) ChangeDirection(TileList, Direction);

                UpdateColumns();

                UpdateSource(SourceNeedsUpdate, SizeChanged, FlipChanged);

                if (SourceNeedsUpdate)
                {
                    TileList.ItemsSource = Images;
                    TileList.Items.Refresh();
                    SourceNeedsUpdate = false;
                }

            }), System.Windows.Threading.DispatcherPriority.Loaded);          
        }

        public void FlipItems(bool _flipX, bool _flipY)
        {
            ModelView.CurrentTransform = new ScaleTransform(FlipX ? -1 : 1, FlipY ? -1 : 1);

            _LastFlipX = _flipX;
            _LastFlipY = _flipY;
        }

        public void ChangeSize(int size)
        {
            _LastImageSize = size;
        }

        public void UpdateColumns()
        {
            int width = (int)TileList.ActualWidth;
            int height = (int)TileList.ActualHeight;

            int tilesPerColumn = AvoidZero(width) / AvoidZero(ImageSize);
            int tilesPerRow = AvoidZero(height) / AvoidZero(ImageSize);

            if (ModelView.ItemColumns != tilesPerColumn) ModelView.ItemColumns = tilesPerColumn;
            if (ModelView.ItemRows != tilesPerRow) ModelView.ItemRows = tilesPerRow;

            int AvoidZero(int value)
            {
                return (value == 0 ? 1 : value);
            }
        }

        public void ChangeDirection(DependencyObject obj, Direction direction)
        {
            switch (direction)
            {
                case Direction.Horizontal:
                    ScrollViewer.SetVerticalScrollBarVisibility(obj, ScrollBarVisibility.Disabled);
                    ScrollViewer.SetHorizontalScrollBarVisibility(obj, ScrollBarVisibility.Visible);
                    ModelView.VerticalAlignment = VerticalAlignment.Stretch;
                    ModelView.HorizontalAlignment = HorizontalAlignment.Center;
                    ModelView.Orientation = Orientation.Horizontal;
                    break;
                case Direction.Vertical:
                    ScrollViewer.SetVerticalScrollBarVisibility(obj, ScrollBarVisibility.Visible);
                    ScrollViewer.SetHorizontalScrollBarVisibility(obj, ScrollBarVisibility.Disabled);
                    ModelView.VerticalAlignment = VerticalAlignment.Center;
                    ModelView.HorizontalAlignment = HorizontalAlignment.Stretch;
                    ModelView.Orientation = Orientation.Vertical;
                    break;
            }

            _LastDirection = direction;
        }
        private void ScrollToSelected()
        {
            if (SelectedIndex == -1) return;
            TileList.ScrollIntoView(TileList.Items[SelectedIndex]);
        }

        #endregion

        #region Event Methods
        private void TileList_Resize(object sender, System.Windows.SizeChangedEventArgs e) 
        {
            Invalidate();
        }
        private void TileList_MouseDown(object sender, MouseButtonEventArgs e)
        {
            /*
            IsMouseDown = true;
            Point p = e.GetPosition(TileList);
            DragRect = null;
            int index = SelectedIndex;
            if (index == -1) return;
            */
        }
        private void TileList_MouseMove(object sender, MouseEventArgs e)
        {
            /*
            if (IsMouseDown)
            {
                ItemDrag(this, new EventArgs());
            }
            */
        }
        private void TileList_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //IsMouseDown = false;
        }
        private void TileList_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }
        private void TileList_KeyDown(object sender, KeyEventArgs e)
        {

        }
        private void TileList_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Invalidate();
        }
        #endregion

        #region Outdated Events

        private void removeChunkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Instance.TilesToolbar.RemoveChunk(SelectedIndex);
        }
        private void duplicateChunkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedIndex != -1)
            {
                Instance.TilesToolbar.DuplicateChunk(SelectedIndex);
            }
        }
        private void importChunkFromClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Methods.Solution.SolutionClipboard.TilesClipboard != null)
            {
                Instance.Chunks.ConvertClipboardtoMultiLayerChunk(Methods.Solution.SolutionClipboard.TilesClipboard);

                Instance.TilesToolbar?.ChunksReload();
            }
        }
        private void editCollisionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Instance.TileManiacInstance == null || Instance.TileManiacInstance.IsEditorClosed) Instance.TileManiacInstance = new ManiacEditor.Controls.TileManiac.CollisionEditor();
            if (Instance.TileManiacInstance.Visibility != System.Windows.Visibility.Visible)
            {
                Instance.TileManiacInstance.Show();
            }
            if (Methods.Solution.CurrentSolution.TileConfig != null && Methods.Solution.CurrentSolution.CurrentTiles != null)
            {
                if (Instance.TileManiacInstance.Visibility != System.Windows.Visibility.Visible || Instance.TileManiacInstance.TileConfig == null)
                {
                    Instance.TileManiacInstance.LoadTileConfigViaIntergration(Methods.Solution.CurrentSolution.TileConfig, ManiacEditor.Methods.Solution.SolutionPaths.TileConfig_Source.ToString(), SelectedIndex);
                }
                else
                {
                    Instance.TileManiacInstance.SetCollisionIndex(SelectedIndex);
                    Instance.TileManiacInstance.Activate();
                }

            }
        }

        #endregion
    }
}
