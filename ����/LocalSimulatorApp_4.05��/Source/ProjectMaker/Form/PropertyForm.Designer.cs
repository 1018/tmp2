namespace LocalSimulator.ProjectMaker
{
    partial class PropertyForm
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
            this.PropertyView = new LocalSimulator.ProjectMaker.CustomPropertyGrid();
            this.SuspendLayout();
            // 
            // PropertyView
            // 
            this.PropertyView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PropertyView.Location = new System.Drawing.Point(0, 0);
            this.PropertyView.Name = "PropertyView";
            this.PropertyView.Size = new System.Drawing.Size(290, 271);
            this.PropertyView.TabIndex = 2;
            this.PropertyView.ToolbarVisible = false;
            this.PropertyView.PropertyValueValidated += new LocalSimulator.ProjectMaker.CustomPropertyGrid.PropertyValueValidatedEventHandler(this.PropertyView_Validated);
            this.PropertyView.Enter += new System.EventHandler(this.PropertyView_Enter);
            this.PropertyView.PropertyValueValidating += new LocalSimulator.ProjectMaker.CustomPropertyGrid.PropertyValueValidatingEventHandler(this.PropertyView_Validating);
            // 
            // PropertyForm
            // 
            this.ClientSize = new System.Drawing.Size(290, 271);
            this.Controls.Add(this.PropertyView);
            this.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Name = "PropertyForm";
            this.Text = "プロパティウィンドウ";
            this.ResumeLayout(false);

        }

        #endregion

        public CustomPropertyGrid PropertyView;
    }
}
