using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Threading;
using System.Windows.Forms;
using CommonClassLibrary;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting;
using System.Reflection;
using System.Runtime.InteropServices;
using KssClassLibrary;

namespace MITSUBISHI_PLC
{
    [Serializable]
    public class PlcCommunication : IPlcCommunication
    {

        // MXｺﾝﾎﾟｰﾈﾝﾄ通信用
        private ActAlmighty plcIF = null;

        // 共有メモリ空間
        public Remoting CommonMemory { private get; set; }

        private bool CommunicationEndRequest = false;

        private bool IsReadThreadRunning = false;

        private bool IsWriteThreadRunning = false;

        private bool IsCyclicThreadRunning = false;

        //private bool IsCommandError = false;

        private Thread ReadThread;

        private Thread WriteThread;

        private Thread CommandErrorThread;

        private PlcConnectionParameter PlcParameter;

        public static LogManager logManager;

        #region コマンド送受信

        // データ読出スレッド
        private void DataReadThread()
        {
            logManager.Write("読出しスレッド起動成功");

            while (true)
            {
                try
                {
                    IsReadThreadRunning = true;

                    // 周期計測 StopWatch
                    Stopwatch sw = Stopwatch.StartNew();

                    // スレッド終了フラグ確認                    
                    if (this.CommunicationEndRequest)
                    {
                        logManager.Write("終了指令にて読出スレッド終了");
                        break;
                    }

                    if (CommandErrorThread != null) { continue; }

                    //受信デバイスリスト取得
                    RecvDeviceFormat recvDeviceList = CommonMemory.RecvDeviceList;

                    //受信データリスト初期化
                    List<RemotingDataFormat> recvDataList = new List<RemotingDataFormat>();

                    #region DeviceBlock処理
                    #region ビットデバイス
                    foreach (DataFormat Data in recvDeviceList.BitDevice_Block)
                    {
                        string DeviceString = Data.Device.ToString();
                        int DeviceAddress = Convert.ToInt32(DeviceString.Substring(1), 16);
                        int DataCount = (Data.DataCount + (Data.DataCount & 0x0F)) / 16 + 1;
                        int addressOffset = 0;

                        // アドレスが16の倍数でない
                        if ((DeviceAddress & 0x0F) != 0)
                        {
                            // 16の倍数にする
                            addressOffset = (DeviceAddress & 0x0F);

                            DeviceString = DeviceString.Substring(0, 1) + (DeviceAddress - addressOffset).ToString();
                        }

                        int[] Value = SendReadCommand("ReadDeviceBlock", new object[] { DeviceString, DataCount + addressOffset });
                        if (Value == null) { continue; }

                        // 受信はワード単位

                        //Binaryデータを１つにまとめる
                        string AllBinary = null;
                        foreach (int WordData in Value)
                        {
                            string Binary = Convert.ToString(WordData, 2).PadLeft(16, '0');
                            AllBinary = Binary + AllBinary;
                        }

                        ushort[] ReadValue = new ushort[Data.DataCount];

                        for (int i = addressOffset; i < Data.DataCount + addressOffset; i++)
                        {
                            ReadValue[i - addressOffset] = Convert.ToUInt16(AllBinary.Substring(AllBinary.Length - i - 1, 1));
                        }

                        recvDataList.Add(new RemotingDataFormat(Data.Device, Data.DataCount, ReadValue));
                    }
                    #endregion
                    #region ワードデバイス
                    foreach (DataFormat Data in recvDeviceList.WordDevice_Block)
                    {
                        string DeviceString = Data.Device.ToString();
                        int Index_Dot = DeviceString.IndexOf('.');

                        // ﾜｰﾄﾞﾃﾞﾊﾞｲｽ の ﾋﾞｯﾄﾌﾞﾛｯｸ でない
                        if (Index_Dot == -1)
                        {
                            int[] Value = SendReadCommand("ReadDeviceBlock", new object[] { DeviceString, Data.DataCount });
                            if (Value == null) { continue; }

                            ushort[] RegistValue = new ushort[Data.DataCount];

                            for (int i = 0; i < Data.DataCount; i++)
                            {
                                RegistValue[i] = (ushort)(Value[i] & 0xFFFF);
                            }

                            recvDataList.Add(new RemotingDataFormat(Data.Device, Data.DataCount, RegistValue));
                        }
                        else
                        {
                            string WordString = Data.Device.ToString().Substring(0, Index_Dot);
                            int BitNumber = Convert.ToInt32(Data.Device.ToString().Substring(Index_Dot + 1));
                            int DataCount = (Data.DataCount + BitNumber - 1) / 16 + 1;
                            int[] Value = SendReadCommand("ReadDeviceBlock", new object[] { WordString, DataCount });
                            if (Value == null) { continue; }

                            //Binaryデータを１つにまとめる
                            string AllBinary = null;
                            foreach (int WordData in Value)
                            {
                                string Binary = Convert.ToString(WordData, 2).PadLeft(16, '0');
                                AllBinary = Binary + AllBinary;
                            }

                            ushort[] ReadValue = new ushort[Data.DataCount];

                            for (int i = BitNumber; i < Data.DataCount + BitNumber; i++)
                            {
                                ReadValue[i - BitNumber] = Convert.ToUInt16(AllBinary.Substring(AllBinary.Length - i - 1, 1));
                            }

                            recvDataList.Add(new RemotingDataFormat(Data.Device, Data.DataCount, ReadValue));
                        }
                    }
                    #endregion

                    #region バッファデバイス
                    foreach (DataFormat Data in recvDeviceList.Buf_Block)
                    {
                        string DeviceString = Data.Device.ToString();
                        int Index_Dot = DeviceString.IndexOf('.');

                        // ﾋﾞｯﾄﾌﾞﾛｯｸ でない
                        if (Index_Dot == -1)
                        {
                            int[] Value = SendReadCommand("ReadBuffer", new object[] { Data.Device.ToString(), Data.DataCount });
                            if (Value == null) { continue; }

                            ushort[] RegistValue = new ushort[Value.Length];

                            for (int i = 0; i < Value.Length; i++)
                                RegistValue[i] = (ushort)(Value[i] & 0xFFFF);

                            recvDataList.Add(new RemotingDataFormat(Data.Device, Data.DataCount, RegistValue));
                        }
                        else
                        {
                            string WordString = Data.Device.ToString().Substring(0, Index_Dot);
                            int BitNumber = Convert.ToInt32(Data.Device.ToString().Substring(Index_Dot + 1));
                            int DataCount = (Data.DataCount + BitNumber - 1) / 16 + 1;
                            //int[] Value = SendCommand("ReadBuffer", new object[] { DeviceString, DataCount });
                            int[] Value = SendReadCommand("ReadBuffer", new object[] { WordString, DataCount });
                            if (Value == null) { continue; }

                            //Binaryデータを１つにまとめる
                            string AllBinary = null;
                            foreach (int WordData in Value)
                            {
                                string Binary = Convert.ToString(WordData, 2).PadLeft(16, '0');
                                AllBinary = Binary + AllBinary;
                            }

                            ushort[] ReadValue = new ushort[Data.DataCount];

                            for (int i = BitNumber; i < Data.DataCount + BitNumber; i++)
                            {
                                ReadValue[i - BitNumber] = Convert.ToUInt16(AllBinary.Substring(AllBinary.Length - i - 1, 1));
                            }

                            recvDataList.Add(new RemotingDataFormat(Data.Device, Data.DataCount, ReadValue));
                        }
                    }
                    #endregion
                    #endregion

                    #region DeviceRandom処理
                    #region ビットデバイス
                    {
                        string ReadRandomString = null;
                        foreach (DataFormat Data in recvDeviceList.BitDevice_Single)
                        {
                            string DeviceString = Data.Device.ToString();
                            int Index_Dot = DeviceString.IndexOf('.');

                            if (Index_Dot == -1)
                            {
                                ReadRandomString += DeviceString + "\n";
                            }
                            //Bitは16進数
                            else
                            {
                                string WordString = DeviceString.Substring(0, Index_Dot);
                                int BitNumber = Convert.ToInt32(DeviceString.Substring(Index_Dot + 1));
                                ReadRandomString += WordString + "." + Convert.ToString(BitNumber, 16) + "\n";
                            }
                        }
                        if (ReadRandomString != null)
                        {
                            int ReadDeviceCount = recvDeviceList.BitDevice_Single.Count;
                            int[] Value = SendReadCommand("ReadDeviceRandom", new object[] { ReadRandomString, ReadDeviceCount });
                            if (Value == null) { continue; }

                            for (int i = 0; i < ReadDeviceCount; i++)
                            {
                                ushort[] ValueArray = new ushort[1];
                                ValueArray[0] = (ushort)(Value[i] & 0xFFFF);
                                recvDataList.Add(new RemotingDataFormat(recvDeviceList.BitDevice_Single[i].Device, 1, ValueArray));
                            }
                        }
                    }
                    #endregion
                    #region ワードデバイス
                    {
                        string ReadRandomString = null;
                        foreach (DataFormat Data in recvDeviceList.WordDevice_Single)
                        {
                            int Dot_Index = Data.Device.ToString().IndexOf('.');

                            if (Dot_Index == -1)
                            {
                                ReadRandomString += Data.Device.ToString() + "\n";
                            }
                            //Bitは16進数
                            else
                            {
                                string WordString = Data.Device.ToString().Substring(0, Dot_Index);
                                int BitNumber = Convert.ToInt32(Data.Device.ToString().Substring(Dot_Index + 1));
                                ReadRandomString += WordString + "." + Convert.ToString(BitNumber, 16) + "\n";
                            }
                        }
                        if (ReadRandomString != null)
                        {
                            int RecvDeviceCount = recvDeviceList.WordDevice_Single.Count;
                            int[] Value = SendReadCommand("ReadDeviceRandom", new object[] { ReadRandomString, RecvDeviceCount });
                            if (Value == null) { continue; }

                            for (int i = 0; i < RecvDeviceCount; i++)
                            {
                                ushort[] ValueArray = new ushort[1];
                                ValueArray[0] = (ushort)(Value[i] & 0xFFFF);
                                recvDataList.Add(new RemotingDataFormat(recvDeviceList.WordDevice_Single[i].Device, 1, ValueArray));
                            }
                        }
                    }
                    #endregion
                    #endregion

                    //受信した値を共通クラスに登録する。
                    CommonMemory.RecvDataList = recvDataList;

                    Thread.Sleep(50);

                    sw.Stop();
                    this.WriterCyclicTime = sw.ElapsedMilliseconds;
                }
                catch (Exception e)
                {
                    logManager.Write("読出スレッドエラー発生");

                    CommandErrorThread = new Thread(new ParameterizedThreadStart(SendCommandErrorThread));
                    CommandErrorThread.IsBackground = true;
                    CommandErrorThread.Start(e.Message);
                }

            }

            IsReadThreadRunning = false;

        }

