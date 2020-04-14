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

        public class ViewModel
        {
            public int ImageSize { get; set; }
            public int ItemColumns { get; set; }
            public int ItemRows { get; set; }
            public ObservableCollection<TileListItem> Items { get; set; } = new ObservableCollection<TileListItem>();
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

        public Transform CurrentTransform { get; set; }

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
            for (int i = 0; i < Images.Count; i++)
            {
                if (i > ModelView.Items.Count - 1) ModelView.Items.Add(new TileListItem());
                if (UpdateSource) ModelView.Items[i].Source = Images[i];
                if (UpdateTransform) ModelView.Items[i].ImageRenderTransform = CurrentTransform;
                if (UpdateSize) ModelView.Items[i].ImageSize = ImageSize;
            }

            if (Images.Count != ModelView.Items.Count)
            {
                for (int i = ModelView.Items.Count; i > Images.Count; i--) ModelView.Items.RemoveAt(i);
            }
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
                    TileList.ItemsSource = ModelView.Items;
                    TileList.Items.Refresh();
                    SourceNeedsUpdate = false;
                }

            }), System.Windows.Threading.DispatcherPriority.Render);          
        }

        public void FlipItems(bool _flipX, bool _flipY)
        {
            CurrentTransform = new ScaleTransform(FlipX ? -1 : 1, FlipY ? -1 : 1);

            _LastFlipX = _flipX;
            _LastFlipY = _flipY;
        }

        public void ChangeSize(int size)
        {
            _LastImageSize = size;
        }

        public void UpdateColumns()
        {
            switch (Direction)
            {
                case Direction.Horizontal:
                    ModelView.ItemColumns = (int)TileList.ActualHeight / ImageSize;
                    ModelView.ItemRows = 0;
                    break;
                case Direction.Vertical:
                    ModelView.ItemColumns = 0;
                    ModelView.ItemRows = (int)TileList.ActualWidth / ImageSize;
                    break;
            }
        }

        public void ChangeDirection(DependencyObject obj, Direction direction)
        {
            switch (direction)
            {
                case Direction.Horizontal:
                    ScrollViewer.SetVerticalScrollBarVisibility(obj, ScrollBarVisibility.Disabled);
                    ScrollViewer.SetHorizontalScrollBarVisibility(obj, ScrollBarVisibility.Visible);
                    break;
                case Direction.Vertical:
                    ScrollViewer.SetVerticalScrollBarVisibility(obj, ScrollBarVisibility.Visible);
                    ScrollViewer.SetHorizontalScrollBarVisibility(obj, ScrollBarVisibility.Disabled);
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
            if (Instance.TilesClipboard != null)
            {
                Instance.Chunks.ConvertClipboardtoMultiLayerChunk(Instance.TilesClipboard.Item1, Instance.TilesClipboard.Item2);

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
            if (Methods.Editor.Solution.TileConfig != null && Methods.Editor.Solution.CurrentTiles != null)
            {
                if (Instance.TileManiacInstance.Visibility != System.Windows.Visibility.Visible || Instance.TileManiacInstance.TileConfig == null)
                {
                    Instance.TileManiacInstance.LoadTileConfigViaIntergration(Methods.Editor.Solution.TileConfig, ManiacEditor.Methods.Editor.SolutionPaths.TileConfig_Source.ToString(), SelectedIndex);
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
