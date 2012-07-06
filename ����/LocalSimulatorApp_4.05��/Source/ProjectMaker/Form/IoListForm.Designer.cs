namespace LocalSimulator.ProjectMaker
{
    partial class IoListForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.UpdateCycle_High = new System.Windows.Forms.ToolStripMenuItem();
            this.UpdateCycle_Normal = new System.Windows.Forms.ToolStripMenuItem();
            this.UpdateCycle_Low = new System.Windows.Forms.ToolStripMenuItem();
            this.UpdateCycle_Stop = new System.Windows.Forms.ToolStripMenuItem();
            this.dgvIOView = new System.Windows.Forms.DataGridView();
            this.DesignMenuItem_Massage = new System.Windows.Forms.ToolStripMenuItem();
            this.DesignMenuItem_ToClipboard = new System.Windows.Forms.ToolStripMenuItem();
            this.DesignMenuItem_Option = new System.Windows.Forms.ToolStripMenuItem();
            this.DesignMenuItem_TopMost = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            ((System.ComponentModel.ISupportInitialize)(this.dgvIOView)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // UpdateCycle_High
            // 
            this.UpdateCycle_High.Name = "UpdateCycle_High";
            this.UpdateCycle_High.Size = new System.Drawing.Size(32, 19);
            // 
            // UpdateCycle_Normal
            // 
            this.UpdateCycle_Normal.Name = "UpdateCycle_Normal";
            this.UpdateCycle_Normal.Size = new System.Drawing.Size(32, 19);
            // 
            // UpdateCycle_Low
            // 
            this.UpdateCycle_Low.Name = "UpdateCycle_Low";
            this.UpdateCycle_Low.Size = new System.Drawing.Size(32, 19);
            // 
            // UpdateCycle_Stop
            // 
            this.UpdateCycle_Stop.Name = "UpdateCycle_Stop";
            this.UpdateCycle_Stop.Size = new System.Drawing.Size(32, 19);
            // 
            // dgvIOView
            // 
            this.dgvIOView.AllowUserToAddRows = false;
            this.dgvIOView.AllowUserToDeleteRows = false;
            this.dgvIOView.AllowUserToOrderColumns = true;
            this.dgvIOView.AllowUserToResizeRows = false;
            this.dgvIOView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvIOView.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dgvIOView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvIOView.Location = new System.Drawing.Point(12, 27);
            this.dgvIOView.MultiSelect = false;
            this.dgvIOView.Name = "dgvIOView";
            this.dgvIOView.RowHeadersVisible = false;
            this.dgvIOView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvIOView.RowTemplate.Height = 21;
            this.dgvIOView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvIOView.ShowCellErrors = false;
            this.dgvIOView.ShowCellToolTips = false;
            this.dgvIOView.ShowRowErrors = false;
            this.dgvIOView.Size = new System.Drawing.Size(501, 536);
            this.dgvIOView.TabIndex = 0;
            this.dgvIOView.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgvIOView_DataError);
            this.dgvIOView.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvIOView_CellEnter);
            // 
            // DesignMenuItem_Massage
            // 
            this.DesignMenuItem_Massage.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.DesignMenuItem_ToClipboard});
            this.DesignMenuItem_Massage.Name = "DesignMenuItem_Massage";
            this.DesignMenuItem_Massage.Size = new System.Drawing.Size(58, 20);
            this.DesignMenuItem_Massage.Text = "操作(&M)";
            // 
            // DesignMenuItem_ToClipboard
            // 
            this.DesignMenuItem_ToClipboard.Name = "DesignMenuItem_ToClipboard";
            this.DesignMenuItem_ToClipboard.Size = new System.Drawing.Size(188, 22);
            this.DesignMenuItem_ToClipboard.Text = "クリップボード貼り付け(&C)";
            this.DesignMenuItem_ToClipboard.Click += new System.EventHandler(this.ToClipboard_Click);
            // 
            // DesignMenuItem_Option
            // 
            this.DesignMenuItem_Option.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.DesignMenuItem_TopMost});
            this.DesignMenuItem_Option.Name = "DesignMenuItem_Option";
            this.DesignMenuItem_Option.Size = new System.Drawing.Size(76, 20);
            this.DesignMenuItem_Option.Text = "オプション(&O)";
            // 
            // DesignMenuItem_TopMost
            // 
            this.DesignMenuItem_TopMost.CheckOnClick = true;
            this.DesignMenuItem_TopMost.Name = "DesignMenuItem_TopMost";
            this.DesignMenuItem_TopMost.Size = new System.Drawing.Size(164, 22);
            this.DesignMenuItem_TopMost.Text = "常に手前に表示(&A)";
            this.DesignMenuItem_TopMost.CheckedChanged += new System.EventHandler(this.DlgTopMost_CheckedChanged);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.DesignMenuItem_Massage,
            this.DesignMenuItem_Option});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.menuStrip1.Size = new System.Drawing.Size(525, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip2";
            // 
            // IoListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(525, 575);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.dgvIOView);
            this.KeyPreview = true;
            this.Name = "IoListForm";
            this.Text = "I/Oリストフォーム";
            ((System.ComponentModel.ISupportInitialize)(this.dgvIOView)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.DataGridView dgvIOView;
        private System.Windows.Forms.ToolStripMenuItem UpdateCycle_Stop;
        private System.Windows.Forms.ToolStripMenuItem UpdateCycle_High;
        private System.Windows.Forms.ToolStripMenuItem UpdateCycle_Normal;
        private System.Windows.Forms.ToolStripMenuItem UpdateCycle_Low;
        private System.Windows.Forms.ToolStripMenuItem DesignMenuItem_Massage;
        private System.Windows.Forms.ToolStripMenuItem DesignMenuItem_ToClipboard;
        private System.Windows.Forms.ToolStripMenuItem DesignMenuItem_Option;
        private System.Windows.Forms.ToolStripMenuItem DesignMenuItem_TopMost;
        private System.Windows.Forms.MenuStrip menuStrip1;

    }
}