namespace LocalSimulator.MainProgram
{
    partial class SetDeviceForm
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
            this.txbValue = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.rbHex = new System.Windows.Forms.RadioButton();
            this.rbDec = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // txbValue
            // 
            this.txbValue.Location = new System.Drawing.Point(12, 12);
            this.txbValue.Name = "txbValue";
            this.txbValue.Size = new System.Drawing.Size(122, 19);
            this.txbValue.TabIndex = 1;
            this.txbValue.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txbValue_KeyDown);
            // 
            // button1
            // 
            this.button1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.button1.Location = new System.Drawing.Point(140, 10);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(55, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // rbHex
            // 
            this.rbHex.AutoSize = true;
            this.rbHex.Location = new System.Drawing.Point(61, 37);
            this.rbHex.Name = "rbHex";
            this.rbHex.Size = new System.Drawing.Size(43, 16);
            this.rbHex.TabIndex = 4;
            this.rbHex.Text = "Hex";
            this.rbHex.UseVisualStyleBackColor = true;
            // 
            // rbDec
            // 
            this.rbDec.AutoSize = true;
            this.rbDec.Checked = true;
            this.rbDec.Location = new System.Drawing.Point(12, 37);
            this.rbDec.Name = "rbDec";
            this.rbDec.Size = new System.Drawing.Size(43, 16);
            this.rbDec.TabIndex = 5;
            this.rbDec.TabStop = true;
            this.rbDec.Text = "Dec";
            this.rbDec.UseVisualStyleBackColor = true;
            // 
            // SetDeviceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(207, 60);
            this.Controls.Add(this.rbDec);
            this.Controls.Add(this.rbHex);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txbValue);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.KeyPreview = true;
            this.Name = "SetDeviceForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "デバイス設定";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txbValue;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.RadioButton rbHex;
        private System.Windows.Forms.RadioButton rbDec;
    }
}