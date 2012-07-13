#define ALLOW_ALL_FILE
#define M1_INVERSION_MODE

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


namespace SioLog
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

        FileDirPath CurrentDir = new FileDirPath();

        Dictionary<string, Action<string>> cpDic = null;
        Dictionary<string, Action<string>> pollingDic = null;
        Dictionary<string, Action<string>> selectingDic = null;

        ConvertedData NewLine = new ConvertedData();

        NodeNumber CurrentNodeNum = new NodeNumber();

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


        #region ｸﾗｽ・定義
        public class FileDirPath
        {
            public string InFile { get; set;}
            public string OutFile { get; set; }
            public string SelectedPath { get; set; }
        }

        public class ConvertedData
        {
            public List<string> AddData { get; set; }
            public int PollingNodeNum { get; set; }
        }

        // node番号
        public class NodeNumber
        {
            public string Node1 { get; set; }
            public string Node2 { get; set; }
            public string CpNode1 { get; set; }
            public string CpNode2 { get; set; }
        }


        private string[] trashBox = new string[]{
//            "*****"
            "占有権",
            "->",
            "モード",
            "初期設定",
            "NOT",
            "READY",
            "RCV 04",   // ｴｺｰﾊﾞｯｸ受信 
            "RCV 4D"
        };
        #endregion


        #region ﾌｫｰﾑｲﾍﾞﾝﾄ
        /// <summary>
        /// "参照"ﾎﾞﾀﾝｸﾘｯｸ時の動作です
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InputFileSelectBtn(object sender, EventArgs e)
        {
            // OpenFileDialogｸﾗｽのｲﾝｽﾀﾝｽ作成
            OpenFileDialog ofd = new OpenFileDialog();

            // はじめのﾌｧｲﾙ名指定
            ofd.FileName = Constants.LogFileName;
            // はじめに表示されるDir指定(ｶﾚﾝﾄDir)            
            //            ofd.InitialDirectory = @"C\";            
            // [ﾌｧｲﾙの種類]ではじめに
            //「すべてのﾌｧｲﾙ」が選択されているようにする
            ofd.FilterIndex = 2;
            // ﾀｲﾄﾙ設定
            ofd.Title = "開くファイルを選択してください";
            // ﾀﾞｲｱﾛｸﾞﾎﾞｯｸｽを閉じる前に現在のDirを復元するようにする
            ofd.RestoreDirectory = true;
            // 存在しないﾌｧｲﾙの名前が指定されたとき警告を表示する
            ofd.CheckFileExists = true;
            // 存在しないﾊﾟｽが指定されたとき警告を表示する
            ofd.CheckPathExists = true;

            // ﾀﾞｲｱﾛｸﾞ表示
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                CurrentDir.InFile = ofd.FileName;
                inputFilePath.Text = CurrentDir.InFile;
            }

        }

        /// <summary>
        /// 出力先ﾃﾞｨﾚｸﾄﾘを指定します
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OutputFileSelectBtn_Click(object sender, EventArgs e)
        {
            //FolderBrowserDialogクラスのインスタンスを作成
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            //上部に表示する説明テキストを指定する
            fbd.Description = "フォルダを指定してください。";
            //ルートフォルダを指定する
            //デフォルトでDesktop
            fbd.RootFolder = Environment.SpecialFolder.Desktop;
            //最初に選択するフォルダを指定する
            //RootFolder以下にあるフォルダである必要がある
            fbd.SelectedPath = System.IO.Directory.GetCurrentDirectory();
            //ユーザーが新しいフォルダを作成できるようにする
            //デフォルトでTrue
            fbd.ShowNewFolderButton = true;

            //ダイアログを表示する
            if (fbd.ShowDialog(this) == DialogResult.OK)
            {
                CurrentDir.SelectedPath = fbd.SelectedPath;
                //選択されたフォルダを表示する
                this.outputFilePath.Text = CurrentDir.SelectedPath;
            }
        }


        /// <summary>
        /// "読込み"ﾎﾞﾀﾝｸﾘｯｸ時の動作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void convertBtn_Click(object sender, EventArgs e)
        {

            if (!getNodeNumber())
            {
                return;
            }

            // OpenFileDialogｸﾗｽのｲﾝｽﾀﾝｽ作成
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.FileName = inputFilePath.Text;

            System.IO.Stream stream = ofd.OpenFile();
            //内容を読み込む
            System.IO.StreamReader sr =
                new System.IO.StreamReader(
                    stream,
                    System.Text.Encoding.GetEncoding(Constants.FileEncording));


            if (this.outputFilePath.Text == "")
            {
                this.outputFilePath.Text = Directory.GetCurrentDirectory();
            }

            // ﾌｧｲﾙ作成
            System.IO.StreamWriter sw = new System.IO.StreamWriter(
                this.outputFilePath.Text + Constants.OutFileName,
                true,
                System.Text.Encoding.GetEncoding(Constants.FileEncording));

            int cpDataCounts = 0;
            string line = "";
            while ((line = sr.ReadLine()) != null)
            {
                string command = "";
                string[] data = line.Split(' ');

                // node別抽出ﾌｨﾙﾀ
                if (!this.filterNode1.Checked && !this.filterNode2.Checked)
                {
                    //
                }
                else if (!this.filterNode1.Checked)
                {
                    if (data.Length > Constants.MinDataLength)
                    {
                        if (line.Contains(CurrentNodeNum.Node1) ||
                            data[5] == CurrentNodeNum.CpNode1)
                        {
                            continue;
                        }
                    }
                    else if (line.Contains(trashBox[0]) && line.Contains(CurrentNodeNum.Node1))
                    {
                        continue;
                    }
                }
                else if (!this.filterNode2.Checked)
                {
                    if (data.Length > Constants.MinDataLength)
                    {
                        if (line.Contains(CurrentNodeNum.Node2) ||
                            data[5] == CurrentNodeNum.CpNode2)
                        {
                            continue;
                        }
                    }
                    else if (line.Contains(trashBox[0]) && line.Contains(CurrentNodeNum.Node2))
                    {
                        continue;
                    }
                }                              
                
                // すっきりもーど(仮)
                bool result = false;
                if (trashMode.Checked)
                {
                    foreach (string str in trashBox)
                    {
#if M1_INVERSION_MODE
                        // M1の場合
                        if (line.Contains("RCV U0C") && data[7] == "02")
                        {
                            break;
                        }                         
                        else if (line.Contains(str))
                        {                            
                            result = true;
                            break;
                        }
#else
                        if (line.Contains(str))
                        {                            
                            result = true;
                            break;
                        }
#endif

                    }
                    // CPｴｺｰﾊﾞｯｸ
                    if (data.Count() == cpDataCounts - 1)
                    { 
                        continue; 
                    }
                }
                else
                {
                    foreach (string str in trashBox)
                    {
                        if (line.Contains(str))
                        {
                            sw.WriteLine(line + sw.NewLine);
                            result = true;
                            break;
                        }
                    }
                    // CPｴｺｰﾊﾞｯｸ
                    if (data.Count() == cpDataCounts - 1) 
                    {
                        sw.WriteLine(line + sw.NewLine);
                        continue; 
                    }
                }
                if (result) continue;


                System.Text.StringBuilder ComAscii = new System.Text.StringBuilder();

                try
                {                    
                    // 2.SND/RCVを確認する
                    if (line.Contains(Constants.SndMsg))
                    {
                        // ﾎﾟｰﾘﾝｸﾞ
                        if (line.Contains(Constants.PollingMark))
                        {
                            if (this.filterCp.Checked) { continue; }

                            ComAscii.Append((char)Convert.ToInt32(data[Constants.ComPollingSndPosi], 16));
                            ComAscii.Append((char)Convert.ToInt32(data[Constants.ComPollingSndPosi + 1], 16));

                            command = ComAscii.ToString();
                            CallPollingDic(command, line);
                        }
                        // ｾﾚｸﾃｨﾝｸﾞ
                        if (data[5] == Constants.HpMsg && !(line.Contains(Constants.PollingMark)))
                        {
                            if (this.filterCp.Checked) { continue; }

                            ComAscii.Append((char)Convert.ToInt32(data[Constants.ComSelectingPosi], 16));
                            ComAscii.Append((char)Convert.ToInt32(data[Constants.ComSelectingPosi + 1], 16));

                            command = ComAscii.ToString();
                            CallSelectingDic(command, line);
                        }
                        // CP
                        if (data[5] == Constants.CpMsg)
                        {
                            if (this.filterHp.Checked) { continue; }

                            ComAscii.Append((char)Convert.ToInt32(data[Constants.ComCpSndPosi], 16));
                            ComAscii.Append((char)Convert.ToInt32(data[Constants.ComCpSndPosi + 1], 16));

                            command = ComAscii.ToString();
                            CallCpDic(command, line);
                            cpDataCounts = data.Count();
                        }

                    }
                    else if (line.Contains(Constants.RcvMsg))
                    {

                        // ｾﾚｸﾃｨﾝｸﾞ
                        if (data[4] == Constants.ack && data.Length == 5)
                        {
                            if (this.filterCp.Checked) { continue; }
                            sw.WriteLine(line);
                            continue;
                        }
#if M1_INVERSION_MODE
                        // M1:測定温度
                        if (line.Contains("RCV U0C"))
                        {
                            if (this.filterCp.Checked) { continue; }
                            ComAscii.Append((char)Convert.ToInt32(data[8], 16));
                            ComAscii.Append((char)Convert.ToInt32(data[9], 16));

                            command = ComAscii.ToString();
                            CallPollingDic(command, line);

                        }
#endif
                        // ﾎﾟｰﾘﾝｸﾞ
                        if (data[5] != CurrentNodeNum.CpNode1 &&
                           data[5] != CurrentNodeNum.CpNode2 &&
                            data[5] != trashBox[1])
                        {
                            if (this.filterCp.Checked) { continue; }

                            ComAscii.Append((char)Convert.ToInt32(data[Constants.ComPollingRcvPosi], 16));
                            ComAscii.Append((char)Convert.ToInt32(data[Constants.ComPollingRcvPosi + 1], 16));

                            command = ComAscii.ToString();
                            CallPollingDic(command, line);
                        }


                        // CP writing
                        if (data[6] == Constants.ack && data.Length == 11)
                        {
                            if (this.filterHp.Checked) { continue; }
                            sw.WriteLine(line);
                            continue;
                        }
                        // CP reading
                        if (data[5] == CurrentNodeNum.CpNode1 ||
                           data[5] == CurrentNodeNum.CpNode2)
                        {
                            if (this.filterHp.Checked) { continue; }

                            ComAscii.Append((char)Convert.ToInt32(data[Constants.ComCpRcvPosi], 16));
                            ComAscii.Append((char)Convert.ToInt32(data[Constants.ComCpRcvPosi + 1], 16));

                            command = ComAscii.ToString();
                            CallCpDic(command, line);
                        }
                    }
                    else
                    {                 
                        throw new Exception();
                    }
                }            
                catch (Exception ex)
                {
                    continue;
                }
                if (NewLine.AddData[0] != Constants.ErrorCase)                    
                {
                    sw.WriteLine(line);
                    NewLine.AddData.ForEach((string str) => sw.Write(str));
                    sw.Write(sw.NewLine);
                }

            }
            //閉じる
            sr.Close();
            sw.Close();
            stream.Close();
            
            MessageBox.Show("読込みが完了しました。",
                            "完了",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
        }


        #region ﾌｧｲﾙdrag&drop
        /// <summary>
        /// 読込先ﾌｧｲﾙ:テキストボックス内にﾌｧｲﾙをﾄﾞﾗｯｸﾞしたときの動作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void inputFilePath_DragEnter(object sender, DragEventArgs e)
        {
            string[] fileNameArray = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            if (!e.Data.GetDataPresent(System.Windows.Forms.DataFormats.FileDrop))
            {
                // ファイル以外のドラッグは受け入れない
                e.Effect = DragDropEffects.None;
                return;
            }

            if (fileNameArray.Length > 1)
            {
                // 複数ファイルのドラッグは受け入れない
                e.Effect = DragDropEffects.None;
                return;
            }

            if (Path.GetExtension(fileNameArray[0]) != ".log")
            {
                if (Path.GetExtension(fileNameArray[0]) != ".old")
                {

                    // 拡張子が.log、.old以外は受け入れない
                    e.Effect = DragDropEffects.None;
                    return;
                }
            }


            // 上記以外は受け入れる
            e.Effect = DragDropEffects.All;
        }

        
        /// <summary>
        /// 読込先ﾌｧｲﾙﾃｷｽﾄﾎﾞｯｸｽ内にﾌｧｲﾙをﾄﾞﾛｯﾌﾟしたときの動作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void inputFilePath_DragDrop(object sender, DragEventArgs e)
        {
            string[] fileNameArray = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            string fileName = fileNameArray[0];

            this.inputFilePath.Text = fileName;
        }


        /// <summary>
        /// ﾌｫｰﾑ内にﾌｧｲﾙをﾄﾞﾗｯｸﾞした時の動作。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            string[] fileNameArray = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            if (!e.Data.GetDataPresent(System.Windows.Forms.DataFormats.FileDrop))
            {
                // ファイル以外のドラッグは受け入れない
                e.Effect = DragDropEffects.None;
                return;
            }

            if (fileNameArray.Length > 1)
            {
                // 複数ファイルのドラッグは受け入れない
                e.Effect = DragDropEffects.None;
                return;
            }
            if (Path.GetExtension(fileNameArray[0]) != ".log")
            {
                if (Path.GetExtension(fileNameArray[0]) != ".old")
                {

                    // 拡張子が.log、.old以外は受け入れない
                    e.Effect = DragDropEffects.None;
                    return;
                }
            }

            // 上記以外は受け入れる
            e.Effect = DragDropEffects.All;
        }

        /// <summary>
        /// ﾌｫｰﾑ内にﾌｧｲﾙをﾄﾞﾛｯﾌﾟﾞしたときの動作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] fileNameArray = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            string fileName = fileNameArray[0];

            this.inputFilePath.Text = fileName;
        } 

 
        #endregion
        #endregion

        #region ﾒｯｾｰｼﾞ別ﾒｿｯﾄﾞ
        #region CP

        // WM:制御ﾓｰﾄﾞ設定
        private void SplitWM_CP(string line)
        {
            NewLine.AddData = new List<string>();
            // SND
            if (line.Contains(CurrentNodeNum.Node1))
            {
                NewLine.AddData.Add("      CP:N1[WM] 制御ﾓｰﾄﾞの設定\r\n");
            }
            else if (line.Contains(CurrentNodeNum.Node2))
            {
                NewLine.AddData.Add("      CP:N2[WM] 制御ﾓｰﾄﾞの設定\r\n");
            }

            string[] data = line.Split(' ');
            for (int i = 0; i < 4; i++)
            {
                System.Text.StringBuilder buf = new System.Text.StringBuilder();
                buf.Append((char)Convert.ToInt32(data[9 + i], 16));
                NewLine.AddData.Add(Constants.HeaderCh);
                NewLine.AddData.Add((i + 1).ToString());
                NewLine.AddData.Add(" 制御ﾓｰﾄﾞ:");
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("\r\n");
            }
            return;

        }

        // RM:制御ﾓｰﾄﾞの取得
        private void SplitRM_CP(string line)
        {
            NewLine.AddData = new List<string>();
            if (line.Contains(CurrentNodeNum.Node1))
            {
                NewLine.AddData.Add("      CP:N1[RM] 制御ﾓｰﾄﾞの取得\r\n");
                return;
            }
            else if (line.Contains(CurrentNodeNum.Node2))
            {
                NewLine.AddData.Add("      CP:N2[RM] 制御ﾓｰﾄﾞの取得\r\n");
                return;
            }
            string[] data = line.Split(' ');

            if (data[5] == CurrentNodeNum.CpNode1 && line.Contains(Constants.RcvMsg))
            {
                NewLine.AddData.Add("      CP:N1[RM] 制御ﾓｰﾄﾞの取得応答\r\n");
            }
            else if (data[5] == CurrentNodeNum.CpNode2 && line.Contains(Constants.RcvMsg))
            {
                NewLine.AddData.Add("      CP:N2[RM] 制御ﾓｰﾄﾞの取得応答\r\n");
            }
            
            for (int i = 0; i < 4; i++)
            {
                System.Text.StringBuilder buf = new System.Text.StringBuilder();
                buf.Append((char)Convert.ToInt32(data[8 + i], 16));
                NewLine.AddData.Add(Constants.HeaderCh);
                NewLine.AddData.Add((i + 1).ToString());
                NewLine.AddData.Add(Constants.WMRM);
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("\r\n");
            }
            return;
        }

        // RR:ｽﾃｰﾀｽの読出し
        private void SplitRR_CP(string line)
        {
            NewLine.AddData = new List<string>();
            if (line.Contains(CurrentNodeNum.Node1))
            {
                NewLine.AddData.Add("      CP:N1[RR] ｽﾃｰﾀｽ読出し\r\n");
                return;
            }
            else if (line.Contains(CurrentNodeNum.Node2))
            {
                NewLine.AddData.Add("      CP:N2[RR] ｽﾃｰﾀｽ読出し\r\n");
                return;
            }
            string[] data = line.Split(' ');

            if (data[5] == CurrentNodeNum.CpNode1 && line.Contains(Constants.RcvMsg))
            {
                NewLine.AddData.Add("      CP:N1[RR] ｽﾃｰﾀｽ読出し応答\r\n");
            }
            else if (data[5] == CurrentNodeNum.CpNode2 && line.Contains(Constants.RcvMsg))
            {
                NewLine.AddData.Add("      CP:N2[RR] ｽﾃｰﾀｽ読出し応答\r\n");
            }
            for (int i = 0; i < 4; i++)
            {
                System.Text.StringBuilder buf = new System.Text.StringBuilder();
                NewLine.AddData.Add(Constants.HeaderCh);
                NewLine.AddData.Add((i + 1).ToString());
                NewLine.AddData.Add(Constants.RR);
                for (int j = 0; j < 4; j++)
                {
                    buf.Append((char)Convert.ToInt32(data[8 + j + (i * 4)], 16));
                }
                NewLine.AddData.Add("\r\n");
                
            }
            return;
        }

        // RX:制御ｾﾝｻ/外部ｾﾝｻ測定温度の読出し
        private void SplitRX_CP(string line)
        {
            NewLine.AddData = new List<string>();
            if (line.Contains(CurrentNodeNum.Node1))
            {
                NewLine.AddData.Add("      CP:N1[RX] 制御ｾﾝｻ/外部ｾﾝｻ測定温度読出し\r\n");
                return;
            }
            else if (line.Contains(CurrentNodeNum.Node2))
            {
                NewLine.AddData.Add("      CP:N2[RX] 制御ｾﾝｻ/外部ｾﾝｻ測定温度読出し\r\n");
                return;
            }
            string[] data = line.Split(' ');

            if (data[5] == CurrentNodeNum.CpNode1 && line.Contains(Constants.RcvMsg))
            {
                NewLine.AddData.Add("      CP:N1[RX] 制御ｾﾝｻ/外部ｾﾝｻ測定温度読出し応答\r\n");
            }
            else if (data[5] == CurrentNodeNum.CpNode2 && line.Contains(Constants.RcvMsg))
            {
                NewLine.AddData.Add("      CP:N2[RX] 制御ｾﾝｻ/外部ｾﾝｻ測定温度読出し応答\r\n");
            }
            for (int i = 0; i < 4; i++)
            {
                System.Text.StringBuilder buf = new System.Text.StringBuilder();
                NewLine.AddData.Add(Constants.HeaderCh);
                NewLine.AddData.Add((i + 1).ToString());
                NewLine.AddData.Add(Constants.RX1);
                for (int j = 0; j < 3; j++)
                {
                    buf.Append((char)Convert.ToInt32(data[8 + j + (i * 3)], 16));
                }
                NewLine.AddData.Add(buf.ToString());

                buf = new System.Text.StringBuilder();
                NewLine.AddData.Add(Constants.HeaderCh);
                NewLine.AddData.Add((i + 1).ToString());
                NewLine.AddData.Add(Constants.RX2);
                for (int k = 0; k < 3; k++)
                {
                    buf.Append((char)Convert.ToInt32(data[11 + k + (i * 3)], 16));
                }
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("\r\n");
            }
            return;
        }

        // WB:PID定数及び表示温度校正値(ADJ)の設定
        private void SplitWB_CP(string line)
        {
            // SND
            NewLine.AddData = new List<string>();
            if (line.Contains(CurrentNodeNum.Node1))
            {
                NewLine.AddData.Add("      CP:N1[WB] PID定数及び表示温度校正値(ADJ)設定\r\n");
            }
            else if (line.Contains(CurrentNodeNum.Node2))
            {
                NewLine.AddData.Add("      CP:N2[WB] PID定数及び表示温度校正値(ADJ)設定\r\n");
            }
             
            string[] data = line.Split(' ');
            for (int i = 0; i < 4; i++)
            {
                System.Text.StringBuilder buf = new System.Text.StringBuilder();
                NewLine.AddData.Add(Constants.HeaderCh);
                NewLine.AddData.Add((i + 1).ToString());
                NewLine.AddData.Add(Constants.Pb);
                for (int p = 0; p < 3; p++)
                {
                    buf.Append((char)Convert.ToInt32(data[9 + p + (i * 3)], 16));
                }
                NewLine.AddData.Add(buf.ToString());

                buf = new System.Text.StringBuilder();
                NewLine.AddData.Add(Constants.HeaderCh);
                NewLine.AddData.Add((i + 1).ToString());
                NewLine.AddData.Add(Constants.Integral);
                for (int _i = 0; _i < 3; _i++)
                {
                    buf.Append((char)Convert.ToInt32(data[12 + _i + (i * 3)], 16));
                }
                NewLine.AddData.Add(buf.ToString());

                buf = new System.Text.StringBuilder();
                NewLine.AddData.Add(Constants.HeaderCh);
                NewLine.AddData.Add((i + 1).ToString());
                NewLine.AddData.Add(Constants.Differential);                
                for (int d = 0; d < 3; d++)
                {
                    buf.Append((char)Convert.ToInt32(data[15 + d + (i * 3)], 16));
                }
                NewLine.AddData.Add(buf.ToString());

                buf = new System.Text.StringBuilder();
                NewLine.AddData.Add(Constants.HeaderCh);
                NewLine.AddData.Add((i + 1).ToString());
                NewLine.AddData.Add(Constants.ADJ);
                for (int adj = 0; adj < 3; adj++)
                {
                    buf.Append((char)Convert.ToInt32(data[18 + adj + (i * 4)], 16));
                }
                NewLine.AddData.Add(buf.ToString());

                NewLine.AddData.Add("\r\n");
            }
            return;

        }

        // RB:PID定数及びｵﾌｾｯﾄ読出し
        private void SplitRB_CP(string line)
        {
            NewLine.AddData = new List<string>();
            if (line.Contains(CurrentNodeNum.Node1))
            {
                NewLine.AddData.Add("      CP:N1[RB] PID定数及びｵﾌｾｯﾄ読出し\r\n");
                return;
            }
            else if (line.Contains(CurrentNodeNum.Node2))
            {
                NewLine.AddData.Add("      CP:N2[RB] PID定数及びｵﾌｾｯﾄ読出し\r\n");
                return;
            }
            string[] data = line.Split(' ');

            if (data[5] == CurrentNodeNum.CpNode1 && line.Contains(Constants.RcvMsg))
            {
                NewLine.AddData.Add("      CP:N1[RB] PID定数及びｵﾌｾｯﾄ読出し応答\r\n");
            }
            else if (data[5] == CurrentNodeNum.CpNode2 && line.Contains(Constants.RcvMsg))
            {
                NewLine.AddData.Add("      CP:N2[RB] PID定数及びｵﾌｾｯﾄ読出し応答\r\n");
            }
            for (int i = 0; i < 4; i++)
            {
                System.Text.StringBuilder buf = new System.Text.StringBuilder();
                NewLine.AddData.Add(Constants.HeaderCh);
                NewLine.AddData.Add((i + 1).ToString());
                NewLine.AddData.Add(Constants.Pb);
                for (int p = 0; p < 3; p++)
                {
                    buf.Append((char)Convert.ToInt32(data[8 + p + (i * 3)], 16));
                }
                NewLine.AddData.Add(buf.ToString());

                buf = new System.Text.StringBuilder();
                NewLine.AddData.Add(Constants.HeaderCh);
                NewLine.AddData.Add((i + 1).ToString());
                NewLine.AddData.Add(Constants.Integral);
                for (int _i = 0; _i < 3; _i++)
                {
                    buf.Append((char)Convert.ToInt32(data[11 + _i + (i * 3)], 16));
                }
                NewLine.AddData.Add(buf.ToString());

                buf = new System.Text.StringBuilder();
                NewLine.AddData.Add(Constants.HeaderCh);
                NewLine.AddData.Add((i + 1).ToString());
                NewLine.AddData.Add(Constants.Differential);
                for (int d = 0; d < 3; d++)
                {
                    buf.Append((char)Convert.ToInt32(data[14 + d + (i * 3)], 16));
                }
                NewLine.AddData.Add(buf.ToString());

                buf = new System.Text.StringBuilder();
                NewLine.AddData.Add(Constants.HeaderCh);
                NewLine.AddData.Add((i + 1).ToString());
                NewLine.AddData.Add(Constants.ADJ);
                for (int adj = 0; adj < 3; adj++)
                {
                    buf.Append((char)Convert.ToInt32(data[17 + adj + (i * 4)], 16));
                }
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("\r\n");
            }
            return;
        }

        // WS:目標温度の設定
        private void SplitWS_CP(string line)
        {
            NewLine.AddData = new List<string>();
            // SND
            if (line.Contains(CurrentNodeNum.Node1))
            {
                NewLine.AddData.Add("      CP:N1[WS] 目標温度設定\r\n");
            }
            else if (line.Contains(CurrentNodeNum.Node2))
            {
                NewLine.AddData.Add("      CP:N2[WS] 目標温度設定\r\n");
            }
        
            string[] data = line.Split(' ');
            for (int i = 0; i < 4; i++)
            {
                System.Text.StringBuilder buf = new System.Text.StringBuilder();
                NewLine.AddData.Add(Constants.HeaderCh);
                NewLine.AddData.Add((i + 1).ToString());
                NewLine.AddData.Add(Constants.WSRS);
                for (int j = 0; j < 3; j++)
                {
                    buf.Append((char)Convert.ToInt32(data[9 + j + (i * 3)], 16));
                }
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("\r\n");
            }
            return;
        }

        // RS:目標温度の読出し
        private void SplitRS_CP(string line)
        {
            NewLine.AddData = new List<string>();
            if (line.Contains(CurrentNodeNum.Node1))
            {
                NewLine.AddData.Add("      CP:N1[RS] 目標温度読出し\r\n");
                return;
            }
            else if (line.Contains(CurrentNodeNum.Node2))
            {
                NewLine.AddData.Add("      CP:N2[RS] 目標温度読出し\r\n");
                return;
            }
            string[] data = line.Split(' ');

            if (data[5] == CurrentNodeNum.CpNode1 && line.Contains(Constants.RcvMsg))
            {
                NewLine.AddData.Add("      CP:N1[RS] 目標温度読出し応答\r\n");
            }
            else if (data[5] == CurrentNodeNum.CpNode2 && line.Contains(Constants.RcvMsg))
            {
                NewLine.AddData.Add("      CP:N2[RS] 目標温度読出し応答\r\n");
            }
            for (int i = 0; i < 4; i++)
            {
                System.Text.StringBuilder buf = new System.Text.StringBuilder();
                NewLine.AddData.Add(Constants.HeaderCh);
                NewLine.AddData.Add((i + 1).ToString());
                NewLine.AddData.Add(Constants.WSRS);
                for (int j = 0; j < 3; j++)
                {
                    buf.Append((char)Convert.ToInt32(data[8 + j + (i * 3)], 16));
                }
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("\r\n");
            }
            return;
        }

        // W%:上下限温度幅の設定
        private void SplitWPe_CP(string line)
        {
            NewLine.AddData = new List<string>();
            // SND
            if (line.Contains(CurrentNodeNum.Node1))
            {
                NewLine.AddData.Add("      CP:N1[W%] 上下温度幅の設定\r\n");
            }
            else if (line.Contains(CurrentNodeNum.Node2))
            {
                NewLine.AddData.Add("      CP:N2[W%] 上下温度幅の設定\r\n");
            }

            string[] data = line.Split(' ');            
            for (int i = 0; i < 4; i++)
            {
                System.Text.StringBuilder buf = new System.Text.StringBuilder();
                NewLine.AddData.Add(Constants.HeaderCh);
                NewLine.AddData.Add((i + 1).ToString());
                NewLine.AddData.Add(Constants.UpperTempWidth);
                for (int j = 0; j < 3; j++)
                {
                    buf.Append((char)Convert.ToInt32(data[9 + j + (i * 3)], 16));
                }
                NewLine.AddData.Add(buf.ToString());

                buf = new System.Text.StringBuilder();
                NewLine.AddData.Add(Constants.HeaderCh);
                NewLine.AddData.Add((i + 1).ToString());
                NewLine.AddData.Add(Constants.LowerTempWidth);
                for (int k = 0; k < 3; k++)
                {
                    buf.Append((char)Convert.ToInt32(data[12 + k + (i * 3)], 16));                    
                }
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("\r\n");

            }
            return;

        }

        // R%:上下温度幅の読出し
        private void SplitRPe_CP(string line)
        {
            NewLine.AddData = new List<string>();
            if (line.Contains(CurrentNodeNum.Node1))
            {
                NewLine.AddData.Add("      CP:N1[R%] 上下温度幅の読出し\r\n");
                return;
            }
            else if (line.Contains(CurrentNodeNum.Node2))
            {
                NewLine.AddData.Add("      CP:N2[R%] 上下温度幅の読出し\r\n");
                return;
            }
            string[] data = line.Split(' ');
 
            if (data[5] == CurrentNodeNum.CpNode1 && line.Contains(Constants.RcvMsg))
            {
                NewLine.AddData.Add("      CP:N1[R%] 上下温度幅の読出し応答\r\n");
            }
            else if (data[5] == CurrentNodeNum.CpNode2 && line.Contains(Constants.RcvMsg))
            {
                NewLine.AddData.Add("      CP:N2[R%] 上下温度幅の読出し応答\r\n");
            }
            for (int i = 0; i < 4; i++)
            {
                System.Text.StringBuilder buf = new System.Text.StringBuilder();
                NewLine.AddData.Add(Constants.HeaderCh);
                NewLine.AddData.Add((i + 1).ToString());
                NewLine.AddData.Add(Constants.UpperTempWidth);
                for (int j = 0; j < 3; j++)
                {
                    buf.Append((char)Convert.ToInt32(data[8 + j + (i * 3)], 16));                    
                }
                NewLine.AddData.Add(buf.ToString());

                buf = new System.Text.StringBuilder();
                NewLine.AddData.Add(Constants.HeaderCh);
                NewLine.AddData.Add((i + 1).ToString());
                NewLine.AddData.Add(Constants.LowerTempWidth);
                for (int k = 0; k < 3; k++)
                {
                    buf.Append((char)Convert.ToInt32(data[11 + k + (i * 3)], 16));
                }
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("\r\n");

            }
            return;
        }

        // WP:Pb(演算開始定数の設定)
        private void SplitWP_CP(string line)
        {
            NewLine.AddData = new List<string>();
            // SND
            if (line.Contains(CurrentNodeNum.Node1))
            {
                NewLine.AddData.Add("      CP:N1[WP] 演算開始定数設定\r\n");
            }
            else if (line.Contains(CurrentNodeNum.Node2))
            {
                NewLine.AddData.Add("      CP:N2[WP] 演算開始定数設定\r\n");
            }
            
            string[] data = line.Split(' ');
            for (int i = 0; i < 4; i++)
            {
                System.Text.StringBuilder buf = new System.Text.StringBuilder();
                NewLine.AddData.Add(Constants.HeaderCh);
                NewLine.AddData.Add((i + 1).ToString());
                NewLine.AddData.Add(Constants.WPRP);
                for (int j = 0; j < 3; j++)
                {
                    buf.Append((char)Convert.ToInt32(data[9 + j + (i * 3)], 16));
                }
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("\r\n");
            }
            return;
        }

        // RP:演算開始定数の読出し
        private void SplitRP_CP(string line)
        {
            NewLine.AddData = new List<string>();
            if (line.Contains(CurrentNodeNum.Node1))
            {
                NewLine.AddData.Add("      CP:N1[RP] 演算開始定数読出し\r\n");
                return;
            }
            else if (line.Contains(CurrentNodeNum.Node2))
            {
                NewLine.AddData.Add("      CP:N2[RP] 演算開始定数読出し\r\n");
                return;
            }
            string[] data = line.Split(' ');

            if (data[5] == CurrentNodeNum.CpNode1 && line.Contains(Constants.RcvMsg))
            {
                NewLine.AddData.Add("      CP:N1[RP] 演算開始定数読出し応答\r\n");
            }
            else if (data[5] == CurrentNodeNum.CpNode2 && line.Contains(Constants.RcvMsg))
            {
                NewLine.AddData.Add("      CP:N2[RP] 演算開始定数読出し応答\r\n");
            }
            for (int i = 0; i < 4; i++)
            {
                System.Text.StringBuilder buf = new System.Text.StringBuilder();
                NewLine.AddData.Add(Constants.HeaderCh);
                NewLine.AddData.Add((i + 1).ToString());
                NewLine.AddData.Add(Constants.WPRP);
                for (int j = 0; j < 3; j++)
                {
                    buf.Append((char)Convert.ToInt32(data[8 + j + (i * 3)], 16));
                }
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("\r\n");
            }
            return;
        }
        #endregion

        #region polling
        // ER:エラーコード：
        private void SplitER_Polling(string line)
        {
            NewLine.AddData = new List<string>();
            if (line.Contains(CurrentNodeNum.Node1) && line.Contains(Constants.SndMsg))
            {
                NewLine.AddData.Add("      HP:N1[ER] エラーコード読出し\r\n");
                NewLine.PollingNodeNum = 1;
                return;
            }
            else if (line.Contains(CurrentNodeNum.Node2) && line.Contains(Constants.SndMsg))
            {
                NewLine.AddData.Add("      HP:N2[ER] エラーコード読出し\r\n");
                NewLine.PollingNodeNum = 2;
                return;
            }

            string[] data = line.Split(' ');
            if (NewLine.PollingNodeNum == 1 && line.Contains(Constants.RcvMsg))
            {
                NewLine.AddData.Add("      HP:N1[ER] エラーコード応答:");
            }
            else if (NewLine.PollingNodeNum == 2 && line.Contains(Constants.RcvMsg))
            {
                NewLine.AddData.Add("      HP:N2[ER] エラーコード応答:");
            }
            else
            {
                NewLine.AddData.Add(Constants.ErrorCase);
                return;
            }           
            System.Text.StringBuilder buf = new System.Text.StringBuilder();
            for (int i = 0; i < 7; i++)
            {
                buf.Append((char)Convert.ToInt32(data[7 + i], 16));
            }
            NewLine.AddData.Add(buf.ToString());
            NewLine.AddData.Add("\r\n");           

            NewLine.PollingNodeNum = 0;
            return;
        }

        // C1：ﾘﾓｰﾄ･ﾛｰｶﾙ設定読出し
        private void SplitC1_Polling(string line)
        {
            NewLine.AddData = new List<string>();
            if (line.Contains(CurrentNodeNum.Node1) && line.Contains(Constants.SndMsg))
            {
                NewLine.AddData.Add("      HP:N1[C1] ﾘﾓｰﾄ･ﾛｰｶﾙ設定読出し\r\n");
                NewLine.PollingNodeNum = 1;
                return;
            }
            else if (line.Contains(CurrentNodeNum.Node2) && line.Contains(Constants.SndMsg))
            {
                NewLine.AddData.Add("      HP:N2[C1] ﾘﾓｰﾄ･ﾛｰｶﾙ設定読出し\r\n");
                NewLine.PollingNodeNum = 2;
                return;
            }
            string[] data = line.Split(' ');

            if (NewLine.PollingNodeNum == 1 && line.Contains(Constants.RcvMsg))
            {
                NewLine.AddData.Add("      HP:N1[C1] ﾘﾓｰﾄ･ﾛｰｶﾙ設定読出し\r\n");
            }
            else if (NewLine.PollingNodeNum == 2 && line.Contains(Constants.RcvMsg))
            {
                NewLine.AddData.Add("      HP:N2[C1] ﾘﾓｰﾄ･ﾛｰｶﾙ設定読出し\r\n");
            }
            else
            {
                NewLine.AddData.Add(Constants.ErrorCase);
                return;
            }
            for (int i = 0; i < 4; i++)
            {
                System.Text.StringBuilder buf = new System.Text.StringBuilder();
                buf.Append((char)Convert.ToInt32(data[10 + (i * 5)], 16));
                NewLine.AddData.Add(Constants.HeaderCh);
                NewLine.AddData.Add((i + 1).ToString());
                NewLine.AddData.Add(Constants.C1_polling);
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("\r\n");
            }

            NewLine.PollingNodeNum = 0;
            return;

        }
        // S1:設定温度読み出し
        private void SplitS1_Polling(string line)
        {
            NewLine.AddData = new List<string>();
            if (line.Contains(CurrentNodeNum.Node1) && line.Contains(Constants.SndMsg))
            {
                NewLine.AddData.Add("      HP:N1[S1] 設定温度読出し\r\r\n");                
                NewLine.PollingNodeNum = 1;
                return;
            }
            else if (line.Contains(CurrentNodeNum.Node2) && line.Contains(Constants.SndMsg))
            {
                NewLine.AddData.Add("      HP:N2[S1] 設定温度読出し\r\n");
                NewLine.PollingNodeNum = 2;
                return;
            }

            string[] data = line.Split(' ');
  
            if (NewLine.PollingNodeNum == 1 && line.Contains(Constants.RcvMsg))
            {
                NewLine.AddData.Add("      HP:N1[S1] 設定温度読出し応答\r\n");
            }
            else if(NewLine.PollingNodeNum == 2 && line.Contains(Constants.RcvMsg))
            {
                NewLine.AddData.Add("      HP:N2[S1] 設定温度読出し応答\r\n");
            }
            else
            {
                NewLine.AddData.Add(Constants.ErrorCase);
                return;
            }
            for (int i = 0; i < 4; i++)
            {
                System.Text.StringBuilder buf = new System.Text.StringBuilder();
                for (int j = 0; j < 5; j++)
                {
                    buf.Append((char)Convert.ToInt32(data[12 + j + (i * 11)], 16));
                }
                NewLine.AddData.Add(Constants.HeaderCh);
                NewLine.AddData.Add((i + 1).ToString());
                NewLine.AddData.Add(Constants.S1);
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("\r\n"); 
            }
 
            NewLine.PollingNodeNum = 0;
            return;
        }

        // PID/AT切換
        private void SplitG1_Polling(string line)
        {
            NewLine.AddData = new List<string>();
            if (line.Contains(CurrentNodeNum.Node1) && line.Contains(Constants.SndMsg))
            {
                NewLine.AddData.Add("      HP:N1[G1] PID/AT切換\r\n");
                NewLine.PollingNodeNum = 1;
                return;
            }
            else if (line.Contains(CurrentNodeNum.Node2) && line.Contains(Constants.SndMsg))
            {
                NewLine.AddData.Add("      HP:N2[G1] PID/AT切換\r\n");
                NewLine.PollingNodeNum = 2;
                return;
            }
            string[] data = line.Split(' ');

            if (NewLine.PollingNodeNum == 1 && line.Contains(Constants.RcvMsg))
            {
                NewLine.AddData.Add("      HP:N1[G1] PID/AT切換応答\r\n");
            }
            else if (NewLine.PollingNodeNum == 2 && line.Contains(Constants.RcvMsg))
            {
                NewLine.AddData.Add("      HP:N2[G1] PID/AT切換応答\r\n");
            }
            else
            {
                NewLine.AddData.Add(Constants.ErrorCase);
                return;
            }
            for (int i = 0; i < 4; i++)
            {
                System.Text.StringBuilder buf = new System.Text.StringBuilder();
                buf.Append((char)Convert.ToInt32(data[10 + (i * 5)], 16));
                NewLine.AddData.Add(Constants.HeaderCh);
                NewLine.AddData.Add((i + 1).ToString());
                NewLine.AddData.Add(Constants.G1);
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("\r\n");
            }

            NewLine.PollingNodeNum = 0;
            return;

        }

        // M1:測定温度
        private void SplitM1_Polling(string line)
        {
            NewLine.AddData = new List<string>();
#if M1_INVERSION_MODE
            if (line.Contains(CurrentNodeNum.Node1) && line.Contains(Constants.SndMsg))
            {
                NewLine.AddData.Add("      HP:N1[M1] 測定温度読出し\r\n");
                NewLine.PollingNodeNum = 100;
                return;
            }
            else if (line.Contains(CurrentNodeNum.Node2) && line.Contains(Constants.SndMsg))
            {
                NewLine.AddData.Add("      HP:N2[M1] 測定温度読出し\r\n");
                NewLine.PollingNodeNum = 200;
                return;
            }

            string[] data = line.Split(' ');

            if (NewLine.PollingNodeNum == 100 && line.Contains(Constants.RcvMsg))
            {
                NewLine.AddData.Add("      HP:N1[M1] 測定温度読出し応答\r\n");
            }
            else if(NewLine.PollingNodeNum == 200 && line.Contains(Constants.RcvMsg))
            {
                NewLine.AddData.Add("      HP:N2[M1] 測定温度読出し応答\r\n");

            }
            else
            {                
                NewLine.AddData.Add(Constants.ErrorCase);
                return;
            }
            for (int i = 0; i < 4; i++)
            {
                System.Text.StringBuilder buf = new System.Text.StringBuilder();
                for (int j = 0; j < 5; j++)
                {
                    buf.Append((char)Convert.ToInt32(data[15 + j + (i * 11)], 16));
                }
                NewLine.AddData.Add(Constants.HeaderCh);
                NewLine.AddData.Add((i + 1).ToString());
                NewLine.AddData.Add(Constants.M1);
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("\r\n");
            }

            NewLine.PollingNodeNum = 0;
            return;
#else
            if (line.Contains(CurrentNodeNum.Node1) && line.Contains(Constants.SndMsg))
            {
                NewLine.AddData.Add("      HP:N1[M1] 測定温度読出し\r\n");
                NewLine.PollingNodeNum = 1;
                return;
            }
            else if (line.Contains(CurrentNodeNum.Node2) && line.Contains(Constants.SndMsg))
            {
                NewLine.AddData.Add("      HP:N2[M1] 測定温度読出し\r\n");
                NewLine.PollingNodeNum = 2;
                return;
            }
            string[] data = line.Split(' ');

            if (NewLine.PollingNodeNum == 1 && line.Contains(Constants.RcvMsg))
            {
                NewLine.AddData.Add("      HP:N1[M1] 測定温度読出し応答\r\n");
            }
            else if(NewLine.PollingNodeNum == 2 && line.Contains(Constants.RcvMsg))
            {
                NewLine.AddData.Add("      HP:N2[M1] 測定温度読出し応答\r\n");

            }
            else
            {                
                NewLine.AddData.Add(Constants.ErrorCase);
                return;
            }
            for (int i = 0; i < 4; i++)
            {
                System.Text.StringBuilder buf = new System.Text.StringBuilder();
                for (int j = 0; j < 5; j++)
                {
                    buf.Append((char)Convert.ToInt32(data[12 + j + (i * 11)], 16));
                }
                NewLine.AddData.Add(Constants.HeaderCh);
                NewLine.AddData.Add((i + 1).ToString());
                NewLine.AddData.Add(Constants.M1);
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("\r\n");
            }

            NewLine.PollingNodeNum = 0;
            return;
#endif
        }

        // AJ:総合ｲﾍﾞﾝﾄ状態
        private void SplitAJ_Polling(string line)
        {
            NewLine.AddData = new List<string>();
            if (line.Contains(CurrentNodeNum.Node1) && line.Contains(Constants.SndMsg))
            {
                NewLine.AddData.Add("      HP:N1[AJ] 総合ｲﾍﾞﾝﾄ状態読出し\r\n");
                NewLine.PollingNodeNum = 1;
                return;
            }
            else if (line.Contains(CurrentNodeNum.Node2) && line.Contains(Constants.SndMsg))
            {
                NewLine.AddData.Add("      HP:N2[AJ] 総合ｲﾍﾞﾝﾄ状態読出し\r\n");
                NewLine.PollingNodeNum = 2;
                return;
            }

            string[] data = line.Split(' ');

            if (NewLine.PollingNodeNum == 1 && line.Contains(Constants.RcvMsg))
            {
                NewLine.AddData.Add("      HP:N1[AJ] 総合ｲﾍﾞﾝﾄ状態読出し応答\r\n");
            }
            else if (NewLine.PollingNodeNum == 2 && line.Contains(Constants.RcvMsg))
            {
                NewLine.AddData.Add("      HP:N2[AJ] 総合ｲﾍﾞﾝﾄ状態読出し応答\r\n");
            }
            else
            {
                NewLine.AddData.Add(Constants.ErrorCase);
                return;
            }
            for (int i = 0; i < 4; i++)
            {
                System.Text.StringBuilder buf = new System.Text.StringBuilder();
                for (int j = 0; j < 5; j++)
                {
                    buf.Append((char)Convert.ToInt32(data[12 + j + (i * 11)], 16));
                }
                NewLine.AddData.Add(Constants.HeaderCh);
                NewLine.AddData.Add((i + 1).ToString());
                NewLine.AddData.Add(Constants.AJ);
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("\r\n");
            }
            NewLine.PollingNodeNum = 0;
            return; 
        }
        #endregion

        #region selecting
        // SR:RUN/STOP設定
        private void SplitSR_Selecting(string line)
        {
            NewLine.AddData = new List<string>();
            if (line.Contains(CurrentNodeNum.Node1) && line.Contains(Constants.SndMsg))
            {
                NewLine.AddData.Add("      HP:N1[SR] RUN/STOP設定\r\n");
                return;
            }
            else if (line.Contains(CurrentNodeNum.Node2) && line.Contains(Constants.SndMsg))
            {
                NewLine.AddData.Add("      HP:N2[SR] RUN/STOP設定\r\n");
                return;
            }
        }

        // C1:HCFLG
        private void SplitC1_Selecting(string line)
        {
            NewLine.AddData = new List<string>();
            string[] selectingData = line.Split(' ');
            System.Text.StringBuilder buf = new System.Text.StringBuilder();

            if (line.Contains(CurrentNodeNum.Node1) && line.Contains(Constants.SndMsg))
            {
                NewLine.AddData.Add("      HP:N1[C1] CH");
            }
            else if (line.Contains(CurrentNodeNum.Node2) && line.Contains(Constants.SndMsg))
            {
                NewLine.AddData.Add("      HP:N1[C1] CH");
            }

            buf.Append((char)Convert.ToInt32(selectingData[12], 16));
            NewLine.AddData.Add(buf.ToString());
            NewLine.AddData.Add(Constants.C1_selecting);
            buf = new System.Text.StringBuilder();
            for (int i = 0; i < 4; i++)
            {
                buf.Append((char)Convert.ToInt32(selectingData[13 + i], 16));
            }
            NewLine.AddData.Add(buf.ToString());
            NewLine.AddData.Add("\r\n");
            return;
        }

        // S1:温度設定
        private void SplitS1_Selecting(string line)
        {
            NewLine.AddData = new List<string>();
            string[] selectingData = line.Split(' ');
            System.Text.StringBuilder buf = new System.Text.StringBuilder();

            if (line.Contains(CurrentNodeNum.Node1) && line.Contains(Constants.SndMsg))
            {
                NewLine.AddData.Add("      HP:N1[S1] CH");
            }
            else if(line.Contains(CurrentNodeNum.Node2) && line.Contains(Constants.SndMsg))
            {
                NewLine.AddData.Add("      HP:N1[S1] CH");
            }

            buf.Append((char)Convert.ToInt32(selectingData[12], 16));
            NewLine.AddData.Add(buf.ToString());
            NewLine.AddData.Add(Constants.S1);
            buf = new System.Text.StringBuilder();
            for (int i = 0; i < 5; i++)
            {
                buf.Append((char)Convert.ToInt32(selectingData[16 + i], 16));
            }
            NewLine.AddData.Add(buf.ToString());
            NewLine.AddData.Add("\r\n");
            return;
        }

        // A1:上限温度
        private void SplitA1_Selecting(string line)
        {
            NewLine.AddData = new List<string>();
            string[] selectingData = line.Split(' ');
            System.Text.StringBuilder buf = new System.Text.StringBuilder();

            if (line.Contains(CurrentNodeNum.Node1) && line.Contains(Constants.SndMsg))
            {
                NewLine.AddData.Add("      HP:N1[A1] CH");
            }
            else if (line.Contains(CurrentNodeNum.Node2) && line.Contains(Constants.SndMsg))
            {
                NewLine.AddData.Add("      HP:N1[A1] CH");
            }

            buf.Append((char)Convert.ToInt32(selectingData[12], 16));
            NewLine.AddData.Add(buf.ToString());
            NewLine.AddData.Add(Constants.A1);
            buf = new System.Text.StringBuilder();
            for (int i = 0; i < 4; i++)
            {
                buf.Append((char)Convert.ToInt32(selectingData[17 + i], 16));
            }
            NewLine.AddData.Add(buf.ToString());
            NewLine.AddData.Add("\r\n");
            return;
        }

        // A2:下限温度
        private void SplitA2_Selecting(string line)
        {
            NewLine.AddData = new List<string>();
            string[] selectingData = line.Split(' ');
            System.Text.StringBuilder buf = new System.Text.StringBuilder();

            if (line.Contains(CurrentNodeNum.Node1) && line.Contains(Constants.SndMsg))
            {
                NewLine.AddData.Add("      HP:N1[A2] CH");
            }
            else if (line.Contains(CurrentNodeNum.Node2) && line.Contains(Constants.SndMsg))
            {
                NewLine.AddData.Add("      HP:N2[A2] CH");
            }

            buf.Append((char)Convert.ToInt32(selectingData[12], 16));
            NewLine.AddData.Add(buf.ToString());
            NewLine.AddData.Add(Constants.A2);
            buf = new System.Text.StringBuilder();
            for (int i = 0; i < 4; i++)
            {
                buf.Append((char)Convert.ToInt32(selectingData[17 + i], 16));
            }
            NewLine.AddData.Add(buf.ToString());
            NewLine.AddData.Add("\r\n");
            return;
        }

        // W1:ｱﾝﾁﾘｾｯﾄﾜｲﾝﾄﾞｱｯﾌﾟ(ARW)
        private void SplitW1_Selecting(string line)
        {
            NewLine.AddData = new List<string>();
            string[] selectingData = line.Split(' ');
            System.Text.StringBuilder buf = new System.Text.StringBuilder();

            if (line.Contains(CurrentNodeNum.Node1) && line.Contains(Constants.SndMsg))
            {
                NewLine.AddData.Add("      HP:N1[W1] CH");
            }
            else if (line.Contains(CurrentNodeNum.Node2) && line.Contains(Constants.SndMsg))
            {
                NewLine.AddData.Add("      HP:N1[W1] CH");
            }
            buf.Append((char)Convert.ToInt32(selectingData[12], 16));
            NewLine.AddData.Add(buf.ToString());
            NewLine.AddData.Add(Constants.W1);
            buf = new System.Text.StringBuilder();
            for (int i = 0; i < 6; i++)
            {
                buf.Append((char)Convert.ToInt32(selectingData[13 + i], 16));
            }
            NewLine.AddData.Add(buf.ToString());
            NewLine.AddData.Add("\r\n");
            return;
        }

        // J1:ｵｰﾄﾁｭｰﾆﾝｸﾞ(AT)実行
        private void SplitJ1_Selecting(string line)
        {
            NewLine.AddData = new List<string>();
            string[] selectingData = line.Split(' ');
            System.Text.StringBuilder buf = new System.Text.StringBuilder();

            if (line.Contains(CurrentNodeNum.Node1) && line.Contains(Constants.SndMsg))
            {
                NewLine.AddData.Add("      HP:N1[J1] CH");
            }
            else if (line.Contains(CurrentNodeNum.Node2) && line.Contains(Constants.SndMsg))
            {
                NewLine.AddData.Add("      HP:N1[J1] CH");
            }
            buf.Append((char)Convert.ToInt32(selectingData[12], 16));
            NewLine.AddData.Add(buf.ToString());
            NewLine.AddData.Add(Constants.J1);
            buf = new System.Text.StringBuilder();
            for (int i = 0; i < 2; i++)
            {
                buf.Append((char)Convert.ToInt32(selectingData[13 + i], 16));
            }
            NewLine.AddData.Add(buf.ToString());
            NewLine.AddData.Add("\r\n");
            return;

        }

        // P1:比例帯
        private void SplitP1_Selecting(string line)
        {
            NewLine.AddData = new List<string>();
            string[] selectingData = line.Split(' ');
            System.Text.StringBuilder buf = new System.Text.StringBuilder();

            if (line.Contains(CurrentNodeNum.Node1) && line.Contains(Constants.SndMsg))
            {
                NewLine.AddData.Add("      HP:N1[P1] CH");
            }
            else if (line.Contains(CurrentNodeNum.Node2) && line.Contains(Constants.SndMsg))
            {
                NewLine.AddData.Add("      HP:N1[P1] CH");
            }

            buf.Append((char)Convert.ToInt32(selectingData[12], 16));
            NewLine.AddData.Add(buf.ToString());
            NewLine.AddData.Add(Constants.P1);
            buf = new System.Text.StringBuilder();
            for (int i = 0; i < 5; i++)
            {
                buf.Append((char)Convert.ToInt32(selectingData[16 + i], 16));
            }
            NewLine.AddData.Add(buf.ToString());
            NewLine.AddData.Add("\r\n");
            return;
        }

        // I1:積分時間
        private void SplitI1_Selecting(string line)
        {
            NewLine.AddData = new List<string>();
            string[] selectingData = line.Split(' ');
            System.Text.StringBuilder buf = new System.Text.StringBuilder();

            if (line.Contains(CurrentNodeNum.Node1) && line.Contains(Constants.SndMsg))
            {
                NewLine.AddData.Add("      HP:N1[I1] CH");
            }
            else if (line.Contains(CurrentNodeNum.Node2) && line.Contains(Constants.SndMsg))
            {
                NewLine.AddData.Add("      HP:N2[I1] CH");
            }

            buf.Append((char)Convert.ToInt32(selectingData[12], 16));
            NewLine.AddData.Add(buf.ToString());
            NewLine.AddData.Add(Constants.I1);
            buf = new System.Text.StringBuilder();
            for (int i = 0; i < 2; i++)
            {
                buf.Append((char)Convert.ToInt32(selectingData[18 + i], 16));
            }
            NewLine.AddData.Add(buf.ToString());
            NewLine.AddData.Add("\r\n");
            return;
        }

        // D1:微分時間
        private void SplitD1_Selecting(string line)
        {
            NewLine.AddData = new List<string>();
            string[] selectingData = line.Split(' ');
            System.Text.StringBuilder buf = new System.Text.StringBuilder();

            if (line.Contains(CurrentNodeNum.Node1) && line.Contains(Constants.SndMsg))
            {
                NewLine.AddData.Add("      HP:N1[D1] CH");
            }
            else if (line.Contains(CurrentNodeNum.Node2) && line.Contains(Constants.SndMsg))
            {
                NewLine.AddData.Add("      HP:N2[D1] CH");
            }

            buf.Append((char)Convert.ToInt32(selectingData[12], 16));
            NewLine.AddData.Add(buf.ToString());
            NewLine.AddData.Add(Constants.D1);
            buf = new System.Text.StringBuilder();
            for (int i = 0; i < 2; i++)
            {
                buf.Append((char)Convert.ToInt32(selectingData[18 + i], 16));
            }
            NewLine.AddData.Add(buf.ToString());
            NewLine.AddData.Add("\r\n");
            return;
        }
        #endregion
        #endregion

        /// <summary>
        /// ﾉｰﾄﾞ番号を取得します
        /// </summary>
        private bool getNodeNumber()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.FileName = inputFilePath.Text;

