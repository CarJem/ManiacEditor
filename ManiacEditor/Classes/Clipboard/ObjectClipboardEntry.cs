using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiacEditor.Classes.Clipboard
{
    [Serializable]
    public class ObjectsClipboardEntry : ClipboardEntry, ICloneable
    {
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        public ObjectsClipboardEntry(List<Classes.Scene.EditorEntity> _Content) : base()
        {
            Type = ContentType.Entities;
            _Content.ForEach(x => x.PrepareForExternalCopy());
            Content = _Content;
        }

        public List<Classes.Scene.EditorEntity> GetData()
        {
            var content = (Content as List<Classes.Scene.EditorEntity>);
            if (content != null)
            {
                foreach (var entry in content)
                {
                    if (entry != null && entry.attributesMap != null) entry.attributesMap.DefaultValue = new RSDKv5.AttributeValue(RSDKv5.AttributeTypes.ENUM);
                }
            }
            return content;
        }
    }
}
