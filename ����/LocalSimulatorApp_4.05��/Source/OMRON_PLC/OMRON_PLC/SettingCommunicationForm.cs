using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OMRON.FinsGateway.Service;
using CommonClassLibrary;

namespace OMRON_PLC
{
    partial class SettingCommunicationForm : Form
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        
        public SettingCommunicationForm()
        {
            InitializeComponent();            
        }

        /// <summary>
        /// フォーム表示前初期値設定
        /// </summary>
        /// <param name="setting"></param>
        public void InitialSetting(PlcConnectionParameter setting)
        {
            //コンボボックス初期設定
            this.cmbServiceName.Items.Clear();
            this.cmbPlcType.Items.Clear();           
            this.cmbPlcType.Items.AddRange(Enum.GetNames(typeof(PlcType)));
            this.cmbServiceName.Items.AddRange(ServiceManager.GetServiceNames().ToArray());

            //初期値設定
            if (setting != null)
            {
                this.cmbPlcType.Text = setting.PlcType.ToString();
                this.cmbServiceName.Text = setting.ServiceName;
                this.txtNet.Text = setting.NetWorkNumber.ToString();
                this.txtNode.Text = setting.NodeNumber.ToString();
                this.txtUnit.Text = setting.UnitNumber.ToString();
            }
        }        

        private void SettingCommunicationForm_KeyDown(object sender, KeyEventArgs e)
        {
            // Alt + F4対策
            if (e.Alt && (e.KeyCode == Keys.F4))
            {
                e.Handled = true;
            }
        }

        private void buttonConnection_Click(object sender, EventArgs e)
        {            
            
            #region 入力チェック

            // Plc機種タイプ
            PlcType selectPlcType = (PlcType)Enum.Parse(typeof(PlcType),this.cmbPlcType.SelectedItem.ToString());

            // サービス名
            string service = ServiceManager.GetServiceNames().FirstOrDefault((s) => ( s == this.cmbServiceName.SelectedItem.ToString()) );
            
            //FinsAddress
            int netNo;
            int nodeNo;
            int unitNo;
            bool tryNet =  Int32.TryParse(this.txtNet.Text, out netNo);
            bool tryNode = Int32.TryParse(this.txtNode.Text, out nodeNo);
            bool tryUnit = Int32.TryParse(this.txtUnit.Text, out unitNo);

            if (tryNet == false || netNo < 0 || netNo > 99 ||
                tryNode == false || nodeNo < 0 || nodeNo > 255 ||
                tryUnit == false || unitNo < 0 || unitNo > 99 )
            {
                MessageBox.Show("Finsアドレスの入力が不正です。");
                return;
            }            

            #endregion

            //設定更新
            PlcConnectionParameter parameter = new PlcConnectionParameter();
            parameter.PlcType = selectPlcType.ToString();
            parameter.ServiceName = service;
            parameter.NetWorkNumber = (short)netNo;
            parameter.NodeNumber = (short)nodeNo;
            parameter.UnitNumber = (short)unitNo;

            UpDateSetting = (object[])ParameterConverter.ToConnectionSetting(parameter);
            ClickButtonType = ConnectionMode.Normal;            
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            ClickButtonType = ConnectionMode.Cancel;           
            this.Close();
        }

        public object[] UpDateSetting { get; private set; }

        public ConnectionMode ClickButtonType { get; private set; }

    }
}
