using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiacEditor.Classes.Clipboard
{
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
}
