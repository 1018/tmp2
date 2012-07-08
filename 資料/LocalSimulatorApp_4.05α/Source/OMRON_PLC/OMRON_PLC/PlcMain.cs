using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Threading;
using System.Windows.Forms;
using CommonClassLibrary;
// FgwCompolet.dll
using OMRON.FinsGateway.NameSpace;
using KssClassLibrary;
using OMRON.FinsGateway.Messaging;

namespace OMRON_PLC
{
    [Serializable]
    public class PlcCommunication : IPlcCommunication
    {
        Remoting CommonMemory;

        public static LogManager logManager;

        private bool IsCommunicationThreadRunning = false;

        private bool IsCyclicThreadRunning = false;

        private PlcConnectionParameter PlcParameter;

        private bool CommunicationEndRequest = false;

        FinsCapsule Plc = null;

        DeviceManager deviceManager = new DeviceManager();

        PlcConnectionParameter parameter = new PlcConnectionParameter();

        private Thread ErrorThread;

        private Thread ComminucationThread;

        private ServiceManager finsGateWayService;

        private void Connection()
        {
            // FinsGatewayサービスの開始
            finsGateWayService = new ServiceManager(PlcParameter.ServiceName);

            finsGateWayService.Start();

            //{
            //    MessageBox.Show("PLCと接続できませんでした。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    CommonMemory.PlcCommunicationIsConnected = false;
            //    return;
            //}

            if (this.Plc.Open((short)PlcParameter.NetWorkNumber, (short)PlcParameter.NodeNumber, (short)PlcParameter.UnitNumber) == false)
            {
                MessageBox.Show("PLCと接続できませんでした。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);

                logManager.Write("PLCオープン失敗");
                CommonMemory.PlcCommunicationIsConnected = false;
                finsGateWayService.End();
                return;
            }


            //通信テスト
            //CPUﾕﾆｯﾄの形式を読み出す。
            ResponceFormat res = this.Plc.ReadConnectInfo(0x00, 1);
            if (res.IsError == true)
            {
                MessageBox.Show("PLCと接続できませんでした。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                logManager.Write("PLCの接続情報が呼び出せませんでした。 " + res.ErrorMessage);
                CommonMemory.PlcCommunicationIsConnected = false;
                finsGateWayService.End();
                return;
            }

            CommonMemory.PlcCommunicationIsConnected = true;
            logManager.Write("PLCとの回線接続完了");


        }

        private void CommunicationThread()
        {

            logManager.Write("通信スレッド起動成功");

            while (true)
            {
                try
                {
                    IsCommunicationThreadRunning = true;

                    Stopwatch sw = Stopwatch.StartNew();

                    #region 通信終了指令確認
                    if (CommunicationEndRequest)
                    {
                        logManager.Write("終了指令にて通信スレッド終了");
                        break;
                    }
                    #endregion

                    if (ErrorThread != null) { continue; }

                    #region リスト(受信処理）初期化処理

                    // 受信デバイスリスト取得
                    var recvEntryDeviceList = CommonMemory.RecvDeviceList;

                    // 受信結果を保持するリスト
                    List<RemotingDataFormat> recvDataList = new List<RemotingDataFormat>();

                    List<RecvDeviceFormat> RecvDeviceList = new List<RecvDeviceFormat>();

                    #endregion

                    #region ワードデバイスを一括して処理
                    IEnumerable<DataFormat> WordDeviceList = new List<DataFormat>();
                    WordDeviceList = Enumerable.Concat(WordDeviceList, recvEntryDeviceList.WordDevice_Block);
                    WordDeviceList = Enumerable.Concat(WordDeviceList, recvEntryDeviceList.WordDevice_Single);
                    #endregion

                    #region 全てのデバイスを要素単位に分解
                    List<DeviceElement> AllDevice = new List<DeviceElement>();
                    foreach (DataFormat WordDevice in WordDeviceList)
                    {
                        DeviceElement AddElement = WordDevice.Device;
                        RecvDeviceList.Add(new RecvDeviceFormat(AddElement, WordDevice.DataCount));

                        AllDevice.Add(AddElement);
                        for (int i = 1; i < WordDevice.DataCount; i++)
                        {
                            AllDevice.Add(AddElement.Offset(i));
                        }
                    }
                    #endregion

                    #region 受信する全てのデバイスを取得
                    List<DeviceElement> AllRecvDeviceList = new List<DeviceElement>();
                    var PrefixGroups =
                        from OneDevice in AllDevice
                        group OneDevice by OneDevice.Prefix;

                    foreach (var OneGroup in PrefixGroups)
                    {
                        var RecvDevices =
                            from OneDevice in OneGroup
                            group OneDevice by OneDevice.AddressOffset into AddrGroup
                            let Ch = AddrGroup.ElementAt(0)
                            select ToBitCancelElement(Ch);

                        AllRecvDeviceList.AddRange(RecvDevices.ToArray());
                    }
                    #endregion

                    #region 受信コマンド発行
                    //logManager.Write("受信コマンド発行");
                    ResponceFormat AllReadValue = Plc.ReadMemoryRandom(AllRecvDeviceList.ToArray());
                    if (AllReadValue.IsError == true)
                    {
                        ErrorThread = new Thread(new ParameterizedThreadStart(CommandErrorThread));
                        ErrorThread.IsBackground = true;
                        ErrorThread.Start(AllReadValue.ErrorMessage);
                        break;
                    }
                    //logManager.Write("受信コマンド発行完了");
                    #endregion

                    #region 受信した値とデバイスを関連付ける

                    List<RecvValueFormat> AllReadDataList = new List<RecvValueFormat>();
                    int AllRecvDeviceCount = AllRecvDeviceList.Count();
                    for (int i = 0; i < AllRecvDeviceCount; i++)
                    {
                        AllReadDataList.Add(new RecvValueFormat(AllRecvDeviceList.ElementAt(i), AllReadValue.EditMessage[i]));
                    }

                    #endregion

                    #region 受信データリストに登録
                    foreach (var WordDevice in WordDeviceList)
                    {
                        ushort[] Values = new ushort[WordDevice.DataCount];
                        DeviceElement Device = WordDevice.Device;

                        for (int i = 0; i < WordDevice.DataCount; i++)
                        {
                            DeviceElement Offset = Device.Offset(i);

                            var FindValue = AllReadDataList.Find(item =>
                                (item.Device.Prefix == Offset.Prefix)
                                && (item.Device.AddressOffset == Offset.AddressOffset));

                            if (Offset.BitOffset == -1)
                            {
                                Values[i] = FindValue.Value;
                            }
                            else
                            {
                                Values[i] = (ushort)((FindValue.Value >> Offset.BitOffset) & 0x01);
                            }
                        }

                        recvDataList.Add(new RemotingDataFormat(WordDevice.Device, WordDevice.DataCount, Values));
                    }

                    #endregion

                    //受信した値を共通クラスに登録する。
                    CommonMemory.RecvDataList = recvDataList;

                    //SendDataを取り出す。
                    RemotingMessageFormat[] tmpSendDataList = CommonMemory.GetSendDataList();

                    #region 送信先の先頭アドレスを要素化
                    List<SendFormat> sendDataList = new List<SendFormat>();
                    foreach (var sendData in tmpSendDataList)
                    {
                        sendDataList.Add(new SendFormat(ToElement(sendData.DeviceString), sendData.DataCount, sendData.Calculator));
                    }
                    #endregion

                    #region デバイス種別毎にグループ化する
                    var prefixGroupList =
                        from e in sendDataList
                        group e by e.Device.Prefix;
                    #endregion

                    foreach (var prefixGroup in prefixGroupList)
                    {
                        // 一番上のアイテム
                        SendFormat firstItem = prefixGroup.ElementAt(0);

                        // 一時ﾒﾓﾘ
                        ushort[] memory = new ushort[65535];

                        #region 送信ブロックの作成
                        List<WordBlock> SendBlockList = new List<WordBlock>();
                        WordBlock ProcessBlock = new WordBlock();
                        var SortPrefixGroup =
                            from item in prefixGroup
                            orderby item.Device.AddressOffset
                            select item;

                        foreach (SendFormat SendData in SortPrefixGroup)
                        {
                            // 初回処理
                            if (ProcessBlock.Device == null)
                            {
                                ProcessBlock.Device = ToBitCancelElement(SendData.Device);
                                ProcessBlock.DataCount = SendData.DataCount;
                            }
                            else
                            {
                                // ﾌﾞﾛｯｸ先頭ｱﾄﾞﾚｽと、追加ﾃﾞｰﾀのｱﾄﾞﾚｽとの差分
                                int DiffOffset = SendData.Device.AddressOffset - ProcessBlock.Device.AddressOffset;

                                // 差分が最大送受信ﾃﾞｰﾀ数を超える
                                if (DiffOffset >= 498)
                                {
                                    // ﾌﾞﾛｯｸﾘｽﾄに登録
                                    SendBlockList.Add(ProcessBlock);

                                    // 新しいﾌﾞﾛｯｸの作成
                                    ProcessBlock = new WordBlock();
                                    ProcessBlock.Device = ToBitCancelElement(SendData.Device);
                                    ProcessBlock.DataCount = SendData.DataCount;
                                }
                                else
                                {
                                    // ﾃﾞｰﾀ数を足す
                                    int AddDataCount = (DiffOffset - ProcessBlock.DataCount) + SendData.DataCount;

                                    if (AddDataCount < 0)
                                    {
                                    }
                                    else
                                    {
                                        ProcessBlock.DataCount += AddDataCount;
                                    }
                                }
                            }

                            // 合計ﾃﾞｰﾀ数が最大送受信ﾃﾞｰﾀ数を超える
                            if (ProcessBlock.DataCount >= 498)
                            {
                                while (ProcessBlock.DataCount >= 498)
                                {
                                    // 新しいﾌﾞﾛｯｸの要素
                                    DeviceElement NewDevice = ProcessBlock.Device.Offset(498);
                                    int NewDataCount = ProcessBlock.DataCount - 498;

                                    // ﾌﾞﾛｯｸを切り出す
                                    ProcessBlock.DataCount = 498;

                                    // ﾌﾞﾛｯｸﾘｽﾄに登録
                                    SendBlockList.Add(ProcessBlock);

                                    // 新しいﾌﾞﾛｯｸの作成
                                    ProcessBlock = new WordBlock();
                                    ProcessBlock.Device = NewDevice;
                                    ProcessBlock.DataCount = NewDataCount;
                                }
                            }
                        }
                        if (ProcessBlock.Device != null)
                        {
                            SendBlockList.Add(ProcessBlock);
                        }
                        #endregion

                        #region 現在値受信

                        logManager.Write("現在値受信");
                        foreach (WordBlock block in SendBlockList)
                        {
                            ResponceFormat RecvValue = Plc.ReadMemory(block.Device, block.DataCount);
                            if (RecvValue.IsError == true)
                            {
                                ErrorThread = new Thread(new ParameterizedThreadStart(CommandErrorThread));
                                ErrorThread.IsBackground = true;
                                ErrorThread.Start(RecvValue.ErrorMessage);
                            }
                            Array.Copy(RecvValue.EditMessage, 0, memory, block.Device.AddressOffset, RecvValue.EditMessage.Length);
                        }
                        logManager.Write("現在値受信完了");
                        #endregion

                        #region 送信ﾃﾞｰﾀの反映
                        foreach (SendFormat SendData in prefixGroup)
                        {
                            // ﾃﾞｰﾀ書込み
                            // ﾜｰﾄﾞﾃﾞﾊﾞｲｽ操作
                            if (SendData.Device.BitOffset == -1)
                            {
                                int DataCount = SendData.DataCount;

                                // 一時ﾒﾓﾘから受信ﾃﾞｰﾀをｺﾋﾟｰ
                                ushort[] Words = new ushort[DataCount];
                                Array.Copy(memory, SendData.Device.AddressOffset, Words, 0, DataCount);

                                // 受信ﾃﾞｰﾀに対する処理
                                Words = SendData.Calculator.Calculate(Words);

                                // 処理した結果を一時ﾒﾓﾘに戻す
                                Array.Copy(Words, 0, memory, SendData.Device.AddressOffset, DataCount);
                            }
                            else
                            {
                                int WordCount = ((SendData.Device.BitOffset + (SendData.DataCount - 1)) >> 4) + 1;

                                // 一時ﾒﾓﾘから受信ﾃﾞｰﾀをﾜｰﾄﾞ単位でｺﾋﾟｰ
                                int[] Words = new int[WordCount];
                                Array.Copy(memory, SendData.Device.AddressOffset, Words, 0, WordCount);

                                // 受信ﾃﾞｰﾀをﾋﾞｯﾄ配列化
                                BitArray BitWords = new BitArray(Words);

                                // bool[] → ushort[]
                                ushort[] CalculateBits = new ushort[SendData.DataCount];
                                for (int i = 0; i < SendData.DataCount; i++)
                                {
                                    int Address = (i + SendData.Device.BitOffset) >> 4;
                                    int Bit = (i + SendData.Device.BitOffset) & 0x0F;

                                    int Offset = (Address * 32) + Bit;

                                    CalculateBits[i] = (ushort)(BitWords[Offset] ? 1 : 0);
                                }

                                // 受信ﾃﾞｰﾀに対する処理
                                CalculateBits = SendData.Calculator.Calculate(CalculateBits);

                                // int[] → bool[]
                                for (int i = 0; i < SendData.DataCount; i++)
                                {
                                    int Address = (i + SendData.Device.BitOffset) >> 4;
                                    int Bit = (i + SendData.Device.BitOffset) & 0x0F;

                                    int Offset = (Address * 32) + Bit;

                                    BitWords[Offset] = (CalculateBits[i] != 0);
                                }

                                // ﾋﾞｯﾄ配列から処理後ﾃﾞｰﾀを作成
                                BitWords.CopyTo(Words, 0);

                                // 処理した結果を一時ﾒﾓﾘに戻す
                                //Array.Copy(Bits, 0, Memory, SendData.Device.AddressOffset, SendData.DataCount);
                                for (int i = 0; i < Words.Length; i++)
                                {
                                    memory[i + SendData.Device.AddressOffset] = (ushort)(Words[i] & 0x0FFFF);
                                }
                            }

                        }

                        #endregion

                        #region 送信コマンド発行
                        logManager.Write("送信コマンド発行");
                        foreach (WordBlock block in SendBlockList)
                        {
                            ushort[] WriteValue = new ushort[block.DataCount];
                            Array.Copy(memory, block.Device.AddressOffset, WriteValue, 0, block.DataCount);
                            ResponceFormat res = Plc.WriteMemory(block.Device, WriteValue);
                            if (res.IsError == true)
                            {
                                ErrorThread = new Thread(new ParameterizedThreadStart(CommandErrorThread));
                                ErrorThread.IsBackground = true;
                                ErrorThread.Start(AllReadValue.ErrorMessage);
                            }
                        }
                        logManager.Write("送信コマンド発行完了");
                        #endregion
                    }

                    Thread.Sleep(50);

                    sw.Stop();
                    CyclicTime = (int)(sw.ElapsedMilliseconds & 0xFFFFFFFF);
                }

                #region 例外処理
                catch (Exception e)
                {
                    logManager.Write("通信スレッドエラー発生");
                    ErrorThread = new Thread(new ParameterizedThreadStart(CommandErrorThread));
                    ErrorThread.IsBackground = true;
                    ErrorThread.Start(e.Message);

                }
                #endregion

            }
            IsCommunicationThreadRunning = false;
        }

        private void CommandErrorThread(object e)
        {
            PlcErrorEventArgs er = new PlcErrorEventArgs((string)e);
            OnCommunicationError(er);
        }

        public void CyclicTimerThread()
        {
            while (true)
            {
                if (this.CommunicationEndRequest == true)
                {
                    logManager.Write("終了指令にてサイクリックタイム更新スレッド終了");
                    break;
                }
                IsCyclicThreadRunning = true;
                CommonMemory.CommunicationCyclicTime = this.CyclicTime;
                Thread.Sleep(50);
            }
            IsCyclicThreadRunning = false;
        }

        public void WaitForEnd()
        {
            while (true)
            {
                if (CommonMemory.PlcCommunicationEndRequest)
                {
                    this.CommunicationEndRequest = true;
                    CommonMemory.PlcCommunicationEndRequest = false;
                }

                if (!IsCommunicationThreadRunning && !IsCyclicThreadRunning)
                {
                    break;
                }
                Application.DoEvents();
                Thread.Sleep(50);
            }

            logManager.Write("PLC送受信停止");

            this.Plc.Close();
            logManager.Write("PLC回線切断");

            finsGateWayService.End();

            RemotingClient.Disconnection();
            logManager.Write("RemotingClient削除");

            logManager.Dispose();

            CommonMemory.PlcCommunicationIsConnected = false;

        }


        private DeviceElement ToBitCancelElement(DeviceElement element)
        {
            if (element.BitOffset == -1)
            {
                return element;
            }
            else
            {
                string Address = element.ToString();

                return ToElement(Address.Substring(0, Address.LastIndexOf('.')));
            }
        }

        private ushort[] BinaryToWord(int offset, ushort[] bitArray)
        {
            List<bool> bitList = new List<bool>();

            foreach (int bit in bitArray)
                bitList.Add(bit != 0);

            int binOffset = offset;
            int wordOffset = 0;

            // 作成される配列の大きさ
            // ((ﾋﾞｯﾄｵﾌｾｯﾄ値 + 全ﾋﾞｯﾄ値) / 16bit) + 1
            // +1 は余りの分
            int arraySize = ((offset + bitList.Count) >> 0x05) + 1;

            ushort[] wordArray = new ushort[arraySize];

            foreach (bool bitOn in bitList)
            {
                if (bitOn) wordArray[wordOffset] |= (ushort)(0x01 << binOffset);

                binOffset++;

                wordOffset += (short)(binOffset >> 4);
                binOffset = binOffset & 0x0F;
            }

            return wordArray;
        }


        private DeviceElement ToElement(string device)
        {
            return deviceManager.ToElement(device);
        }

        private ushort BitSet(ushort value, int offset, bool state)
        {
            if (state)
            {
                // ビットON
                return (ushort)(value | (0x01 << offset));
            }
            else
            {
                // ビットOFF

                // bitOffset = 3 の場合、
                // value と 0xFFF7 をAND処理する

                return (ushort)(value & (0xFFFF ^ (0x01 << offset)));
            }
        }

        protected void OnCommunicationError(PlcErrorEventArgs e)
        {
            CommonMemory.OnCommunicationError(e);
        }


        #region Class -RecvDeviceFormat-
        private class RecvDeviceFormat
        {
            public DeviceElement Device;
            public int DataCount;
            public RecvDeviceFormat(DeviceElement device, int dataCount)
            {
                Device = device;
                DataCount = dataCount;
            }
        }
        #endregion

        #region Class -RecvValueFormat-
        private class RecvValueFormat
        {
            public DeviceElement Device;
            public ushort Value;
            public RecvValueFormat(DeviceElement device, ushort value)
            {
                Device = device;
                Value = value;
            }
        }
        #endregion

        #region Class -SendFormat-
        /// <summary>
        /// 送信を行うデバイスを表します
        /// </summary>
        private class SendFormat
        {
            // 送信先ﾃﾞﾊﾞｲｽのｱﾄﾞﾚｽ
            public DeviceElement Device;
            // 送信ﾃﾞｰﾀ数
            public int DataCount;
            // 送信ﾃﾞｰﾀ(削除予定)
            //public ushort[] Value;
            // ﾃﾞﾊﾞｲｽに対する操作
            public ICalculator Calculator;
            //public SendFormat(DeviceElement device, int dataCount, int[] value)
            //{
            //    Device = device;
            //    DataCount = dataCount;
            //    Value = value;
            //}
            public SendFormat(DeviceElement device, int dataCount, ICalculator calculator)
            {
                Device = device;
                DataCount = dataCount;
                Calculator = calculator;
            }
        }
        #endregion

        #region Class -WordBlock-
        /// <summary>
        /// 送受信を行うワード単位のブロックを表します
        /// </summary>
        private class WordBlock
        {
            // ﾌﾞﾛｯｸ開始ｱﾄﾞﾚｽ
            public DeviceElement Device;
            // ﾌﾞﾛｯｸ対象ﾃﾞﾊﾞｲｽ数
            public int DataCount;

            public WordBlock()
            {
            }
            public WordBlock(DeviceElement device, int dataCount)
            {
                Device = device;
                DataCount = dataCount;
            }
        }
        #endregion

        #region Enum -RecvResult-
        enum RecvResult
        {
            OK,
            Error,
            TimeOut,
        }
        #endregion

        #region Enum -SendResult-
        enum SendResult
        {
            OK,
            Error,
            TimeOut,
        }
        #endregion

        public void Start()
        {
            //LogManager生成
            logManager = LogManager.CreateInstance
                (this, AppSetting.LogPath + Path.DirectorySeparatorChar + "PlcCommunication.log", true);

            CommonMemory = RemotingClient.Connection();
            if (CommonMemory == null)
            {
                MessageBox.Show("PLCと接続できませんでした。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                logManager.Write("RemotingClient接続エラー");
                return;
            }
            logManager.Write("RemotingClient接続成功");

            //設定読み込み
            PlcParameter = (PlcConnectionParameter)ParameterConverter.FromConnectionSetting
                (typeof(PlcConnectionParameter), CommonMemory.ConnectSetting);

            this.Plc = new FinsCapsule((PlcType)Enum.Parse(typeof(PlcType), PlcParameter.PlcType));

            //選択された方法により、接続を行う。
            if (CommonMemory.Mode == ConnectionMode.Normal)   //通常接続
            {
                Connection();
            }

            //非接続時処理
            if (CommonMemory.PlcCommunicationIsConnected == false)
            {
                RemotingClient.Disconnection();
                logManager.Write("RemotingClient削除");
                if (logManager != null) { logManager.Dispose(); }
                return;
            }


            //送受信開始
            this.CommunicationEndRequest = false;
            this.IsCommunicationThreadRunning = false;
            this.IsCyclicThreadRunning = false;
            this.ErrorThread = null;

            // 送受信スレッド
            this.ComminucationThread = new Thread(new ThreadStart(this.CommunicationThread));
            this.ComminucationThread.IsBackground = true;
            this.ComminucationThread.Start();


            //サイクルタイム更新スレッド
            Thread threadCyclicTimer = new Thread(new ThreadStart(CyclicTimerThread));
            threadCyclicTimer.IsBackground = true;
            threadCyclicTimer.Start();

            //終了待機スレッド
            Thread waitForEndThread = new Thread(new ThreadStart(WaitForEnd));
            waitForEndThread.IsBackground = true;
            waitForEndThread.Start();

        }



        #region CyclicTime
        object cyclicTimeLock = new object();
        long cyclicTime = 0;
        public long CyclicTime
        {
            get
            {
                lock (this.cyclicTimeLock)
                {
                    return this.cyclicTime;
                }
            }
            set
            {
                lock (this.cyclicTimeLock)
                {
                    if (this.cyclicTime >= 0)
                    {
                        this.cyclicTime = value;
                    }
                    else
                    {
                        throw new ArgumentException("CyclicTime");
                    }
                }
            }
        }
        #endregion

        public string MakerName
        {
            get { return "オムロン"; }
        }
    }

    [Serializable]
    public class PlcConnectionParameter
    {
        public PlcConnectionParameter()
        {
            this.PlcType = "";
            this.ServiceName = "";
            this.NetWorkNumber = 0;
            this.NodeNumber = 0;
            this.UnitNumber = 0;
        }

        [ParameterID(0)]
        public string PlcType { get; set; }

        [ParameterID(1)]
        public string ServiceName { get; set; }

        [ParameterID(2)]
        public int NetWorkNumber { get; set; }

        [ParameterID(3)]
        public int NodeNumber { get; set; }

        [ParameterID(4)]
        public int UnitNumber { get; set; }
    }

    internal static class ConstValue
    {
        // 
        // 一回のFINSｺﾏﾝﾄﾞのやり取りで読出せる、1byteﾃﾞｰﾀの最大読出要素数
        // (SYSWAY, SYSMAC LINK, DeviceNet)
        // 
        public const int MaxReadByte = 538;
        // 
        // 一回のFINSｺﾏﾝﾄﾞのやり取りで書込める、1byteﾃﾞｰﾀの最大書込要素数
        // (SYSWAY, SYSMAC LINK, DeviceNet)
        // 
        public const int MaxWriteByte = 534;
        // 
        // 一回のFINSｺﾏﾝﾄﾞのやり取りで読出せる、1byteﾃﾞｰﾀの最大読込要素数
        // (Ethernet, Controller Link)
        // 
        public const int MaxReadByteHigh = 1998;
        // 
        // 一回のFINSｺﾏﾝﾄﾞのやり取りで書込める、1byteﾃﾞｰﾀの最大書込要素数
        // (Ethernet, Controller Link)
        // 
        public const int MaxWriteByteHigh = 1992;
        // 
        // 一回のI/O情報の読出し(ｺﾏﾝﾄﾞ:01 04)で読み出せる、最大読込要素数
        // 
        public const int MaxRandomReadCount = 89;
    }

    public class PlcSetting : IPlcSetting
    {

        public ConnectionMode FormShow(ref object[] setting)
        {
            PlcConnectionParameter connectionSetting =
                (PlcConnectionParameter)ParameterConverter.FromConnectionSetting(typeof(PlcConnectionParameter), setting);


            SettingCommunicationForm frm = new SettingCommunicationForm();

            //初期設定
            frm.InitialSetting(connectionSetting);

            frm.StartPosition = FormStartPosition.CenterScreen;

            //設定フォーム表示
            frm.ShowDialog();

            //通信設定更新
            setting = frm.UpDateSetting;

            return frm.ClickButtonType;


        }
    }

    [Serializable]
    public class ResponceFormat
    {
        public bool IsError = false;
        public string ErrorMessage = "";
        public byte[] ResponceMessage = null;
        public FinsHead ResponceHead = null;
        public ushort[] EditMessage = null;

        public static string ResponseErrorLogOut(byte mainCode, byte subCode)
        {
            string mainResponse = Convert.ToString(mainCode, 2);
            string subResponse = Convert.ToString(subCode, 2);
            mainResponse = mainResponse.PadLeft(8, '0');
            subResponse = subResponse.PadLeft(8, '0');

            int relayError = Convert.ToInt32(mainResponse.Substring(0, 1));
            int plcStop = Convert.ToInt32(subResponse.Substring(0, 1), 2);
            int plcError = Convert.ToInt32(subResponse.Substring(1, 1), 2);

            mainResponse = mainResponse.Substring(1).PadLeft(8, '0');
            subResponse = subResponse.Substring(2).PadLeft(8, '0');

            string error = string.Empty;
            if (relayError != 0)
                error += "中継異常発生 ";
            if (plcStop != 0)
                error += "本体停止異常発生 ";
            if (plcError != 0)
                error += "本体継続異常発生 ";

            error += Convert.ToInt32(mainResponse, 2).ToString("X2") + " ";
            error += Convert.ToInt32(subResponse, 2).ToString("X2");

            return error;

        }

    }


}