        //データ書込スレッド
        private void DataWriteThread()
        {
            logManager.Write("書込みスレッド起動成功");


            while (true)
            {
                try
                {
                    IsWriteThreadRunning = true;

                    // 周期計測 StopWatch
                    Stopwatch sw = Stopwatch.StartNew();

                    if (this.CommunicationEndRequest)
                    {
                        logManager.Write("終了指令にて書込スレッド終了");
                        break;
                    }

                    if (CommandErrorThread != null) { continue; }


                    //SendDataを取り出す。
                    RemotingMessageFormat[] sendDataList = CommonMemory.GetSendDataList();

                    #region バッファデバイス処理
                    foreach (RemotingMessageFormat sendData in sendDataList)
                    {
                        if (sendData.DeviceString.Substring(0, 1) == "U" && sendData.DeviceString.IndexOf("G") != -1)
                        {
                            string myStr = sendData.DeviceString;
                            short[] value = new short[sendData.DataCount];
                            int indexG = myStr.IndexOf('G');
                            int unitNumber = Convert.ToInt32(myStr.Substring(1, indexG - 1), 16);
                            int wordNumber;

                            int indexDot = myStr.IndexOf('.');

                            //ワードデータ処理
                            if (indexDot == -1)
                            {
                                wordNumber = Convert.ToInt32(myStr.Substring(indexG + 1, myStr.Length - indexG - 1));

                                // 読み出し
                                int[] readValue = SendReadCommand("ReadBuffer", new object[] { myStr, sendData.DataCount });
                                if (readValue == null) { continue; }

                                // デバイス操作
                                ushort[] calculateArray = new ushort[readValue.Length];
                                for (int i = 0; i < readValue.Length; i++)
                                    calculateArray[i] = (ushort)(readValue[i] & 0xFFFF);

                                calculateArray = sendData.Calculator.Calculate(calculateArray);

                                value = new short[calculateArray.Length];
                                for (int i = 0; i < calculateArray.Length; i++)
                                {
                                    value[i] = (short)calculateArray[i];
                                }

                                // 書き込み
                                SendWriteCommand("WriteBuffer", new object[] { unitNumber / 16, wordNumber, sendData.DataCount, value });

                            }

                            //ビットデータ処理
                            else
                            {
                                //バッファデバイスはワード単位でしか書き込めない為、
                                //一度ワードデータを読み込んでから、ビットを更新し、書き込む。
                                int bitNumber = Convert.ToInt32(myStr.Substring(indexDot + 1, myStr.Length - indexDot - 1));

                                wordNumber = Convert.ToInt32(myStr.Substring(indexG + 1, indexDot - indexG - 1));
                                int wordCount = (sendData.DataCount + bitNumber - 1) / 16 + 1;

                                int[] readValue = SendReadCommand("ReadBuffer", new object[] { myStr.Substring(0, indexDot), wordCount });
                                if (readValue == null) { continue; }

                                // 読み出しデータをBit配列に変換する
                                BitArray binTotalData = new BitArray(readValue);

                                // デバイス操作用の配列を作成
                                ushort[] calculateValue = new ushort[sendData.DataCount];
                                for (int i = 0; i < sendData.DataCount; i++)
                                {
                                    int address = (i + bitNumber) >> 4;
                                    int bit = (i + bitNumber) & 0x0F;

                                    int offset = (address * 32) + bit;

                                    calculateValue[i] = (ushort)(binTotalData[offset] ? 1 : 0);
                                }

                                // デバイスの操作
                                calculateValue = sendData.Calculator.Calculate(calculateValue);

                                // 操作データをBit配列に戻す
                                for (int i = 0; i < sendData.DataCount; i++)
                                {
                                    int address = (i + bitNumber) >> 4;
                                    int bit = (i + bitNumber) & 0x0F;

                                    int offset = (address << 4) + bit;

                                    binTotalData[offset] = (calculateValue[i] != 0);
                                }

                                // Bit配列をint型のワードデータに復元
                                binTotalData.CopyTo(readValue, 0);

                                // 送信ワードデータの作成
                                short[] wordData = new short[wordCount];
                                for (int i = 0; i < wordCount; i++)
                                {
                                    wordData[i] = (short)(readValue[i] & 0xFFFF);
                                }

                                SendWriteCommand("WriteBuffer", new object[] { unitNumber / 16, wordNumber, wordCount, wordData });
                            }

                        }
                    }
                    #endregion

                    #region 通常デバイス処理
                    string deviceStringList = "";
                    List<ICalculator> deviceCalculatorList = new List<ICalculator>();
                    foreach (RemotingMessageFormat sendData in sendDataList)
                    {
                        if (sendData.DeviceString.Substring(0, 1) != "U" && sendData.DeviceString.Substring(0, 2) != "KM")
                        {
                            #region Single処理
                            if (sendData.DataCount == 1)
                            {
                                // 書込むデータを合体
                                string deviceString = sendData.DeviceString;
                                int indexDot = deviceString.IndexOf('.');
                                if (indexDot == -1)
                                {
                                    deviceStringList += deviceString + "\n";
                                }
                                else
                                {
                                    string wordNumber = deviceString.Substring(0, indexDot);
                                    string bitNumber = deviceString.Substring(indexDot + 1, deviceString.Length - indexDot - 1);
                                    string hexBitNumber = Convert.ToString(Convert.ToInt32(bitNumber), 16);
                                    deviceStringList += wordNumber + "." + hexBitNumber + "\n";
                                }
                                //WriteValue.Add(SendData.Value[0]);
                                deviceCalculatorList.Add(sendData.Calculator);
                            }
                            #endregion

                            #region Block処理
                            else
                            {
                                string deviceString = sendData.DeviceString;
                                int[] value = new int[sendData.DataCount];
                                int indexDot = deviceString.IndexOf('.');

                                #region ワードデータ処理
                                if (indexDot == -1)
                                {
                                    #region ビットデバイス(ブロック書込みに制約があるので分けた)
                                    if (deviceString.Substring(0, 1) == "X" || deviceString.Substring(0, 1) == "Y")
                                    {
                                        int baseAddress = Convert.ToInt32(deviceString.Substring(1), 16);
                                        int addressOffset = 0;
                                        int wordCount = (sendData.DataCount + (sendData.DataCount & 0x0F)) / 16 + 1;


                                        // アドレスが16の倍数でない
                                        if ((baseAddress & 0x0F) != 0)
                                        {
                                            // 16の倍数にする
                                            addressOffset = (baseAddress & 0x0F);
                                            deviceString = deviceString.Substring(0, 1) + (baseAddress - addressOffset).ToString();
                                        }

                                        int[] ComValue = SendReadCommand("ReadDeviceBlock", new object[] { deviceString, wordCount });
                                        if (ComValue == null) { continue; }

                                        //Binaryデータを１つにまとめる
                                        string allBinary = null;
                                        foreach (int wordData in ComValue)
                                        {
                                            string binary = Convert.ToString(wordData, 2).PadLeft(16, '0');
                                            allBinary = binary + allBinary;
                                        }
                                        for (int i = 0; i < sendData.DataCount; i++)
                                        {
                                            int offset = i + addressOffset;
                                            value[i] = Convert.ToUInt16(allBinary.Substring(allBinary.Length - offset - 1, 1));
                                        }

                                        // int[] → UInt16[]
                                        ushort[] calculateData = new ushort[sendData.DataCount];
                                        for (int i = 0; i < calculateData.Length; i++)
                                        {
                                            calculateData[i] = (ushort)(value[i] & 0xFFFF);
                                        }

                                        // デバイス操作
                                        calculateData = sendData.Calculator.Calculate(calculateData);

                                        BitArray bitSetter = new BitArray(ComValue);
                                        for (int i = addressOffset; i < sendData.DataCount + addressOffset; i++)
                                        {
                                            int address = i >> 4;
                                            int bit = i & 0x0F;

                                            int offset = (address << 5) + bit;

                                            //BitSetter[i] = CalculateData[i - addressOffset] != 0;
                                            bitSetter[offset] = calculateData[i - addressOffset] != 0;
                                        }

                                        // UInt16[] → int[]
                                        //Array.Copy(CalculateData, 0, Value, addressOffset, SendData.DataCount);
                                        bitSetter.CopyTo(ComValue, 0);

                                        //int Result = PlcIF.WriteDeviceBlock(DeviceString, (SendData.DataCount + addressOffset), ref Value[0]);
                                        SendWriteCommand("WriteDeviceBlock", new object[] { deviceString, wordCount, ComValue });
                                        //int result = plcIF.WriteDeviceBlock(deviceString, wordCount, ComValue);
                                        //if (result != 0)
                                        //{
                                        //    string logingStr = "";
                                        //    logingStr += "PLCコマンド書込エラー（ワードバッファ）" + " ";
                                        //    logingStr += "デバイス:" + sendData.DeviceString + " ";
                                        //    logManager.Write(logingStr);

                                        //    //this.comResult = CommunicationResult.SendDataError;
                                        //    //this.errorStock = logingStr.Replace(' ', '\n');
                                        //    //End();
                                        //    return;
                                        //}

                                    }
                                    #endregion
                                    #region ワードデバイス
                                    else
                                    {
                                        value = SendReadCommand("ReadDeviceBlock", new object[] { sendData.DeviceString, sendData.DataCount });
                                        if (value == null) { continue; }
                                        //plcIF.ReadDeviceBlock(sendData.DeviceString, sendData.DataCount, value);

                                        // int[] → UInt16[]
                                        ushort[] calculateData = new ushort[sendData.DataCount];
                                        for (int i = 0; i < calculateData.Length; i++)
                                        {
                                            calculateData[i] = (ushort)(value[i] & 0xFFFF);
                                        }

                                        // デバイス操作
                                        calculateData = sendData.Calculator.Calculate(calculateData);

                                        // UInt16[] → int[]
                                        Array.Copy(calculateData, value, sendData.DataCount);

                                        SendWriteCommand("WriteDeviceBlock", new object[] { sendData.DeviceString, sendData.DataCount, value });
                                        //int result = plcIF.WriteDeviceBlock(sendData.DeviceString, sendData.DataCount, value);
                                        //if (result != 0)
                                        //{
                                        //    string logingStr = "";
                                        //    logingStr += "PLCコマンド書込エラー（ワードバッファ）" + " ";
                                        //    logingStr += "デバイス:" + sendData.DeviceString + " ";
                                        //    logManager.Write(logingStr);

                                        //    //this.comResult = CommunicationResult.SendDataError;
                                        //    //this.errorStock = logingStr.Replace(' ', '\n');
                                        //    //End();
                                        //    return;
                                        //}
                                    }
                                    #endregion
                                }
                                #endregion

                                #region ビットデータ処理
                                else
                                {
                                    //バッファデバイスはワード単位でしか書き込めない為、
                                    //一度ワードデータを読み込んでから、ビットを更新し、書き込む。
                                    int bitNumber = Convert.ToInt32(deviceString.Substring(indexDot + 1));
                                    string wordString = deviceString.Substring(0, indexDot);
                                    int wordCount = (sendData.DataCount + bitNumber - 1) / 16 + 1;

                                    //int[] readValue = new int[wordCount];

                                    int[] readValue = SendReadCommand("ReadDeviceBlock", new object[] { wordString, wordCount });
                                    if (readValue == null) { continue; }
                                    //int result = plcIF.ReadDeviceBlock(wordString, wordCount, readValue);

                                    //if (result != 0)
                                    //{
                                    //    string logingStr = "";
                                    //    logingStr += "PLCコマンド書込エラー（ビットバッファ読込）" + " ";
                                    //    logingStr += "デバイス:" + sendData.DeviceString + " ";
                                    //    logManager.Write(logingStr);

                                    //    //this.comResult = CommunicationResult.RecvDataError;
                                    //    //this.errorStock = logingStr.Replace(' ', '\n');
                                    //    //End();
                                    //    return;
                                    //}

                                    //// 読み出しデータ(short[])をint[]に変換(BitArrayがint[]しか受け付けないため)
                                    //int[] intRecvValue = new int[wordCount];
                                    //Array.Copy(readValue, intRecvValue, readValue.Length);

                                    // 読み出しデータをBit配列に変換する
                                    BitArray binTotalData = new BitArray(readValue);

                                    // デバイス操作用の配列を作成
                                    ushort[] calculateValue = new ushort[sendData.DataCount];
                                    for (int i = 0; i < sendData.DataCount; i++)
                                    {
                                        int address = (i + bitNumber) >> 4;
                                        int bit = (i + bitNumber) & 0x0F;

                                        int offset = (address * 32) + bit;

                                        calculateValue[i] = (ushort)(binTotalData[offset] ? 1 : 0);
                                    }

                                    // デバイスの操作
                                    calculateValue = sendData.Calculator.Calculate(calculateValue);

                                    // 操作データをBit配列に戻す
                                    for (int i = 0; i < sendData.DataCount; i++)
                                    {
                                        int address = (i + bitNumber) >> 4;
                                        int bit = (i + bitNumber) & 0x0F;

                                        int offset = (address << 5) + bit;

                                        binTotalData[offset] = (calculateValue[i] != 0);
                                    }

                                    // Bit配列をint型のワードデータに復元
                                    binTotalData.CopyTo(readValue, 0);

                                    SendWriteCommand("WriteDeviceBlock", new object[] { wordString, wordCount, readValue });

                                }
                                #endregion

                            }
                            #endregion

                        }
                    }

                    if (deviceStringList.Length != 0)
                    {
                        //int[] valueArray = new int[deviceCalculatorList.Count];
                        //int result;

                        // 読み出し
                        int[] valueArray = SendReadCommand("ReadDeviceRandom", new object[] { deviceStringList, deviceCalculatorList.Count });
                        if (valueArray == null) { continue; }

                        // デバイス操作
                        for (int i = 0; i < deviceCalculatorList.Count; i++)
                        {
                            ushort[] calculateValue = new ushort[] { (ushort)(valueArray[i] & 0xFFFF) };
                            ICalculator calculator = deviceCalculatorList[i];

                            calculateValue = calculator.Calculate(calculateValue);

                            valueArray[i] = calculateValue[0];
                        }

                        SendWriteCommand("WriteDeviceRandom", new object[] { deviceStringList, valueArray.Length, valueArray });

                    }

                    #endregion

                    Thread.Sleep(50);

                    sw.Stop();

                    this.ReadCyclicTime = sw.ElapsedMilliseconds;
                }
                catch (Exception e)
                {
                    logManager.Write("書込スレッドエラー発生");
                    CommandErrorThread = new Thread(new ParameterizedThreadStart(SendCommandErrorThread));
                    CommandErrorThread.IsBackground = true;
                    CommandErrorThread.Start(e.Message);
                }
            }

            IsWriteThreadRunning = false;
        }

