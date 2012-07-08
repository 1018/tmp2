using System;
using System.Threading;
using CommonClassLibrary;

namespace LocalSimulator.MainProgram
{
    public static class CyclicTimeMonitor
    {
        static Timer CyclicTimeThread = new Timer(CyclicTimeCheck, null, Timeout.Infinite, 1000);

        public static void StartMonitoring()
        {
            CyclicTimeThread.Change(0, 1000);
        }

        public static void StopMonitoring()
        {
            CyclicTimeThread.Change(0, Timeout.Infinite);
        }

        private static void CyclicTimeCheck(object obj)
        {
            if (MainForm.Instance != null)
            {
                long cyclicTime = Remoting.Instance.CommunicationCyclicTime;

                // 実行する処理
                Action setCyclicTime = new Action(() =>
                {
                    MainForm.Instance.toolStrip_lblCyclicTime.Text = "周期 = " + cyclicTime.ToString() + "ms";
                });

                // Invokeを使ってアクセスする必要がある
                if (MainForm.Instance.InvokeRequired)
                {
                    MainForm.Instance.BeginInvoke(setCyclicTime);
                }
                else
                {
                    setCyclicTime();
                }
            }
        }
    }
}
