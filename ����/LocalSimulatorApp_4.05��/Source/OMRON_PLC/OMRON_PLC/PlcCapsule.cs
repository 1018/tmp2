using System;
using System.Collections.Generic;
// FgwDotNet.dll
using OMRON.FinsGateway;
using OMRON.FinsGateway.Messaging;
// FgwNameSpaceDotNet.dll
using CommonClassLibrary;
using System.Linq;
using System.IO;
using System.ComponentModel;

namespace OMRON_PLC
{
    public class FinsCapsule : IDisposable
    {
        FinsDeviceNumber numManager;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FinsCapsule(PlcType plcType)
        {
            #region フィールドとプロパティの初期化

            this.FinsPort = new Port();

            this.TimeOut = 2000;

            this.numManager = new FinsDeviceNumber(plcType);

            #endregion
        }

        /// <summary>
        /// 通信回線をオープンする
        /// </summary>
        /// <param name="peerAddress"></param>
        /// <returns></returns>
        public bool Open(short netNo, short nodeNo, short unitNo)
        {
            //FINSヘッダー作成
            this.SendHead = new FinsHead();
            bool result = SendHead.Compose(new FinsAddress(netNo, nodeNo, unitNo), -1);
            if (result == false) { return false; }


            try
            {
                if (!FinsPort.Open("LocalSimulator"))
                {   // 接続失敗
                    return false;
                }
            }
            catch (FgwException)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 通信回線をクローズする
        /// </summary>
        /// <returns></returns>
        public bool Close()
        {
            if (finsPort.IsOpened)
            {
                return finsPort.Close();
            }
            else
            {
                throw new InvalidOperationException("Close");
            }
        }

        /// <summary>
        /// 接続情報の読出し (コマンドコード : 05 02)
        /// 
        /// このメソッドはまだ完全に実装されていません。
        /// </summary>
        /// <param name="unitNumber"></param>
        /// <param name="readCount"></param>
        /// <returns></returns>       
        public ResponceFormat ReadConnectInfo(int unitNumber, int readCount)
        {
            List<byte> sendMessage = new List<byte>();

            // ｺﾏﾝﾄﾞｺｰﾄﾞ
            sendMessage.AddRange(new byte[] { 0x05, 0x02 });
            // 読出し号機ｱﾄﾞﾚｽ
            sendMessage.Add((byte)(unitNumber & 0x0FF));
            // 読出し情報数
            sendMessage.Add((byte)(readCount & 0x0FF));

            // 送信
            return Send(SendHead, sendMessage.ToArray(), false);


        }

        /// <summary>
        /// I/O情報の読出し (コマンドコード : 01 01)
        /// </summary>
        /// <param name="readMemory"></param>
        /// <param name="dataCount"></param>
        /// <returns></returns>
        public ResponceFormat ReadMemory(DeviceElement recvMemory, int dataCount)
        {
            ResponceFormat res = new ResponceFormat();
            List<ushort> recvList = new List<ushort>();
            int count = dataCount;
            int offset = 0;

            while (count > 0)
            {
                DeviceElement Device = recvMemory.Offset(offset);
                int RecvCount;

                // 送信が可能なﾃﾞｰﾀ量か
                if (count <= 200)
                //if (DataCount <= 498)
                {
                    RecvCount = count;

                    count = 0;
                    offset = 0;
                }
                // 分割の必要あり
                else
                {
                    RecvCount = 200;

                    count -= 200;
                    offset += 200;
                }

                #region Fins送信

                List<byte> SendMessage = new List<byte>();

                // ｺﾏﾝﾄﾞｺｰﾄﾞ
                SendMessage.AddRange(new byte[] { 0x01, 0x01 });

                // ﾊﾟﾗﾒｰﾀ種別
                int MemoryType = numManager.GetAreaCode(recvMemory.Prefix);
                SendMessage.Add((byte)(MemoryType & 0x0FF));

                // 読出し開始ｱﾄﾞﾚｽ
                int Address = Device.AddressOffset;
                int Bit = Device.BitOffset;
                SendMessage.AddRange(new byte[] { (byte)((Address >> 8) & 0x0FF), (byte)((Address >> 0) & 0x0FF) });

                // ﾋﾞｯﾄ指定
                if (Bit == -1)
                    SendMessage.Add((byte)0);
                else
                    SendMessage.Add((byte)(Bit & 0x0FF));

                // 読出し要素数
                SendMessage.AddRange(new byte[] { (byte)((RecvCount >> 8) & 0x0FF), (byte)((RecvCount >> 0) & 0x0FF) });

                // 送信
                ResponceFormat result = Send(SendHead, SendMessage.ToArray(), true);


                if (result.IsError == false)
                {
                    recvList.AddRange(ExpandReadMessage(recvMemory, result.ResponceMessage, RecvCount));
                }
                else
                {
                    return result;
                }

                #endregion

            }

            res.IsError = false;
            res.EditMessage = recvList.ToArray();
            return res;

        }

        /// <summary>
        /// I/O情報の読出し (コマンドコード : 01 04)
        /// </summary>
        /// <param name="readMemorys"></param>
        /// <returns></returns>
        public ResponceFormat ReadMemoryRandom(DeviceElement[] readMemorys)
        {
            ResponceFormat res = new ResponceFormat();
            List<ushort> RecvDataList = new List<ushort>();
            List<DeviceElement> ReadDeviceList = new List<DeviceElement>(readMemorys);

            string deviceList = "";
            foreach (DeviceElement ele in ReadDeviceList)
            {
                deviceList = deviceList + ele + ",";

            }

            while (ReadDeviceList.Count > 0)
            {
                int RecvCount;
                IEnumerable<DeviceElement> RecvData;

                // 送信が可能なﾃﾞｰﾀ量か
                if (ReadDeviceList.Count <= ConstValue.MaxRandomReadCount)
                {
                    int DataCount = ReadDeviceList.Count;

                    RecvData = ReadDeviceList.GetRange(0, DataCount);
                    ReadDeviceList.RemoveRange(0, DataCount);

                    RecvCount = DataCount;
                }
                // 分割の必要あり
                else
                {
                    RecvData = ReadDeviceList.GetRange(0, ConstValue.MaxRandomReadCount);
                    ReadDeviceList.RemoveRange(0, ConstValue.MaxRandomReadCount);
                    RecvCount = ConstValue.MaxRandomReadCount;
                }

                #region Finsコマンド発行

                List<byte> SendMessage = new List<byte>();

                // コマンドコード
                SendMessage.AddRange(new byte[] { 0x01, 0x04 });

                // 読出しメモリ
                foreach (DeviceElement ele in RecvData)
                {
                    byte MemoryType = (byte)(numManager.GetAreaCode(ele.Prefix));  // I/Oメモリ種別
                    byte Upper = (byte)((ele.AddressOffset >> 8) & 0x0FF);          // アドレス上位
                    byte Lower = (byte)((ele.AddressOffset >> 0) & 0x0FF);          // アドレス下位
                    byte Bit = (byte)((ele.BitOffset == -1) ? 0 : ele.BitOffset & 0x0FF);
                    // ビット位置

                    SendMessage.AddRange(new byte[] { MemoryType, Upper, Lower, Bit });
                }

                // 送信
                ResponceFormat result = Send(SendHead, SendMessage.ToArray(), true);

                if (result.IsError == false)
                {

                    // ﾒｯｾｰｼﾞを展開し、読出し値配列で返す
                    ushort[] ExpandData = ExpandReadRandomMessage(result.ResponceMessage, RecvCount);
                    RecvDataList.AddRange(ExpandData);
                }
                else
                {
                    return result;
                }

                #endregion

            }

            res.IsError = false;
            res.EditMessage = RecvDataList.ToArray();
            return res;


        }

        /// <summary>
        /// I/O情報の書込み (コマンドコード : 01 02)
        /// </summary>
        /// <param name="writeMemory"></param>
        /// <param name="writeData"></param>
        public ResponceFormat WriteMemory(DeviceElement writeMemory, ushort[] writeData)
        {

            List<byte> SendMessage = new List<byte>();

            // ｺﾏﾝﾄﾞｺｰﾄﾞ
            SendMessage.AddRange(new byte[] { 0x01, 0x02 });

            // ﾊﾟﾗﾒｰﾀ種別
            int AreaCode = numManager.GetAreaCode(writeMemory.Prefix);
            SendMessage.Add((byte)(AreaCode & 0x0FF));

            // 書込み開始ｱﾄﾞﾚｽ
            int Address = writeMemory.AddressOffset;
            int Bit = writeMemory.BitOffset;
            SendMessage.AddRange(new byte[] { (byte)((Address >> 8) & 0x0FF), (byte)((Address >> 0) & 0x0FF) });

            // ﾋﾞｯﾄｱﾄﾞﾚｽ指定
            if (Bit == -1)
                SendMessage.Add((byte)0);
            else
                SendMessage.Add((byte)Bit);

            // 書込みﾜｰﾄﾞ数
            int DataCount = writeData.Length;
            SendMessage.AddRange(new byte[] { (byte)((DataCount >> 8) & 0x0FF), (byte)((DataCount >> 0) & 0x0FF) });

            // 書込みﾃﾞｰﾀ
            for (int i = 0; i < DataCount; i++)
            {
                if (Bit == -1)
                    SendMessage.AddRange(new byte[] { (byte)((writeData[i] >> 8) & 0x0FF), (byte)((writeData[i] >> 0) & 0x0FF) });
                else
                    SendMessage.Add((byte)(writeData[i] & 0x0FF));
            }

            //送信            
            ResponceFormat result = Send(SendHead, SendMessage.ToArray(), false);
            return result;

        }

        public ResponceFormat Send(FinsHead finsHead, byte[] message, bool isRetry)
        {
            //コマンド送信処理
            int sendSize = FinsPort.Send(finsHead, message, message.Length);

            if (sendSize != message.Length)
            {
                ResponceFormat result = new ResponceFormat();
                result.IsError = sendSize != message.Length;
                result.ErrorMessage = "FINS送信失敗";
                return result;
            }

            //レスポンス受信処理

            FinsHead recvFinsHead;
            byte[] recvMessage;

            ResponceFormat res = new ResponceFormat();
            res.IsError = false;

            //int readSize = FinsPort.Receive(out recvFinsHead, out recvMessage, this.TimeOut);

            //// 受信成功
            //if (readSize != 0)
            //{
            //    // レスポンスエラー
            //    if (recvMessage[2] != 0 && recvMessage[3] != 0)
            //    {
            //        res.IsError = true;
            //        res.ErrorMessage = ResponceFormat.ResponseErrorLogOut(recvMessage[2], recvMessage[3]);
            //    }
            //    else
            //    {
            //        res.ResponceHead = recvFinsHead;
            //        res.ResponceMessage = recvMessage;
            //    }
            //}
            //// 受信タイムアウト
            //else
            //{
            //    res.IsError = true;
            //    res.ErrorMessage = "受信タイムアウト";
            //}


            int readSize;
            int errCnt = 0;
            while (errCnt < 3)
            {
                try
                {
                    readSize = FinsPort.Receive(out recvFinsHead, out recvMessage, this.TimeOut);

                    if (readSize != 0)
                    {
                        // レスポンスエラー
                        if (recvMessage[2] != 0 && recvMessage[3] != 0)
                        {
                            res.IsError = true;
                            res.ErrorMessage = ResponceFormat.ResponseErrorLogOut(recvMessage[2], recvMessage[3]);
                        }
                        else
                        {
                            res.ResponceHead = recvFinsHead;
                            res.ResponceMessage = recvMessage;
                        }

                        // 受信完了
                        break;
                    }
                }
                catch
                {
                    // 受信失敗
                }

                errCnt++;
            }

            if (errCnt >= 3)
            {
                res.IsError = true;
                res.ErrorMessage = "受信タイムアウト";
            }

            return res;
        }


        private ushort[] ExpandReadMessage(DeviceElement device, byte[] recvMessage, int dataCount)
        {
            try
            {
                int areaCode;

                // I/O ﾒﾓﾘ種別を取得
                if (device.BitOffset == -1)
                    areaCode = numManager.GetAreaCode(device.Prefix);
                else
                    areaCode = numManager.GetAreaCode(device.Prefix, true);

                // 受信ﾒｯｾｰｼﾞの一要素のﾃﾞｰﾀ長
                int dataLen = numManager.GetDataLength(areaCode);

                ushort[] result = new ushort[dataCount];
                int msgIndex = 0;   // 受信ﾒｯｾｰｼﾞ配列のｲﾝﾃﾞｯｸｽ
                int resIndex = 0;   // 値配列のｲﾝﾃﾞｯｸｽ

                resIndex += 2;  // ｺﾏﾝﾄﾞｺｰﾄﾞ
                resIndex += 2;  // 終了ｺｰﾄﾞ

                switch (dataLen)
                {
                    case 1:
                        for (int i = 0; i < dataCount; i++)
                        {
                            result[msgIndex] = recvMessage[resIndex];

                            msgIndex++;
                            resIndex++;
                        }
                        return result;

                    case 2:
                        for (int i = 0; i < dataCount; i++)
                        {
                            result[msgIndex] = (ushort)((recvMessage[resIndex + 0] << 8) + (recvMessage[resIndex + 1] << 0));

                            msgIndex++;
                            resIndex += 2;
                        }
                        return result;

                    case 4:
                        for (int i = 0; i < dataCount; i++)
                        {
                            result[msgIndex] = (ushort)((recvMessage[resIndex + 2] << 8) + (recvMessage[resIndex + 3] << 0));

                            msgIndex++;
                            resIndex += 4;
                        }
                        return result;

                    default:
                        throw new InvalidOperationException("ExpandReadMessage");   // 発生しない。発生した場合、AreaCodeクラスを疑うこと
                }
            }
            catch (IndexOutOfRangeException)
            {
                // この辺りはﾃﾞﾊﾞｯｸﾞ用
                throw new IndexOutOfRangeException("ExpandReadMessage");
            }
        }

        private ushort[] ExpandReadRandomMessage(byte[] recvMessage, int dataCount)
        {
            int recvMsgSize = recvMessage.Length;

            ushort[] result = new ushort[dataCount];

            int msgIndex = 0;   // 受信ﾒｯｾｰｼﾞ配列のｲﾝﾃﾞｯｸｽ
            int resIndex = 0;   // 値配列のｲﾝﾃﾞｯｸｽ

            msgIndex += 2;  // ｺﾏﾝﾄﾞｺｰﾄﾞ
            msgIndex += 2;  // 終了ｺｰﾄﾞ

            try
            {
                while (msgIndex < recvMsgSize)
                {
                    int index = msgIndex;
                    int memoryNumber = recvMessage[index + 0];


                    // CIOﾋﾞｯﾄ
                    if (memoryNumber == 0x30 || memoryNumber == 0x70)
                    {
                        result[resIndex] = recvMessage[index + 1];
                        msgIndex += 2;
                    }
                    // DMﾋﾞｯﾄ
                    else if (memoryNumber == 0x02)
                    {
                        result[resIndex] = recvMessage[index + 1];
                        msgIndex += 2;
                    }
                    // Wﾋﾞｯﾄ
                    else if (memoryNumber == 0x31)
                    {
                        result[resIndex] = recvMessage[index + 1];
                        msgIndex += 2;
                    }
                    // Hﾋﾞｯﾄ
                    else if (memoryNumber == 0x32)
                    {
                        result[resIndex] = recvMessage[index + 1];
                        msgIndex += 2;
                    }
                    // E0ﾋﾞｯﾄ
                    else if (memoryNumber == 0x20)
                    {
                        result[resIndex] = recvMessage[index + 1];
                        msgIndex += 2;
                    }
                    // CIOﾜｰﾄﾞ
                    else if (memoryNumber == 0xB0 || memoryNumber == 0xF0)
                    {
                        result[resIndex] = (ushort)((recvMessage[index + 1] << 8) + (recvMessage[index + 2] << 0));
                        msgIndex += 3;
                    }
                    // DMﾜｰﾄﾞ
                    else if (memoryNumber == 0x82)
                    {
                        result[resIndex] = (ushort)((recvMessage[index + 1] << 8) + (recvMessage[index + 2] << 0));
                        msgIndex += 3;
                    }
                    // Wﾜｰﾄﾞ
                    else if (memoryNumber == 0xB1)
                    {
                        result[resIndex] = (ushort)((recvMessage[index + 1] << 8) + (recvMessage[index + 2] << 0));
                        msgIndex += 3;
                    }
                    // Hﾜｰﾄﾞ
                    else if (memoryNumber == 0xB2)
                    {
                        result[resIndex] = (ushort)((recvMessage[index + 1] << 8) + (recvMessage[index + 2] << 0));
                        msgIndex += 3;
                    }
                    // E0ﾜｰﾄﾞ
                    else if (memoryNumber == 0xA0)
                    {
                        result[resIndex] = (ushort)((recvMessage[index + 1] << 8) + (recvMessage[index + 2] << 0));
                        msgIndex += 3;
                    }
                    else
                    {
                        return null;
                    }

                    resIndex++;
                }
            }
            catch (IndexOutOfRangeException)
            {
                throw new IndexOutOfRangeException("ExpandReadMessage");
            }

            return result;
        }


        int timeOut;
        /// <summary>
        /// 受信タイムアウト時間
        /// </summary>
        public int TimeOut
        {
            get { return timeOut; }
            set
            {
                if (value < 0)
                {
                    if (value != -1)
                    {
                        string errorMsg = string.Empty;
                        errorMsg += "TimeOut に負の値を設定することは出来ません。\n";
                        errorMsg += "待ち時間を指定しない場合、-1 を設定してください。";

                        throw new ArgumentException(errorMsg);
                    }
                }

                timeOut = value;
            }
        }

        Port finsPort;
        /// <summary>
        /// FINSポート
        /// </summary>
        private Port FinsPort
        {
            get { return finsPort; }
            set { finsPort = value; }
        }

        FinsHead sendHead = new FinsHead();
        /// <summary>
        /// 送信FINSメッセージヘッダ
        /// </summary>
        private FinsHead SendHead
        {
            get { return sendHead; }
            set
            {
                sendHead = value;
            }
        }


        #region IDisposable メンバ

        public void Dispose()
        {
            if (FinsPort != null && FinsPort.IsOpened)
            {
                // 明示的なクローズ
                this.Close();
            }
        }

        #endregion
    }






