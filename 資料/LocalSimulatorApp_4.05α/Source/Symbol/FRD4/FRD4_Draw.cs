using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CommonClassLibrary;
using System.Diagnostics;

namespace SymbolLibrary
{
    using SplitterDictionary = Dictionary<string, Action<byte[]>>;

    public partial class FRD4_Draw : Symbol_Draw
    {
        #region Constructor

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FRD4_Draw()
        {
            InitializeComponent();
            containTrackingBox();

            base.CyclicFlag = true;

            base.SymbolType = "マルチコントローラ(FRD-4)";
            base.Category = "温調器";

            #region 初期化

            this.RecvMsgDevice = new DeviceFormat("KM2000", 512, IoType.In, SetType.Word);
            this.RecvNumDevice = new DeviceFormat("KM2600", 1, IoType.In, SetType.Word);
            this.RecvFlagDevice = new DeviceFormat("KM2601.1", 1, IoType.In, SetType.Bit);
            this.SendMsgDevice = new DeviceFormat("KM0", 512, IoType.Io, SetType.Word);
            this.SendNumDevice = new DeviceFormat("KM600", 1, IoType.Io, SetType.Word);
            this.SendFlagDevice = new DeviceFormat("KM601.1", 1, IoType.Io, SetType.Bit);
            this.NodeNumber = 1;
            this.NowTemperature = new DeviceFormat("KM10000", 1, IoType.Out, SetType.Word);
               
            this._IsByteCount = true;

            #endregion
        }

        #endregion
        const int MAX_LOG = 100;
        const string LOG_TEXT_COLUMN_NAME = "LoggingText";

        bool _IsByteCount;

        Queue<byte[]> _SendQueue = new Queue<byte[]>();

        DataTable _LogMessage = new DataTable();

        Log_Form _LogForm = null;
 
        double _nowTemperature;

        ThermoSetting[] _SettingArray = new[] {
            new ThermoSetting(), new ThermoSetting(),
            new ThermoSetting(), new ThermoSetting() 
        };

        ThermoStatus[] _StatusArray = new[] {
            new ThermoStatus(), new ThermoStatus(),
            new ThermoStatus(), new ThermoStatus()
        };
        CheckBox[] trackingBox;
        NumericUpDown[] nowTemp;
        CheckBox[] ccflg0Box;
        CheckBox[] ccflg1Box;
        CheckBox[] ccflg2Box;
        CheckBox[] ccflg3Box;
        CheckBox[] ccflg4Box;
        CheckBox[] ccflg5Box; 
        CheckBox[] ccflg6Box;

        private void containTrackingBox()
        {
            // 温度追尾しない
            this.trackingBox = new CheckBox[4];
            this.trackingBox[0] = this.noTracking01;
            this.trackingBox[1] = this.noTracking02;
            this.trackingBox[2] = this.noTracking03;
            this.trackingBox[3] = this.noTracking04;

            // 現在温度
            this.nowTemp = new NumericUpDown[4];
            this.nowTemp[0] = this.measuredTemp01;
            this.nowTemp[1] = this.measuredTemp02;
            this.nowTemp[2] = this.measuredTemp03;
            this.nowTemp[3] = this.measuredTemp04;


            // ccflg0:ｻｰﾓﾓｼﾞｭｰﾙ電源電圧上昇、低下
            this.ccflg0Box = new CheckBox[4];
            this.ccflg0Box[0] = this.ccflg1_0;
            this.ccflg0Box[1] = this.ccflg2_0;
            this.ccflg0Box[2] = this.ccflg3_0;
            this.ccflg0Box[3] = this.ccflg4_0;          

            // ccflg1:温度ｾﾝｻ過昇温、過冷却
            this.ccflg1Box = new CheckBox[4];
            this.ccflg1Box[0] = this.ccflg1_1;
            this.ccflg1Box[1] = this.ccflg2_1;
            this.ccflg1Box[2] = this.ccflg3_1;
            this.ccflg1Box[3] = this.ccflg4_1;

            // ccflg2:温度ｾﾝｻ断線、短絡
            this.ccflg2Box = new CheckBox[4];
            this.ccflg2Box[0] = this.ccflg1_2;
            this.ccflg2Box[1] = this.ccflg2_2;
            this.ccflg2Box[2] = this.ccflg3_2;
            this.ccflg2Box[3] = this.ccflg4_2;

            // ccflg3:ｻｰﾓｽﾀｯﾄｱﾗｰﾑ
            this.ccflg3Box = new CheckBox[4];
            this.ccflg3Box[0] = this.ccflg1_3;
            this.ccflg3Box[1] = this.ccflg2_3;
            this.ccflg3Box[2] = this.ccflg3_3;
            this.ccflg3Box[3] = this.ccflg4_3;

            // ccflg4:ｻｰﾓﾓｼﾞｭｰﾙ電源異常
            this.ccflg4Box = new CheckBox[4];
            this.ccflg4Box[0] = this.ccflg1_4;
            this.ccflg4Box[1] = this.ccflg2_4;
            this.ccflg4Box[2] = this.ccflg3_4;
            this.ccflg4Box[3] = this.ccflg4_4;

            // ccflg5:温度上限
            this.ccflg5Box = new CheckBox[4];
            this.ccflg5Box[0] = this.ccflg1_5;
            this.ccflg5Box[1] = this.ccflg2_5;
            this.ccflg5Box[2] = this.ccflg3_5;
            this.ccflg5Box[3] = this.ccflg4_5;

            // ccflg6:温度下限
            this.ccflg6Box = new CheckBox[4];
            this.ccflg6Box[0] = this.ccflg1_6;
            this.ccflg6Box[1] = this.ccflg2_6;
            this.ccflg6Box[2] = this.ccflg3_6;
            this.ccflg6Box[3] = this.ccflg4_6;

        }

