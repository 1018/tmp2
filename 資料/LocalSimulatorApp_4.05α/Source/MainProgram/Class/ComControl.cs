using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using CommonClassLibrary;
using System.Threading;

namespace PLC_LocalSimulatorApp
{
    public static class ComControl
    {
        private static Process p;

        public static void Start()
        {
            //PLC通信プロセス起動
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = psi.FileName = AppSetting.PlcComPath;
            p = Process.Start(psi);
        }

        public static void End()
        {
            CommonClass Common = CommonClass.Instance;
            Common.PlcComEndOrder = true;
            p.WaitForExit();

        }

        public static bool StatusCheck()
        {
            return p.HasExited;
            //return false;  
        }
    }
}