        private int[] SendReadCommand(string type, object[] param)
        {
            int result = -1;
            int[] value = null;

            logManager.Write(type);

            if (type == "ReadDeviceBlock")
            {
                string deviceString = (string)param[0];
                int dataCount = (int)param[1];
                value = new int[dataCount];
                result = this.plcIF.ReadDeviceBlock(deviceString, dataCount, value);
            }
            else if (type == "ReadDeviceRandom")
            {
                string deviceString = (string)param[0];
                int dataCount = (int)param[1];
                value = new int[dataCount];
                result = this.plcIF.ReadDeviceRandom(deviceString, dataCount, value);
            }
            else if (type == "ReadBuffer")
            {
                string deviceString = (string)param[0];
                int index_G = deviceString.IndexOf('G');
                int deviceAddress = Convert.ToInt32(deviceString.Substring(index_G + 1));
                int ioAddress = Convert.ToInt32(deviceString.Substring(1, index_G - 1), 16);
                int dataCount = (int)param[1];
                short[] shortValue = new short[dataCount];
                value = new int[dataCount];
                result = this.plcIF.ReadBuffer(ioAddress / 16, deviceAddress, dataCount, shortValue);
                for (int i = 0; i < shortValue.Length; i++) { value[i] = (int)shortValue[i]; }
            }

            //正常完了
            if (result == 0)
            {
                logManager.Write("コマンド正常送信完了");
                return value;
            }

            // エラーメッセージ生成
            List<string> errorStrList = new List<string>();
            errorStrList.Add("PLCコマンド送信エラー [" + type + "] ");
            errorStrList.Add("コード:0x" + result.ToString("X8") + " ");

            // ログ出力
            logManager.Write(string.Join("", errorStrList.ToArray()));

            CommandErrorThread = new Thread(new ParameterizedThreadStart(SendCommandErrorThread));
            CommandErrorThread.IsBackground = true;
            CommandErrorThread.Start(string.Join(Environment.NewLine, errorStrList.ToArray()));

            return null;

        }