        SplitterDictionary _SplitterDictionary = null;


        private void InitSplitterDictionary()
        {
            if (_SplitterDictionary == null)
            {
                _SplitterDictionary = new Dictionary<string, Action<byte[]>>();

                _SplitterDictionary.Add("WM", (msg) => SplitWMMsg(msg));
                _SplitterDictionary.Add("RM", (msg) => SplitRMMsg(msg));
                _SplitterDictionary.Add("WS", (msg) => SplitWSMsg(msg));
                _SplitterDictionary.Add("RS", (msg) => SplitRSMsg(msg));
                _SplitterDictionary.Add("RX", (msg) => SplitRXMsg(msg));
                _SplitterDictionary.Add("WB", (msg) => SplitWBMsg(msg));
                _SplitterDictionary.Add("RB", (msg) => SplitRBMsg(msg));
                _SplitterDictionary.Add("W%", (msg) => SplitWPerMsg(msg));
                _SplitterDictionary.Add("R%", (msg) => SplitRPerMsg(msg));
                _SplitterDictionary.Add("WP", (msg) => SplitWPMsg(msg));
                _SplitterDictionary.Add("RP", (msg) => SplitRPMsg(msg));
                _SplitterDictionary.Add("RR", (msg) => SplitRRMsg(msg));
            }
        }

        private bool CallSplitter(string command, byte[] recvMsg)
        {
            if (_SplitterDictionary.ContainsKey(command))
            {
                _SplitterDictionary[command].Invoke(recvMsg);
                return true;
            }
            else
            {
                return false;
            }
        }


        #region フォームイベント

        /// <summary>
        /// [現在温度](アップダウン) ValueChangedイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void measuredTemp01_ValueChanged(object sender, EventArgs e)
        {
            _StatusArray[0].measuredTemparature = (double)measuredTemp01.Value;

        }

        private void measuredTemp02_ValueChanged(object sender, EventArgs e)
        {
            _StatusArray[1].measuredTemparature = (double)measuredTemp02.Value;

        }

        private void measuredTemp03_ValueChanged(object sender, EventArgs e)
        {
            _StatusArray[2].measuredTemparature = (double)measuredTemp03.Value;
        }

        private void measuredTemp04_ValueChanged(object sender, EventArgs e)
        {
            _StatusArray[3].measuredTemparature = (double)measuredTemp04.Value;
        }


        /// <summary>
        /// [ログ表示] Clickイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btLogMonitor_Click(object sender, EventArgs e)
        {
            btLogMonitor.Enabled = false;

            _LogForm = new Log_Form();
            _LogForm.Text = string.Format("ログ - {0} -", base.SymbolName);
            _LogForm.lbLog.DataSource = _LogMessage;
            _LogForm.lbLog.DisplayMember = LOG_TEXT_COLUMN_NAME;
            _LogForm.Disposed += new EventHandler(LogForm_Disposed);

            _LogForm.Show();

        }
        /// <summary>
        /// ログウィンドウ Disposedイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LogForm_Disposed(object sender, EventArgs e)
        {
            _LogForm = null;
            btLogMonitor.Enabled = true;
        }

        #endregion


        /// <summary>
        /// Symbol_Draw.Initial オーバーライド
        /// </summary>
        public override void Initial()
        {
            InitSplitterDictionary();

//            bindThermoStatus.DataSource = _Status;

            if (!_LogMessage.Columns.Contains(LOG_TEXT_COLUMN_NAME))
            {
                _LogMessage.Columns.Add(LOG_TEXT_COLUMN_NAME);
            }

            updateDisplay();
        }

        /// <summary>
        /// Symbol_Draw.CyclicMethod オーバーライド
        /// </summary>
        public override void CyclicMethod()
        {
            // メッセージ受信有りチェック
            if (RecvFlagDevice.Value[0] != 0)
            {
                RecvMsgProc();
            }

            // 送信メッセージ有りチェック
            if (!EmptyQueue())
            {
                // 前回の送信が完了していれば、メッセージを送信する
                if (SendFlagDevice.Value[0] == 0)
                {
                    byte[] sendMsg = Dequeue();
                    SetSendMsg(sendMsg);

                    AddSendLog(sendMsg);

                    // 送信フラグON
                    SetDeviceData(this.SendFlagDevice.Address, 1);
                    this.SendFlagDevice.Value[0] = 1;
                }
            }
        }

        /// <summary>
        /// Symbol_Draw.Abort オーバーライド
        /// </summary>
        protected override void Abort()
        {
            if (_LogForm != null)
            {
                _LogForm.Close();
            }
        }

