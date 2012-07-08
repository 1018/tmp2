/* <更新履歴>
 * 20??/??/??   作成
 * 2012/06/19   動作不安定な為、全面的に書き換え
 * 
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using CommonClassLibrary;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace SymbolLibrary
{
    public partial class MelsecSerialUnit_Draw : Symbol_Draw
    {
        private int _UnitNum;

        #region シリアル通信ユニット バッファメモリ

        private const int CH1_MODE_CHANGE_PROTCOL = 0x90;
        private const int CH1_MODE_CHANGE_SETUP = 0x91;
        private const int CH1_WORD_BYTE_SETUP = 0x96;
        private const int CH1_RECV_END_TIME = 0x9C;
        private const int CH1_RECV_BUF_CLEAR = 0xA8;
        private const int CH1_SEND_DATA_COUNT = 0x400;
        private const int CH1_SEND_DATA_AREA = 0x401;
        private const int CH1_RECV_DATA_COUNT = 0x600;
        private const int CH1_RECV_DATA_AREA = 0x601;

        private const int CH2_MODE_CHANGE_PROTCOL = 0x130;
        private const int CH2_MODE_CHANGE_SETUP = 0x131;
        private const int CH2_WORD_BYTE_SETUP = 0x136;
        private const int CH2_RECV_END_TIME = 0x13C;
        private const int CH2_RECV_BUF_CLEAR = 0x148;
        private const int CH2_SEND_DATA_COUNT = 0x800;
        private const int CH2_SEND_DATA_AREA = 0x801;
        private const int CH2_RECV_DATA_COUNT = 0xA00;
        private const int CH2_RECV_DATA_AREA = 0xA01;
        private const int SEND_AREA_SIZE = 0x200 - 0x1;
        private const int RECV_AREA_SIZE = 0x200 - 0x1;

        #endregion

        private const ushort ON = 1;
        private const ushort OFF = 0;
        private string logHeader;

        private ushort[] beforeY;
        private bool[] updateRecvFlag = new bool[2];
        private ushort[] beforeRecvbufClear = new ushort[2];
        private bool initial;

        ushort[] oldDvInFlag1 = null;
        ushort[] oldDvInFlag2 = null;


        public class SerialInOut
        {
            public DeviceFormat Data;
            public DeviceFormat Count;
            public DeviceFormat Flag;
        }

        private SerialInOut[] _DvIn = new SerialInOut[2];
        private SerialInOut[] _DvOut = new SerialInOut[2];

        private delegate void ValueChangedDelegate(ValueChangedEventArgs e);

        private ValueChangedDelegate ValueChangedY;
        private ValueChangedDelegate ValueChangedRecvBufClear;
        private ValueChangedDelegate ValueChangedRecvFlag;


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MelsecSerialUnit_Draw()
        {
            InitializeComponent();
            base.SymbolType = "シリアルユニット";
            base.Category = "PLCユニット";
            base.HeightStretch = false;
            base.WidthStretch = false;

            DvX = new DeviceFormat("X0", 32, IoType.Io, SetType.Bit);
            DvY = new DeviceFormat("Y0", 32, IoType.In, SetType.Bit);

            beforeY = new ushort[32];

            DvRecvBufClear = CreateInstanceArray<DeviceFormat>(2);
            DvModeChangeProtcol = CreateInstanceArray<DeviceFormat>(2);
            DvModeChangeSetup = CreateInstanceArray<DeviceFormat>(2);
            DvWordByteSetup = CreateInstanceArray<DeviceFormat>(2);
            DvSendDataCount = CreateInstanceArray<DeviceFormat>(2);
            DvSendDataArea = CreateInstanceArray<DeviceFormat>(2);
            DvRecvDataCount = CreateInstanceArray<DeviceFormat>(2);
            DvRecvDataArea = CreateInstanceArray<DeviceFormat>(2);
            DvRecvEndTime = CreateInstanceArray<DeviceFormat>(2);

            _DvIn[0] = new SerialInOut();
            _DvIn[0].Data = new DeviceFormat("KM0", RECV_AREA_SIZE, IoType.Io, SetType.Word);
            _DvIn[0].Count = new DeviceFormat("KM600", 1, IoType.Io, SetType.Word);
            _DvIn[0].Flag = new DeviceFormat("KM601.1", 1, IoType.Io, SetType.Bit);

            _DvIn[1] = new SerialInOut();
            _DvIn[1].Data = new DeviceFormat("KM1000", RECV_AREA_SIZE, IoType.Io, SetType.Word);
            _DvIn[1].Count = new DeviceFormat("KM1600", 1, IoType.Io, SetType.Word);
            _DvIn[1].Flag = new DeviceFormat("KM1601.1", 1, IoType.Io, SetType.Bit);

            _DvOut[0] = new SerialInOut();
            _DvOut[0].Data = new DeviceFormat("KM2000", SEND_AREA_SIZE, IoType.Io, SetType.Word);
            _DvOut[0].Count = new DeviceFormat("KM2600", 1, IoType.Io, SetType.Word);
            _DvOut[0].Flag = new DeviceFormat("KM2601.1", 1, IoType.Io, SetType.Bit);

            _DvOut[1] = new SerialInOut();
            _DvOut[1].Data = new DeviceFormat("KM3000", SEND_AREA_SIZE, IoType.Io, SetType.Word);
            _DvOut[1].Count = new DeviceFormat("KM3600", 1, IoType.Io, SetType.Word);
            _DvOut[1].Flag = new DeviceFormat("KM3601.1", 1, IoType.Io, SetType.Bit);

            this.lampCh1NEU.BackColor = Color.Yellow;
            this.lampCh2NEU.BackColor = Color.Yellow;

            base.CyclicFlag = true;

        }
        /// <summary>
        /// MainProgram初回起動
        /// </summary>
        public override void Initial()
        {
            string prefix = "U" + Convert.ToString(UnitNumber, 16).PadLeft(2, '0') + "G";

            logHeader = "QJ71C24N U:" + Convert.ToString(UnitNumber, 16).PadLeft(2, '0') + " ";

            //関係するデバイスを初期設定する。

            DvX = new DeviceFormat("X" + Convert.ToString(UnitNumber, 16) + "0", 32, IoType.Io, SetType.Bit);
            DvY = new DeviceFormat("Y" + Convert.ToString(UnitNumber, 16) + "0", 32, IoType.In, SetType.Bit);

            DvRecvBufClear[0] = new DeviceFormat(prefix + CH1_RECV_BUF_CLEAR.ToString(), 1, IoType.Io, SetType.Word);
            DvRecvBufClear[1] = new DeviceFormat(prefix + CH2_RECV_BUF_CLEAR.ToString(), 1, IoType.Io, SetType.Word);

            DvModeChangeProtcol[0] = new DeviceFormat(prefix + CH1_MODE_CHANGE_PROTCOL.ToString(), 1, IoType.Io, SetType.Word);
            DvModeChangeProtcol[1] = new DeviceFormat(prefix + CH2_MODE_CHANGE_PROTCOL.ToString(), 1, IoType.Io, SetType.Word);

            DvModeChangeSetup[0] = new DeviceFormat(prefix + CH1_MODE_CHANGE_SETUP.ToString(), 1, IoType.Io, SetType.Word);
            DvModeChangeSetup[1] = new DeviceFormat(prefix + CH2_MODE_CHANGE_SETUP.ToString(), 1, IoType.Io, SetType.Word);

            DvWordByteSetup[0] = new DeviceFormat(prefix + CH1_WORD_BYTE_SETUP.ToString(), 1, IoType.Io, SetType.Word);
            DvWordByteSetup[1] = new DeviceFormat(prefix + CH2_WORD_BYTE_SETUP.ToString(), 1, IoType.Io, SetType.Word);

            DvSendDataCount[0] = new DeviceFormat(prefix + CH1_SEND_DATA_COUNT.ToString(), 1, IoType.Io, SetType.Word);
            DvSendDataCount[1] = new DeviceFormat(prefix + CH2_SEND_DATA_COUNT.ToString(), 1, IoType.Io, SetType.Word);

            DvSendDataArea[0] = new DeviceFormat(prefix + CH1_SEND_DATA_AREA.ToString(), SEND_AREA_SIZE, IoType.Io, SetType.Word);
            DvSendDataArea[1] = new DeviceFormat(prefix + CH2_SEND_DATA_AREA.ToString(), SEND_AREA_SIZE, IoType.Io, SetType.Word);

            DvRecvDataCount[0] = new DeviceFormat(prefix + CH1_RECV_DATA_COUNT.ToString(), 1, IoType.Io, SetType.Word);
            DvRecvDataCount[1] = new DeviceFormat(prefix + CH2_RECV_DATA_COUNT.ToString(), 1, IoType.Io, SetType.Word);

            DvRecvDataArea[0] = new DeviceFormat(prefix + CH1_RECV_DATA_AREA.ToString(), RECV_AREA_SIZE, IoType.Io, SetType.Word);
            DvRecvDataArea[1] = new DeviceFormat(prefix + CH2_RECV_DATA_AREA.ToString(), RECV_AREA_SIZE, IoType.Io, SetType.Word);

            DvRecvEndTime[0] = new DeviceFormat(prefix + CH1_RECV_END_TIME.ToString(), 0x1, IoType.Io, SetType.Word);
            DvRecvEndTime[1] = new DeviceFormat(prefix + CH2_RECV_END_TIME.ToString(), 0x1, IoType.Io, SetType.Word);

            this.ValueChangedY = MelsecSerialUnit_ValueChangedY;
            this.ValueChangedRecvBufClear = MelsecSerialUnit_ValueChangedRecvBufClear;
            this.ValueChangedRecvFlag = MelsecSerialUnit_ValueChangedRecvFlag;

            DataListMake();

            initial = false;
        }
        /// <summary>
        /// MainProgram定周期処理
        /// </summary>
        public override void CyclicMethod()
        {
            InitialSetReady();

            CheckUpdatePlcOutput();

            CheckRecvCh1();

            CheckRecvCh2();

            CheckBufClearCh1();

            CheckBufClearCh2();
        }
        /// <summary>
        /// MainProgram起動時 レディ信号ON
        /// </summary>
        private void InitialSetReady()
        {
            if (!initial)
            {
                //シリアルユニットレディ信号
                SetDeviceData(DeviceManager.Offset(DvX.Address, 0x1E), ON);

                this.lampRUN.BackColor = Color.Yellow;
                this.txtCh1Status.Text = "Initialized!";
                this.txtCh2Status.Text = "Initialized!";

                WriteLog("初期化完了");

                initial = true;
            }
        }
        /// <summary>
        /// Yデバイス更新監視
        /// </summary>
        private void CheckUpdatePlcOutput()
        {
            for (int i = 0; i < DvY.Value.Length; i++)
            {
                if (DvY.Value[i] != beforeY[i])
                {
                    WriteLog("Yデバイス状変発生 Address:" +
                        Convert.ToString(i, 16).PadLeft(2, '0') + " 値:" + DvY.Value[i].ToString());

                    ValueChangedEventArgs e = new ValueChangedEventArgs();
                    e.Address = i;
                    e.ChangedValue = DvY.Value[i];

                    if (InvokeRequired)
                    {
                        Invoke(ValueChangedY, e);
                    }
                    else
                    {
                        ValueChangedY(e);
                    }
                }
                beforeY[i] = DvY.Value[i];
            }
        }
        /// <summary>
        /// CH.1 「受信フラグ」更新監視
        /// </summary>
        private void CheckRecvCh1()
        {
            if (updateRecvFlag[0])
            {
                updateRecvFlag[0] = false;

                WriteLog("CH1 外部受信フラグ状変発生 " + DvInFlag1.Value[0].ToString());

                ValueChangedEventArgs e = new ValueChangedEventArgs();
                e.ChNum = 1;
                e.ChangedValue = DvInFlag1.Value[0];

                if (InvokeRequired)
                {
                    Invoke(ValueChangedRecvFlag, e);
                }
                else
                {
                    ValueChangedRecvFlag(e);
                }
            }
        }
        /// <summary>
        /// CH.2 「受信フラグ」更新監視
        /// </summary>
        private void CheckRecvCh2()
        {
            if (updateRecvFlag[1])
            {
                updateRecvFlag[1] = false;

                WriteLog("CH2 外部受信フラグ状変発生 " + DvInFlag2.Value[0].ToString());

                ValueChangedEventArgs e = new ValueChangedEventArgs();
                e.ChNum = 2;
                e.ChangedValue = DvInFlag2.Value[0];

                if (InvokeRequired)
                {
                    Invoke(ValueChangedRecvFlag, e);
                }
                else
                {
                    ValueChangedRecvFlag(e);
                }
            }
        }
        /// <summary>
        /// CH.1 「受信バッファクリア要求」更新監視
        /// </summary>
        private void CheckBufClearCh1()
        {
            if (DvRecvBufClear[0].Value[0] != beforeRecvbufClear[0])
            {
                ValueChangedEventArgs e = new ValueChangedEventArgs();
                e.ChNum = 1;
                e.ChangedValue = DvRecvBufClear[0].Value[0];

                if (InvokeRequired)
                {
                    Invoke(ValueChangedRecvBufClear, e);
                }
                else
                {
                    ValueChangedRecvBufClear(e);
                }
            }
            beforeRecvbufClear[0] = DvRecvBufClear[0].Value[0];
        }
        /// <summary>
        /// CH.2 「受信バッファクリア要求」更新監視
        /// </summary>
        private void CheckBufClearCh2()
        {
            if (DvRecvBufClear[1].Value[0] != beforeRecvbufClear[1])
            {
                ValueChangedEventArgs e = new ValueChangedEventArgs();
                e.ChNum = 2;
                e.ChangedValue = DvRecvBufClear[1].Value[0];

                if (InvokeRequired)
                {
                    Invoke(ValueChangedRecvBufClear, e);
                }
                else
                {
                    ValueChangedRecvBufClear(e);
                }
            }
            beforeRecvbufClear[1] = DvRecvBufClear[1].Value[0];
        }

        /// <summary>
        /// Yデバイス 更新時発生イベント
        /// </summary>
        /// <param name="e"></param>
        private void MelsecSerialUnit_ValueChangedY(ValueChangedEventArgs e)
        {
            int offset = e.Address;
            int changedValue = e.ChangedValue;

            switch (offset)
            {
                case 0:     //CH1送信要求
                    SendRequest(offset, changedValue, 1);
                    break;

                case 1:     //CH1受信読み出し完了
                    RecvComp(offset, changedValue, 1);
                    break;

                case 2:     //CH1モードチェンジ要求
                    ModeChangeRequest(offset, changedValue, 1);
                    break;

                case 7:     //CH2送信要求
                    SendRequest(offset, changedValue, 2);
                    break;

                case 8:     //CH2受信読み出し完了
                    RecvComp(offset, changedValue, 2);
                    break;

                case 9:     //CH2モードチェンジ要求
                    ModeChangeRequest(offset, changedValue, 2);
                    break;
            }
        }
        /// <summary>
        /// Yデバイス 「送信要求」更新時発生イベント
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="changedValue"></param>
        /// <param name="chNum"></param>
        private void SendRequest(int offset, int changedValue, int chNum)
        {
            if (changedValue == ON)     //OFF → ON
            {
                if (chNum == 1)
                {
                    this.lampCh1SD.BackColor = Color.Yellow;
                }
                else
                {
                    this.lampCh2SD.BackColor = Color.Yellow;
                }

                Thread transferThread = new Thread(
                    new ParameterizedThreadStart(PlcToNdeThread));

                transferThread.Start(chNum);
            }
            else                        //ON → OFF
            {
                //送信正常完了フラグOFF                    
                SetDeviceData(DeviceManager.Offset(DvX.Address, 0 + (chNum - 1) * 7), OFF);

                if (chNum == 1)
                {
                    this.lampCh1SD.BackColor = Color.LightGray;
                }
                else
                {
                    this.lampCh2SD.BackColor = Color.LightGray;
                }
            }
        }
        /// <summary>
        /// Yデバイス 「受信読み出し完了」更新時発生イベント
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="changedValue"></param>
        /// <param name="chNum"></param>
        private void RecvComp(int offset, int changedValue, int chNum)
        {
            if (changedValue == ON)
            {
                //受信読み出し要求OFF
                SetDeviceData(DeviceManager.Offset(DvX.Address, 3 + (chNum - 1) * 7), OFF);

                if (chNum == 1)
                {
                    this.lampCh1RD.BackColor = Color.LightGray;
                }
                else
                {
                    this.lampCh2RD.BackColor = Color.LightGray;
                }
            }
        }
        /// <summary>
        /// Yデバイス 「モードチェンジ要求」更新時発生イベント
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="changedValue"></param>
        /// <param name="chNum"></param>
        private void ModeChangeRequest(int offset, int changedValue, int chNum)
        {
            if (changedValue == ON)
            {
                //モード切替フラグ1秒ON
                SetDeviceData(DeviceManager.Offset(DvX.Address, 6 + (chNum - 1) * 7), ON);

                System.Threading.Thread.Sleep(1000);

                SetDeviceData(DeviceManager.Offset(DvX.Address, 6 + (chNum - 1) * 7), OFF);

                WriteLog("CH" + chNum.ToString() + " モードチェンジ完了");

            }
            else
            {
                SetDeviceData(DeviceManager.Offset(DvX.Address, 3 + (chNum - 1) * 7), OFF);
                SetDeviceData(DeviceManager.Offset(DvX.Address, 4 + (chNum - 1) * 7), OFF);
                SetDeviceData(DeviceManager.Offset(DvX.Address, 7 + (chNum - 1) * 7), OFF);
                SetDeviceData(DeviceManager.Offset(DvX.Address, 8 + (chNum - 1) * 7), OFF);

                if (chNum == 1)
                {
                    this.lampCh1NEU.BackColor = Color.LightGray;
                }
                else
                {
                    this.lampCh2NEU.BackColor = Color.LightGray;
                }
            }
        }

        /// <summary>
        /// CH. 受信フラグ更新時発生イベント
        /// </summary>
        /// <param name="e"></param>
        private void MelsecSerialUnit_ValueChangedRecvFlag(ValueChangedEventArgs e)
        {
            if (e.ChangedValue == ON)
            {
                if (e.ChNum == 1)
                {
                    this.lampCh1RD.BackColor = Color.Yellow;
                }
                else
                {
                    this.lampCh2RD.BackColor = Color.Yellow;
                }

                Thread transferThread = new Thread(
                    new ParameterizedThreadStart(NdeToPlcThread));

                transferThread.Start(e.ChNum);
            }
        }

        /// <summary>
        /// CH. 受信バッファクリア要求更新時イベント
        /// </summary>
        /// <param name="e"></param>
        private void MelsecSerialUnit_ValueChangedRecvBufClear(ValueChangedEventArgs e)
        {
            // TODO: 
        }

        /// <summary>
        /// CH.→バッファメモリ 転送スレッド
        /// </summary>
        /// <param name="argv"></param>
        private void NdeToPlcThread(object argv)
        {
            int chNum = (int)argv;
            string logHeader = string.Format("[NDE]CH.{0} ", chNum);

            WriteLog(logHeader + "送信要求確認");

            //送信要求がONしていても、
            //送信データがデバイスに書き終えていないことがある。
            //データが完全に書込まれるのを待つ。
            WaitForReadComplateNde(chNum);

            //PLCの受信バッファへの書き込みを確認するために、
            //受信バッファを 0 クリアしてからデータを書き込む。
            ushort[] emptyCount = new ushort[DvRecvDataCount[chNum - 1].DataCount];
            ushort[] emptyData = new ushort[DvRecvDataArea[chNum - 1].DataCount];

            ClearArrayAll(emptyCount);
            ClearArrayAll(emptyData);

            SetDeviceData(DvRecvDataCount[chNum - 1].Address, emptyCount);
            SetDeviceData(DvRecvDataArea[chNum - 1].Address, emptyData);

            WaitClearWriteAreaPlc(chNum);

            WriteLog(logHeader + "送信準備完了");

            //CH.からの送信データをPLCの受信バッファに書き込む
            ushort[] recvData = GetNdeRecvData(chNum);

            SetDeviceData(DvRecvDataCount[chNum - 1].Address, (ushort)GetNdeRecvDataCount(chNum));
            SetDeviceData(DvRecvDataArea[chNum - 1].Address, recvData);

            WaitForWriteCompletePlc(chNum);

            WriteLog(logHeader + "送信データセット DATA=" +
                string.Join(" ", recvData.Select((elem) => elem.ToString("X4")).ToArray()));

            //PLCの受信要求セット
            SetDeviceData(DeviceManager.Offset(DvX.Address, 3 + (chNum - 1) * 7), ON);
            DvX.Value[3 + (chNum - 1) * 7] = ON;

            WriteLog(logHeader + "受信要求セット");


            //送信が完了するのを待つ。
            //PLCがメッセージ受信を完了すると、受信読み出し完了通知(Y1,Y8)がONする。
            //完了通知がONするとPLCの受信要求フラグがOFFするようになっている(RecvComplete関数)ので、
            //このフラグがOFFするのを待つ。
            WaitForRecvCompletePlc(chNum);
            WriteLog(logHeader + "送信正常終了");

            //送信フラグOFF(CH.に送信完了通知)
            SetDeviceData(_DvIn[chNum - 1].Flag.Address, OFF);
            WriteLog(logHeader + "送信要求リセット");
        }
        /// <summary>
        /// バッファメモリ→CH. 転送スレッド
        /// </summary>
        /// <param name="argv"></param>
        private void PlcToNdeThread(object argv)
        {
            int chNum = (int)argv;
            string logHeader = string.Format("[PLC]CH.{0} ", chNum);

            WriteLog(logHeader + "送信要求確認");

            //送信要求がONしていても、
            //送信データがデバイスに書き終えていないことがある。
            //データが完全に書込まれるのを待つ。
            WaitForReadCompletePlc(chNum);

            //CH.の受信バッファへの書き込みを確認するために、
            //受信バッファを 0 クリアしてからデータを書き込む。
            ushort[] emptyCount = new ushort[_DvOut[chNum - 1].Count.DataCount];
            ushort[] emptyData = new ushort[_DvOut[chNum - 1].Data.DataCount];

            ClearArrayAll(emptyCount);
            ClearArrayAll(emptyData);

            SetDeviceData(_DvOut[chNum - 1].Count.Address, emptyCount);
            SetDeviceData(_DvOut[chNum - 1].Data.Address, emptyData);

            WaitClearWriteAreaNde(chNum);

            WriteLog(logHeader + "送信準備完了");

            //PLCからの送信データをCH.の受信バッファに書き込む。
            ushort[] recvData = GetPlcRecvData(chNum);

            SetDeviceData(_DvOut[chNum - 1].Count.Address, (ushort)GetPlcRecvDataCount(chNum));
            SetDeviceData(_DvOut[chNum - 1].Data.Address, recvData);

            WaitForWriteCompleteNde(chNum);

            WriteLog(logHeader + "送信データセット DATA=" +
                string.Join(" ", recvData.Select((elem) => elem.ToString("X4")).ToArray()));

            //CH.の受信要求セット
            SetDeviceData(_DvOut[chNum - 1].Flag.Address, ON);
            _DvOut[chNum - 1].Flag.Value[0] = ON;

            WriteLog(logHeader + "受信要求セット");


            //送信が完了するのを待つ。
            //子局がメッセージ受信を完了すると、受信要求フラグがOFFする。
            //このフラグがOFFするのを待つ。
            WaitForRecvCompleteNde(chNum);

            WriteLog(logHeader + "送信正常終了");

            //送信正常完了フラグON
            SetDeviceData(DeviceManager.Offset(DvX.Address, 0 + (chNum - 1) * 7), ON);

            WriteLog(logHeader + "送信正常完了セット");
        }

        /// <summary>
        /// CH.→バッファメモリ 転送元データ 書き込み完了待ち
        /// </summary>
        /// <param name="chNum"></param>
        private void WaitForReadComplateNde(int chNum)
        {
            while (_DvIn[chNum - 1].Count.Value.All((elem) => elem == 0x00))
            {
                System.Threading.Thread.Sleep(10);
            }
            while (_DvIn[chNum - 1].Data.Value.All((elem) => elem == 0x00))
            {
                System.Threading.Thread.Sleep(10);
            }
        }
        /// <summary>
        /// CH.→バッファメモリ 転送先データ バッファメモリ書き込み完了待ち
        /// </summary>
        /// <param name="chNum"></param>
        private void WaitForWriteCompletePlc(int chNum)
        {
            while (DvRecvDataCount[chNum - 1].Value.All((elem) => elem == 0x00))
            {
                System.Threading.Thread.Sleep(10);
            }
            while (DvRecvDataArea[chNum - 1].Value.All((elem) => elem == 0x00))
            {
                System.Threading.Thread.Sleep(10);
            }
        }
        /// <summary>
        /// バッファメモリ→CH. 転送元データ バッファメモリ書き込み完了待ち
        /// </summary>
        /// <param name="chNum"></param>
        private void WaitForReadCompletePlc(int chNum)
        {
            while (DvSendDataCount[chNum - 1].Value.All((elem) => elem == 0x00))
            {
                System.Threading.Thread.Sleep(10);
            }
            while (DvSendDataArea[chNum - 1].Value.All((elem) => elem == 0x00))
            {
                System.Threading.Thread.Sleep(10);
            }
        }
        /// <summary>
        /// バッファメモリ→CH. 転送先データ CH.書き込み完了待ち
        /// </summary>
        /// <param name="chNum"></param>
        private void WaitForWriteCompleteNde(int chNum)
        {
            while (_DvOut[chNum - 1].Count.Value.All((elem) => elem == 0x00))
            {
                System.Threading.Thread.Sleep(10);
            }
            while (_DvOut[chNum - 1].Data.Value.All((elem) => elem == 0x00))
            {
                System.Threading.Thread.Sleep(10);
            }
        }

        /// <summary>
        /// バッファメモリ→CH. 転送完了待ち
        /// </summary>
        /// <param name="chNum"></param>
        private void WaitForRecvCompleteNde(int chNum)
        {
            while (_DvOut[chNum - 1].Flag.Value[0] == ON)
            {
                System.Threading.Thread.Sleep(10);
            }
        }
        /// <summary>
        /// CH.→バッファメモリ 転送完了待ち
        /// </summary>
        /// <param name="chNum"></param>
        private void WaitForRecvCompletePlc(int chNum)
        {
            while (DvX.Value[3 + (chNum - 1) * 7] == ON)
            {
                System.Threading.Thread.Sleep(10);
            }
        }

        /// <summary>
        /// CH.→バッファメモリ バッファメモリクリア完了待ち
        /// </summary>
        /// <param name="chNum"></param>
        private void WaitClearWriteAreaPlc(int chNum)
        {
            while (DvRecvDataCount[chNum - 1].Value.Any((v) => v != 0))
            {
                Thread.Sleep(10);
            }

            while (DvRecvDataArea[chNum - 1].Value.Any((v) => v != 0))
            {
                Thread.Sleep(10);
            }
        }
        /// <summary>
        /// バッファメモリ→CH. CH.クリア完了待ち
        /// </summary>
        /// <param name="chNum"></param>
        private void WaitClearWriteAreaNde(int chNum)
        {
            while (_DvOut[chNum - 1].Count.Value.Any((v) => v != 0))
            {
                Thread.Sleep(10);
            }

            while (_DvOut[chNum - 1].Data.Value.Any((v) => v != 0))
            {
                Thread.Sleep(10);
            }
        }

        /// <summary>
        /// CH. 転送データ取得
        /// </summary>
        /// <param name="chNum"></param>
        /// <returns></returns>
        private ushort[] GetNdeRecvData(int chNum)
        {
            //受信数を取得する
            int dataCount = GetNdeRecvDataCount(chNum);

            int wordSize = dataCount / 2;
            wordSize += (dataCount % 2) != 0 ? 1 : 0;	//余りがあれば1加算

            //データを受信する
            ushort[] inArea = _DvIn[chNum - 1].Data.Value;
            return inArea.Take(wordSize).ToArray();
        }
        /// <summary>
        /// CH. 転送データ数取得
        /// </summary>
        /// <param name="chNum"></param>
        /// <returns></returns>
        private int GetNdeRecvDataCount(int chNum)
        {
            return _DvIn[chNum - 1].Count.Value[0];
        }

        /// <summary>
        /// バッファメモリ 転送データ取得
        /// </summary>
        /// <param name="chNum"></param>
        /// <returns></returns>
        private ushort[] GetPlcRecvData(int chNum)
        {
            // 受信数を取得する
            int dataCount = GetPlcRecvDataCount(chNum);

            int wordSize = dataCount / 2;
            wordSize += (dataCount % 2) != 0 ? 1 : 0;	//余りがあれば1加算

            //データを受信する
            ushort[] inArea = DvSendDataArea[chNum - 1].Value;
            return inArea.Take(wordSize).ToArray();
        }
        /// <summary>
        /// バッファメモリ 転送データ数取得
        /// </summary>
        /// <param name="chNum"></param>
        /// <returns></returns>
        private int GetPlcRecvDataCount(int chNum)
        {
            return DvSendDataCount[chNum - 1].Value[0];
        }

        /// <summary>
        /// 要素のインスタンスが存在する配列を生成
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="length"></param>
        /// <returns></returns>
        private T[] CreateInstanceArray<T>(int length)
        {
            if (typeof(T).GetConstructor(Type.EmptyTypes) == null) { throw new InvalidOperationException(); }

            Array result = Array.CreateInstance(typeof(T), length);

            var constructor = typeof(T).GetConstructor(Type.EmptyTypes);
            for (int i = 0; i < length; i++)
            {
                result.SetValue((T)constructor.Invoke(null), i);
            }

            return (T[])result;
        }
        /// <summary>
        /// 配列の全ての要素を初期化
        /// </summary>
        /// <param name="array"></param>
        private void ClearArrayAll(Array array)
        {
            Array.Clear(array, 0, array.Length);
        }

        /// <summary>
        /// MainProgram.logへのログ書き込み(デバッグ時のみ)
        /// </summary>
        /// <param name="message"></param>
        [Conditional("DEBUG")]
        private void WriteLog(string message)
        {
            Global.LogManager.Write(logHeader + message);
        }

        /// <summary>
        /// SetDeviceDataの隠蔽
        /// </summary>
        /// <param name="address"></param>
        /// <param name="values"></param>
        new private void SetDeviceData(string address, ushort[] values)
        {
            //SetDeviceDataにvaluesの参照を渡すと
            //都合が悪かったので、valuesのコピーを渡すようにする。

            WriteLog(
                string.Format("{0} = {1}", address,
                    string.Join(" ", values.Select((v) => v.ToString("X4")).ToArray())));

            base.SetDeviceData(address, values.ToArray());
        }


        //プロパティ
        [Category("01_設定")]
        [Visible, DisplayName("ユニット№")]
        public int UnitNumber
        {
            get
            {
                return _UnitNum;
            }
            set
            {
                _UnitNum = value;
                this.txtUnitNum.Text = _UnitNum.ToString();
            }
        }

        [Category("02_CH1設定")]
        [Visible, DisplayName("CH1入力エリア")]
        public DeviceFormat DvInData1
        {
            get { return _DvIn[0].Data; }
            set { _DvIn[0].Data = value; }
        }
        [Category("02_CH1設定")]
        [Visible, DisplayName("CH1入力数")]
        public DeviceFormat DvInCount1
        {
            get { return _DvIn[0].Count; }
            set { _DvIn[0].Count = value; }
        }
        [Category("02_CH1設定")]
        [Visible, DisplayName("CH1入力フラグ")]
        public DeviceFormat DvInFlag1
        {
            get { return _DvIn[0].Flag; }
            //set { _DvIn[0].Flag = value; }
            set
            {
                _DvIn[0].Flag = value;

                if (oldDvInFlag1 == null)
                {
                    updateRecvFlag[0] = true;
                }
                else if (!DvInFlag1.Value.SequenceEqual(oldDvInFlag1))
                {
                    updateRecvFlag[0] = true;
                }
                oldDvInFlag1 = DvInFlag1.Value.ToArray();
            }
        }

        [Category("03_CH2設定")]
        [Visible, DisplayName("CH2入力エリア")]
        public DeviceFormat DvInData2
        {
            get { return _DvIn[1].Data; }
            set { _DvIn[1].Data = value; }
        }
        [Category("03_CH2設定")]
        [Visible, DisplayName("CH2入力数")]
        public DeviceFormat DvInCount2
        {
            get { return _DvIn[1].Count; }
            set { _DvIn[1].Count = value; }
        }
        [Category("03_CH2設定")]
        [Visible, DisplayName("CH2入力フラグ")]
        public DeviceFormat DvInFlag2
        {
            get { return _DvIn[1].Flag; }
            //set { _DvIn[1].Flag = value; }
            set
            {
                _DvIn[1].Flag = value;

                if (oldDvInFlag2 == null)
                {
                    updateRecvFlag[1] = true;
                }
                else if (!DvInFlag2.Value.SequenceEqual(oldDvInFlag2))
                {
                    updateRecvFlag[1] = true;
                }
                oldDvInFlag2 = DvInFlag2.Value.ToArray();
            }
        }

        [Category("02_CH1設定")]
        [Visible, DisplayName("CH1出力エリア")]
        public DeviceFormat DvOutData1
        {
            get { return _DvOut[0].Data; }
            set { _DvOut[0].Data = value; }
        }
        [Category("02_CH1設定")]
        [Visible, DisplayName("CH1出力数")]
        public DeviceFormat DvOutCount1
        {
            get { return _DvOut[0].Count; }
            set { _DvOut[0].Count = value; }
        }
        [Category("02_CH1設定")]
        [Visible, DisplayName("CH1出力フラグ")]
        public DeviceFormat DvOutFlag1
        {
            get { return _DvOut[0].Flag; }
            set { _DvOut[0].Flag = value; }
        }

        [Category("03_CH2設定")]
        [Visible, DisplayName("CH2出力エリア")]
        public DeviceFormat DvOutData2
        {
            get { return _DvOut[1].Data; }
            set { _DvOut[1].Data = value; }
        }
        [Category("03_CH2設定")]
        [Visible, DisplayName("CH2出力数")]
        public DeviceFormat DvOutCount2
        {
            get { return _DvOut[1].Count; }
            set { _DvOut[1].Count = value; }
        }
        [Category("03_CH2設定")]
        [Visible, DisplayName("CH2出力フラグ")]
        public DeviceFormat DvOutFlag2
        {
            get { return _DvOut[1].Flag; }
            set { _DvOut[1].Flag = value; }
        }


        [Browsable(false), DisplayName("DvX")]
        public DeviceFormat DvX { get; set; }

        [Browsable(false), DisplayName("DvY")]
        public DeviceFormat DvY { get; set; }

        [Browsable(false), DisplayName("受信バッファクリア指令")]
        public DeviceFormat[] DvRecvBufClear { get; set; }

        [Browsable(false), DisplayName("モードチェンジ時プロトコル指定")]
        public DeviceFormat[] DvModeChangeProtcol { get; set; }

        [Browsable(false), DisplayName("モードチェンジ時設定")]
        public DeviceFormat[] DvModeChangeSetup { get; set; }

        [Browsable(false), DisplayName("ワード/バイト設定")]
        public DeviceFormat[] DvWordByteSetup { get; set; }

        [Browsable(false), DisplayName("送信バッファデータ数")]
        public DeviceFormat[] DvSendDataCount { get; set; }

        [Browsable(false), DisplayName("送信バッファエリア")]
        public DeviceFormat[] DvSendDataArea { get; set; }

        [Browsable(false), DisplayName("受信バッファ数")]
        public DeviceFormat[] DvRecvDataCount { get; set; }

        [Browsable(false), DisplayName("受信バッファエリア")]
        public DeviceFormat[] DvRecvDataArea { get; set; }

        [Browsable(false), DisplayName("受信タイムアウトバイト")]
        public DeviceFormat[] DvRecvEndTime { get; set; }

    }

    public class ValueChangedEventArgs : EventArgs
    {
        public int Address;
        public int ChNum;
        public ushort ChangedValue;
    }
}
