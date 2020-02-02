using System;
using System.Drawing;
using System.Windows.Forms;

namespace ManiacEditor.Controls
{
    public partial class EditorFormsModel : UserControl, IDrawArea
    {
        public Controls.Base.MainEditor EditorInstance;
        public ManiacEditor.DevicePanel GraphicPanel;

		public Global.Controls.HScrollBar hScrollBar;
		public Global.Controls.VScrollBar vScrollBar;

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


        public EditorFormsModel(Controls.Base.MainEditor instance)
        {
            EditorInstance = instance;
            InitializeComponent();
            UpdateScrollbars();
            SetupGraphicsPanel();
		}

        public void UpdateScrollbars(bool refreshing = false)
        {
            hScrollBar = new Global.Controls.HScrollBar();
            vScrollBar = new Global.Controls.VScrollBar();
            hScrollBar1Host.Child = hScrollBar;
            vScrollBar1Host.Child = vScrollBar;
            if (refreshing) ManiacEditor.Controls.Base.MainEditor.Instance.UpdateScrollBars();
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
            return Classes.Core.SolutionState.Zoom;
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
            if (Core.Settings.MySettings.EntityFreeCam) return new Rectangle(Classes.Core.SolutionState.CustomX, Classes.Core.SolutionState.CustomY, (int)EditorInstance.ViewPanelForm.ActualWidth, (int)EditorInstance.ViewPanelForm.ActualHeight);
            else return new Rectangle((int)Classes.Core.SolutionState.ViewPositionX, (int)Classes.Core.SolutionState.ViewPositionY, (int)EditorInstance.ViewPanelForm.ActualWidth, (int)EditorInstance.ViewPanelForm.ActualHeight);
        }

        public void DisposeTextures()
        {
            // Make sure to dispose the textures of the extra layers too
            Classes.Core.Solution.CurrentTiles?.Dispose();
            if (Classes.Core.Solution.FGHigh != null) Classes.Core.Solution.FGHigh?.DisposeTextures();
            if (Classes.Core.Solution.FGLow != null) Classes.Core.Solution.FGLow?.DisposeTextures();
            if (Classes.Core.Solution.FGHigher != null) Classes.Core.Solution.FGHigher?.DisposeTextures();
            if (Classes.Core.Solution.FGLower != null) Classes.Core.Solution.FGLower?.DisposeTextures();

			if (Classes.Core.Solution.CurrentScene != null)
			{
				foreach (var el in Classes.Core.Solution.CurrentScene?.OtherLayers)
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
