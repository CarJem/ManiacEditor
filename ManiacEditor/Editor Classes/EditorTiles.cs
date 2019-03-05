using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using RSDKv5;
using SharpDX.Direct3D9;
using ManiacEditor.Actions;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;

namespace ManiacEditor
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
