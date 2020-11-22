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
}
