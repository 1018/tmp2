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

        Dictionary<string, Action<string>> cpDic = null;
        Dictionary<string, Action<string>> pollingDic = null;
        Dictionary<string, Action<string>> selectingDic = null;

        chData[] ch = new[] { new chData(), 
            new chData(), 
            new chData(), 
            new chData()};   // 4CH分確保

        chData addData = new chData();


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
            System.IO.Stream stream;            
            string line = "";
            string command = "";
            int cpDataCounts = 0;

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


            bool flagPolling = false;
            bool flagCp = false;
            while ((line = sr.ReadLine()) != null)
            {
                // 1.lineをstring[]に格納する
                string[] data = line.Split(' ');

                // ﾁｪｯｸﾎﾞｯｸｽの条件を見るところ
                if (trashMode.Checked)
                {
                    // "占有権"を含んでいるか
                    if (line.Contains(DEL_DEMENSE_LINE)) { continue; }
                    // "->"を含んでいるか
                    if (line.Contains(DEL_ARROW_LINE)) { continue; }
                    // ｴｺｰﾊﾞｯｸ受信は無視する
                    // HP
                    if (line.Contains("RCV 04")) { continue; }
                    // CP
                    if (data.Count() == cpDataCounts - 1) { continue; }
                }

                // ﾛｸﾞが途中で始まった場合の処理
                if (line.Contains("StartLogging SIO.log"))
                {
                    flagPolling = false;
                    flagCp = false;
                    addData.rcvFlag = 0;
                }

                sw.WriteLine(line);

                System.Text.StringBuilder com1 = new System.Text.StringBuilder();
                // 2.SND/RCVを確認する
                if(line.Contains("SND"))
                {
                    // 3.SND/RCV別にｺﾏﾝﾄﾞ位置を確認してCP/HPを確認する
                    // ﾎﾟｰﾘﾝｸﾞ
                    if (line.Contains("4B"))
                    {
                        if (!this.ExtractHp.Checked) { continue; }
                        
                        // data[10]とdata[11]を変換する
                        com1.Append((char)Convert.ToInt32(data[10], 16));
                        com1.Append((char)Convert.ToInt32(data[11], 16));

                        command = com1.ToString();             
                        flagPolling = true;

                        CallPollingDic(command, line); 
                    }
                    // ｾﾚｸﾃｨﾝｸﾞ
                    if(data[5] == "04" && !(line.Contains("4B")))
                    {
                 
                        // data[9]とdata[10]を変換する
                        com1.Append((char)Convert.ToInt32(data[9], 16));
                        com1.Append((char)Convert.ToInt32(data[10], 16));

                        command = com1.ToString();
                        CallSelectingDic(command, line); 
                    }
                    // CP
                    if (data[5] == "02")
                    {

                        // data[7]とdata[8]を変換する
                        com1.Append((char)Convert.ToInt32(data[7], 16));
                        com1.Append((char)Convert.ToInt32(data[8], 16));

                        command = com1.ToString();
                        CallCpDic(command, line);
                        cpDataCounts = data.Count();
                        flagCp = true;

                    }

                }
                else if (line.Contains("RCV"))
                {
                    //// ｴｺｰﾊﾞｯｸ受信は無視する
                    //// HP
                    //if (line.Contains("RCV 04")) { continue; }
                    //// CP
                    //if (data.Count() == cpDataCounts - 1) { continue; }


                    // 3.SND/RCV別にｺﾏﾝﾄﾞ位置を確認してCP/HPを確認する
                    // ﾎﾟｰﾘﾝｸﾞ
                    if (flagPolling)
                    {                        
                        // data[5]とdata[6]を変換する
                        com1.Append((char)Convert.ToInt32(data[5], 16));
                        com1.Append((char)Convert.ToInt32(data[6], 16));

                        command = com1.ToString();
                        CallPollingDic(command, line);
                        flagPolling = false; 
                    }
                    // ｾﾚｸﾃｨﾝｸﾞ
                    if (data[4] == "06" && data.Length == 5)
                    {                        

                        sw.Write(sw.NewLine);
                        continue;
                    }
                    // CP writing
                    if (data[4] == "06" && data.Length == 11)
                    {
                        sw.Write(sw.NewLine);
                        continue;
                    }
                    // CP reading
                    if (flagCp)
                    {                  
                        // data[5]とdata[6]を変換する
                        com1.Append((char)Convert.ToInt32(data[5], 16));
                        com1.Append((char)Convert.ToInt32(data[6], 16));

                        command = com1.ToString();
                        CallCpDic(command, line);
                        flagCp = false; 
                    }   
                }
                else
                {
                    continue;
                }

                addData.data.ForEach((string str) => sw.Write(str));
                sw.Write(sw.NewLine + sw.NewLine);
            }
            //閉じる
            sr.Close();
            sw.Close();
            stream.Close();

        }

        #region CP

        // WM:制御ﾓｰﾄﾞ設定
        private void SplitWM_CP(string line)
        {
            addData.data = new List<string>();
            if (line.Contains("S08"))
            {
                addData.data.Add("    CP:N1[WM] 制御ﾓｰﾄﾞの設定");
            }
            else if (line.Contains("S09"))
            {
                addData.data.Add("    CP:N2[WM] 制御ﾓｰﾄﾞの設定");
            }
            string[] data = line.Split(' ');

            foreach (chData chCnt in ch)
            {
                chCnt.data = new List<string>();
            }
            for (int i = 0; i < 4; i++)
            {
                System.Text.StringBuilder buf = new System.Text.StringBuilder();
                buf.Append((char)Convert.ToInt32(data[9 + i], 16));
                addData.data.Add(" CH");
                addData.data.Add((i + 1).ToString());
                addData.data.Add(" 制御ﾓｰﾄﾞ:");
                addData.data.Add(buf.ToString());
            }

        }

        // RM:制御ﾓｰﾄﾞの取得
        private void SplitRM_CP(string line)
        {
            addData.data = new List<string>();
            if (line.Contains("S08"))
            {
                addData.data.Add("    CP:N1[RM] 制御ﾓｰﾄﾞの取得");
                return;
            }
            else if (line.Contains("S09"))
            {
                addData.data.Add("    CP:N2[RM] 制御ﾓｰﾄﾞの取得");
                return;
            }
            string[] data = line.Split(' ');

            foreach (chData chCnt in ch)
            {
                chCnt.data = new List<string>();
            }

            if (data[5] == "32" && line.Contains("RCV"))
            {
                addData.data.Add("    CP:N1[RM]");
            }
            else if (data[5] == "35" && line.Contains("RCV"))
            {
                addData.data.Add("    CP:N2[RM]");
            }
            for (int i = 0; i < 4; i++)
            {
                System.Text.StringBuilder buf = new System.Text.StringBuilder();
                buf.Append((char)Convert.ToInt32(data[8 + i], 16));
                addData.data.Add(" CH");
                addData.data.Add((i + 1).ToString());
                addData.data.Add(" 制御ﾓｰﾄﾞ:");
                addData.data.Add(buf.ToString());
            }
            return;
        }

        // RR:ｽﾃｰﾀｽの読出し
        private void SplitRR_CP(string line)
        {
            addData.data = new List<string>();
            if (line.Contains("S08"))
            {
                addData.data.Add("    CP:N1[RR] ｽﾃｰﾀｽ読出し");
                return;
            }
            else if (line.Contains("S09"))
            {
                addData.data.Add("    CP:N2[RR] ｽﾃｰﾀｽ読出し");
                return;
            }
            string[] data = line.Split(' ');

            foreach (chData chCnt in ch)
            {
                chCnt.data = new List<string>();
            }

            if (data[5] == "32" && line.Contains("RCV"))
            {
                addData.data.Add("    CP:N1[RR]");
            }
            else if (data[5] == "35" && line.Contains("RCV"))
            {
                addData.data.Add("    CP:N2[RR]");
            }
            for (int i = 0; i < 4; i++)
            {
                System.Text.StringBuilder buf = new System.Text.StringBuilder();
                for (int j = 0; j < 4; j++)
                {
                    buf.Append((char)Convert.ToInt32(data[8 + j + (i * 4)], 16));
                    addData.data.Add(" CH");
                    addData.data.Add((i + 1).ToString());
                    addData.data.Add(" ｽﾃｰﾀｽ:");
                    addData.data.Add(buf.ToString());
                }
            }
            return;
        }

        // RX:制御ｾﾝｻ/外部ｾﾝｻ測定温度の読出し
        private void SplitRX_CP(string line)
        {
            addData.data = new List<string>();
            if (line.Contains("S08"))
            {
                addData.data.Add("    CP:N1[RX] 制御ｾﾝｻ/外部ｾﾝｻ測定温度読出し");
                return;
            }
            else if (line.Contains("S09"))
            {
                addData.data.Add("    CP:N2[RX] 制御ｾﾝｻ/外部ｾﾝｻ測定温度読出し");
                return;
            }
            string[] data = line.Split(' ');

            foreach (chData chCnt in ch)
            {
                chCnt.data = new List<string>();
            }

            if (data[5] == "32" && line.Contains("RCV"))
            {
                addData.data.Add("    CP:N1[RX]");
            }
            else if (data[5] == "35" && line.Contains("RCV"))
            {
                addData.data.Add("    CP:N2[RX]");
            }
            for (int i = 0; i < 4; i++)
            {
                System.Text.StringBuilder buf = new System.Text.StringBuilder();
                for (int j = 0; j < 3; j++)
                {
                    buf.Append((char)Convert.ToInt32(data[8 + j + (i * 3)], 16));
                    addData.data.Add(" CH");
                    addData.data.Add((i + 1).ToString());
                    addData.data.Add(" 制御ｾﾝｻ:");
                    addData.data.Add(buf.ToString());
                }
                for (int k = 0; k < 3; k++)
                {
                    buf.Append((char)Convert.ToInt32(data[11 + k + (i * 3)], 16));
                    addData.data.Add(" CH");
                    addData.data.Add((i + 1).ToString());
                    addData.data.Add(" 外部ｾﾝｻ:");
                    addData.data.Add(buf.ToString());
                }
            }
            return;
        }

        // WB:PID定数及び表示温度校正値(ADJ)の設定
        private void SplitWB_CP(string line)
        {
            addData.data = new List<string>();
            if (line.Contains("S08"))
            {
                addData.data.Add("    CP:N1[WB] PID定数及び表示温度校正値(ADJ)設定");
            }
            else if (line.Contains("S09"))
            {
                addData.data.Add("    CP:N2[WB] PID定数及び表示温度校正値(ADJ)設定");
            }
            string[] data = line.Split(' ');

            foreach (chData chCnt in ch)
            {
                chCnt.data = new List<string>();
            }
            for (int i = 0; i < 4; i++)
            {
                System.Text.StringBuilder buf = new System.Text.StringBuilder();
                for (int p = 0; p < 3; p++)
                {
                    buf.Append((char)Convert.ToInt32(data[9 + p + (i * 3)], 16));
                    addData.data.Add(" CH");
                    addData.data.Add((i + 1).ToString());
                    addData.data.Add(" 比例帯温度幅:");
                    addData.data.Add(buf.ToString());
                }
                for (int _i = 0; _i < 3; _i++)
                {
                    buf.Append((char)Convert.ToInt32(data[12 + _i + (i * 3)], 16));
                    addData.data.Add(" CH");
                    addData.data.Add((i + 1).ToString());
                    addData.data.Add(" 積分時間:");
                    addData.data.Add(buf.ToString());
                }
                for (int d = 0; d < 3; d++)
                {
                    buf.Append((char)Convert.ToInt32(data[15 + d + (i * 3)], 16));
                    addData.data.Add(" CH");
                    addData.data.Add((i + 1).ToString());
                    addData.data.Add(" 微分時間:");
                    addData.data.Add(buf.ToString());
                }
                for (int adj = 0; adj < 3; adj++)
                {
                    buf.Append((char)Convert.ToInt32(data[18 + adj + (i * 4)], 16));
                    addData.data.Add(" CH");
                    addData.data.Add((i + 1).ToString());
                    addData.data.Add(" ｵﾌｾｯﾄ値:");
                    addData.data.Add(buf.ToString());
                }
            }

        }

        // RB:PID定数及びｵﾌｾｯﾄ読出し
        private void SplitRB_CP(string line)
        {
            addData.data = new List<string>();
            if (line.Contains("S08"))
            {
                addData.data.Add("    CP:N1[RB] PID定数及びｵﾌｾｯﾄ読出し");
                return;
            }
            else if (line.Contains("S09"))
            {
                addData.data.Add("    CP:N2[RB] PID定数及びｵﾌｾｯﾄ読出し");
                return;
            }
            string[] data = line.Split(' ');

            foreach (chData chCnt in ch)
            {
                chCnt.data = new List<string>();
            }

            if (data[5] == "32" && line.Contains("RCV"))
            {
                addData.data.Add("    CP:N1[RB]");
            }
            else if (data[5] == "35" && line.Contains("RCV"))
            {
                addData.data.Add("    CP:N2[RB]");
            }
            for (int i = 0; i < 4; i++)
            {
                System.Text.StringBuilder buf = new System.Text.StringBuilder();
                for (int p = 0; p < 3; p++)
                {
                    buf.Append((char)Convert.ToInt32(data[8 + p + (i * 3)], 16));
                    addData.data.Add(" CH");
                    addData.data.Add((i + 1).ToString());
                    addData.data.Add(" 比例帯温度幅:");
                    addData.data.Add(buf.ToString());
                }
                for (int _i = 0; _i < 3; _i++)
                {
                    buf.Append((char)Convert.ToInt32(data[11 + _i + (i * 3)], 16));
                    addData.data.Add(" CH");
                    addData.data.Add((i + 1).ToString());
                    addData.data.Add(" 積分時間:");
                    addData.data.Add(buf.ToString());
                }
                for (int d = 0; d < 3; d++)
                {
                    buf.Append((char)Convert.ToInt32(data[14 + d + (i * 3)], 16));
                    addData.data.Add(" CH");
                    addData.data.Add((i + 1).ToString());
                    addData.data.Add(" 微分時間:");
                    addData.data.Add(buf.ToString());
                }
                for (int adj = 0; adj < 3; adj++)
                {
                    buf.Append((char)Convert.ToInt32(data[17 + adj + (i * 4)], 16));
                    addData.data.Add(" CH");
                    addData.data.Add((i + 1).ToString());
                    addData.data.Add(" ｵﾌｾｯﾄ値:");
                    addData.data.Add(buf.ToString());
                }
            }
            return;
        }

        // WS:目標温度の設定
        private void SplitWS_CP(string line)
        {
            addData.data = new List<string>();
            if (line.Contains("S08"))
            {
                addData.data.Add("    CP:N1[WS] 目標温度設定");
            }
            else if (line.Contains("S09"))
            {
                addData.data.Add("    CP:N2[WS] 目標温度設定");
            }
            string[] data = line.Split(' ');

            foreach (chData chCnt in ch)
            {
                chCnt.data = new List<string>();
            }
            for (int i = 0; i < 4; i++)
            {
                System.Text.StringBuilder buf = new System.Text.StringBuilder();
                for (int j = 0; j < 3; j++)
                {
                    buf.Append((char)Convert.ToInt32(data[9 + j + (i * 3)], 16));
                    addData.data.Add(" CH");
                    addData.data.Add((i + 1).ToString());
                    addData.data.Add(" 目標温度:");
                    addData.data.Add(buf.ToString());
                }
            }
        }

        // RS:目標温度の読出し
        private void SplitRS_CP(string line)
        {
            addData.data = new List<string>();
            if (line.Contains("S08"))
            {
                addData.data.Add("    CP:N1[RS] 目標温度読出し");
                return;
            }
            else if (line.Contains("S09"))
            {
                addData.data.Add("    CP:N2[RS] 目標温度読出し");
                return;
            }
            string[] data = line.Split(' ');

            foreach (chData chCnt in ch)
            {
                chCnt.data = new List<string>();
            }

            if (data[5] == "32" && line.Contains("RCV"))
            {
                addData.data.Add("    CP:N1[RS]");
            }
            else if (data[5] == "35" && line.Contains("RCV"))
            {
                addData.data.Add("    CP:N2[RS]");
            }
            for (int i = 0; i < 4; i++)
            {
                System.Text.StringBuilder buf = new System.Text.StringBuilder();
                for (int j = 0; j < 3; j++)
                {
                    buf.Append((char)Convert.ToInt32(data[8 + j + (i * 3)], 16));
                    addData.data.Add(" CH");
                    addData.data.Add((i + 1).ToString());
                    addData.data.Add(" 目標温度:");
                    addData.data.Add(buf.ToString());
                }
            }
            return;
        }

        // W%:上下限温度幅の設定
        private void SplitWPe_CP(string line)
        {
            addData.data = new List<string>();
            if (line.Contains("S08"))
            {
                addData.data.Add("    CP:N1[W%] 上下温度幅の設定");
            }
            else if (line.Contains("S09"))
            {
                addData.data.Add("    CP:N2[W%] 上下温度幅の設定");
            }
            string[] data = line.Split(' ');

            foreach (chData chCnt in ch)
            {
                chCnt.data = new List<string>();
            }
            for (int i = 0; i < 4; i++)
            {
                System.Text.StringBuilder buf = new System.Text.StringBuilder();
                for (int j = 0; j < 3; j++)
                {
                    buf.Append((char)Convert.ToInt32(data[9 + j + (i * 3)], 16));
                    addData.data.Add(" CH");
                    addData.data.Add((i + 1).ToString());
                    addData.data.Add(" 上限温度幅:");
                    addData.data.Add(buf.ToString());
                }
                for (int k = 0; k < 3; k++)
                {
                    buf.Append((char)Convert.ToInt32(data[12 + k + (i * 3)], 16));
                    addData.data.Add(" CH");
                    addData.data.Add((i + 1).ToString());
                    addData.data.Add(" 下限温度幅:");
                    addData.data.Add(buf.ToString());
                }

            }

        }

        // R%:上下温度幅の読出し
        private void SplitRPe_CP(string line)
        {
            addData.data = new List<string>();
            if (line.Contains("S08"))
            {
                addData.data.Add("    CP:N1[R%] 上下温度幅の読出し");
                return;
            }
            else if (line.Contains("S09"))
            {
                addData.data.Add("    CP:N2[R%] 上下温度幅の読出し");
                return;
            }
            string[] data = line.Split(' ');

            foreach (chData chCnt in ch)
            {
                chCnt.data = new List<string>();
            }

            if (data[5] == "32" && line.Contains("RCV"))
            {
                addData.data.Add("    CP:N1[R%]");
            }
            else if (data[5] == "35" && line.Contains("RCV"))
            {
                addData.data.Add("    CP:N2[R%]");
            }
            for (int i = 0; i < 4; i++)
            {
                System.Text.StringBuilder buf = new System.Text.StringBuilder();
                for (int j = 0; j < 3; j++)
                {
                    buf.Append((char)Convert.ToInt32(data[8 + j + (i * 3)], 16));
                    addData.data.Add(" CH");
                    addData.data.Add((i + 1).ToString());
                    addData.data.Add(" 上限温度幅:");
                    addData.data.Add(buf.ToString());
                }
                for (int k = 0; k < 3; k++)
                {
                    buf.Append((char)Convert.ToInt32(data[11 + k + (i * 3)], 16));
                    addData.data.Add(" CH");
                    addData.data.Add((i + 1).ToString());
                    addData.data.Add(" 下限温度幅:");
                    addData.data.Add(buf.ToString());
                }

            }
            return;
        }

        // WP:Pb(演算開始定数の設定)
        private void SplitWP_CP(string line)
        {
            addData.data = new List<string>();
            if (line.Contains("S08"))
            {
                addData.data.Add("    CP:N1[WP] 演算開始定数設定");
            }
            else if (line.Contains("S09"))
            {
                addData.data.Add("    CP:N2[WP] 演算開始定数設定");
            }
            string[] data = line.Split(' ');

            foreach (chData chCnt in ch)
            {
                chCnt.data = new List<string>();
            }
            for (int i = 0; i < 4; i++)
            {
                System.Text.StringBuilder buf = new System.Text.StringBuilder();
                for (int j = 0; j < 3; j++)
                {
                    buf.Append((char)Convert.ToInt32(data[9 + j + (i * 3)], 16));
                    addData.data.Add(" CH");
                    addData.data.Add((i + 1).ToString());
                    addData.data.Add(" 演算開始定数:");
                    addData.data.Add(buf.ToString());
                }
            }
        }

        // RP:演算開始定数の読出し
        private void SplitRP_CP(string line)
        {
            addData.data = new List<string>();
            if (line.Contains("S08"))
            {
                addData.data.Add("    CP:N1[RP] 演算開始定数読出し");
                return;
            }
            else if (line.Contains("S09"))
            {
                addData.data.Add("    CP:N2[RP] 演算開始定数読出し");
                return;
            }
            string[] data = line.Split(' ');

            foreach (chData chCnt in ch)
            {
                chCnt.data = new List<string>();
            }

            if (data[5] == "32" && line.Contains("RCV"))
            {
                addData.data.Add("    CP:N1[RP]");
            }
            else if (data[5] == "35" && line.Contains("RCV"))
            {
                addData.data.Add("    CP:N2[RP]");
            }
            for (int i = 0; i < 4; i++)
            {
                System.Text.StringBuilder buf = new System.Text.StringBuilder();
                for (int j = 0; j < 3; j++)
                {
                    buf.Append((char)Convert.ToInt32(data[8 + j + (i * 3)], 16));
                    addData.data.Add(" CH");
                    addData.data.Add((i + 1).ToString());
                    addData.data.Add(" 演算開始定数:");
                    addData.data.Add(buf.ToString());
                }
            }
            return;
        }
        #endregion

        #region polling
        // ER:エラーコード：
        private void SplitER_Polling(string line)
        {
            addData.data = new List<string>();
            if (line.Contains("S08") && line.Contains("SND"))
            {
                addData.data.Add("    HP:N1[ER] エラーコード読出し");
                addData.rcvFlag = 1;
                return;
            }
            else if (line.Contains("S09") && line.Contains("SND"))
            {
                addData.data.Add("    HP:N2[ER] エラーコード読出し");
                addData.rcvFlag = 2;
                return;
            }

            if (addData.rcvFlag == 1 && line.Contains("RCV"))
            {
                addData.data.Add("    HP:N1[ER]エラーコード");
                addData.data.Add(line.Substring(42, 13));
                addData.rcvFlag = 0;
                return;
            }
            else if (addData.rcvFlag == 2 && line.Contains("RCV"))
            {
                addData.data.Add("    HP:N2[ER]エラーコード");
                addData.data.Add(line.Substring(42, 13));
                addData.rcvFlag = 0;
                return;
            }
        }

        // C1：ﾘﾓｰﾄ･ﾛｰｶﾙ設定読出し
        private void SplitC1_Polling(string line)
        {
            addData.data = new List<string>();
            if (line.Contains("S08") && line.Contains("SND"))
            {
                addData.data.Add("    HP:N1[C1] ﾘﾓｰﾄ･ﾛｰｶﾙ設定読出し");
                addData.rcvFlag = 1;
                return;
            }
            else if (line.Contains("S09") && line.Contains("SND"))
            {
                addData.data.Add("    HP:N2[C1] ﾘﾓｰﾄ･ﾛｰｶﾙ設定読出し");
                addData.rcvFlag = 2;
                return;
            }
            string[] data = line.Split(' ');

            foreach (chData chCnt in ch)
            {
                chCnt.data = new List<string>();
            }

            if (addData.rcvFlag == 1 && line.Contains("RCV"))
            {
                addData.data.Add("    HP:N1[C1]");
            }
            else if (addData.rcvFlag == 2 && line.Contains("RCV"))
            {
                addData.data.Add("    HP:N2[C1]");
            }
            for (int i = 0; i < 4; i++)
            {
                System.Text.StringBuilder buf = new System.Text.StringBuilder();
                buf.Append((char)Convert.ToInt32(data[10 + (i * 5)], 16));
                addData.data.Add(" CH");
                addData.data.Add((i + 1).ToString());
                addData.data.Add(" ﾘﾓｰﾄ・ﾛｰｶﾙ設定:");
                addData.data.Add(buf.ToString());
            }

            addData.rcvFlag = 0;
            return;

        }
        // S1:設定温度読み出し
        private void SplitS1_Polling(string line)
        {
            addData.data = new List<string>();
            if (line.Contains("S08") && line.Contains("SND"))
            {
                addData.data.Add("    HP:N1[S1] 設定温度読出し");
                addData.rcvFlag = 1;
                return;
            }
            else if (line.Contains("S09") && line.Contains("SND"))
            {
                addData.data.Add("    HP:N2[S1] 設定温度読出し");
                addData.rcvFlag = 2;
                return;
            }

            string[] data = line.Split(' ');

            foreach (chData chCnt in ch)
            {
                chCnt.data = new List<string>();
            }         
            

            if (addData.rcvFlag == 1 && line.Contains("RCV"))
            {
                addData.data.Add("    HP:N1[S1]");
            }
            else if(addData.rcvFlag == 2 && line.Contains("RCV"))
            {
                addData.data.Add("    HP:N2[S1]");
            }
            for (int i = 0; i < 4; i++)
            {
                System.Text.StringBuilder buf = new System.Text.StringBuilder();
                for (int j = 0; j < 5; j++)
                {
                    buf.Append((char)Convert.ToInt32(data[12 + j + (i * 11)], 16));
                }
                addData.data.Add(" CH");
                addData.data.Add((i + 1).ToString());
                addData.data.Add(" 設定温度:");
                addData.data.Add(buf.ToString());                   
            }
 
            addData.rcvFlag = 0;
            return;
        }

        // PID/AT切換
        private void SplitG1_Polling(string line)
        {
            addData.data = new List<string>();
            if (line.Contains("S08") && line.Contains("SND"))
            {
                addData.data.Add("    HP:N1[G1] PID/AT切換");
                addData.rcvFlag = 1;
                return;
            }
            else if (line.Contains("S09") && line.Contains("SND"))
            {
                addData.data.Add("    HP:N2[G1] PID/AT切換");
                addData.rcvFlag = 2;
                return;
            }
            string[] data = line.Split(' ');

            foreach (chData chCnt in ch)
            {
                chCnt.data = new List<string>();
            }

            if (addData.rcvFlag == 1 && line.Contains("RCV"))
            {
                addData.data.Add("    HP:N1[G1]");
            }
            else if (addData.rcvFlag == 2 && line.Contains("RCV"))
            {
                addData.data.Add("    HP:N2[G1]");
            }
            for (int i = 0; i < 4; i++)
            {
                System.Text.StringBuilder buf = new System.Text.StringBuilder();
                buf.Append((char)Convert.ToInt32(data[10 + (i * 5)], 16));
                addData.data.Add(" CH");
                addData.data.Add((i + 1).ToString());
                addData.data.Add(" PID/AT切換:");
                addData.data.Add(buf.ToString());
            }

            addData.rcvFlag = 0;
            return;

        }

        // M1:測定温度
        private void SplitM1_Polling(string line)
        {
            addData.data = new List<string>();
            if (line.Contains("S08") && line.Contains("SND"))
            {
                addData.data.Add("    HP:N1[M1] 測定温度読出し");
                addData.rcvFlag = 1;
                return;
            }
            else if (line.Contains("S09") && line.Contains("SND"))
            {
                addData.data.Add("    HP:N2[M1] 測定温度読出し");
                addData.rcvFlag = 2;
                return;
            }

            string[] data = line.Split(' ');
            foreach (chData chCnt in ch)
            {
                chCnt.data = new List<string>();
            }

            if (addData.rcvFlag == 1 && line.Contains("RCV"))
            {
                addData.data.Add("    HP:N1[M1]");
            }
            else if(addData.rcvFlag == 2 && line.Contains("RCV"))
            {
                addData.data.Add("    HP:N2[M1]");
            }
            for (int i = 0; i < 4; i++)
            {
                System.Text.StringBuilder buf = new System.Text.StringBuilder();
                for (int j = 0; j < 5; j++)
                {
                    buf.Append((char)Convert.ToInt32(data[12 + j + (i * 11)], 16));
                }
                addData.data.Add(" CH");
                addData.data.Add((i + 1).ToString());
                addData.data.Add(" 測定温度:");
                addData.data.Add(buf.ToString());
            }
            addData.rcvFlag = 0;
            return;
        }

        // AJ:総合ｲﾍﾞﾝﾄ状態
        private void SplitAJ_Polling(string line)
        {
            addData.data = new List<string>();
            if (line.Contains("S08") && line.Contains("SND"))
            {
                addData.data.Add("    HP:N1[AJ] 総合ｲﾍﾞﾝﾄ状態読出し");
                addData.rcvFlag = 1;
                return;
            }
            else if (line.Contains("S09") && line.Contains("SND"))
            {
                addData.data.Add("    HP:N2[AJ] 総合ｲﾍﾞﾝﾄ状態読出し");
                addData.rcvFlag = 2;
                return;
            }

            string[] data = line.Split(' ');
            foreach (chData chCnt in ch)
            {
                chCnt.data = new List<string>();
            }

            if (addData.rcvFlag == 1 && line.Contains("RCV"))
            {
                addData.data.Add("    HP:N1[AJ]");
            }
            else if(addData.rcvFlag == 2 && line.Contains("RCV"))
            {
                addData.data.Add("    HP:N2[AJ]");
            }
            for (int i = 0; i < 4; i++)
            {
                System.Text.StringBuilder buf = new System.Text.StringBuilder();
                for (int j = 0; j < 5; j++)
                {
                    buf.Append((char)Convert.ToInt32(data[12 + j + (i * 11)], 16));
                }
                addData.data.Add(" CH");
                addData.data.Add((i + 1).ToString());
                addData.data.Add(" 総合ｲﾍﾞﾝﾄ状態:");
                addData.data.Add(buf.ToString());
            }
            addData.rcvFlag = 0;
            return; 
        }
        #endregion

        #region selecting
        // SR:RUN/STOP設定
        private void SplitSR_Selecting(string line)
        {
            addData.data = new List<string>();
            if (line.Contains("S08") && line.Contains("SND"))
            {
                addData.data.Add("    HP:N1[SR] RUN/STOP設定");
                addData.rcvFlag = 1;
                return;
            }
            else if (line.Contains("S09") && line.Contains("SND"))
            {
                addData.data.Add("    HP:N2[SR] RUN/STOP設定");
                addData.rcvFlag = 2;
                return;
            }
        }

        // C1:HCFLG
        private void SplitC1_Selecting(string line)
        {
            addData.data = new List<string>();
            string[] selectingData = line.Split(' ');
            System.Text.StringBuilder buf = new System.Text.StringBuilder();

            if (line.Contains("S08") && line.Contains("SND"))
            {
                addData.data.Add("    HP:N1[C1] CH");
            }
            else if (line.Contains("S09") && line.Contains("SND"))
            {
                addData.data.Add("    HP:N1[C1] CH");
            }

            buf.Append((char)Convert.ToInt32(selectingData[12], 16));
            addData.data.Add(buf.ToString());
            addData.data.Add("HCFLG     :");
            buf = new System.Text.StringBuilder();
            for (int i = 0; i < 4; i++)
            {
                buf.Append((char)Convert.ToInt32(selectingData[13 + i], 16));
            }
            addData.data.Add(buf.ToString());
            return;
        }

        // S1:温度設定
        private void SplitS1_Selecting(string line)
        {
            addData.data = new List<string>();
            string[] selectingData = line.Split(' ');
            System.Text.StringBuilder buf = new System.Text.StringBuilder();

            if (line.Contains("S08") && line.Contains("SND"))
            {
                addData.data.Add("    HP:N1[S1] CH");
            }
            else if(line.Contains("S09") && line.Contains("SND"))
            {
                addData.data.Add("    HP:N1[S1] CH");
            }

            buf.Append((char)Convert.ToInt32(selectingData[12], 16));
            addData.data.Add(buf.ToString());
            addData.data.Add("目標温度  :");
            buf = new System.Text.StringBuilder();
            for (int i = 0; i < 5; i++)
            {
                buf.Append((char)Convert.ToInt32(selectingData[16 + i], 16));
            }
            addData.data.Add(buf.ToString());
            return;
        }

        // A1:上限温度
        private void SplitA1_Selecting(string line)
        {
            addData.data = new List<string>();
            string[] selectingData = line.Split(' ');
            System.Text.StringBuilder buf = new System.Text.StringBuilder();

            if (line.Contains("S08") && line.Contains("SND"))
            {
                addData.data.Add("    HP:N1[A1] CH");
            }
            else if (line.Contains("S09") && line.Contains("SND"))
            {
                addData.data.Add("    HP:N1[A1] CH");
            }

            buf.Append((char)Convert.ToInt32(selectingData[12], 16));
            addData.data.Add(buf.ToString());
            addData.data.Add("上限温度  :");
            buf = new System.Text.StringBuilder();
            for (int i = 0; i < 4; i++)
            {
                buf.Append((char)Convert.ToInt32(selectingData[17 + i], 16));
            }
            addData.data.Add(buf.ToString());
            return;
        }

        private void SplitA2_Selecting(string line)
        {
            addData.data = new List<string>();
            string[] selectingData = line.Split(' ');
            System.Text.StringBuilder buf = new System.Text.StringBuilder();

            if (line.Contains("S08") && line.Contains("SND"))
            {
                addData.data.Add("    HP:N1[A2] CH");
            }
            else if (line.Contains("S09") && line.Contains("SND"))
            {
                addData.data.Add("    HP:N1[A2] CH");
            }

            buf.Append((char)Convert.ToInt32(selectingData[12], 16));
            addData.data.Add(buf.ToString());
            addData.data.Add("下限温度  :");
            buf = new System.Text.StringBuilder();
            for (int i = 0; i < 4; i++)
            {
                buf.Append((char)Convert.ToInt32(selectingData[17 + i], 16));
            }
            addData.data.Add(buf.ToString());
            return;
        }

        // W1:ｱﾝﾁﾘｾｯﾄﾜｲﾝﾄﾞｱｯﾌﾟ(ARW)
        private void SplitW1_Selecting(string line)
        {
            addData.data = new List<string>();
            string[] selectingData = line.Split(' ');
            System.Text.StringBuilder buf = new System.Text.StringBuilder();

            if (line.Contains("S08") && line.Contains("SND"))
            {
                addData.data.Add("    HP:N1[W1] CH");
            }
            else if (line.Contains("S09") && line.Contains("SND"))
            {
                addData.data.Add("    HP:N1[W1] CH");
            }
            buf.Append((char)Convert.ToInt32(selectingData[12], 16));
            addData.data.Add(buf.ToString());
            addData.data.Add("ｱﾝﾁﾘｾｯﾄﾜｲﾝﾄﾞｱｯﾌﾟ:");
            buf = new System.Text.StringBuilder();
            for (int i = 0; i < 6; i++)
            {
                buf.Append((char)Convert.ToInt32(selectingData[13 + i], 16));
            }
            addData.data.Add(buf.ToString());
            return;
        }

        // J1:ｵｰﾄﾁｭｰﾆﾝｸﾞ(AT)実行
        private void SplitJ1_Selecting(string line)
        {
            addData.data = new List<string>();
            string[] selectingData = line.Split(' ');
            System.Text.StringBuilder buf = new System.Text.StringBuilder();

            if (line.Contains("S08") && line.Contains("SND"))
            {
                addData.data.Add("    HP:N1[J1] CH");
            }
            else if (line.Contains("S09") && line.Contains("SND"))
            {
                addData.data.Add("    HP:N1[J1] CH");
            }
            buf.Append((char)Convert.ToInt32(selectingData[12], 16));
            addData.data.Add(buf.ToString());
            addData.data.Add("ｵｰﾄﾁｭｰﾆﾝｸﾞ:");
            buf = new System.Text.StringBuilder();
            for (int i = 0; i < 2; i++)
            {
                buf.Append((char)Convert.ToInt32(selectingData[13 + i], 16));
            }
            addData.data.Add(buf.ToString());
            return;

        }

        // P1:比例帯
        private void SplitP1_Selecting(string line)
        {
            addData.data = new List<string>();
            string[] selectingData = line.Split(' ');
            System.Text.StringBuilder buf = new System.Text.StringBuilder();

            if (line.Contains("S08") && line.Contains("SND"))
            {
                addData.data.Add("    HP:N1[P1] CH");
            }
            else if (line.Contains("S09") && line.Contains("SND"))
            {
                addData.data.Add("    HP:N1[P1] CH");
            }

            buf.Append((char)Convert.ToInt32(selectingData[12], 16));
            addData.data.Add(buf.ToString());
            addData.data.Add("比例帯    :");
            buf = new System.Text.StringBuilder();
            for (int i = 0; i < 5; i++)
            {
                buf.Append((char)Convert.ToInt32(selectingData[16 + i], 16));
            }
            addData.data.Add(buf.ToString());
            return;
        }

        // I1:積分時間
        private void SplitI1_Selecting(string line)
        {
            addData.data = new List<string>();
            string[] selectingData = line.Split(' ');
            System.Text.StringBuilder buf = new System.Text.StringBuilder();

            if (line.Contains("S08") && line.Contains("SND"))
            {
                addData.data.Add("    HP:N1[I1] CH");
            }
            else if (line.Contains("S09") && line.Contains("SND"))
            {
                addData.data.Add("    HP:N1[I1] CH");
            }

            buf.Append((char)Convert.ToInt32(selectingData[12], 16));
            addData.data.Add(buf.ToString());
            addData.data.Add("積分時間  :");
            buf = new System.Text.StringBuilder();
            for (int i = 0; i < 2; i++)
            {
                buf.Append((char)Convert.ToInt32(selectingData[18 + i], 16));
            }
            addData.data.Add(buf.ToString());
            return;
        }

        // D1:微分時間
        private void SplitD1_Selecting(string line)
        {
            addData.data = new List<string>();
            string[] selectingData = line.Split(' ');
            System.Text.StringBuilder buf = new System.Text.StringBuilder();

            if (line.Contains("S08") && line.Contains("SND"))
            {
                addData.data.Add("    HP:N1[D1] CH");
            }
            else if (line.Contains("S09") && line.Contains("SND"))
            {
                addData.data.Add("    HP:N1[D1] CH");
            }

            buf.Append((char)Convert.ToInt32(selectingData[12], 16));
            addData.data.Add(buf.ToString());
            addData.data.Add("微分時間  :");
            buf = new System.Text.StringBuilder();
            for (int i = 0; i < 2; i++)
            {
                buf.Append((char)Convert.ToInt32(selectingData[18 + i], 16));
            }
            addData.data.Add(buf.ToString());
            return;
        }
        #endregion

        // CH毎のデータ
        public class chData
        {
            public List<string> data { get; set; }
            public int rcvFlag { get; set; }
        }

    }
}

// EOF