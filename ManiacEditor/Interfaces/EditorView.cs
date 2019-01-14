using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ManiacEditor.Interfaces
{
    public partial class EditorView : UserControl, IDrawArea
    {
        public Editor EditorInstance;
        public EditorView(Editor instance)
        {
            EditorInstance = instance;
            InitializeComponent();
            //var allLangItems = menuLanguageToolStripMenuItem.Items.Cast<System.Windows.Controls.MenuItem>().ToArray();
        }

        public double GetZoom()
        {
            if (EditorInstance.isExportingImage) return 1;
            else return EditorInstance.Zoom;
        }

        public Rectangle GetScreen()
        {
            if (EditorInstance.mySettings.EntityFreeCam && !EditorInstance.isExportingImage) return new Rectangle(EditorInstance.CustomX, EditorInstance.CustomY, mainPanel.Width, mainPanel.Height);
            else if (EditorInstance.isExportingImage) return new Rectangle(0, 0, EditorInstance.SceneWidth, EditorInstance.SceneHeight);
            else return new Rectangle(EditorInstance.ShiftX, EditorInstance.ShiftY, mainPanel.Width, mainPanel.Height);
        }

        public void DisposeTextures()
        {
            // Make sure to dispose the textures of the extra layers too
            EditorInstance.StageTiles?.DisposeTextures();
            if (EditorInstance.FGHigh != null) EditorInstance.FGHigh.DisposeTextures();
            if (EditorInstance.FGLow != null) EditorInstance.FGLow.DisposeTextures();
            if (EditorInstance.FGHigher != null) EditorInstance.FGHigher.DisposeTextures();
            if (EditorInstance.FGLower != null) EditorInstance.FGLower.DisposeTextures();

            foreach (var el in EditorInstance.EditorScene.OtherLayers)
            {
                el.DisposeTextures();
            }
        }
    }
}