        private void SendWriteCommand(string type, object[] param)
        {
            logManager.Write(type);
            int result = -1;
            if (type == "WriteBuffer")
            {
                result = plcIF.WriteBuffer((int)param[0], (int)param[1], (int)param[2], (short[])param[3]);
            }
            else if (type == "WriteDeviceBlock")
            {
                result = plcIF.WriteDeviceBlock((string)param[0], (int)param[1], (int[])param[2]);
            }
            else if (type == "WriteDeviceRandom")
            {
                result = plcIF.WriteDeviceRandom((string)param[0], (int)param[1], (int[])param[2]);
            }
            if (result == 0)
            {
                logManager.Write("コマンド正常送信完了");
                return;
            }

            List<string> errorStrList = new List<string>();
            errorStrList.Add("PLCコマンド送信エラー [" + type + "] ");
            errorStrList.Add("コード:0x" + result.ToString("X8") + " ");

            // ログ出力
            logManager.Write(string.Join("", errorStrList.ToArray()));

            CommandErrorThread = new Thread(new ParameterizedThreadStart(SendCommandErrorThread));
            CommandErrorThread.IsBackground = true;
            CommandErrorThread.Start(string.Join(Environment.NewLine, errorStrList.ToArray()));

        }

        private void SendCommandErrorThread(object e)
        {
            PlcErrorEventArgs er = new PlcErrorEventArgs((string)e);
            OnCommunicationError(er);
        }

