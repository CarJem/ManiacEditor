using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiacEditor.Classes.Edit.Scene
{
    public class EditorTiles
    {
        private Editor Instance;
        public StageTiles StageTiles;

        public EditorTiles(Editor instance)
        {
            Instance = instance;
        }

        public void Dispose()
        {
            if (StageTiles != null) StageTiles.Dispose();
        }

        public void DisposeTextures()
        {
            if (StageTiles != null) StageTiles.DisposeTextures();
        }
    }
}
