namespace LocalSimulator.ProjectMaker
{
    partial class SymbolListForm
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.SymbolListView = new System.Windows.Forms.ListView();
            this.SymbolImageList = new System.Windows.Forms.ImageList(this.components);
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(369, 20);
            this.tabControl1.TabIndex = 19;
            this.tabControl1.Selected += new System.Windows.Forms.TabControlEventHandler(this.tabControl1_Selected);
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 24);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(361, 0);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "全て表示";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // SymbolListView
            // 
            this.SymbolListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.SymbolListView.BackColor = System.Drawing.SystemColors.Menu;
            this.SymbolListView.HideSelection = false;
            this.SymbolListView.LargeImageList = this.SymbolImageList;
            this.SymbolListView.Location = new System.Drawing.Point(0, 20);
            this.SymbolListView.Name = "SymbolListView";
            this.SymbolListView.Size = new System.Drawing.Size(381, 376);
            this.SymbolListView.TabIndex = 0;
            this.SymbolListView.UseCompatibleStateImageBehavior = false;            
            this.SymbolListView.Click += new System.EventHandler(this.SymbolListView_Click);
            // 
            // SymbolImageList
            // 
            this.SymbolImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.SymbolImageList.ImageSize = new System.Drawing.Size(16, 16);
            this.SymbolImageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // SymbolListForm
            // 
            this.ClientSize = new System.Drawing.Size(381, 396);
            this.Controls.Add(this.SymbolListView);
            this.Controls.Add(this.tabControl1);
            this.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Name = "SymbolListForm";
            this.Text = "シンボルリストウィンドウ";
            this.Load += new System.EventHandler(this.SymbolListForm_Load);
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        public System.Windows.Forms.ListView SymbolListView;
        public System.Windows.Forms.ImageList SymbolImageList;

    }
}
