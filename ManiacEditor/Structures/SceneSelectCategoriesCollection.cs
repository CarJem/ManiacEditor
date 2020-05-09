using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiacEditor.Structures
{
    public class SceneSelectCategoriesCollection
    {
        public List<SceneSelectCategory> Items { get; set; } = new List<SceneSelectCategory>();

        public void Clear()
        {
            Items.Clear();
        }

        public void Add(string _name, List<SceneSelectScene> _entries)
        {
            Items.Add(new SceneSelectCategory(_name, _entries));
        }
    }

    public class SceneSelectCategory
    {
        public string Name { get; set; }
        public List<SceneSelectScene> Entries { get; set; } = new List<SceneSelectScene>();

        public SceneSelectCategory(string _name, List<SceneSelectScene> _entries)
        {
            Name = _name;
            Entries = _entries;
        }
    }

    public struct SceneSelectScene
    {
        public string Name { get; set; }
        public string Path { get; set; }

        public SceneSelectScene(string _name, string _path)
        {
            Name = _name;
            Path = _path;
        }
    }
}
