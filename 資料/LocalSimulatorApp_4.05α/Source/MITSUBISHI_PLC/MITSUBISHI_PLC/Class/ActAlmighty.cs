using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Globalization;
using System.Reflection.Emit;

namespace MITSUBISHI_PLC
{
    public delegate void DeviceStatusEventHandler(string szDevice, int lData, int lReturnCode);

    public class ActAlmighty
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="actInstance">ACTクラスのインスタンス</param>
        public ActAlmighty(object actInstance)
        {
            this.actControlInstance = actInstance;
        }

        /// <summary>
        /// イベント告知
        /// </summary>
        public event DeviceStatusEventHandler OnDeviceStatus
        {
            add
            {
                EventInfo eventInfo = GetEvent("OnDeviceStatus");
                MethodInfo addMethod = eventInfo.GetAddMethod();

                object[] argv = new object[1];
                argv[0] = value;

                addMethod.Invoke(this.ActControlInstance, argv);
            }
            remove
            {
                EventInfo eventInfo = GetEvent("OnDeviceStatus");
                MethodInfo removeMethod = eventInfo.GetRemoveMethod();

                object[] argv = new object[1];
                argv[0] = value;

                removeMethod.Invoke(this.ActControlInstance, argv);
            }
        }


        delegate int ReadDeviceBlockCallback(string szDevice, int lSize, ref int lplData);
        delegate int WriteDeviceBlockCallback(string szDevice, int lSize, ref int lplData);
        delegate int ReadDeviceBlock2Callback(string szDevice, int lSize, ref short lpsData);
        delegate int WriteDeviceBlock2Callback(string szDevice, int lSize, ref short lplData);
        delegate int ReadDeviceRandomCallback(string szDeviceList, int lSize, ref int lplData);
        delegate int WriteDeviceRandomCallback(string szDeviceList, int lSize, ref int lplData);
        delegate int ReadDeviceRandom2Callback(string szDeviceList, int lSize, ref short lpsData);
        delegate int WriteDeviceRandom2Callback(string szDeviceList, int lSize, ref short lpsData);
        delegate int ReadBufferCallback(int lStartIO, int lAddress, int lReadSize, ref short lpsData);
        delegate int WriteBufferCallback(int lStartIO, int lAddress, int lWriteSize, ref short lpsData);
        delegate int EntryDeviceStatusCallback(string szDeviceList, int lSize, int lMonitorCycle, ref int lplData);

        object actControlInstance;


        /// <summary>
        /// 電話回線の接続
        /// </summary>
        /// <returns>
        /// 正常終了：0を返す。
        /// 異常終了：0以外を返す。
        /// </returns>
        public int Connect()
        {
            MethodInfo method = GetMethod("Connect");

            int result = (int)method.Invoke(this.ActControlInstance, null);

            return result;
        }

        /// <summary>
        /// 通信回線のオープン
        /// </summary>
        /// <returns>
        /// 正常終了：0を返す。
        /// 異常終了：0以外を返す。
        /// </returns>
        public int Open()
        {
            MethodInfo method = GetMethod("Open");

            int result = (int)method.Invoke(this.ActControlInstance, null);  

            return result;
        }

        /// <summary>
        /// 通信回線のクローズ
        /// </summary>
        /// <returns>
        /// 正常終了：0を返す。
        /// 異常終了：0以外を返す。
        /// </returns>
        public int Close()
        {
            try
            {
                MethodInfo method = GetMethod("Close");
                int result = (int)method.Invoke(this.ActControlInstance, null);
                return result;
            }
            catch
            {
                return 0xff;
            }
        }

        /// <summary>
        /// 電話回線の切断
        /// </summary>
        /// <returns>
        /// 正常終了：0を返す。
        /// 異常終了：0以外を返す。
        /// </returns>
        public int Disconnect()
        {
            MethodInfo method = GetMethod("Disconnect");

            int result = (int)method.Invoke(this.ActControlInstance, null);

            return result;
        }

