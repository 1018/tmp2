using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace LocalSimulator.ProgramSelecter
{   

    static class StartProgram
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {            
            Global.CommandLineDatas = System.Environment.GetCommandLineArgs();

            //コマンドライン引数が有る場合
            if (Global.CommandLineDatas.Count() != 1)
            {
                //Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new SelectForm());
            }
            //コマンドライン引数が無い場合
            else
            {
                //ファイルタイプを取得
                Microsoft.Win32.RegistryKey regKey =
                    Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(Define.ExtensionName);

                if (regKey == null)
                {
                    QuestionAdd();                    
                }
                else
                {                    

                    string fileType = (string) regKey.GetValue("");                

                    //「アクションを実行するアプリケーション」を取得
                    Microsoft.Win32.RegistryKey regKey2 = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(
                                                          string.Format(@"{0}\shell\{1}\command", fileType, Define.ActionName));
                    if (regKey2 == null)
                    {
                        QuestionAdd();                        
                    }
                    else
                    {
                        regKey2.Close();
                        QuestionDelete();
                    }

                    regKey.Close();
                }                

            }
        }
        /// <summary>
        /// 拡張子登録
        /// </summary>
        static void QuestionAdd()
        {
            DialogResult answer = MessageBox.Show("拡張子登録を行いますか？","",MessageBoxButtons.YesNo);

            if(answer == DialogResult.Yes)
            {
                Extension.Add();
            }
        }
        /// <summary>
        /// 拡張子解除
        /// </summary>
        static void QuestionDelete()
        {
            DialogResult answer = MessageBox.Show("拡張子解除を行いますか？", "", MessageBoxButtons.YesNo);

            if (answer == DialogResult.Yes)
            {
                Extension.Delete();
            }
        }

    }
}