        private void updateDisplay()
        {
            Action action = new Action(
                () =>
                {
                    //設定データを表示
                    // CH1
                    controlMode01.Text = GetControlModeText(_SettingArray[0].ControlMode);
                    targetTemp01.Text = _SettingArray[0].TargetTemperature.ToString("0.00");
                    PB_Width01.Text = _SettingArray[0].PB_Width.ToString("0.00");
                    I_Constant01.Text = _SettingArray[0].I_Constant.ToString("0");
                    D_Constant01.Text = _SettingArray[0].D_Constant.ToString("0");
                    offsetValue01.Text = _SettingArray[0].OffsetValue.ToString("0.00");
                    sensorProofread01.Text = _SettingArray[0].SensorProofread.ToString("0.00");
                    maxTemp01.Text = _SettingArray[0].MaxTemperature.ToString("0.0");
                    minTemp01.Text = _SettingArray[0].MinTemperature.ToString("0.0");
                    pbConst01.Text = _SettingArray[0].PbConst.ToString("0.00");

                    // CH2
                    controlMode02.Text = GetControlModeText(_SettingArray[1].ControlMode);
                    targetTemp02.Text = _SettingArray[1].TargetTemperature.ToString("0.00");
                    PB_Width02.Text = _SettingArray[1].PB_Width.ToString("0.00");
                    I_Constant02.Text = _SettingArray[1].I_Constant.ToString("0");
                    D_Constant02.Text = _SettingArray[1].D_Constant.ToString("0");
                    offsetValue02.Text = _SettingArray[1].OffsetValue.ToString("0.00");
                    sensorProofread02.Text = _SettingArray[1].SensorProofread.ToString("0.00");
                    maxTemp02.Text = _SettingArray[1].MaxTemperature.ToString("0.0");
                    minTemp02.Text = _SettingArray[1].MinTemperature.ToString("0.0");
                    pbConst02.Text = _SettingArray[1].PbConst.ToString("0.00");

                    // CH3
                    controlMode03.Text = GetControlModeText(_SettingArray[2].ControlMode);
                    targetTemp03.Text = _SettingArray[2].TargetTemperature.ToString("0.00");
                    PB_Width03.Text = _SettingArray[2].PB_Width.ToString("0.00");
                    I_Constant03.Text = _SettingArray[2].I_Constant.ToString("0");
                    D_Constant03.Text = _SettingArray[2].D_Constant.ToString("0");
                    offsetValue03.Text = _SettingArray[2].OffsetValue.ToString("0.00");
                    sensorProofread03.Text = _SettingArray[2].SensorProofread.ToString("0.00");
                    maxTemp03.Text = _SettingArray[2].MaxTemperature.ToString("0.0");
                    minTemp03.Text = _SettingArray[2].MinTemperature.ToString("0.0");
                    pbConst03.Text = _SettingArray[2].PbConst.ToString("0.00");

                    // CH1
                    controlMode04.Text = GetControlModeText(_SettingArray[3].ControlMode);
                    targetTemp04.Text = _SettingArray[3].TargetTemperature.ToString("0.00");
                    PB_Width04.Text = _SettingArray[3].PB_Width.ToString("0.00");
                    I_Constant04.Text = _SettingArray[3].I_Constant.ToString("0");
                    D_Constant04.Text = _SettingArray[3].D_Constant.ToString("0");
                    offsetValue04.Text = _SettingArray[3].OffsetValue.ToString("0.00");
                    sensorProofread04.Text = _SettingArray[3].SensorProofread.ToString("0.00");
                    maxTemp04.Text = _SettingArray[3].MaxTemperature.ToString("0.0");
                    minTemp04.Text = _SettingArray[3].MinTemperature.ToString("0.0");
                    pbConst04.Text = _SettingArray[3].PbConst.ToString("0.00");

                    // ステータスを表示
                    measuredTemp01.Value = (decimal)_StatusArray[0].measuredTemparature;
                    measuredTemp02.Value = (decimal)_StatusArray[1].measuredTemparature;
                    measuredTemp03.Value = (decimal)_StatusArray[2].measuredTemparature;
                    measuredTemp04.Value = (decimal)_StatusArray[3].measuredTemparature;
                });

            if (this.InvokeRequired)
            {
                this.Invoke(action);
            }
            else
            {
                action();
            }
        }
        #region 通信処理

        private byte[] Dequeue()
        {
            return _SendQueue.Dequeue();
        }

        private void Enqueue(byte[] msg)
        {
            _SendQueue.Enqueue(msg);
        }

        private bool EmptyQueue()
        {
            return (_SendQueue.Count == 0);
        }

        private byte[] GetMsgData(byte[] msg)
        {
            //int dataLen = msg[1 + 3];
            //return msg.Skip(1 + 5).Take(dataLen).ToArray();
            return msg.Skip(1).Take(msg.Length - 5).ToArray();
        }

        private bool getAckPosition(byte[] msg)
        {
            return (msg[2] == 0x06);

        }

        private byte[] getMsgCommand(byte[] msg)
        {
            return msg.Skip(2).Take(2).ToArray();
        }