    public class FinsDeviceNumber
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="plcType">PLCの機種タイプ</param>
        public FinsDeviceNumber(PlcType plcType)
        {
            type = plcType;
        }

        /// <summary>
        /// 指定のエリアのデータ長を取得します。
        /// </summary>
        /// <param name="areaCode">データ長を取得するエリアのエリアコード</param>
        /// <returns>
        /// エリアのデータ長（byte)
        /// </returns>
        public int GetDataLength(int areaCode)
        {
            try
            {
                return GetAreaData(areaCode).DataLen;
            }
            catch (ArgumentException)
            {
                throw new ArgumentException("GetDataLength");
            }
        }

        /// <summary>
        /// 指定のエリアの情報を取得します。
        /// </summary>
        /// <param name="areaCode">エリア情報を取得するエリアのエリアコード</param>
        /// <returns>
        /// エリアのエリア情報
        /// </returns>
        public AreaData GetAreaData(int areaCode)
        {
            switch (areaCode)
            {
                // CIO ﾋﾞｯﾄ
                case 0x30:
                case 0x00:
                    return new AreaData("CIO", areaCode, 1);

                // CIO ﾋﾞｯﾄ (強制ON/OFF付)
                case 0x70:
                case 0x40:
                    return new AreaData("CIO", areaCode, 1);

                // CIO ﾜｰﾄﾞ
                case 0xB0:
                case 0x80:
                    return new AreaData("CIO", areaCode, 2);

                // CIO ﾜｰﾄﾞ (強制ON/OFF付)
                case 0xF0:
                case 0xC0:
                    return new AreaData("CIO", areaCode, 4);

                // DM ﾋﾞｯﾄ
                case 0x02:
                    return new AreaData("DM", areaCode, 1);

                // DM ﾜｰﾄﾞ
                case 0x82:
                    return new AreaData("DM", areaCode, 2);

                //W ﾋﾞｯﾄ
                case 0x31:
                    return new AreaData("W", areaCode, 1);

                //W ﾜｰﾄﾞ
                case 0xB1:
                    return new AreaData("W", areaCode, 2);

                //H ﾋﾞｯﾄ
                case 0x32:
                    return new AreaData("H", areaCode, 1);

                //H ﾜｰﾄﾞ
                case 0xB2:
                    return new AreaData("H", areaCode, 2);

                //E0 ﾋﾞｯﾄ
                case 0x20:
                    return new AreaData("E0", areaCode, 1);

                //E0 ﾜｰﾄﾞ
                case 0xA0:
                    return new AreaData("E0", areaCode, 2);

                default:
                    throw new ArgumentException("GetAreaData");
            }
        }