        protected void OnCommunicationError(PlcErrorEventArgs e)
        {
            CommonMemory.OnCommunicationError(e);
        }


        #region WriteCyclicTime
        object writeCyclicTimeLock = new object();
        long writeCyclicTime = 0;
        /// <summary>
        /// 送信スレッド更新周期
        /// </summary>
        private long WriterCyclicTime
        {
            get
            {
                lock (this.writeCyclicTimeLock)
                {
                    return this.writeCyclicTime;
                }
            }
            set
            {
                lock (this.writeCyclicTimeLock)
                {
                    // 0以上のみ設定可能
                    if (value >= 0)
                    {
                        this.writeCyclicTime = value;
                    }
                    else
                    {
                        throw new ArgumentException("WriteCyclicTime");
                    }
                }
            }
        }
        #endregion

        #region ReadCyclicTime
        object readCyclicTimeLock = new object();
        long readCyclicTime = 0;
        /// <summary>
        /// 受信スレッド更新周期
        /// </summary>
        private long ReadCyclicTime
        {
            get
            {
                lock (this.readCyclicTimeLock)
                {
                    return this.readCyclicTime;
                }
            }
            set
            {
                lock (this.readCyclicTimeLock)
                {
                    if (value >= 0)
                    {
                        this.readCyclicTime = value;
                    }
                    else
                    {
                        throw new ArgumentException("ReadCyclicTime");
                    }
                }
            }
        }
        #endregion