        private int GetMsgNumber(byte[] msg)
        {
            int msgNumber = msg[1 + 4];

            return msgNumber;
        }

        private void RecvMsgProc()
        {
            // メッセージ受信
            byte[] recvMsg = GetRecvMsg();

            // 自ノードへのメッセージで無い
            if (!IsMyAddressMsg(recvMsg))
            {
                return;
            }

            try
            {
                // 無効なメッセージ
                if (!CheckEnableMsg(recvMsg))
                {
                    return;
                }

                // ACK返信
//                SendAck(recvMsg);
                // エコーバック返信
               sendEchoBack(recvMsg);

                // メッセージコマンド
                string command = AsciiUtils.ToString(getMsgCommand(recvMsg), 0, 2);

                // コマンド展開処理
                if (CallSplitter(command, recvMsg))
                {
                    AddRecvLog(recvMsg);

                    // 表示の更新
                    updateDisplay();
                }
                else
                {
                    AddLog("不明なコマンド", recvMsg);
                }
            }
            finally
            {
                // 受信フラグOFF
                SetDeviceData(this.RecvFlagDevice.Address, 0);
                this.RecvFlagDevice.Value[0] = 0;
            }
        }
        //WM:制御モードの設定
        private void SplitWMMsg(byte[] recvMsg)
        {
            byte[] data = GetMsgData(recvMsg);

//            _Setting.ControlMode = (ControlMode)dataArea[2];

            for (int i = 0; i < 4; i++)
            {
                _SettingArray[i].ControlMode = (ControlMode)data[2 + i];
            }

            List<byte> sndData = new List<byte>();
            sndData.Add(0x06);
            byte[] sendMsg = SetHeader(sndData);
            Enqueue(sendMsg);
        }

        //RM:制御モードの読み出し
        private void SplitRMMsg(byte[] recvMsg)
        {
            if (!this.stopCommunication.Checked)
            {
                List<byte> data = new List<byte>();                
                data.AddRange(AsciiUtils.ToAscii("RM"));

                for (int i = 0; i < 4; i++)
                {
                    data.AddRange(BCDUtils.ToBCD((byte)_SettingArray[i].ControlMode, 1));   
                }

                byte[] sendMsg = SetHeader(data);
                Enqueue(sendMsg);
            }
        }

        //WS:目標温度の設定
        private void SplitWSMsg(byte[] recvMsg)
        {
            byte[] data = GetMsgData(recvMsg);

            double testResult;
            for (int i = 0; i < 4; i++)
            {
                if (double.TryParse(AsciiUtils.ToString(data, 3 + (i * 3), 3), out testResult))
                {
                    _SettingArray[i].TargetTemperature = testResult / 10;
                }


                //現在温度を目標温度に
                if (!trackingBox[i].Checked)
                {
                    _StatusArray[i].measuredTemparature = _SettingArray[i].TargetTemperature;
                }
            }

            List<byte> sndData = new List<byte>();
            sndData.Add(0x06);
            byte[] sendMsg = SetHeader(sndData);
            Enqueue(sendMsg);
        }

        //RS:目標温度の読み出し
        private void SplitRSMsg(byte[] recvMsg)
        {
            if (!this.stopCommunication.Checked)
            {
                List<byte> data = new List<byte>();
                data.AddRange(AsciiUtils.ToAscii("RS"));
                for(int i = 0; i < 4; i++)
                {
                    int targetTemp = (int)Math.Round(_SettingArray[i].TargetTemperature * 10);
                    data.AddRange(AsciiUtils.ToAscii(targetTemp, 3));   // 目標温度
                }

                byte[] sendMsg = SetHeader(data);
                Enqueue(sendMsg);
            }
        }

        //RX:内部センサ及び外部センサ測定温度の読み出し
        private void SplitRXMsg(byte[] recvMsg)
        {
            if (!this.stopCommunication.Checked)
            {
                List<byte> data = new List<byte>();
                data.AddRange(AsciiUtils.ToAscii("RX"));
                for (int i = 0; i < 4; i++)
                {
                    int nowTemp = (int)Math.Round(_StatusArray[i].measuredTemparature * 10);                    
                    data.AddRange(AsciiUtils.ToAscii(nowTemp, 3));    // 現在温度                    
                    data.Add(0x46);                             // 外部センサなし
                    data.Add(0x46);                             // 外部センサなし
                    data.Add(0x46);                             // 外部センサなし
                }

                byte[] sendMsg = SetHeader(data);
                Enqueue(sendMsg);
            }
        }

        //WB:PID定数及びオフセット値の設定
        private void SplitWBMsg(byte[] recvMsg)
        {
            byte[] data = GetMsgData(recvMsg);

            double testResult;
            for (int i = 0; i < 4; i++)
            { 
                if (double.TryParse(AsciiUtils.ToString(data, 3 + (i * 13), 3), out testResult))
                {
                    _SettingArray[i].PB_Width = testResult / 100;
                }
                
                if(double.TryParse(AsciiUtils.ToString(data, 6 + ( i * 13), 3), out testResult))
                {
                    _SettingArray[i].I_Constant = (int)testResult;
                }

                if (double.TryParse(AsciiUtils.ToString(data, 9 + (i * 13), 3), out testResult))
                {
                    _SettingArray[i].D_Constant = (int)testResult;
                }                
              
                if (double.TryParse(AsciiUtils.ToString(data, 12 + (i * 13), 4), out testResult))
                {
                    _SettingArray[i].OffsetValue = testResult / 100;
                }
            }

            List<byte> sndData = new List<byte>();
            sndData.Add(0x06);
            byte[] sendMsg = SetHeader(sndData);
            Enqueue(sendMsg);
        }