        /// <summary>
        /// エラーメッセージの取得
        /// </summary>
        /// <param name="lErrorCode">エラーコード</param>
        /// <param name="lpszErrorMessage">エラーメッセージ</param>
        /// <returns>
        /// 正常終了：0を返す。
        /// 異常終了：0以外を返す。
        /// </returns>
        public int GetErrorMessage(int lErrorCode, out string lpszErrorMessage)
        {
            ACTSUPPORTLib.ActSupportClass supportClass = new ACTSUPPORTLib.ActSupportClass();

            int result = supportClass.GetErrorMessage(lErrorCode, out lpszErrorMessage);

            return result;
        }

        /// <summary>
        /// デバイスの一括読出し
        /// </summary>
        /// <param name="szDevice">デバイス名</param>
        /// <param name="lSize">読出し点数</param>
        /// <param name="lplData">読み出したデバイス値</param>
        /// <returns>
        /// 正常終了：0を返す。
        /// 異常終了：0以外を返す。
        /// </returns>
        public int ReadDeviceBlock(string szDevice, int lSize, int[] lplData)
        {
            ReadDeviceBlockCallback method =
                GetMethodDelegate<ReadDeviceBlockCallback>("ReadDeviceBlock");

            return method(szDevice, lSize, ref lplData[0]);
        }

        /// <summary>
        /// デバイスの一括書込み
        /// </summary>
        /// <param name="szDevice">デバイス名</param>
        /// <param name="lSize">書込み点数</param>
        /// <param name="lplData">書き込むデバイス値</param>
        /// <returns>
        /// 正常終了：0を返す。
        /// 異常終了：0以外を返す。
        /// </returns>
        public int WriteDeviceBlock(string szDevice, int lSize, int[] lplData)
        {
            WriteDeviceBlockCallback method =
                GetMethodDelegate<WriteDeviceBlockCallback>("WriteDeviceBlock");

            return method(szDevice, lSize, ref lplData[0]);
        }

        /// <summary>
        /// デバイスの一括読出し
        /// </summary>
        /// <param name="szDevice">デバイス名</param>
        /// <param name="lSize">読出し点数</param>
        /// <param name="lpsData">読み出したデバイス値</param>
        /// <returns>
        /// 正常終了：0を返す。
        /// 異常終了：0以外を返す。
        /// </returns>
        public int ReadDeviceBlock2(string szDevice, int lSize, short[] lpsData)
        {
            ReadDeviceBlock2Callback method =
                GetMethodDelegate<ReadDeviceBlock2Callback>("ReadDeviceBlock2");

            return method(szDevice, lSize, ref lpsData[0]);
        }

        /// <summary>
        /// デバイスの一括書込み
        /// </summary>
        /// <param name="szDevice">デバイス名</param>
        /// <param name="lSize">書込み点数</param>
        /// <param name="lpsData">書き込むデバイス値</param>
        /// <returns>
        /// 正常終了：0を返す。
        /// 異常終了：0以外を返す。
        /// </returns>
        public int WriteDeviceBlock2(string szDevice, int lSize, short[] lpsData)
        {
            WriteDeviceBlock2Callback method =
                GetMethodDelegate<WriteDeviceBlock2Callback>("WriteDeviceBlock2");

            return method(szDevice, lSize, ref lpsData[0]);
        }

        /// <summary>
        /// デバイスのランダム読出し
        /// </summary>
        /// <param name="szDeviceList">デバイス名</param>
        /// <param name="lSize">読出し点数</param>
        /// <param name="lplData">読み出したデバイス値</param>
        /// <returns>
        /// 正常終了：0を返す。
        /// 異常終了：0以外を返す。
        /// </returns>
        public int ReadDeviceRandom(string szDeviceList, int lSize, int[] lplData)
        {
            ReadDeviceRandomCallback method =
                GetMethodDelegate<ReadDeviceRandomCallback>("ReadDeviceRandom");

            return method(szDeviceList, lSize, ref lplData[0]); ;
        }

