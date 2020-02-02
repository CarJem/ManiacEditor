using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiacEditor.Classes.Editor.Scene
{
    public class EditorTiles
    {
        private ManiacEditor.Controls.Base.MainEditor Instance;
        public StageTiles StageTiles;

        public EditorTiles(ManiacEditor.Controls.Base.MainEditor instance)
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
