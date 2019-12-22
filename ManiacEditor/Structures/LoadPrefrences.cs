using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiacEditor.Structures
{
    public class LoadPrefrences
    {
        public string ForegroundLower { get; set; }
        public string ForegroundHigher { get; set; }
        public System.Drawing.Color WaterColor { get; set; }
        public List<string> SpritePaths { get; set; }
        public string EncoreACTFile { get; set; }
        public string CustomMenuFontText { get; set; }
        public string CustomMenuSmallFontText { get; set; }
        public string CustomLSelectFontText { get; set; }
        public Dictionary<string,string> EntityRenderSwaps { get; set; }
        public List<Tuple<string, string>> Positions { get; set; }

        public void Reset()
        {

        }
    }
}