        /// <summary>
        /// デバイスのランダム書込み
        /// </summary>
        /// <param name="szDeviceList">デバイス名</param>
        /// <param name="lSize">書込み点数</param>
        /// <param name="lplData">書き込むデバイス値</param>
        /// <returns>
        /// 正常終了：0を返す。
        /// 異常終了：0以外を返す。
        /// </returns>
        public int WriteDeviceRandom(string szDeviceList, int lSize, int[] lplData)
        {
            WriteDeviceRandomCallback method =
                GetMethodDelegate<WriteDeviceRandomCallback>("WriteDeviceRandom");

            return method(szDeviceList, lSize, ref lplData[0]);
        }

        /// <summary>
        /// デバイスのランダム読出し
        /// </summary>
        /// <param name="szDeviceList">デバイス名</param>
        /// <param name="lSize">読出し点数</param>
        /// <param name="lpsData">読み出したデバイス値</param>
        /// <returns>
        /// 正常終了：0を返す。
        /// 異常終了：0以外を返す。
        /// </returns>
        public int ReadDeviceRandom2(string szDeviceList, int lSize, short[] lpsData)
        {
            ReadDeviceRandom2Callback method =
                GetMethodDelegate<ReadDeviceRandom2Callback>("ReadDeviceRandom2");

            return method(szDeviceList, lSize, ref lpsData[0]);
        }

        /// <summary>
        /// デバイスのランダム書込み
        /// </summary>
        /// <param name="szDeviceList">デバイス名</param>
        /// <param name="lSize">書込み点数</param>
        /// <param name="lpsData">書き込むデバイス値</param>
        /// <returns>
        /// 正常終了：0を返す。
        /// 異常終了：0以外を返す。
        /// </returns>
        public int WriteDeviceRandom2(string szDeviceList, int lSize, short[] lpsData)
        {
            WriteDeviceRandom2Callback method =
                GetMethodDelegate<WriteDeviceRandom2Callback>("WriteDeviceRandom2");

            return method(szDeviceList, lSize, ref lpsData[0]);
        }

        /// <summary>
        /// デバイスデータの設定
        /// </summary>
        /// <param name="szDevice">デバイス名</param>
        /// <param name="lData">設定データ</param>
        /// <returns>
        /// 正常終了：0を返す。
        /// 異常終了：0以外を返す。
        /// </returns>
        public int SetDevice(string szDevice, int lData)
        {
            MethodInfo method = GetMethod("SetDevice");

            object[] argv = new object[2];
            argv[0] = szDevice;
            argv[1] = lData;

            int result = (int)method.Invoke(this.ActControlInstance, argv);

            return result;
        }

        /// <summary>
        /// デバイスデータの取得
        /// </summary>
        /// <param name="szDevice">デバイス名</param>
        /// <param name="lData">取得データ</param>
        /// <returns>
        /// 正常終了：0を返す。
        /// 異常終了：0以外を返す。
        /// </returns>
        public int GetDevice(string szDevice, out int lData)
        {
            MethodInfo method = GetMethod("GetMethod");

            object[] argv = new object[2];
            argv[0] = szDevice;

            int result = (int)method.Invoke(this.ActControlInstance, argv);

            lData = (int)argv[1];

            return result;
        }

        /// <summary>
        /// デバイスデータの設定
        /// </summary>
        /// <param name="szDevice">デバイス名</param>
        /// <param name="sData">設定データ</param>
        /// <returns>
        /// 正常終了：0を返す。
        /// 異常終了：0以外を返す。</returns>
        public int SetDevice2(string szDevice, short sData)
        {
            MethodInfo method = GetMethod("SetDevice2");

            object[] argv = new object[2];
            argv[0] = szDevice;
            argv[1] = sData;

            int result = (int)method.Invoke(this.ActControlInstance, argv);

            return result;
        }

        /// <summary>
        /// デバイスデータの取得
        /// </summary>
        /// <param name="szDevice">デバイス名</param>
        /// <param name="sData">取得データ</param>
        /// <returns>
        /// 正常終了：0を返す。
        /// 異常終了：0以外を返す。
        /// </returns>
        public int GetDevice2(string szDevice, out short sData)
        {
            MethodInfo method = GetMethod("GetDevice2");

            object[] argv = new object[2];
            argv[0] = szDevice;

            int result = (int)method.Invoke(this.ActControlInstance, argv);

            sData = (short)argv[1];

            return result;
        }

