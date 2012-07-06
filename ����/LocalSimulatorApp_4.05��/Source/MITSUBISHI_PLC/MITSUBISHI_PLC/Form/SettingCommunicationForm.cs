using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using CommonClassLibrary;

namespace MITSUBISHI_PLC
{
    public partial class SettingCommunicationForm : Form
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SettingCommunicationForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// TryConnectionForm KeyDown
        /// </summary>
        /// <param name="e"></param>
        protected override void  OnKeyDown(KeyEventArgs e)
        {
            // Alt + F4 対策
            if (e.Alt && e.KeyCode == Keys.F4)
            {
                e.Handled = true;   // 処理完了
            }
            base.OnKeyDown(e);
        }         
        
        private void SettingCommunicationForm_Load(object sender, EventArgs e)
        {
            comboSelectPlc.Items.Clear();
            comboPcSideIF.Items.Clear();

            // [機種] ComboBoxに表示される全ての項目
            IEnumerable<ActCpuTypeMember> enableMembers =
                ActCpuType.GetEnumerable().Where((item) => item.IsCompletenes);

            foreach (ActCpuTypeMember type in enableMembers)
                comboSelectPlc.Items.Add(type.Name);

            // [接続先] ComboBoxに表示される全ての項目
            IEnumerable<CommunicationType> communicationTypes =
                (IEnumerable<CommunicationType>)Enum.GetValues(typeof(CommunicationType));

            foreach (CommunicationType type in communicationTypes)
                comboPcSideIF.Items.Add(NameAttribute.GetName(type));


            // 無効表示ComboBox項目
            string[] invalidItems = new string[] {
                NameAttribute.GetName(CommunicationType.Ethernet),
                NameAttribute.GetName(CommunicationType.RS_232C)//,
                //NameAttribute.GetName(CommunicationType.USB)
            };

            comboPcSideIF.InvalidityItems = invalidItems;


            // [機種] ComboBox初期選択状態
            if (!string.IsNullOrEmpty(this.SelectedPlcName) &&
                comboSelectPlc.Items.Contains(this.SelectedPlcName))
            {
                // SelectedPlcNameプロパティが設定されており、
                // かつ、SelectedPlcName文字列がコンボボックスのアイテムとして存在する

                comboSelectPlc.SelectedItem = this.SelectedPlcName;
            }
            else
            {
                comboSelectPlc.SelectedIndex = 0;
            }

            // [接続先] ComboBox初期選択状態
            if (!string.IsNullOrEmpty(this.SelectedConnectionPoint) &&
                comboPcSideIF.Items.Contains(this.SelectedConnectionPoint))
            {
                comboPcSideIF.SelectedItem = this.SelectedConnectionPoint;
            }
            else
            {
                comboPcSideIF.SelectedIndex = 0;
            }
        }

        #region Property       

        /// <summary>
        /// 選択されたPLC名を取得・設定する
        /// </summary>
        public string SelectedPlcName
        {
            get;
            set;
        }

        /// <summary>
        /// 選択された接続先を取得・設定する
        /// </summary>
        public string SelectedConnectionPoint
        {
            get;
            set;
        }
        /// <summary>
        /// 選択された号機№を取得・設定する
        /// </summary>
        public int SelectedUnitNumber
        {
            get;
            set;
        }

        #endregion

        private void buttonConnection_Click(object sender, EventArgs e)
        {
            this.SelectedPlcName = (string)comboSelectPlc.SelectedItem;
            this.SelectedConnectionPoint = (string)comboPcSideIF.SelectedItem;
            this.SelectedUnitNumber = (int)numericUpDown1.Value;
            //PlcCommunication.Instance.SettingCommunicationForm_Connection_Click(sender, e);
            ClickButtonType = ConnectionMode.Normal;
            this.Close();
        }

        private void buttonAutoConnection_Click(object sender, EventArgs e)
        {
            this.SelectedPlcName = (string)comboSelectPlc.SelectedItem;
            this.SelectedConnectionPoint = (string)comboPcSideIF.SelectedItem;
            //PlcCommunication.Instance.SettingCommunicationForm_AutoConnection_Click(sender, e);
            ClickButtonType = ConnectionMode.Auto;
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            ClickButtonType = ConnectionMode.Cancel;
            this.Close();
        }

        private void buttonDetails_Click(object sender, EventArgs e)
        {

        }

        public ConnectionMode ClickButtonType { get; private set; }
        
    }
}
