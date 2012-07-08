namespace LocalSimulator.ProjectMaker
{
    partial class SystemSettingForm
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.comboBoxMakerName = new System.Windows.Forms.ComboBox();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.comboBoxMakerName);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(151, 65);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "PLCメーカー名";
            // 
            // comboBoxMakerName
            // 
            this.comboBoxMakerName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxMakerName.FormattingEnabled = true;
            this.comboBoxMakerName.Location = new System.Drawing.Point(21, 30);
            this.comboBoxMakerName.Name = "comboBoxMakerName";
            this.comboBoxMakerName.Size = new System.Drawing.Size(110, 20);
            this.comboBoxMakerName.TabIndex = 7;
            this.comboBoxMakerName.SelectedIndexChanged += new System.EventHandler(this.comboBoxMakerName_SelectedIndexChanged);
            // 
            // SystemSettingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(175, 93);
            this.Controls.Add(this.groupBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "SystemSettingForm";
            this.Text = "設定変更";
            this.Load += new System.EventHandler(this.SystemSettingForm_Load);
            this.Activated += new System.EventHandler(this.SystemSettingForm_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SystemSettingForm_FormClosing);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox comboBoxMakerName;
    }
}