        /// <summary>
        /// バッファメモリ読出し
        /// </summary>
        /// <param name="lStartIO">値を読み出すユニットの先頭I/O番号</param>
        /// <param name="lAddress">バッファメモリのアドレス</param>
        /// <param name="lReadSize">読み出すサイズ</param>
        /// <param name="lpsData">バッファメモリから読み出した値</param>
        /// <returns>
        /// 正常終了：0を返す。
        /// 異常終了：0以外を返す。
        /// </returns>
        public int ReadBuffer(int lStartIO, int lAddress, int lReadSize, short[] lpsData)
        {
            ReadBufferCallback method =
                GetMethodDelegate<ReadBufferCallback>("ReadBuffer");

            return method(lStartIO, lAddress, lReadSize, ref lpsData[0]);
        }

        /// <summary>
        /// バッファメモリ書込み
        /// </summary>
        /// <param name="lStartIO">値を書き込むユニットの先頭I/O番号</param>
        /// <param name="lAddress">バッファメモリのアドレス</param>
        /// <param name="lWriteSize">書き込むサイズ</param>
        /// <param name="lpsData">バッファメモリに書き込む値</param>
        /// <returns>
        /// 正常終了：0を返す。
        /// 異常終了：0以外を返す。
        /// </returns>
        public int WriteBuffer(int lStartIO, int lAddress, int lWriteSize, short[] lpsData)
        {
            WriteBufferCallback method =
                GetMethodDelegate<WriteBufferCallback>("WriteBuffer");

            return method(lStartIO, lAddress, lWriteSize, ref lpsData[0]);
        }

        /// <summary>
        /// 時計データ読出し
        /// </summary>
        /// <param name="lpsYear">読み出した年の値</param>
        /// <param name="lpsMonth">読み出した月の値</param>
        /// <param name="lpsDay">読み出した日の値</param>
        /// <param name="lpsDayOfWeek">読み出した曜日の値</param>
        /// <param name="lpsHour">読み出した時間の値</param>
        /// <param name="lpsMinute">読み出した分の値</param>
        /// <param name="lpsSecond">読み出した秒の値</param>
        /// <returns>
        /// 正常終了：0を返す。
        /// 異常終了：0以外を返す。
        /// </returns>
        public int GetClockData(out short lpsYear, out short lpsMonth, out short lpsDay,
            out short lpsDayOfWeek, out short lpsHour, out short lpsMinute, out short lpsSecond)
        {
            MethodInfo method = GetMethod("GetClockData");

            object[] argv = new object[7];

            int result = (int)method.Invoke(this.ActControlInstance, argv);

            lpsYear = (short)argv[0];
            lpsMonth = (short)argv[1];
            lpsDay = (short)argv[2];
            lpsDayOfWeek = (short)argv[3];
            lpsHour = (short)argv[4];
            lpsMinute = (short)argv[5];
            lpsSecond = (short)argv[6];

            return result;
        }

        /// <summary>
        /// 時計データ書込み
        /// </summary>
        /// <param name="sYear">書き込む年の値</param>
        /// <param name="sMonth">書き込む月の値</param>
        /// <param name="sDay">書き込む日の値</param>
        /// <param name="sDayOfWeek">書き込む曜日の値</param>
        /// <param name="sHour">書き込む時間の値</param>
        /// <param name="sMinute">書き込む分の値</param>
        /// <param name="sSecond">書き込む秒の値</param>
        /// <returns>
        /// 正常終了：0を返す。
        /// 異常終了：0以外を返す。
        /// </returns>
        public int SetClockData(short sYear, short sMonth, short sDay,
            short sDayOfWeek, short sHour, short sMinute, short sSecond)
        {
            MethodInfo method = GetMethod("SetClockData");

            object[] argv = new object[7];
            argv[0] = sYear;
            argv[1] = sMonth;
            argv[2] = sDay;
            argv[3] = sDayOfWeek;
            argv[4] = sHour;
            argv[5] = sMinute;
            argv[6] = sSecond;

            int result = (int)method.Invoke(this.ActControlInstance, argv);

            return result;
        }