        //RB:PID定数及びオフセット値の読み出し
        private void SplitRBMsg(byte[] recvMsg)
        {
            if (!this.stopCommunication.Checked)
            {
                List<byte> data = new List<byte>();
                data.AddRange(AsciiUtils.ToAscii("RB"));  
                for (int i = 0; i < 4; i++)
                {
                    int pbValue = (int)Math.Round(_SettingArray[i].PB_Width * 100);
                    int iValue = _SettingArray[i].I_Constant;
                    int dValue = _SettingArray[i].D_Constant;
                    int offset = (int)Math.Round(_SettingArray[i].OffsetValue * 100);
                    data.AddRange(BCDUtils.ToBCD(pbValue, 3));
                    data.AddRange(BCDUtils.ToBCD(iValue, 3));
                    data.AddRange(BCDUtils.ToBCD(dValue, 3));
                    data.AddRange(BCDUtils.ToBCD(offset, 4, true));

                }

                byte[] sendMsg = SetHeader(data);
                Enqueue(sendMsg);
            }
        }  

        //W%:温度上下限温度の設定
        private void SplitWPerMsg(byte[] recvMsg)
        {
            byte[] data = GetMsgData(recvMsg);

            double testResult;
            for (int i = 0; i < 4; i++)
            {
                if(double.TryParse(AsciiUtils.ToString(data, 3 + (i * 6), 3), out testResult))
                {
                    _SettingArray[i].MaxTemperature = testResult / 10;
                }

                if (double.TryParse(AsciiUtils.ToString(data, 6 + (i * 6), 3), out testResult))
                {
                    _SettingArray[i].MinTemperature = testResult / 10;
                }
            }

            List<byte> sndData = new List<byte>();
            sndData.Add(0x06);
            byte[] sendMsg = SetHeader(sndData);
            Enqueue(sendMsg);
        }

        //R%:温度上下限温度の読み出し
        private void SplitRPerMsg(byte[] recvMsg)
        {
            if (!this.stopCommunication.Checked)
            {
                List<byte> data = new List<byte>();
                data.AddRange(AsciiUtils.ToAscii("R%"));
                for (int i = 0; i < 4; i++)
                {
                    int max = (int)Math.Round(_SettingArray[i].MaxTemperature);
                    int min = (int)Math.Round(_SettingArray[i].MinTemperature);
                    data.AddRange(AsciiUtils.ToAscii(max, 3));
                    data.AddRange(AsciiUtils.ToAscii(min, 3));
                }

                byte[] sendMsg = SetHeader(data);
                Enqueue(sendMsg);
            }
        }

        // WP:Pb(演算開始定数)の設定
        private void SplitWPMsg(byte[] recvMsg)
        {
            byte[] data = GetMsgData(recvMsg);

            double testResult;
            for (int i = 0; i < 4; i++)
            {
                if (double.TryParse(AsciiUtils.ToString(data, 3 + (i * 3), 3), out testResult))                    
                {
                    _SettingArray[i].PbConst = testResult;
                }
            }
            List<byte> sndData = new List<byte>();
            sndData.Add(0x06);
            byte[] sendMsg = SetHeader(sndData);
            Enqueue(sendMsg);
        }

        // RP:演算開始定数の読出し
        private void SplitRPMsg(byte[] recvMsg)
        {
            if(!this.stopCommunication.Checked)
            {
                List<byte> data = new List<byte>();
                data.AddRange(AsciiUtils.ToAscii("RP"));

                for(int i = 0; i < 4; i++)
                {
                    int tmp = (int)_SettingArray[i].PbConst;
                    data.AddRange(AsciiUtils.ToAscii(tmp, 3));
                }
                byte[] sendMsg = SetHeader(data);
                Enqueue(sendMsg);
            }
        }

        //RR:ステータスの読み出し
        private void SplitRRMsg(byte[] recvMsg)
        {
            if (!this.stopCommunication.Checked)
            {
                List<byte> data = new List<byte>();
                data.AddRange(AsciiUtils.ToAscii("RR"));

                for (int i = 0; i < 4; i++)
                {                    
                    byte status = 0;
                    data.Add(0x00);
                    if (ccflg3Box[i].Checked)   // ｻｰﾓｽﾀｯﾄｱﾗｰﾑ
                    {
                        status = 1 << 0;
                    }
                    if (ccflg4Box[i].Checked)   // 温度ｾﾝｻ断線、短絡
                    {
                        status += 1 << 1;
                    }
                    if (ccflg5Box[i].Checked)   // 温度ｾﾝｻ過昇温、過冷却
                    {
                        status += 1 << 2;
                    }
                    if (ccflg6Box[i].Checked)   // ｻｰﾓﾓｼﾞｭｰﾙ電源電圧上昇、低下
                    {
                        status += 1 << 3;
                    }
                    data.AddRange(AsciiUtils.ToAscii(status, 1, true));

                    if(ccflg2Box[i].Checked)    // ｻｰﾓﾓｼﾞｭｰﾙ電源異常
                    {
                        status = 1 << 3;
                    }
                    data.AddRange(AsciiUtils.ToAscii(status, 1, true));

                    if (ccflg0Box[i].Checked)   // 温度下限
                    {
                        status = 1 << 0;
                    }
                    if (ccflg1Box[i].Checked)   // 温度上限
                    {
                        status += 1 << 1;
                    }
                    data.AddRange(AsciiUtils.ToAscii(status, 1, true));
                }

                
                byte[] sendMsg = SetHeader(data);
                Enqueue(sendMsg);
            }
        }

