using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RSDKv5;

namespace ManiacEditor
{
    public partial class TilesList : UserControl, IDrawArea
    {
        public GIF TilesImage = null;

        private DevicePanel graphicPanel;
		private ManiacEditor.Interfaces.VScrollBar vScrollBar1;


		const int TILE_SIZE = 16;

        public String SelectedTileNumber;

        int _tileScale = 2;
        public int TileScale { get { return _tileScale; } set { _tileScale = value; AdjustControls(); } }

        public int BorderSize = 1;

        public int SelectedTile = -1;

        public Action<int> TileDoubleClick;

        public Point mouseDownPos;

        public Editor EditorInstance; 

        bool _flipHorizontal;
        bool _flipVertical;

        public bool FlipHorizontal { get { return _flipHorizontal; } set { _flipHorizontal = value; graphicPanel.Render(); } }
        public bool FlipVertical { get { return _flipVertical; } set { _flipVertical = value; graphicPanel.Render(); } }

        public TilesList(Editor instance)
        {
            InitializeComponent();
            this.graphicPanel = new DevicePanel(instance);
            SetupGraphicsPanel();
            EditorInstance = instance;

            if (Properties.Settings.Default.NightMode)
            {
                graphicPanel.DeviceBackColor = Editor.darkTheme1;
            }
            
            graphicPanel.Init(this);
        }