        /// <summary>
        /// シーケンサCPU型名読出し
        /// </summary>
        /// <param name="szCpuName">シーケンサCPU型名文字列</param>
        /// <param name="lplCpuType">シーケンサCPU型名コード</param>
        /// <returns>
        /// 正常終了：0を返す。
        /// 異常終了：0以外を返す。
        /// </returns>
        public int GetCpuType(out string szCpuName, out int lplCpuType)
        {
            MethodInfo method = GetMethod("GetCpuType");

            object[] argv = new object[2];

            int result = (int)method.Invoke(this.ActControlInstance, argv);

            szCpuName = (string)argv[0];
            lplCpuType = (int)argv[1];

            return result;
        }

        /// <summary>
        /// リモートコントロール
        /// </summary>
        /// <param name="lOperation">リモートRUN/STOP/PAUSE</param>
        /// <returns>
        /// 正常終了：0を返す。
        /// 異常終了：0以外を返す。
        /// </returns>
        public int SetCpuStatus(int lOperation)
        {
            MethodInfo method = GetMethod("SetCpuStatus");

            object[] argv = new object[1];
            argv[0] = lOperation;

            int result = (int)method.Invoke(this.ActControlInstance, argv);

            return result;
        }

        /// <summary>
        /// デバイスの状態監視登録
        /// </summary>
        /// <param name="szDeviceList">登録デバイス名リスト</param>
        /// <param name="lSize">登録デバイス点数</param>
        /// <param name="lMonitorCycle">状態監視間隔時間</param>
        /// <param name="lplData">登録デバイス値リスト</param>
        /// <returns>
        /// 正常終了：0を返す。
        /// 異常終了：0以外を返す。
        /// </returns>
        public int EntryDeviceStatus(string szDeviceList, int lSize, int lMonitorCycle, int[] lplData)
        {
            EntryDeviceStatusCallback method =
                GetMethodDelegate<EntryDeviceStatusCallback>("EntryDeviceStatus");

            return method(szDeviceList, lSize, lMonitorCycle, ref lplData[0]);
        }

        /// <summary>
        /// デバイスの状態監視登録解除
        /// </summary>
        /// <returns>
        /// 正常終了：0を返す。
        /// 異常終了：0以外を返す。
        /// </returns>
        public int FreeDeviceStatus()
        {
            MethodInfo method = GetMethod("FreeDeviceStatus");

            int result = (int)method.Invoke(this.ActControlInstance, null);

            return result;
        }


        protected MethodInfo GetMethod(string methodName)
        {
            MethodInfo method = this.ActControlInstance.GetType().GetMethod(methodName);

            if (method == null)
            {
                throw (new InvalidOperationException(
                    string.Format("インスタンスは、メソッド\"{0}\"をサポートしていません。", methodName)));
            }

            return method;
        }

        protected EventInfo GetEvent(string eventName)
        {
            EventInfo eventInfo = this.ActControlInstance.GetType().GetEvent(eventName);

            if (eventInfo == null)
            {
                throw (new InvalidOperationException(
                    string.Format("インスタンスは、イベント\"{0}\"をサポートしていません。", eventName)));
            }

            return eventInfo;
        }

        protected T GetMethodDelegate<T>(string methodName) where T : class
        {
            MethodInfo method = this.ActControlInstance.GetType().GetMethod(methodName);

            if (method == null)
            {
                throw (new InvalidOperationException(
                    string.Format("インスタンスは、メソッド\"{0}\"をサポートしていません。", methodName)));
            }

            return Delegate.CreateDelegate(typeof(T), this.ActControlInstance, method) as T;
        }

        //protected T GetMethodDelegate<T>(string methodName) where T : Delegate
        //{
        //    MethodInfo Method = this.ActControlInstance.GetType().GetMethod(methodName);

        //    if (Method == null)
        //    {
        //        throw (new InvalidOperationException(
        //            string.Format("インスタンスは、メソッド\"{0}\"をサポートしていません。", methodName)));
        //    }
        //    return (T)Delegate.CreateDelegate(typeof(T), this.ActControlInstance, Method);
        //}

        protected object ActControlInstance
        {
            get
            {
                return this.actControlInstance;
            }
        }

    }
}
