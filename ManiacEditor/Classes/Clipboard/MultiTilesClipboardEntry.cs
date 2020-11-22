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
    public class MultiTilesClipboardEntry : ClipboardEntry
    {

        public MultiTilesClipboardEntry() : base()
        {
            Type = ContentType.MultiTiles;
            Content = new List<Dictionary<Point, ushort>>() { new Dictionary<Point, ushort>(), new Dictionary<Point, ushort>(), new Dictionary<Point, ushort>(), new Dictionary<Point, ushort>() };
        }
        public MultiTilesClipboardEntry(TilesClipboardEntry EntryA, TilesClipboardEntry EntryB, TilesClipboardEntry EntryC, TilesClipboardEntry EntryD) : base()
        {
            Type = ContentType.MultiTiles;
            var dataA = (EntryA != null ? EntryA.GetData() : new Dictionary<Point, ushort>());
            var dataB = (EntryB != null ? EntryB.GetData() : new Dictionary<Point, ushort>());
            var dataC = (EntryC != null ? EntryC.GetData() : new Dictionary<Point, ushort>());
            var dataD = (EntryD != null ? EntryD.GetData() : new Dictionary<Point, ushort>());
            Content = new List<Dictionary<Point, ushort>>() { dataA, dataB, dataC, dataD };
        }
        public MultiTilesClipboardEntry(List<Dictionary<Point, ushort>> _Content) : base()
        {
            Type = ContentType.MultiTiles;
            Content = _Content;
        }

        public List<Dictionary<Point, ushort>> GetData()
        {
            return (Content as List<Dictionary<Point, ushort>>);
        }

        public TilesClipboardEntry GetDataA()
        {
            return new TilesClipboardEntry((Content as List<Dictionary<Point, ushort>>)[0]);
        }
        public TilesClipboardEntry GetDataB()
        {
            return new TilesClipboardEntry((Content as List<Dictionary<Point, ushort>>)[1]);
        }
        public TilesClipboardEntry GetDataC()
        {
            return new TilesClipboardEntry((Content as List<Dictionary<Point, ushort>>)[2]);
        }
        public TilesClipboardEntry GetDataD()
        {
            return new TilesClipboardEntry((Content as List<Dictionary<Point, ushort>>)[3]);
        }
    }
}
