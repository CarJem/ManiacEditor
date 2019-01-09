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
using SharpDX.Direct3D9;
using SharpDX;

namespace ManiacEditor
{
    public partial class ChunksList : UserControl, IDrawArea
    {
        public GIF TilesImage = null;

        private DevicePanel graphicPanel;

        const int TILE_SIZE = 16;

        public String SelectedTileNumber;

        int _tileScale = 2;
        public int TileScale { get { return _tileScale; } set { _tileScale = value; AdjustControls(); } }

        public int BorderSize = 1;

        public int SelectedTile = -1;

        public Action<int> TileDoubleClick;

        public System.Drawing.Point mouseDownPos;

        public Editor EditorInstance; 

        bool _flipHorizontal;
        bool _flipVertical;

        public bool FlipHorizontal { get { return _flipHorizontal; } set { _flipHorizontal = value; graphicPanel.Render(); } }
        public bool FlipVertical { get { return _flipVertical; } set { _flipVertical = value; graphicPanel.Render(); } }

        public ChunksList(Editor instance)
        {
            InitializeComponent();
            this.graphicPanel = new ManiacEditor.DevicePanel(instance);
            SetupGraphicsPanel();
            EditorInstance = instance;

            //if (Properties.Settings.Default.NightMode)
            //{
            //    graphicPanel.DeviceBackColor = Editor.Instance.darkTheme1;
            //}

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
            int CHUNK_GRID_HEIGHT = 10;
            int CHUNK_GRID_WIDTH = 2;   
            int CHUNK_SIZE = 128;


            for (int x = 0; x <= CHUNK_GRID_WIDTH-1; x++)
            {
                for (int y = 0; y < CHUNK_GRID_HEIGHT; y++)
                {
                    EditorInstance.ManiacChunks.DrawChunk(graphicPanel, x * 8, y * 8, 255, 1);
                }
            }

            for (int x2 = 0; x2 <= CHUNK_GRID_WIDTH; x2++)
            {
                graphicPanel.DrawLine(x2 * CHUNK_SIZE, 0, x2 * CHUNK_SIZE, CHUNK_SIZE * CHUNK_GRID_HEIGHT, System.Drawing.Color.Red);
            }

            for (int y2 = 0; y2 < CHUNK_GRID_HEIGHT; y2++)
            {
                graphicPanel.DrawLine(0, y2 * CHUNK_SIZE, 32 * CHUNK_GRID_HEIGHT, y2 * CHUNK_SIZE, System.Drawing.Color.Red);
            }

        }

        public System.Drawing.Rectangle GetScreen()
        {
            return new System.Drawing.Rectangle(0, vScrollBar1.Value, graphicPanel.DrawWidth, graphicPanel.DrawHeight);
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
            int tile_size = (TILE_SIZE + BorderSize * 2);
            int tiles_per_line = panel1.Width / tile_size / TileScale;

            vScrollBar1.Maximum = ((0x400 + tiles_per_line + 1) / tiles_per_line) * tile_size * TileScale;
            vScrollBar1.LargeChange = Math.Min(panel1.Height, vScrollBar1.Maximum);
            if (vScrollBar1.LargeChange == vScrollBar1.Maximum)
                vScrollBar1.Enabled = false;
            else
                vScrollBar1.Enabled = true;
            vScrollBar1.SmallChange = tile_size * TileScale * (panel1.Height / tile_size / TileScale);
            vScrollBar1.Value = Math.Min(vScrollBar1.Value, vScrollBar1.Maximum - vScrollBar1.LargeChange);

            graphicPanel.DrawWidth = panel1.Width;
            graphicPanel.DrawHeight = Math.Max(vScrollBar1.Maximum, panel1.Height);
            while (panel1.Width > graphicPanel.Width)
                graphicPanel.Width *= 2;
            while (panel1.Height > graphicPanel.Height)
                graphicPanel.Height *= 2;
            graphicPanel.Render();
        }

        private void vScrollBar1_ValueChanged(object sender, EventArgs e)
        {
            graphicPanel.Render();
        }

        private void graphicPanel_MouseWheel(object sender, MouseEventArgs e)
        {
            vScrollBar1.Value = Math.Max(Math.Min(vScrollBar1.Value - (e.Delta * vScrollBar1.SmallChange / 120), vScrollBar1.Maximum - vScrollBar1.LargeChange), 0);
        }

        private void ClickTile(int x, int y, bool rightClick = false, MouseEventArgs e = null)
        {

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
