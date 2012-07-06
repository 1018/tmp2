using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using CommonClassLibrary;

namespace LocalSimulator.MainProgram
{
    public static class CommunicationControl
    {
        public static bool Start()
        {
            //PLC設定
            object[] setting = ProjectManager.ConnectSetting.ToArray();
            ConnectionMode mode = Global.PlcSetting.FormShow(ref setting);

            if (mode == ConnectionMode.Cancel) { return false; }

            // PLCとの通信を開始する
            Remoting.Instance.ConnectSetting = setting;
            Remoting.Instance.Mode = mode;
            Global.PlcCommunication.Start();

            //PLCと接続できなかった場合は、処理を中断する。
            if (!Remoting.Instance.PlcCommunicationIsConnected) { return false; }
            Global.LogManager.Write("PLC通信開始");

            // KM Communication スレッド開始
            KMCommunication.KMConnection();
            Global.LogManager.Write("KM通信開始");

            // サイクルタイム表示スレッド開始
            CyclicTimeMonitor.StartMonitoring();

            // PLC設定の更新
            ProjectManager.ConnectSetting = new List<object>(setting);

            return true;
        }

        public static void End()
        {
            // PLCとの通信を終了する
            Remoting.Instance.PlcCommunicationEndRequest = true;

            //終了待機
            while (Remoting.Instance.PlcCommunicationIsConnected)
            {
                Application.DoEvents();
                Thread.Sleep(100);
            }

            Global.LogManager.Write("PLC通信終了");

            // KM Communication の処理を終了する
            KMCommunication.KMDisconnection();
            Global.LogManager.Write("KM通信終了");

            // サイクルタイム表示スレッドを終了する

            Global.LogManager.Write("PLC通信処理停止");
        }
    }
}