        private byte[] GetRecvMsg()
        {
            // ﾃﾞｰﾀ数
            int dataCnt = this.RecvNumDevice.Value[0];

            int byteCnt;
            int wordCnt;

            // ﾃﾞｰﾀ数 = ﾊﾞｲﾄ数
            if (this._IsByteCount)
            {
                byteCnt = dataCnt;
                wordCnt = (dataCnt / 2) + (dataCnt % 2);
            }
            // ﾃﾞｰﾀ数 = ﾜｰﾄﾞ数
            else
            {
                byteCnt = dataCnt * 2;
                wordCnt = dataCnt;
            }

            ushort[] recvData = new ushort[wordCnt];

            // 受信メッセージコピー
            Array.Copy(this.RecvMsgDevice.Value, recvData, wordCnt);

            // byte配列に変換
            byte[] recvByteData = ConvertToByteArray(recvData);

            // ﾊﾞｲﾄ数が奇数なら、終端1byteを削除
            if (dataCnt % 2 == 1)
            {
                recvByteData = recvByteData.Take(recvByteData.Length - 1).ToArray();
            }

            return recvByteData;
        }

        private void SetSendMsg(byte[] msg)
        {
            // ﾃﾞｰﾀ数
            int dataCnt = msg.Length;

            int byteCnt;
            int wordCnt;

            // ﾃﾞｰﾀ数 = ﾊﾞｲﾄ数
            if (this._IsByteCount)
            {
                byteCnt = dataCnt;
                wordCnt = (dataCnt / 2) + (dataCnt % 2);
            }
            // ﾃﾞｰﾀ数 = ﾜｰﾄﾞ数
            else
            {
                byteCnt = dataCnt * 2;
                wordCnt = dataCnt;
            }

            // ushort配列に変換
            ushort[] sendData = ConvertToUShortArray(msg);

            // 送信メッセージ書き込み
            SetDeviceData(this.SendMsgDevice.Address, sendData);

            // 送信データ数書き込み
            if (this._IsByteCount)
            {
                SetDeviceData(this.SendNumDevice.Address, (ushort)byteCnt);
            }
            else
            {
                SetDeviceData(this.SendNumDevice.Address, (ushort)wordCnt);
            }
        }


        private bool IsMyAddressMsg(byte[] recvMsg)
        {
            /* <メッセージフォーマット>
             * 
             *   STX - デバイスコード(1byte) - デバイスNo.(2byte) - データレングス(1byte) - メッセージNo.(1byte)
             */

            // 判別不可
            if (recvMsg.Length < 4)
            {
                return false;
            }


            // メッセージの最初はSTXで始まる
            if (recvMsg[0] != 0x02)
            {
                return false;
            }

            // デバイスNo.(宛先ノード)チェック
            //if (('0' <= recvMsg[2] && recvMsg[2] <= '9') &&
            //    ('0' <= recvMsg[3] && recvMsg[3] <= '9'))
            if('0' <= recvMsg[1] && recvMsg[1] <= '9')
            {
                if (AsciiUtils.ToInt(recvMsg, 1, 1) == this.NodeNumber)
                {
                    return true;
                }
            }

            return false;
        }

        private bool CheckEnableMsg(byte[] recvMsg)
        {
            if(recvMsg[2] == 'R')
            {
                int dataLength = 8;

                // ﾒｯｾｰｼﾞ長ﾁｪｯｸ
                if (recvMsg.Length < dataLength)
                {
                    AddLog("メッセージ長異常", recvMsg);
                    return false;
                }
                
                // BCCﾁｪｯｸ
                var bccTarget = recvMsg.Skip(1).Take(3).ToArray();
//                int bccValue = bccTarget.Sum((b) => b) & 0xFF;

                //int bccValue = 0x00;
                //for (int i = 0; i < bccTarget.Length; i++)
                //{
                //    bccValue += bccTarget[i];
                //}

                int bccValue = bccTarget.Sum((i) => i) & 0xFF;

                int msgBcc = (AsciiUtils.ToInt(recvMsg, 5, 1, true) << 4) + 
                    (AsciiUtils.ToInt(recvMsg, 6 , 1 , true));

                if(bccValue == msgBcc)
                {
                    return true;
                }
                else
                {
                    AddLog("メッセージパリティ異常", recvMsg);
                    return false;
                }
            }
            else if (recvMsg[2] == 'W')
            {
                if (recvMsg.Length < 7)
                {
                    AddLog("メッセージ長異常", recvMsg);
                    return false;
                }

                // BCCﾁｪｯｸ
                var bccTarget = recvMsg.Skip(1).Take(recvMsg.Length - 5).ToArray();
                //int bccValue = 0x00;
                //for (int i = 0; i < bccTarget.Length; i++)
                //{
                //    bccValue += bccTarget[i];
                //}
                int bccValue = bccTarget.Sum((i) => i) & 0xFF;

                int msgBcc = (AsciiUtils.ToInt(recvMsg, recvMsg.Length - 3, 1, true) << 4) +
                    (AsciiUtils.ToInt(recvMsg, recvMsg.Length - 2, 1, true));

                if (bccValue == msgBcc)
                {
                    return true;
                }
                else
                {
                    AddLog("メッセージパリティ異常", recvMsg);
                    return false;
                }
            }
            return false;

        } 