#if ALLOW_ALL_FILE
#else 
            if (this.inputFilePath.Text == null)
            {
                MessageBox.Show("「SIO.log」を選択してください",
                                "エラー",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                return false;
            }
            if(!this.inputFilePath.Text.Contains(Constants.LogFileName))
            {
                if (!this.inputFilePath.Text.Contains(Constants.OldFileName))
                {
                    MessageBox.Show("「SIO.log」を選択してください",
                                    "エラー",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                    return false;
                }
            }
#endif
            System.IO.Stream stream = ofd.OpenFile();

            System.IO.StreamReader sr =
                new System.IO.StreamReader(
                    stream,
                    System.Text.Encoding.GetEncoding(Constants.FileEncording)); 

            string line = "";
            while ((line = sr.ReadLine()) != null)
            {
                if ((CurrentNodeNum.Node1 != null && CurrentNodeNum.Node2 != null) && 
                    (CurrentNodeNum.CpNode1 != null && CurrentNodeNum.CpNode2 != null))
                {
                    break;
                }

                string[] buf = line.Split(' ');
                // CP
                if (line.Contains(Constants.SndMsg) && buf[5] == Constants.CpMsg)
                {
                    if (CurrentNodeNum.CpNode1 == null)
                    {
                        CurrentNodeNum.Node1 = buf[4];
                        CurrentNodeNum.CpNode1 = buf[6];
                    }
                    else
                    {
                        int result = CurrentNodeNum.Node1.CompareTo(buf[4]);
                        switch (result)
                        {
                            case -1:
                                CurrentNodeNum.Node2 = buf[4];
                                CurrentNodeNum.CpNode2 = buf[6];
                                break;
                            case 0:
                                break;
                            case 1:
                                string tmpNodeNum = CurrentNodeNum.Node1;
                                string tmpCpNodeNum = CurrentNodeNum.CpNode1;
                                CurrentNodeNum.Node1 = buf[4];
                                CurrentNodeNum.CpNode1 = buf[6];
                                CurrentNodeNum.Node2 = tmpNodeNum;
                                CurrentNodeNum.CpNode2 = tmpCpNodeNum;
                                break;
                        }
                    }
                }
                // HP
                else if (line.Contains(Constants.SndMsg) && buf[5] == Constants.HpMsg)
                {
                    if (CurrentNodeNum.Node1 == null)
                    {
                        CurrentNodeNum.Node1 = buf[4];
                    }
                    else
                    {
                        int result = CurrentNodeNum.Node1.CompareTo(buf[4]);
                        switch (result)
                        {
                            case -1:
                                CurrentNodeNum.Node2 = buf[4];
                                break;
                            case 0:
                                break;
                            case 1:
                                string tmpNodeNum = CurrentNodeNum.Node1;
                                CurrentNodeNum.Node1 = buf[4];
                                CurrentNodeNum.Node2 = tmpNodeNum;
                                break;
                        }
                    }

                }

            }
            if (CurrentNodeNum.Node2 == null)
            {
                CurrentNodeNum.Node2 = Constants.NodeError;
            }
            if(CurrentNodeNum.CpNode2 == null)
            {
                CurrentNodeNum.CpNode2 = Constants.NodeError;
            }
            sr.Close();
            stream.Close();

            return true;
        }

        /// <summary>
        /// 定数ｸﾗｽ
        /// </summary>
        abstract class Constants
        {
            public const string NodeError = "15";            
            public const string PollingMark = "4B";
            public const string SndMsg = "SND";
            public const string RcvMsg = "RCV";
            public const string ack = "06";
            public const string HeaderCh = "      CH";
            public const string CpMsg = "02";
            public const string HpMsg = "04";
            public const int MinDataLength = 5;
            public const string ErrorCase = "ERROR";

            public const string M1 = " 測定温度  :";
            public const string C1_polling = " ﾘﾓｰﾄ/ﾛｰｶﾙ設定   :";
            public const string G1 = " PID/AT切換  :";
            public const string AJ = " 総合ｲﾍﾞﾝﾄ状態 :";
            public const string ER = " ｴﾗｰｺｰﾄﾞ:";
            
            public const string C1_selecting = "HCFLG     :";
            public const string S1 = " 目標温度  :";
            public const string A1 = " 上限温度  :";
            public const string A2 = " 下限温度  :";
            public const string W1 = " ｱﾝﾁﾘｾｯﾄｲﾝﾄﾞｱｯﾌﾟ";
            public const string J1 = " ｵｰﾄﾁｭｰﾆﾝｸﾞ実行";
            public const string P1 = " 比例帯    :";
            public const string I1 = " 積分時間  :";
            public const string D1 = " 微分時間  :";

            public const string WMRM = " 制御ﾓｰﾄ:";
            public const string RR = " ｽﾃｰﾀｽ;";
            public const string RX1 = " 制御ｾﾝｻ:";
            public const string RX2 = " 外部ｾﾝｻ:";
            public const string Pb = " 比例帯温度幅:";
            public const string Integral = " 積分時間:";
            public const string Differential = " 微分時間:";
            public const string ADJ = " ｵﾌｾｯﾄ値:";
            public const string WSRS = " 目標温度:";
            public const string UpperTempWidth = " 上限温度幅:";
            public const string LowerTempWidth = " 下限温度幅;";
            public const string WPRP = " 演算開始定数:";

            public const string FileEncording = "shift_jis";

            public const int ComPollingSndPosi = 10;
            public const int ComPollingRcvPosi = 5;

            public const int ComSelectingPosi = 9;

            public const int ComCpSndPosi = 7;
            public const int ComCpRcvPosi = 6;

            public const string LogFileName = "SIO.log";
            public const string OldFileName = "SIO.old";           
              
            public const string OutFileName = @"\SIO.txt"; 

        }

    }
}

// EOF