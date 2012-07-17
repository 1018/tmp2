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
            this.components = new System.ComponentModel.Container();
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
            this.filterNode0 = new System.Windows.Forms.CheckBox();
            this.filterNode1 = new System.Windows.Forms.CheckBox();
            this.filterNode3 = new System.Windows.Forms.CheckBox();
            this.filterNode2 = new System.Windows.Forms.CheckBox();
            this.filterNode5 = new System.Windows.Forms.CheckBox();
            this.filterNode4 = new System.Windows.Forms.CheckBox();
            this.CopyDirPath = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.filterNode7 = new System.Windows.Forms.CheckBox();
            this.filterNode6 = new System.Windows.Forms.CheckBox();
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
            this.label2.Location = new System.Drawing.Point(8, 74);
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
            this.inputFilePath.Size = new System.Drawing.Size(424, 19);
            this.inputFilePath.TabIndex = 2;
            this.inputFilePath.DragDrop += new System.Windows.Forms.DragEventHandler(this.inputFilePath_DragDrop);
            this.inputFilePath.DragEnter += new System.Windows.Forms.DragEventHandler(this.inputFilePath_DragEnter);
            // 
            // outputFilePath
            // 
            this.outputFilePath.AllowDrop = true;
            this.outputFilePath.Location = new System.Drawing.Point(8, 88);
            this.outputFilePath.Name = "outputFilePath";
            this.outputFilePath.Size = new System.Drawing.Size(424, 19);
            this.outputFilePath.TabIndex = 3;
            this.outputFilePath.DragDrop += new System.Windows.Forms.DragEventHandler(this.outputFilePath_DragDrop);
            this.outputFilePath.DragEnter += new System.Windows.Forms.DragEventHandler(this.outputFilePath_DragEnter);
            // 
            // trashMode
            // 
            this.trashMode.AutoSize = true;
            this.trashMode.Location = new System.Drawing.Point(8, 120);
            this.trashMode.Name = "trashMode";
            this.trashMode.Size = new System.Drawing.Size(107, 16);
            this.trashMode.TabIndex = 4;
            this.trashMode.Text = "すっきりもーど(仮)";
            this.trashMode.UseVisualStyleBackColor = true;
            // 
            // convertBtn
            // 
            this.convertBtn.Location = new System.Drawing.Point(440, 128);
            this.convertBtn.Name = "convertBtn";
            this.convertBtn.Size = new System.Drawing.Size(64, 32);
            this.convertBtn.TabIndex = 8;
            this.convertBtn.Text = "変換";
            this.convertBtn.UseVisualStyleBackColor = true;
            this.convertBtn.Click += new System.EventHandler(this.convertBtn_Click);
            // 
            // selectBtn
            // 
            this.selectBtn.Location = new System.Drawing.Point(440, 25);
            this.selectBtn.Name = "selectBtn";
            this.selectBtn.Size = new System.Drawing.Size(64, 32);
            this.selectBtn.TabIndex = 9;
            this.selectBtn.Text = "参照";
            this.selectBtn.UseVisualStyleBackColor = true;
            this.selectBtn.Click += new System.EventHandler(this.InputFileSelectBtn);
            // 
            // OutputFileSelectBtn
            // 
            this.OutputFileSelectBtn.Location = new System.Drawing.Point(440, 80);
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
            this.filterCp.Location = new System.Drawing.Point(120, 120);
            this.filterCp.Name = "filterCp";
            this.filterCp.Size = new System.Drawing.Size(84, 16);
            this.filterCp.TabIndex = 14;
            this.filterCp.Text = "CPのみ抽出";
            this.filterCp.UseVisualStyleBackColor = true;
            // 
            // filterHp
            // 
            this.filterHp.AutoSize = true;
            this.filterHp.Location = new System.Drawing.Point(120, 136);
            this.filterHp.Name = "filterHp";
            this.filterHp.Size = new System.Drawing.Size(84, 16);
            this.filterHp.TabIndex = 15;
            this.filterHp.Text = "HPのみ抽出";
            this.filterHp.UseVisualStyleBackColor = true;
            // 
            // filterNode0
            // 
            this.filterNode0.AutoSize = true;
            this.filterNode0.Location = new System.Drawing.Point(216, 120);
            this.filterNode0.Name = "filterNode0";
            this.filterNode0.Size = new System.Drawing.Size(98, 16);
            this.filterNode0.TabIndex = 16;
            this.filterNode0.Text = "ﾉｰﾄﾞ0のみ抽出";
            this.filterNode0.UseVisualStyleBackColor = true;
            // 
            // filterNode1
            // 
            this.filterNode1.AutoSize = true;
            this.filterNode1.Location = new System.Drawing.Point(216, 136);
            this.filterNode1.Name = "filterNode1";
            this.filterNode1.Size = new System.Drawing.Size(98, 16);
            this.filterNode1.TabIndex = 17;
            this.filterNode1.Text = "ﾉｰﾄﾞ1のみ抽出";
            this.filterNode1.UseVisualStyleBackColor = true;
            // 
            // filterNode3
            // 
            this.filterNode3.AutoSize = true;
            this.filterNode3.Location = new System.Drawing.Point(216, 168);
            this.filterNode3.Name = "filterNode3";
            this.filterNode3.Size = new System.Drawing.Size(98, 16);
            this.filterNode3.TabIndex = 19;
            this.filterNode3.Text = "ﾉｰﾄﾞ3のみ抽出";
            this.filterNode3.UseVisualStyleBackColor = true;
            // 
            // filterNode2
            // 
            this.filterNode2.AutoSize = true;
            this.filterNode2.Location = new System.Drawing.Point(216, 152);
            this.filterNode2.Name = "filterNode2";
            this.filterNode2.Size = new System.Drawing.Size(98, 16);
            this.filterNode2.TabIndex = 18;
            this.filterNode2.Text = "ﾉｰﾄﾞ2のみ抽出";
            this.filterNode2.UseVisualStyleBackColor = true;
            // 
            // filterNode5
            // 
            this.filterNode5.AutoSize = true;
            this.filterNode5.Location = new System.Drawing.Point(328, 136);
            this.filterNode5.Name = "filterNode5";
            this.filterNode5.Size = new System.Drawing.Size(98, 16);
            this.filterNode5.TabIndex = 21;
            this.filterNode5.Text = "ﾉｰﾄﾞ5のみ抽出";
            this.filterNode5.UseVisualStyleBackColor = true;
            // 
            // filterNode4
            // 
            this.filterNode4.AutoSize = true;
            this.filterNode4.Location = new System.Drawing.Point(328, 120);
            this.filterNode4.Name = "filterNode4";
            this.filterNode4.Size = new System.Drawing.Size(98, 16);
            this.filterNode4.TabIndex = 20;
            this.filterNode4.Text = "ﾉｰﾄﾞ4のみ抽出";
            this.filterNode4.UseVisualStyleBackColor = true;
            // 
            // CopyDirPath
            // 
            this.CopyDirPath.Font = new System.Drawing.Font("MS UI Gothic", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.CopyDirPath.Location = new System.Drawing.Point(200, 57);
            this.CopyDirPath.Name = "CopyDirPath";
            this.CopyDirPath.Size = new System.Drawing.Size(32, 23);
            this.CopyDirPath.TabIndex = 22;
            this.CopyDirPath.Text = "↓";
            this.toolTip1.SetToolTip(this.CopyDirPath, "ﾌｫﾙﾀﾞﾊﾟｽをｺﾋﾟｰします");
            this.CopyDirPath.UseVisualStyleBackColor = true;
            this.CopyDirPath.Click += new System.EventHandler(this.CopyDirPath_Click);
            // 
            // filterNode7
            // 
            this.filterNode7.AutoSize = true;
            this.filterNode7.Location = new System.Drawing.Point(328, 168);
            this.filterNode7.Name = "filterNode7";
            this.filterNode7.Size = new System.Drawing.Size(98, 16);
            this.filterNode7.TabIndex = 24;
            this.filterNode7.Text = "ﾉｰﾄﾞ7のみ抽出";
            this.filterNode7.UseVisualStyleBackColor = true;
            // 
            // filterNode6
            // 
            this.filterNode6.AutoSize = true;
            this.filterNode6.Location = new System.Drawing.Point(328, 152);
            this.filterNode6.Name = "filterNode6";
            this.filterNode6.Size = new System.Drawing.Size(98, 16);
            this.filterNode6.TabIndex = 23;
            this.filterNode6.Text = "ﾉｰﾄﾞ6のみ抽出";
            this.filterNode6.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightGray;
            this.ClientSize = new System.Drawing.Size(527, 195);
            this.Controls.Add(this.filterNode7);
            this.Controls.Add(this.filterNode6);
            this.Controls.Add(this.CopyDirPath);
            this.Controls.Add(this.filterNode5);
            this.Controls.Add(this.filterNode4);
            this.Controls.Add(this.filterNode3);
            this.Controls.Add(this.filterNode2);
            this.Controls.Add(this.filterNode1);
            this.Controls.Add(this.filterNode0);
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
        private System.Windows.Forms.CheckBox filterNode0;
        private System.Windows.Forms.CheckBox filterNode1;
        private System.Windows.Forms.CheckBox filterNode3;
        private System.Windows.Forms.CheckBox filterNode2;
        private System.Windows.Forms.CheckBox filterNode5;
        private System.Windows.Forms.CheckBox filterNode4;
        private System.Windows.Forms.Button CopyDirPath;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox filterNode7;
        private System.Windows.Forms.CheckBox filterNode6;
    }
}