        /// <summary>
        /// エコーバック返信
        /// </summary>
        /// <param name="msg"></param>
        private void sendEchoBack(byte[] recvMsg)
        {
//            Debug.Assert(recvMsg.Length >= 1 + 5);

            if (!this.stopCommunication.Checked)
            {
                Enqueue(recvMsg);
            }
        }

        private byte[] SetHeader(IEnumerable<byte> data)
        {
            List<byte> msg = new List<byte>();
            msg.Add(0x02);
            msg.AddRange(AsciiUtils.ToAscii(NodeNumber, 1));
            msg.AddRange(data);
            msg.Add(0x03);

            var bccTarget = msg.Skip(1).Take(msg.Count - (1 + 1));  // メッセージ長 - (STX + ETX)
            int bccValue = bccTarget.Sum((b) => b) & 0xFF;

            msg.AddRange(AsciiUtils.ToAscii(bccValue, 2, true));
            

            msg.Add(0x0D);

            return msg.ToArray();
        }


        #endregion

        #region メソッド

        private void AddSendLog(byte[] log)
        {
//            Debug.Assert(log.Length >= (1 + 5 + 2));

            // ACK送信時
            if (getAckPosition(log))
            {
                AddLog(string.Format("SND[ACK] {0}", ByteArrayToString(log)));
            }
            else
            {
                string command = AsciiUtils.ToString(log, 2, 2);

                AddLog(string.Format("SND[{0,-3}] {1}", command, ByteArrayToString(log)));
            }
        }

        private void AddRecvLog(byte[] log)
        {
//            Debug.Assert(log.Length >= (1 + 5 + 2));

            string command = AsciiUtils.ToString(log, 2, 2);

            AddLog(string.Format("RCV[{0,-3}] {1}", command, ByteArrayToString(log)));
        }

        private void AddLog(string message, byte[] log)
        {
            AddLog(string.Format("{0} {1}", message, ByteArrayToString(log)));
        }

        private void AddLog(string message)
        {
            DateTime dtNow = DateTime.Now;
            string time = string.Format("{0:D2}:{1:D2}:{2:D2}.{3:D3}", dtNow.Hour, dtNow.Minute, dtNow.Second, dtNow.Millisecond);

            _LogMessage.Rows.Add(string.Format("{0} {1}", time, message));

            if (_LogMessage.Rows.Count > MAX_LOG)
            {
                _LogMessage.Rows.RemoveAt(0);
            }

            if (_LogForm != null)
            {
                _LogForm.lbLog.SelectedIndex = _LogForm.lbLog.Items.Count - 1;
                _LogForm.lbLog.Refresh();
            }
        }

        /// <summary>
        /// 制御モードテキストを取得
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        private string GetControlModeText(ControlMode mode)
        {
            switch (mode)
            {
                case ControlMode.制御動作停止モード:
                    return "制御動作停止ﾓｰﾄﾞ";
                case ControlMode.標準モード:
                    return "標準ﾓｰﾄﾞ";
                case ControlMode.学習制御モード:
                    return "学習制御ﾓｰﾄﾞ";
                case ControlMode.外部同調制御モード:
                    return "外部同調制御ﾓｰﾄﾞ";
                case ControlMode.オートチューニング中:
                    return "ｵｰﾄﾁｭｰﾆﾝｸﾞ中";
                case ControlMode.無指令:
                    return "無指令";
                default:
                    return "";
            }
        }

        /// <summary>
        /// デバイス値をバイト配列に変換します
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private byte[] ConvertToByteArray(ushort[] source)
        {
            List<byte> byteList = new List<byte>();

            foreach (ushort v in source)
            {
                byteList.Add((byte)((v & 0x00FF) >> 0));
                byteList.Add((byte)((v & 0xFF00) >> 8));
            }

            return byteList.ToArray();
        }

        /// <summary>
        /// バイト配列をデバイス値に変換します
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private ushort[] ConvertToUShortArray(byte[] source)
        {
            List<byte> sourceList = new List<byte>(source);

            // 奇数バイトチェック
            if (sourceList.Count % 2 == 1)
            {
                // 空バイト追加
                sourceList.Add((byte)0);
            }

            List<ushort> ushortList = new List<ushort>();

            for (int i = 0; i < sourceList.Count; i += 2)
            {
                ushort v = (ushort)((sourceList[i + 1] << 8) | (sourceList[i + 0] << 0));
                ushortList.Add(v);
            }

            return ushortList.ToArray();
        }


