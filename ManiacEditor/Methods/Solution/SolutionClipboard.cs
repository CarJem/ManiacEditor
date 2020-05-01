using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Point = System.Drawing.Point;
using System.ComponentModel;

namespace ManiacEditor.Methods.Solution
{
    public static class SolutionClipboard
    {
        #region Data Model
        public class ClipboardModel : INotifyPropertyChanged
        {
            #region Definitions
            private MultiTilesClipboardEntry _TilesClipboard;
            private TilesClipboardEntry _FindReplaceClipboard;
            private ObjectsClipboardEntry _ObjectsClipboard;
            private LayerClipboardEntry _LayerClipboard;

            private ObservableCollection<MultiTilesClipboardEntry> _TilesClipboardHistory = new ObservableCollection<MultiTilesClipboardEntry>();
            private ObservableCollection<ObjectsClipboardEntry> _ObjectsClipboardHistory = new ObservableCollection<ObjectsClipboardEntry>();
            private ObservableCollection<LayerClipboardEntry> _LayerClipboardHistory = new ObservableCollection<LayerClipboardEntry>();
            #endregion

            #region Methods
            public void RemoveFromTilesHistory(MultiTilesClipboardEntry value)
            {
                var list = _TilesClipboardHistory;
                list.Remove(value);
                TilesClipboardHistory = list;
            }
            public void RemoveFromObjectsHistory(ObjectsClipboardEntry value)
            {
                var list = _ObjectsClipboardHistory;
                list.Remove(value);
                ObjectsClipboardHistory = list;
            }
            public void RemoveFromLayerHistory(LayerClipboardEntry value)
            {
                var list = _LayerClipboardHistory;
                list.Remove(value);
                LayerClipboardHistory = list;
            }

            public void SetTileClipboard(MultiTilesClipboardEntry value, bool add = false)
            {
                if (add)
                {
                    TilesClipboard = value;
                }
                else
                {
                    _TilesClipboard = value;
                    NotifyPropertyChanged("TilesClipboard");
                }

                try
                {
                    System.Windows.Forms.Clipboard.SetDataObject(new System.Windows.Forms.DataObject("ManiacTiles", value.GetData()), true);
                }
                catch
                {

                }
            }
            public void SetObjectClipboard(ObjectsClipboardEntry value, bool add = false)
            {
                if (add)
                {
                    ObjectsClipboard = value;
                }
                else
                {
                    _ObjectsClipboard = value;
                    NotifyPropertyChanged("ObjectsClipboard");
                }


                try
                {
                    System.Windows.Forms.Clipboard.SetDataObject(new System.Windows.Forms.DataObject("ManiacEntities", value), true);
                }
                catch
                {

                }
            }
            public void SetLayerClipboard(LayerClipboardEntry value, bool add = false)
            {
                if (add)
                {
                    LayerClipboard = value;
                }
                else
                {
                    _LayerClipboard = value;
                    NotifyPropertyChanged("LayerClipboard");
                }

                try
                {
                    System.Windows.Forms.Clipboard.SetDataObject(new System.Windows.Forms.DataObject("ManiacLayer", value), true);
                }
                catch
                {

                }
            }

            private void AddToTilesHistory(MultiTilesClipboardEntry value)
            {
                var list = _TilesClipboardHistory;
                list.Add(value);
                TilesClipboardHistory = list;
            }
            private void AddToObjectsHistory(ObjectsClipboardEntry value)
            {
                var list = _ObjectsClipboardHistory;
                list.Add(value);
                ObjectsClipboardHistory = list;
            }
            private void AddToLayerHistory(LayerClipboardEntry value)
            {
                var list = _LayerClipboardHistory;
                list.Add(value);
                LayerClipboardHistory = list;
            }
            #endregion

            #region Properties
            public ObservableCollection<MultiTilesClipboardEntry> TilesClipboardHistory
            {
                get
                {
                    return _TilesClipboardHistory;
                }
                set
                {
                    _TilesClipboardHistory = value;
                    NotifyPropertyChanged("TilesClipboardHistory");
                }
            }

            public ObservableCollection<ObjectsClipboardEntry> ObjectsClipboardHistory
            {
                get
                {
                    return _ObjectsClipboardHistory;
                }
                set
                {
                    _ObjectsClipboardHistory = value;
                    NotifyPropertyChanged("ObjectsClipboardHistory");
                }
            }

            public ObservableCollection<LayerClipboardEntry> LayerClipboardHistory
            {
                get
                {
                    return _LayerClipboardHistory;
                }
                set
                {
                    _LayerClipboardHistory = value;
                    NotifyPropertyChanged("LayerClipboardHistory");
                }
            }


            public MultiTilesClipboardEntry TilesClipboard
            {
                get
                {
                    return _TilesClipboard;
                }
                set
                {
                    _TilesClipboard = value;
                    AddToTilesHistory(value);
                    NotifyPropertyChanged("TilesClipboard");
                }
            }
            public TilesClipboardEntry FindReplaceClipboard
            {
                get
                {
                    return _FindReplaceClipboard;
                }
                set
                {
                    _FindReplaceClipboard = value;
                    NotifyPropertyChanged("FindReplaceClipboard");
                }
            }
            public ObjectsClipboardEntry ObjectsClipboard
            {
                get
                {
                    return _ObjectsClipboard;
                }
                set
                {
                    _ObjectsClipboard = value;
                    AddToObjectsHistory(value);
                    NotifyPropertyChanged("ObjectsClipboard");
                }
            }
            public LayerClipboardEntry LayerClipboard
            {
                get
                {
                    return _LayerClipboard;
                }
                set
                {
                    _LayerClipboard = value;
                    AddToLayerHistory(value);
                    NotifyPropertyChanged("LayerClipboard");
                }
            }
            #endregion

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
        public static ClipboardModel ClipboardViewModel { get; set; } = new ClipboardModel();

