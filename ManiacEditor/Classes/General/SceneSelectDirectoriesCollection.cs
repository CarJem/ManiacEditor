using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RSDKv5;

namespace ManiacEditor.Classes.General
{
    public struct SceneSelectDirectory
    {
        public string Name { get; set; }
        public Tuple<Gameconfig.SceneInfo,string> SceneInfo { get; set; }

        public SceneSelectDirectory(string _name, Gameconfig.SceneInfo _sceneInfo, string _path)
        {
            Name = _name;
            SceneInfo = new Tuple<Gameconfig.SceneInfo, string>(_sceneInfo, _path);
        }
    }
}
