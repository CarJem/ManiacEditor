namespace ManiacEditor
{
    partial class TilesList
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
			this.vScrollBar1Host = new System.Windows.Forms.Integration.ElementHost();
			this.panel1 = new System.Windows.Forms.Panel();
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.editTile0WithTileManiacToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.contextMenuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// vScrollBar1Host
			// 
			this.vScrollBar1Host.Dock = System.Windows.Forms.DockStyle.Right;
			this.vScrollBar1Host.Location = new System.Drawing.Point(126, 0);
			this.vScrollBar1Host.Name = "vScrollBar1Host";
			this.vScrollBar1Host.Size = new System.Drawing.Size(20, 146);
			this.vScrollBar1Host.TabIndex = 0;
			this.vScrollBar1Host.Child = null;
			// 
			// panel1
			// 
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(126, 146);
			this.panel1.TabIndex = 2;
			this.panel1.Resize += new System.EventHandler(this.panel1_Resize);
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editTile0WithTileManiacToolStripMenuItem});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(224, 48);
			// 
			// editTile0WithTileManiacToolStripMenuItem
			// 
			this.editTile0WithTileManiacToolStripMenuItem.Name = "editTile0WithTileManiacToolStripMenuItem";
			this.editTile0WithTileManiacToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
			this.editTile0WithTileManiacToolStripMenuItem.Text = "Edit Tile {0} with Tile Maniac";
			this.editTile0WithTileManiacToolStripMenuItem.Click += new System.EventHandler(this.editTile0WithTileManiacToolStripMenuItem_Click);
			// 
			// TilesList
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.vScrollBar1Host);
			this.Name = "TilesList";
			this.Size = new System.Drawing.Size(146, 146);
			this.contextMenuStrip1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

        private System.Windows.Forms.Integration.ElementHost vScrollBar1Host;
        private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripMenuItem editTile0WithTileManiacToolStripMenuItem;
	}
}
