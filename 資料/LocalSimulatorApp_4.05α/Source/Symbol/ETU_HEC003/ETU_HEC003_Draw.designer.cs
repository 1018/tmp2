namespace SymbolLibrary
{
    partial class ETU_HEC003_Draw
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
            this.components = new System.ComponentModel.Container();
            this.tbControlMode = new System.Windows.Forms.TextBox();
            this.cbLabel3 = new System.Windows.Forms.Label();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.cbControlOn = new System.Windows.Forms.CheckBox();
            this.cbCooling = new System.Windows.Forms.CheckBox();
            this.cbHeating = new System.Windows.Forms.CheckBox();
            this.numNowTemperature = new System.Windows.Forms.NumericUpDown();
            this.label15 = new System.Windows.Forms.Label();
            this.cbLabel1 = new System.Windows.Forms.Label();
            this.cbLabel2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tbMinTemperature = new System.Windows.Forms.TextBox();
            this.tbMaxTemperature = new System.Windows.Forms.TextBox();
            this.tbSensorProofread = new System.Windows.Forms.TextBox();
            this.tbI_Constant = new System.Windows.Forms.TextBox();
            this.tbOffsetValue = new System.Windows.Forms.TextBox();
            this.tbPB_Width = new System.Windows.Forms.TextBox();
            this.tbD_Constant = new System.Windows.Forms.TextBox();
            this.tbTargetTemperature = new System.Windows.Forms.TextBox();
            this.label23 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.label33 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.cbNoHoming = new System.Windows.Forms.CheckBox();
            this.btLogMonitor = new System.Windows.Forms.Button();
            this.cbNoCommunication = new System.Windows.Forms.CheckBox();
            this.btAlarmOutput = new System.Windows.Forms.Button();
            this.bindThermoStatus = new System.Windows.Forms.BindingSource(this.components);
            this.groupBox8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numNowTemperature)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindThermoStatus)).BeginInit();
            this.SuspendLayout();
            // 
            // tbControlMode
            // 
            this.tbControlMode.BackColor = System.Drawing.Color.Silver;
            this.tbControlMode.Font = new System.Drawing.Font("MS UI Gothic", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.tbControlMode.Location = new System.Drawing.Point(59, 21);
            this.tbControlMode.Margin = new System.Windows.Forms.Padding(1);
            this.tbControlMode.Name = "tbControlMode";
            this.tbControlMode.ReadOnly = true;
            this.tbControlMode.Size = new System.Drawing.Size(99, 18);
            this.tbControlMode.TabIndex = 7;
            this.tbControlMode.Text = "0";
            this.tbControlMode.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // cbLabel3
            // 
            this.cbLabel3.AutoSize = true;
            this.cbLabel3.Location = new System.Drawing.Point(25, 97);
            this.cbLabel3.Name = "cbLabel3";
            this.cbLabel3.Size = new System.Drawing.Size(45, 12);
            this.cbLabel3.TabIndex = 39;
            this.cbLabel3.Text = "制御ON";
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.cbControlOn);
            this.groupBox8.Controls.Add(this.cbCooling);
            this.groupBox8.Controls.Add(this.cbHeating);
            this.groupBox8.Controls.Add(this.numNowTemperature);
            this.groupBox8.Controls.Add(this.label15);
            this.groupBox8.Controls.Add(this.cbLabel1);
            this.groupBox8.Controls.Add(this.cbLabel2);
            this.groupBox8.Controls.Add(this.cbLabel3);
            this.groupBox8.Location = new System.Drawing.Point(175, 3);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(121, 116);
            this.groupBox8.TabIndex = 28;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "ステータス";
            // 
            // cbControlOn
            // 
            this.cbControlOn.AutoSize = true;
            this.cbControlOn.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.bindThermoStatus, "ControlOn", true));
            this.cbControlOn.Location = new System.Drawing.Point(77, 97);
            this.cbControlOn.Name = "cbControlOn";
            this.cbControlOn.Size = new System.Drawing.Size(15, 14);
            this.cbControlOn.TabIndex = 46;
            this.cbControlOn.UseVisualStyleBackColor = true;
            // 
            // cbCooling
            // 
            this.cbCooling.AutoSize = true;
            this.cbCooling.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.bindThermoStatus, "Cooling", true));
            this.cbCooling.Location = new System.Drawing.Point(77, 82);
            this.cbCooling.Name = "cbCooling";
            this.cbCooling.Size = new System.Drawing.Size(15, 14);
            this.cbCooling.TabIndex = 46;
            this.cbCooling.UseVisualStyleBackColor = true;
            // 
            // cbHeating
            // 
            this.cbHeating.AutoSize = true;
            this.cbHeating.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.bindThermoStatus, "Heating", true));
            this.cbHeating.Location = new System.Drawing.Point(77, 67);
            this.cbHeating.Name = "cbHeating";
            this.cbHeating.Size = new System.Drawing.Size(15, 14);
            this.cbHeating.TabIndex = 46;
            this.cbHeating.UseVisualStyleBackColor = true;
            // 
            // numNowTemperature
            // 
            this.numNowTemperature.DecimalPlaces = 2;
            this.numNowTemperature.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numNowTemperature.Location = new System.Drawing.Point(28, 36);
            this.numNowTemperature.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            131072});
            this.numNowTemperature.Name = "numNowTemperature";
            this.numNowTemperature.Size = new System.Drawing.Size(55, 19);
            this.numNowTemperature.TabIndex = 0;
            this.numNowTemperature.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numNowTemperature.UpDownAlign = System.Windows.Forms.LeftRightAlignment.Left;
            this.numNowTemperature.ValueChanged += new System.EventHandler(this.numNowTemperature_ValueChanged);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(22, 20);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(77, 12);
            this.label15.TabIndex = 29;
            this.label15.Text = "現在温度（℃）";
            // 
            // cbLabel1
            // 
            this.cbLabel1.AutoSize = true;
            this.cbLabel1.Location = new System.Drawing.Point(30, 67);
            this.cbLabel1.Name = "cbLabel1";
            this.cbLabel1.Size = new System.Drawing.Size(41, 12);
            this.cbLabel1.TabIndex = 31;
            this.cbLabel1.Text = "加熱中";
            // 
            // cbLabel2
            // 
            this.cbLabel2.AutoSize = true;
            this.cbLabel2.Location = new System.Drawing.Point(30, 82);
            this.cbLabel2.Name = "cbLabel2";
            this.cbLabel2.Size = new System.Drawing.Size(41, 12);
            this.cbLabel2.TabIndex = 30;
            this.cbLabel2.Text = "冷却中";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.tbMinTemperature);
            this.groupBox1.Controls.Add(this.tbMaxTemperature);
            this.groupBox1.Controls.Add(this.tbSensorProofread);
            this.groupBox1.Controls.Add(this.tbI_Constant);
            this.groupBox1.Controls.Add(this.tbOffsetValue);
            this.groupBox1.Controls.Add(this.tbPB_Width);
            this.groupBox1.Controls.Add(this.tbD_Constant);
            this.groupBox1.Controls.Add(this.tbTargetTemperature);
            this.groupBox1.Controls.Add(this.tbControlMode);
            this.groupBox1.Controls.Add(this.label23);
            this.groupBox1.Controls.Add(this.label25);
            this.groupBox1.Controls.Add(this.label19);
            this.groupBox1.Controls.Add(this.label31);
            this.groupBox1.Controls.Add(this.label33);
            this.groupBox1.Controls.Add(this.label21);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(166, 209);
            this.groupBox1.TabIndex = 29;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "設定値";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(56, 44);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(53, 12);
            this.label13.TabIndex = 16;
            this.label13.Text = "目標温度";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(77, 64);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 12);
            this.label1.TabIndex = 17;
            this.label1.Text = "PB幅";
            // 
            // tbMinTemperature
            // 
            this.tbMinTemperature.BackColor = System.Drawing.Color.Silver;
            this.tbMinTemperature.Location = new System.Drawing.Point(113, 181);
            this.tbMinTemperature.Margin = new System.Windows.Forms.Padding(1);
            this.tbMinTemperature.Name = "tbMinTemperature";
            this.tbMinTemperature.ReadOnly = true;
            this.tbMinTemperature.Size = new System.Drawing.Size(45, 19);
            this.tbMinTemperature.TabIndex = 7;
            this.tbMinTemperature.Text = "0";
            this.tbMinTemperature.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // tbMaxTemperature
            // 
            this.tbMaxTemperature.BackColor = System.Drawing.Color.Silver;
            this.tbMaxTemperature.Location = new System.Drawing.Point(113, 161);
            this.tbMaxTemperature.Margin = new System.Windows.Forms.Padding(1);
            this.tbMaxTemperature.Name = "tbMaxTemperature";
            this.tbMaxTemperature.ReadOnly = true;
            this.tbMaxTemperature.Size = new System.Drawing.Size(45, 19);
            this.tbMaxTemperature.TabIndex = 7;
            this.tbMaxTemperature.Text = "0";
            this.tbMaxTemperature.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // tbSensorProofread
            // 
            this.tbSensorProofread.BackColor = System.Drawing.Color.Silver;
            this.tbSensorProofread.Location = new System.Drawing.Point(113, 141);
            this.tbSensorProofread.Margin = new System.Windows.Forms.Padding(1);
            this.tbSensorProofread.Name = "tbSensorProofread";
            this.tbSensorProofread.ReadOnly = true;
            this.tbSensorProofread.Size = new System.Drawing.Size(45, 19);
            this.tbSensorProofread.TabIndex = 7;
            this.tbSensorProofread.Text = "0";
            this.tbSensorProofread.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // tbI_Constant
            // 
            this.tbI_Constant.BackColor = System.Drawing.Color.Silver;
            this.tbI_Constant.Location = new System.Drawing.Point(113, 81);
            this.tbI_Constant.Margin = new System.Windows.Forms.Padding(1);
            this.tbI_Constant.Name = "tbI_Constant";
            this.tbI_Constant.ReadOnly = true;
            this.tbI_Constant.Size = new System.Drawing.Size(45, 19);
            this.tbI_Constant.TabIndex = 7;
            this.tbI_Constant.Text = "0";
            this.tbI_Constant.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // tbOffsetValue
            // 
            this.tbOffsetValue.BackColor = System.Drawing.Color.Silver;
            this.tbOffsetValue.Location = new System.Drawing.Point(113, 121);
            this.tbOffsetValue.Margin = new System.Windows.Forms.Padding(1);
            this.tbOffsetValue.Name = "tbOffsetValue";
            this.tbOffsetValue.ReadOnly = true;
            this.tbOffsetValue.Size = new System.Drawing.Size(45, 19);
            this.tbOffsetValue.TabIndex = 7;
            this.tbOffsetValue.Text = "0";
            this.tbOffsetValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // tbPB_Width
            // 
            this.tbPB_Width.BackColor = System.Drawing.Color.Silver;
            this.tbPB_Width.Location = new System.Drawing.Point(113, 61);
            this.tbPB_Width.Margin = new System.Windows.Forms.Padding(1);
            this.tbPB_Width.Name = "tbPB_Width";
            this.tbPB_Width.ReadOnly = true;
            this.tbPB_Width.Size = new System.Drawing.Size(45, 19);
            this.tbPB_Width.TabIndex = 7;
            this.tbPB_Width.Text = "0";
            this.tbPB_Width.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // tbD_Constant
            // 
            this.tbD_Constant.BackColor = System.Drawing.Color.Silver;
            this.tbD_Constant.Location = new System.Drawing.Point(113, 101);
            this.tbD_Constant.Margin = new System.Windows.Forms.Padding(1);
            this.tbD_Constant.Name = "tbD_Constant";
            this.tbD_Constant.ReadOnly = true;
            this.tbD_Constant.Size = new System.Drawing.Size(45, 19);
            this.tbD_Constant.TabIndex = 7;
            this.tbD_Constant.Text = "0";
            this.tbD_Constant.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // tbTargetTemperature
            // 
            this.tbTargetTemperature.BackColor = System.Drawing.Color.Silver;
            this.tbTargetTemperature.Location = new System.Drawing.Point(113, 41);
            this.tbTargetTemperature.Margin = new System.Windows.Forms.Padding(1);
            this.tbTargetTemperature.Name = "tbTargetTemperature";
            this.tbTargetTemperature.ReadOnly = true;
            this.tbTargetTemperature.Size = new System.Drawing.Size(45, 19);
            this.tbTargetTemperature.TabIndex = 7;
            this.tbTargetTemperature.Text = "0";
            this.tbTargetTemperature.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(57, 124);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(52, 12);
            this.label23.TabIndex = 20;
            this.label23.Text = "ｵﾌｾｯﾄ値";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(9, 144);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(100, 12);
            this.label25.TabIndex = 21;
            this.label25.Text = "制御用ｾﾝｻ校正値";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(77, 84);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(32, 12);
            this.label19.TabIndex = 18;
            this.label19.Text = "I定数";
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(44, 164);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(65, 12);
            this.label31.TabIndex = 22;
            this.label31.Text = "上限温度幅";
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Location = new System.Drawing.Point(44, 184);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(65, 12);
            this.label33.TabIndex = 23;
            this.label33.Text = "下限温度幅";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(72, 104);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(37, 12);
            this.label21.TabIndex = 19;
            this.label21.Text = "D定数";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(4, 24);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(53, 12);
            this.label12.TabIndex = 15;
            this.label12.Text = "制御ﾓｰﾄﾞ";
            // 
            // cbNoHoming
            // 
            this.cbNoHoming.AutoSize = true;
            this.cbNoHoming.Location = new System.Drawing.Point(186, 124);
            this.cbNoHoming.Name = "cbNoHoming";
            this.cbNoHoming.Size = new System.Drawing.Size(101, 16);
            this.cbNoHoming.TabIndex = 46;
            this.cbNoHoming.Text = "温度追尾しない";
            this.cbNoHoming.UseVisualStyleBackColor = true;
            // 
            // btLogMonitor
            // 
            this.btLogMonitor.Location = new System.Drawing.Point(175, 169);
            this.btLogMonitor.Name = "btLogMonitor";
            this.btLogMonitor.Size = new System.Drawing.Size(57, 34);
            this.btLogMonitor.TabIndex = 47;
            this.btLogMonitor.Text = "ログ表示";
            this.btLogMonitor.UseVisualStyleBackColor = true;
            this.btLogMonitor.Click += new System.EventHandler(this.btLogMonitor_Click);
            // 
            // cbNoCommunication
            // 
            this.cbNoCommunication.AutoSize = true;
            this.cbNoCommunication.Location = new System.Drawing.Point(186, 143);
            this.cbNoCommunication.Name = "cbNoCommunication";
            this.cbNoCommunication.Size = new System.Drawing.Size(72, 16);
            this.cbNoCommunication.TabIndex = 48;
            this.cbNoCommunication.Text = "通信停止";
            this.cbNoCommunication.UseVisualStyleBackColor = true;
            // 
            // btAlarmOutput
            // 
            this.btAlarmOutput.Location = new System.Drawing.Point(239, 169);
            this.btAlarmOutput.Name = "btAlarmOutput";
            this.btAlarmOutput.Size = new System.Drawing.Size(57, 34);
            this.btAlarmOutput.TabIndex = 47;
            this.btAlarmOutput.Text = "アラーム出力";
            this.btAlarmOutput.UseVisualStyleBackColor = true;
            this.btAlarmOutput.Click += new System.EventHandler(this.btAlarmOutput_Click);
            // 
            // bindThermoStatus
            // 
            this.bindThermoStatus.DataSource = typeof(SymbolLibrary.ETU_HEC003_Draw.ThermoStatus);
            // 
            // ETU_HEC003_Draw
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightGray;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.cbNoCommunication);
            this.Controls.Add(this.btAlarmOutput);
            this.Controls.Add(this.btLogMonitor);
            this.Controls.Add(this.cbNoHoming);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox8);
            this.DrawLocation = new System.Drawing.Point(15, 15);
            this.DrawSize = new System.Drawing.Size(303, 219);
            this.Name = "ETU_HEC003_Draw";
            this.Size = new System.Drawing.Size(301, 217);
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numNowTemperature)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindThermoStatus)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbControlMode;
        private System.Windows.Forms.Label cbLabel3;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.NumericUpDown numNowTemperature;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label cbLabel2;
        private System.Windows.Forms.Label cbLabel1;
        private System.Windows.Forms.CheckBox cbHeating;
        private System.Windows.Forms.CheckBox cbControlOn;
        private System.Windows.Forms.CheckBox cbCooling;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbMinTemperature;
        private System.Windows.Forms.TextBox tbMaxTemperature;
        private System.Windows.Forms.TextBox tbSensorProofread;
        private System.Windows.Forms.TextBox tbI_Constant;
        private System.Windows.Forms.TextBox tbOffsetValue;
        private System.Windows.Forms.TextBox tbPB_Width;
        private System.Windows.Forms.TextBox tbD_Constant;
        private System.Windows.Forms.TextBox tbTargetTemperature;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.CheckBox cbNoHoming;
        private System.Windows.Forms.Button btLogMonitor;
        private System.Windows.Forms.CheckBox cbNoCommunication;
        private System.Windows.Forms.Button btAlarmOutput;
        private System.Windows.Forms.BindingSource bindThermoStatus;




    }
}