        public void SetupGraphicsPanel()
        {
            this.graphicPanel.AllowDrop = true;
            this.graphicPanel.DeviceBackColor = System.Drawing.Color.White;
            this.graphicPanel.Location = new System.Drawing.Point(0, 0);
            this.graphicPanel.Name = "graphicPanel";
            this.graphicPanel.Size = new System.Drawing.Size(126, 146);
            this.graphicPanel.TabIndex = 2;
            this.graphicPanel.OnRender += new ManiacEditor.RenderEventHandler(this.graphicPanel_OnRender);
            this.graphicPanel.DragEnter += new System.Windows.Forms.DragEventHandler(this.graphicPanel_DragEnter);
            this.graphicPanel.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.graphicPanel_MouseDoubleClick);
            this.graphicPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.graphicPanel_MouseDown);
            this.graphicPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.graphicPanel_MouseMove);
            this.graphicPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.graphicPanel_MouseUp);
            this.graphicPanel.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.graphicPanel_MouseWheel);
            this.graphicPanel.Resize += new System.EventHandler(this.graphicPanel_Resize);
            this.panel1.Controls.Add(this.graphicPanel);

			vScrollBar1 = new Interfaces.VScrollBar();
			vScrollBar1Host.Child = vScrollBar1;
			this.vScrollBar1.scroller.ValueChanged += new System.Windows.RoutedPropertyChangedEventHandler<double>(this.vScrollBar1_ValueChanged);
		}
        private void ResizeGraphicPanel(int width = 0, int height = 0)
        {
            graphicPanel.Width = width;
            graphicPanel.Height = height;
        }

        public void Reload(string colors = null)
        {
            TilesImage.Reload(colors);
            graphicPanel.Refresh();
        }

        public void DisposeTextures()
        {
            TilesImage?.DisposeTextures();
        }

        private void graphicPanel_Resize(object sender, EventArgs e)
        {
            graphicPanel.ResetDevice();

        }

        private void graphicPanel_OnRender(object sender, DeviceEventArgs e)
        {
            var lineColor = System.Drawing.Color.Black;
            if (Settings.mySettings.NightMode) lineColor = Editor.darkTheme5;
            if (TilesImage != null) {
                // Create a local version of the graphics object for the PictureBox.

                int tile_size = (TILE_SIZE + BorderSize * 2);

                int tiles_per_line = panel1.Width / tile_size / TileScale;

                // Draw a string on the PictureBox.
                for (int i = ((int)vScrollBar1.scroller.Value / tile_size / TileScale) * tiles_per_line; i < (((int)vScrollBar1.scroller.Value + panel1.Height) / tile_size / TileScale) * tiles_per_line + tiles_per_line; ++i)
                {
                    if (i < 0x400)
                    {
                        graphicPanel.DrawBitmap(TilesImage.GetTexture(graphicPanel._device, new Rectangle(0, i * TILE_SIZE, TILE_SIZE, TILE_SIZE), _flipHorizontal, _flipVertical),
                            (i % tiles_per_line) * tile_size + BorderSize, (i / tiles_per_line) * tile_size + BorderSize, TILE_SIZE, TILE_SIZE, false, 255);
                    }
                    else
                    {
                        // Draw red X
                        graphicPanel.DrawLine(
                            (i % tiles_per_line) * tile_size + BorderSize, 
                            (i / tiles_per_line) * tile_size + BorderSize, 
                            (i % tiles_per_line) * tile_size + tile_size - BorderSize,
                            (i / tiles_per_line) * tile_size + tile_size - BorderSize, 
                            System.Drawing.Color.Red);
                        graphicPanel.DrawLine(
                            (i % tiles_per_line) * tile_size + tile_size - BorderSize * 2,
                            (i / tiles_per_line) * tile_size + BorderSize,
                            (i % tiles_per_line) * tile_size,
                            (i / tiles_per_line) * tile_size + tile_size - BorderSize,
                            System.Drawing.Color.Red);
                    }
                }
                for (int i = 0; i < tiles_per_line; ++i)
                {

                    graphicPanel.DrawLine(i * tile_size, (int)vScrollBar1.scroller.Value / TileScale, i * tile_size, ((int)vScrollBar1.scroller.Value + panel1.Height) / TileScale, lineColor);
                    graphicPanel.DrawLine(i * tile_size + tile_size - BorderSize, (int)vScrollBar1.scroller.Value / TileScale, i * tile_size + tile_size - BorderSize, ((int)vScrollBar1.scroller.Value + panel1.Height) / TileScale, lineColor);
                }
                for (int i = ((int)vScrollBar1.scroller.Value / tile_size / TileScale); i < (((int)vScrollBar1.scroller.Value + panel1.Height) / tile_size / TileScale) + 1; ++i)
                {
                    graphicPanel.DrawLine(0, i * tile_size, tiles_per_line * tile_size, i * tile_size, lineColor);
                    graphicPanel.DrawLine(0, i * tile_size + tile_size - BorderSize, tiles_per_line * tile_size, i * tile_size + tile_size - BorderSize, lineColor);
                }
                if (SelectedTile != -1)
                {
                    graphicPanel.DrawLine((SelectedTile % tiles_per_line) * tile_size, (SelectedTile / tiles_per_line) * tile_size, (SelectedTile % tiles_per_line) * tile_size + tile_size - BorderSize, (SelectedTile / tiles_per_line) * tile_size, System.Drawing.Color.Red);
                    graphicPanel.DrawLine((SelectedTile % tiles_per_line) * tile_size, (SelectedTile / tiles_per_line) * tile_size, (SelectedTile % tiles_per_line) * tile_size, (SelectedTile / tiles_per_line) * tile_size + tile_size - BorderSize, System.Drawing.Color.Red);
                    graphicPanel.DrawLine((SelectedTile % tiles_per_line) * tile_size + tile_size - BorderSize, (SelectedTile / tiles_per_line) * tile_size, (SelectedTile % tiles_per_line) * tile_size + tile_size - BorderSize, (SelectedTile / tiles_per_line) * tile_size + tile_size - BorderSize, System.Drawing.Color.Red);
                    graphicPanel.DrawLine((SelectedTile % tiles_per_line) * tile_size, (SelectedTile / tiles_per_line) * tile_size + tile_size - BorderSize, (SelectedTile % tiles_per_line) * tile_size + tile_size - BorderSize, (SelectedTile / tiles_per_line) * tile_size + tile_size - BorderSize, System.Drawing.Color.Red);
                    // TODO: Need to fix the line drawing the DevicePanel
                    graphicPanel.DrawLine((SelectedTile % tiles_per_line) * tile_size + tile_size - BorderSize, (SelectedTile / tiles_per_line) * tile_size + tile_size - BorderSize, (SelectedTile % tiles_per_line) * tile_size + tile_size - BorderSize, (SelectedTile / tiles_per_line) * tile_size + tile_size - BorderSize, System.Drawing.Color.Red);
                }
            }
        }

        public Rectangle GetScreen()
        {
            return new Rectangle(0, (int)vScrollBar1.scroller.Value, graphicPanel.DrawWidth, graphicPanel.DrawHeight);
        }

        public double GetZoom()
        {
            return TileScale;
        }

        private void panel1_Resize(object sender, EventArgs e)
        {
            AdjustControls();
        }

        private void AdjustControls()
        {
			try
			{
				int tile_size = (TILE_SIZE + BorderSize * 2);
				int tiles_per_line = panel1.Width / tile_size / TileScale;

				vScrollBar1.scroller.Maximum = ((0x400 + tiles_per_line + 1) / tiles_per_line) * tile_size * TileScale - vScrollBar1.scroller.LargeChange;
				vScrollBar1.scroller.LargeChange = Math.Min(panel1.Height, vScrollBar1.scroller.Maximum);
				if (vScrollBar1.scroller.LargeChange == vScrollBar1.scroller.Maximum)
					vScrollBar1.scroller.IsEnabled = false;
				else
					vScrollBar1.scroller.IsEnabled = true;
				vScrollBar1.scroller.SmallChange = tile_size * TileScale * (panel1.Height / tile_size / TileScale);
				vScrollBar1.scroller.Value = Math.Min(vScrollBar1.scroller.Value, vScrollBar1.scroller.Maximum);

				graphicPanel.DrawWidth = panel1.Width;
				graphicPanel.DrawHeight = (int)Math.Max(vScrollBar1.scroller.Maximum, panel1.Height);
				while (panel1.Width > graphicPanel.Width)
					graphicPanel.Width *= 2;
				while (panel1.Height > graphicPanel.Height)
					graphicPanel.Height *= 2;
				graphicPanel.Render();
			}
			catch
			{
				
			}

        }

        private void vScrollBar1_ValueChanged(object sender, EventArgs e)
        {
            graphicPanel.Render();
        }

		public void Render()
		{
			graphicPanel.Render();
		}

        private void graphicPanel_MouseWheel(object sender, MouseEventArgs e)
        {
            vScrollBar1.scroller.Value = Math.Max(Math.Min(vScrollBar1.scroller.Value - (e.Delta * vScrollBar1.scroller.SmallChange / 120), vScrollBar1.scroller.Maximum), 0);
        }

        private void ClickTile(int x, int y, bool rightClick = false, MouseEventArgs e = null)
        {
			
            int tile_size = (TILE_SIZE + BorderSize * 2);
            int tiles_per_line = panel1.Width / tile_size / TileScale;

            int tile_number = ((y / tile_size / TileScale) * tiles_per_line) + (x / tile_size / TileScale);
            if (x / tile_size / TileScale < tiles_per_line && tile_number >= 0 && tile_number < 0x400)
            {
                SelectedTile = tile_number;
                EditorInstance.ToolbarSelectedTile = tile_number.ToString();
                EditorInstance.TilesToolbar.editTile0WithTileManiacToolStripMenuItem.Header = String.Format("Edit Tile {0} in Tile Maniac", tile_number);
				if (rightClick)
				{
					EditorInstance.TilesToolbar.TileContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Mouse;
					EditorInstance.TilesToolbar.TileContextMenu.IsOpen = true;
				}

            }
            else
            {
                SelectedTile = -1;
            }
			graphicPanel.Render();
            EditorInstance.TilesToolbar.RefreshTileSelected();
        }


        private void graphicPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ClickTile(e.X, e.Y);
                mouseDownPos = e.Location;
            }
            else if (e.Button == MouseButtons.Right)
            {
                ClickTile(e.X, e.Y, true, e);
                mouseDownPos = e.Location;
            }
        }

        private void graphicPanel_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ClickTile(e.X, e.Y);
                if (SelectedTile != -1 && TileDoubleClick != null)
                {
                    TileDoubleClick(SelectedTile);
                }
            }
        }

        private void graphicPanel_MouseUp(object sender, MouseEventArgs e)
        {
        }

        private void graphicPanel_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void graphicPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                dynamic dx = e.X - mouseDownPos.X;
                dynamic dy = e.Y - mouseDownPos.Y;
                if (Math.Abs(dx) >= SystemInformation.DoubleClickSize.Width || Math.Abs(dy) >= SystemInformation.DoubleClickSize.Height)
                {
                    if (SelectedTile != -1)
                    {
                        Int32 val = SelectedTile;
                        DoDragDrop(val, DragDropEffects.Move);
                    }
                }
            }
        }

        public new void Dispose()
        {
            TilesImage?.Dispose();
            graphicPanel.Dispose();
            base.Dispose();
        }
    }
}
