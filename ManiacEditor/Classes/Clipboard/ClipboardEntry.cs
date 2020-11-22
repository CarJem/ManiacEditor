using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Point = System.Drawing.Point;
using System.ComponentModel;

namespace ManiacEditor.Classes.Clipboard
{
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
}
