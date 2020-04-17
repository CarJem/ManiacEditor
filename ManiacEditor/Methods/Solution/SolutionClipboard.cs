using System;
using System.Collections.Generic;
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
            #region Clipboards
            private MultiTilesClipboardEntry _TilesClipboard;
            private TilesClipboardEntry _FindReplaceClipboard;
            private ObjectsClipboardEntry _ObjectsClipboard;
            private LayerClipboardEntry _LayerClipboard;


            public MultiTilesClipboardEntry TilesClipboard
            {
                get
                {
                    return _TilesClipboard;
                }
                set
                {
                    _TilesClipboard = value;
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
        public static ClipboardModel ClipboardData { get; set; } = new ClipboardModel();

        #endregion

        #region Clipboards
        public static MultiTilesClipboardEntry TilesClipboard
        {
            get
            {
                return ClipboardData.TilesClipboard;
            }
            set
            {
                ClipboardData.TilesClipboard = value;
            }
        }
        public static TilesClipboardEntry FindReplaceClipboard
        {
            get
            {
                return ClipboardData.FindReplaceClipboard;
            }
            set
            {
                ClipboardData.FindReplaceClipboard = value;
            }
        }
        public static ObjectsClipboardEntry ObjectsClipboard
        {
            get
            {
                return ClipboardData.ObjectsClipboard;
            }
            set
            {
                ClipboardData.ObjectsClipboard = value;
            }
        }
        public static LayerClipboardEntry LayerClipboard
        {
            get
            {
                return ClipboardData.LayerClipboard;
            }
            set
            {
                ClipboardData.LayerClipboard = value;
            }
        }

        #endregion

        #region Classes
        public class ClipboardEntry
        {
            public object Content { get; set; }
            public ContentType Type { get; set; }
            public DateTime Timestamp { get; set; }
            public string DisplayName { get; set; }
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
            }
        }
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
    }
}
