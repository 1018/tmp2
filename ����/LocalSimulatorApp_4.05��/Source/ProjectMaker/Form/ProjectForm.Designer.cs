namespace LocalSimulator.ProjectMaker
{
    partial class ProjectForm
    {
        /// <summary>
        /// 必要なデザイナ変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナで生成されたコード

        /// <summary>
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.ProjectWindow = new System.Windows.Forms.TreeView();
            this.TreeContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.CutMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.CopyMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.PasteMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.DeleteMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.ExportMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.TreeContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // ProjectWindow
            // 
            this.ProjectWindow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ProjectWindow.HideSelection = false;
            this.ProjectWindow.LabelEdit = true;
            this.ProjectWindow.Location = new System.Drawing.Point(0, 0);
            this.ProjectWindow.Name = "ProjectWindow";
            this.ProjectWindow.Size = new System.Drawing.Size(290, 271);
            this.ProjectWindow.TabIndex = 1;
            this.ProjectWindow.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.ProjectWindow_NodeMouseDoubleClick);
            this.ProjectWindow.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.ProjectWindow_AfterLabelEdit);
            this.ProjectWindow.Enter += new System.EventHandler(this.ProjectWindow_Enter);
            this.ProjectWindow.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ProjectWindow_MouseDown);
            this.ProjectWindow.BeforeLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.ProjectWindow_BeforeLabelEdit);
            // 
            // TreeContextMenu
            // 
            this.TreeContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CutMenu,
            this.CopyMenu,
            this.PasteMenu,
            this.MenuSeparator,
            this.DeleteMenu,
            this.ExportMenu});
            this.TreeContextMenu.Name = "contextMenuStrip1";
            this.TreeContextMenu.Size = new System.Drawing.Size(125, 120);
            this.TreeContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.TreeContextMenu_Opening);
            // 
            // CutMenu
            // 
            this.CutMenu.Name = "CutMenu";
            this.CutMenu.Size = new System.Drawing.Size(124, 22);
            this.CutMenu.Text = "切り取り";
            this.CutMenu.Click += new System.EventHandler(this.CutMenu_Click);
            // 
            // CopyMenu
            // 
            this.CopyMenu.Name = "CopyMenu";
            this.CopyMenu.Size = new System.Drawing.Size(124, 22);
            this.CopyMenu.Text = "コピー";
            this.CopyMenu.Click += new System.EventHandler(this.CopyMenu_Click);
            // 
            // PasteMenu
            // 
            this.PasteMenu.Name = "PasteMenu";
            this.PasteMenu.Size = new System.Drawing.Size(124, 22);
            this.PasteMenu.Text = "貼り付け";
            this.PasteMenu.Click += new System.EventHandler(this.PasteMenu_Click);
            // 
            // MenuSeparator
            // 
            this.MenuSeparator.Name = "MenuSeparator";
            this.MenuSeparator.Size = new System.Drawing.Size(121, 6);
            // 
            // DeleteMenu
            // 
            this.DeleteMenu.Name = "DeleteMenu";
            this.DeleteMenu.Size = new System.Drawing.Size(124, 22);
            this.DeleteMenu.Text = "削除";
            this.DeleteMenu.Click += new System.EventHandler(this.DeleteMenu_Click);
            // 
            // ExportMenu
            // 
            this.ExportMenu.Name = "ExportMenu";
            this.ExportMenu.Size = new System.Drawing.Size(124, 22);
            this.ExportMenu.Text = "エクスポート";
            this.ExportMenu.Click += new System.EventHandler(this.ExportMenu_Click);
            // 
            // ProjectForm
            // 
            this.ClientSize = new System.Drawing.Size(290, 271);
            this.Controls.Add(this.ProjectWindow);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Name = "ProjectForm";
            this.Text = "プロジェクトウィンドウ";
            this.TreeContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.TreeView ProjectWindow;
        private System.Windows.Forms.ContextMenuStrip TreeContextMenu;
        private System.Windows.Forms.ToolStripMenuItem CutMenu;
        private System.Windows.Forms.ToolStripMenuItem CopyMenu;
        private System.Windows.Forms.ToolStripMenuItem PasteMenu;
        private System.Windows.Forms.ToolStripSeparator MenuSeparator;
        private System.Windows.Forms.ToolStripMenuItem DeleteMenu;
        private System.Windows.Forms.ToolStripMenuItem ExportMenu;


    }
}