        public int GetAreaCode(string type)
        {
            return GetAreaCode(type, false, false);
        }
        public int GetAreaCode(string type, bool isBit)
        {
            return GetAreaCode(type, isBit, false);
        }
        public int GetAreaCode(string type, bool isBit, bool isCompelSet)
        {
            string divergeType = type.ToUpper();

            switch (divergeType)
            {
                case "CIO":
                    if (this.PlcType != PlcType.CV)
                    {
                        if (isBit && !isCompelSet)
                            return 0x30;
                        else if (isBit && isCompelSet)
                            return 0x70;
                        else if (!isBit && !isCompelSet)
                            return 0xB0;
                        else if (!isBit && isCompelSet)
                            return 0xF0;
                        else
                            throw new ArgumentException();  // 発生しない
                    }
                    else
                    {
                        if (isBit && !isCompelSet)
                            return 0x00;
                        else if (isBit && isCompelSet)
                            return 0x40;
                        else if (!isBit && !isCompelSet)
                            return 0x80;
                        else if (!isBit && isCompelSet)
                            return 0xC0;
                        else
                            throw new ArgumentException();
                    }

                case "DM":
                    if (this.PlcType != PlcType.CV)
                    {
                        if (isBit)
                            return 0x02;
                        else
                            return 0x82;
                    }
                    else
                    {
                        if (isBit)
                            throw new ArgumentException("CVモードでDMのビットを表現することは出来ません。");
                        else
                            return 0x82;
                    }

                case "E0":
                    if (this.PlcType != PlcType.CV)
                    {
                        if (isBit)
                            return 0x20;
                        else
                            return 0xA0;
                    }
                    else
                    {
                        if (isBit)
                            throw new ArgumentException("CVモードでE0のビットを表現することは出来ません。");
                        else
                            return 0x90;
                    }

                case "W":
                    if (this.PlcType != PlcType.CV)
                    {
                        if (isBit)
                            return 0x31;
                        else
                            return 0xB1;
                    }
                    else
                    {
                        throw new ArgumentException("CVモードでWを表現することは出来ません。");
                    }
                case "H":
                    if (this.PlcType != PlcType.CV)
                    {
                        if (isBit)
                            return 0x32;
                        else
                            return 0xB2;
                    }
                    else
                    {
                        throw new ArgumentException("CVモードでHを表現することは出来ません。");
                    }


                default:
                    throw new ArgumentException("GetAreaCode");
            }
        }

