namespace MITSUBISHI_PLC
{
    partial class SettingCommunicationForm
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
            System.Windows.Forms.Label constlabel1;
            System.Windows.Forms.Label constlabel2;
            System.Windows.Forms.Label label1;
            this.comboSelectPlc = new System.Windows.Forms.ComboBox();
            this.buttonConnection = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonAutoConnection = new System.Windows.Forms.Button();
            this.buttonDetails = new System.Windows.Forms.Button();
            this.comboPcSideIF = new MITSUBISHI_PLC.EnableComboBox();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            constlabel1 = new System.Windows.Forms.Label();
            constlabel2 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.SuspendLayout();
            // 
            // constlabel1
            // 
            constlabel1.AutoSize = true;
            constlabel1.Location = new System.Drawing.Point(12, 9);
            constlabel1.Name = "constlabel1";
            constlabel1.Size = new System.Drawing.Size(35, 12);
            constlabel1.TabIndex = 1;
            constlabel1.Text = "機種：";
            // 
            // constlabel2
            // 
            constlabel2.AutoSize = true;
            constlabel2.Location = new System.Drawing.Point(12, 57);
            constlabel2.Name = "constlabel2";
            constlabel2.Size = new System.Drawing.Size(47, 12);
            constlabel2.TabIndex = 5;
            constlabel2.Text = "接続先：";
            // 
            // comboSelectPlc
            // 
            this.comboSelectPlc.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboSelectPlc.FormattingEnabled = true;
            this.comboSelectPlc.Location = new System.Drawing.Point(27, 24);
            this.comboSelectPlc.Name = "comboSelectPlc";
            this.comboSelectPlc.Size = new System.Drawing.Size(121, 20);
            this.comboSelectPlc.Sorted = true;
            this.comboSelectPlc.TabIndex = 0;
            // 
            // buttonConnection
            // 
            this.buttonConnection.Location = new System.Drawing.Point(14, 168);
            this.buttonConnection.Name = "buttonConnection";
            this.buttonConnection.Size = new System.Drawing.Size(65, 30);
            this.buttonConnection.TabIndex = 0;
            this.buttonConnection.Text = "接続";
            this.buttonConnection.UseVisualStyleBackColor = true;
            this.buttonConnection.Click += new System.EventHandler(this.buttonConnection_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(156, 168);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(65, 30);
            this.buttonCancel.TabIndex = 1;
            this.buttonCancel.Text = "キャンセル";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonAutoConnection
            // 
            this.buttonAutoConnection.Location = new System.Drawing.Point(85, 168);
            this.buttonAutoConnection.Name = "buttonAutoConnection";
            this.buttonAutoConnection.Size = new System.Drawing.Size(65, 30);
            this.buttonAutoConnection.TabIndex = 3;
            this.buttonAutoConnection.Text = "自動接続";
            this.buttonAutoConnection.UseVisualStyleBackColor = true;
            this.buttonAutoConnection.Click += new System.EventHandler(this.buttonAutoConnection_Click);
            // 
            // buttonDetails
            // 
            this.buttonDetails.Enabled = false;
            this.buttonDetails.Location = new System.Drawing.Point(154, 69);
            this.buttonDetails.Name = "buttonDetails";
            this.buttonDetails.Size = new System.Drawing.Size(61, 25);
            this.buttonDetails.TabIndex = 6;
            this.buttonDetails.Text = "詳細設定";
            this.buttonDetails.UseVisualStyleBackColor = true;
            this.buttonDetails.Click += new System.EventHandler(this.buttonDetails_Click);
            // 
            // comboPcSideIF
            // 
            this.comboPcSideIF.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.comboPcSideIF.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboPcSideIF.FormattingEnabled = true;
            this.comboPcSideIF.InvalidityIndex = new int[0];
            this.comboPcSideIF.InvalidityItems = new object[0];
            this.comboPcSideIF.Location = new System.Drawing.Point(27, 72);
            this.comboPcSideIF.Name = "comboPcSideIF";
            this.comboPcSideIF.Size = new System.Drawing.Size(121, 20);
            this.comboPcSideIF.Sorted = true;
            this.comboPcSideIF.TabIndex = 7;
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(27, 119);
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(45, 19);
            this.numericUpDown1.TabIndex = 8;
            this.numericUpDown1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numericUpDown1.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(12, 104);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(47, 12);
            label1.TabIndex = 9;
            label1.Text = "号機№：";
            // 
            // SettingCommunicationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(233, 210);
            this.ControlBox = false;
            this.Controls.Add(label1);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.comboPcSideIF);
            this.Controls.Add(this.buttonDetails);
            this.Controls.Add(constlabel2);
            this.Controls.Add(constlabel1);
            this.Controls.Add(this.buttonAutoConnection);
            this.Controls.Add(this.comboSelectPlc);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonConnection);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.Name = "SettingCommunicationForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "接続設定";
            this.Load += new System.EventHandler(this.SettingCommunicationForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.Button buttonConnection;
        internal System.Windows.Forms.Button buttonCancel;
        internal System.Windows.Forms.ComboBox comboSelectPlc;
        internal System.Windows.Forms.Button buttonAutoConnection;
        internal System.Windows.Forms.Button buttonDetails;
        internal EnableComboBox comboPcSideIF;
        private System.Windows.Forms.NumericUpDown numericUpDown1;

    }
}