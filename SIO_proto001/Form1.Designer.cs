namespace SIO_proto001
{
    partial class Form1
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.inputFilePath = new System.Windows.Forms.TextBox();
            this.outputFilePath = new System.Windows.Forms.TextBox();
            this.trashMode = new System.Windows.Forms.CheckBox();
            this.ExtractCp = new System.Windows.Forms.CheckBox();
            this.ExtractHp = new System.Windows.Forms.CheckBox();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            this.convertBtn = new System.Windows.Forms.Button();
            this.selectBtn = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Input";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "OutPut";
            // 
            // inputFilePath
            // 
            this.inputFilePath.Location = new System.Drawing.Point(72, 24);
            this.inputFilePath.Name = "inputFilePath";
            this.inputFilePath.Size = new System.Drawing.Size(336, 19);
            this.inputFilePath.TabIndex = 2;
            // 
            // outputFilePath
            // 
            this.outputFilePath.Location = new System.Drawing.Point(72, 48);
            this.outputFilePath.Name = "outputFilePath";
            this.outputFilePath.Size = new System.Drawing.Size(336, 19);
            this.outputFilePath.TabIndex = 3;
            this.outputFilePath.Text = "..\\..\\..\\SIO.txt";
            // 
            // trashMode
            // 
            this.trashMode.AutoSize = true;
            this.trashMode.Location = new System.Drawing.Point(24, 80);
            this.trashMode.Name = "trashMode";
            this.trashMode.Size = new System.Drawing.Size(84, 16);
            this.trashMode.TabIndex = 4;
            this.trashMode.Text = "ごみ箱もーど";
            this.trashMode.UseVisualStyleBackColor = true;
            // 
            // ExtractCp
            // 
            this.ExtractCp.AutoSize = true;
            this.ExtractCp.Location = new System.Drawing.Point(144, 80);
            this.ExtractCp.Name = "ExtractCp";
            this.ExtractCp.Size = new System.Drawing.Size(84, 16);
            this.ExtractCp.TabIndex = 5;
            this.ExtractCp.Text = "CPのみ抽出";
            this.ExtractCp.UseVisualStyleBackColor = true;
            // 
            // ExtractHp
            // 
            this.ExtractHp.AutoSize = true;
            this.ExtractHp.Location = new System.Drawing.Point(144, 104);
            this.ExtractHp.Name = "ExtractHp";
            this.ExtractHp.Size = new System.Drawing.Size(84, 16);
            this.ExtractHp.TabIndex = 6;
            this.ExtractHp.Text = "HPのみ抽出";
            this.ExtractHp.UseVisualStyleBackColor = true;
            // 
            // checkBox4
            // 
            this.checkBox4.AutoSize = true;
            this.checkBox4.Location = new System.Drawing.Point(240, 80);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(98, 16);
            this.checkBox4.TabIndex = 7;
            this.checkBox4.Text = "ﾉｰﾄﾞ1のみ抽出";
            this.checkBox4.UseVisualStyleBackColor = true;
            // 
            // convertBtn
            // 
            this.convertBtn.Location = new System.Drawing.Point(416, 64);
            this.convertBtn.Name = "convertBtn";
            this.convertBtn.Size = new System.Drawing.Size(64, 32);
            this.convertBtn.TabIndex = 8;
            this.convertBtn.Text = "convert";
            this.convertBtn.UseVisualStyleBackColor = true;
            this.convertBtn.Click += new System.EventHandler(this.convertBtn_Click);
            // 
            // selectBtn
            // 
            this.selectBtn.Location = new System.Drawing.Point(416, 24);
            this.selectBtn.Name = "selectBtn";
            this.selectBtn.Size = new System.Drawing.Size(64, 32);
            this.selectBtn.TabIndex = 9;
            this.selectBtn.Text = "select";
            this.selectBtn.UseVisualStyleBackColor = true;
            this.selectBtn.Click += new System.EventHandler(this.selectBtn_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(240, 104);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(98, 16);
            this.checkBox1.TabIndex = 10;
            this.checkBox1.Text = "ﾉｰﾄﾞ2のみ抽出";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightGray;
            this.ClientSize = new System.Drawing.Size(502, 138);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.selectBtn);
            this.Controls.Add(this.convertBtn);
            this.Controls.Add(this.checkBox4);
            this.Controls.Add(this.ExtractHp);
            this.Controls.Add(this.ExtractCp);
            this.Controls.Add(this.trashMode);
            this.Controls.Add(this.outputFilePath);
            this.Controls.Add(this.inputFilePath);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "SIOﾛｸﾞ変換ﾂｰﾙ";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox inputFilePath;
        private System.Windows.Forms.TextBox outputFilePath;
        private System.Windows.Forms.CheckBox trashMode;
        private System.Windows.Forms.CheckBox ExtractCp;
        private System.Windows.Forms.CheckBox ExtractHp;
        private System.Windows.Forms.CheckBox checkBox4;
        private System.Windows.Forms.Button convertBtn;
        private System.Windows.Forms.Button selectBtn;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}

