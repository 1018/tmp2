namespace SymbolLibrary
{
    partial class MelsecSerialUnit_Draw
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

        #region コンポーネント デザイナで生成されたコード

        /// <summary> 
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.lampRUN = new System.Windows.Forms.Label();
            this.lampERR = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.txtUnitNum = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.customGroupBox1 = new SymbolLibrary.CustomGroupBox();
            this.txtCh2Status = new System.Windows.Forms.Label();
            this.lampCh2RD = new System.Windows.Forms.Label();
            this.lampCh2SD = new System.Windows.Forms.Label();
            this.lampCh2NEU = new System.Windows.Forms.Label();
            this.grpCh1 = new SymbolLibrary.CustomGroupBox();
            this.txtCh1Status = new System.Windows.Forms.Label();
            this.lampCh1RD = new System.Windows.Forms.Label();
            this.lampCh1SD = new System.Windows.Forms.Label();
            this.lampCh1NEU = new System.Windows.Forms.Label();
            this.customGroupBox1.SuspendLayout();
            this.grpCh1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lampRUN
            // 
            this.lampRUN.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lampRUN.Font = new System.Drawing.Font("MS UI Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lampRUN.Location = new System.Drawing.Point(20, 23);
            this.lampRUN.Name = "lampRUN";
            this.lampRUN.Size = new System.Drawing.Size(46, 20);
            this.lampRUN.TabIndex = 0;
            this.lampRUN.Text = "RUN";
            this.lampRUN.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lampERR
            // 
            this.lampERR.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lampERR.Font = new System.Drawing.Font("MS UI Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lampERR.Location = new System.Drawing.Point(72, 23);
            this.lampERR.Name = "lampERR";
            this.lampERR.Size = new System.Drawing.Size(46, 20);
            this.lampERR.TabIndex = 1;
            this.lampERR.Text = "ERR";
            this.lampERR.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(133, 27);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(19, 12);
            this.label9.TabIndex = 6;
            this.label9.Text = "№:";
            // 
            // txtUnitNum
            // 
            this.txtUnitNum.BackColor = System.Drawing.Color.White;
            this.txtUnitNum.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUnitNum.Font = new System.Drawing.Font("MS UI Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.txtUnitNum.Location = new System.Drawing.Point(153, 23);
            this.txtUnitNum.Name = "txtUnitNum";
            this.txtUnitNum.Size = new System.Drawing.Size(29, 20);
            this.txtUnitNum.TabIndex = 7;
            this.txtUnitNum.Text = "99";
            this.txtUnitNum.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Arial Black", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(61, 3);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(69, 15);
            this.label13.TabIndex = 8;
            this.label13.Text = "QJ71C24N";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // customGroupBox1
            // 
            this.customGroupBox1.BorderColor = System.Drawing.Color.Gray;
            this.customGroupBox1.Controls.Add(this.txtCh2Status);
            this.customGroupBox1.Controls.Add(this.lampCh2RD);
            this.customGroupBox1.Controls.Add(this.lampCh2SD);
            this.customGroupBox1.Controls.Add(this.lampCh2NEU);
            this.customGroupBox1.Location = new System.Drawing.Point(7, 121);
            this.customGroupBox1.Name = "customGroupBox1";
            this.customGroupBox1.Size = new System.Drawing.Size(175, 69);
            this.customGroupBox1.TabIndex = 5;
            this.customGroupBox1.TabStop = false;
            this.customGroupBox1.Text = "CH2";
            // 
            // txtCh2Status
            // 
            this.txtCh2Status.BackColor = System.Drawing.Color.White;
            this.txtCh2Status.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtCh2Status.Location = new System.Drawing.Point(13, 40);
            this.txtCh2Status.Name = "txtCh2Status";
            this.txtCh2Status.Size = new System.Drawing.Size(150, 23);
            this.txtCh2Status.TabIndex = 7;
            this.txtCh2Status.Text = "label12";
            this.txtCh2Status.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lampCh2RD
            // 
            this.lampCh2RD.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lampCh2RD.Font = new System.Drawing.Font("MS UI Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lampCh2RD.Location = new System.Drawing.Point(117, 15);
            this.lampCh2RD.Name = "lampCh2RD";
            this.lampCh2RD.Size = new System.Drawing.Size(46, 20);
            this.lampCh2RD.TabIndex = 5;
            this.lampCh2RD.Text = "RD";
            this.lampCh2RD.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lampCh2SD
            // 
            this.lampCh2SD.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lampCh2SD.Font = new System.Drawing.Font("MS UI Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lampCh2SD.Location = new System.Drawing.Point(65, 15);
            this.lampCh2SD.Name = "lampCh2SD";
            this.lampCh2SD.Size = new System.Drawing.Size(46, 20);
            this.lampCh2SD.TabIndex = 4;
            this.lampCh2SD.Text = "SD";
            this.lampCh2SD.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lampCh2NEU
            // 
            this.lampCh2NEU.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lampCh2NEU.Font = new System.Drawing.Font("MS UI Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lampCh2NEU.Location = new System.Drawing.Point(13, 15);
            this.lampCh2NEU.Name = "lampCh2NEU";
            this.lampCh2NEU.Size = new System.Drawing.Size(46, 20);
            this.lampCh2NEU.TabIndex = 3;
            this.lampCh2NEU.Text = "NEU.";
            this.lampCh2NEU.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // grpCh1
            // 
            this.grpCh1.BorderColor = System.Drawing.Color.Gray;
            this.grpCh1.Controls.Add(this.txtCh1Status);
            this.grpCh1.Controls.Add(this.lampCh1RD);
            this.grpCh1.Controls.Add(this.lampCh1SD);
            this.grpCh1.Controls.Add(this.lampCh1NEU);
            this.grpCh1.Location = new System.Drawing.Point(7, 46);
            this.grpCh1.Name = "grpCh1";
            this.grpCh1.Size = new System.Drawing.Size(175, 69);
            this.grpCh1.TabIndex = 2;
            this.grpCh1.TabStop = false;
            this.grpCh1.Text = "CH1";
            // 
            // txtCh1Status
            // 
            this.txtCh1Status.BackColor = System.Drawing.Color.White;
            this.txtCh1Status.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtCh1Status.Location = new System.Drawing.Point(13, 40);
            this.txtCh1Status.Name = "txtCh1Status";
            this.txtCh1Status.Size = new System.Drawing.Size(150, 23);
            this.txtCh1Status.TabIndex = 6;
            this.txtCh1Status.Text = "label11";
            this.txtCh1Status.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lampCh1RD
            // 
            this.lampCh1RD.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lampCh1RD.Font = new System.Drawing.Font("MS UI Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lampCh1RD.Location = new System.Drawing.Point(117, 15);
            this.lampCh1RD.Name = "lampCh1RD";
            this.lampCh1RD.Size = new System.Drawing.Size(46, 20);
            this.lampCh1RD.TabIndex = 5;
            this.lampCh1RD.Text = "RD";
            this.lampCh1RD.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lampCh1SD
            // 
            this.lampCh1SD.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lampCh1SD.Font = new System.Drawing.Font("MS UI Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lampCh1SD.Location = new System.Drawing.Point(65, 15);
            this.lampCh1SD.Name = "lampCh1SD";
            this.lampCh1SD.Size = new System.Drawing.Size(46, 20);
            this.lampCh1SD.TabIndex = 4;
            this.lampCh1SD.Text = "SD";
            this.lampCh1SD.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lampCh1NEU
            // 
            this.lampCh1NEU.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lampCh1NEU.Font = new System.Drawing.Font("MS UI Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lampCh1NEU.Location = new System.Drawing.Point(13, 15);
            this.lampCh1NEU.Name = "lampCh1NEU";
            this.lampCh1NEU.Size = new System.Drawing.Size(46, 20);
            this.lampCh1NEU.TabIndex = 3;
            this.lampCh1NEU.Text = "NEU.";
            this.lampCh1NEU.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MelsecSerialUnit_Draw
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightGray;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.label13);
            this.Controls.Add(this.txtUnitNum);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.customGroupBox1);
            this.Controls.Add(this.grpCh1);
            this.Controls.Add(this.lampERR);
            this.Controls.Add(this.lampRUN);
            this.DrawLocation = new System.Drawing.Point(15, 15);
            this.DrawSize = new System.Drawing.Size(192, 197);
            this.Name = "MelsecSerialUnit_Draw";
            this.Size = new System.Drawing.Size(190, 195);
            this.customGroupBox1.ResumeLayout(false);
            this.grpCh1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lampRUN;
        private System.Windows.Forms.Label lampERR;
        private CustomGroupBox grpCh1;
        private System.Windows.Forms.Label lampCh1RD;
        private System.Windows.Forms.Label lampCh1SD;
        private System.Windows.Forms.Label lampCh1NEU;
        private CustomGroupBox customGroupBox1;
        private System.Windows.Forms.Label lampCh2RD;
        private System.Windows.Forms.Label lampCh2SD;
        private System.Windows.Forms.Label lampCh2NEU;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label txtUnitNum;
        private System.Windows.Forms.Label txtCh2Status;
        private System.Windows.Forms.Label txtCh1Status;
        private System.Windows.Forms.Label label13;



    }
}
