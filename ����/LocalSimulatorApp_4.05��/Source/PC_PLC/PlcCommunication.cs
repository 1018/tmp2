using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonClassLibrary;
using KssClassLibrary;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

namespace PC_PLC
{
    [Serializable]
    public class PlcCommunication : IPlcCommunication
    {

        Remoting CommonMemory;
        public static LogManager logManager;
        IDeviceManager deviceManager = new DeviceManager();
        Thread comThread;

        #region IPlcCommunication メンバ

        public void Start()
        {

            //LogManager生成
            logManager = LogManager.CreateInstance
                (this, AppSetting.LogPath + Path.DirectorySeparatorChar + "PlcCommunication.log", true);

            CommonMemory = RemotingClient.Connection();
            if (CommonMemory == null)
            {
                MessageBox.Show("パソコンと接続できませんでした。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                logManager.Write("RemotingClient接続エラー");
                return;
            }
            logManager.Write("RemotingClient接続成功");

            Connection();

            //非接続時処理
            if (CommonMemory.PlcCommunicationIsConnected == false)
            {
                //RemotingClient.Disconnection();
                logManager.Write("RemotingClient削除");
                if (logManager != null) { logManager.Dispose(); }
                return;
            }

            // 送受信スレッド
            CommonMemory.PlcCommunicationEndRequest = false;
            comThread = new Thread(new ThreadStart(this.SendRecvThread));
            comThread.IsBackground = true;
            comThread.Start();

            //終了待機スレッド
            Thread waitForEndThread = new Thread(new ThreadStart(WaitForEnd));
            waitForEndThread.IsBackground = true;
            waitForEndThread.Start();

        }

        #endregion

        public void WaitForEnd()
        {
            while (comThread.IsAlive)
            {
                Application.DoEvents();
                Thread.Sleep(10);
            }

            logManager.Write("PLC送受信停止");

            RemotingClient.Disconnection();
            logManager.Write("RemotingClient削除");

            logManager.Dispose();

            CommonMemory.PlcCommunicationIsConnected = false;

        }

        #region IPlcUniqueData メンバ

        public string MakerName
        {
            get { return "パソコン通信"; }
        }

        #endregion


        public void Connection()
        {
            if (MemoryManager.OpenMemory() == 0)
            {
                CommonMemory.PlcCommunicationIsConnected = true;
                return;
            }
            MessageBox.Show("PC仮想メモリと接続できませんでした。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        //bool plcComEnd = false;

        //public bool PlcComEnd
        //{
        //    get { return plcComEnd; }
        //    set { plcComEnd = value; }
        //}
        //RecvDeviceFormat recvDeviceList = new RecvDeviceFormat();

        //public RecvDeviceFormat RecvDeviceList
        //{
        //    get { return recvDeviceList; }
        //    set
        //    {
        //        recvDeviceList = value;
        //        Console.WriteLine("RecvDeviceList.Buf_Block.Count = {0}", recvDeviceList.Buf_Block.Count);

        //    }
        //}

        //送受信スレッド
        public void SendRecvThread()
        {
            while (true)
            {
                Stopwatch sw = Stopwatch.StartNew();

                #region 通信終了指令確認
                if (CommonMemory.PlcCommunicationEndRequest)
                //if (PlcComEnd)
                {
                    logManager.Write("終了指令にて通信スレッド終了");
                    break;
                }
                #endregion

                //*
                //受信
                RecvDeviceFormat recvDeviceList = CommonMemory.RecvDeviceList;
                /*/
                RecvDeviceFormat recvDeviceList = new RecvDeviceFormat();
                //*/

                //受信データリスト初期化
                List<RemotingDataFormat> recvDataList = new List<RemotingDataFormat>();

                //ビットデバイス
                foreach (DataFormat deviceData in recvDeviceList.BitDevice_Single)
                {
                    recvDataList.Add(RecvBitDevice(deviceData));
                }
                foreach (DataFormat deviceData in recvDeviceList.BitDevice_Block)
                {
                    recvDataList.Add(RecvBitDevice(deviceData));
                }

                //バッファデバイス
                foreach (DataFormat deviceData in recvDeviceList.Buf_Block)
                {
                    recvDataList.Add(RecvBufDevice(deviceData));
                }


                //受信した値を共通クラスに登録する。
                CommonMemory.RecvDataList = recvDataList;

                //送信
                //SendDataを取り出す。
                RemotingMessageFormat[] getSendList = CommonMemory.GetSendDataList();

                //配列内に重複を削除する
                List<RemotingMessageFormat> arraySendList = new List<RemotingMessageFormat>(getSendList);
                Dictionary<string, RemotingMessageFormat> sendDic = new Dictionary<string, RemotingMessageFormat>();

                foreach (RemotingMessageFormat listValue in getSendList)
                {
                    int cnt = listValue.DataCount;
                    string searchString = listValue.DeviceString + " CNT:" + cnt.ToString();

                    //if (!sendDic.ContainsKey(searchString))
                    //{
                    //    logManager.Write("SendData [" + searchString + "]");
                    //}
                    sendDic[searchString] = listValue;
                }

                //logManager.Write("送信リストデータ個数：" + sendDic.Values.Count);

                foreach (RemotingMessageFormat sendMsg in sendDic.Values)
                {
                    int count = sendMsg.DataCount;
                    string deviceStr = sendMsg.DeviceString;
                    ICalculator calculator = sendMsg.Calculator;
                    ushort[] setValue = calculator.Calculate(new ushort[count]);

                    DeviceElement deviceE = deviceManager.ToElement(deviceStr);

                    //XYデバイス
                    if (deviceE.DeviceType == DeviceType.BitDevice)
                    {
                        logManager.Write(
                            string.Format("[DBG]{0},CNT={1},VAL={2}",
                                deviceE.ToString(), count, string.Join(" ", setValue.Select((v) => v.ToString("X4")).ToArray())));

                        for (int i = 0; i < count; i++)
                        {
                            bool[] setValue2 = new bool[1];
                            setValue2[0] = Convert.ToBoolean(setValue[i]);

                            MemoryManager.WriteBit(ConvertDeviceType(deviceE.Prefix), deviceE.AddressOffset + i, 1, setValue2);

                        }
                    }
                    //BUFデバイス
                    if (deviceE.DeviceType == DeviceType.BufDevice)
                    {
                        if (deviceE.BitOffset == -1)
                        {
                            MemoryManager.WriteBuf(deviceE.UnitNumber, deviceE.AddressOffset, count, setValue);
                        }
                        else
                        {
                            //MemoryManager.WriteBufBit(deviceE.UnitNumber, deviceE.AddressOffset, deviceE.BitOffset, Convert.ToBoolean(setValue[0]));

                            for (int i = 0; i < count; i++)
                            {
                                MemoryManager.WriteBufBit(deviceE.UnitNumber, deviceE.AddressOffset, deviceE.BitOffset, Convert.ToBoolean(setValue[i]));
                                deviceE = deviceE.Offset(1);
                            }
                        }

                    }
                }


                System.Threading.Thread.Sleep(10);

                CommonMemory.CommunicationCyclicTime = sw.ElapsedMilliseconds;
            }


        }

        private RemotingDataFormat RecvBitDevice(DataFormat deviceData)
        {
            DeviceElement deviceE = deviceData.Device;
            int dataCount = deviceData.DataCount;
            bool[] readDatas = new bool[dataCount];
            ushort[] convertReadDatas = new ushort[dataCount];

            MemoryManager.ReadBit(ConvertDeviceType(deviceE.Prefix), deviceE.AddressOffset, dataCount, ref readDatas);

            for (int i = 0; i < dataCount; i++)
            {
                convertReadDatas[i] = Convert.ToUInt16(readDatas[i]);
            }

            return new RemotingDataFormat(deviceData.Device, dataCount, convertReadDatas);

        }

        private RemotingDataFormat RecvBufDevice(DataFormat deviceData)
        {
            DeviceElement deviceE = deviceData.Device;
            int dataCount = deviceData.DataCount;
            ushort[] readDatas = new ushort[dataCount];

            bool readData = false;

            if (deviceE.BitOffset == -1)
            {
                MemoryManager.ReadBuf(deviceE.UnitNumber, deviceE.AddressOffset, dataCount, ref readDatas);
            }
            else
            {
                //MemoryManager.ReadBufBit(deviceE.UnitNumber, deviceE.AddressOffset, deviceE.BitOffset, ref readData);
                //readDatas[0] = Convert.ToUInt16(readData);

                for (int i = 0; i < dataCount; i++)
                {
                    MemoryManager.ReadBufBit(deviceE.UnitNumber, deviceE.AddressOffset, deviceE.BitOffset, ref readData);
                    readDatas[i] = Convert.ToUInt16(readData);
                    deviceE = deviceE.Offset(1);
                }
            }

            return new RemotingDataFormat(deviceData.Device, dataCount, readDatas);

        }

        int ConvertDeviceType(string deviceType)
        {

            switch (deviceType)
            {
                case "X":
                    return Const.VM_X;

                case "Y":
                    return Const.VM_Y;

                case "M":
                    return Const.VM_M;
            }

            return 0;
        }
    }
}
