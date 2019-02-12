namespace ManiacEditor.Interfaces
{
    partial class RetroEDTileList
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.components = new System.ComponentModel.Container();
			this.vScrollBar1Old = new System.Windows.Forms.VScrollBar();
			this.hScrollBar1 = new System.Windows.Forms.HScrollBar();
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.removeChunkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.duplicateChunkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.moveChunkUpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.moveChunkDownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.importChunkFromClipboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.contextMenuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// vScrollBar1Old
			// 
			this.vScrollBar1Old.Dock = System.Windows.Forms.DockStyle.Right;
			this.vScrollBar1Old.LargeChange = 32;
			this.vScrollBar1Old.Location = new System.Drawing.Point(133, 0);
			this.vScrollBar1Old.Name = "vScrollBar1Old";
			this.vScrollBar1Old.Size = new System.Drawing.Size(17, 150);
			this.vScrollBar1Old.SmallChange = 16;
			this.vScrollBar1Old.TabIndex = 0;
			this.vScrollBar1Old.Visible = false;
			this.vScrollBar1Old.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBar1_Scroll);
			// 
			// hScrollBar1
			// 
			this.hScrollBar1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.hScrollBar1.LargeChange = 32;
			this.hScrollBar1.Location = new System.Drawing.Point(0, 133);
			this.hScrollBar1.Name = "hScrollBar1";
			this.hScrollBar1.Size = new System.Drawing.Size(133, 17);
			this.hScrollBar1.SmallChange = 16;
			this.hScrollBar1.TabIndex = 1;
			this.hScrollBar1.Visible = false;
			this.hScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBar1_Scroll);
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removeChunkToolStripMenuItem,
            this.duplicateChunkToolStripMenuItem,
            this.toolStripSeparator1,
            this.moveChunkUpToolStripMenuItem,
            this.moveChunkDownToolStripMenuItem,
            this.toolStripSeparator2,
            this.importChunkFromClipboardToolStripMenuItem});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(233, 126);
			// 
			// removeChunkToolStripMenuItem
			// 
			this.removeChunkToolStripMenuItem.Name = "removeChunkToolStripMenuItem";
			this.removeChunkToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
			this.removeChunkToolStripMenuItem.Text = "Remove Chunk";
			this.removeChunkToolStripMenuItem.Click += new System.EventHandler(this.removeChunkToolStripMenuItem_Click);
			// 
			// duplicateChunkToolStripMenuItem
			// 
			this.duplicateChunkToolStripMenuItem.Name = "duplicateChunkToolStripMenuItem";
			this.duplicateChunkToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
			this.duplicateChunkToolStripMenuItem.Text = "Duplicate Chunk";
			this.duplicateChunkToolStripMenuItem.Click += new System.EventHandler(this.duplicateChunkToolStripMenuItem_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(229, 6);
			// 
			// moveChunkUpToolStripMenuItem
			// 
			this.moveChunkUpToolStripMenuItem.Enabled = false;
			this.moveChunkUpToolStripMenuItem.Name = "moveChunkUpToolStripMenuItem";
			this.moveChunkUpToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
			this.moveChunkUpToolStripMenuItem.Text = "Move Chunk Up";
			// 
			// moveChunkDownToolStripMenuItem
			// 
			this.moveChunkDownToolStripMenuItem.Enabled = false;
			this.moveChunkDownToolStripMenuItem.Name = "moveChunkDownToolStripMenuItem";
			this.moveChunkDownToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
			this.moveChunkDownToolStripMenuItem.Text = "Move Chunk Down";
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(229, 6);
			// 
			// importChunkFromClipboardToolStripMenuItem
			// 
			this.importChunkFromClipboardToolStripMenuItem.Name = "importChunkFromClipboardToolStripMenuItem";
			this.importChunkFromClipboardToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
			this.importChunkFromClipboardToolStripMenuItem.Text = "Import Chunk from Clipboard";
			this.importChunkFromClipboardToolStripMenuItem.Click += new System.EventHandler(this.importChunkFromClipboardToolStripMenuItem_Click);
			// 
			// RetroEDTileList
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Window;
			this.Controls.Add(this.hScrollBar1);
			this.Controls.Add(this.vScrollBar1Old);
			this.DoubleBuffered = true;
			this.Name = "RetroEDTileList";
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.TileList_Paint);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TileList_KeyDown);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TileList_MouseDown);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.TileList_MouseMove);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.TileList_MouseUp);
			this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.TileList_MouseWheel);
			this.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.TileList_PreviewKeyDown);
			this.Resize += new System.EventHandler(this.TileList_Resize);
			this.contextMenuStrip1.ResumeLayout(false);
			this.ResumeLayout(false);

        }

		#endregion

		private System.Windows.Forms.VScrollBar vScrollBar1Old;
        private System.Windows.Forms.HScrollBar hScrollBar1;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		public System.Windows.Forms.ToolStripMenuItem removeChunkToolStripMenuItem;
		public System.Windows.Forms.ToolStripMenuItem duplicateChunkToolStripMenuItem;
		public System.Windows.Forms.ToolStripMenuItem moveChunkUpToolStripMenuItem;
		public System.Windows.Forms.ToolStripMenuItem moveChunkDownToolStripMenuItem;
		public System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem importChunkFromClipboardToolStripMenuItem;
	}
}