        #endregion

        #region PLC接続

        /// <summary>
        /// 通常接続
        /// </summary>
        public void Connection()
        {
            ProgressForm pf = new ProgressForm();
            pf.ProgressText.Text = "PLCと接続中です...";
            pf.StartPosition = FormStartPosition.CenterScreen;
            pf.Show();
            Application.DoEvents();

            // 指定された接続方法
            string pcSideIF = this.PlcParameter.ConnectionPoint;

            // CommunicationType値を取得
            FieldInfo comTypeField = typeof(CommunicationType).GetFields().First(
                (field) =>
                {
                    NameAttribute[] attributes = (NameAttribute[])
                        field.GetCustomAttributes(typeof(NameAttribute), false);

                    return ((attributes.Length != 0) && (attributes[0].Name.CompareTo(pcSideIF) == 0));
                });

            CommunicationType communicationType = (CommunicationType)comTypeField.GetValue(null);


            #region  接続方法により分岐
            switch (communicationType)
            {
                case CommunicationType.Simulator:
                case CommunicationType.USB:
                    {
                        ActAlmighty accessor;

                        // 選択されたCPUを取得する
                        ActCpuTypeMember cpu = ActCpuType.GetEnumerable().First(
                            (item) => (PlcParameter.PlcTypeName.CompareTo(item.Name) == 0));

                        int result;
                        if (communicationType == CommunicationType.Simulator)
                        {
                            result = ConnectionToSimulator(cpu, out accessor);
                        }
                        else
                        {
                            result = ConnectionToUSB(cpu, out accessor);
                        }

                        if (result == 0)
                        {
                            // 接続成功   
                            this.plcIF = accessor;
                            this.plcIF.Open();
                            CommonMemory.PlcCommunicationIsConnected = true;

                        }
                        else
                        {
                            // 接続失敗                        
                            plcIF = null;
                            CommonMemory.PlcCommunicationIsConnected = false;

                        }
                    }
                    break;

                case CommunicationType.Ethernet:
                    {
                    }
                    break;

                case CommunicationType.RS_232C:
                    {
                    }
                    break;
                
            }
            #endregion

            pf.Close();

            if (CommonMemory.PlcCommunicationIsConnected == false)
            {
                MessageBox.Show("PLCと接続できませんでした。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                logManager.Write("オープン失敗");
                return;
            }

            logManager.Write("PLCとの回線接続");

        }

        /// <summary>
        /// 自動接続
        /// </summary>        
        public void AutoConnection()
        {
            IEnumerable<ActCpuTypeMember> CpuTypes =
                ActCpuType.GetEnumerable().Where((member) => member.IsCompletenes);

            ActAlmighty accessor;

            AutoConnectForm acForm = new AutoConnectForm();
            acForm.StartPosition = FormStartPosition.CenterScreen;
            acForm.Show();

            string successCpuType = null;

            foreach (ActCpuTypeMember cpuType in CpuTypes)
            {
                acForm.MessageLabel.Text = "[" + cpuType.Name + "]と接続を試みています．．．";
                acForm.Refresh();
                Application.DoEvents();

                if (ConnectionToSimulator(cpuType, out accessor) == 0)
                {
                    // 接続成功
                    successCpuType = cpuType.ToString();
                    this.plcIF = accessor;
                    this.plcIF.Open();
                    CommonMemory.PlcCommunicationIsConnected = true;
                }

                if (acForm.Break)
                {
                    break;
                }
            }
            acForm.Close();

            if (CommonMemory.PlcCommunicationIsConnected == false)
            {
                MessageBox.Show("PLCと接続できませんでした。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                logManager.Write("オープン失敗");
                return;
            }

            logManager.Write("PLCとの自動接続" + successCpuType);

        }

        /// <summary>
        /// GX Simulatorによる接続を行う
        /// </summary>
        /// <param name="cpuType">機種</param>
        /// <param name="accessor">生成されたPLCアクセサ</param>
        /// <returns>
        /// 接続成功：0を返す。
        /// 接続失敗：0以外を返す。
        /// </returns>
        private int ConnectionToSimulator(ActCpuTypeMember cpuType, out ActAlmighty accessor)
        {
            return ConnectionToSimulator(cpuType, 0, 0xff, out accessor);
        }

        /// <summary>
        /// USBによる接続を行う
        /// </summary>
        /// <param name="cpuType">機種</param>
        /// <param name="accessor">生成されたPLCアクセサ</param>
        /// <returns>
        /// 接続成功：0を返す。
        /// 接続失敗：0以外を返す。
        /// </returns>
        private int ConnectionToUSB(ActCpuTypeMember cpuType, out ActAlmighty accessor)
        {
            return ConnectionToUSB(cpuType, 0, 0xff, out accessor);
        }


        /// <summary>
        /// GX Simulatorによる接続を行う
        /// </summary>
        /// <param name="cpuType">テストを行う機種</param>
        /// <param name="networkNumber">
        /// MELSECNET/10(H)でのネットワーク番号。
        /// 自局指定時は0を指定する。
        /// </param>
        /// <param name="stationNumber">
        /// MELSECNET/10(H), CC-Linkでの局番号。
        /// 自局指定時は0を指定する。
        /// </param>
        /// <returns>
        /// 接続成功：0を返す。
        /// 接続失敗：0以外を返す。
        /// </returns>
        private int ConnectionToSimulator(ActCpuTypeMember cpuType, int networkNumber, int stationNumber, out ActAlmighty accessor)
        {
            ACTLLTLib.ActLLTClass actLLT = new ACTLLTLib.ActLLTClass();

            int result = 0;

            // プロパティ設定
            actLLT.ActCpuType = (int)cpuType.PropertyCode;
            actLLT.ActNetworkNumber = networkNumber;
            actLLT.ActStationNumber = stationNumber;
            actLLT.ActTimeOut = 300;

            // PLCアクセサの生成
            ActAlmighty actAccessor = new ActAlmighty(actLLT);
            accessor = actAccessor;

            // 回線オープン
            int openResult = actAccessor.Open();
            if (openResult != 0)
            {
                // 失敗
                result = openResult;
                goto ERROR;
            }


            // CPUタイプ一致確認
            string name;
            int typeCode;

            int getCpuTypeResult = actAccessor.GetCpuType(out name, out typeCode);
            if (getCpuTypeResult != 0)
            {
                // 失敗
                result = getCpuTypeResult;
                goto ERROR;
            }

            if (typeCode != (int)cpuType.SimTypeCode)
            {
                // CPUタイプ不一致
                result = 0x01801007;    // 指定CPUエラー
                goto ERROR;
            }

            // 接続成功


        ERROR:
            int closeResult = actAccessor.Close();

            return result;
        }

        /// <summary>
        /// USBによる接続を行う
        /// </summary>
        /// <param name="cpuType">テストを行う機種</param>
        /// <param name="networkNumber">
        /// MELSECNET/10(H)でのネットワーク番号。
        /// 自局指定時は0を指定する。
        /// </param>
        /// <param name="stationNumber">
        /// MELSECNET/10(H), CC-Linkでの局番号。
        /// 自局指定時は0を指定する。
        /// </param>
        /// <returns>
        /// 接続成功：0を返す。
        /// 接続失敗：0以外を返す。
        /// </returns>
        private int ConnectionToUSB(ActCpuTypeMember cpuType, int networkNumber, int stationNumber, out ActAlmighty accessor)
        {            
            ACTPCUSBLib.ActQCPUQUSBClass actUSB = new ACTPCUSBLib.ActQCPUQUSBClass();



            int result = 0;

            // プロパティ設定
            actUSB.ActCpuType = (int)cpuType.PropertyCode;
            actUSB.ActNetworkNumber = networkNumber;
            actUSB.ActStationNumber = stationNumber;
            actUSB.ActIONumber = this.PlcParameter.UnitNumber + 0x3e0 - 1;
            actUSB.ActTimeOut = 3000;

            // PLCアクセサの生成
            ActAlmighty actAccessor = new ActAlmighty(actUSB);
            accessor = actAccessor;

            // 回線オープン
            int openResult = actAccessor.Open();
            if (openResult != 0)
            {
                // 失敗
                result = openResult;
                goto ERROR;
            }


            // CPUタイプ一致確認
            //string name;
            //int typeCode;

            //int getCpuTypeResult = actAccessor.GetCpuType(out name, out typeCode);
            //if (getCpuTypeResult != 0)
            //{
            //    // 失敗
            //    result = getCpuTypeResult;
            //    goto ERROR;
            //}

            //if (typeCode != (int)cpuType.SimTypeCode)
            //{
            //    // CPUタイプ不一致
            //    result = 0x01801007;    // 指定CPUエラー
            //    goto ERROR;
            //}

            // 接続成功


        ERROR:
            int closeResult = actAccessor.Close();

            return result;
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

            PlcParameter = (PlcConnectionParameter)ParameterConverter.FromConnectionSetting
                (typeof(PlcConnectionParameter), CommonMemory.ConnectSetting);

            //選択された方法により、接続を行う。
            if (CommonMemory.Mode == ConnectionMode.Normal)   //通常接続
            {
                Connection();
            }
            else if (CommonMemory.Mode == ConnectionMode.Auto) //自動接続
            {
                AutoConnection();
            }

            //送受信開始
            if (CommonMemory.PlcCommunicationIsConnected == false)
            {
                RemotingClient.Disconnection();
                logManager.Write("RemotingClient削除");
                if (logManager != null) { logManager.Dispose(); }
                return;
            }

            this.CommunicationEndRequest = false;
            this.IsReadThreadRunning = false;
            this.IsWriteThreadRunning = false;
            this.IsCyclicThreadRunning = false;
            this.CommandErrorThread = null;

            // ﾃﾞｰﾀ読出しｽﾚｯﾄﾞ
            this.ReadThread = new Thread(new ThreadStart(this.DataReadThread));
            this.ReadThread.IsBackground = true;
            this.ReadThread.Start();

            // ﾃﾞｰﾀ書込みｽﾚｯﾄﾞ
            this.WriteThread = new Thread(new ThreadStart(this.DataWriteThread));
            this.WriteThread.IsBackground = true;
            this.WriteThread.Start();

            //サイクルタイム更新スレッド
            Thread threadCyclicTimer = new Thread(new ThreadStart(CyclicTimerThread));
            threadCyclicTimer.IsBackground = true;
            threadCyclicTimer.Start();

            //終了待機スレッド
            Thread waitForEndThread = new Thread(new ThreadStart(WaitForEnd));
            waitForEndThread.IsBackground = true;
            waitForEndThread.Start();



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
                Thread.Sleep(100);
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

                if (!IsReadThreadRunning && !IsWriteThreadRunning && !IsCyclicThreadRunning)
                {
                    break;
                }
                Application.DoEvents();
                Thread.Sleep(100);
            }

            logManager.Write("PLC送受信停止");

            this.plcIF.Close();
            logManager.Write("PLC回線切断");

            RemotingClient.Disconnection();
            logManager.Write("RemotingClient削除");

            logManager.Dispose();

            CommonMemory.PlcCommunicationIsConnected = false;

        }

        public long CyclicTime
        {
            get
            {
                return ((this.WriterCyclicTime + this.ReadCyclicTime) >> 1);
            }
        }

        public string MakerName
        {
            get { return "三菱"; }
        }



    }

