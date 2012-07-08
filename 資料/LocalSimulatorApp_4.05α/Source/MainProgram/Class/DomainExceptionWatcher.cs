using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Net.Mail;

namespace LocalSimulator.MainProgram
{
    static class DomainExceptionWatcher
    {
        public static void StartWatch()
        {
            AppDomain activeAppDomain = Thread.GetDomain();

            activeAppDomain.UnhandledException += new UnhandledExceptionEventHandler(DomainExceptionManager_UnhandledException);
        }

        public static void EndWatch()
        {
            AppDomain activeAppDomain = Thread.GetDomain();

            activeAppDomain.UnhandledException -= new UnhandledExceptionEventHandler(DomainExceptionManager_UnhandledException);
        }

        private static void DomainExceptionManager_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;

            if (ex != null)
            {
                StringBuilder errorMsg = new StringBuilder();
                errorMsg.AppendLine("エラーが発生した為、プログラムを終了します。");
                errorMsg.AppendLine("作成者にキャプチャーしたこの画面を送って下さい。");
                errorMsg.AppendLine("");
                errorMsg.AppendLine("【エラー内容】");
                errorMsg.AppendLine("");
                errorMsg.AppendLine(ex.Message);
                errorMsg.AppendLine("");
                errorMsg.AppendLine("");
                errorMsg.AppendLine("【スタックトレース】");
                errorMsg.AppendLine("");
                errorMsg.AppendLine(ex.StackTrace);

                MessageBox.Show(
                    errorMsg.ToString(), 
                    "プログラム続行不可", 
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                    );

                //SendErrorMail(errorMsg.ToString().Replace("\n", "\r\n"));

                Environment.Exit(-1);
            }
        }

        //private static void SendErrorMail(string mailMsg)
        //{
        //    MailMessage message = new MailMessage();
        //    message.From = new MailAddress("hitomi@kansetsu.co.jp");
        //    message.To.Add("hitomi@kansetsu.co.jp");
        //    message.Subject = "[エラー報告]MainProgram";
        //    message.BodyEncoding = System.Text.Encoding.UTF8;

        //    message.Body = mailMsg;

        //    System.Net.Mail.SmtpClient client = new SmtpClient(System.Net.Dns.GetHostName());
        //    try
        //    {
        //        client.Send(message);
        //    }
        //    catch (Exception e)
        //    {
        //    }
        //}
    }
}
