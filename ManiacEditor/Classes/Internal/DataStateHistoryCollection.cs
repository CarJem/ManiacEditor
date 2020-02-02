using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiacEditor.Classes.Internal
{
    public class DataStateHistoryCollection
    {
        public IList<SaveState> List = new List<SaveState>();

        public class SaveState
        {
            public string EntryName = "";
            public string RealEntryName = "";
            public string DataDirectory = "";
            public string LoadedDataPack = "";
            public bool isDataPack = false;
            public IList<string> ResourcePacks = new List<string>();


            public SaveState()
            {
                EntryName = "";
                DataDirectory = "";
                isDataPack = false;
                ResourcePacks = new List<string>();
            }
        }
        public DataStateHistoryCollection()
        {

        }
    }
}
