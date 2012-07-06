using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;

namespace LocalSimulator.MainProgram
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Process alreadyStartedProcess = GetAlreadyStartedProcess();

            if (alreadyStartedProcess == null)
            {
                DomainExceptionWatcher.StartWatch();

                //Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(MainForm.Instance);

                //DomainExceptionWatcher.EndWatch();
            }
            else
            {
                ActivateProcessProgram(alreadyStartedProcess);
            }
        }

        static Process GetAlreadyStartedProcess()
        {
            // 自身と同名のプロセスを検索
            var compNameProcesses =
                from process in Process.GetProcesses()
                where process.ProcessName == Process.GetCurrentProcess().ProcessName
                select process;

            if (compNameProcesses.Count() > 1)
            {
                return compNameProcesses.First(
                    (process) => process != Process.GetCurrentProcess());
            }
            else
            {
                return null;
            }
        }

        static void ActivateProcessProgram(Process targetProcess)
        {
            Control mainForm = Form.FromHandle(targetProcess.MainWindowHandle);
            if (mainForm != null)
            {
                mainForm.Focus();
            }
        }
    }
}
