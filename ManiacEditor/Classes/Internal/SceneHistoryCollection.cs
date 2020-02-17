using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiacEditor.Classes.Internal
{
    public class SceneHistoryCollection
    { 
        public IList<SaveState> List = new List<SaveState>();

        public class SaveState
        {
            public string RealEntryName = "";
            public string EntryName = "";
            public SceneState SceneState = new SceneState();
            public int ZoomLevel;
            public string LoadedDataPack = "";
            public IList<string> ResourcePacks = new List<string>();
            public int x;
            public int y;


            public SaveState()
            {
                LoadedDataPack = "";
                EntryName = "";
                SceneState = new SceneState();
                ResourcePacks = new List<string>();
                x = 0;
                y = 0;
            }
        }
        public SceneHistoryCollection()
        {

        }
    }
}