        /// <summary>
        /// バイト配列をバイナリ形式の文字列に変換します
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private string ByteArrayToString(byte[] source)
        {
            string[] strHex = source.Select((b) => b.ToString("X2")).ToArray();

            return string.Join(" ", strHex);
        }
 
        #endregion       
        
        
        #region プロパティ
        [Visible]
        [Category("01 受信設定"), DisplayName("入力エリア")]
        public DeviceFormat RecvMsgDevice
        {
            get;
            set;
        }

        [Visible]
        [Category("01 受信設定"), DisplayName("入力数")]
        public DeviceFormat RecvNumDevice
        {
            get;
            set;
        }

        [Visible]
        [Category("01 受信設定"), DisplayName("入力フラグ")]
        public DeviceFormat RecvFlagDevice
        {
            get;
            set;
        }

        [Visible]
        [Category("02 送信設定"), DisplayName("出力エリア")]
        public DeviceFormat SendMsgDevice
        {
            get;
            set;
        }

        [Visible]
        [Category("02 送信設定"), DisplayName("出力数")]
        public DeviceFormat SendNumDevice
        {
            get;
            set;
        }

        [Visible]
        [Category("02 送信設定"), DisplayName("出力フラグ")]
        public DeviceFormat SendFlagDevice
        {
            get;
            set;
        }

        [Visible]
        [Category("03 その他"), DisplayName("ノードNo")]
        public int NodeNumber { get; set; }

        [Visible]
        [Category("03 その他"), DisplayName("現在温度")]
        public DeviceFormat NowTemperature { get; set; }

        /// <summary>
        /// 現在温度
        /// </summary>
        [Visible(false)]
        public double _NowTemperature
        {
            get
            {
                return _nowTemperature;
            }
            set
            {
                _nowTemperature = value;

                ushort integerNowTemp = (ushort)Math.Round(_nowTemperature * 100);
                SetDeviceData(this.NowTemperature.Address, integerNowTemp);
            }
        }

        #endregion
        
        #region クラス・定義

        //制御モード
        public enum ControlMode : byte
        {
            制御動作停止モード = 0x00,
            標準モード = 0x01,
            学習制御モード = 0x02,
            外部同調制御モード = 0x03,
            オートチューニング中 = 0x04,
            無指令 = 0x0f,
        }


        //温調器設定
        public class ThermoSetting
        {
            public ControlMode ControlMode { get; set; }         //制御モード
            public double TargetTemperature { get; set; }        //目標温度
            public double PB_Width { get; set; }                 //PB幅
            public int I_Constant { get; set; }                  //I定数
            public int D_Constant { get; set; }                  //D定数
            public double OffsetValue { get; set; }              //オフセット値
            public double SensorProofread { get; set; }          //センサ微調整値
            public double MaxTemperature { get; set; }           //上限温度幅
            public double MinTemperature { get; set; }           //下限温度幅
            public double PbConst { get; set; }                  // Pb(演算開始定数)
        }


        public interface IEntity : INotifyPropertyChanged
        {
            void NotifyPropertyChanged(String propertyName);
        }


        //温調器ステータス
        public class ThermoStatus
        {
            public double measuredTemparature;

            private class BitAttribute : Attribute
            {
                public BitAttribute(int bit)
                {
                    _Bit = bit;
                }

                int _Bit;
                public int Bit { get { return _Bit; } }
            }

            [Bit(11)]
            public bool ThermoModulePowerSupplyVoltageUpLow { get; set; }   // ｻｰﾓﾓｼﾞｭｰﾙ電源電圧上昇、低下
            [Bit(10)]
            public bool ExcessiveTemperatureRise { get; set; }              // 温度過昇温センサ、過冷却
            [Bit(9)]
            public bool TempSensorError { get; set; }                       // 温度センサ断線、短絡アラーム
            [Bit(8)]
            public bool ThermoStatAlarm { get; set; }                       // サーモスタットアラーム
            [Bit(7)]
            public bool ThermoModulePowerFailureAlarm { get; set; }         // ｻｰﾓﾓｼﾞｭｰﾙ電源異常アラーム            
            [Bit(1)]
            public bool TempUpperAlarm { get; set; }                        // 温度上限アラーム
            [Bit(0)]
            public bool TempLowerAlarm { get; set; }                        // 温度下限アラーム


            /// <summary>
            /// 現在ステータスを2byteデータで取得
            /// </summary>
            /// <returns></returns>
            public ushort GetDataStatus()
            {
                ushort statusData = 0;

                foreach (PropertyDescriptor pd in TypeDescriptor.GetProperties(this))
                {
                    var bitAttrs = pd.Attributes.OfType<BitAttribute>();

                    if (bitAttrs.Count() != 0)
                    {
                        BitAttribute attr = bitAttrs.First();

                        if ((bool)pd.GetValue(this))
                        {
                            statusData |= (ushort)(0x01 << attr.Bit);
                        }
                    }
                }

                return statusData;
            }

              
        }

        #endregion    
 
    }
}
