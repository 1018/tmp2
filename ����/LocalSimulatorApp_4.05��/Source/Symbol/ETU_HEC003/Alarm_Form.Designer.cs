namespace SymbolLibrary
{
    partial class Alarm_Form
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
            this.components = new System.ComponentModel.Container();
            this.cbTemperatureUnderAlarm = new System.Windows.Forms.CheckBox();
            this.cbTemperatureUpperAlarm = new System.Windows.Forms.CheckBox();
            this.cbOutSensorError = new System.Windows.Forms.CheckBox();
            this.cbFlowSwitchAlarm = new System.Windows.Forms.CheckBox();
            this.cbOutputErrorAlarm = new System.Windows.Forms.CheckBox();
            this.cbInSensorUnderError = new System.Windows.Forms.CheckBox();
            this.cbDC_Error = new System.Windows.Forms.CheckBox();
            this.cbThermoStatAlarm = new System.Windows.Forms.CheckBox();
            this.cbTankLevelAlarm = new System.Windows.Forms.CheckBox();
            this.cbAT_Error = new System.Windows.Forms.CheckBox();
            this.cbControlSensorError = new System.Windows.Forms.CheckBox();
            this.cbInSensorUpperError = new System.Windows.Forms.CheckBox();
            this.cbLabel12 = new System.Windows.Forms.Label();
            this.cbLabel8 = new System.Windows.Forms.Label();
            this.cbLabel11 = new System.Windows.Forms.Label();
            this.cbLabel10 = new System.Windows.Forms.Label();
            this.cbLabel9 = new System.Windows.Forms.Label();
            this.cbLabel6 = new System.Windows.Forms.Label();
            this.cbLabel7 = new System.Windows.Forms.Label();
            this.cbLabel5 = new System.Windows.Forms.Label();
            this.cbLabel15 = new System.Windows.Forms.Label();
            this.cbLabel4 = new System.Windows.Forms.Label();
            this.cbLabel14 = new System.Windows.Forms.Label();
            this.cbLabel13 = new System.Windows.Forms.Label();
            this.bindThermoStatus = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.bindThermoStatus)).BeginInit();
            this.SuspendLayout();
            // 
            // cbTemperatureUnderAlarm
            // 
            this.cbTemperatureUnderAlarm.AutoSize = true;
            this.cbTemperatureUnderAlarm.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.bindThermoStatus, "TemperatureUnderAlarm", true));
            this.cbTemperatureUnderAlarm.Location = new System.Drawing.Point(161, 174);
            this.cbTemperatureUnderAlarm.Name = "cbTemperatureUnderAlarm";
            this.cbTemperatureUnderAlarm.Size = new System.Drawing.Size(15, 14);
            this.cbTemperatureUnderAlarm.TabIndex = 59;
            this.cbTemperatureUnderAlarm.UseVisualStyleBackColor = true;
            // 
            // cbTemperatureUpperAlarm
            // 
            this.cbTemperatureUpperAlarm.AutoSize = true;
            this.cbTemperatureUpperAlarm.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.bindThermoStatus, "TemperatureUpperAlarm", true));
            this.cbTemperatureUpperAlarm.Location = new System.Drawing.Point(161, 159);
            this.cbTemperatureUpperAlarm.Name = "cbTemperatureUpperAlarm";
            this.cbTemperatureUpperAlarm.Size = new System.Drawing.Size(15, 14);
            this.cbTemperatureUpperAlarm.TabIndex = 63;
            this.cbTemperatureUpperAlarm.UseVisualStyleBackColor = true;
            // 
            // cbOutSensorError
            // 
            this.cbOutSensorError.AutoSize = true;
            this.cbOutSensorError.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.bindThermoStatus, "OutSensorError", true));
            this.cbOutSensorError.Location = new System.Drawing.Point(161, 144);
            this.cbOutSensorError.Name = "cbOutSensorError";
            this.cbOutSensorError.Size = new System.Drawing.Size(15, 14);
            this.cbOutSensorError.TabIndex = 64;
            this.cbOutSensorError.UseVisualStyleBackColor = true;
            // 
            // cbFlowSwitchAlarm
            // 
            this.cbFlowSwitchAlarm.AutoSize = true;
            this.cbFlowSwitchAlarm.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.bindThermoStatus, "FlowSwitchAlarm", true));
            this.cbFlowSwitchAlarm.Location = new System.Drawing.Point(161, 99);
            this.cbFlowSwitchAlarm.Name = "cbFlowSwitchAlarm";
            this.cbFlowSwitchAlarm.Size = new System.Drawing.Size(15, 14);
            this.cbFlowSwitchAlarm.TabIndex = 62;
            this.cbFlowSwitchAlarm.UseVisualStyleBackColor = true;
            // 
            // cbOutputErrorAlarm
            // 
            this.cbOutputErrorAlarm.AutoSize = true;
            this.cbOutputErrorAlarm.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.bindThermoStatus, "OutputErrorAlarm", true));
            this.cbOutputErrorAlarm.Location = new System.Drawing.Point(161, 84);
            this.cbOutputErrorAlarm.Name = "cbOutputErrorAlarm";
            this.cbOutputErrorAlarm.Size = new System.Drawing.Size(15, 14);
            this.cbOutputErrorAlarm.TabIndex = 60;
            this.cbOutputErrorAlarm.UseVisualStyleBackColor = true;
            // 
            // cbInSensorUnderError
            // 
            this.cbInSensorUnderError.AutoSize = true;
            this.cbInSensorUnderError.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.bindThermoStatus, "InSensorUnderError", true));
            this.cbInSensorUnderError.Location = new System.Drawing.Point(161, 129);
            this.cbInSensorUnderError.Name = "cbInSensorUnderError";
            this.cbInSensorUnderError.Size = new System.Drawing.Size(15, 14);
            this.cbInSensorUnderError.TabIndex = 61;
            this.cbInSensorUnderError.UseVisualStyleBackColor = true;
            // 
            // cbDC_Error
            // 
            this.cbDC_Error.AutoSize = true;
            this.cbDC_Error.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.bindThermoStatus, "DC_Error", true));
            this.cbDC_Error.Location = new System.Drawing.Point(161, 24);
            this.cbDC_Error.Name = "cbDC_Error";
            this.cbDC_Error.Size = new System.Drawing.Size(15, 14);
            this.cbDC_Error.TabIndex = 65;
            this.cbDC_Error.UseVisualStyleBackColor = true;
            // 
            // cbThermoStatAlarm
            // 
            this.cbThermoStatAlarm.AutoSize = true;
            this.cbThermoStatAlarm.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.bindThermoStatus, "ThermoStatAlarm", true));
            this.cbThermoStatAlarm.Location = new System.Drawing.Point(161, 69);
            this.cbThermoStatAlarm.Name = "cbThermoStatAlarm";
            this.cbThermoStatAlarm.Size = new System.Drawing.Size(15, 14);
            this.cbThermoStatAlarm.TabIndex = 69;
            this.cbThermoStatAlarm.UseVisualStyleBackColor = true;
            // 
            // cbTankLevelAlarm
            // 
            this.cbTankLevelAlarm.AutoSize = true;
            this.cbTankLevelAlarm.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.bindThermoStatus, "TankLevelAlarm", true));
            this.cbTankLevelAlarm.Location = new System.Drawing.Point(161, 114);
            this.cbTankLevelAlarm.Name = "cbTankLevelAlarm";
            this.cbTankLevelAlarm.Size = new System.Drawing.Size(15, 14);
            this.cbTankLevelAlarm.TabIndex = 70;
            this.cbTankLevelAlarm.UseVisualStyleBackColor = true;
            // 
            // cbAT_Error
            // 
            this.cbAT_Error.AutoSize = true;
            this.cbAT_Error.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.bindThermoStatus, "AT_Error", true));
            this.cbAT_Error.Location = new System.Drawing.Point(161, 9);
            this.cbAT_Error.Name = "cbAT_Error";
            this.cbAT_Error.Size = new System.Drawing.Size(15, 14);
            this.cbAT_Error.TabIndex = 68;
            this.cbAT_Error.UseVisualStyleBackColor = true;
            // 
            // cbControlSensorError
            // 
            this.cbControlSensorError.AutoSize = true;
            this.cbControlSensorError.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.bindThermoStatus, "ControlSensorError", true));
            this.cbControlSensorError.Location = new System.Drawing.Point(161, 54);
            this.cbControlSensorError.Name = "cbControlSensorError";
            this.cbControlSensorError.Size = new System.Drawing.Size(15, 14);
            this.cbControlSensorError.TabIndex = 66;
            this.cbControlSensorError.UseVisualStyleBackColor = true;
            // 
            // cbInSensorUpperError
            // 
            this.cbInSensorUpperError.AutoSize = true;
            this.cbInSensorUpperError.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.bindThermoStatus, "InSensorUpperError", true));
            this.cbInSensorUpperError.Location = new System.Drawing.Point(161, 39);
            this.cbInSensorUpperError.Name = "cbInSensorUpperError";
            this.cbInSensorUpperError.Size = new System.Drawing.Size(15, 14);
            this.cbInSensorUpperError.TabIndex = 67;
            this.cbInSensorUpperError.UseVisualStyleBackColor = true;
            // 
            // cbLabel12
            // 
            this.cbLabel12.AutoSize = true;
            this.cbLabel12.Location = new System.Drawing.Point(57, 130);
            this.cbLabel12.Name = "cbLabel12";
            this.cbLabel12.Size = new System.Drawing.Size(100, 12);
            this.cbLabel12.TabIndex = 58;
            this.cbLabel12.Text = "内部ｾﾝｻ異常低温";
            // 
            // cbLabel8
            // 
            this.cbLabel8.AutoSize = true;
            this.cbLabel8.Location = new System.Drawing.Point(72, 70);
            this.cbLabel8.Name = "cbLabel8";
            this.cbLabel8.Size = new System.Drawing.Size(84, 12);
            this.cbLabel8.TabIndex = 47;
            this.cbLabel8.Text = "ｻｰﾓｽﾀｯﾄｱﾗｰﾑ";
            // 
            // cbLabel11
            // 
            this.cbLabel11.AutoSize = true;
            this.cbLabel11.Location = new System.Drawing.Point(52, 114);
            this.cbLabel11.Name = "cbLabel11";
            this.cbLabel11.Size = new System.Drawing.Size(103, 12);
            this.cbLabel11.TabIndex = 57;
            this.cbLabel11.Text = "ﾀﾝｸﾚﾍﾞﾙ低下ｱﾗｰﾑ";
            // 
            // cbLabel10
            // 
            this.cbLabel10.AutoSize = true;
            this.cbLabel10.Location = new System.Drawing.Point(70, 99);
            this.cbLabel10.Name = "cbLabel10";
            this.cbLabel10.Size = new System.Drawing.Size(85, 12);
            this.cbLabel10.TabIndex = 49;
            this.cbLabel10.Text = "ﾌﾛｰｽｲｯﾁｱﾗｰﾑ";
            // 
            // cbLabel9
            // 
            this.cbLabel9.AutoSize = true;
            this.cbLabel9.Location = new System.Drawing.Point(72, 85);
            this.cbLabel9.Name = "cbLabel9";
            this.cbLabel9.Size = new System.Drawing.Size(83, 12);
            this.cbLabel9.TabIndex = 56;
            this.cbLabel9.Text = "出力異常ｱﾗｰﾑ";
            // 
            // cbLabel6
            // 
            this.cbLabel6.AutoSize = true;
            this.cbLabel6.Location = new System.Drawing.Point(52, 40);
            this.cbLabel6.Name = "cbLabel6";
            this.cbLabel6.Size = new System.Drawing.Size(106, 12);
            this.cbLabel6.TabIndex = 55;
            this.cbLabel6.Text = "内部センサ異常高温";
            // 
            // cbLabel7
            // 
            this.cbLabel7.AutoSize = true;
            this.cbLabel7.Location = new System.Drawing.Point(6, 55);
            this.cbLabel7.Name = "cbLabel7";
            this.cbLabel7.Size = new System.Drawing.Size(150, 12);
            this.cbLabel7.TabIndex = 48;
            this.cbLabel7.Text = "制御用ｾﾝｻ断線、短絡ｱﾗｰﾑ";
            // 
            // cbLabel5
            // 
            this.cbLabel5.AutoSize = true;
            this.cbLabel5.Location = new System.Drawing.Point(86, 25);
            this.cbLabel5.Name = "cbLabel5";
            this.cbLabel5.Size = new System.Drawing.Size(69, 12);
            this.cbLabel5.TabIndex = 54;
            this.cbLabel5.Text = "DC電源異常";
            // 
            // cbLabel15
            // 
            this.cbLabel15.AutoSize = true;
            this.cbLabel15.Location = new System.Drawing.Point(72, 175);
            this.cbLabel15.Name = "cbLabel15";
            this.cbLabel15.Size = new System.Drawing.Size(83, 12);
            this.cbLabel15.TabIndex = 50;
            this.cbLabel15.Text = "温度下限ｱﾗｰﾑ";
            // 
            // cbLabel4
            // 
            this.cbLabel4.AutoSize = true;
            this.cbLabel4.Location = new System.Drawing.Point(110, 9);
            this.cbLabel4.Name = "cbLabel4";
            this.cbLabel4.Size = new System.Drawing.Size(44, 12);
            this.cbLabel4.TabIndex = 53;
            this.cbLabel4.Text = "AT異常";
            // 
            // cbLabel14
            // 
            this.cbLabel14.AutoSize = true;
            this.cbLabel14.Location = new System.Drawing.Point(72, 160);
            this.cbLabel14.Name = "cbLabel14";
            this.cbLabel14.Size = new System.Drawing.Size(83, 12);
            this.cbLabel14.TabIndex = 51;
            this.cbLabel14.Text = "温度上限ｱﾗｰﾑ";
            // 
            // cbLabel13
            // 
            this.cbLabel13.AutoSize = true;
            this.cbLabel13.Location = new System.Drawing.Point(17, 144);
            this.cbLabel13.Name = "cbLabel13";
            this.cbLabel13.Size = new System.Drawing.Size(138, 12);
            this.cbLabel13.TabIndex = 52;
            this.cbLabel13.Text = "外部ｾﾝｻ断線、短絡ｱﾗｰﾑ";
            // 
            // bindThermoStatus
            // 
            this.bindThermoStatus.DataSource = typeof(SymbolLibrary.ETU_HEC003_Draw.ThermoStatus);
            // 
            // Alarm_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(195, 202);
            this.Controls.Add(this.cbTemperatureUnderAlarm);
            this.Controls.Add(this.cbTemperatureUpperAlarm);
            this.Controls.Add(this.cbOutSensorError);
            this.Controls.Add(this.cbFlowSwitchAlarm);
            this.Controls.Add(this.cbOutputErrorAlarm);
            this.Controls.Add(this.cbInSensorUnderError);
            this.Controls.Add(this.cbDC_Error);
            this.Controls.Add(this.cbThermoStatAlarm);
            this.Controls.Add(this.cbTankLevelAlarm);
            this.Controls.Add(this.cbAT_Error);
            this.Controls.Add(this.cbControlSensorError);
            this.Controls.Add(this.cbInSensorUpperError);
            this.Controls.Add(this.cbLabel12);
            this.Controls.Add(this.cbLabel8);
            this.Controls.Add(this.cbLabel11);
            this.Controls.Add(this.cbLabel10);
            this.Controls.Add(this.cbLabel9);
            this.Controls.Add(this.cbLabel6);
            this.Controls.Add(this.cbLabel7);
            this.Controls.Add(this.cbLabel5);
            this.Controls.Add(this.cbLabel15);
            this.Controls.Add(this.cbLabel4);
            this.Controls.Add(this.cbLabel14);
            this.Controls.Add(this.cbLabel13);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "Alarm_Form";
            this.Text = "アラーム出力";
            ((System.ComponentModel.ISupportInitialize)(this.bindThermoStatus)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label cbLabel12;
        private System.Windows.Forms.Label cbLabel8;
        private System.Windows.Forms.Label cbLabel11;
        private System.Windows.Forms.Label cbLabel10;
        private System.Windows.Forms.Label cbLabel9;
        private System.Windows.Forms.Label cbLabel6;
        private System.Windows.Forms.Label cbLabel7;
        private System.Windows.Forms.Label cbLabel5;
        private System.Windows.Forms.Label cbLabel15;
        private System.Windows.Forms.Label cbLabel4;
        private System.Windows.Forms.Label cbLabel14;
        private System.Windows.Forms.Label cbLabel13;
        public System.Windows.Forms.CheckBox cbAT_Error;
        public System.Windows.Forms.CheckBox cbFlowSwitchAlarm;
        public System.Windows.Forms.CheckBox cbOutputErrorAlarm;
        public System.Windows.Forms.CheckBox cbDC_Error;
        public System.Windows.Forms.CheckBox cbThermoStatAlarm;
        public System.Windows.Forms.CheckBox cbTankLevelAlarm;
        public System.Windows.Forms.CheckBox cbControlSensorError;
        public System.Windows.Forms.CheckBox cbInSensorUpperError;
        public System.Windows.Forms.CheckBox cbTemperatureUnderAlarm;
        public System.Windows.Forms.CheckBox cbTemperatureUpperAlarm;
        public System.Windows.Forms.CheckBox cbOutSensorError;
        public System.Windows.Forms.CheckBox cbInSensorUnderError;
        internal System.Windows.Forms.BindingSource bindThermoStatus;
    }
}