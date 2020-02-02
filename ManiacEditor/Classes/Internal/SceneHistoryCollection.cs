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
            public string DataDirectory = "";
            public string Result = "";
            public int LevelID;
            public int ZoomLevel;
            public bool isEncore = false;
            public string CurrentZone = "";
            public string CurrentName = "";
            public string LoadedDataPack = "";
            public string CurrentSceneID = "";
            public bool Browsed = false;
            public IList<string> ResourcePacks = new List<string>();
            public int x;
            public int y;


            public SaveState()
            {
                LoadedDataPack = "";
                EntryName = "";
                DataDirectory = "";
                Result = "";
                LevelID = -1;
                isEncore = false;
                CurrentZone = "";
                CurrentName = "";
                CurrentSceneID = "";
                Browsed = false;
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
