namespace LocalSimulator.ProgramSelecter
{
    partial class SelectForm
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
            this.btnMainProgram = new System.Windows.Forms.Button();
            this.btnProjectMaker = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnMainProgram
            // 
            this.btnMainProgram.Location = new System.Drawing.Point(12, 12);
            this.btnMainProgram.Name = "btnMainProgram";
            this.btnMainProgram.Size = new System.Drawing.Size(134, 35);
            this.btnMainProgram.TabIndex = 0;
            this.btnMainProgram.Text = "MainProgram起動";
            this.btnMainProgram.UseVisualStyleBackColor = true;
            this.btnMainProgram.Click += new System.EventHandler(this.btnMainProgram_Click);
            // 
            // btnProjectMaker
            // 
            this.btnProjectMaker.Location = new System.Drawing.Point(12, 53);
            this.btnProjectMaker.Name = "btnProjectMaker";
            this.btnProjectMaker.Size = new System.Drawing.Size(134, 35);
            this.btnProjectMaker.TabIndex = 1;
            this.btnProjectMaker.Text = "ProjectMaker起動";
            this.btnProjectMaker.UseVisualStyleBackColor = true;
            this.btnProjectMaker.Click += new System.EventHandler(this.btnProjectMaker_Click);
            // 
            // SelectForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(160, 100);
            this.Controls.Add(this.btnProjectMaker);
            this.Controls.Add(this.btnMainProgram);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "SelectForm";
            this.Text = "起動選択";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnMainProgram;
        private System.Windows.Forms.Button btnProjectMaker;
    }
}

