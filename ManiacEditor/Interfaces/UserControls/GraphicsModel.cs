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
    public partial class EditorFormsModel : UserControl, IDrawArea
    {
        public Editor EditorInstance;
        public ManiacEditor.DevicePanel GraphicPanel;

		public HScrollBar hScrollBar;
		public VScrollBar vScrollBar;

		public System.Windows.Controls.Primitives.ScrollBar vScrollBar1 { get => vScrollBar.scroller; }
		public System.Windows.Controls.Primitives.ScrollBar hScrollBar1 { get => hScrollBar.scroller; }

		public EditorFormsModel(Editor instance)
        {
            EditorInstance = instance;
            InitializeComponent();
            SetupGraphicsPanel();

			hScrollBar = new HScrollBar();
			vScrollBar = new VScrollBar();
			hScrollBar1Host.Child = hScrollBar;
			vScrollBar1Host.Child = vScrollBar;
		}

        public void SetupGraphicsPanel()
        {
            this.GraphicPanel = new ManiacEditor.DevicePanel(EditorInstance);
            this.GraphicPanel.AllowDrop = true;
            this.GraphicPanel.AutoSize = true;
            this.GraphicPanel.DeviceBackColor = System.Drawing.Color.White;
            this.GraphicPanel.Location = new System.Drawing.Point(-1, 0);
            this.GraphicPanel.Margin = new System.Windows.Forms.Padding(0);
            this.GraphicPanel.Name = "GraphicPanel";
            this.GraphicPanel.Size = new System.Drawing.Size(643, 449);
            this.GraphicPanel.TabIndex = 10;
            this.viewPanel.Controls.Add(this.GraphicPanel);
        }

        public double GetZoom()
        {
            return EditorInstance.StateModel.Zoom;
        }

		public new void Dispose()
		{
			this.GraphicPanel.Dispose();
			this.GraphicPanel = null;
			hScrollBar = null;
			vScrollBar = null;
			this.Controls.Clear();
			base.Dispose(true);
		}

        public Rectangle GetScreen()
        {
            if (Settings.mySettings.EntityFreeCam) return new Rectangle(EditorInstance.StateModel.CustomX, EditorInstance.StateModel.CustomY, (int)EditorInstance.ViewPanelForm.ActualWidth, (int)EditorInstance.ViewPanelForm.ActualHeight);
            else return new Rectangle((int)EditorInstance.StateModel.ShiftX, (int)EditorInstance.StateModel.ShiftY, (int)EditorInstance.ViewPanelForm.ActualWidth, (int)EditorInstance.ViewPanelForm.ActualHeight);
        }

        public void DisposeTextures()
        {
            // Make sure to dispose the textures of the extra layers too
            EditorInstance.EditorTiles?.Dispose();
            if (EditorInstance.FGHigh != null) EditorInstance.FGHigh?.DisposeTextures();
            if (EditorInstance.FGLow != null) EditorInstance.FGLow?.DisposeTextures();
            if (EditorInstance.FGHigher != null) EditorInstance.FGHigher?.DisposeTextures();
            if (EditorInstance.FGLower != null) EditorInstance.FGLower?.DisposeTextures();

			if (EditorInstance.EditorScene != null)
			{
				foreach (var el in EditorInstance.EditorScene?.OtherLayers)
				{
					el.DisposeTextures();
				}
			}

        }

		private void EditorView_Load(object sender, EventArgs e)
		{

		}
	}
}
