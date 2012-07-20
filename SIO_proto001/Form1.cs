//#define ONLY_SIO_LOG

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


        Dictionary<string, Action<string>> cpDic = null;
        Dictionary<string, Action<string>> pollingDic = null;
        Dictionary<string, Action<string>> selectingDic = null;
        Dictionary<string, Action<string>> SpinDic = null;

        ConvertedData NewLine = new ConvertedData();
        MsgInfo OriginalMsg = new MsgInfo();


        CheckBox[] NodeFileter;
        CheckBox[] SlaveFilter;

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
            if (SpinDic == null)
            {
                SpinDic = new Dictionary<string, Action<string>>();

                // ETU
                SpinDic["WM"] = SplitWM_ETU;    // 制御ﾓｰﾄﾞ設定
                SpinDic["RM"] = SplitRM_ETU;    // 制御ﾓｰﾄﾞ取得
                SpinDic["RR"] = SplitRR_ETU;    // ｽﾃｰﾀｽの読み出し
                SpinDic["RX"] = SplitRX_ETU;    // 制御ｾﾝｻ測定温度読み出し
                SpinDic["WB"] = SplitWB_ETU;    // PID定数及び表示温度校正値(ADJ)設定
                SpinDic["RB"] = SplitRB_ETU;    // PID定数及びｵﾌｾｯﾄ読み出し
                SpinDic["WS"] = SplitWS_ETU;    // 目標温度設定
                SpinDic["RS"] = SplitRS_ETU;    // 目標温度読み出し
                SpinDic["W%"] = SplitWPe_ETU;   // 上下温度幅設定
                SpinDic["R%"] = SplitRPe_ETU;   // 上下温度幅読み出し
                SpinDic["WU"] = SplitWU_ETU;    // 内部ｾﾝｻ及び外部ｾﾝｻ微調整値設定
                SpinDic["RU"] = SplitRU_ETU;    // 内部ｾﾝｻ及び外部ｾﾝｻ微調整値読出し
                SpinDic["WA"] = SplitWA_ETU;    // ARW幅設定
                SpinDic["RA"] = SplitRA_ETU;    // ARW幅読出し
                SpinDic["RV"] = SplitRV_ETU;    // ｿﾌﾄﾊﾞｰｼﾞｮﾝ読出し

                // ACU
                SpinDic["M1"] = SplitM1_ACU;    // 制御出口空気温度測定値
                SpinDic["M5"] = SplitM5_ACU;    // 制御出口空気湿度測定値
                SpinDic["S1"] = SplitS1_ACU;    // 制御出口空気温度設定値
                SpinDic["S5"] = SplitS5_ACU;    // 制御出口空気湿度設定値
                SpinDic["JO"] = SplitJO_ACU;    // 運転状態
                SpinDic["ER"] = SplitER_ACU;    // 警報信号
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

        private bool CallSpinDic(string command, string line)
        {
            if (SpinDic.ContainsKey(command))
            {
                SpinDic[command].Invoke(line);
                return true;
            }
            else
            {
                return false;
            }
        }


        private int ContainsSlaveFilterCheckBox()
        {
            this.SlaveFilter = new CheckBox[5];
            this.SlaveFilter[0] = this.filterCp;
            this.SlaveFilter[1] = this.filterHp;
            this.SlaveFilter[3] = this.filterAcu;
            this.SlaveFilter[2] = this.filterEtu;
            this.SlaveFilter[4] = this.filterPulse;

            int result = 0x00;
            for (int i = 0; i < 5; i++)
            {
                if (SlaveFilter[i].Checked)
                {
                    result += 1 << i;
                }
            }

            if (result == 0x00)
            {
                result = 0x1F;
            }

            return result;

        }


        private int ContainsNodeFilterCheckBox()
        {
            this.NodeFileter = new CheckBox[8];
            this.NodeFileter[0] = this.filterNode0;
            this.NodeFileter[1] = this.filterNode1;
            this.NodeFileter[2] = this.filterNode2;
            this.NodeFileter[3] = this.filterNode3;
            this.NodeFileter[4] = this.filterNode4;
            this.NodeFileter[5] = this.filterNode5;
            this.NodeFileter[6] = this.filterNode6;
            this.NodeFileter[7] = this.filterNode7;

            int result = 0x00;
            for (int i = 0; i < 8; i++)
            {
                if (NodeFileter[i].Checked)
                {
                    result += 1 << i;
                }
            }

            if (result == 0x00)
            {
                result = 0xFF;
            }

            return result;
        }


        #region ｸﾗｽ・定義
        public class ConvertedData
        {
            public List<string> AddData { get; set; }
            public string RcvHpNodeNum { get; set; }
            public string RcvAcuNodeNum { get; set; }
        }

        public class MsgInfo
        {
            public string command   { get; set; }
            public int MsgKind      { get; set; }
        }
       

        private string[] trashBox = new string[]{
            "占有権",
            "モード",
            "初期設定",
            "NOT",
            "READY",
            "[U0C P1]RCV",
            "[U0A P2]RCV 02",
            "[U0A P2]RCV 04"
        };

        private string[] joData = new string[]{
            "(停止)",
            "(準備運転中)",
            "(運転)",
            "(送風機(制御空気側)単独運転)"
        };

        private string[] wm = new string[]{
            "(制御動作停止ﾓｰﾄﾞ)",
            "(標準ﾓｰﾄﾞ)",
            "(学習制御ﾓｰﾄﾞ)",
            "(外部同調制御ﾓｰﾄﾞ)",
            "(無指令)"
        };

        private string[] rm = new string[]{
            "(制御動作停止ﾓｰﾄﾞ)",
            "(標準ﾓｰﾄﾞ)",
            "(学習制御ﾓｰﾄﾞ)",
            "(外部同調制御ﾓｰﾄﾞ)",
            "(ｵｰﾄﾁｭｰﾆﾝｸﾞ中)"
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
            ofd.FileName = myConstants.LogFileName;
            // はじめに表示されるDir指定(ｶﾚﾝﾄDir)            
            //            ofd.InitialDirectory = @"C\";            
            // [ﾌｧｲﾙの種類]ではじめに
            //「すべてのﾌｧｲﾙ」が選択されているようにする
            ofd.FilterIndex = 2;
            // ﾀｲﾄﾙ設定
            ofd.Title = "読込むﾌｧｲﾙを選択してください";
            // ﾀﾞｲｱﾛｸﾞﾎﾞｯｸｽを閉じる前に現在のDirを復元するようにする
            ofd.RestoreDirectory = true;
            // 存在しないﾌｧｲﾙの名前が指定されたとき警告を表示する
            ofd.CheckFileExists = true;
            // 存在しないﾊﾟｽが指定されたとき警告を表示する
            ofd.CheckPathExists = true;

            // ﾀﾞｲｱﾛｸﾞ表示
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                inputFilePath.Text = ofd.FileName;
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
            fbd.Description = "ﾌｫﾙﾀﾞを指定してください。";
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
                //選択されたフォルダを表示する
                this.outputFilePath.Text = fbd.SelectedPath;
            }
        }


        /// <summary>
        /// "読込み"ﾎﾞﾀﾝｸﾘｯｸ時の動作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void convertBtn_Click(object sender, EventArgs e)
        {
            if (this.inputFilePath.Text == "")
            {
                MessageBox.Show("読込むﾌｧｲﾙを選択してください",
                                "エラー",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                return;
            }
#if ONLY_SIO_LOG

            if (!this.inputFilePath.Text.Contains(myConstants.LogFileName))
            {
                MessageBox.Show("「SIO.log」を選択してください",
                    "エラー",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return false;
            }
#endif

            // OpenFileDialogｸﾗｽのｲﾝｽﾀﾝｽ作成
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.FileName = inputFilePath.Text;

            System.IO.Stream stream = ofd.OpenFile();
            //内容を読み込む
            System.IO.StreamReader sr =
                new System.IO.StreamReader(
                    stream,
                    System.Text.Encoding.GetEncoding(myConstants.FileEncording));


            if (this.outputFilePath.Text == "")
            {
                this.outputFilePath.Text = 
                    System.IO.Path.GetDirectoryName(this.inputFilePath.Text);
            }

            // ﾌｧｲﾙ作成
            System.IO.StreamWriter sw = new System.IO.StreamWriter(
                this.outputFilePath.Text + myConstants.OutFileName,
                true,
                System.Text.Encoding.GetEncoding(myConstants.FileEncording));

            // 
            int nodeResult = ContainsNodeFilterCheckBox();
            int slaveResult = ContainsSlaveFilterCheckBox();

            int cpDataCounts = 0;
            string line = "";
            while ((line = sr.ReadLine()) != null)
            {
                string[] data = line.Split(' '); 
                
                // すっきりもーど(仮)
                bool result = false;
                if (trashMode.Checked)
                {
                    foreach (string str in trashBox)
                    {
                        if (line.Contains(str))
                        {                            
                            result = true;
                            break;
                        }
                    }
                    // ｴｺｰﾊﾞｯｸ
                    // BAKE
                    if (line.Contains(myConstants.RcvMsg) && data.Length > 7)
                    {
                        // CPｴｺｰﾊﾞｯｸ
                        if (data.Count() == cpDataCounts + 2)
                        {
                            continue;
                        }
                        // pollingｴｺｰﾊﾞｯｸ
                        if (data[7] == myConstants.EOT)
                        {
                            continue;
                        }
                    }
                    // SPIN
                    else if (line.Contains(myConstants.SpinRcvmsg))
                    {
                        if (data.Length == 0x10)
                        {
                            continue;
                        }

                        if (data[data.Length - 1] == myConstants.ENQ)
                        {
                            continue;
                        }
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

                    // ｴｺｰﾊﾞｯｸ
                    // BAKE
                    if (line.Contains(myConstants.RcvMsg) && data.Length > 7)
                    {
                        // CPｴｺｰﾊﾞｯｸ
                        if (data.Count() == cpDataCounts + 2)
                        {
                            sw.WriteLine(line + sw.NewLine);
                            continue;
                        }
                        // pollingｴｺｰﾊﾞｯｸ
                        if (data[7] == myConstants.EOT)
                        {
                            sw.WriteLine(line + sw.NewLine);
                            continue;
                        }
                    }
                    // SPIN
                    else if (line.Contains(myConstants.SpinRcvmsg))
                    {
                        if (data.Length == 0x0F)
                        {
                            sw.WriteLine(line + sw.NewLine);
                            continue;
                        }
                        if (data[data.Length - 1] == myConstants.ENQ)
                        {
                            sw.WriteLine(line + sw.NewLine);
                            continue;
                        }
                        
                    }
                }
                if (result) continue;


                System.Text.StringBuilder ComAscii = new System.Text.StringBuilder();

//                OriginalMsg = CheckMsg(line);
                CheckMsg(line);
                try
                {
                    switch (OriginalMsg.MsgKind)
                    {
                        case 1: // CP SND
                            if (!this.filterCp.Checked && slaveResult != 0x1F)
                            {
                                continue;
                            }
                            
                            if (!CallCpDic(OriginalMsg.command, line))
                            {
                                throw new Exception(myConstants.ErrorMsg);
                            }
                            cpDataCounts = data.Count();
                            break;

                        case 2: // Polling SND
                            if (!this.filterHp.Checked && slaveResult != 0x1F)
                            {
                                continue;
                            }
                            if (!CallPollingDic(OriginalMsg.command, line))
                            {
                                throw new Exception(myConstants.ErrorMsg);
                            }
                            break;

                        case 3: // Selecting SND
                            if (!this.filterHp.Checked && slaveResult != 0x1F)
                            {
                                continue;
                            }
                            if (!CallSelectingDic(OriginalMsg.command, line))
                            {
                                throw new Exception(myConstants.ErrorMsg);
                            }
                            break;

                        case 4: // ETU SND
                        case 40:
                            if (!this.filterEtu.Checked && slaveResult != 0x1F)
                            {
                                continue;
                            }
                            if (!CallSpinDic(OriginalMsg.command, line))
                            {
                                throw new Exception(myConstants.ErrorMsg);
                            }                            
                            break;

                        case 5: // ACU(5/50) SND/RCV
                        case 50:
                            if (!this.filterAcu.Checked && slaveResult != 0x1F)
                            {
                                continue;
                            }
                            if (!CallSpinDic(OriginalMsg.command, line))
                            {
                                throw new Exception(myConstants.ErrorMsg);
                            }
                            break;

                        case 10:    // CPwriting RCV
                            if (!this.filterCp.Checked && slaveResult != 0x1F)
                            {
                                continue;
                            }
                            sw.WriteLine(line + sw.NewLine);
                            continue;

                        case 11:    // CPreading RCV
                            if (!this.filterCp.Checked && slaveResult != 0x1F)
                            {
                                continue;
                            }

                            if (!CallCpDic(OriginalMsg.command, line))
                            {
                                throw new Exception(myConstants.ErrorMsg);
                            }
                            break;

                        case 20:    // Polling RCV
                            if (!this.filterHp.Checked && slaveResult != 0x1F)
                            {
                                continue;
                            }
                            if (!CallPollingDic(OriginalMsg.command, line))
                            {
                                sw.WriteLine(line);
                                sw.WriteLine(myConstants.ErrorMsg + sw.NewLine);
                                continue;
                            }
                            break;

                        case 30:    // HP Selecting RCV
                            if (!this.filterHp.Checked && slaveResult != 0x1F)
                            {
                                continue;
                            }
                            sw.WriteLine(line + sw.NewLine);
                            continue;

                        case 31:    // ACU Selecting RCV
                            if (!this.filterAcu.Checked && slaveResult != 0x1F)
                            {
                                continue;
                            }
                            sw.WriteLine(line + sw.NewLine);
                            continue;
         
                        case 41:    // ETUwriting
                            if (!this.filterEtu.Checked && slaveResult != 0x1F)
                            {
                                continue;
                            }
                            sw.WriteLine(line);
                            sw.WriteLine("ETU書き込み完了" + sw.NewLine);

                            continue;

                        default :
                            throw new Exception(myConstants.ErrorMsg);
                    }

                }            
                catch (Exception ex)
                {
                    sw.WriteLine(line);
                    sw.WriteLine("{0}",ex.Message.ToString() + sw.NewLine);
                    continue;
                }

                if (nodeResult == 0xFF)
                {
                    sw.WriteLine(line);
                    NewLine.AddData.ForEach((string str) => sw.Write(str));
                    sw.Write(sw.NewLine);
                    continue;
                }

                for (int i = 0; i < 8; i++)
                {
                    if (NodeFileter[i].Checked)
                    {
                        if (NewLine.AddData[1] == i.ToString())
                        {
                            sw.WriteLine(line);
                            NewLine.AddData.ForEach((string str) => sw.Write(str));
                            sw.Write(sw.NewLine);
                            break;
                        }                        
                    }
                }
            }
            //閉じる
            sr.Close();
            sw.Close();
            stream.Close();

            MessageBox.Show("変換が完了しました。",
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

        /// <summary>
        /// '↓'ﾎﾞﾀﾝｸﾘｯｸ時の動作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CopyDirPath_Click(object sender, EventArgs e)
        {
            if (this.inputFilePath.Text != "")
            {
                this.outputFilePath.Text =
                    System.IO.Path.GetDirectoryName(this.inputFilePath.Text);
            }
        }

        /// <summary>
        /// 出力先ﾌｫﾙﾀﾞ名ﾃｷｽﾄﾎﾞｯｸｽにﾌｧｲﾙ又はﾌｫﾙﾀﾞをﾄﾞﾗｯｸﾞしたときの動作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void outputFilePath_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        /// <summary>
        /// 出力先ﾌｫﾙﾀﾞ名ﾃｷｽﾄﾎﾞｯｸｽにﾌｧｲﾙ又はﾌｫﾙﾀﾞをﾄﾞﾛｯﾌﾟﾞしたときの動作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void outputFilePath_DragDrop(object sender, DragEventArgs e)
        {
            string[] fileNameArray = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            string fileName = fileNameArray[0];

            this.outputFilePath.Text = System.IO.Path.GetDirectoryName(fileName);

        }
 
        #endregion
        #endregion

        #region ﾒｯｾｰｼﾞ別ﾒｿｯﾄﾞ
        #region CP

        // WM:制御ﾓｰﾄﾞ設定
        private void SplitWM_CP(string line)
        {
            NewLine.AddData = new List<string>();
            string[] data = line.Split(' ');
            System.Text.StringBuilder buf = new System.Text.StringBuilder();
            // SND
            if (line.Contains(myConstants.SndMsg))
            {
                NewLine.AddData.Add(myConstants.HeaderCP);
                buf.Append((char)Convert.ToInt32(data[6], 16));
                NewLine.AddData.Add(buf.ToString());                
                NewLine.AddData.Add("[WM] 制御ﾓｰﾄﾞ設定\r\n");
            }

            for (int i = 0; i < 4; i++)
            {
                buf = new System.Text.StringBuilder();
                buf.Append((char)Convert.ToInt32(data[9 + i], 16));
                NewLine.AddData.Add(myConstants.HeaderCh);
                NewLine.AddData.Add((i + 1).ToString());
                NewLine.AddData.Add(" 制御ﾓｰﾄ: ");
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("\r\n");
            }
            return;

        }

        // RM:制御ﾓｰﾄﾞの取得
        private void SplitRM_CP(string line)
        {
            NewLine.AddData = new List<string>();
            string[] data = line.Split(' ');
            System.Text.StringBuilder buf = new System.Text.StringBuilder();
            // SND
            if (line.Contains(myConstants.SndMsg))
            {
                NewLine.AddData.Add(myConstants.HeaderCP);
                buf.Append((char)Convert.ToInt32(data[6], 16));
                NewLine.AddData.Add(buf.ToString());   
                NewLine.AddData.Add("[RM] 制御ﾓｰﾄﾞ取得\r\n");
                return;
            }

            if (line.Contains(myConstants.RcvMsg))
            {
                NewLine.AddData.Add(myConstants.HeaderCP);
                buf.Append((char)Convert.ToInt32(data[8], 16));
                NewLine.AddData.Add(buf.ToString());   
                NewLine.AddData.Add("[RM] 制御ﾓｰﾄﾞ取得応答\r\n");
            }
            
            for (int i = 0; i < 4; i++)
            {
                buf = new System.Text.StringBuilder();
                buf.Append((char)Convert.ToInt32(data[11 + i], 16));
                NewLine.AddData.Add(myConstants.HeaderCh);
                NewLine.AddData.Add((i + 1).ToString());
                NewLine.AddData.Add(" 制御ﾓｰﾄ: ");
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("\r\n");
            }
            return;
        }

        // RR:ｽﾃｰﾀｽ読出し
        private void SplitRR_CP(string line)
        {
            NewLine.AddData = new List<string>();
            string[] data = line.Split(' ');
            System.Text.StringBuilder buf = new System.Text.StringBuilder();
            // SND
            if (line.Contains(myConstants.SndMsg))
            {
                NewLine.AddData.Add(myConstants.HeaderCP);
                buf.Append((char)Convert.ToInt32(data[6], 16));
                NewLine.AddData.Add(buf.ToString());   
                NewLine.AddData.Add("[RR] ｽﾃｰﾀｽ読出し\r\n");
                return;
            }

            // RCV
            if (line.Contains(myConstants.RcvMsg))
            {
                NewLine.AddData.Add(myConstants.HeaderCP);
                buf.Append((char)Convert.ToInt32(data[8], 16));
                NewLine.AddData.Add(buf.ToString());   
                NewLine.AddData.Add("[RR] ｽﾃｰﾀｽ読出し応答\r\n");
            }
            for (int i = 0; i < 4; i++)
            {
                buf = new System.Text.StringBuilder();
                NewLine.AddData.Add(myConstants.HeaderCh);
                NewLine.AddData.Add((i + 1).ToString());
                NewLine.AddData.Add(" ｽﾃｰﾀｽ: ");
                for (int j = 0; j < 4; j++)
                {
                    buf.Append((char)Convert.ToInt32(data[11 + j + (i * 4)], 16));
                }
                NewLine.AddData.Add("\r\n");
                
            }
            return;
        }

        // RX:制御ｾﾝｻ/外部ｾﾝｻ測定温度読出し
        private void SplitRX_CP(string line)
        {
            NewLine.AddData = new List<string>();
            string[] data = line.Split(' ');
            System.Text.StringBuilder buf = new System.Text.StringBuilder();
            // SND
            if (line.Contains(myConstants.SndMsg))
            {
                NewLine.AddData.Add(myConstants.HeaderCP);
                buf.Append((char)Convert.ToInt32(data[6], 16));
                NewLine.AddData.Add(buf.ToString());   
                NewLine.AddData.Add("[RX] 制御ｾﾝｻ/外部ｾﾝｻ測定温度読出し\r\n");
                return;
            }

            if (line.Contains(myConstants.RcvMsg))
            {
                NewLine.AddData.Add(myConstants.HeaderCP);
                buf.Append((char)Convert.ToInt32(data[8], 16));
                NewLine.AddData.Add(buf.ToString());  
                NewLine.AddData.Add("[RX] 制御ｾﾝｻ/外部ｾﾝｻ測定温度読出し応答\r\n");
            }
            for (int i = 0; i < 4; i++)
            {
                buf = new System.Text.StringBuilder();
                NewLine.AddData.Add(myConstants.HeaderCh);
                NewLine.AddData.Add((i + 1).ToString());
                NewLine.AddData.Add(" 制御ｾﾝｻ: ");
                for (int j = 0; j < 3; j++)
                {
                    buf.Append((char)Convert.ToInt32(data[11 + j + (i * 6)], 16));
                }
                NewLine.AddData.Add(buf.ToString());

                buf = new System.Text.StringBuilder();
                NewLine.AddData.Add(myConstants.HeaderCh);
                NewLine.AddData.Add((i + 1).ToString());
                NewLine.AddData.Add(" 外部ｾﾝｻ: ");
                for (int k = 0; k < 3; k++)
                {
                    buf.Append((char)Convert.ToInt32(data[14 + k + (i * 6)], 16));
                }
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("\r\n");
            }
            return;
        }

        // WB:PID定数及び表示温度校正値(ADJ)設定
        private void SplitWB_CP(string line)
        {
            NewLine.AddData = new List<string>();
            string[] data = line.Split(' ');
            System.Text.StringBuilder buf = new System.Text.StringBuilder();
            // SND
            if (line.Contains(myConstants.SndMsg))
            {
                NewLine.AddData.Add(myConstants.HeaderCP);
                buf.Append((char)Convert.ToInt32(data[6], 16));
                NewLine.AddData.Add(buf.ToString());  
                NewLine.AddData.Add("[WB] PID定数及び表示温度校正値(ADJ)設定\r\n");
            }
             
            for (int i = 0; i < 4; i++)
            {
                // P
                buf = new System.Text.StringBuilder();
                NewLine.AddData.Add(myConstants.HeaderCh);
                NewLine.AddData.Add((i + 1).ToString());
                NewLine.AddData.Add(" 比例帯温度幅: ");
                for (int p = 0; p < 3; p++)
                {
                    buf.Append((char)Convert.ToInt32(data[9 + p + (i * 3)], 16));
                }
                NewLine.AddData.Add(buf.ToString());

                // I
                buf = new System.Text.StringBuilder();
                NewLine.AddData.Add(myConstants.HeaderCh);
                NewLine.AddData.Add((i + 1).ToString());
                NewLine.AddData.Add(" 積分時間: ");
                for (int _i = 0; _i < 3; _i++)
                {
                    buf.Append((char)Convert.ToInt32(data[12 + _i + (i * 3)], 16));
                }
                NewLine.AddData.Add(buf.ToString());

                // D
                buf = new System.Text.StringBuilder();
                NewLine.AddData.Add(myConstants.HeaderCh);
                NewLine.AddData.Add((i + 1).ToString());
                NewLine.AddData.Add(" 微分時間: ");                
                for (int d = 0; d < 3; d++)
                {
                    buf.Append((char)Convert.ToInt32(data[15 + d + (i * 3)], 16));
                }
                NewLine.AddData.Add(buf.ToString());

                // ADJ
                buf = new System.Text.StringBuilder();
                NewLine.AddData.Add(myConstants.HeaderCh);
                NewLine.AddData.Add((i + 1).ToString());
                NewLine.AddData.Add(" ｵﾌｾｯﾄ値: ");
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
            string[] data = line.Split(' ');
            System.Text.StringBuilder buf = new System.Text.StringBuilder();
            // SND
            if (line.Contains(myConstants.SndMsg))
            {
                NewLine.AddData.Add(myConstants.HeaderCP);
                buf.Append((char)Convert.ToInt32(data[6], 16));
                NewLine.AddData.Add(buf.ToString());  
                NewLine.AddData.Add("[RB] PID定数及びｵﾌｾｯﾄ読出し\r\n");
                return;
            }

            if (line.Contains(myConstants.RcvMsg))
            {
                NewLine.AddData.Add(myConstants.HeaderCP);
                buf.Append((char)Convert.ToInt32(data[8], 16));
                NewLine.AddData.Add(buf.ToString());  
                NewLine.AddData.Add("[RB] PID定数及びｵﾌｾｯﾄ読出し応答\r\n");
            }
            for (int i = 0; i < 4; i++)
            {
                // P
                buf = new System.Text.StringBuilder();
                NewLine.AddData.Add(myConstants.HeaderCh);
                NewLine.AddData.Add((i + 1).ToString());
                NewLine.AddData.Add(" 比例帯温度幅: ");
                for (int p = 0; p < 3; p++)
                {
                    buf.Append((char)Convert.ToInt32(data[11 + p + (i * 3)], 16));
                }
                NewLine.AddData.Add(buf.ToString());

                // I
                buf = new System.Text.StringBuilder();
                NewLine.AddData.Add(myConstants.HeaderCh);
                NewLine.AddData.Add((i + 1).ToString());
                NewLine.AddData.Add(" 積分時間: ");
                for (int _i = 0; _i < 3; _i++)
                {
                    buf.Append((char)Convert.ToInt32(data[14 + _i + (i * 3)], 16));
                }
                NewLine.AddData.Add(buf.ToString());

                // D
                buf = new System.Text.StringBuilder();
                NewLine.AddData.Add(myConstants.HeaderCh);
                NewLine.AddData.Add((i + 1).ToString());
                NewLine.AddData.Add(" 微分時間: ");
                for (int d = 0; d < 3; d++)
                {
                    buf.Append((char)Convert.ToInt32(data[14 + d + (i * 3)], 16));
                }
                NewLine.AddData.Add(buf.ToString());

                // ADJ
                buf = new System.Text.StringBuilder();
                NewLine.AddData.Add(myConstants.HeaderCh);
                NewLine.AddData.Add((i + 1).ToString());
                NewLine.AddData.Add(" ｵﾌｾｯﾄ値: ");
                for (int adj = 0; adj < 3; adj++)
                {
                    buf.Append((char)Convert.ToInt32(data[17 + adj + (i * 4)], 16));
                }
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("\r\n");
            }
            return;
        }

        // WS:目標温度設定
        private void SplitWS_CP(string line)
        {
            NewLine.AddData = new List<string>();
            string[] data = line.Split(' ');
            System.Text.StringBuilder buf = new System.Text.StringBuilder();
            // SND
            if (line.Contains(myConstants.SndMsg))
            {
                NewLine.AddData.Add(myConstants.HeaderCP);
                buf.Append((char)Convert.ToInt32(data[6], 16));
                NewLine.AddData.Add(buf.ToString());  
                NewLine.AddData.Add("[WS] 目標温度設定\r\n");

                for (int i = 0; i < 4; i++)
                {
                    buf = new System.Text.StringBuilder();
                    NewLine.AddData.Add(myConstants.HeaderCh);
                    NewLine.AddData.Add((i + 1).ToString());
                    NewLine.AddData.Add(" 目標温度: ");
                    for (int j = 0; j < 3; j++)
                    {
                        buf.Append((char)Convert.ToInt32(data[9 + j + (i * 3)], 16));
                    }
                    NewLine.AddData.Add(buf.ToString());
                    NewLine.AddData.Add("\r\n");
                }
            }
  
        }

        // RS:目標温度読出し
        private void SplitRS_CP(string line)
        {
            NewLine.AddData = new List<string>();
            string[] data = line.Split(' ');
            System.Text.StringBuilder buf = new System.Text.StringBuilder();
            // SND
            if (line.Contains(myConstants.SndMsg))
            {
                NewLine.AddData.Add(myConstants.HeaderCP);
                buf.Append((char)Convert.ToInt32(data[6], 16));
                NewLine.AddData.Add(buf.ToString());  
                NewLine.AddData.Add("[RS] 目標温度読出し\r\n");
            }
            else if (line.Contains(myConstants.RcvMsg))
            {
                NewLine.AddData.Add(myConstants.HeaderCP);
                buf.Append((char)Convert.ToInt32(data[8], 16));
                NewLine.AddData.Add(buf.ToString());  
                NewLine.AddData.Add("[RS] 目標温度読出し応答\r\n");

                for (int i = 0; i < 4; i++)
                {
                    buf = new System.Text.StringBuilder();
                    NewLine.AddData.Add(myConstants.HeaderCh);
                    NewLine.AddData.Add((i + 1).ToString());
                    NewLine.AddData.Add(" 目標温度: ");
                    for (int j = 0; j < 3; j++)
                    {
                        buf.Append((char)Convert.ToInt32(data[11 + j + (i * 3)], 16));
                    }
                    NewLine.AddData.Add(buf.ToString());
                    NewLine.AddData.Add("\r\n");
                }
            }

        }

        // W%:上下限温度幅設定
        private void SplitWPe_CP(string line)
        {
            NewLine.AddData = new List<string>();
            string[] data = line.Split(' ');
            System.Text.StringBuilder buf = new System.Text.StringBuilder();
            // SND
            if (line.Contains(myConstants.SndMsg))
            {
                NewLine.AddData.Add(myConstants.HeaderCP);
                buf.Append((char)Convert.ToInt32(data[6], 16));
                NewLine.AddData.Add(buf.ToString()); 
                NewLine.AddData.Add("[W%] 上下温度幅設定\r\n");

                for (int i = 0; i < 4; i++)
                {
                    buf = new System.Text.StringBuilder();
                    NewLine.AddData.Add(myConstants.HeaderCh);
                    NewLine.AddData.Add((i + 1).ToString());
                    NewLine.AddData.Add(" 上限温度幅: ");
                    for (int j = 0; j < 3; j++)
                    {
                        buf.Append((char)Convert.ToInt32(data[9 + j + (i * 3)], 16));
                    }
                    NewLine.AddData.Add(buf.ToString());

                    buf = new System.Text.StringBuilder();
                    NewLine.AddData.Add(myConstants.HeaderCh);
                    NewLine.AddData.Add((i + 1).ToString());
                    NewLine.AddData.Add(" 下限温度幅: ");
                    for (int k = 0; k < 3; k++)
                    {
                        buf.Append((char)Convert.ToInt32(data[12 + k + (i * 3)], 16));
                    }
                    NewLine.AddData.Add(buf.ToString());
                    NewLine.AddData.Add("\r\n");

                }
            }

        }

        // R%:上下温度幅読出し
        private void SplitRPe_CP(string line)
        {
            NewLine.AddData = new List<string>();
            string[] data = line.Split(' ');
            System.Text.StringBuilder buf = new System.Text.StringBuilder();
            // SND
            if (line.Contains(myConstants.SndMsg))
            {
                NewLine.AddData.Add(myConstants.HeaderCP);
                buf.Append((char)Convert.ToInt32(data[6], 16));
                NewLine.AddData.Add(buf.ToString()); 
                NewLine.AddData.Add("[R%] 上下温度幅読出し\r\n");
            }
            else if (line.Contains(myConstants.RcvMsg))
            {
                NewLine.AddData.Add(myConstants.HeaderCP);
                buf.Append((char)Convert.ToInt32(data[8], 16));
                NewLine.AddData.Add(buf.ToString());  
                NewLine.AddData.Add("[R%] 上下温度幅読出し応答\r\n");

                for (int i = 0; i < 4; i++)
                {
                    buf = new System.Text.StringBuilder();
                    NewLine.AddData.Add(myConstants.HeaderCh);
                    NewLine.AddData.Add((i + 1).ToString());
                    NewLine.AddData.Add(" 上限温度幅: ");
                    for (int j = 0; j < 3; j++)
                    {
                        buf.Append((char)Convert.ToInt32(data[11 + j + (i * 3)], 16));
                    }
                    NewLine.AddData.Add(buf.ToString());

                    buf = new System.Text.StringBuilder();
                    NewLine.AddData.Add(myConstants.HeaderCh);
                    NewLine.AddData.Add((i + 1).ToString());
                    NewLine.AddData.Add(" 下限温度幅: ");
                    for (int k = 0; k < 3; k++)
                    {
                        buf.Append((char)Convert.ToInt32(data[14 + k + (i * 3)], 16));
                    }
                    NewLine.AddData.Add(buf.ToString());
                    NewLine.AddData.Add("\r\n");

                }
            }

        }

        // WP:Pb(演算開始定数設定)
        private void SplitWP_CP(string line)
        {
            NewLine.AddData = new List<string>();
            string[] data = line.Split(' ');
            System.Text.StringBuilder buf = new System.Text.StringBuilder();
            // SND
            if (line.Contains(myConstants.SndMsg))
            {
                NewLine.AddData.Add(myConstants.HeaderCP);
                buf.Append((char)Convert.ToInt32(data[6], 16));
                NewLine.AddData.Add(buf.ToString()); 
                NewLine.AddData.Add("[WP] 演算開始定数設定\r\n");

                for (int i = 0; i < 4; i++)
                {
                    buf = new System.Text.StringBuilder();
                    NewLine.AddData.Add(myConstants.HeaderCh);
                    NewLine.AddData.Add((i + 1).ToString());
                    NewLine.AddData.Add(" 演算開始定数: ");
                    for (int j = 0; j < 3; j++)
                    {
                        buf.Append((char)Convert.ToInt32(data[9 + j + (i * 3)], 16));
                    }
                    NewLine.AddData.Add(buf.ToString());
                    NewLine.AddData.Add("\r\n");
                }
            }

        }

        // RP:演算開始定数読出し
        private void SplitRP_CP(string line)
        {
            NewLine.AddData = new List<string>();
            string[] data = line.Split(' ');
            System.Text.StringBuilder buf = new System.Text.StringBuilder();
            // SND
            if (line.Contains(myConstants.SndMsg))
            {
                NewLine.AddData.Add(myConstants.HeaderCP);
                buf.Append((char)Convert.ToInt32(data[6], 16));
                NewLine.AddData.Add(buf.ToString()); 
                NewLine.AddData.Add("[RP] 演算開始定数読出し\r\n");
            }
            else if (line.Contains(myConstants.RcvMsg))
            {
                NewLine.AddData.Add(myConstants.HeaderCP);
                buf.Append((char)Convert.ToInt32(data[8], 16));
                NewLine.AddData.Add(buf.ToString());  
                NewLine.AddData.Add("[RP] 演算開始定数読出し応答\r\n");

                for (int i = 0; i < 4; i++)
                {
                    buf = new System.Text.StringBuilder();
                    NewLine.AddData.Add(myConstants.HeaderCh);
                    NewLine.AddData.Add((i + 1).ToString());
                    NewLine.AddData.Add(" 演算開始定数: ");
                    for (int j = 0; j < 3; j++)
                    {
                        buf.Append((char)Convert.ToInt32(data[11 + j + (i * 3)], 16));
                    }
                    NewLine.AddData.Add(buf.ToString());
                    NewLine.AddData.Add("\r\n");
                }
            }
        }
        #endregion

        #region polling
        // ER:エラーコード：
        private void SplitER_Polling(string line)
        {
            string[] data = line.Split(' ');
            NewLine.AddData = new List<string>();
            System.Text.StringBuilder buf = new System.Text.StringBuilder();
            if (line.Contains(myConstants.SndMsg))
            {
                NewLine.AddData.Add(myConstants.HeaderHP);
                buf.Append((char)Convert.ToInt32(data[7], 16));
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("[ER] エラーコード読出し\r\n");

                NewLine.RcvHpNodeNum = buf.ToString();
            }
            else if (line.Contains(myConstants.RcvMsg))
            {
                NewLine.AddData.Add(myConstants.HeaderHP);
                if (NewLine.RcvHpNodeNum != null)
                {

                    NewLine.AddData.Add(NewLine.RcvHpNodeNum);
                }
                else
                {
                    NewLine.AddData.Add(myConstants.ErrorNodeNum);
                }
                NewLine.AddData.Add("[ER] エラーコード読出し応答: ");
                buf = new System.Text.StringBuilder();
                for (int i = 0; i < 7; i++)
                {
                    buf.Append((char)Convert.ToInt32(data[10 + i], 16));
                }
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("\r\n");

                NewLine.RcvHpNodeNum = null;
            }
        }

        // C1：ﾘﾓｰﾄ･ﾛｰｶﾙ設定読出し
        private void SplitC1_Polling(string line)
        {
            string[] data = line.Split(' ');
            NewLine.AddData = new List<string>();
            System.Text.StringBuilder buf = new System.Text.StringBuilder();
            if (line.Contains(myConstants.SndMsg))
            {
                NewLine.AddData.Add(myConstants.HeaderHP);
                buf.Append((char)Convert.ToInt32(data[7], 16));
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("[C1] ﾘﾓｰﾄ･ﾛｰｶﾙ設定読出し\r\n");

                NewLine.RcvHpNodeNum = buf.ToString();
            }
            else if (line.Contains(myConstants.RcvMsg))
            {
                NewLine.AddData.Add(myConstants.HeaderHP);
                if (NewLine.RcvHpNodeNum != null)
                {

                    NewLine.AddData.Add(NewLine.RcvHpNodeNum);
                }
                else
                {
                    NewLine.AddData.Add(myConstants.ErrorNodeNum);
                }
                NewLine.AddData.Add("[C1] ﾘﾓｰﾄ･ﾛｰｶﾙ設定読出し応答\r\n");

                for (int i = 0; i < 4; i++)
                {
                    buf = new System.Text.StringBuilder();
                    buf.Append((char)Convert.ToInt32(data[13 + (i * 5)], 16));
                    NewLine.AddData.Add(myConstants.HeaderCh);
                    NewLine.AddData.Add((i + 1).ToString());
                    NewLine.AddData.Add(" ﾘﾓｰﾄ/ﾛｰｶﾙ設定   : ");
                    NewLine.AddData.Add(buf.ToString());
                    NewLine.AddData.Add("\r\n");

                    NewLine.RcvHpNodeNum = null;
                }
            } 
        }
        
        // S1:設定温度読み出し
        private void SplitS1_Polling(string line)
        {
            string[] data = line.Split(' ');
            NewLine.AddData = new List<string>();
            System.Text.StringBuilder buf = new System.Text.StringBuilder();
            if (line.Contains(myConstants.SndMsg))
            {
                NewLine.AddData.Add(myConstants.HeaderHP);
                buf.Append((char)Convert.ToInt32(data[7], 16));
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("[S1] 設定温度読出し\r\r\n");

                NewLine.RcvHpNodeNum = buf.ToString();
            }
            else if (line.Contains(myConstants.RcvMsg))
            {
                NewLine.AddData.Add(myConstants.HeaderHP);
                if (NewLine.RcvHpNodeNum != null)
                {

                    NewLine.AddData.Add(NewLine.RcvHpNodeNum);
                }
                else
                {
                    NewLine.AddData.Add(myConstants.ErrorNodeNum);
                } 
                NewLine.AddData.Add("[S1] 設定温度読出し応答\r\n");

                for (int i = 0; i < 4; i++)
                {
                    buf = new System.Text.StringBuilder();
                    for (int j = 0; j < 5; j++)
                    {
                        buf.Append((char)Convert.ToInt32(data[15 + j + (i * 11)], 16));
                    }
                    NewLine.AddData.Add(myConstants.HeaderCh);
                    NewLine.AddData.Add((i + 1).ToString());
                    NewLine.AddData.Add(" 目標温度  : ");
                    NewLine.AddData.Add(buf.ToString());
                    NewLine.AddData.Add("\r\n");
                }

                NewLine.RcvHpNodeNum = null;
            }
        }

        // G1:PID/AT切換
        private void SplitG1_Polling(string line)
        {
            string[] data = line.Split(' ');
            NewLine.AddData = new List<string>();
            System.Text.StringBuilder buf = new System.Text.StringBuilder();

            if (line.Contains(myConstants.SndMsg))
            {
                NewLine.AddData.Add(myConstants.HeaderHP);
                buf.Append((char)Convert.ToInt32(data[7], 16));
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("[G1] PID/AT切換\r\n");

                NewLine.RcvHpNodeNum = buf.ToString();
            }
            else if (line.Contains(myConstants.RcvMsg))
            {
                NewLine.AddData.Add(myConstants.HeaderHP);
                if (NewLine.RcvHpNodeNum != null)
                {

                    NewLine.AddData.Add(NewLine.RcvHpNodeNum);
                }
                else
                {
                    NewLine.AddData.Add(myConstants.ErrorNodeNum);
                } 
                NewLine.AddData.Add("[G1] PID/AT切換応答\r\n");

                for (int i = 0; i < 4; i++)
                {
                    buf = new System.Text.StringBuilder();
                    buf.Append((char)Convert.ToInt32(data[13 + (i * 5)], 16));
                    NewLine.AddData.Add(myConstants.HeaderCh);
                    NewLine.AddData.Add((i + 1).ToString());
                    NewLine.AddData.Add(" PID/AT切換  : ");
                    NewLine.AddData.Add(buf.ToString());
                    NewLine.AddData.Add("\r\n");
                }

                NewLine.RcvHpNodeNum = null;
            } 
        }

        // M1:測定温度
        private void SplitM1_Polling(string line)
        {
            string[] data = line.Split(' ');
            NewLine.AddData = new List<string>();
            System.Text.StringBuilder buf = new System.Text.StringBuilder();
            if (line.Contains(myConstants.SndMsg))
            {
                NewLine.AddData.Add(myConstants.HeaderHP);
                buf.Append((char)Convert.ToInt32(data[7], 16));
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("[M1] 測定温度読出し\r\n");

                NewLine.RcvHpNodeNum = buf.ToString();
            }
            else if (line.Contains(myConstants.RcvMsg))
            {
                NewLine.AddData.Add(myConstants.HeaderHP);
                if (NewLine.RcvHpNodeNum != null)
                {

                    NewLine.AddData.Add(NewLine.RcvHpNodeNum);
                }
                else
                {
                    NewLine.AddData.Add(myConstants.ErrorNodeNum);
                } 
                NewLine.AddData.Add("[M1] 測定温度読出し応答\r\n");

                for (int i = 0; i < 4; i++)
                {
                    buf = new System.Text.StringBuilder();
                    for (int j = 0; j < 5; j++)
                    {
                        buf.Append((char)Convert.ToInt32(data[15 + j + (i * 11)], 16));
                    }
                    NewLine.AddData.Add(myConstants.HeaderCh);
                    NewLine.AddData.Add((i + 1).ToString());
                    NewLine.AddData.Add(" 測定温度  : ");
                    NewLine.AddData.Add(buf.ToString());
                    NewLine.AddData.Add("\r\n");
                }

                NewLine.RcvHpNodeNum = null;
            }
        }

        // AJ:総合ｲﾍﾞﾝﾄ状態
        private void SplitAJ_Polling(string line)
        {
            string[] data = line.Split(' ');
            NewLine.AddData = new List<string>();
            System.Text.StringBuilder buf = new System.Text.StringBuilder();
            if (line.Contains(myConstants.SndMsg))
            {
                NewLine.AddData.Add(myConstants.HeaderHP);
                buf.Append((char)Convert.ToInt32(data[7], 16));
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("[AJ] 総合ｲﾍﾞﾝﾄ状態読出し\r\n");

                NewLine.RcvHpNodeNum = buf.ToString();
            }
            else if (line.Contains(myConstants.RcvMsg))
            {
                NewLine.AddData.Add(myConstants.HeaderHP);
                if (NewLine.RcvHpNodeNum != null)
                {

                    NewLine.AddData.Add(NewLine.RcvHpNodeNum);
                }
                else
                {
                    NewLine.AddData.Add(myConstants.ErrorNodeNum);
                } 
                NewLine.AddData.Add("[AJ] 総合ｲﾍﾞﾝﾄ状態読出し応答\r\n");

                for (int i = 0; i < 4; i++)
                {
                    buf = new System.Text.StringBuilder();
                    for (int j = 0; j < 7; j++)
                    {
                        buf.Append((char)Convert.ToInt32(data[13 + j + (i * 11)], 16));
                    }
                    NewLine.AddData.Add(myConstants.HeaderCh);
                    NewLine.AddData.Add((i + 1).ToString());
                    NewLine.AddData.Add(" 総合ｲﾍﾞﾝﾄ状態 : ");
                    NewLine.AddData.Add(buf.ToString());
                    NewLine.AddData.Add("\r\n");
                }
                NewLine.RcvHpNodeNum = null;
            }

        }
        #endregion

        #region selecting
        // SR:RUN/STOP設定
        private void SplitSR_Selecting(string line)
        {
            string[] data = line.Split(' ');
            NewLine.AddData = new List<string>();
            System.Text.StringBuilder buf = new System.Text.StringBuilder();
            if (line.Contains(myConstants.SndMsg))
            {
                NewLine.AddData.Add(myConstants.HeaderHP);
                buf.Append((char)Convert.ToInt32(data[7], 16));
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("[SR] RUN/STOP設定\r\n");
            }

        }

        // C1:HCFLG
        private void SplitC1_Selecting(string line)
        {
            string[] data = line.Split(' ');
            NewLine.AddData = new List<string>();
            System.Text.StringBuilder buf = new System.Text.StringBuilder();
            if (line.Contains(myConstants.SndMsg))
            {
                NewLine.AddData.Add(myConstants.HeaderHP);
                buf.Append((char)Convert.ToInt32(data[7], 16));
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("[C1] CH");
                buf.Append((char)Convert.ToInt32(data[12], 16));
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("HCFLG     : ");
                buf = new System.Text.StringBuilder();
                for (int i = 0; i < 4; i++)
                {
                    buf.Append((char)Convert.ToInt32(data[13 + i], 16));
                }
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("\r\n");
            }

        }

        // S1:温度設定
        private void SplitS1_Selecting(string line)
        {
            string[] data = line.Split(' ');
            NewLine.AddData = new List<string>();
            System.Text.StringBuilder buf = new System.Text.StringBuilder();
            if (line.Contains(myConstants.SndMsg))
            {
                NewLine.AddData.Add(myConstants.HeaderHP);
                buf.Append((char)Convert.ToInt32(data[7], 16));
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("[S1] CH");
                buf.Append((char)Convert.ToInt32(data[12], 16));
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add(" 目標温度  : ");
                buf = new System.Text.StringBuilder();
                for (int i = 0; i < 5; i++)
                {
                    buf.Append((char)Convert.ToInt32(data[16 + i], 16));
                }
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("\r\n");
            }
        }

        // A1:上限温度
        private void SplitA1_Selecting(string line)
        {
            string[] data = line.Split(' ');
            NewLine.AddData = new List<string>();
            System.Text.StringBuilder buf = new System.Text.StringBuilder();
            if (line.Contains(myConstants.SndMsg))
            {
                NewLine.AddData.Add(myConstants.HeaderHP);
                buf.Append((char)Convert.ToInt32(data[7], 16));
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("[A1] CH");
                buf.Append((char)Convert.ToInt32(data[12], 16));
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add(" 上限温度  : ");
                buf = new System.Text.StringBuilder();
                for (int i = 0; i < 4; i++)
                {
                    buf.Append((char)Convert.ToInt32(data[17 + i], 16));

                }
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("\r\n");
            } 
        }

        // A2:下限温度
        private void SplitA2_Selecting(string line)
        {
            string[] data = line.Split(' ');
            NewLine.AddData = new List<string>();
            System.Text.StringBuilder buf = new System.Text.StringBuilder();
            if (line.Contains(myConstants.SndMsg))
            {
                NewLine.AddData.Add(myConstants.HeaderHP);
                buf.Append((char)Convert.ToInt32(data[7], 16));
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("[A2] CH");

                buf.Append((char)Convert.ToInt32(data[12], 16));
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add(" 下限温度  : ");
                buf = new System.Text.StringBuilder();
                for (int i = 0; i < 4; i++)
                {
                    buf.Append((char)Convert.ToInt32(data[17 + i], 16));
                }
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("\r\n");
            }
        }

        // W1:ｱﾝﾁﾘｾｯﾄﾜｲﾝﾄﾞｱｯﾌﾟ(ARW)
        private void SplitW1_Selecting(string line)
        {
            string[] data = line.Split(' ');
            NewLine.AddData = new List<string>();
            System.Text.StringBuilder buf = new System.Text.StringBuilder();
            if (line.Contains(myConstants.SndMsg))
            {
                NewLine.AddData.Add(myConstants.HeaderHP);
                buf.Append((char)Convert.ToInt32(data[7], 16));
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("[W1] CH");

                buf.Append((char)Convert.ToInt32(data[12], 16));
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add(" ｱﾝﾁﾘｾｯﾄｲﾝﾄﾞｱｯﾌﾟ: ");
                buf = new System.Text.StringBuilder();
                for (int i = 0; i < 6; i++)
                {
                    buf.Append((char)Convert.ToInt32(data[13 + i], 16));
                }
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("\r\n");
            }

        }

        // J1:ｵｰﾄﾁｭｰﾆﾝｸﾞ(AT)実行
        private void SplitJ1_Selecting(string line)
        {
            string[] data = line.Split(' ');
            NewLine.AddData = new List<string>();
            System.Text.StringBuilder buf = new System.Text.StringBuilder();
            if (line.Contains(myConstants.SndMsg))
            {
                NewLine.AddData.Add(myConstants.HeaderHP);
                buf.Append((char)Convert.ToInt32(data[7], 16));
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("[J1] CH");

                buf.Append((char)Convert.ToInt32(data[12], 16));
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add(" ｵｰﾄﾁｭｰﾆﾝｸﾞ実行: ");
                buf = new System.Text.StringBuilder();
                for (int i = 0; i < 2; i++)
                {
                    buf.Append((char)Convert.ToInt32(data[13 + i], 16));
                }
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("\r\n");
            }

        }

        // P1:比例帯
        private void SplitP1_Selecting(string line)
        {
            string[] data = line.Split(' ');
            NewLine.AddData = new List<string>();
            System.Text.StringBuilder buf = new System.Text.StringBuilder();
            if (line.Contains(myConstants.SndMsg))
            {
                NewLine.AddData.Add(myConstants.HeaderHP);
                buf.Append((char)Convert.ToInt32(data[7], 16));
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("[P1] CH");

                buf.Append((char)Convert.ToInt32(data[12], 16));
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add(" 比例帯    : ");
                buf = new System.Text.StringBuilder();
                for (int i = 0; i < 5; i++)
                {
                    buf.Append((char)Convert.ToInt32(data[16 + i], 16));
                }
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("\r\n");
            }

        }

        // I1:積分時間
        private void SplitI1_Selecting(string line)
        {
            string[] data = line.Split(' ');
            NewLine.AddData = new List<string>();
            System.Text.StringBuilder buf = new System.Text.StringBuilder();
            if (line.Contains(myConstants.SndMsg))
            {
                NewLine.AddData.Add(myConstants.HeaderHP);
                buf.Append((char)Convert.ToInt32(data[7], 16));
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("[I1] CH");

                buf.Append((char)Convert.ToInt32(data[12], 16));
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add(" 積分時間  : ");
                buf = new System.Text.StringBuilder();
                for (int i = 0; i < 2; i++)
                {
                    buf.Append((char)Convert.ToInt32(data[18 + i], 16));
                }
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("\r\n");
            }

        }

        // D1:微分時間
        private void SplitD1_Selecting(string line)
        {
            string[] data = line.Split(' ');
            NewLine.AddData = new List<string>();
            System.Text.StringBuilder buf = new System.Text.StringBuilder();
            if (line.Contains(myConstants.SndMsg))
            {
                NewLine.AddData.Add(myConstants.HeaderHP);
                buf.Append((char)Convert.ToInt32(data[7], 16));
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("[D1] CH");

                buf.Append((char)Convert.ToInt32(data[12], 16));
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add(" 微分時間  : ");
                buf = new System.Text.StringBuilder();
                for (int i = 0; i < 2; i++)
                {
                    buf.Append((char)Convert.ToInt32(data[18 + i], 16));
                }
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("\r\n");
            }
        }
        #endregion

        #region ETU
        // WM:制御ﾓｰﾄﾞ設定
        private void SplitWM_ETU(string line)
        {
            NewLine.AddData = new List<string>();
            string[] data = line.Split(' ');
            System.Text.StringBuilder buf = new System.Text.StringBuilder();
            // SND
            if (line.Contains(myConstants.SpinSndMsg))
            {
                NewLine.AddData.Add(myConstants.HeaderEtu);
                buf.Append((char)Convert.ToInt32(data[8], 16));
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("[WM] 制御ﾓｰﾄﾞ設定\r\n");
                NewLine.AddData.Add("      制御ﾓｰﾄﾞ: ");
                NewLine.AddData.Add(data[13]);
                if (data[13] == "0F")
                {
                    NewLine.AddData.Add(wm[4]);
                }
                else
                {
                    NewLine.AddData.Add(wm[int.Parse(data[13])]);
                }
                NewLine.AddData.Add("\r\n");
            }
            else if (line.Contains(myConstants.SpinRcvmsg))
            {
                NewLine.AddData.Add(myConstants.HeaderEtu);
                buf.Append((char)Convert.ToInt32(data[8], 16));
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("[WM] 制御ﾓｰﾄﾞ設定応答\r\n");
            }

        }

        // RM:制御ﾓｰﾄﾞ読出し
        private void SplitRM_ETU(string line)
        {
            NewLine.AddData = new List<string>();
            string[] data = line.Split(' ');
            System.Text.StringBuilder buf = new System.Text.StringBuilder();
            // SND
            if (line.Contains(myConstants.SpinSndMsg))
            {
                NewLine.AddData.Add(myConstants.HeaderEtu);
                buf.Append((char)Convert.ToInt32(data[8], 16));
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("[RM] 制御ﾓｰﾄﾞ取得\r\n");
            }
            else if (line.Contains(myConstants.SpinRcvmsg))
            {
                NewLine.AddData.Add(myConstants.HeaderEtu);
                buf.Append((char)Convert.ToInt32(data[8], 16));
                NewLine.AddData.Add(buf.ToString());                          
                NewLine.AddData.Add("[RM] 制御ﾓｰﾄﾞ取得応答\r\n");
                NewLine.AddData.Add("      制御ﾓｰﾄﾞ: ");
                NewLine.AddData.Add(data[33]);
                NewLine.AddData.Add(rm[int.Parse(data[33])]);
                NewLine.AddData.Add("\r\n");

            } 

        }

        // RR:ｽﾃｰﾀｽ読出し
        private void SplitRR_ETU(string line)
        {
            NewLine.AddData = new List<string>();
            string[] data = line.Split(' ');
            System.Text.StringBuilder buf = new System.Text.StringBuilder();
            // SND
            if (line.Contains(myConstants.SpinSndMsg))
            {
                NewLine.AddData.Add(myConstants.HeaderEtu);
                buf.Append((char)Convert.ToInt32(data[8], 16));
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("[RR] ｽﾃｰﾀｽ読出し\r\n");
            }
            else if (line.Contains(myConstants.SpinRcvmsg))
            {
                NewLine.AddData.Add(myConstants.HeaderEtu);
                buf.Append((char)Convert.ToInt32(data[8], 16));
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("[RR] ｽﾃｰﾀｽ読出し応答\r\n");
                NewLine.AddData.Add("      ｽﾃｰﾀｽ: ");
                NewLine.AddData.Add(data[33]);
                NewLine.AddData.Add(data[34]);
                NewLine.AddData.Add("\r\n");
            }

        }

        // RX:制御ｾﾝｻ/外部ｾﾝｻ測定温度読出し
        private void SplitRX_ETU(string line)
        {
            NewLine.AddData = new List<string>();
            string[] data = line.Split(' ');
            System.Text.StringBuilder buf = new System.Text.StringBuilder();
            // SND
            if (line.Contains(myConstants.SpinSndMsg))
            {
                NewLine.AddData.Add(myConstants.HeaderEtu);
                buf.Append((char)Convert.ToInt32(data[8], 16));
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("[RX] 制御ｾﾝｻ/外部ｾﾝｻ測定温度読出し\r\n");
            }
            else if (line.Contains(myConstants.SpinRcvmsg))
            {
                NewLine.AddData.Add(myConstants.HeaderEtu);
                buf.Append((char)Convert.ToInt32(data[8], 16));
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("[RX] 制御ｾﾝｻ/外部ｾﾝｻ測定温度読出し応答\r\n");

                NewLine.AddData.Add("      制御ｾﾝｻ: ");
                NewLine.AddData.Add(data[33]);
                NewLine.AddData.Add(".");
                NewLine.AddData.Add(data[34]);

                NewLine.AddData.Add("   外部ｾﾝｻ: ");    // 0FFF
                NewLine.AddData.Add(data[35]);
                NewLine.AddData.Add(data[36]);
                NewLine.AddData.Add("\r\n");
            }

        }

        // WB:PID定数及び表示温度校正値(ADJ)設定
        private void SplitWB_ETU(string line)
        {
            NewLine.AddData = new List<string>();
            string[] data = line.Split(' ');
            System.Text.StringBuilder buf = new System.Text.StringBuilder();
            // SND
            if (line.Contains(myConstants.SpinSndMsg))
            {
                NewLine.AddData.Add(myConstants.HeaderEtu);
                buf.Append((char)Convert.ToInt32(data[8], 16));
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("[WB] PID定数及び表示温度校正値(ADJ)設定\r\n");

                // P
                NewLine.AddData.Add("      比例帯温度幅: ");
                NewLine.AddData.Add(data[13]);
                NewLine.AddData.Add(".");
                NewLine.AddData.Add(data[14]);

                // I
                NewLine.AddData.Add("   積分時間: ");
                NewLine.AddData.Add(data[15]);
                NewLine.AddData.Add(data[16]);

                // D
                NewLine.AddData.Add("   微分時間: ");
                NewLine.AddData.Add(data[17]);
                NewLine.AddData.Add(data[18]);

                // ADJ
                NewLine.AddData.Add("   ｵﾌｾｯﾄ値: ");

                int tmp = int.Parse(data[19], System.Globalization.NumberStyles.HexNumber);
                // 符号判定
                if ((tmp & 0xF0) == 0x10)
                {
                    NewLine.AddData.Add("-");
                    NewLine.AddData.Add((tmp & 0x0F).ToString());
                }
                else
                {
                    NewLine.AddData.Add((tmp & 0xFF).ToString());
                }
                NewLine.AddData.Add(".");
                NewLine.AddData.Add(data[20]);

                NewLine.AddData.Add("\r\n");

            }
            else if(line.Contains(myConstants.SpinRcvmsg))
            {
                NewLine.AddData.Add(myConstants.HeaderEtu);
                buf.Append((char)Convert.ToInt32(data[8], 16));
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("[WB] PID定数及び表示温度校正値(ADJ)設定応答\r\n");

            }
            
        }

        // RB:PID定数及び表示温度校正値(ADJ)読出し
        private void SplitRB_ETU(string line)
        {
            NewLine.AddData = new List<string>();
            string[] data = line.Split(' ');
            System.Text.StringBuilder buf = new System.Text.StringBuilder();
            // SND
            if (line.Contains(myConstants.SpinSndMsg))
            {
                NewLine.AddData.Add(myConstants.HeaderEtu);
                buf.Append((char)Convert.ToInt32(data[8], 16));
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("[RB] PID定数及び表示温度校正値(ADJ)設定読出し\r\n");
            }
            else if (line.Contains(myConstants.SpinRcvmsg))
            {
                NewLine.AddData.Add(myConstants.HeaderEtu);
                buf.Append((char)Convert.ToInt32(data[8], 16));
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("[RB] PID定数及び表示温度校正値(ADJ)読出し応答\r\n");

                // P
                NewLine.AddData.Add("      比例帯温度幅: ");
                NewLine.AddData.Add(data[33]);
                NewLine.AddData.Add(".");
                NewLine.AddData.Add(data[34]);

                // I
                NewLine.AddData.Add("   積分時間: ");
                NewLine.AddData.Add(data[35]);
                NewLine.AddData.Add(data[36]);

                // D
                NewLine.AddData.Add("   微分時間: ");
                NewLine.AddData.Add(data[37]);
                NewLine.AddData.Add(data[38]);

                // ADJ
                NewLine.AddData.Add("   ｵﾌｾｯﾄ値: ");

                int tmp = int.Parse(data[39], System.Globalization.NumberStyles.HexNumber);
                // 符号判定
                if ((tmp & 0xF0) == 0x10)
                {
                    NewLine.AddData.Add("-");
                    NewLine.AddData.Add((tmp & 0x0F).ToString());
                }
                else
                {
                    NewLine.AddData.Add((tmp & 0xFF).ToString());
                }
                NewLine.AddData.Add(".");
                NewLine.AddData.Add(data[40]);

                NewLine.AddData.Add("\r\n");
            }
  
        }

        // WS:目標温度設定
        private void SplitWS_ETU(string line)
        {
            NewLine.AddData = new List<string>();
            string[] data = line.Split(' ');
            System.Text.StringBuilder buf = new System.Text.StringBuilder();
            // SND
            if (line.Contains(myConstants.SpinSndMsg))
            {
                NewLine.AddData.Add(myConstants.HeaderEtu);
                buf.Append((char)Convert.ToInt32(data[8], 16));
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("[WS] 目標温度設定\r\n");

                NewLine.AddData.Add("      目標温度: ");
                double tmp = (double.Parse(data[13]) * 10 + double.Parse(data[14])/10);
                NewLine.AddData.Add(tmp.ToString());

                NewLine.AddData.Add("\r\n");
            }
            else if (line.Contains(myConstants.SpinRcvmsg))
            {
                NewLine.AddData.Add(myConstants.HeaderEtu);
                buf.Append((char)Convert.ToInt32(data[8], 16));
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("[WS] 目標温度設定応答\r\n");

            }

        }

        // RS:目標温度読出し
        private void SplitRS_ETU(string line)
        {
            NewLine.AddData = new List<string>();
            string[] data = line.Split(' ');
            System.Text.StringBuilder buf = new System.Text.StringBuilder();
            // SND
            if (line.Contains(myConstants.SpinSndMsg))
            {
                NewLine.AddData.Add(myConstants.HeaderEtu);
                buf.Append((char)Convert.ToInt32(data[8], 16));
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("[RS] 目標温度読出し\r\n");
            }
            else if (line.Contains(myConstants.SpinRcvmsg))
            {
                NewLine.AddData.Add(myConstants.HeaderEtu);
                buf.Append((char)Convert.ToInt32(data[8], 16));
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("[RS] 目標温度読出し応答\r\n");

                NewLine.AddData.Add("      目標温度: ");
                double tmp = (double.Parse(data[33]) * 10 + double.Parse(data[34]) / 10);
                NewLine.AddData.Add(tmp.ToString());

                NewLine.AddData.Add("\r\n");
            }

        }

        // W%:上下限温度幅設定
        private void SplitWPe_ETU(string line)
        {
            NewLine.AddData = new List<string>();
            string[] data = line.Split(' ');
            System.Text.StringBuilder buf = new System.Text.StringBuilder();
            // SND
            if (line.Contains(myConstants.SpinSndMsg))
            {
                NewLine.AddData.Add(myConstants.HeaderEtu);
                buf.Append((char)Convert.ToInt32(data[8], 16));
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("[W%] 上下温度幅設定\r\n");

                NewLine.AddData.Add("      上限温度幅: ");
                double tmp = (double.Parse(data[13]) * 10 + double.Parse(data[14]) / 10);
                NewLine.AddData.Add(tmp.ToString());

                NewLine.AddData.Add("      下限温度幅: ");
                tmp = (double.Parse(data[15]) * 10 + double.Parse(data[16]) / 10);
                NewLine.AddData.Add(tmp.ToString());   

                NewLine.AddData.Add("\r\n");

            }
            else if (line.Contains(myConstants.SpinRcvmsg))
            {
                NewLine.AddData.Add(myConstants.HeaderEtu);
                buf.Append((char)Convert.ToInt32(data[8], 16));
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("[W%] 上下温度幅設定応答\r\n");

            }


        }

        // R%:上下限温度幅読出し
        private void SplitRPe_ETU(string line)
        {
            NewLine.AddData = new List<string>();
            string[] data = line.Split(' ');
            System.Text.StringBuilder buf = new System.Text.StringBuilder();
            // SND
            if (line.Contains(myConstants.SpinSndMsg))
            {
                NewLine.AddData.Add(myConstants.HeaderEtu);
                buf.Append((char)Convert.ToInt32(data[8], 16));
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("[R%] 上下限温度幅読出し\r\n");
            }
            else if (line.Contains(myConstants.SpinRcvmsg))
            {
                NewLine.AddData.Add(myConstants.HeaderEtu);
                buf.Append((char)Convert.ToInt32(data[8], 16));
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("[R%] 上下限温度幅読出し応答\r\n");

                NewLine.AddData.Add("      上限温度幅: ");
                double tmp = (double.Parse(data[33]) * 10 + double.Parse(data[34]) / 10);
                NewLine.AddData.Add(tmp.ToString());

                NewLine.AddData.Add("      下限温度幅: ");
                tmp = (double.Parse(data[35]) * 10 + double.Parse(data[36]) / 10);
                NewLine.AddData.Add(tmp.ToString());   

                NewLine.AddData.Add("\r\n");
            }

        }

        // WU:制御用ｾﾝｻ及び外部ｾﾝｻ微調整値設定
        private void SplitWU_ETU(string line)
        {
            NewLine.AddData = new List<string>();
            string[] data = line.Split(' ');
            System.Text.StringBuilder buf = new System.Text.StringBuilder();
            // SND
            if (line.Contains(myConstants.SpinSndMsg))
            {
                NewLine.AddData.Add(myConstants.HeaderEtu);
                buf.Append((char)Convert.ToInt32(data[8], 16));
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("[WU] 制御用ｾﾝｻ及び外部ｾﾝｻ微調整値設定\r\n");

                NewLine.AddData.Add("      制御用ｾﾝｻ及び外部ｾﾝｻ微調整値: ");

                int tmp = int.Parse(data[13], System.Globalization.NumberStyles.HexNumber);
                // 符号判定
                if ((tmp & 0xF0) == 0x10)
                {
                    NewLine.AddData.Add("-");
                    NewLine.AddData.Add((tmp & 0x0F).ToString());
                }
                else
                {
                    NewLine.AddData.Add((tmp & 0xFF).ToString());
                }
                NewLine.AddData.Add(".");
                NewLine.AddData.Add(data[14]);
                NewLine.AddData.Add("\r\n");
            }
            else if (line.Contains(myConstants.SpinRcvmsg))
            {
                NewLine.AddData.Add(myConstants.HeaderEtu);
                buf.Append((char)Convert.ToInt32(data[8], 16));
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("[WU] 制御用ｾﾝｻ及び外部ｾﾝｻ微調整値応答\r\n");

            }
        }

        // RU:制御用ｾﾝｻ及び外部ｾﾝｻ微調整値読出し
        private void SplitRU_ETU(string line)
        {
            NewLine.AddData = new List<string>();
            string[] data = line.Split(' ');
            System.Text.StringBuilder buf = new System.Text.StringBuilder();
            // SND
            if (line.Contains(myConstants.SpinSndMsg))
            {
                NewLine.AddData.Add(myConstants.HeaderEtu);
                buf.Append((char)Convert.ToInt32(data[8], 16));
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("[RU] 制御用ｾﾝｻ及び外部ｾﾝｻ微調整値読出し\r\n");
            }
            else if (line.Contains(myConstants.SpinRcvmsg))
            {
                NewLine.AddData.Add(myConstants.HeaderEtu);
                buf.Append((char)Convert.ToInt32(data[8], 16));
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("[RU] 制御用ｾﾝｻ及び外部ｾﾝｻ微調整値読出し応答\r\n");

                NewLine.AddData.Add("      制御用ｾﾝｻ及び外部ｾﾝｻ微調整値: ");

                int tmp = int.Parse(data[33], System.Globalization.NumberStyles.HexNumber);
                // 符号判定
                if ((tmp & 0xF0) == 0x10)
                {
                    NewLine.AddData.Add("-");
                    NewLine.AddData.Add((tmp & 0x0F).ToString());
                }
                else
                {
                    NewLine.AddData.Add((tmp & 0xFF).ToString());
                }
                NewLine.AddData.Add(".");
                NewLine.AddData.Add(data[34]);

                NewLine.AddData.Add("\r\n");
            }

        }

        // WA:ARW幅設定
        private void SplitWA_ETU(string line)
        {
            NewLine.AddData = new List<string>();
            string[] data = line.Split(' ');
            System.Text.StringBuilder buf = new System.Text.StringBuilder();
            // SND
            if (line.Contains(myConstants.SpinSndMsg))
            {
                NewLine.AddData.Add(myConstants.HeaderEtu);
                buf.Append((char)Convert.ToInt32(data[8], 16));
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("[WA] ARW幅設定\r\n");

                NewLine.AddData.Add("      ARW幅: ");
                NewLine.AddData.Add(data[13]);
                NewLine.AddData.Add(".");
                NewLine.AddData.Add(data[14]);

                NewLine.AddData.Add("\r\n");
            }
            else if (line.Contains(myConstants.SpinRcvmsg))
            {
                NewLine.AddData.Add(myConstants.HeaderEtu);
                buf.Append((char)Convert.ToInt32(data[8], 16));
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("[WA] ARW幅設定応答\r\n");

            }


        }

        // RA:ARW幅読出し
        private void SplitRA_ETU(string line)
        {
            NewLine.AddData = new List<string>();
            string[] data = line.Split(' ');
            System.Text.StringBuilder buf = new System.Text.StringBuilder();
            // SND
            if (line.Contains(myConstants.SpinSndMsg))
            {
                NewLine.AddData.Add(myConstants.HeaderEtu);
                buf.Append((char)Convert.ToInt32(data[8], 16));
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("[RA] ARW幅読出し\r\n");
            }
            else if (line.Contains(myConstants.SpinRcvmsg))
            {
                NewLine.AddData.Add(myConstants.HeaderEtu);
                buf.Append((char)Convert.ToInt32(data[8], 16));
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("[RA] ARW幅読出し応答\r\n");

                NewLine.AddData.Add("      ARW幅: ");
                NewLine.AddData.Add(data[33]);
                NewLine.AddData.Add(".");
                NewLine.AddData.Add(data[34]);

                NewLine.AddData.Add("\r\n");
            }

        }

        // RV:ｿﾌﾄﾊﾞｰｼﾞｮﾝ読出し
        private void SplitRV_ETU(string line)
        {
            NewLine.AddData = new List<string>();
            string[] data = line.Split(' ');
            System.Text.StringBuilder buf = new System.Text.StringBuilder();
            // SND
            if (line.Contains(myConstants.SpinSndMsg))
            {
                NewLine.AddData.Add(myConstants.HeaderEtu);
                buf.Append((char)Convert.ToInt32(data[8], 16));
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("[RV] ｿﾌﾄﾊﾞｰｼﾞｮﾝ読出し\r\n");
            }
            else if (line.Contains(myConstants.SpinRcvmsg))
            {
                NewLine.AddData.Add(myConstants.HeaderEtu);
                buf.Append((char)Convert.ToInt32(data[8], 16));
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("[RV] ｿﾌﾄﾊﾞｰｼﾞｮﾝ読出し応答\r\n");

                NewLine.AddData.Add("      ｿﾌﾄﾊﾞｰｼﾞｮﾝ: ");
                NewLine.AddData.Add(data[33]);
                NewLine.AddData.Add(".");
                NewLine.AddData.Add(data[34]);

                NewLine.AddData.Add("\r\n");
            }

        }
        #endregion

        #region ACU

        // M1:制御出口空気温度測定値
        private void SplitM1_ACU(string line)
        {
            string[] data = line.Split(' ');
            NewLine.AddData = new List<string>();
            System.Text.StringBuilder buf = new System.Text.StringBuilder();
            if (line.Contains(myConstants.SpinSndMsg))
            {
                NewLine.AddData.Add(myConstants.HeaderAcu);
                buf.Append((char)Convert.ToInt32(data[6], 16));
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("[M1] 制御出口空気温度測定値読出し\r\n");

                NewLine.RcvAcuNodeNum = buf.ToString();

            }
            else if (line.Contains(myConstants.SpinRcvmsg))
            {
                NewLine.AddData.Add(myConstants.HeaderAcu);
                if (NewLine.RcvAcuNodeNum != null)
                {

                    NewLine.AddData.Add(NewLine.RcvAcuNodeNum);
                }
                else
                {
                    NewLine.AddData.Add(myConstants.ErrorNodeNum);
                } 
                NewLine.AddData.Add("[M1] 制御出口空気温度測定値読出し応答\r\n");
                buf = new System.Text.StringBuilder();
                for (int i = 0; i < 6; i++)
                {
                    buf.Append((char)Convert.ToInt32(data[10 + i], 16));
                }
                NewLine.AddData.Add("      測定温度  : ");
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("\r\n");

                NewLine.RcvAcuNodeNum = null;
            }
        }

        // M5:制御出口空気湿度測定値
        private void SplitM5_ACU(string line)
        {
            string[] data = line.Split(' ');
            NewLine.AddData = new List<string>();
            System.Text.StringBuilder buf = new System.Text.StringBuilder();
            if (line.Contains(myConstants.SpinSndMsg))
            {
                NewLine.AddData.Add(myConstants.HeaderAcu);
                buf.Append((char)Convert.ToInt32(data[6], 16));
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("[M5] 制御出口空気湿度測定値読出し\r\n");

                NewLine.RcvAcuNodeNum = buf.ToString();

            }
            else if (line.Contains(myConstants.SpinRcvmsg))
            {
                NewLine.AddData.Add(myConstants.HeaderAcu);
                if (NewLine.RcvAcuNodeNum != null)
                {
                    NewLine.AddData.Add(NewLine.RcvAcuNodeNum);
                }
                else
                {
                    NewLine.AddData.Add(myConstants.ErrorNodeNum);
                }
                NewLine.AddData.Add("[M5] 制御出口空気湿度測定値読出し応答\r\n");                
                for (int i = 0; i < 5; i++)
                {
                    buf.Append((char)Convert.ToInt32(data[9 + i], 16));
                }
                NewLine.AddData.Add("      測定湿度  : ");
                NewLine.AddData.Add(buf.ToString());

                buf = new System.Text.StringBuilder();
                for (int i = 0; i < 4; i++)
                {
                    buf.Append((char)Convert.ToInt32(data[17 + i], 16));
                }
                NewLine.AddData.Add("   測定温度  : ");
                NewLine.AddData.Add(buf.ToString());

                NewLine.AddData.Add("\r\n");

                NewLine.RcvAcuNodeNum = null;
            }
        }

        // S1:制御出口空気温度設定値
        private void SplitS1_ACU(string line)
        {
            string[] data = line.Split(' ');
            NewLine.AddData = new List<string>();
            System.Text.StringBuilder buf = new System.Text.StringBuilder();
            if (line.Contains(myConstants.SpinSndMsg))
            {
                // polling
                if (data.Length == 10)
                {
                    NewLine.AddData.Add(myConstants.HeaderAcu);
                    buf.Append((char)Convert.ToInt32(data[6], 16));
                    NewLine.AddData.Add(buf.ToString());
                    NewLine.AddData.Add("[S1] 制御出口空気温度設定温度読出し\r\n");

                    NewLine.RcvAcuNodeNum = buf.ToString();
                }
                // selecting
                else
                {
                    NewLine.AddData.Add(myConstants.HeaderAcu);
                    buf.Append((char)Convert.ToInt32(data[6], 16));
                    NewLine.AddData.Add(buf.ToString());
                    NewLine.AddData.Add("      [S1] 制御出口空気温度設定  : ");
                    buf = new System.Text.StringBuilder();
                    for (int i = 0; i < 5; i++)
                    {
                        buf.Append((char)Convert.ToInt32(data[14 + i], 16));
                    }
                    NewLine.AddData.Add(buf.ToString());
                    NewLine.AddData.Add("\r\n");
                }

            }
            else if (line.Contains(myConstants.SpinRcvmsg))
            {
                NewLine.AddData.Add(myConstants.HeaderAcu);
                if (NewLine.RcvAcuNodeNum != null)
                {

                    NewLine.AddData.Add(NewLine.RcvAcuNodeNum);
                }
                else
                {
                    NewLine.AddData.Add(myConstants.ErrorNodeNum);
                }
                NewLine.AddData.Add("[S1] 制御出口空気温度設定温度読出し応答\r\n");
                buf = new System.Text.StringBuilder();
                for (int i = 0; i < 6; i++)
                {
                    buf.Append((char)Convert.ToInt32(data[10 + i], 16));
                }
                NewLine.AddData.Add("      設定温度  : ");
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("\r\n");

                NewLine.RcvAcuNodeNum = null;
            }
        }

        // S5:制御出口空気湿度設定値
        private void SplitS5_ACU(string line)
        {
            string[] data = line.Split(' ');
            NewLine.AddData = new List<string>();
            System.Text.StringBuilder buf = new System.Text.StringBuilder();
            if (line.Contains(myConstants.SpinSndMsg))
            {
                // polling
                if (data.Length == 10)
                {
                    NewLine.AddData.Add(myConstants.HeaderAcu);
                    buf.Append((char)Convert.ToInt32(data[6], 16));
                    NewLine.AddData.Add(buf.ToString());
                    NewLine.AddData.Add("[S5] 制御出口空気湿度設定値読出し\r\n");

                    NewLine.RcvAcuNodeNum = buf.ToString();
                }
                // selecting
                else
                {
                    NewLine.AddData.Add(myConstants.HeaderAcu);
                    buf.Append((char)Convert.ToInt32(data[6], 16));
                    NewLine.AddData.Add(buf.ToString());                    
                    buf = new System.Text.StringBuilder();
                    for (int i = 0; i < 5; i++)
                    {
                        buf.Append((char)Convert.ToInt32(data[14 + i], 16));
                    }
                    NewLine.AddData.Add("      [S5] 制御出口空気温度設定  : ");
                    NewLine.AddData.Add(buf.ToString());

                    for (int i = 0; i < 4; i++)
                    {
                        buf.Append((char)Convert.ToInt32(data[20 + i], 16));
                    }
                    NewLine.AddData.Add("   制御出口空気湿度設定  : ");
                    NewLine.AddData.Add(buf.ToString());

                    NewLine.AddData.Add("\r\n");
                }

            }
            else if (line.Contains(myConstants.SpinRcvmsg))
            {
                NewLine.AddData.Add(myConstants.HeaderAcu);
                if (NewLine.RcvAcuNodeNum != null)
                {

                    NewLine.AddData.Add(NewLine.RcvAcuNodeNum);
                }
                else
                {
                    NewLine.AddData.Add(myConstants.ErrorNodeNum);
                }
                NewLine.AddData.Add("[S5] 制御出口空気湿度設定値読出し応答\r\n");
                buf = new System.Text.StringBuilder();
                for (int i = 0; i < 5; i++)
                {
                    buf.Append((char)Convert.ToInt32(data[9 + i], 16));
                }
                NewLine.AddData.Add("      設定温度  : ");
                NewLine.AddData.Add(buf.ToString());

                for (int i = 0; i < 4; i++)
                {
                    buf.Append((char)Convert.ToInt32(data[17 + i], 16));
                }
                NewLine.AddData.Add("   設定湿度  : ");
                NewLine.AddData.Add(buf.ToString());

                NewLine.AddData.Add("\r\n");

                NewLine.RcvAcuNodeNum = null;
            }
        }

        // JO:運転状態
        private void SplitJO_ACU(string line)
        {
            string[] data = line.Split(' ');
            NewLine.AddData = new List<string>();
            System.Text.StringBuilder buf = new System.Text.StringBuilder();
            if (line.Contains(myConstants.SpinSndMsg))
            {
                // polling
                if (data.Length == 11)
                {
                    NewLine.AddData.Add(myConstants.HeaderAcu);
                    buf.Append((char)Convert.ToInt32(data[6], 16));
                    NewLine.AddData.Add(buf.ToString());
                    NewLine.AddData.Add("[JO] 運転状態読出し\r\n");

                    NewLine.RcvAcuNodeNum = buf.ToString();
                }
                // selecting
                else
                {
                    NewLine.AddData.Add(myConstants.HeaderAcu);
                    buf.Append((char)Convert.ToInt32(data[6], 16));
                    NewLine.AddData.Add(buf.ToString());
                    NewLine.AddData.Add("      [JO] 運転状態設定  : ");
                    buf.Append((char)Convert.ToInt32(data[11], 16));
                    NewLine.AddData.Add(buf.ToString());
                    NewLine.AddData.Add(joData[int.Parse(buf.ToString())]);
                    NewLine.AddData.Add("\r\n");
                }

            }
            else if (line.Contains(myConstants.SpinRcvmsg))
            {
                NewLine.AddData.Add(myConstants.HeaderAcu);
                if (NewLine.RcvAcuNodeNum != null)
                {

                    NewLine.AddData.Add(NewLine.RcvAcuNodeNum);
                }
                else
                {
                    NewLine.AddData.Add(myConstants.ErrorNodeNum);
                }
                NewLine.AddData.Add("[JO] 運転状態読出し応答\r\n");
                buf.Append((char)Convert.ToInt32(data[8], 16));
                NewLine.AddData.Add("      運転状態  : ");
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add(joData[int.Parse(buf.ToString())]);
                NewLine.AddData.Add("\r\n");

                NewLine.RcvAcuNodeNum = buf.ToString();
            }
        }

        // ER:警報信号
        private void SplitER_ACU(string line)
        {
            string[] data = line.Split(' ');
            NewLine.AddData = new List<string>();
            System.Text.StringBuilder buf = new System.Text.StringBuilder();
            if (line.Contains(myConstants.SpinSndMsg))
            {
                NewLine.AddData.Add(myConstants.HeaderAcu);
                buf.Append((char)Convert.ToInt32(data[6], 16));
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("[ER] 警報信号読出し\r\n");

                NewLine.RcvAcuNodeNum = buf.ToString();

            }
            else if (line.Contains(myConstants.SpinRcvmsg))
            {
                NewLine.AddData.Add(myConstants.HeaderAcu);
                if (NewLine.RcvAcuNodeNum != null)
                {

                    NewLine.AddData.Add(NewLine.RcvAcuNodeNum);
                }
                else
                {
                    NewLine.AddData.Add(myConstants.ErrorNodeNum);
                }
                NewLine.AddData.Add("[ER] 警報信号読出し応答\r\n");
                buf = new System.Text.StringBuilder();
                for (int i = 0; i < 4; i++)
                {
                    buf.Append((char)Convert.ToInt32(data[8 + i], 16));
                }
                NewLine.AddData.Add("      警報信号  : ");
                NewLine.AddData.Add(buf.ToString());
                NewLine.AddData.Add("\r\n");

                NewLine.RcvAcuNodeNum = buf.ToString();

            }

        }
        
        #endregion
        #endregion

        /// <summary>
        /// ﾒｯｾｰｼﾞがどのﾒｯｾｰｼﾞに該当するか調べる
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private MsgInfo CheckMsg(string line)
        {
            System.Text.StringBuilder ComAscii = new System.Text.StringBuilder();
            string[] data = line.Split(' ');
            OriginalMsg = new MsgInfo();

            // BAKE SND
            if(line.Contains(myConstants.SndMsg))
            {
                // CP
                if (data[5] == myConstants.STX)
                {
                    ComAscii.Append((char)Convert.ToInt32(data[myConstants.ComCpSndPosi], 16));
                    ComAscii.Append((char)Convert.ToInt32(data[myConstants.ComCpSndPosi + 1], 16));
                    
                    OriginalMsg.command = ComAscii.ToString();
                    OriginalMsg.MsgKind = 1;
                    return OriginalMsg;
                }
                else if (data[5] == myConstants.EOT)
                {
                    // Polling
                    if (data[8] == myConstants.PollingMark)
                    {
                        ComAscii.Append((char)Convert.ToInt32(data[myConstants.ComPollingSndPosi], 16));
                        ComAscii.Append((char)Convert.ToInt32(data[myConstants.ComPollingSndPosi + 1], 16));

                        OriginalMsg.command = ComAscii.ToString();
                        OriginalMsg.MsgKind = 2;
                        return OriginalMsg;
                    }
                    // Selecting
                    if (data[8] != myConstants.PollingMark)
                    {
                        ComAscii.Append((char)Convert.ToInt32(data[myConstants.ComSelectingPosi], 16));
                        ComAscii.Append((char)Convert.ToInt32(data[myConstants.ComSelectingPosi + 1], 16));

                        OriginalMsg.command = ComAscii.ToString();
                        OriginalMsg.MsgKind = 3;
                        return OriginalMsg;

                    }
                }

            }

            // SPIN SND
            if(line.Contains(myConstants.SpinSndMsg))
            {
                // ETU
                if (data[5] == myConstants.STX && data[6] == myConstants.EtuMark)
                {
                    ComAscii.Append((char)Convert.ToInt32(data[myConstants.ComEtuPosi], 16));
                    ComAscii.Append((char)Convert.ToInt32(data[myConstants.ComEtuPosi + 1], 16));

                    OriginalMsg.command = ComAscii.ToString();
                    OriginalMsg.MsgKind = 4;
                    return OriginalMsg;                    

                }

                // ACUpolling
                if (data[5] == myConstants.EOT && data[data.Length - 1] == myConstants.ENQ)
                {
                    ComAscii.Append((char)Convert.ToInt32(data[myConstants.ComAcuSndPosi], 16));
                    ComAscii.Append((char)Convert.ToInt32(data[myConstants.ComAcuSndPosi + 1], 16));

                    OriginalMsg.command = ComAscii.ToString();
                    OriginalMsg.MsgKind = 5;
                    return OriginalMsg;  

                }

                // ACUselecting
                if (data[5] == myConstants.EOT && data[8] == myConstants.STX)
                {
                    ComAscii.Append((char)Convert.ToInt32(data[myConstants.ComAcuSndPosi + 1], 16));
                    ComAscii.Append((char)Convert.ToInt32(data[myConstants.ComAcuSndPosi + 2], 16));

                    OriginalMsg.command = ComAscii.ToString();
                    OriginalMsg.MsgKind = 5;
                    return OriginalMsg;  
                }

            }
            
            // BAKE RCV
            if (line.Contains(myConstants.RcvMsg))
            {
                // Selecting   
                if (data[7] == myConstants.ACK && data.Length == 8)
                {
                    OriginalMsg.command = "";
                    OriginalMsg.MsgKind = 30;
                    return OriginalMsg;
                }
                // Polling
                if (data[7] == myConstants.STX && data[8].CompareTo("40") > 0)
                {
                    ComAscii.Append((char)Convert.ToInt32(data[myConstants.ComPollingRcvPosi], 16));
                    ComAscii.Append((char)Convert.ToInt32(data[myConstants.ComPollingRcvPosi + 1], 16));

                    OriginalMsg.command = ComAscii.ToString();
                    OriginalMsg.MsgKind = 20;
                    return OriginalMsg;
                }

                // CPwriting
                if (data[9] == myConstants.ACK && data.Length == 14)
                {
                    OriginalMsg.command = "";
                    OriginalMsg.MsgKind = 10;
                    return OriginalMsg;                     
                }
                // CPreading
                if (data.Length > 15 && data[8].CompareTo("40") < 0)
                {
                    ComAscii.Append((char)Convert.ToInt32(data[myConstants.ComCpRcvPosi], 16));
                    ComAscii.Append((char)Convert.ToInt32(data[myConstants.ComCpRcvPosi + 1], 16));

                    OriginalMsg.command = ComAscii.ToString();
                    OriginalMsg.MsgKind = 11;
                    return OriginalMsg; 
                }



            }            
                        
            // SPIN RCV
            if (line.Contains(myConstants.SpinRcvmsg))
            {
                // ETU
                if(data[6] == myConstants.EtuMark && data[data.Length - 3] == myConstants.ETX)
                {
                    // reading
                    if (data.Length > 15)
                    {
                        ComAscii.Append((char)Convert.ToInt32(data[myConstants.ComEtuPosi], 16));
                        ComAscii.Append((char)Convert.ToInt32(data[myConstants.ComEtuPosi + 1], 16));

                        OriginalMsg.command = ComAscii.ToString();
                        OriginalMsg.MsgKind = 40;
                        return OriginalMsg;
                    }
                    // writing
                    else
                    {
                        OriginalMsg.command = "";
                        OriginalMsg.MsgKind = 41;
                        return OriginalMsg;

                    }
                }
                // ACUpolling
                if(data[data.Length - 2] == myConstants.ETX)
                {
                    ComAscii.Append((char)Convert.ToInt32(data[myConstants.ComAcuRcvPosi], 16));
                    ComAscii.Append((char)Convert.ToInt32(data[myConstants.ComAcuRcvPosi + 1], 16));

                    OriginalMsg.command = ComAscii.ToString();
                    OriginalMsg.MsgKind = 50;
                    return OriginalMsg;  
                }
                // ACUselecting
                if (data[4] == myConstants.ACK && data.Length == 5)
                {
                    OriginalMsg.command = "";
                    OriginalMsg.MsgKind = 31;
                    return OriginalMsg;           
                }

            }

            // ﾊﾟﾙｽﾓｰﾀｰﾄﾞﾗｲﾊﾞ
            // どんなんか人見さんに確認する
            return OriginalMsg;
        }

        /// <summary>
        /// 定数ｸﾗｽ
        /// </summary>
        abstract class myConstants
        {  
            public const string PollingMark = "4B";
            public const string SndMsg = "[U0C P1]SND";
            public const string RcvMsg = "RCV U0C P1 ->";
            public const string ErrorMsg = "      未対応のﾒｯｾｰｼﾞです";
            public const string ErrorNodeNum = "!!ﾉｰﾄﾞ番号が不明です!!";          

            public const string FileEncording = "shift_jis";

            public const int ComPollingSndPosi = 10;
            public const int ComPollingRcvPosi = 8;

            public const int ComSelectingPosi = 9;

            public const int ComCpSndPosi = 7;
            public const int ComCpRcvPosi = 9;

            public const string LogFileName = "SIO.";
            public const string OutFileName = @"\SIO.txt";

            public const string STX = "02";            
            public const string ETX = "03";
            public const string EOT = "04";
            public const string ENQ = "05";
            public const string ACK = "06";

            public const string HeaderHP = "      HP:N";
            public const string HeaderCP = "      CP:N";
            public const string HeaderCh = "      CH";
            public const string HeaderEtu = "      ETU:N";
            public const string HeaderAcu = "      ACU:N";

            public const string SpinSndMsg = "[U0A P2]SND";
            public const string EtuMark = "45";
            public const string SpinRcvmsg = "[U0A P2]RCV";

            public const int ComEtuPosi = 11;
            
            public const int ComAcuSndPosi = 8;
            public const int ComAcuRcvPosi = 6;            
            
        }       
 

 

    }
}

// EOF