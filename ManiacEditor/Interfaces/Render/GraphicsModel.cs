using System;
using System.Drawing;
using System.Windows.Forms;

namespace ManiacEditor.Interfaces
{
    public partial class EditorFormsModel : UserControl, IDrawArea
    {
        public Editor EditorInstance;
        public ManiacEditor.DevicePanel GraphicPanel;

		public HScrollBar hScrollBar;
		public VScrollBar vScrollBar;

		public System.Windows.Controls.Primitives.ScrollBar vScrollBar1 { get => GetScrollBarV(); }
		public System.Windows.Controls.Primitives.ScrollBar hScrollBar1 { get => GetScrollBarH(); }

        private System.Windows.Controls.Primitives.ScrollBar GetScrollBarV()
        {
            return vScrollBar.scroller;
        }

        private System.Windows.Controls.Primitives.ScrollBar GetScrollBarH()
        {
            return hScrollBar.scroller;
        }


        public EditorFormsModel(Editor instance)
        {
            EditorInstance = instance;
            InitializeComponent();
            UpdateScrollbars();
            SetupGraphicsPanel();
		}

        public void UpdateScrollbars(bool refreshing = false)
        {
            hScrollBar = new HScrollBar();
            vScrollBar = new VScrollBar();
            hScrollBar1Host.Child = hScrollBar;
            vScrollBar1Host.Child = vScrollBar;
            if (refreshing) Editor.Instance.UpdateScrollBars();
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
            return Classes.Editor.SolutionState.Zoom;
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
            if (Settings.MySettings.EntityFreeCam) return new Rectangle(Classes.Editor.SolutionState.CustomX, Classes.Editor.SolutionState.CustomY, (int)EditorInstance.ViewPanelForm.ActualWidth, (int)EditorInstance.ViewPanelForm.ActualHeight);
            else return new Rectangle((int)Classes.Editor.SolutionState.ViewPositionX, (int)Classes.Editor.SolutionState.ViewPositionY, (int)EditorInstance.ViewPanelForm.ActualWidth, (int)EditorInstance.ViewPanelForm.ActualHeight);
        }

        public void DisposeTextures()
        {
            // Make sure to dispose the textures of the extra layers too
            Classes.Editor.Solution.CurrentTiles?.Dispose();
            if (Classes.Editor.Solution.FGHigh != null) Classes.Editor.Solution.FGHigh?.DisposeTextures();
            if (Classes.Editor.Solution.FGLow != null) Classes.Editor.Solution.FGLow?.DisposeTextures();
            if (Classes.Editor.Solution.FGHigher != null) Classes.Editor.Solution.FGHigher?.DisposeTextures();
            if (Classes.Editor.Solution.FGLower != null) Classes.Editor.Solution.FGLower?.DisposeTextures();

			if (Classes.Editor.Solution.CurrentScene != null)
			{
				foreach (var el in Classes.Editor.Solution.CurrentScene?.OtherLayers)
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
