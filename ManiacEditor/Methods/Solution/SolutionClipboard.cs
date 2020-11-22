using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Point = System.Drawing.Point;
using System.ComponentModel;
using ManiacEditor.Classes.Clipboard;

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
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("There was a problem with setting the clipboard: " + Environment.NewLine + ex.Message);
                    return;
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
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("There was a problem with setting the clipboard: " + Environment.NewLine + ex.Message);
                    return;
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
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("There was a problem with setting the clipboard: " + Environment.NewLine + ex.Message);
                    return;
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
                list.Add(value.Clone() as ObjectsClipboardEntry);
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
