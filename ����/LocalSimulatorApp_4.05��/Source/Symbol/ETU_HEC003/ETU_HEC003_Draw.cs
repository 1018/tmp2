/* <更新履歴>
 * 2012/??/??   作成
 * 2012/06/07   奇数Byteのメッセージを正常に受信しない不具合を修正
 *              ログフォームにシンボル名を表示するように変更
 * 2012/06/19   動作不安定な為、全面的に書き換え
 * 2012/06/21   RRメッセージの応答で、既にBCDのデータをBCD変換していた不具合を修正
 * 
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using CommonClassLibrary;

namespace SymbolLibrary
{
    using SplitterDictionary = Dictionary<string, Action<byte[]>>;


    public partial class ETU_HEC003_Draw : Symbol_Draw
    {
        #region Constructor

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ETU_HEC003_Draw()
        {
            InitializeComponent();

            base.CyclicFlag = true;

            base.SymbolType = "ETUコントローラ(HEC003)";
            base.Category = "温調器";

            #region 初期化

            this.RecvMsgDevice = new DeviceFormat("KM3000", 512, IoType.In, SetType.Word);
            this.RecvNumDevice = new DeviceFormat("KM3600", 1, IoType.In, SetType.Word);
            this.RecvFlagDevice = new DeviceFormat("KM3601.1", 1, IoType.In, SetType.Bit);
            this.SendMsgDevice = new DeviceFormat("KM1000", 512, IoType.Io, SetType.Word);
            this.SendNumDevice = new DeviceFormat("KM1600", 1, IoType.Io, SetType.Word);
            this.SendFlagDevice = new DeviceFormat("KM1601.1", 1, IoType.Io, SetType.Bit);
            this.NodeNumber = 1;
            this.NowTemperature = new DeviceFormat("KM0", 1, IoType.Out, SetType.Word);
               
            this._IsByteCount = true;

            #endregion
        }

        #endregion


        const int       MAX_LOG = 100;
        const string    LOG_TEXT_COLUMN_NAME = "LoggingText";

        bool            _IsByteCount;

        Queue<byte[]>   _SendQueue = new Queue<byte[]>();

        ThermoSetting   _Setting = new ThermoSetting();
        ThermoStatus    _Status = new ThermoStatus();

        DataTable       _LogMessage = new DataTable();

        Log_Form        _LogForm = null;
        Alarm_Form      _AlarmForm = null;

        double          _nowTemperature;

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
                _SplitterDictionary.Add("WU", (msg) => SplitWUMsg(msg));
                _SplitterDictionary.Add("RU", (msg) => SplitRUMsg(msg));
                _SplitterDictionary.Add("W%", (msg) => SplitWPerMsg(msg));
                _SplitterDictionary.Add("R%", (msg) => SplitRPerMsg(msg));
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
        private void numNowTemperature_ValueChanged(object sender, EventArgs e)
        {
            _NowTemperature = (double)numNowTemperature.Value;
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
        /// [アラーム出力] Clickイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btAlarmOutput_Click(object sender, EventArgs e)
        {
            btAlarmOutput.Enabled = false;

            _AlarmForm = new Alarm_Form();
            _AlarmForm.Disposed += new EventHandler(AlarmForm_Disposed);
            _AlarmForm.bindThermoStatus.DataSource = _Status;

            _AlarmForm.Show();
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

        /// <summary>
        /// アラーム出力ウィンドウ Disposedイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void AlarmForm_Disposed(object sender, EventArgs e)
        {
            _AlarmForm = null;
            btAlarmOutput.Enabled = true;
        }

        #endregion


        /// <summary>
        /// Symbol_Draw.Initial オーバーライド
        /// </summary>
        public override void Initial()
        {
            InitSplitterDictionary();

            bindThermoStatus.DataSource = _Status;

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

            if (_AlarmForm != null)
            {
                _AlarmForm.Close();
            }
        }

        private void updateDisplay()
        {
            Action action = new Action(
                () =>
                {
                    //設定データを表示
                    tbControlMode.Text = GetControlModeText(_Setting.ControlMode);

                    tbTargetTemperature.Text = _Setting.TargetTemperature.ToString("0.00");
                    tbPB_Width.Text = _Setting.PB_Width.ToString("0.00");
                    tbI_Constant.Text = _Setting.I_Constant.ToString("0");
                    tbD_Constant.Text = _Setting.D_Constant.ToString("0");
                    tbOffsetValue.Text = _Setting.OffsetValue.ToString("0.00");
                    tbSensorProofread.Text = _Setting.SensorProofread.ToString("0.00");
                    tbMaxTemperature.Text = _Setting.MaxTemperature.ToString("0.0");
                    tbMinTemperature.Text = _Setting.MinTemperature.ToString("0.0");

                    //現在温度
                    numNowTemperature.Value = (decimal)_NowTemperature;
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
            int dataLen = msg[1 + 3];

            return msg.Skip(1 + 5).Take(dataLen).ToArray();
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
                SendAck(recvMsg);

                // メッセージコマンド
                string command = AsciiUtils.ToString(GetMsgData(recvMsg), 0, 2);

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
            byte[] dataArea = GetMsgData(recvMsg);

            _Setting.ControlMode = (ControlMode)dataArea[2];
        }

        //RM:制御モードの読み出し
        private void SplitRMMsg(byte[] recvMsg)
        {
            if (!this.cbNoCommunication.Checked)
            {
                List<byte> data = new List<byte>();
                data.AddRange(AsciiUtils.ToAscii("RM"));
                data.AddRange(BCDUtils.ToBCD((byte)_Setting.ControlMode, 1));            // 制御モード
                data.AddRange(BCDUtils.ToBCD(0, 1));            //Reserve1
                data.AddRange(BCDUtils.ToBCD(0, 1));            //Reserve2
                data.AddRange(BCDUtils.ToBCD(0, 1));            //Reserve3

                byte[] sendMsg = SetHeader(GetMsgNumber(recvMsg), data);
                Enqueue(sendMsg);
            }
        }

        //WS:目標温度の設定
        private void SplitWSMsg(byte[] recvMsg)
        {
            byte[] dataArea = GetMsgData(recvMsg);

            _Setting.TargetTemperature = (double)BCDUtils.ToInt(dataArea, 2, 2) / 10;

            //現在温度を目標温度に
            if (!this.cbNoHoming.Checked)
            {
                _NowTemperature = _Setting.TargetTemperature;
            }
        }

        //RS:目標温度の読み出し
        private void SplitRSMsg(byte[] recvMsg)
        {
            if (!this.cbNoCommunication.Checked)
            {
                int targetTemp = (int)Math.Round(_Setting.TargetTemperature * 100);

                List<byte> data = new List<byte>();
                data.AddRange(AsciiUtils.ToAscii("RS"));
                data.AddRange(BCDUtils.ToBCD(targetTemp, 2));   // 目標温度
                data.AddRange(BCDUtils.ToBCD(0, 2));            // Reserve1
                data.AddRange(BCDUtils.ToBCD(0, 2));            // Reserve2
                data.AddRange(BCDUtils.ToBCD(0, 2));            // Reserve3

                byte[] sendMsg = SetHeader(GetMsgNumber(recvMsg), data);
                Enqueue(sendMsg);
            }
        }

        //RX:内部センサ及び外部センサ測定温度の読み出し
        private void SplitRXMsg(byte[] recvMsg)
        {
            if (!this.cbNoCommunication.Checked)
            {
                int nowTemp = (int)Math.Round(_NowTemperature * 100);

                List<byte> data = new List<byte>();
                data.AddRange(AsciiUtils.ToAscii("RX"));
                data.AddRange(BCDUtils.ToBCD(nowTemp, 2));      // 現在温度
                data.AddRange(BCDUtils.ToBCD(0, 2));            // Reserve1
                data.AddRange(BCDUtils.ToBCD(0, 2));            // Reserve2
                data.AddRange(BCDUtils.ToBCD(0, 2));            // Reserve3

                byte[] sendMsg = SetHeader(GetMsgNumber(recvMsg), data);
                Enqueue(sendMsg);
            }
        }

        //WB:PID定数及びオフセット値の設定
        private void SplitWBMsg(byte[] recvMsg)
        {
            byte[] dataArea = GetMsgData(recvMsg);

            _Setting.PB_Width = (double)BCDUtils.ToInt(dataArea, 2, 2) / 100;
            _Setting.I_Constant = BCDUtils.ToInt(dataArea, 4, 2);
            _Setting.D_Constant = BCDUtils.ToInt(dataArea, 6, 2);
            _Setting.OffsetValue = (double)BCDUtils.ToInt(dataArea, 8, 2, true) / 100;
        }

        //RB:PID定数及びオフセット値の読み出し
        private void SplitRBMsg(byte[] recvMsg)
        {
            if (!this.cbNoCommunication.Checked)
            {
                int pbValue = (int)Math.Round(_Setting.PB_Width * 100);
                int iValue = _Setting.I_Constant;
                int dValue = _Setting.D_Constant;
                int offset = (int)Math.Round(_Setting.OffsetValue * 100);

                List<byte> data = new List<byte>();
                data.AddRange(AsciiUtils.ToAscii("RB"));
                data.AddRange(BCDUtils.ToBCD(pbValue, 2));
                data.AddRange(BCDUtils.ToBCD(iValue, 2));
                data.AddRange(BCDUtils.ToBCD(dValue, 2));
                data.AddRange(BCDUtils.ToBCD(offset, 2, true));
                data.AddRange(BCDUtils.ToBCD(0, 8));            //Reserve1
                data.AddRange(BCDUtils.ToBCD(0, 8));            //Reserve2
                data.AddRange(BCDUtils.ToBCD(0, 8));            //Reserve3

                byte[] sendMsg = SetHeader(GetMsgNumber(recvMsg), data);
                Enqueue(sendMsg);
            }
        }

        //WU:制御用センサ及び外部センサ微調整値の設定
        private void SplitWUMsg(byte[] recvMsg)
        {
            byte[] dataArea = GetMsgData(recvMsg);

            _Setting.SensorProofread = (double)BCDUtils.ToInt(dataArea, 2, 2, true) / 100;
        }

        //RU:制御用センサ及び外部センサ微調整値の読み出し
        private void SplitRUMsg(byte[] recvMsg)
        {
            if (!this.cbNoCommunication.Checked)
            {
                int proofread = (int)Math.Round(_Setting.SensorProofread * 100);

                List<byte> data = new List<byte>();
                data.AddRange(AsciiUtils.ToAscii("RU"));
                data.AddRange(BCDUtils.ToBCD(proofread, 2, true));
                data.AddRange(BCDUtils.ToBCD(0, 2));
                data.AddRange(BCDUtils.ToBCD(0, 4));            //Reserve1
                data.AddRange(BCDUtils.ToBCD(0, 4));            //Reserve2
                data.AddRange(BCDUtils.ToBCD(0, 4));            //Reserve3

                byte[] sendMsg = SetHeader(GetMsgNumber(recvMsg), data);
                Enqueue(sendMsg);
            }
        }

        //W%:温度上下限温度の設定
        private void SplitWPerMsg(byte[] recvMsg)
        {
            byte[] dataArea = GetMsgData(recvMsg);

            _Setting.MaxTemperature = (double)BCDUtils.ToInt(dataArea, 2, 2) / 10;
            _Setting.MinTemperature = (double)BCDUtils.ToInt(dataArea, 4, 2) / 10;
        }

        //R%:温度上下限温度の読み出し
        private void SplitRPerMsg(byte[] recvMsg)
        {
            if (!this.cbNoCommunication.Checked)
            {
                int max = (int)Math.Round(_Setting.MaxTemperature * 10);
                int min = (int)Math.Round(_Setting.MinTemperature * 10);

                List<byte> data = new List<byte>();
                data.AddRange(AsciiUtils.ToAscii("R%"));
                data.AddRange(BCDUtils.ToBCD(max, 2));
                data.AddRange(BCDUtils.ToBCD(min, 2));
                data.AddRange(BCDUtils.ToBCD(0, 4));            //Reserve1
                data.AddRange(BCDUtils.ToBCD(0, 4));            //Reserve2
                data.AddRange(BCDUtils.ToBCD(0, 4));            //Reserve3

                byte[] sendMsg = SetHeader(GetMsgNumber(recvMsg), data);
                Enqueue(sendMsg);
            }
        }

        //RR:ステータスの読み出し
        private void SplitRRMsg(byte[] recvMsg)
        {
            if (!this.cbNoCommunication.Checked)
            {
                List<byte> data = new List<byte>();
                data.AddRange(AsciiUtils.ToAscii("RR"));
                data.AddRange(_Status.GetDataStatus());
                data.AddRange(BCDUtils.ToBCD(0, 2));            //Reserve1
                data.AddRange(BCDUtils.ToBCD(0, 2));            //Reserve2
                data.AddRange(BCDUtils.ToBCD(0, 2));            //Reserve3

                byte[] sendMsg = SetHeader(GetMsgNumber(recvMsg), data);
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
            if (('0' <= recvMsg[2] && recvMsg[2] <= '9') &&
                ('0' <= recvMsg[3] && recvMsg[3] <= '9'))
            {
                if (AsciiUtils.ToInt(recvMsg, 2, 2) == this.NodeNumber)
                {
                    return true;
                }
            }

            return false;
        }

        private bool CheckEnableMsg(byte[] recvMsg)
        {
            int dataLength = recvMsg[4];

            // メッセージ長チェック
            if (recvMsg.Length != (1 + 5 + dataLength + 1 + 2))
            {
                AddLog("メッセージ長異常", recvMsg);
                return false;
            }

            // パリティチェック
            var bccTarget = recvMsg.Skip(1).Take(5 + dataLength);
            int bccValue = bccTarget.Sum((b) => b) & 0xFF;

            int offsetBcc = 1 + 5 + dataLength + 1;

            int msgBcc = (AsciiUtils.ToInt(recvMsg, offsetBcc + 0, 1, true) << 4) +
                         (AsciiUtils.ToInt(recvMsg, offsetBcc + 1, 1, true) << 0);

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

        /// <summary>
        /// ACK送信
        /// </summary>
        /// <param name="msg"></param>
        private void SendAck(byte[] recvMsg)
        {
            Debug.Assert(recvMsg.Length >= 1 + 5);

            if (!this.cbNoCommunication.Checked)
            {
                int msgNumber = recvMsg[1 + 4];

                byte[] sendMsg = SetHeader(msgNumber, new byte[] { });
                Enqueue(sendMsg);
            }
        }

        private byte[] SetHeader(int msgNum, IEnumerable<byte> data)
        {
            const byte STX = 0x02;
            const byte ETX = 0x03;
            const byte DEVICE_CODE = (byte)'E';

            List<byte> msg = new List<byte>();
            msg.Add(STX);
            msg.Add(DEVICE_CODE);
            msg.AddRange(AsciiUtils.ToAscii(NodeNumber, 2));
            msg.Add((byte)data.Count());
            msg.Add((byte)(msgNum & 0xFF));
            msg.AddRange(data);
            msg.Add(ETX);

            var bccTarget = msg.Skip(1).Take(msg.Count - (1 + 1));  // メッセージ長 - (STX + ETX)
            int bccValue = bccTarget.Sum((b) => b) & 0xFF;

            msg.AddRange(AsciiUtils.ToAscii(bccValue, 2));

            return msg.ToArray();
        }


        #endregion

        #region メソッド

        private void AddSendLog(byte[] log)
        {
            Debug.Assert(log.Length >= (1 + 5 + 2));

            // ACK送信時
            if (GetMsgData(log).Length == 0)
            {
                AddLog(string.Format("SND[ACK] {0}", ByteArrayToString(log)));
            }
            else
            {
                string command = AsciiUtils.ToString(log, 1 + 5, 2);

                AddLog(string.Format("SND[{0,-3}] {1}", command, ByteArrayToString(log)));
            }
        }

        private void AddRecvLog(byte[] log)
        {
            Debug.Assert(log.Length >= (1 + 5 + 2));

            string command = AsciiUtils.ToString(log, 1 + 5, 2);

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

            for(int i = 0; i < sourceList.Count; i+=2)
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

        //new private void SetDeviceData(string address, ushort value)
        //{
        //    SetDeviceData(address, new ushort[] { value });
        //}

        //new private void SetDeviceData(string address, ushort[] values)
        //{
        //    Global.LogManager.Write(string.Format("{0} = {1}", address, string.Join(" ", values.Select((v) => v.ToString("X4")).ToArray())));
        //    base.SetDeviceData(address, values);
        //}

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
        }


        public interface IEntity : INotifyPropertyChanged
        {
            void NotifyPropertyChanged(String propertyName);
        }

        //温調器ステータス
        public class ThermoStatus
        {
            private class BitAttribute : Attribute
            {
                public BitAttribute(int bit)
                {
                    _Bit = bit;
                }

                int _Bit;
                public int Bit { get { return _Bit; } }
            }

            [Bit(15)]
            public bool Heating { get; set; }                    //加熱中
            [Bit(14)]
            public bool Cooling { get; set; }                    //冷却中
            [Bit(13)]
            public bool ControlOn { get; set; }                  //制御ON
            [Bit(12)]
            public bool AT_Error { get; set; }                   //AT異常
            [Bit(11)]
            public bool DC_Error { get; set; }                   //DC電源異常
            [Bit(10)]
            public bool InSensorUpperError { get; set; }         //内部センサ異常高温
            [Bit(9)]
            public bool ControlSensorError { get; set; }         //制御用センサ断線、短絡アラーム
            [Bit(8)]
            public bool ThermoStatAlarm { get; set; }            //サーモスタットアラーム
            [Bit(7)]
            public bool OutputErrorAlarm { get; set; }           //出力異常アラーム
            [Bit(6)]
            public bool FlowSwitchAlarm { get; set; }            //フロースイッチアラーム
            [Bit(5)]
            public bool TankLevelAlarm { get; set; }             //タンクレベル低下アラーム
            [Bit(4)]
            public bool InSensorUnderError { get; set; }         //内部センサ異常低温
            [Bit(3)]
            public bool OutSensorError { get; set; }             //外部センサ断線、短絡アラーム
            [Bit(1)]
            public bool TemperatureUpperAlarm { get; set; }      //温度上限アラーム
            [Bit(0)]
            public bool TemperatureUnderAlarm { get; set; }      //温度下限アラーム

            ///// <summary>
            ///// 現在ステータスを2byteデータで取得
            ///// </summary>
            ///// <returns></returns>
            //public ushort GetDataStatus()
            //{
            //    ushort statusData = 0;

            //    foreach (PropertyDescriptor pd in TypeDescriptor.GetProperties(this))
            //    {
            //        var bitAttrs = pd.Attributes.OfType<BitAttribute>();

            //        if (bitAttrs.Count() != 0)
            //        {
            //            BitAttribute attr = bitAttrs.First();

            //            if ((bool)pd.GetValue(this))
            //            {
            //                statusData |= (ushort)(0x01 << attr.Bit);
            //            }
            //        }
            //    }

            //    return statusData;
            //}

            public byte[] GetDataStatus()
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

                return new byte[] { (byte)((statusData & 0xFF00) >> 8), (byte)(statusData & 0xFF) };
            }
        }
        #endregion
    }
}
