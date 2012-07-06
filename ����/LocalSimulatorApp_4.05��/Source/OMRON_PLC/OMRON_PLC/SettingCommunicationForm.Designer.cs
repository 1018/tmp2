namespace OMRON_PLC
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
            this.labelConst1 = new System.Windows.Forms.Label();
            this.buttonConnection = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.cmbPlcType = new System.Windows.Forms.ComboBox();
            this.cmbServiceName = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtNet = new System.Windows.Forms.TextBox();
            this.txtNode = new System.Windows.Forms.TextBox();
            this.txtUnit = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelConst1
            // 
            this.labelConst1.AutoSize = true;
            this.labelConst1.Location = new System.Drawing.Point(51, 9);
            this.labelConst1.Name = "labelConst1";
            this.labelConst1.Size = new System.Drawing.Size(35, 12);
            this.labelConst1.TabIndex = 0;
            this.labelConst1.Text = "機種：";
            // 
            // buttonConnection
            // 
            this.buttonConnection.Location = new System.Drawing.Point(49, 169);
            this.buttonConnection.Name = "buttonConnection";
            this.buttonConnection.Size = new System.Drawing.Size(65, 30);
            this.buttonConnection.TabIndex = 4;
            this.buttonConnection.Text = "接続";
            this.buttonConnection.UseVisualStyleBackColor = true;
            this.buttonConnection.Click += new System.EventHandler(this.buttonConnection_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(120, 169);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(65, 30);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "キャンセル";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // cmbPlcType
            // 
            this.cmbPlcType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPlcType.FormattingEnabled = true;
            this.cmbPlcType.Location = new System.Drawing.Point(53, 24);
            this.cmbPlcType.Name = "cmbPlcType";
            this.cmbPlcType.Size = new System.Drawing.Size(121, 20);
            this.cmbPlcType.TabIndex = 6;
            // 
            // cmbServiceName
            // 
            this.cmbServiceName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbServiceName.FormattingEnabled = true;
            this.cmbServiceName.Location = new System.Drawing.Point(53, 72);
            this.cmbServiceName.Name = "cmbServiceName";
            this.cmbServiceName.Size = new System.Drawing.Size(121, 20);
            this.cmbServiceName.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(51, 57);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 12);
            this.label1.TabIndex = 9;
            this.label1.Text = "サービス名：";
            // 
            // txtNet
            // 
            this.txtNet.Location = new System.Drawing.Point(15, 34);
            this.txtNet.Name = "txtNet";
            this.txtNet.Size = new System.Drawing.Size(50, 19);
            this.txtNet.TabIndex = 10;
            // 
            // txtNode
            // 
            this.txtNode.Location = new System.Drawing.Point(80, 34);
            this.txtNode.Name = "txtNode";
            this.txtNode.Size = new System.Drawing.Size(50, 19);
            this.txtNode.TabIndex = 11;
            // 
            // txtUnit
            // 
            this.txtUnit.Location = new System.Drawing.Point(145, 34);
            this.txtUnit.Name = "txtUnit";
            this.txtUnit.Size = new System.Drawing.Size(50, 19);
            this.txtUnit.TabIndex = 12;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 12);
            this.label2.TabIndex = 13;
            this.label2.Text = "ネットワーク№";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(84, 19);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 12);
            this.label3.TabIndex = 14;
            this.label3.Text = "ノード№";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(146, 19);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 12);
            this.label4.TabIndex = 15;
            this.label4.Text = "ユニット№";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.txtNet);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtNode);
            this.groupBox1.Controls.Add(this.txtUnit);
            this.groupBox1.Location = new System.Drawing.Point(12, 98);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(209, 65);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Finsアドレス";
            // 
            // SettingCommunicationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(234, 219);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbServiceName);
            this.Controls.Add(this.cmbPlcType);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonConnection);
            this.Controls.Add(this.labelConst1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingCommunicationForm";
            this.Text = "接続設定";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SettingCommunicationForm_KeyDown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelConst1;
        public System.Windows.Forms.Button buttonConnection;
        public System.Windows.Forms.Button buttonCancel;
        public System.Windows.Forms.ComboBox cmbPlcType;
        public System.Windows.Forms.ComboBox cmbServiceName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtNet;
        private System.Windows.Forms.TextBox txtNode;
        private System.Windows.Forms.TextBox txtUnit;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}