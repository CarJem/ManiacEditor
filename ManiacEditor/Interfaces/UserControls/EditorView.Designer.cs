namespace ManiacEditor.Interfaces
{
    partial class EditorView
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
			this.mainPanel = new System.Windows.Forms.Panel();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.viewPanel = new System.Windows.Forms.Panel();
			this.hScrollBar1Host = new System.Windows.Forms.Integration.ElementHost();
			this.vScrollBar1Host = new System.Windows.Forms.Integration.ElementHost();
			this.mainPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.viewPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// mainPanel
			// 
			this.mainPanel.AutoSize = true;
			this.mainPanel.Controls.Add(this.splitContainer1);
			this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mainPanel.Location = new System.Drawing.Point(0, 0);
			this.mainPanel.Margin = new System.Windows.Forms.Padding(0);
			this.mainPanel.Name = "mainPanel";
			this.mainPanel.Size = new System.Drawing.Size(1389, 697);
			this.mainPanel.TabIndex = 11;
			// 
			// splitContainer1
			// 
			this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Margin = new System.Windows.Forms.Padding(0);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.viewPanel);
			this.splitContainer1.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.No;
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.splitContainer1.Size = new System.Drawing.Size(1389, 697);
			this.splitContainer1.SplitterDistance = 1269;
			this.splitContainer1.TabIndex = 5;
			// 
			// viewPanel
			// 
			this.viewPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.viewPanel.Controls.Add(this.vScrollBar1Host);
			this.viewPanel.Controls.Add(this.hScrollBar1Host);
			this.viewPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.viewPanel.Location = new System.Drawing.Point(0, 0);
			this.viewPanel.Name = "viewPanel";
			this.viewPanel.Size = new System.Drawing.Size(1267, 695);
			this.viewPanel.TabIndex = 13;
			// 
			// hScrollBar1Host
			// 
			this.hScrollBar1Host.AutoSize = true;
			this.hScrollBar1Host.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.hScrollBar1Host.Location = new System.Drawing.Point(0, 694);
			this.hScrollBar1Host.Margin = new System.Windows.Forms.Padding(0);
			this.hScrollBar1Host.Name = "hScrollBar1Host";
			this.hScrollBar1Host.Size = new System.Drawing.Size(1267, 1);
			this.hScrollBar1Host.TabIndex = 11;
			this.hScrollBar1Host.Child = null;
			// 
			// vScrollBar1Host
			// 
			this.vScrollBar1Host.AutoSize = true;
			this.vScrollBar1Host.Dock = System.Windows.Forms.DockStyle.Right;
			this.vScrollBar1Host.Location = new System.Drawing.Point(1266, 0);
			this.vScrollBar1Host.Margin = new System.Windows.Forms.Padding(0);
			this.vScrollBar1Host.Name = "vScrollBar1Host";
			this.vScrollBar1Host.Size = new System.Drawing.Size(1, 694);
			this.vScrollBar1Host.TabIndex = 12;
			this.vScrollBar1Host.Child = null;
			// 
			// EditorView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.mainPanel);
			this.Name = "EditorView";
			this.Size = new System.Drawing.Size(1389, 697);
			this.Load += new System.EventHandler(this.EditorView_Load);
			this.mainPanel.ResumeLayout(false);
			this.splitContainer1.Panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.viewPanel.ResumeLayout(false);
			this.viewPanel.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Panel mainPanel;
		public System.Windows.Forms.SplitContainer splitContainer1;
		public System.Windows.Forms.Panel viewPanel;
		public System.Windows.Forms.Integration.ElementHost vScrollBar1Host;
		public System.Windows.Forms.Integration.ElementHost hScrollBar1Host;
	}
}
