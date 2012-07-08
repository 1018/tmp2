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
    public partial class SRZ_Z_TIO_Draw : Symbol_Draw
    {
        #region constructor
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SRZ_Z_TIO_Draw()
        {
            InitializeComponent();
            containTrackingBox();

            base.CyclicFlag = true;

            base.SymbolType = "SRZ_Z_TIO";
            base.Category = "温調機";

            #region 初期化
            this.RecvMsgDevice = new DeviceFormat("KM2000", 512, IoType.In, SetType.Word);
            this.RecvNumDevice = new DeviceFormat("KM2600", 1, IoType.In, SetType.Word);
            this.RecvFlagDevice = new DeviceFormat("KM2601.1", 1, IoType.In, SetType.Bit);
            this.SendMsgDevice = new DeviceFormat("KM0", 512, IoType.Io, SetType.Word);
            this.SendNumDevice = new DeviceFormat("KM600", 1, IoType.Io, SetType.Word);
            this.SendFlagDevice = new DeviceFormat("KM601.1", 1, IoType.Io, SetType.Bit);
            this.NodeNumber = 0;
            this.NowTemperature = new DeviceFormat("KM10000", 1, IoType.Out, SetType.Word);

            this._IsByteCount = true;

            #endregion
        }
        #endregion

        const int MAX_LOG = 100;
        const string LOG_TEXT_COLUMN_NAME = "LogginText";

        bool _IsByteCount;

        Queue<byte[]> _SendQueue = new Queue<byte[]>();

        ThermoSetting _Setting = new ThermoSetting();
        ThermoStatus _Status = new ThermoStatus();

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
        CheckBox[] hcflg0Box;
        CheckBox[] hcflg1Box;
        CheckBox[] hcflg2Box;
        CheckBox[] hcflg3Box;
        CheckBox[] hcflg4Box;
        CheckBox[] hcflg6Box;
        CheckBox[] hcflg7Box;
        TextBox[] almNoBox;

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


            // hcflg0:オーバーヒート検出
            this.hcflg0Box = new CheckBox[4];
            this.hcflg0Box[0] = this.hcflg1_0;
            this.hcflg0Box[1] = this.hcflg2_0;
            this.hcflg0Box[2] = this.hcflg3_0;
            this.hcflg0Box[3] = this.hcflg4_0;

            // hcflg1:アンダーヒート検出
            this.hcflg1Box = new CheckBox[4];
            this.hcflg1Box[0] = this.hcflg1_1;
            this.hcflg1Box[1] = this.hcflg2_1;
            this.hcflg1Box[2] = this.hcflg3_1;
            this.hcflg1Box[3] = this.hcflg4_1;

            // hcflg2:バーンアウト検出
            this.hcflg2Box = new CheckBox[4];
            this.hcflg2Box[0] = this.hcflg1_2;
            this.hcflg2Box[1] = this.hcflg2_2;
            this.hcflg2Box[2] = this.hcflg3_2;
            this.hcflg2Box[3] = this.hcflg4_2;

            // hcflg3:温度安定
            this.hcflg3Box = new CheckBox[4];
            this.hcflg3Box[0] = this.hcflg1_3;
            this.hcflg3Box[1] = this.hcflg2_3;
            this.hcflg3Box[2] = this.hcflg3_3;
            this.hcflg3Box[3] = this.hcflg4_3;

            // hcflg4:温度異常
            this.hcflg4Box = new CheckBox[4];
            this.hcflg4Box[0] = this.hcflg1_4;
            this.hcflg4Box[1] = this.hcflg2_4;
            this.hcflg4Box[2] = this.hcflg3_4;
            this.hcflg4Box[3] = this.hcflg4_4;

            // hcflg6:オートチューニング中
            this.hcflg6Box = new CheckBox[4];
            this.hcflg6Box[0] = this.hcflg1_6;
            this.hcflg6Box[1] = this.hcflg2_6;
            this.hcflg6Box[2] = this.hcflg3_6;
            this.hcflg6Box[3] = this.hcflg4_6;

            // hcflg7:オートチューニング完了
            this.hcflg7Box = new CheckBox[4];
            this.hcflg7Box[0] = this.hcflg1_7;
            this.hcflg7Box[1] = this.hcflg2_7;
            this.hcflg7Box[2] = this.hcflg3_7;
            this.hcflg7Box[3] = this.hcflg4_7;

            // 警報番号
            this.almNoBox = new TextBox[4];
            this.almNoBox[0] = this.almNum01;
            this.almNoBox[1] = this.almNum02;
            this.almNoBox[2] = this.almNum03;
            this.almNoBox[3] = this.almNum04;
        }


        DataTable _LogMessage = new DataTable();
        Log_Form _LogForm = null;

        Dictionary<string, Action<byte[]>> _PolingSplitterDic = null;
        Dictionary<string, Action<byte[]>> _SelectingSplitterDic = null;

        private void InitSplitterDictionary()
        {
            if (_SelectingSplitterDic == null)
            {
                _SelectingSplitterDic = new Dictionary<string, Action<byte[]>>();

                _SelectingSplitterDic["SR"] = SplitSR_Selecting;
                _SelectingSplitterDic["C1"] = SplitC1_Selecting;  // 未
                _SelectingSplitterDic["S1"] = SplitS1_Selecting;
                _SelectingSplitterDic["A1"] = SplitA1_Selecting;
                _SelectingSplitterDic["A2"] = SplitA2_Selecting;
//                _SelectingSplitterDic["W1"] = SplitW1_Selecting;  // 未
                _SelectingSplitterDic["J1"] = SplitJ1_Selecting;  // 未
                _SelectingSplitterDic["P1"] = SplitP1_Selecting;
                _SelectingSplitterDic["I1"] = SplitI1_Selecting;
                _SelectingSplitterDic["D1"] = SplitD1_Selecting;
            }
            if (_PolingSplitterDic == null)
            {
                _PolingSplitterDic = new Dictionary<string, Action<byte[]>>();

                _PolingSplitterDic["ER"] = SplitER_Poling;
                _PolingSplitterDic["C1"] = SplitC1_Poling;  // 未
                _PolingSplitterDic["S1"] = SplitS1_Poling;
                _PolingSplitterDic["G1"] = SplitG1_Poling;  // 未
                _PolingSplitterDic["M1"] = SplitM1_Poling;
                _PolingSplitterDic["AJ"] = SplitAJ_Poling;
                
            }
        }

        private bool CallPolingSplitter(string command, byte[] recvMsg)
        {
            if (_PolingSplitterDic.ContainsKey(command))
            {
                _PolingSplitterDic[command].Invoke(recvMsg);
                return true;
            }
            else
            {
                return false;
            }
        }


        private bool CallSelectingSplitter(string command, byte[] recvMsg)
        {
            if (_SelectingSplitterDic.ContainsKey(command))
            {
                _SelectingSplitterDic[command].Invoke(recvMsg);
                return true;
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// MainProgram通信開始時処理
        /// </summary>
        public override void Initial()
        {
            InitSplitterDictionary();
            if (!_LogMessage.Columns.Contains(LOG_TEXT_COLUMN_NAME))
            {
                _LogMessage.Columns.Add(LOG_TEXT_COLUMN_NAME);
            }
            updateDisplay();
        }

        /// <summary>
        /// MainProgram 定周期処理
        /// </summary>
        public override void CyclicMethod()
        {
            // メッセージ受信ありチェック
            if (RecvFlagDevice.Value[0] != 0)
            {
                RecvMsgProc();
            }
            // 送信メッセージありチェック
            if (!EmptyQueue())
            {
                // 前回の送信が完了していれば、メッセージを送信する
                if (SendFlagDevice.Value[0] == 0)
                {
                    byte[] sendMsg = Dequeue();
                    SetSendMsg(sendMsg);

                    // 送信フラグON
                    SetDeviceData(this.SendFlagDevice.Address, 1);
                    this.SendFlagDevice.Value[0] = 1;
                }
            }
        }

        /// <summary>
        /// mainprogram 通信切断処理
        /// </summary>
        protected override void Abort()
        {
            if (_LogForm != null)
            {
                _LogForm.Close();
            }
        }

        //        protected override void updateDisplay()        
        protected void updateDisplay()
        {
            Action action = new Action(
                () =>
                {
                    // 設定データを表示
                    targetTemp01.Text = _SettingArray[0].TargetTemperature.ToString("0.0");
                    targetH01.Text = _SettingArray[0].upper.ToString("0.0");
                    targetL01.Text = _SettingArray[0].lower.ToString("0.0");
                    targetP01.Text = _SettingArray[0].targetP.ToString("0.0");
                    targetI01.Text = _SettingArray[0].targetI.ToString("0");
                    targetD01.Text = _SettingArray[0].targetD.ToString("0");

                    targetTemp02.Text = _SettingArray[1].TargetTemperature.ToString("0.0");
                    targetH02.Text = _SettingArray[1].upper.ToString("0.0");
                    targetL02.Text = _SettingArray[1].lower.ToString("0.0");
                    targetP02.Text = _SettingArray[1].targetP.ToString("0.0");
                    targetI02.Text = _SettingArray[1].targetI.ToString("0");
                    targetD02.Text = _SettingArray[1].targetD.ToString("0");


                    targetTemp03.Text = _SettingArray[2].TargetTemperature.ToString("0.0");
                    targetH03.Text = _SettingArray[2].upper.ToString("0.0");
                    targetL03.Text = _SettingArray[2].lower.ToString("0.0");
                    targetP03.Text = _SettingArray[2].targetP.ToString("0.0");
                    targetI03.Text = _SettingArray[2].targetI.ToString("0");
                    targetD03.Text = _SettingArray[2].targetD.ToString("0");

                    targetTemp04.Text = _SettingArray[3].TargetTemperature.ToString("0.0");
                    targetH04.Text = _SettingArray[3].upper.ToString("0.0");
                    targetL04.Text = _SettingArray[3].lower.ToString("0.0");
                    targetP04.Text = _SettingArray[3].targetP.ToString("0.0");
                    targetI04.Text = _SettingArray[3].targetI.ToString("0");
                    targetD04.Text = _SettingArray[3].targetD.ToString("0");

                    // ステータスを表示
                    measuredTemp01.Value = (decimal)_StatusArray[0].measuredTemparature;
                    measuredTemp02.Value = (decimal)_StatusArray[1].measuredTemparature;
                    measuredTemp03.Value = (decimal)_StatusArray[2].measuredTemparature;
                    measuredTemp04.Value = (decimal)_StatusArray[3].measuredTemparature;

                    almNum01.Text = _StatusArray[0].almNumber;
                    almNum02.Text = _StatusArray[1].almNumber;
                    almNum03.Text = _StatusArray[2].almNumber;
                    almNum04.Text = _StatusArray[3].almNumber;

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

        #region 通信処理

        /// <summary>
        /// メッセージ受信処理
        /// </summary>
        private void RecvMsgProc()
        {
            byte[] recvMsg = GetRecvMsg();
           
            if (!IsMyAddressMsg(recvMsg))
            {
                return;
            }

            try
            {
                if (!CheckEnableMsg(recvMsg))
                {
                    return;
                }
                if (GetMsgType(recvMsg) == MsgType.Selecting)
                {
                    string command = AsciiUtils.ToString(recvMsg, 4, 2);
                    if (CallSelectingSplitter(command, recvMsg))
                    {
                        AddRecvLog(recvMsg);
                        updateDisplay();
                    }
                    else
                    {
                        AddLog("不明なコマンド", recvMsg);
                    }
                }
                else if (GetMsgType(recvMsg) == MsgType.Poling)
                {
                    string command = AsciiUtils.ToString(recvMsg, 5, 2);

                    if (CallPolingSplitter(command, recvMsg))
                    {
                        updateDisplay();
                    }
                    else
                    {
                    }
                }
            }
            finally
            {
                // 受信フラグOFF
                SetDeviceData(this.RecvFlagDevice.Address, 0);
                this.RecvFlagDevice.Value[0] = 0;
            }
        }

        // SR:RUN/STOP設定
        private void SplitSR_Selecting(byte[] recvMsg)
        {
            byte[] data = GetSelectingData(recvMsg);
            // 最小ﾒｯｾｰｼﾞ長ﾁｪｯｸ
            if (data.Length < 1)
            {
                AddLog("SRﾒｯｾｰｼﾞが最小ｻｲｽﾞに満たない", recvMsg);
                SendNak();
                return;
            }

            if (!this.stopCommunication.Checked)
            {
                // 最後の00を削除
                byte[] recvMsg2 = new byte[9];
                Array.Copy(recvMsg, 0, recvMsg2, 0, 9);
                sendEchoBack(recvMsg2);
                SendAck();
            }
            else
            {
                SendNak();
            }

        }

        // ER:警報番号読出し
        private void SplitER_Poling(byte[] recvMsg)
        {
            if (!this.stopCommunication.Checked)
            {
                sendEchoBack(recvMsg);
                List<byte> data = new List<byte>();
                data.AddRange(AsciiUtils.ToAscii("ER"));
                for (int i = 0; i < 4; i++)
                {
                    if (i == 3)
                    {
                        data.AddRange(AsciiUtils.ToAscii(_StatusArray[i].almNumber));
                    }
                    else
                    {
                        data.AddRange(AsciiUtils.ToAscii(_StatusArray[i].almNumber.PadLeft(2)));
                    }
                }
                byte[] sendMsg = SetHeader(data);
                Enqueue(sendMsg);
            }
        }

               


        // C1:ﾘﾓｰﾄ・ﾛｰｶﾙ設定の読み出し
        private void SplitC1_Poling(byte[] recvMsg)
        {
            if (!this.stopCommunication.Checked)
            {
                sendEchoBack(recvMsg);

                List<byte> data = new List<byte>();
                data.AddRange(AsciiUtils.ToAscii("C1"));
                data.AddRange(AsciiUtils.ToAscii(this.modeSwitch.Text));

                byte[] sendMsg = SetHeader(data);
                Enqueue(sendMsg);
            }
        }

        // S1:目標温度の読み出し
        private void SplitS1_Poling(byte[] recvMsg)
        {
            if (!this.stopCommunication.Checked)
            {
                sendEchoBack(recvMsg);

                List<byte> data = new List<byte>();
                data.AddRange(AsciiUtils.ToAscii("S1"));
                for (int i = 0; i < 4; i++)
                {
                    // CH番号
                    data.Add(0x30);
                    byte[] chNo = System.Text.Encoding.ASCII.GetBytes((i + 1).ToString());
                    data.Add(chNo[0]);

                    data.Add(0x20);
                    data.Add(0x20);
                    data.Add(0x20);

                    string strTemp = string.Format("{0, 5:F1}", _SettingArray[i].TargetTemperature);
                    data.AddRange(AsciiUtils.ToAscii(strTemp));
                    // ｶﾝﾏ
                    if (i < 3)
                    {
                        data.Add(0x2C);
                    }
                }
                byte[] sendMsg = SetHeader(data);
                Enqueue(sendMsg);
            }

        }

        // G1:PID/AT切換
        private void SplitG1_Poling(byte[] recvMsg)
        {
            if (!this.stopCommunication.Checked)
            {
                sendEchoBack(recvMsg);

                List<byte> data = new List<byte>();
                data.AddRange(AsciiUtils.ToAscii("G1"));
                data.AddRange(AsciiUtils.ToAscii(this.autoTuning.Text));

                byte[] sendMsg = SetHeader(data);
                Enqueue(sendMsg);
            }
        }

        // M1:測定温度の読み出し
        private void SplitM1_Poling(byte[] recvMsg)
        {
            if (!this.stopCommunication.Checked)
            {
                sendEchoBack(recvMsg);

                List<byte> data = new List<byte>();
                data.AddRange(AsciiUtils.ToAscii("M1"));
                for (int i = 0; i < 4; i++)
                {
                    // CH番号
                    data.Add(0x30);
                    byte[] chNo = System.Text.Encoding.ASCII.GetBytes((i + 1).ToString());
                    data.Add(chNo[0]);

                    data.Add(0x20);
                    data.Add(0x20);
                    data.Add(0x20);

                    string strTemp = string.Format("{0, 5:F1}", _StatusArray[i].measuredTemparature);
                    data.AddRange(AsciiUtils.ToAscii(strTemp));
                    // ｶﾝﾏ
                    if (i < 3)  data.Add(0x2C);
                }
                byte[] sendMsg = SetHeader(data);
                Enqueue(sendMsg);
            }

        }

        // AJ:総合イベント状態
        private void SplitAJ_Poling(byte[] recvMsg)
        {
            if (!this.stopCommunication.Checked)
            {
                sendEchoBack(recvMsg);
                List<byte> data = new List<byte>();
                data.AddRange(AsciiUtils.ToAscii("AJ"));
                for (int i = 0; i < 4; i++)
                {
                    // CH番号
                    data.Add(0x30);
                    byte[] chNo = System.Text.Encoding.ASCII.GetBytes((i + 1).ToString());
                    data.Add(chNo[0]);

                    data.Add(0x20);

                    // hcflg0
                    if (this.hcflg0Box[i].Checked) data.Add(0x31);
                    else data.Add(0x30);

                    // hcflg1
                    if (this.hcflg1Box[i].Checked) data.Add(0x31);
                    else data.Add(0x30);

                    // hcflg2
                    if (this.hcflg2Box[i].Checked) data.Add(0x31);
                    else data.Add(0x30);

                    // hcflg3
                    if (this.hcflg3Box[i].Checked) data.Add(0x31);
                    else data.Add(0x30);

                    // hcflg4
                    if (this.hcflg4Box[i].Checked) data.Add(0x31);
                    else data.Add(0x30);                    

                    // hcflg6
                    if (this.hcflg6Box[i].Checked) data.Add(0x31);
                    else data.Add(0x30);

                    // hcflg7
                    if (this.hcflg7Box[i].Checked) data.Add(0x31);
                    else data.Add(0x30);

                    if (i < 3) data.Add(0x2C);
                }
                byte[] sendMsg = SetHeader(data);
                Enqueue(sendMsg);
            }
        }


        // C1:オートチューニングのキャンセル
        private void SplitC1_Selecting(byte[] recvMsg)
        {
            byte[] data = GetSelectingData(recvMsg);

            // 最小ﾒｯｾｰｼﾞ長ﾁｪｯｸ
            if(data.Length < 8 + 2)
            {
                AddLog("C1ﾒｯｾｰｼﾞｻｲｽﾞが最小ｻｲｽﾞに満たない", recvMsg);
                SendNak();
                return;
            }


            if (!this.stopCommunication.Checked)
            {
                sendEchoBack(recvMsg);
                SendAck();

                for (int i = 0; i < 4; i++)
                {
                    if (hcflg6Box[i].Checked) hcflg6Box[i].Checked = false;
                    if (hcflg7Box[i].Checked) hcflg7Box[i].Checked = false;
                }
            }
            else
            {
                SendNak();
            }
        }                


        // S1:目標温度の設定
        private void SplitS1_Selecting(byte[] recvMsg)
        {
            byte[] data = GetSelectingData(recvMsg);

            // 最小メッセージ長チェック
            if (data.Length < 8 + 2)
            {
                AddLog("S1ﾒｯｾｰｼﾞが最小ｻｲｽﾞに満たない", recvMsg);
                SendNak();
                return;
            }

            if (!this.stopCommunication.Checked)
            {                
                sendEchoBack(recvMsg);
                SendAck();                

                // 目標温度を取得
                string strTemp = AsciiUtils.ToString(data, 5, 5);
                double testResult;

                for (int i = 0; i < 4; i++)
                {
                    // ﾉｰﾄﾞ番号とCH番号が同じものがあれば温度だけ上書き
                    if (_SettingArray[i].nodeNum == recvMsg[2] && _SettingArray[i].chNo == recvMsg[7])
                    {
                        // 目標温度を取得                    
                        if (double.TryParse(strTemp, out testResult))
                        {
                            _SettingArray[i].TargetTemperature = testResult;
                        }
                        // 現在温度を目標温度に
//                        if (!this.noTracking01.Checked)
                        if(!trackingBox[i].Checked)
                        {
                            _StatusArray[i].measuredTemparature = _SettingArray[i].TargetTemperature;
                        }
                        break;
                    }
                    // 初回の場合
                    else if (_SettingArray[i].nodeNum == 0x20 && _SettingArray[i].chNo == 0x20)
                    {
                        _SettingArray[i].nodeNum = recvMsg[2];
                        _SettingArray[i].chNo = recvMsg[7];
                        // 目標温度を取得
                        if (double.TryParse(strTemp, out testResult))
                        {
                            _SettingArray[i].TargetTemperature = testResult;
                        }
                        // 現在温度を目標温度に
//                        if (!this.noTracking01.Checked)
                        if (!trackingBox[i].Checked)
                        {
                            _StatusArray[i].measuredTemparature = _SettingArray[i].TargetTemperature;
                        }
                        break;
                    }
                }
            }
            else
            {
                SendNak();
            }
        }

        // A1：上限温度(オーバーヒート)
        private void SplitA1_Selecting(byte[] recvMsg)
        {
            byte[] data = GetSelectingData(recvMsg);

            // 最小メッセージ長チェック
            if (data.Length < 8 + 2)
            {
                AddLog("A1ﾒｯｾｰｼﾞが最小ｻｲｽﾞに満たない", recvMsg);
                SendNak();
                return;
            }

            if (!this.stopCommunication.Checked)
            {                
                sendEchoBack(recvMsg);
                SendAck();
                
                // 文字列を取得
                string strData = AsciiUtils.ToString(data, 6, 4);
                double testResult;

                for (int i = 0; i < 8; i++)
                {
                    // ﾉｰﾄﾞ番号とCH番号が同じものがあれば温度だけ上書き
                    if (_SettingArray[i].nodeNum == recvMsg[2] && _SettingArray[i].chNo == recvMsg[7])
                    {
                        // 上限温度幅を取得                    
                        if (double.TryParse(strData, out testResult))
                        {
                            _SettingArray[i].upper = testResult;
                        }
                        break;
                    }
                    // 初回の場合
                    else if (_SettingArray[i].nodeNum == 0x20 && _SettingArray[i].chNo == 0x20)
                    {
                        _SettingArray[i].nodeNum = recvMsg[2];
                        _SettingArray[i].chNo = recvMsg[7];
                        // 上限温度幅を取得
                        if (double.TryParse(strData, out testResult))
                        {
                            _SettingArray[i].upper = testResult;
                        }
                        break;
                    }
                }
            }
            else
            {
                SendNak();
            }
        }
        // A2：下限温度(アンダーヒート)
        private void SplitA2_Selecting(byte[] recvMsg)
        {
            byte[] data = GetSelectingData(recvMsg);

            // 最小メッセージ長チェック
            if (data.Length < 8 + 2)
            {
                AddLog("A2ﾒｯｾｰｼﾞが最小ｻｲｽﾞに満たない", recvMsg);
                SendNak();
                return;
            }

            if (!this.stopCommunication.Checked)
            {               
                sendEchoBack(recvMsg);
                SendAck();
                
                // 文字列を取得
                string strData = AsciiUtils.ToString(data, 6, 4);
                double testResult;

                for (int i = 0; i < 8; i++)
                {
                    // ﾉｰﾄﾞ番号とCH番号が同じものがあれば温度だけ上書き
                    if (_SettingArray[i].nodeNum == recvMsg[2] && _SettingArray[i].chNo == recvMsg[7])
                    {
                        // 下限温度幅を取得                    
                        if (double.TryParse(strData, out testResult))
                        {
                            _SettingArray[i].lower = testResult;
                        }
                        break;
                    }
                    // 初回の場合
                    else if (_SettingArray[i].nodeNum == 0x20 && _SettingArray[i].chNo == 0x20)
                    {
                        _SettingArray[i].nodeNum = recvMsg[2];
                        _SettingArray[i].chNo = recvMsg[7];
                        // 下限温度幅を取得
                        if (double.TryParse(strData, out testResult))
                        {
                            _SettingArray[i].lower = testResult;
                        }
                        break;
                    }
                }
            }
            else
            {
                SendNak();
            }
        }

        // W1:アンチリセットワインドアップ(ｽﾚｰﾌﾞ側実装保留)
        private void SplitW1_Selecting(byte[] recvMsg)
        {
            byte[] data = GetSelectingData(recvMsg);

            // 最小ﾒｯｾｰｼﾞ長ﾁｪｯｸ
            if(data.Length < 8 + 2)
            {
                AddLog("W1ﾒｯｾｰｼﾞｻｲｽﾞが最小ｻｲｽﾞに満たない", recvMsg);
                SendNak();
                return;
            }

            if (!this.stopCommunication.Checked)
            {
            }
            else
            {
                SendNak();
            }

        }

        // J1:オートチューニングの実行
        private void SplitJ1_Selecting(byte[] recvMsg)
        {
            byte[] data = GetSelectingData(recvMsg);

            // 最小ﾒｯｾｰｼﾞ長ﾁｪｯｸ
            if (data.Length < 8 + 2)
            {
                AddLog("J1ﾒｯｾｰｼﾞｻｲｽﾞが最小ｻｲｽﾞに満たない", recvMsg);
                SendNak();
                return;
            }


            if (!this.stopCommunication.Checked)
            {
                sendEchoBack(recvMsg);
                SendAck();
                
                for (int i = 0; i < 4; i++)
                {
                    if (!hcflg6Box[i].Checked) hcflg6Box[i].Checked = true;
                }
            }
            else
            {
                SendNak();
            }

        }                

        // P1：比例帯
        private void SplitP1_Selecting(byte[] recvMsg)
        {
            byte[] data = GetSelectingData(recvMsg);

            // 最小メッセージ長チェック
            if (data.Length < 8 + 2)
            {
                AddLog("P1ﾒｯｾｰｼﾞが最小ｻｲｽﾞに満たない", recvMsg);
                SendNak();
                return;
            }

            if (!this.stopCommunication.Checked)
            {
                sendEchoBack(recvMsg);
                SendAck();                
                
                // 文字列を取得
                string strData = AsciiUtils.ToString(data, 6, 4);
                double testResult;

                for (int i = 0; i < 4; i++)
                {
                    // ﾉｰﾄﾞ番号とCH番号が同じものがあれば温度だけ上書き
                    if (_SettingArray[i].nodeNum == recvMsg[2] && _SettingArray[i].chNo == recvMsg[7])
                    {
                        // 比例帯を取得                    
                        if (double.TryParse(strData, out testResult))
                        {
                            _SettingArray[i].targetP = testResult;
                        }
                        break;
                    }
                    // 初回の場合
                    else if (_SettingArray[i].nodeNum == 0x20 && _SettingArray[i].chNo == 0x20)
                    {
                        _SettingArray[i].nodeNum = recvMsg[2];
                        _SettingArray[i].chNo = recvMsg[7];
                        // 比例帯を取得
                        if (double.TryParse(strData, out testResult))
                        {
                            _SettingArray[i].targetP = testResult;
                        }
                        break;
                    }
                }
            }
            else
            {
                SendNak();
            }
        }

        // I1：積分時間
        private void SplitI1_Selecting(byte[] recvMsg)
        {
            byte[] data = GetSelectingData(recvMsg);

            // 最小メッセージ長チェック
            if (data.Length < 8 + 2)
            {
                AddLog("I1ﾒｯｾｰｼﾞが最小ｻｲｽﾞに満たない", recvMsg);
                SendNak();
                return;
            }

            if (!this.stopCommunication.Checked)
            {
                sendEchoBack(recvMsg);
                SendAck();
                
                // 文字列を取得
                string strData = AsciiUtils.ToString(data, 7, 3);
                double testResult;

                for (int i = 0; i < 8; i++)
                {
                    // ﾉｰﾄﾞ番号とCH番号が同じものがあれば温度だけ上書き
                    if (_SettingArray[i].nodeNum == recvMsg[2] && _SettingArray[i].chNo == recvMsg[7])
                    {
                        // 積分時間を取得                    
                        if (double.TryParse(strData, out testResult))
                        {
                            _SettingArray[i].targetI = (int)testResult;
                        }
                        break;
                    }
                    // 初回の場合
                    else if (_SettingArray[i].nodeNum == 0x20 && _SettingArray[i].chNo == 0x20)
                    {
                        _SettingArray[i].nodeNum = recvMsg[2];
                        _SettingArray[i].chNo = recvMsg[7];
                        // 積分時間を取得
                        if (double.TryParse(strData, out testResult))
                        {
                            _SettingArray[i].targetI = (int)testResult;
                        }
                        break;
                    }
                }
            }
            else
            {
                SendNak();
            }
        }

        // D1：微分時間
        private void SplitD1_Selecting(byte[] recvMsg)
        {
            byte[] data = GetSelectingData(recvMsg);

            // 最小メッセージ長チェック
            if (data.Length < 8 + 2)
            {
                AddLog("D1ﾒｯｾｰｼﾞが最小ｻｲｽﾞに満たない", recvMsg);
                SendNak();
                return;
            }

            if (!this.stopCommunication.Checked)
            {
                sendEchoBack(recvMsg);
                SendAck();
                
                // 文字列を取得
                string strData = AsciiUtils.ToString(data, 7, 3);
                double testResult;

                for (int i = 0; i < 8; i++)
                {
                    // ﾉｰﾄﾞ番号とCH番号が同じものがあれば温度だけ上書き
                    if (_SettingArray[i].nodeNum == recvMsg[2] && _SettingArray[i].chNo == recvMsg[7])
                    {
                        // 微分時間を取得                    
                        if (double.TryParse(strData, out testResult))
                        {
                            _SettingArray[i].targetD = (int)testResult;
                        }
                        break;
                    }
                    // 初回の場合
                    else if (_SettingArray[i].nodeNum == 0x20 && _SettingArray[i].chNo == 0x20)
                    {
                        _SettingArray[i].nodeNum = recvMsg[2];
                        _SettingArray[i].chNo = recvMsg[7];
                        // 微分時間を取得
                        if (double.TryParse(strData, out testResult))
                        {
                            _SettingArray[i].targetD = (int)testResult;
                        }
                        break;
                    }
                }
            }
            else
            {
                SendNak();
            }
        }

        private byte[] GetRecvMsg()
        {
            // データ数
            int dataCnt = this.RecvNumDevice.Value[0];

            int byteCnt;
            int wordCnt;

            // データ数 = バイト数
            if (this._IsByteCount)
            {
                byteCnt = dataCnt;
                wordCnt = (dataCnt / 2) + (dataCnt % 2);
            }
            // データ数 = ワード数
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

            return recvByteData;
        }

        private void SetSendMsg(byte[] msg)
        {
            // データ数
            int dataCnt = msg.Length;

            int byteCnt;
            int wordCnt;

            // データ数 = バイト数
            if (this._IsByteCount)
            {
                byteCnt = dataCnt;
                wordCnt = (dataCnt / 2) + (dataCnt % 2);
            }
            // データ数 = ワード数
            else
            {
                byteCnt = dataCnt * 2;
                wordCnt = dataCnt;
            }

            //ushort配列に変換
            ushort[] sendData = ConvertToUShortArray(msg);

            // 送信メッセージ書き込み            
            SetDeviceData(this.SendMsgDevice.Address, sendData);


            // 送信メッセージ数書き込み
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
            // 判断不可
            if (recvMsg.Length < 3)
            {
                return false;
            }

            // メッセージの最初はEOTで始まる
            if (recvMsg[0] != 0x04)
            {
                return false;
            }

            // デバイスNo.(宛先ﾉｰﾄﾞ)チェック
            if (('0' <= recvMsg[1] && recvMsg[1] <= '9') &&
               ('0' <= recvMsg[2] && recvMsg[2] <= '9'))
            {
                if (AsciiUtils.ToInt(recvMsg, 1, 2) == this.NodeNumber)
//                    (recvMsg[4] == 'S' && recvMsg[5] == 'R'))
                {
                    return true;
                }
            }
            // ｽﾚｰﾌﾞの受信メッセージチェック
            if ((recvMsg[0] == 0x04) && recvMsg[1] == 0x00)                
            {
                return true;
            }
            return false;
        }

        private bool CheckEnableMsg(byte[] recvMsg)
        {
            // 判別不可
            if (recvMsg.Length < 6)
            {
                AddLog("ﾒｯｾｰｼﾞ長異常", recvMsg);
                return false;
            }

            // ｾﾚｸﾃｨﾝｸﾞ後のEOT
            if (recvMsg[0] == 0x04 && recvMsg[1] == 0x00)
            {
                return true;
            }

            // ポーリングメッセージ
            if (GetMsgType(recvMsg) == MsgType.Poling)
            {
                // メッセージ長チェック
                if (recvMsg.Length != (1 + 2 + 2 + 2 + 1))
                {
                    AddLog("ﾒｯｾｰｼﾞ長異常", recvMsg);
                    return false;
                }

                // ENQチェック
                if (recvMsg[7] != 0x05)
                {
                    AddLog("ﾒｯｾｰｼﾞﾌｫｰﾏｯﾄ異常", recvMsg);
                    return false;
                }

                return true;
            }
            // セレクティングメッセージ
            else if (GetMsgType(recvMsg) == MsgType.Selecting)
            {
                byte[] last_2byte;
                // 終端がETX - [BCC]か
                if (recvMsg[4] == 'S' && recvMsg[5] == 'R')
                {
                    last_2byte = recvMsg.Skip(recvMsg.Length - 3).ToArray();
                }
                else
                {
                    last_2byte = recvMsg.Skip(recvMsg.Length - 2).ToArray();
                }
  
                if (last_2byte[0] != 0x03)
                {
                    AddLog("ﾒｯｾｰｼﾞﾌｫｰﾏｯﾄ異常", recvMsg);
                    return false;
                }

                // TODO:データ長チェック
                // TODO:BCCチェック

                return true;
            }

            return false;
        }
        enum MsgType
        {
            Poling,
            Selecting,
        }

        private MsgType GetMsgType(byte[] recvMsg)
        {
            Debug.Assert(recvMsg.Length > 4);

            // セレクティングなら4byte目がSTX
            if (recvMsg[3] == 0x02)
            {
                return MsgType.Selecting;
            }
            else
            {
                return MsgType.Poling;
            }
        }

        private byte[] GetSelectingData(byte[] recvMsg)
        {
            var fromStx = recvMsg.SkipWhile((v) => v != 0x02);
            var nextStx = fromStx.Skip(1);
            var fromData = nextStx.Skip(2);
            var dataRange = fromData.TakeWhile((v) => v != 0x03);

            return dataRange.ToArray();
        }

        /// <summary>
        /// ACK送信
        /// </summary>
        private void SendAck()
        {
            if (!this.stopCommunication.Checked)
            {
                byte[] sendMsg = { 0x06 };
                Enqueue(sendMsg);
            }
        }

        /// <summary>
        /// エコーバック返信
        /// </summary>
        /// <param name="msg"></param>
        private void sendEchoBack(byte[] recvMsg)
        {
             Enqueue(recvMsg);
        }

        /// <summary>
        /// NAK
        /// </summary>
        private void SendNak()
        {
            if (!this.hcflg1_2.Checked)
            {
                byte[] sendMsg = { 0x15 };
                Enqueue(sendMsg);
            }
        }

        private byte[] SetHeader(IEnumerable<byte> data)
        {
            List<byte> msg = new List<byte>();
            msg.Add(0x02);
            msg.AddRange(data);
            msg.Add(0x03);

            var bccTarget = msg.Skip(1).Take(msg.Count - 1);
            byte bccValue = 0;
            foreach (byte bin in bccTarget)
            {
                bccValue ^= bin;
            }

            msg.Add(bccValue);

            return msg.ToArray();
        }

        #endregion

        #region メソッド

        private void AddSendLog(byte[] log)
        {
            // ACK or NAK (セレクティングへの応答)
            if (log.Length == 1)
            {
                AddLog(string.Format("SND[S-{0}] {1}", log[0] == 0x06 ? "ACK" : "NAK", ByteArrayToString(log)));
            }
            // それ以外はポーリングへの応答とする
            else
            {
                Debug.Assert(log.Length >= (1 + 2));

                string command = AsciiUtils.ToString(log, 1, 2);

                AddLog(string.Format("SND[P-{0} ] {1}", command, ByteArrayToString(log)));
            }
        }

        private void AddRecvLog(byte[] log)
        {
            if (GetMsgType(log) == MsgType.Poling)
            {
                Debug.Assert(log.Length >= (1 + 2 + 2));

                string command = AsciiUtils.ToString(log, 1 + 2, 2);
                AddLog(string.Format("RCV[P-{0} ] {1}", command, ByteArrayToString(log)));
            }
            else if (GetMsgType(log) == MsgType.Selecting)
            {
                Debug.Assert(log.Length >= (1 + 2 + 1 + 2));

                string command = AsciiUtils.ToString(log, 1 + 2 + 1, 2);
                AddLog(string.Format("RCV[S-{0} ] {1}", command, ByteArrayToString(log)));
            }
            else
            {
                Debug.Assert(false);
                AddLog(string.Format("RCV {0}", ByteArrayToString(log)));
            }
        }

        private void AddLog(string message, byte[] log)
        {
            AddLog(string.Format("{0} {1}", message, ByteArrayToString(log)));
        }

        private void AddLog(string message)
        {
            DateTime dtNow = DateTime.Now;
            string time = string.Format("{0:D2}:{1:D2}:{2:D2}.{3:D3}", dtNow.Hour, dtNow.Minute, dtNow.Second, dtNow.Millisecond);
                        
            int length = message.Length;
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
        /// <para name="sourve"></param>
        /// <returns>< /returns>
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
            string[] strHex = new string[source.Length];

            for (int i = 0; i < source.Length; i++)
            {
                strHex[i] = string.Format("{0:X2}", source[i]);
            }

            return string.Join(" ", strHex);
        }

        #endregion
        #region ﾌﾟﾛﾊﾟﾃｨ
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
        public double _NowTemparature
        {
            get
            {
                return _Status.measuredTemparature;
            }
            set
            {
                _Status.measuredTemparature = value;
                ushort integerNowTemp = (ushort)Math.Round(_NowTemparature * 100);
                base.SetDeviceData(this.NowTemperature.Address, integerNowTemp);
            }
        }
        #endregion

        #region クラス・定義
        //温調器設定
        public class ThermoSetting
        {
            public double TargetTemperature { get; set; }    //目標温度
            public byte nodeNum { get; set; }                // node番号
            public byte chNo { get; set; }                   // CH番号
            public double targetP { get; set; }              // P(比例帯)
            public int targetI { get; set; }                 // I(積分時間)
            public int targetD { get; set; }                 // D(微分時間)
            public double adjust { get; set; }               // 温度表示補正
            public double upper { get; set; }                // UPPER(上限温度幅)
            public double lower { get; set; }                // LOWER(下限温度幅)

            public ThermoSetting()
            {
                this.nodeNum = 0x20;
                this.chNo = 0x20;
            }

        }

        //温調器ステータス
        public class ThermoStatus
        {
            public double measuredTemparature { get; set; }
            public bool hcflg0 { get; set; }        // .0 : オーバーヒート検出
            public bool hcflg1 { get; set; }        // .1 : アンダーヒート検出
            public bool hcflg2 { get; set; }        // .2 : バーンアウト検出
            public bool hcflg3 { get; set; }        // .3 : 温度安定
            public bool hcflg4 { get; set; }        // .4 : 温度異常
            public bool hcflg5 { get; set; }        // .5 : XXXX 
            public bool hcflg6 { get; set; }        // .6 : オートチューニング中
            public bool hcflg7 { get; set; }        // .6 : オートチューニング完了
            public string almNumber { get; set; }   // 警報番号

            public ThermoStatus()
            {
                this.almNumber = " ";
            }
        }

        #endregion  

        #region ﾌｫｰﾑｲﾍﾞﾝﾄ
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
        #endregion


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
 
    }
}

// EOF