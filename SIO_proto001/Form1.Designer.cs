namespace SioLog
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
            this.convertBtn = new System.Windows.Forms.Button();
            this.selectBtn = new System.Windows.Forms.Button();
            this.OutputFileSelectBtn = new System.Windows.Forms.Button();
            this.filterCp = new System.Windows.Forms.CheckBox();
            this.filterHp = new System.Windows.Forms.CheckBox();
            this.filterNode1 = new System.Windows.Forms.CheckBox();
            this.filterNode2 = new System.Windows.Forms.CheckBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(104, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "読込み先ファイル名：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "出力先ﾌｫﾙﾀﾞ名：";
            // 
            // inputFilePath
            // 
            this.inputFilePath.AllowDrop = true;
            this.inputFilePath.Location = new System.Drawing.Point(8, 32);
            this.inputFilePath.Name = "inputFilePath";
            this.inputFilePath.Size = new System.Drawing.Size(456, 19);
            this.inputFilePath.TabIndex = 2;
            this.inputFilePath.DragDrop += new System.Windows.Forms.DragEventHandler(this.inputFilePath_DragDrop);
            this.inputFilePath.DragEnter += new System.Windows.Forms.DragEventHandler(this.inputFilePath_DragEnter);
            // 
            // outputFilePath
            // 
            this.outputFilePath.AllowDrop = true;
            this.outputFilePath.Location = new System.Drawing.Point(8, 80);
            this.outputFilePath.Name = "outputFilePath";
            this.outputFilePath.Size = new System.Drawing.Size(456, 19);
            this.outputFilePath.TabIndex = 3;
            // 
            // trashMode
            // 
            this.trashMode.AutoSize = true;
            this.trashMode.Location = new System.Drawing.Point(16, 112);
            this.trashMode.Name = "trashMode";
            this.trashMode.Size = new System.Drawing.Size(107, 16);
            this.trashMode.TabIndex = 4;
            this.trashMode.Text = "すっきりもーど(仮)";
            this.trashMode.UseVisualStyleBackColor = true;
            // 
            // convertBtn
            // 
            this.convertBtn.Location = new System.Drawing.Point(472, 112);
            this.convertBtn.Name = "convertBtn";
            this.convertBtn.Size = new System.Drawing.Size(64, 32);
            this.convertBtn.TabIndex = 8;
            this.convertBtn.Text = "変換";
            this.convertBtn.UseVisualStyleBackColor = true;
            this.convertBtn.Click += new System.EventHandler(this.convertBtn_Click);
            // 
            // selectBtn
            // 
            this.selectBtn.Location = new System.Drawing.Point(472, 25);
            this.selectBtn.Name = "selectBtn";
            this.selectBtn.Size = new System.Drawing.Size(64, 32);
            this.selectBtn.TabIndex = 9;
            this.selectBtn.Text = "参照";
            this.selectBtn.UseVisualStyleBackColor = true;
            this.selectBtn.Click += new System.EventHandler(this.InputFileSelectBtn);
            // 
            // OutputFileSelectBtn
            // 
            this.OutputFileSelectBtn.Location = new System.Drawing.Point(472, 72);
            this.OutputFileSelectBtn.Name = "OutputFileSelectBtn";
            this.OutputFileSelectBtn.Size = new System.Drawing.Size(64, 32);
            this.OutputFileSelectBtn.TabIndex = 11;
            this.OutputFileSelectBtn.Text = "参照";
            this.OutputFileSelectBtn.UseVisualStyleBackColor = true;
            this.OutputFileSelectBtn.Click += new System.EventHandler(this.OutputFileSelectBtn_Click);
            // 
            // filterCp
            // 
            this.filterCp.AutoSize = true;
            this.filterCp.Location = new System.Drawing.Point(136, 112);
            this.filterCp.Name = "filterCp";
            this.filterCp.Size = new System.Drawing.Size(84, 16);
            this.filterCp.TabIndex = 14;
            this.filterCp.Text = "CPのみ抽出";
            this.filterCp.UseVisualStyleBackColor = true;
            // 
            // filterHp
            // 
            this.filterHp.AutoSize = true;
            this.filterHp.Location = new System.Drawing.Point(136, 128);
            this.filterHp.Name = "filterHp";
            this.filterHp.Size = new System.Drawing.Size(84, 16);
            this.filterHp.TabIndex = 15;
            this.filterHp.Text = "HPのみ抽出";
            this.filterHp.UseVisualStyleBackColor = true;
            // 
            // filterNode1
            // 
            this.filterNode1.AutoSize = true;
            this.filterNode1.Location = new System.Drawing.Point(240, 112);
            this.filterNode1.Name = "filterNode1";
            this.filterNode1.Size = new System.Drawing.Size(98, 16);
            this.filterNode1.TabIndex = 16;
            this.filterNode1.Text = "ﾉｰﾄﾞ1のみ抽出";
            this.filterNode1.UseVisualStyleBackColor = true;
            // 
            // filterNode2
            // 
            this.filterNode2.AutoSize = true;
            this.filterNode2.Location = new System.Drawing.Point(240, 128);
            this.filterNode2.Name = "filterNode2";
            this.filterNode2.Size = new System.Drawing.Size(98, 16);
            this.filterNode2.TabIndex = 17;
            this.filterNode2.Text = "ﾉｰﾄﾞ2のみ抽出";
            this.filterNode2.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(344, 128);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(98, 16);
            this.checkBox1.TabIndex = 19;
            this.checkBox1.Text = "ﾉｰﾄﾞ4のみ抽出";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(344, 112);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(98, 16);
            this.checkBox2.TabIndex = 18;
            this.checkBox2.Text = "ﾉｰﾄﾞ3のみ抽出";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightGray;
            this.ClientSize = new System.Drawing.Size(563, 159);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.filterNode2);
            this.Controls.Add(this.filterNode1);
            this.Controls.Add(this.filterHp);
            this.Controls.Add(this.filterCp);
            this.Controls.Add(this.OutputFileSelectBtn);
            this.Controls.Add(this.selectBtn);
            this.Controls.Add(this.convertBtn);
            this.Controls.Add(this.trashMode);
            this.Controls.Add(this.outputFilePath);
            this.Controls.Add(this.inputFilePath);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "SIOﾛｸﾞ変換ﾂｰﾙ";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form1_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form1_DragEnter);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox inputFilePath;
        private System.Windows.Forms.TextBox outputFilePath;
        private System.Windows.Forms.CheckBox trashMode;
        private System.Windows.Forms.Button convertBtn;
        private System.Windows.Forms.Button selectBtn;
        private System.Windows.Forms.Button OutputFileSelectBtn;
        private System.Windows.Forms.CheckBox filterCp;
        private System.Windows.Forms.CheckBox filterHp;
        private System.Windows.Forms.CheckBox filterNode1;
        private System.Windows.Forms.CheckBox filterNode2;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.CheckBox checkBox2;
    }
}

