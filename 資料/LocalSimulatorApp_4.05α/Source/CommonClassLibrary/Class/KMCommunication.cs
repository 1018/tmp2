using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace CommonClassLibrary
{
    public class KM_Device
    {
        #region Field
            #region Private
            ushort[] Memory = new ushort[32768];

            object _memLock = new object();
            #endregion
        #endregion

        #region Constructor
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public KM_Device()
        {
            // 配列初期化
            Array.Clear(Memory, 0, Memory.Length);
        }
        #endregion

        #region Method
            #region Public
            /// <summary>
            /// MX-Component ReadDeviceBlock関数の再現
            /// </summary>
            /// <param name="DeviceString"></param>
            /// <param name="DataCount"></param>
            /// <param name="Value"></param>
            /// <returns></returns>
            public int ReadDeviceBlock(string DeviceString, int DataCount, out int[] Value)
            {
                lock (_memLock)
                {
                    string Prefix = "KM";

                    Value = new int[DataCount];
                    Array.Clear(Value, 0, DataCount);

                    if (DeviceString.IndexOf(Prefix) != 0)
                    {
                        return 1;
                    }

                    string strAddress = DeviceString.Substring(Prefix.Length);
                    int commaPos = strAddress.IndexOf('.');

                    if (commaPos == -1)
                    {
                        // ビット指定なし
                        int address;

                        if (!int.TryParse(strAddress, out address))
                        {
                            return 1;
                        }

                        Array.Copy(Memory, address, Value, 0, Value.Length);
                    }
                    else
                    {
                        // ビット指定あり
                        int bit_address;
                        int address;

                        if (!int.TryParse(strAddress.Substring(0, commaPos), out address))
                        {
                            return 1;
                        }

                        if (!int.TryParse(strAddress.Substring(commaPos + 1), NumberStyles.HexNumber, null, out bit_address))
                        {
                            return 1;
                        }

                        for (int i = 0; i < DataCount; i++)
                        {
                            int cur_bit = (bit_address + i) & 0x0F;
                            int cur_addr = address + ((bit_address + i) >> 4);

                            Value[i] = (Memory[cur_addr] >> cur_bit) & 0x01;
                        }
                    }

                    return 0;
                }
            }

            /// <summary>
            /// MX-Component ReadDeviceRandom関数の再現
            /// </summary>
            /// <param name="DeviceString"></param>
            /// <param name="DataCount"></param>
            /// <param name="Value"></param>
            /// <returns></returns>
            public int ReadDeviceRandom(string DeviceString, int DataCount, out int[] Value)
            {
                lock (_memLock)
                {
                    string[] addrs = DeviceString.Split(new char[] { '\n' });
                    string Prefix = "KM";
                    int Idx = 0;

                    Value = new int[DataCount];
                    Array.Clear(Value, 0, DataCount);

                    foreach (string DeviceAddress in addrs)
                    {
                        if (DeviceAddress.Length == 0)
                        {
                            continue;
                        }

                        if (DeviceAddress.IndexOf(Prefix) != 0)
                        {
                            return 1;
                        }

                        string strAddress = DeviceAddress.Substring(Prefix.Length);
                        int commaPos = strAddress.IndexOf('.');

                        if (commaPos == -1)
                        {
                            // ビット指定なし
                            int address;

                            if (!int.TryParse(strAddress, out address))
                            {
                                return 1;
                            }

                            int current = address;
                            Value[Idx] = Memory[current];
                        }
                        else
                        {
                            // ビット指定あり
                            int bit_address;
                            int address;

                            if (!int.TryParse(strAddress.Substring(0, commaPos), out address))
                            {
                                return 1;
                            }

                            if (!int.TryParse(strAddress.Substring(commaPos + 1), NumberStyles.HexNumber, null, out bit_address))
                            {
                                return 1;
                            }

                            int cur_bit = (bit_address) & 0x0F;
                            int cur_addr = address + ((bit_address) >> 4);

                            Value[Idx] = (Memory[cur_addr] >> cur_bit) & 0x01;
                        }

                        Idx++;
                    }

                    return 0;
                }
            }

            /// <summary>
            /// MX-Component WriteDeviceBlock関数の再現
            /// </summary>
            /// <param name="DeviceString"></param>
            /// <param name="DataCount"></param>
            /// <param name="Value"></param>
            /// <returns></returns>
            public int WriteDeviceBlock(string DeviceString, int DataCount, ref int[] Value)
            {
                lock (_memLock)
                {
                    string Prefix = "KM";

                    if (DeviceString.IndexOf(Prefix) != 0)
                    {
                        return 1;
                    }

                    string strAddress = DeviceString.Substring(Prefix.Length);
                    int commaPos = strAddress.IndexOf('.');

                    if (commaPos == -1)
                    {
                        // ビット指定なし
                        int address;

                        if (!int.TryParse(strAddress, out address))
                        {
                            return 1;
                        }

                        for (int i = 0; i < DataCount; i++)
                        {
                            int current = address + i;

                            Memory[current] = (ushort)(Value[i] & 0x0FFFF);
                        }
                    }
                    else
                    {
                        // ビット指定あり
                        int bit_address;
                        int address;

                        if (!int.TryParse(strAddress.Substring(0, commaPos), out address))
                        {
                            return 1;
                        }

                        if (!int.TryParse(strAddress.Substring(commaPos + 1), NumberStyles.HexNumber, null, out bit_address))
                        {
                            return 1;
                        }

                        for (int i = 0; i < DataCount; i++)
                        {
                            int cur_bit = (bit_address + i) & 0x0F;
                            int cur_addr = address + ((bit_address + i) >> 4);

                            if (Value[i] != 0)
                            {
                                // ビットON
                                Memory[cur_addr] |= (ushort)(0x01 << cur_bit);
                            }
                            else
                            {
                                // ビットOFF
                                Memory[cur_addr] &= (ushort)(0x0FFFF ^ (0x01 << cur_bit));

                                /*
                                 * 処理
                                 * 
                                 * ①(0x01 << cur_bit)
                                 *      指定のビットのみONの値。
                                 * ②(0x0FFFF ^ (0x01 << cur_bit))
                                 *      指定のビットのみOFFの値。
                                 * ③KM_Memory[cur_addr] &= (short)(0x0FFFF ^ (0x01 << cur_bit))
                                 *      AND処理すると、値の指定のビットのみOFFに出来る。
                                 */
                            }
                        }
                    }

                    return 0;
                }
            }

            /// <summary>
            /// MX-Component WriteDeviceRandom関数の再現
            /// </summary>
            /// <param name="DeviceString"></param>
            /// <param name="DataCount"></param>
            /// <param name="Value"></param>
            /// <returns></returns>
            public int WriteDeviceRandom(string DeviceString, int DataCount, ref int[] Value)
            {
                lock (_memLock)
                {
                    string[] addrs = DeviceString.Split(new char[] { '\n' });
                    string Prefix = "KM";
                    int Idx = 0;

                    foreach (string DeviceAddress in addrs)
                    {
                        if (DeviceAddress.Length == 0)
                        {
                            continue;
                        }

                        if (DeviceAddress.IndexOf(Prefix) != 0)
                        {
                            return 1;
                        }

                        string strAddress = DeviceAddress.Substring(Prefix.Length);
                        int commaPos = strAddress.IndexOf('.');

                        if (commaPos == -1)
                        {
                            // ビット指定なし
                            int address;

                            if (!int.TryParse(strAddress, out address))
                            {
                                return 1;
                            }

                            int current = address;

                            Memory[current] = (ushort)(Value[Idx] & 0xFFFF);
                        }
                        else
                        {
                            // ビット指定あり
                            int bit_address;
                            int address;

                            if (!int.TryParse(strAddress.Substring(0, commaPos), out address))
                            {
                                return 1;
                            }

                            if (!int.TryParse(strAddress.Substring(commaPos + 1), NumberStyles.HexNumber, null, out bit_address))
                            {
                                return 1;
                            }

                            int cur_bit = bit_address & 0x0F;
                            int cur_addr = address + (bit_address >> 4);

                            if (Value[Idx] != 0)
                            {
                                // ビットON
                                Memory[cur_addr] |= (ushort)(0x01 << cur_bit);
                            }
                            else
                            {
                                // ビットOFF
                                Memory[cur_addr] &= (ushort)(0x0FFFF ^ (0x01 << cur_bit));
                            }
                        }

                        Idx++;
                    }

                    return 0;
                }
            }
            #endregion
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public static class KMCommunication
    {
        static KM_Device km;
        static System.Threading.Thread comThread;

        public static bool ComEndOrder = false;

        public static void KMConnection()
        {
            if (comThread == null)
            {
                ComEndOrder = false;
                km = new KM_Device();
                comThread = new System.Threading.Thread(CommunicationThread);
                comThread.Start();
            }
        }

        public static void KMDisconnection()
        {
            ComEndOrder = true;
            comThread = null;
        }

        private static void CommunicationThread()
        {
            Remoting common = Remoting.Instance;

            while (true)
            {
                bool plcComEndOrder = ComEndOrder;
                if (plcComEndOrder)
                {
                    break;
                }

                #region 受信

                //受信デバイスリスト取得
                RecvDeviceFormat recvDeviceList = common.RecvDeviceList;

                //受信データリスト初期化
                List<RemotingDataFormat> recvDataList = new List<RemotingDataFormat>();


                #region DeviceBlock処理
                foreach (DataFormat data in recvDeviceList.Virtual_Block)
                {
                    string deviceString = data.Device.ToString();
                    int indexDot = deviceString.IndexOf('.');

                    // ビット指定なし
                    if (indexDot == -1)
                    {
                        // メモリ読取り
                        int[] readValue;
                        km.ReadDeviceBlock(data.Device.ToString(), data.DataCount, out readValue);

                        // int[] → ushort[] 変換
                        ushort[] registValue = new ushort[readValue.Length];
                        for (int i = 0; i < readValue.Length; i++)
                        {
                            registValue[i] = (ushort)(readValue[i] & 0xFFFF);
                        }

                        // 受信データリストへの登録
                        recvDataList.Add(new RemotingDataFormat(data.Device, data.DataCount, registValue));
                    }
                    else
                    {
                        string wordString = deviceString.Substring(0, indexDot);
                        int bitNumber = Convert.ToInt32(data.Device.ToString().Substring(indexDot + 1));
                        int dataCount = (data.DataCount + bitNumber - 1) / 16 + 1;

                        // メモリ読取り
                        int[] readValue;
                        km.ReadDeviceBlock(wordString, dataCount, out readValue);

                        //Binaryデータを１つにまとめる
                        string allBinary = null;
                        foreach (int wordData in readValue)
                        {
                            string binary = Convert.ToString(wordData, 2).PadLeft(16, '0');
                            allBinary = binary + allBinary;
                        }

                        // BIN(string) → ushort[] 変換
                        ushort[] registValue = new ushort[data.DataCount];
                        for (int i = bitNumber; i < data.DataCount + bitNumber; i++)
                        {
                            registValue[i - bitNumber] = Convert.ToUInt16(allBinary.Substring(allBinary.Length - i - 1, 1));
                        }

                        // 受信データリストへの登録
                        recvDataList.Add(new RemotingDataFormat(data.Device, data.DataCount, registValue));
                    }
                }
                #endregion
                #region DeviceRandom処理
                string readRandomKMString = string.Empty;

                // 受信データをまとめる
                foreach (DataFormat data in recvDeviceList.Virtual_Single)
                {
                    int indexDot = data.Device.ToString().IndexOf('.');
                    if (indexDot == -1)
                    {
                        readRandomKMString += data.Device + "\n";
                    }
                    else
                    {
                        string wordString = data.Device.ToString().Substring(0, indexDot);
                        int bitNumber = Convert.ToInt32(data.Device.ToString().Substring(indexDot + 1));
                        readRandomKMString += wordString + "." + Convert.ToString(bitNumber, 16) + "\n";
                    }
                }

                // 受信処理
                if (readRandomKMString != string.Empty)
                {
                    int recvDeviceCount = recvDeviceList.Virtual_Single.Count;
                    
                    // メモリ読取り
                    int[] readValue;
                    km.ReadDeviceRandom(readRandomKMString, recvDeviceCount, out readValue);

                    for (int i = 0; i < recvDeviceCount; i++)
                    {
                        string deviceString = recvDeviceList.Virtual_Single[i].Device.ToString();
                        ushort[] registValue = new ushort[] { (ushort)(readValue[i] & 0xFFFF) };
                        
                        // 受信データリストへの登録
                        recvDataList.Add(new RemotingDataFormat(DeviceManager.ToElement(deviceString), 1, registValue));
                    }
                }
                #endregion


                //受信した値を共通クラスに登録する。
                common.RecvKMList = recvDataList;

                #endregion

                #region 送信

                // SendDataを取り出す。
                RemotingMessageFormat[] sendDataList = common.GetSendKMDataList();
                
                // 送信デバイスリスト
                string deviceStringList = string.Empty;
                // デバイス値操作リスト
                List<ICalculator> calculatorList = new List<ICalculator>();

                foreach (RemotingMessageFormat sendData in sendDataList)
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
                        calculatorList.Add(sendData.Calculator);
                    }
                    #endregion
                    #region Block処理
                    else
                    {
                        string deviceString = sendData.DeviceString;
                        int indexDot = deviceString.IndexOf('.');

                        #region ワードデータ処理
                        if (indexDot == -1)
                        {
                            // 読み出し
                            int[] readValue;
                            km.ReadDeviceBlock(sendData.DeviceString, sendData.DataCount, out readValue);

                            // int[] → ushort[]
                            ushort[] calculateData = new ushort[sendData.DataCount];
                            for (int i = 0; i < calculateData.Length; i++)
                            {
                                calculateData[i] = (ushort)(readValue[i] & 0xFFFF);
                            }

                            // デバイス操作
                            calculateData = sendData.Calculator.Calculate(calculateData);

                            // ushort[] → int[]
                            int[] writeValue = new int[readValue.Length];
                            Array.Copy(calculateData, writeValue, sendData.DataCount);

                            // 書き込み
                            km.WriteDeviceBlock(sendData.DeviceString, sendData.DataCount, ref writeValue);
                        }
                        #endregion
                        #region ビットデータ処理
                        else
                        {
                            int bitNumber = Convert.ToInt32(deviceString.Substring(indexDot + 1));
                            string wordString = deviceString.Substring(0, indexDot);
                            int wordCount = (sendData.DataCount + bitNumber - 1) / 16 + 1;

                            // 読み出し
                            int[] readValue = new int[wordCount];
                            km.ReadDeviceBlock(wordString, wordCount, out readValue);

                            // 読み出しデータをBit配列に変換する
                            BitArray binTotalData = new BitArray(readValue);

                            // デバイス操作用の配列を作成
                            ushort[] calculateValue = new ushort[sendData.DataCount];
                            for (int i = 0; i < sendData.DataCount; i++)
                            {
                                int address = (i + bitNumber) >> 4;
                                int bit = (i + bitNumber) & 0x0F;

                                int offset = (address << 4) + bit;

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
                            int[] writeValue = new int[readValue.Length];
                            binTotalData.CopyTo(writeValue, 0);

                            km.WriteDeviceBlock(wordString, wordCount, ref writeValue);
                        }
                        #endregion
                    }
                    #endregion
                }

                if (deviceStringList.Length != 0)
                {
                    // 読み出し
                    int[] readValue;
                    km.ReadDeviceRandom(deviceStringList, calculatorList.Count, out readValue);

                    // デバイス操作
                    int[] writeValue = new int[readValue.Length];
                    for (int i = 0; i < calculatorList.Count; i++)
                    {
                        ushort[] calculateValue = new ushort[] { (ushort)(readValue[i] & 0xFFFF) };
                        ICalculator calculator = calculatorList[i];

                        calculateValue = calculator.Calculate(calculateValue);

                        writeValue[i] = calculateValue[0];
                    }

                    // 書込み
                    km.WriteDeviceRandom(deviceStringList, readValue.Length, ref writeValue);
                }

                #endregion

                //System.Threading.Thread.Sleep(50);
                System.Threading.Thread.Sleep(10);
            }
        }

    }
}
