using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LocalSimulator.ProgramSelecter
{
    static class Extension
    {
        public static void Add()
        { 
            //実行するコマンドライン
            string commandline = "\"" + Application.ExecutablePath + "\" " + "\"" +"%1" + "\"";

            ////ファイルタイプ名
            //string fileType = "LocalSimulator";
            ////説明（「ファイルの種類」として表示される）

            ////（必要なし）
            //string description = "LocalSimulator";

            //動詞
            //string verb = "open";
            //動詞の説明（エクスプローラのコンテキストメニューに表示される）

            //（必要なし）
            string verb_description = "開く(&O)";

            //アイコンのパスとインデックス
            //string iconPath = Application.ExecutablePath;
            //int iconIndex = 0;

            //ファイルタイプを登録
            Microsoft.Win32.RegistryKey regkey =
                Microsoft.Win32.Registry.ClassesRoot.CreateSubKey(Define.ExtensionName);
            regkey.SetValue("", Define.ProgramName);
            regkey.Close();

            //ファイルタイプとその説明を登録
            Microsoft.Win32.RegistryKey shellkey =
                Microsoft.Win32.Registry.ClassesRoot.CreateSubKey(Define.ProgramName);
            shellkey.SetValue("", Define.TypeCaption);

            //動詞とその説明を登録
            shellkey = shellkey.CreateSubKey("shell\\" + Define.ActionName);
            shellkey.SetValue("", verb_description);

            //コマンドラインを登録
            shellkey = shellkey.CreateSubKey("command");
            shellkey.SetValue("", commandline);
            shellkey.Close();

            ////アイコンの登録
            //Microsoft.Win32.RegistryKey iconkey =
            //    Microsoft.Win32.Registry.ClassesRoot.CreateSubKey(
            //    fileType + "\\DefaultIcon");
            //iconkey.SetValue("", iconPath + "," + iconIndex.ToString());
            //iconkey.Close();
        }

        public static void Delete()
        {           

            //レジストリキーを削除
            Microsoft.Win32.Registry.ClassesRoot.DeleteSubKeyTree(Define.ExtensionName);
            Microsoft.Win32.Registry.ClassesRoot.DeleteSubKeyTree("LocalSimulator");


        }
    }
}
