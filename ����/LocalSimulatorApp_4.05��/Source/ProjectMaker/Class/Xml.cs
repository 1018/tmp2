using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Xml.Serialization;
using System.Reflection;
using CommonClassLibrary;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace LocalSimulator.ProjectMaker
{
    public static class Xml
    {
        //public static BaseFormDataFormat Load()
        //{

        //    string fileName = OpenDialog();

        //    //XmlSerializerオブジェクトを作成            
        //    //XmlSerializer serializer = new XmlSerializer(typeof(BaseFormDataFormat));
        //    Type[] et = new Type[] { typeof(string[]) };

        //    XmlSerializer serializer = new XmlSerializer(typeof(BaseFormDataFormat), et);
        //    //XmlSerializer serializer = new XmlSerializer(typeof(BaseFormDataFormat), useType.ToArray());

        //    BaseFormDataFormat BaseFormData;

        //    using (FileStream fs = new FileStream(fileName, FileMode.Open))
        //    {
        //        //XMLファイルから読み込み、逆シリアル化する
        //        BaseFormData = (BaseFormDataFormat)serializer.Deserialize(fs);
        //    }

        //    return BaseFormData;
        //}

        //public static void Save(BaseFormDataFormat SaveObj, string fileName)
        //{

        //    Type[] et = new Type[] { typeof(string[]) };

        //    //XmlSerializerオブジェクトを作成
        //    //書き込むオブジェクトの型を指定する
        //    //XmlSerializer serializer = new XmlSerializer(typeof(BaseFormDataFormat));
        //    //XmlSerializer serializer = new XmlSerializer(typeof(BaseFormDataFormat), et);

        //    BinaryFormatter bf = new BinaryFormatter();

        //    //ファイルを開く
        //    FileStream fs = new FileStream(fileName, FileMode.Create);

        //    //シリアル化し、XMLファイルに保存する
        //    bf.Serialize(fs, SaveObj);

        //    //閉じる
        //    fs.Close();


        //}

        private static string OpenDialog()
        {
            //OpenFileDialogクラスのインスタンスを作成
            OpenFileDialog ofd = new OpenFileDialog();

            //はじめに表示されるフォルダを指定する            
            ofd.InitialDirectory = Directory.GetCurrentDirectory();

            //はじめに「ファイル名」で表示される文字列を指定する
            ofd.FileName = Directory.GetFiles(ofd.InitialDirectory, "*.lcs")
                                        .FirstOrDefault() ?? "default.lcs";

            //[ファイルの種類]に表示される選択肢を指定する
            ofd.Filter = "LCSファイル(*.lcs)|*.lcs";

            //[ファイルの種類]ではじめに「すべてのファイル」が選択されているようにする
            //ofd.FilterIndex = 2;

            //タイトルを設定する
            ofd.Title = "開くファイルを選択してください";

            //ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
            ofd.RestoreDirectory = true;

            //存在しないファイルの名前が指定されたとき警告を表示する            
            ofd.CheckFileExists = true;

            //存在しないパスが指定されたとき警告を表示する            
            ofd.CheckPathExists = true;

            //ダイアログを表示する
            if (ofd.ShowDialog() != DialogResult.OK) { return null; }


            return ofd.FileName;

        }

    }
}
