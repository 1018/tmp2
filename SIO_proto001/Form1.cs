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
            InitSplitterDictionary();
        }
        #endregion

        FileDirPath currentFile = new FileDirPath();
        const string DEL_DEMENSE_LINE = "占有権";
        const string DEL_ARROW_LINE = "->";
//        public string addData = "";

        chData[] ch = new[] { new chData(), 
            new chData(), 
            new chData(), 
            new chData()};   // 4CH分確保

        chData addData = new chData();

        Dictionary<string, Action<string>> cpDic = null;
        Dictionary<string, Action<string>> pollingDic = null;
        Dictionary<string, Action<string>> selectingDic = null;

        private void InitSplitterDictionary()

        {
            if (cpDic == null)
            {
                cpDic = new Dictionary<string, Action<string>>();

                cpDic["WM"] = SplitWM_CP;   // 制御ﾓｰﾄﾞ設定
                cpDic["RM"] = SplitRM_CP;   // 制御ﾓｰﾄﾞ取得
                cpDic["RR"] = SplitRR_CP;   // ｽﾃｰﾀｽの読み出し
                cpDic["RX"] = SplitRX_CP;   // 制御ｾﾝｻ測定温度の読み出し
                cpDic["WB"] = SplitWB_CP;   // PID定数及び表示温度校正値(ADJ)の設定
                cpDic["RB"] = SplitRB_CP;   // PID定数及びｵﾌｾｯﾄ読み出し
                cpDic["WS"] = SplitWS_CP;   // 目標温度の設定
                cpDic["RS"] = SplitRS_CP;   // 目標温度の読み出し
                cpDic["W%"] = SplitWPe_CP;  // 上下温度幅の設定
                cpDic["R%"] = SplitRPe_CP;  // 上下温度幅の読み出し
                cpDic["WP"] = SplitWP_CP;   // Pb(演算開始定数)の設定
                cpDic["RP"] = SplitRP_CP;   // 演算開始定数の読み出し
            }
            if (pollingDic == null)
            {
                pollingDic = new Dictionary<string, Action<string>>();
                
                pollingDic["ER"] = SplitER_Polling; // ｴﾗｰｺｰﾄﾞ
                pollingDic["C1"] = SplitC1_Polling; // ﾘﾓｰﾄ･ﾛｰｶﾙ設定読み出し
                pollingDic["S1"] = SplitS1_Polling; // 設定温度読み出し
                pollingDic["G1"] = SplitG1_Polling; // PID/AT切換
                pollingDic["M1"] = SplitM1_Polling; // 測定値読み出し
                pollingDic["AJ"] = SplitAJ_Polling; // 総合ｲﾍﾞﾝﾄ状態

            }
            if (selectingDic == null)
            {
                selectingDic = new Dictionary<string, Action<string>>();

                selectingDic["SR"] = SplitSR_Selecting; // RUN/STOP設定送信
                selectingDic["C1"] = SplitC1_Selecting; // HCFLG
                selectingDic["S1"] = SplitS1_Selecting; // 設定温度
                selectingDic["A1"] = SplitA1_Selecting; // 上限温度
                selectingDic["A2"] = SplitA2_Selecting; // 下限温度
                selectingDic["W1"] = SplitW1_Selecting; // ｱﾝﾁﾘｾｯﾄﾜｲﾝﾄﾞｱｯﾌﾟ(ARW)
                selectingDic["J1"] = SplitJ1_Selecting; // ｵｰﾄﾁｭｰﾆﾝｸﾞ(AT)実行
                selectingDic["P1"] = SplitP1_Selecting; // 比例帯
                selectingDic["I1"] = SplitI1_Selecting; // 積分時間
                selectingDic["D1"] = SplitD1_Selecting; // 微分時間
            }

        }

        private bool CallCpDic(string command, string line)
        {
            if (cpDic.ContainsKey(command))
            {
                cpDic[command].Invoke(line);
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool CallPollingDic(string command, string line)
        {
            if (pollingDic.ContainsKey(command))
            {
                pollingDic[command].Invoke(line);
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool CallSelectingDic(string command, string line)
        {
            if (selectingDic.ContainsKey(command))
            {
                selectingDic[command].Invoke(line);
                return true;
            }
            else
            {
                return false;
            }
        }



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
            ofd.InitialDirectory = @"C:\Users\neko\Documents\Visual Studio 2010\Projects\SIO_proto001\";
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
            System.IO.Stream stream;            
            string line = "";
            bool flagPolling = false;
            string command = "";
            int cpDataCounts = 0;

//            System.Text.StringBuilder com1 = new System.Text.StringBuilder();

            //OpenFileDialogクラスのインスタンスを作成
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.FileName = inputFilePath.Text;
            stream = ofd.OpenFile();

            //FileInfoオブジェクトを作成
            System.IO.FileInfo fi = new System.IO.FileInfo(ofd.FileName);          

            //内容を読み込み、表示する
            System.IO.StreamReader sr =
                new System.IO.StreamReader(
                    stream,
                    System.Text.Encoding.GetEncoding("shift_jis"));
            
            //Shift-Jisでファイルを作成
            System.IO.StreamWriter sw = new System.IO.StreamWriter(
                outputFilePath.Text,
                true,
                System.Text.Encoding.GetEncoding("shift_jis"));
           
                
            while ((line = sr.ReadLine()) != null)
            {
                System.Text.StringBuilder com1 = new System.Text.StringBuilder();

                // ﾁｪｯｸﾎﾞｯｸｽの条件を見るところ
                if (trashMode.Checked)
                {
                    // "占有権"を含んでいるか
                    if (line.Contains(DEL_DEMENSE_LINE)) continue;
                    // "->"を含んでいるか
                    if (line.Contains(DEL_ARROW_LINE)) continue;
                }
                sw.Write(line + sw.NewLine);

 
                // 1.lineをstring[]に格納する
                string[] data = line.Split(' ');

                // 2.SND/RCVを確認する
                if(line.Contains("SND"))
                {
                    // 3.SND/RCV別にｺﾏﾝﾄﾞ位置を確認してCP/HPを確認する
                    // ﾎﾟｰﾘﾝｸﾞ
                    if (line.Contains("4B"))
                    {
                        // data[10]とdata[11]を変換する
                        com1.Append((char)Convert.ToInt32(data[10], 16));
                        com1.Append((char)Convert.ToInt32(data[11], 16));

                        command = com1.ToString();
//                        MessageBox.Show(command);                        
                        flagPolling = true;

                        // 4.それぞれの辞書をｺｰﾙ
                        CallPollingDic(command, line);
                    }
                    // ｾﾚｸﾃｨﾝｸﾞ
                    if(data[5] == "04" && !(line.Contains("4B")))
                    {
                        // data[9]とdata[10]を変換する
                        com1.Append((char)Convert.ToInt32(data[9], 16));
                        com1.Append((char)Convert.ToInt32(data[10], 16));

                        command = com1.ToString();
//                        MessageBox.Show(command);
                        // 4.それぞれの辞書をｺｰﾙ
                        CallSelectingDic(command, line);
                    }
                    // CP
                    if(data[5] == "02")
                    {
                        // data[7]とdata[8]を変換する
                        com1.Append((char)Convert.ToInt32(data[7], 16));
                        com1.Append((char)Convert.ToInt32(data[8], 16));

                        command = com1.ToString();                        
//                        MessageBox.Show(command);

                        // 4.それぞれの辞書をｺｰﾙ
                        CallCpDic(command, line);
                        cpDataCounts = data.Count();
                    }
                }
                if(line.Contains("RCV"))
                {
                    // ｴｺｰﾊﾞｯｸ受信は無視する
                    // HP
                    if (line.Contains("RCV 04")) { continue; }
                    // CP
                    if (data.Count() == cpDataCounts - 1) { continue; }


                    // 3.SND/RCV別にｺﾏﾝﾄﾞ位置を確認してCP/HPを確認する
                    // ﾎﾟｰﾘﾝｸﾞ
                    if (flagPolling)
                    {
                        // data[5]とdata[6]を変換する
                        com1.Append((char)Convert.ToInt32(data[5], 16));
                        com1.Append((char)Convert.ToInt32(data[6], 16));

                        command = com1.ToString();
//                        MessageBox.Show(command);
                        // 4.それぞれの辞書をｺｰﾙ
                        CallPollingDic(command, line);
                    }
                    // ｾﾚｸﾃｨﾝｸﾞ
                    if (data[4] == "06" && data.Length == 5)
                    {
                        continue;
                    }
                    // CP writing
                    if(data[4] == "06" && data.Length == 11)
                    {
                        continue;
                    }
                    // CP reading
                    if (!flagPolling)
                    {
                        // data[5]とdata[6]を変換する
                        com1.Append((char)Convert.ToInt32(data[5], 16));
                        com1.Append((char)Convert.ToInt32(data[6], 16));

                        command = com1.ToString();
//                        MessageBox.Show(command);
                        // 4.それぞれの辞書をｺｰﾙ
                        CallCpDic(command, line);
                    }


                }
                
                // 5.辞書で変換した文字列を書き込む
                sw.WriteLine(addData.data[0]);


                // 変換処理するﾙｰﾁﾝ
                // CPとHPの区別をどうつけるか？

                // SNDのﾎﾟｰﾘﾝｸﾞのｺﾏﾝﾄﾞ位置 == 52-53 55-56(1始まり)   07月04日 13:11:30.815 [U0C P1]SND S08 04 30 31 4B 31 4D 31 05
                // lineに"4B"を含んでいればﾎﾟｰﾘﾝｸﾞのSND。ここでﾎﾟｰﾘﾝｸﾞﾌﾗｸﾞをたてう
                // RCVのﾎﾟｰﾘﾝｸﾞのｺﾏﾝﾄﾞ位置 == 36-37 39-40   07月04日 13:11:31.284 [U0C P1]RCV 02 4D 31 30 31 20 20 20 20 20 30 2E 30 2C 30 32 20 20 20 20 20 30 2E 30 2C 30 33 20 20 20 20 20 30 2E 30 2C 30 34 20 20 20 20 20 30 2E 30 03 57
                // ﾎﾟｰﾘﾝｸﾞﾌﾗｸﾞが建ってたらﾎﾟｰﾘﾝｸﾞのRCV
                
                // SNDのｾﾚｸﾃｨﾝｸﾞのｺﾏﾝﾄﾞ位置 == 49-50 52-53  07月04日 13:11:08.206 [U0C P1]SND S08 04 30 30 02 53 52 31 03 33
                // lineにSNDがあって、SNDの後ろに"04"があって、"4B"を含んでいなければｾﾚｸﾃｨﾝｸﾞのSND
                // RCVのｾﾚｸﾃｨﾝｸﾞのｺﾏﾝﾄﾞ位置 == なし         07月04日 13:35:36.721 [U0C P1]RCV 06
                // lineに"RCV 06"があればｾﾚｸﾃｨﾝｸﾞのRCV
                
                // SNDのCPのWritingのｺﾏﾝﾄﾞ位置 == 43-44 46-47    07月04日 13:11:38.784 [U0C P1]SND S08 02 32 57 53 32 33 30 32 33 30 32 33 30 46 46 46 03 36 44 0D
                // lineに"SND"があって"02"と"57"があれば↑ﾌﾗｸﾞをたてよう
                // RCVのCPのWritingのｺﾏﾝﾄﾞ位置 == なし      　   07月04日 13:34:50.409 [U0C P1]RCV 02 32 06 03 33 38 0D
                // lineに"RCV"があってﾌﾗｸﾞがあれば↑

                // SNDのCPのReadingのｺﾏﾝﾄﾞ位置 == 43-44 46-47   07月04日 13:34:50.721 [U0C P1]SND S08 02 32 52 53 03 44 37 0D
                // lineに"SND"があって"02"と"52"があれば↑ﾌﾗｸﾞをたてよう
                // RCVのCPのReadingのｺﾏﾝﾄﾞ位置 == 39-40 42-43   07月04日 13:34:51.300 [U0C P1]RCV 02 32 52 53 32 33 30 32 33 30 32 33 30 20 20 30 03 20 36 0D
                // lineに"RCV"があってﾌﾗｸﾞがあれば↑




            }
            //閉じる
            sr.Close();
            sw.Close();
            stream.Close();            

        }

        #region CPmsg
        private void SplitWM_CP(string line)
        {
        }

        private void SplitRM_CP(string line)
        {
        }

        private void SplitRR_CP(string line)
        {
        }

        private void SplitRX_CP(string line)
        {
        }

        private void SplitWB_CP(string line)
        {
        }

        private void SplitRB_CP(string line)
        {
        }

        private void SplitWS_CP(string line)
        {
        }

        private void SplitRS_CP(string line)
        {
        }

        private void SplitWPe_CP(string line)
        {
        }

        private void SplitRPe_CP(string line)
        {
        }

        private void SplitWP_CP(string line)
        {
        }

        private void SplitRP_CP(string line)
        {
        }
        #endregion

        #region polling
        // エラーコード：
        private void SplitER_Polling(string line)
        {     
            if (line.Contains("S08") && line.Contains("SND"))
            {
//                addData = "HP:N1[ER] エラーコード読出し".PadLeft(8);
                addData.data.Add("HP:N1[ER] エラーコード読出し".PadLeft(8));
                addData.rcvFlag = 1;
                return;                 
            }
            else if (line.Contains("S09") && line.Contains("SND"))
            {
                addData.data.Add("HP:N2[ER] エラーコード読出し".PadLeft(8));
                addData.rcvFlag = 2;
                return;
            }

            if (addData.rcvFlag == 1 && line.Contains("RCV"))
            {
//                addData = "HP:N1[ER]エラーコード".PadLeft(8) + line.Substring(42, 13);
                addData.data.Add("HP:N1[ER]エラーコード".PadLeft(8) + line.Substring(42, 13));
                addData.rcvFlag = 0;
                return;
            }
            else if (addData.rcvFlag == 2 && line.Contains("RCV"))
            {
                addData.data.Add("HP:N2[ER]エラーコード".PadLeft(8) + line.Substring(42, 13));
                addData.rcvFlag = 0;
                return;
            }
        }

        private void SplitC1_Polling(string line)
        {
        }
        // 設定温度読み出し
        private void SplitS1_Polling(string line)
        {
            addData.data = new List<string>();
            if (line.Contains("S08") && line.Contains("SND"))
            {
                addData.data.Add("HP:N1[S1] 設定温度読出し");
                addData.rcvFlag = 1;
                return;
            }
            else if (line.Contains("S09") && line.Contains("SND"))
            {
                addData.data.Add("HP:N2[S1] 設定温度読出し");
                addData.rcvFlag = 2;
                return;
            }

            System.Text.StringBuilder buf = new System.Text.StringBuilder();
            string[] data = line.Split(' ');
            foreach (int i in ch)
            {
                ch[i].data = new List<string>();
            }

            if (addData.rcvFlag == 1 && line.Contains("RCV"))
            {
                addData.data.Add("HP:N1[S1] ");

                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        buf.Append((char)Convert.ToInt32(data[12 + j + (i * 11)], 16));
                        ch[i].data.Add(buf.ToString());
                    }
                    addData.data.Add("CH" + (i + 1) + " 設定温度：" + ch[i].data);
                }
                addData.rcvFlag = 0;
                return;
            }
            else if (addData.rcvFlag == 2 && line.Contains("RCV"))
            {
                addData.data.Add("HP:N2[S1] ");

                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        buf.Append((char)Convert.ToInt32(data[12 + j + (i * 11)], 16));
                        ch[i].data.Add(buf.ToString());
                    }
                    addData.data.Add("CH" + (i + 1) + " 設定温度：" + ch[i].data);
                }
                addData.rcvFlag = 0;
                return;
            }
            //if (line.Contains("S08") && line.Contains("RCV"))
            //{
            //    addData = "HP:N1[S1] CH1設定温度：".PadLeft(8) + ch1 + '\n' + 
            //        "CH2設定温度：".PadLeft(18) + ch1 + '\n' + 
            //        "CH3設定温度：".PadLeft(18) + ch1 + '\n' + 
            //        "CH4設定温度：".PadLeft(18) + ch1;
            //    return;
            //}
            //else if (line.Contains("S09") && line.Contains("RCV"))
            //{
            //    addData = "HP:N2[S1] CH1設定温度：".PadLeft(8) + ch1 + '\n' +
            //        "CH2設定温度：".PadLeft(18) + ch1 + '\n' +
            //        "CH3設定温度：".PadLeft(18) + ch1 + '\n' +
            //        "CH4設定温度：".PadLeft(18) + ch1;
            //    return;
            //}
        }

        private void SplitG1_Polling(string line)
        {
        }

        private void SplitM1_Polling(string line)
        {
        }

        private void SplitAJ_Polling(string line)
        {
        }
        #endregion

        #region selecting
        private void SplitSR_Selecting(string line)
        {
        }

        private void SplitC1_Selecting(string line)
        {
        }

        private void SplitS1_Selecting(string line)
        {
        }

        private void SplitA1_Selecting(string line)
        {
        }

        private void SplitA2_Selecting(string line)
        {
        }

        private void SplitW1_Selecting(string line)
        {
        }

        private void SplitJ1_Selecting(string line)
        {
        }

        private void SplitP1_Selecting(string line)
        {
        }

        private void SplitI1_Selecting(string line)
        {
        }

        private void SplitD1_Selecting(string line)
        {
        }
        #endregion

        // CH毎のデータ
        public class chData
        {
            public List<string> data {get; set; } 
//            public List<string> data = new List<string>();
            public int rcvFlag { get; set; }

        }



    }
}

// EOF