        PlcType type = PlcType.C;
        public PlcType PlcType
        {
            get { return type; }
        }
    }


    public class AreaData
    {
        public string AreaType;
        public int Code;
        public int DataLen;
        public bool IsBit = false;

        public AreaData(string areaType, int code, int dataLen)
        {
            this.AreaType = areaType;
            this.Code = code;
            this.DataLen = dataLen;
        }

        public AreaData(string areaType, int code, int dataLen, bool isBit)
        {
            this.AreaType = areaType;
            this.Code = code;
            this.DataLen = dataLen;
            this.IsBit = isBit;
        }
    }


    /// <summary>
    /// 処理の順番が正しくないとき発生する例外。
    /// </summary>
    public class ProcessException : Exception
    {
        /// <summary>
        /// ProcessException クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="neceProcess">例外が発生した処理を行う前に処理すべきプロセス。</param>
        public ProcessException(string neceProcess) { this.NeceProcess = neceProcess; }
        /// <summary>
        /// ProcessException クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="message">エラーを説明するメッセージ。</param>
        /// <param name="neceProcess">例外が発生した処理を行う前に処理すべきプロセス。</param>
        public ProcessException(string message, string neceProcess) : base(message) { this.NeceProcess = neceProcess; }

        /// <summary>
        /// 例外が発生した処理を行う前に処理すべきプロセス。
        /// </summary>
        public string NeceProcess { get; set; }
    }

    public enum PlcType
    {
        C,
        CV,
        CS,
        CJ,
    }

}