        #endregion

        #region Clipboards

        public static MultiTilesClipboardEntry TilesClipboard
        {
            get
            {
                return ClipboardViewModel.TilesClipboard;
            }
        }
        public static TilesClipboardEntry FindReplaceClipboard
        {
            get
            {
                return ClipboardViewModel.FindReplaceClipboard;
            }
            set
            {
                ClipboardViewModel.FindReplaceClipboard = value;
            }
        }
        public static ObjectsClipboardEntry ObjectsClipboard
        {
            get
            {
                return ClipboardViewModel.ObjectsClipboard;
            }
        }
        public static LayerClipboardEntry LayerClipboard
        {
            get
            {
                return ClipboardViewModel.LayerClipboard;
            }
        }

        #endregion

        #region Classes
        [Serializable]
        public class ClipboardEntry : INotifyPropertyChanged
        {

            private string _displayName;
            private DateTime _timestamp;

            public object Content { get; set; }
            public ContentType Type { get; set; }
            public DateTime Timestamp
            {
                get
                {
                    return _timestamp;
                }
                set
                {
                    _timestamp = value;
                    NotifyPropertyChanged(nameof(Timestamp));
                }
            }
            public string DisplayName
            {
                get
                {
                    return _displayName;
                }
                set
                {
                    _displayName = value;
                    NotifyPropertyChanged(nameof(DisplayName));
                }
            }
            public List<string> Tags { get; set; }

            public enum ContentType : int
            {
                Tiles,
                MultiTiles,
                Entities,
                Collision,
                Layers
            }


            public ClipboardEntry()
            {
                Timestamp = DateTime.Now;
                DisplayName = Timestamp.ToString("MM/dd/yyyy HH:mm:ss:fff");
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

        [Serializable]
        public class MultiTilesClipboardEntry : ClipboardEntry
        {

            public MultiTilesClipboardEntry() : base()
            {
                Type = ContentType.MultiTiles;
                Content = new Tuple<Dictionary<Point, ushort>, Dictionary<Point, ushort>>(new Dictionary<Point, ushort>(), new Dictionary<Point, ushort>());
            }
            public MultiTilesClipboardEntry(TilesClipboardEntry EntryA, TilesClipboardEntry EntryB) : base()
            {
                Type = ContentType.MultiTiles;
                var dataA = (EntryA != null ? EntryA.GetData() : new Dictionary<Point, ushort>());
                var dataB = (EntryB != null ? EntryB.GetData() : new Dictionary<Point, ushort>());
                Content = new Tuple<Dictionary<Point, ushort>, Dictionary<Point, ushort>>(dataA, dataB);
            }
            public MultiTilesClipboardEntry(Tuple<Dictionary<Point, ushort>, Dictionary<Point, ushort>> _Content) : base()
            {
                Type = ContentType.MultiTiles;
                Content = _Content;
            }

            public Tuple<Dictionary<Point, ushort>, Dictionary<Point, ushort>> GetData()
            {
                return (Content as Tuple<Dictionary<Point, ushort>, Dictionary<Point, ushort>>);
            }

            public TilesClipboardEntry GetDataA()
            {
                return new TilesClipboardEntry((Content as Tuple<Dictionary<Point, ushort>, Dictionary<Point, ushort>>).Item1);
            }
            public TilesClipboardEntry GetDataB()
            {
                return new TilesClipboardEntry((Content as Tuple<Dictionary<Point, ushort>, Dictionary<Point, ushort>>).Item2);
            }
        }

        [Serializable]
        public class TilesClipboardEntry : ClipboardEntry
        {
            public TilesClipboardEntry() : base()
            {
                Type = ContentType.Tiles;
                Content = new Dictionary<Point, ushort>();
            }
            public TilesClipboardEntry(Dictionary<Point, ushort> _Content) : base()
            {
                Type = ContentType.Tiles;
                Content = _Content;
            }

            public Dictionary<Point, ushort> GetData()
            {
                return (Content as Dictionary<Point, ushort>);
            }

        }

        [Serializable]
        public class ObjectsClipboardEntry : ClipboardEntry
        {

            public ObjectsClipboardEntry(List<Classes.Scene.EditorEntity> _Content) : base()
            {
                Type = ContentType.Entities;
                _Content.ForEach(x => x.PrepareForExternalCopy());
                Content = _Content;
            }

            public List<Classes.Scene.EditorEntity> GetData()
            {
                return (Content as List<Classes.Scene.EditorEntity>);
            }
        }

        [Serializable]
        public class LayerClipboardEntry : ClipboardEntry
        {
            public LayerClipboardEntry(Classes.Scene.EditorLayer _Content) : base()
            {
                Type = ContentType.Layers;
                Content = _Content;
            }

            public Classes.Scene.EditorLayer GetData()
            {
                return (Content as Classes.Scene.EditorLayer);
            }
        }
        #endregion

        #region Methods

        public static void SetTileClipboard(MultiTilesClipboardEntry value)
        {
            ClipboardViewModel.SetTileClipboard(value, true);
        }

        public static void SetObjectClipboard(ObjectsClipboardEntry value)
        {
            ClipboardViewModel.SetObjectClipboard(value, true);
        }

        public static void SetLayerClipboard(LayerClipboardEntry value)
        {
            ClipboardViewModel.SetLayerClipboard(value, true);
        }

        #endregion
    }
}