    [Serializable]
    public class PlcConnectionParameter
    {
        public PlcConnectionParameter()
        {
            // 初期値設定
            this.PlcTypeName = null;
            this.ConnectionPoint = null;
            this.UnitNumber = 1;
            
        }

        [ParameterID(0)]
        public string PlcTypeName { get; set; }

        [ParameterID(1)]
        public string ConnectionPoint { get; set; }

        [ParameterID(2)]
        public int UnitNumber { get; set; }

    }

    public class PlcSetting : IPlcSetting
    {
        SettingCommunicationForm frm = new SettingCommunicationForm();

        PlcConnectionParameter parameter = new PlcConnectionParameter();

        public ConnectionMode FormShow(ref object[] setting)
        {
            parameter = (PlcConnectionParameter)ParameterConverter.FromConnectionSetting(typeof(PlcConnectionParameter), setting);

            // 初期選択状態の登録
            frm.SelectedPlcName = this.parameter.PlcTypeName;
            frm.SelectedConnectionPoint = this.parameter.ConnectionPoint;
            frm.SelectedUnitNumber = this.parameter.UnitNumber;

            //設定フォーム表示
            frm.ShowDialog();

            if (frm.ClickButtonType == ConnectionMode.Cancel) { return ConnectionMode.Cancel; }

            //設定データ更新
            this.parameter.PlcTypeName = frm.SelectedPlcName;
            this.parameter.ConnectionPoint = frm.SelectedConnectionPoint;
            this.parameter.UnitNumber = frm.SelectedUnitNumber;

            setting = (object[])ParameterConverter.ToConnectionSetting(this.parameter);

            return frm.ClickButtonType;

        }

    }



}
