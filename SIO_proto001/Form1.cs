using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;

namespace SIO_proto001
{
    public partial class Form1 : Form
    {
        #region constructor
        /// <summary>
        /// コンストラクタ
        /// </summary>

        public Form1()
        {
            InitializeComponent();
        }
        #endregion

        FileDirPath currentFile = new FileDirPath();

        private void updateDisplay()
        {
            Action action = new Action(
                () =>
                {
                    inputFilePath.Text = currentFile.InFile;
                });
        }

        #region ｸﾗｽ・定義
        public class FileDirPath
        {
            public string InFile { get; set;}
            public string OutFile { get; set; }
        }            
        #endregion


        #region ﾌｫｰﾑｲﾍﾞﾝﾄ
        private void selectBtn_Click(object sender, EventArgs e)
        {
            //OpenFileDialogクラスのインスタンスを作成
            OpenFileDialog ofd = new OpenFileDialog();

            //はじめのファイル名を指定する
            //はじめに「ファイル名」で表示される文字列を指定する
            ofd.FileName = "SIO.log";
            //はじめに表示されるフォルダを指定する
            //指定しない（空の文字列）の時は、現在のディレクトリが表示される
            ofd.InitialDirectory = @"D:\nagase\WorkSpace\SIO_proto001\";
            //[ファイルの種類]に表示される選択肢を指定する
            //指定しないとすべてのファイルが表示される
//            ofd.Filter =
//                "HTMLファイル(*.html;*.htm)|*.html;*.htm|すべてのファイル(*.*)|*.*";
            //[ファイルの種類]ではじめに
            //「すべてのファイル」が選択されているようにする
            ofd.FilterIndex = 2;
            //タイトルを設定する
            ofd.Title = "開くファイルを選択してください";
            //ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
            ofd.RestoreDirectory = true;
            //存在しないファイルの名前が指定されたとき警告を表示する
            //デフォルトでTrueなので指定する必要はない
            ofd.CheckFileExists = true;
            //存在しないパスが指定されたとき警告を表示する
            //デフォルトでTrueなので指定する必要はない
            ofd.CheckPathExists = true;

            //ダイアログを表示する
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                //OKボタンがクリックされたとき
                //選択されたファイル名を表示する
//                Console.WriteLine(ofd.FileName);
                currentFile.InFile = ofd.FileName;
                inputFilePath.Text = currentFile.InFile;

                // 出力先
                currentFile.OutFile = outputFilePath.Text;
            }
        }

        
        #endregion

        private void convertBtn_Click(object sender, EventArgs e)
        {

            // ﾌｧｲﾙを複製する
            //OpenFileDialogクラスのインスタンスを作成
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.FileName = inputFilePath.Text;

            //FileInfoオブジェクトを作成
            System.IO.FileInfo fi = new System.IO.FileInfo(ofd.FileName);

            // SIOﾛｸﾞﾌｧｲﾙをｺﾋﾟｰ
            fi.CopyTo(currentFile.OutFile, true);
            ofd.FileName = outputFilePath.Text;



            string line = "";
            ArrayList al = new ArrayList();

            System.IO.Stream stream;
            stream = ofd.OpenFile();          

            if (stream != null)
            {
                //内容を読み込み、表示する
                System.IO.StreamReader sr =
                    new System.IO.StreamReader(stream);

                while ((line = sr.ReadLine()) != null)
                {
                    al.Add(line);                    
                }
//                MessageBox.Show(sr.ReadToEnd());
                //閉じる
                sr.Close();
                stream.Close();

                for (int i = 0; i < al.Count; i++)
                {
                    MessageBox.Show(al[i].ToString());
                }
            } 
        }

    }
}

// EOF