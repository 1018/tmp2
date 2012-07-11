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
            this.filterCp = new System.Windows.Forms.CheckBox();
            this.filterHp = new System.Windows.Forms.CheckBox();
            this.filterNode1 = new System.Windows.Forms.CheckBox();
            this.convertBtn = new System.Windows.Forms.Button();
            this.selectBtn = new System.Windows.Forms.Button();
            this.filterNode2 = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Input File Path";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "OutPut File Path";
            // 
            // inputFilePath
            // 
            this.inputFilePath.AllowDrop = true;
            this.inputFilePath.Location = new System.Drawing.Point(104, 24);
            this.inputFilePath.Name = "inputFilePath";
            this.inputFilePath.Size = new System.Drawing.Size(336, 19);
            this.inputFilePath.TabIndex = 2;
            this.inputFilePath.DragDrop += new System.Windows.Forms.DragEventHandler(this.inputFilePath_DragDrop);
            this.inputFilePath.DragEnter += new System.Windows.Forms.DragEventHandler(this.inputFilePath_DragEnter);
            // 
            // outputFilePath
            // 
            this.outputFilePath.Location = new System.Drawing.Point(104, 48);
            this.outputFilePath.Name = "outputFilePath";
            this.outputFilePath.Size = new System.Drawing.Size(336, 19);
            this.outputFilePath.TabIndex = 3;
            this.outputFilePath.Text = "..\\..\\..\\SIO.txt";
            // 
            // trashMode
            // 
            this.trashMode.AutoSize = true;
            this.trashMode.Location = new System.Drawing.Point(104, 80);
            this.trashMode.Name = "trashMode";
            this.trashMode.Size = new System.Drawing.Size(84, 16);
            this.trashMode.TabIndex = 4;
            this.trashMode.Text = "ごみ箱もーど";
            this.trashMode.UseVisualStyleBackColor = true;
            // 
            // filterCp
            // 
            this.filterCp.AutoSize = true;
            this.filterCp.Location = new System.Drawing.Point(224, 80);
            this.filterCp.Name = "filterCp";
            this.filterCp.Size = new System.Drawing.Size(84, 16);
            this.filterCp.TabIndex = 5;
            this.filterCp.Text = "CPのみ抽出";
            this.filterCp.UseVisualStyleBackColor = true;
            // 
            // filterHp
            // 
            this.filterHp.AutoSize = true;
            this.filterHp.Location = new System.Drawing.Point(224, 104);
            this.filterHp.Name = "filterHp";
            this.filterHp.Size = new System.Drawing.Size(84, 16);
            this.filterHp.TabIndex = 6;
            this.filterHp.Text = "HPのみ抽出";
            this.filterHp.UseVisualStyleBackColor = true;
            // 
            // filterNode1
            // 
            this.filterNode1.AutoSize = true;
            this.filterNode1.Location = new System.Drawing.Point(320, 80);
            this.filterNode1.Name = "filterNode1";
            this.filterNode1.Size = new System.Drawing.Size(98, 16);
            this.filterNode1.TabIndex = 7;
            this.filterNode1.Text = "ﾉｰﾄﾞ1のみ抽出";
            this.filterNode1.UseVisualStyleBackColor = true;
            // 
            // convertBtn
            // 
            this.convertBtn.Location = new System.Drawing.Point(448, 64);
            this.convertBtn.Name = "convertBtn";
            this.convertBtn.Size = new System.Drawing.Size(64, 32);
            this.convertBtn.TabIndex = 8;
            this.convertBtn.Text = "Convert";
            this.convertBtn.UseVisualStyleBackColor = true;
            this.convertBtn.Click += new System.EventHandler(this.convertBtn_Click);
            // 
            // selectBtn
            // 
            this.selectBtn.Location = new System.Drawing.Point(448, 24);
            this.selectBtn.Name = "selectBtn";
            this.selectBtn.Size = new System.Drawing.Size(64, 32);
            this.selectBtn.TabIndex = 9;
            this.selectBtn.Text = "Select";
            this.selectBtn.UseVisualStyleBackColor = true;
            this.selectBtn.Click += new System.EventHandler(this.selectBtn_Click);
            // 
            // filterNode2
            // 
            this.filterNode2.AutoSize = true;
            this.filterNode2.Location = new System.Drawing.Point(320, 104);
            this.filterNode2.Name = "filterNode2";
            this.filterNode2.Size = new System.Drawing.Size(98, 16);
            this.filterNode2.TabIndex = 10;
            this.filterNode2.Text = "ﾉｰﾄﾞ2のみ抽出";
            this.filterNode2.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightGray;
            this.ClientSize = new System.Drawing.Size(535, 138);
            this.Controls.Add(this.filterNode2);
            this.Controls.Add(this.selectBtn);
            this.Controls.Add(this.convertBtn);
            this.Controls.Add(this.filterNode1);
            this.Controls.Add(this.filterHp);
            this.Controls.Add(this.filterCp);
            this.Controls.Add(this.trashMode);
            this.Controls.Add(this.outputFilePath);
            this.Controls.Add(this.inputFilePath);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "SIOﾛｸﾞ読込みﾂｰﾙ";
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
        private System.Windows.Forms.CheckBox filterCp;
        private System.Windows.Forms.CheckBox filterHp;
        private System.Windows.Forms.CheckBox filterNode1;
        private System.Windows.Forms.Button convertBtn;
        private System.Windows.Forms.Button selectBtn;
        private System.Windows.Forms.CheckBox filterNode2;
    }
}

