using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiacEditor.Classes.Internal
{
    public class SceneState
    {
		public IList<string> ResourcePacks { get; set; } = new List<string>();
		public string FilePath { get; set; } = "";
		public int LevelID { get; set; } = -1;
		public bool IsEncoreMode { get; set; } = false;
		public string SceneDirectory { get; set; } = "";
		public string Zone { get; set; } = "";
		public string Name { get; set; } = "";
		public string SceneID { get; set; } = "";
		public string DataDirectory { get; private set; } = "";
		public LoadMethod LoadType { get; set; } = SceneState.LoadMethod.Unspecified;
		public bool IsFullPath
		{
			get
			{
				if (LoadType == LoadMethod.RelativePath) return false;
				else return true;
			}
		}
		public bool WasSelfLoaded
		{
			get
			{
				if (LoadType == LoadMethod.SelfLoaded) return true;
				else return false;
			}
		}

		public enum LoadMethod
		{
			RelativePath, FullPath, SelfLoaded, Unspecified
		}

		public SceneState(string filePath = "", int levelID = -1, bool isEncore = false, string sceneDirectory = "", string zone = "", string name = "", string sceneID = "", LoadMethod loadType = SceneState.LoadMethod.Unspecified, IList<string> rpList = null)
		{
			FilePath = filePath;
			LevelID = levelID;
			IsEncoreMode = isEncore;
			SceneDirectory = sceneDirectory;
			Zone = zone;
			Name = name;
			SceneID = sceneID;
			LoadType = loadType;
			if (rpList != null) ResourcePacks = rpList;
		}

		public void SetDataDirectory(string path)
		{
			DataDirectory = path;
		}

		public void Clear()
		{
			FilePath = "";
			LevelID = -1;
			IsEncoreMode = false;
			SceneDirectory = "";
			Zone = "";
			Name = "";
			SceneID = "";
			LoadType = LoadMethod.Unspecified;
			ResourcePacks = new List<string>();
		}

		public SceneState()
		{

		}

	}
}
