using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CommonClassLibrary;
using KssClassLibrary;
using WeifenLuo.WinFormsUI.Docking;

namespace LocalSimulator.MainProgram
{
    public partial class MainForm : Form
    {
        public static readonly MainForm Instance = new MainForm();

        // beforefieldinit属性を付加しない為に必要
        static MainForm()
        {
        }

        /// <summary>
        /// プログラム状態
        /// </summary>
        enum ProgramStatus
        {
            Unloaded,           // 
            Online,
            Offline,
        }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        private MainForm()
        {
            InitializeComponent();

            // グローバルオブジェクトの初期化
            InitializeGlobal();

            this.Status = ProgramStatus.Unloaded;
            this.ZoomCombo.SelectedItem = "100%";

            Application.Idle += new EventHandler(Application_Idle);
        }


        private double _Zoom = 1;
        public double Zoom
        {
            get { return _Zoom; }
            set { _Zoom = value; }
        }

        ProgramStatus _Status;
        private ProgramStatus Status
        {
            get
            {
                return _Status;
            }
            set
            {
                _Status = value;

                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(ChangedStatus));
                }
                else
                {
                    ChangedStatus();
                }
            }
        }


        private void Application_Idle(object sender, EventArgs e)
        {
            Application.Idle -= new EventHandler(Application_Idle);

            // コマンドライン入力チェック
            CheckCommandLine();
        }

        #region MainFormイベント

        /// <summary>
        /// MainForm.Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            Global.LogManager.Write("MaiProgram起動成功");

            //レイアウト復元
            SubFormManager.Instance.LoadLayout(this.dockPanel1);

            //共通クラスアクセス開始
            RemotingServer.Start();
        }

        /// <summary>
        /// MainForm.FormClosing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                if (this.Status == ProgramStatus.Online)
                {
                    EndConnection();
                }

                if (ProjectManager.Loaded)
                {
                    SaveProject();
                }

                SubFormManager.Instance.SaveLayout(this.dockPanel1);
            }

            Global.LogManager.Write("MainProgram終了");
        }

        #endregion

        #region BaseFormListイベント

        public void BaseFormList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListView listView = (ListView)sender;

            int selectedItemCount = listView.SelectedItems.Count;
            if (selectedItemCount == 0) { return; }

            string baseFormName = listView.SelectedItems[0].Text;

            foreach (Form childForm in ProjectManager.BaseFormList)
            {
                if (childForm.Text == baseFormName)
                {
                    if (!childForm.Visible)
                    {
                        childForm.Show();
                    }
                    else
                    {
                        if (!childForm.Enabled)
                        {
                            childForm.Enabled = true;
                            childForm.Activate();
                            childForm.Enabled = false;
                        }
                        else
                        {
                            if (childForm.WindowState == FormWindowState.Minimized)
                            {
                                childForm.WindowState = FormWindowState.Normal;
                            }
                            childForm.Activate();
                        }
                    }
                }
            }
        }

        #endregion

        #region ToolStripMenuイベント

        private void WindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //string str = sender.ToString();

            //if (str == "重ねて表示")
            //{
            //    //重ねて表示
            //    this.LayoutMdi(MdiLayout.Cascade);
            //}
            //else if (str == "上下に並べて表示")
            //{
            //    //上下に並べて表示
            //    this.LayoutMdi(MdiLayout.TileHorizontal);
            //}
            //else if (str == "左右に並べて表示")
            //{
            //    //左右に並べて表示
            //    this.LayoutMdi(MdiLayout.TileVertical);
            //}
            //else if (str == "アイコンの整列")
            //{
            //    //アイコンの整列
            //    this.LayoutMdi(MdiLayout.ArrangeIcons);
            //}
        }

        /// <summary>
        /// ツールメニュー[ﾌﾟﾛｼﾞｪｸﾄ読込] クリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripLoadProject_Click(object sender, EventArgs e)
        {
            string openProject = string.Empty;

            // 「ファイルを開く」ダイアログ表示
            if (ShowOpenFileDialog(ref openProject))
            {
                LoadProject(openProject);
            }
        }

        /// <summary>
        /// ツールメニュー[I/O Monitor表示] クリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripShowIOMonitor_Click(object sender, EventArgs e)
        {
            UseIOToolStripMenuItem_Click(null, EventArgs.Empty);
        }

        /// <summary>
        /// ツールメニュー[接続開始] クリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripStartConnection_Click(object sender, EventArgs e)
        {
            StartConnection();
        }

        /// <summary>
        /// ツールメニュー[接続切断] クリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripEndConnection_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("切断しますか？", "確認", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                EndConnection();
            }
        }

        private void SelectBaseFormToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DockContent targetForm = SubFormManager.Instance.BaseFormListForm;

            if (!targetForm.Visible)
            {
                targetForm.Show(this.dockPanel1);
            }
        }

        private void UseIOToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DockContent targetForm = SubFormManager.Instance.IOMonitorForm;

            // フォームが表示されていない
            if (!targetForm.Visible)
            {
                targetForm.Show(this.dockPanel1);
            }
        }

        private void btnEditActiveProject_Click(object sender, EventArgs e)
        {
            OpenEditor(ProjectManager.ProjectFilePath);
        }

        private void btnReloadActiveProject_Click(object sender, EventArgs e)
        {
            LoadProject(ProjectManager.ProjectFilePath);
        }

        private void ZoomCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Zoom = Convert.ToDouble(this.ZoomCombo.Text.Substring(0, this.ZoomCombo.Text.Length - 1)) / 100;

            ZoomUpdate();
        }

        #endregion

        #region PlcCommunicationイベント

        private void PlcCommunication_CommunicationError(object sender, PlcErrorEventArgs e)
        {
            string errorMsg = string.Empty;
            errorMsg += "通信中にエラーが発生したため、通信を停止します。\n";
            errorMsg += "\n";
            errorMsg += "エラー内容：\n";
            errorMsg += e.Message;
            MessageBox.Show(errorMsg, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);

            // 通信エラー時、別スレッドから呼ばれる
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(EndConnection));
            }
            else
            {
                EndConnection();
            }
        }

        #endregion


        private void CheckCommandLine()
        {
            string[] commandLines = System.Environment.GetCommandLineArgs();

            if (commandLines.Length > 1)
            {
                // [0]  =  自身の実行ファイルへのパス
                // [1] ～  ユーザーからの入力

                LoadProject(commandLines[1]);
            }
        }

        private void ChangedStatus()
        {
            ChangeFormTitle();

            ChangeBaseFormEnable();

            switch (this.Status)
            {
                case ProgramStatus.Unloaded:

                    this.toolStripShowIOMonitor.Enabled = true;
                    this.toolStripStartConnection.Enabled = false;
                    this.toolStripEndConnection.Enabled = false;
                    break;

                case ProgramStatus.Online:

                    this.toolStripShowIOMonitor.Enabled = true;
                    this.toolStripStartConnection.Enabled = false;
                    this.toolStripEndConnection.Enabled = true;
                    break;

                case ProgramStatus.Offline:

                    this.toolStripStartConnection.Enabled = true;
                    this.toolStripEndConnection.Enabled = false;
                    this.toolStripShowIOMonitor.Enabled = true;
                    break;
            }

        }

        private void ChangeFormTitle()
        {
            if (this.Status == ProgramStatus.Online)
            {
                this.Text = "PLC装置シミュレータ [オンライン] " + Global.VerInfo();
            }
            else
            {
                this.Text = "PLC装置シミュレータ [オフライン] " + Global.VerInfo();
            }
        }

        private void ChangeBaseFormEnable()
        {
            if (ProjectManager.Loaded)
            {
                bool status = (this.Status == ProgramStatus.Online);

                foreach (IBaseForm frm in ProjectManager.BaseFormList)
                {
                    foreach (Control bfCtrl in ((Form)frm).Controls)
                    {
                        bfCtrl.Enabled = status;
                    }
                }
            }
        }

        private void SymbolInitialize()
        {
            ProgressDialog pd1 = new ProgressDialog();
            //ダイアログのタイトルを設定
            pd1.Title = "進捗状況";
            //プログレスバーの最小値を設定
            pd1.Minimum = 0;
            //プログレスバーの最大値を設定
            pd1.Maximum = ProjectManager.BaseFormList.Count;
            //プログレスバーの初期値を設定
            pd1.Value = 0;

            pd1.SecondForm = false;

            pd1.Show(this);

            // Initial中のDataListMakeを禁止
            Global.LockDataListMake = true;

            string format = "フォーム {0:D3} / {1:D3} 初期処理実行中...";
            int formCounter = 1;

            foreach (BaseForm baseForm in ProjectManager.BaseFormList)
            {
                //メッセージを変更する
                pd1.Message = string.Format(format, formCounter, ProjectManager.BaseFormList.Count);

                foreach (Symbol_Draw symbol in baseForm.Controls.OfType<Symbol_Draw>())
                {
                    SymbolCyclicThread.Registration(symbol);
                    symbol.Initial();
                }

                pd1.Value = pd1.Value + 1;
                formCounter = formCounter + 1;
            }
            pd1.Close();

            Global.LockDataListMake = false;
            Global.DataListMake();
        }

        /// <summary>
        /// プロジェクトファイルを開くダイアログを表示します。
        /// </summary>
        private bool ShowOpenFileDialog(ref string filePath)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            if (ProjectManager.Loaded)
            {
                // プロジェクトが読み込まれていれば、
                // 読み込まれたプロジェクトのファイルをデフォルト表示する

                string projectPath = ProjectManager.ProjectFilePath;

                ofd.InitialDirectory = Path.GetDirectoryName(projectPath);
                ofd.FileName = Path.GetFileName(projectPath);
            }
            else
            {
                // それ以外の場合、
                // 実行ファイルと同じフォルダにあるlcsファイルをデフォルト表示する

                ofd.InitialDirectory = Directory.GetCurrentDirectory();
                ofd.FileName = Directory.GetFiles(ofd.InitialDirectory, "*.lcs")
                                            .FirstOrDefault() ?? "default.lcs";
            }
            ofd.Filter = "LCSファイル(*.lcs)|*.lcs";
            ofd.Title = "開くファイルを選択してください";
            ofd.RestoreDirectory = true;
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                filePath = ofd.FileName;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// プロジェクトをクローズします。
        /// </summary>
        private void CloseProject()
        {
            if (ProjectManager.Loaded)
            {
                SaveProject();
            }

            foreach (BaseForm frm in ProjectManager.BaseFormList)
            {
                frm.Dispose();
            }

            // BaseForm選択リストをクリア
            SubFormManager.Instance.BaseFormListForm.ClearDisplayBaseForms();

            // I/O Monitorに表示されたシンボルをクリア
            SubFormManager.Instance.IOMonitorForm.ClearMonitorSymbols();

            ProjectManager.Loaded = false;

            if (this.Status == ProgramStatus.Online)
            {
                EndConnection();
            }

            // シンボル定周期処理登録解除
            SymbolCyclicThread.Clear();

            this.Status = ProgramStatus.Unloaded;
        }

        /// <summary>
        /// プロジェクトファイルを読み込みます。
        /// </summary>
        /// <param name="filePath">プロジェクトファイル入力パス</param>
        private void LoadProject(string filePath)
        {
            // 既にプロジェクトが開かれている
            if (ProjectManager.Loaded)
            {
                CloseProject();
            }

            dockPanel1.SuspendLayout(true);

            if (!ProjectManager.Load(filePath))
            {
                MessageBox.Show("プロジェクトデータではありません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!ExistsMakerPlc(ProjectManager.MakerName))
            {
                string msg = string.Format("メーカー: \"{0}\" に対応するDLLが存在しません。", ProjectManager.MakerName);
                MessageBox.Show(msg, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            CreatePlcObject(ProjectManager.MakerName);

            this.toolStrip_lblCyclicTime.Text = "初期処理中";

            RestoreBaseForms(ProjectManager.SerializeData);

            RegisterBaseFormList();

            ProjectManager.Loaded = true;

            SubFormManager.Instance.IOMonitorForm.SetMonitorSymbols(
                ProjectManager.MonitorSymbolList);

            //先頭のBaseFormを表示
            if (ProjectManager.BaseFormList.Count > 0)
            {
                BaseForm firstBaseForm = (BaseForm)ProjectManager.BaseFormList.First();

                firstBaseForm.WindowState = FormWindowState.Maximized;
                firstBaseForm.Show();
            }

            this.toolStrip_lblCyclicTime.Text = "";

            //DB作成
            Global.FormList = ProjectManager.BaseFormList;

            Global.DataListMake();

            this.Status = ProgramStatus.Offline;

            this.dockPanel1.ResumeLayout(true, true);
        }

        private void SaveProject()
        {
            IEnumerable<IOMonitorSymbolFormat> monitorSymbols =
                SubFormManager.Instance.IOMonitorForm.GetMonitorSymbols();

            ProjectManager.MonitorSymbolList = new List<IOMonitorSymbolFormat>(monitorSymbols);

            ProjectManager.Save();
        }

        private bool ExistsMakerPlc(string makerName)
        {
            string[] existsMakers = LoadAssembly.GetMakerNames(Application.StartupPath);

            if(existsMakers.Contains(makerName))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void RestoreBaseForms(ProjectDataSerializeFormat projectData)
        {
            ProgressDialog pd = new ProgressDialog();
            pd.Title = "進歩状況";                          // タイトル
            pd.Minimum = 0;                                 // プログレスバー最小値
            pd.Maximum = projectData.BaseFormData.Count;    // プログレスバー最大値
            pd.Value = 0;                                   // プログレスバー初期値
            pd.SecondForm = false;
            pd.Show(this);

            this.SuspendLayout();

            string format = "フォーム {0:D3} / {1:D3} 読込中...";
            int formCounter = 1;

            // ベースフォームデータ復元
            foreach (BaseFormDataSerializeFormat frmData in projectData.BaseFormData)
            {
                // メッセージを変更する
                pd.Message = string.Format(format, formCounter, projectData.BaseFormData.Count);

                BaseForm myForm = DeserializeBaseForm(frmData);

                ProjectManager.BaseFormList.Add(myForm);

                // プログレスバーを進める
                pd.Value += 1;
                formCounter += 1;
            }

            // 倍率を反映させる
            this.ZoomUpdate();

            this.ResumeLayout();

            pd.Close();
        }

        private BaseForm DeserializeBaseForm(BaseFormDataSerializeFormat frmData)
        {
            // ベースフォームデシリアライズ
            BaseForm myForm = (BaseForm)SerializeSupport.Deserialize(frmData, typeof(BaseForm));
            myForm.Deserizlie(frmData);

            FixedFormSize(myForm);

            myForm.MdiParent = MainForm.Instance;
            return myForm;
        }

        private void RegisterBaseFormList()
        {
            var baseFormNames =
                from baseForm in ProjectManager.BaseFormList
                select baseForm.Text;

            SubFormManager.Instance.BaseFormListForm.SetDisplayBaseForms(baseFormNames);
        }

        private void CreatePlcObject(string makerName)
        {
            // メーカー固有オブジェクト生成
            Global.DeviceManager    = LoadAssembly.LoadDeviceManager(Application.StartupPath, makerName);       // デバイス管理オブジェクト
            Global.PlcCommunication = LoadAssembly.LoadPlcCommunication(Application.StartupPath, makerName);    // PLC通信オブジェクト
            Global.PlcSetting       = LoadAssembly.LoadPlcSetting(Application.StartupPath, makerName);          // PLC通信オブジェクト

            // PLC通信クラスイベント
            Remoting.Instance.CommunicationError += new PlcErrorEventHandler(PlcCommunication_CommunicationError);
        }

        private void FixedFormSize(BaseForm myForm)
        {
            #region 画面右下に透明シンボルを配置（ProjectMakerで指定した画面サイズで表示させるため）

            Label formAdjuster = new Label();
            formAdjuster.Size = new Size(1, 1);
            formAdjuster.BackColor = Color.Transparent;     // 背景色を透明に設定

            Point adjusterLocation = Point.Empty;
            adjusterLocation.X = (myForm.Size.Width - 1);
            adjusterLocation.Y = (myForm.Size.Height - 1);

            formAdjuster.Location = adjusterLocation;       // 画面右下表示

            myForm.Controls.Add(formAdjuster);

            #endregion
        }

        private void ZoomUpdate()
        {
            if (!ProjectManager.Loaded) { return; }

            foreach (BaseForm baseForm in ProjectManager.BaseFormList)
            {
                foreach (Symbol_Draw symbol in baseForm.Controls.OfType<Symbol_Draw>())
                {
                    symbol.Zoom = _Zoom;
                }

                baseForm.Size = new Size((int)(baseForm.BaseFormSize.Width * _Zoom), (int)(baseForm.BaseFormSize.Height * _Zoom));
                baseForm.Refresh();
            }
        }

        private void OpenEditor(string projectPath)
        {
            ProjectEditor editor = new ProjectEditor();

            if (!string.IsNullOrEmpty(projectPath))
            {
                editor.Open(projectPath);
            }
            else
            {
                editor.Open();
            }
        }

        /// <summary>
        /// Global初期化
        /// </summary>
        private void InitializeGlobal()
        {
            Global.MainForm = this;

            Global.LogManager = LogManager.CreateInstance
                (this, AppSetting.LogPath + Path.DirectorySeparatorChar + "MainProgram.log", true);
        }

        /// <summary>
        /// 接続開始処理
        /// </summary>
        private void StartConnection()
        {
            if (this.Status == ProgramStatus.Offline)
            {
                //接続開始
                if (!CommunicationControl.Start()) { return; }

                //シンボル初期処理
                Global.LogManager.Write("シンボル初期処理開始");
                SymbolInitialize();

                //デバイスデータ状態変化ポスティング開始
                Global.LogManager.Write("デバイスイベントポスティング開始");
                DeviceEvent.Start();

                //サイクリックスレッド開始
                SymbolCyclicThread.Start();

                this.Status = ProgramStatus.Online;
            }
        }

        /// <summary>
        /// 接続終了処理
        /// </summary>
        private void EndConnection()
        {
            if (this.Status == ProgramStatus.Online)
            {
                //シンボル定周期処理停止
                SymbolCyclicThread.End();
                //SymbolAbort();
                foreach (BaseForm baseForm in ProjectManager.BaseFormList)
                {
                    foreach (Symbol_Draw symbol in baseForm.Controls.OfType<Symbol_Draw>())
                    {
                        symbol.BaseAbort();
                    }
                }

                //デバイスイベント停止
                DeviceEvent.End();

                //PLC通信処理停止
                CommunicationControl.End();

                if (ProjectManager.Loaded)
                {
                    this.Status = ProgramStatus.Offline;
                }
                else
                {
                    this.Status = ProgramStatus.Unloaded;
                }
            }
        }


        private class ProjectEditor
        {
            public ProjectEditor()
            {
                AbsolutePathGenerator generator = new AbsolutePathGenerator(Application.StartupPath);
                this.editorPath = generator.ToAbsolutePath("projectmaker.exe");

                this.editor = null;
            }

            string editorPath;
            Process editor;

            public void Open()
            {
                this.editor = Process.Start(this.editorPath);
            }

            public void Open(string projectPath)
            {
                this.editor = Process.Start(this.editorPath, projectPath);
            }
        }
    }
}
