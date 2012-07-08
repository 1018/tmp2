using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;
using CommonClassLibrary;
using WeifenLuo.WinFormsUI.Docking;

namespace LocalSimulator.ProjectMaker
{
    public partial class MainForm : Form
    {
        private static MainForm _Instance = null;
        public static MainForm Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new MainForm();
                }
                return _Instance;
            }
        }

        public SymbolListForm SymbolListForm = null;

        public ListView SymbolListView = null;

        public CustomPropertyGrid PropertyView = null;

        public ProjectForm ProjectForm = null;

        public PropertyForm PropertyForm = null;

        public ProjectDataFormat ProjectData = null;

        public SystemSettingForm SystemSettingForm = null;

        public BaseFormDataSerializeFormat ClipBoardBaseForm = null;

        readonly string SAVE_LAYOUT_PATH = Application.StartupPath + @"\FormLayout_ProjectMaker.xml";

        public Panel nonPanel;

        public bool updated = false;

        private MainForm()
        {
            InitializeComponent();

            Global.LogManager = KssClassLibrary.LogManager.CreateInstance
                (this, AppSetting.LogPath + Path.DirectorySeparatorChar + "ProjectMaker.log", true);

            string[] makerNames = LoadAssembly.GetMakerNames(Application.StartupPath);

            if (makerNames.Length == 0)
            {
                MessageBox.Show("通信用DLLが存在しません。プログラムを終了します。"
                                 , "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);

                Environment.Exit(0);
            }


            //#region フォームレイアウト
            //if (System.IO.File.Exists(SAVE_LAYOUT_PATH))
            //{
            //    LoadDockLayout();

            //    //SymbolListForm作成
            //    SymbolListView = SymbolListForm.SymbolListView;
            //    SymbolListForm.CloseButtonVisible = false;

            //    //ProjectForm作成
            //    ProjectForm.CloseButtonVisible = false;

            //    //PropertyForm作成
            //    PropertyView = PropertyForm.PropertyView;
            //    PropertyForm.CloseButtonVisible = false;

            //    //DockContentのActivateを呼び出す
            //    //(起動時、[Close]ボタンが表示されることへの対策)
            //    foreach (DockContent content in this.dockPanel1.Contents)
            //    {
            //        content.Activate();
            //    }
            //}
            //else
            //{
            //    // XMLがなければデフォルト   

            //    //SymbolListForm作成
            //    SymbolListForm = new SymbolListForm();
            //    SymbolListView = SymbolListForm.SymbolListView;
            //    SymbolListForm.CloseButtonVisible = false;
            //    SymbolListForm.Show(dockPanel1, DockState.DockRightAutoHide);

            //    //ProjectForm作成
            //    ProjectForm = new ProjectForm();
            //    ProjectForm.CloseButtonVisible = false;
            //    ProjectForm.Show(dockPanel1, DockState.DockLeft);

            //    //PropertyForm作成
            //    PropertyForm = new PropertyForm();
            //    PropertyView = PropertyForm.PropertyView;
            //    PropertyForm.CloseButtonVisible = false;
            //    PropertyForm.Show(ProjectForm.DockPanel, DockState.DockBottom);
            //    PropertyForm.DockTo(ProjectForm.Pane, DockStyle.Bottom, 0xffff);

            //}

            //#endregion

            Global.DesignMode = true;

            //this.WindowState = System.Windows.Forms.FormWindowState.Maximized;

            //this.ZoomCombo.SelectedItem = "100%";

            ////シンボル描画用パネル設置
            //nonPanel = new Panel();
            //nonPanel.Size = new Size(1, 1);
            //this.Controls.Add(nonPanel);

            //PropertyView.PropertySort = PropertySort.Categorized;

            //Application.Idle += new EventHandler(Application_Idle);

            //this.Text = this.Text + " " + Global.VerInfo();
        }


        #region メインフォームイベント

        private void Application_Idle(object sender, EventArgs e)
        {
            Application.Idle -= new EventHandler(Application_Idle);

            SymbolListView.Invoke((MethodInvoker)delegate()
            {
                SymbolListForm.CreateThumbnailList();
            });

            string[] cmds = System.Environment.GetCommandLineArgs();
            if (cmds.Count() != 1)
            {
                LoadFromFile(cmds[1]);
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            #region フォームレイアウト
            if (System.IO.File.Exists(SAVE_LAYOUT_PATH))
            {
                LoadDockLayout();

                //SymbolListForm作成
                SymbolListView = SymbolListForm.SymbolListView;
                SymbolListForm.CloseButtonVisible = false;

                //ProjectForm作成
                ProjectForm.CloseButtonVisible = false;

                //PropertyForm作成
                PropertyView = PropertyForm.PropertyView;
                PropertyForm.CloseButtonVisible = false;

                //DockContentのActivateを呼び出す
                //(起動時、[Close]ボタンが表示されることへの対策)
                foreach (DockContent content in this.dockPanel1.Contents)
                {
                    content.Activate();
                }
            }
            else
            {
                // XMLがなければデフォルト   

                //SymbolListForm作成
                SymbolListForm = new SymbolListForm();
                SymbolListView = SymbolListForm.SymbolListView;
                SymbolListForm.CloseButtonVisible = false;
                SymbolListForm.Show(dockPanel1, DockState.DockRightAutoHide);

                //ProjectForm作成
                ProjectForm = new ProjectForm();
                ProjectForm.CloseButtonVisible = false;
                ProjectForm.Show(dockPanel1, DockState.DockLeft);

                //PropertyForm作成
                PropertyForm = new PropertyForm();
                PropertyView = PropertyForm.PropertyView;
                PropertyForm.CloseButtonVisible = false;
                PropertyForm.Show(ProjectForm.DockPanel, DockState.DockBottom);
                PropertyForm.DockTo(ProjectForm.Pane, DockStyle.Bottom, 0xffff);

            }

            #endregion

            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;

            this.ZoomCombo.SelectedItem = "100%";

            //シンボル描画用パネル設置
            nonPanel = new Panel();
            nonPanel.Size = new Size(1, 1);
            this.Controls.Add(nonPanel);

            PropertyView.PropertySort = PropertySort.Categorized;

            Application.Idle += new EventHandler(Application_Idle);

            this.Text = this.Text + " " + Global.VerInfo();

            Global.LogManager.Write("MainForm起動完了");

        }

        private void MainForm_MdiChildActivate(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild == null) { return; }

            if (this.ActiveMdiChild.GetType() == typeof(BaseForm))
            {

                BaseForm frm = (BaseForm)this.ActiveMdiChild;

                MainCtrl.PropertyView_Set(frm);
                if (frm != null)
                {
                    this.ZoomCombo.Enabled = true;

                    string zoomString = ((int)(frm.Zoom * 100)).ToString() + "%";

                    for (int num = 0; num < this.ZoomCombo.Items.Count; num++)
                    {
                        if (zoomString == this.ZoomCombo.Items[num].ToString())
                        {
                            this.ZoomCombo.SelectedIndex = num;
                        }
                    }

                }
                else
                {
                    this.ZoomCombo.Enabled = false;
                }
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                // 保存が完了していない時、確認メッセージ表示
                if (this.updated)
                {
                    DialogResult result =
                        MessageBox.Show("作成した画面は保存されていません。\n変更を保存しますか？", "確認",
                        MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);

                    if (result == DialogResult.Yes)
                    {
                        // [上書き保存]を押した時と同じ処理
                        OverWriteToolStripMenuItem_Click(null, EventArgs.Empty);
                    }
                    else if (result == DialogResult.No)
                    {
                        // 保存せずに終了
                    }
                    else
                    {
                        // キャンセル
                        e.Cancel = true;
                    }
                }
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            SaveDockLayout();
        }

        #endregion

        #region メインメニュー

        // [ファイル]→[新規作成]
        private void ProjectNewCreateMenuItem_Click(object sender, EventArgs e)
        {
            //既にコンテンツデータがある場合は、保存するか確認する
            if (ProjectData != null)
            {
                DialogResult yn = MessageBox.Show("プロジェクトデータ'" + ProjectData.ProjectName + "'を保存しますか？", "保存確認", MessageBoxButtons.YesNo);

                if (yn == DialogResult.Yes)
                {
                    //保存処理
                    SaveToolStripMenuItem_Click(null, null);
                    return;

                }
                else
                {
                    DialogResult yn2 = MessageBox.Show("プロジェクトデータ'" + ProjectData.ProjectName + "'を破棄しますか？", "破棄確認", MessageBoxButtons.YesNo);

                    if (yn2 == DialogResult.Yes)
                    {
                        //破棄処理
                        foreach (Form frm in ProjectData.BaseFormData)
                        {
                            frm.Close();
                        }
                        SystemSettingForm.Close();
                    }
                    else
                    {
                        return;
                    }

                }

            }

            //プロジェクトデータ新規作成
            this.ProjectData = new ProjectDataFormat();
            this.ProjectData.ProjectName = "新規プロジェクト";
            this.ProjectData.MakerName = LoadAssembly.GetMakerNames(Application.StartupPath).First();

            //SystemSettingForm作成
            SystemSettingForm = new SystemSettingForm();
            SystemSettingForm.MdiParent = this;
            //SystemSettingForm.Show();
            SystemSettingForm.Hide();


            FormNewCreate();
        }
        // [ファイル]→[フォーム追加]
        private void FormNewToolStrip_Click(object sender, EventArgs e)
        {
            //プロジェクトデータがあるか確認する
            if (ProjectData != null)
            {
                FormNewCreate();
            }
        }
        // [ファイル]→[開く]
        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ProjectData != null)
            {
                DialogResult yn = MessageBox.Show("現在のプロジェクトは破棄されます。",
                                                   "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

                if (yn == DialogResult.Cancel) { return; }

            }

            string fileName = null;

            if (GetOpenFileName(ref fileName, "lcs") == false) { return; }

            if (ProjectData != null)
            {
                //破棄処理
                foreach (Form frm in ProjectData.BaseFormData)
                {
                    frm.Close();
                }
                SystemSettingForm.Close();
            }

            LoadFromFile(fileName);

        }
        // [ファイル]→[上書き保存]
        private void OverWriteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // プロジェクトが開かれていない(作成されていない)
            if (this.ProjectData == null) { return; }

            // 新規作成
            if (!this.ProjectData.FileCreated)
            {
                SaveToolStripMenuItem_Click(null, null);
                return;
            }
            // 上書き保存
            else
            {
                SaveToFile(this.ProjectData.FilePath, true);
                MessageBox.Show("保存完了");
            }
        }
        // [ファイル]→[名前を付けて保存]
        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ProjectData == null) { return; }

            string filePath = null;

            if (GetSaveFileName(ref filePath, ProjectData.ProjectName, "lcs") == false) { return; }

            SaveToFile(filePath, false);

            MessageBox.Show("保存完了");

        }
        // [ファイル]→[フォームインポート]→[プロジェクトファイルからインポート]
        private void ImportFromProject_Click(object sender, EventArgs e)
        {
            if (ProjectData == null) { return; }

            string fileName = null;
            if (GetOpenFileName(ref fileName, "lcs", "xml") == false) { return; }

            //シリアライズ用プロジェクトデータをロードする。
            Type[] et = new Type[] { typeof(string[]) };
            XmlSerializer serializer = new XmlSerializer(typeof(ProjectDataSerializeFormat), et);

            ProjectDataSerializeFormat projectData = new ProjectDataSerializeFormat();

            using (FileStream fs = new FileStream(fileName, FileMode.Open))
            {
                try
                {
                    projectData = (ProjectDataSerializeFormat)serializer.Deserialize(fs);
                }
                catch
                {
                    MessageBox.Show("プロジェクトデータではありません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            ImportProjectForm ipf = new ImportProjectForm(projectData);

            //リスト作成
            foreach (BaseFormDataSerializeFormat frmData in projectData.BaseFormData)
            {
                ipf.listBox1.Items.Add(frmData.Text);
            }

            //選択ダイアログ表示
            ipf.ShowDialog();
        }
        // [ファイル]→[フォームインポート]→[フォームファイルからインポート]
        private void ImportFromForm_Click(object sender, EventArgs e)
        {
            if (ProjectData == null) { return; }

            string fileName = null;
            if (GetOpenFileName(ref fileName, "xml") == false) { return; }

            //シリアライズ用プロジェクトデータをロードする。
            Type[] et = new Type[] { typeof(string[]) };
            XmlSerializer serializer = new XmlSerializer(typeof(BaseFormDataSerializeFormat), et);

            BaseFormDataSerializeFormat loadData = new BaseFormDataSerializeFormat();

            using (FileStream fs = new FileStream(fileName, FileMode.Open))
            {
                try
                {
                    loadData = (BaseFormDataSerializeFormat)serializer.Deserialize(fs);
                }
                catch
                {
                    MessageBox.Show("ベースフォームデータではありません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            // ベースフォームデシリアライズ
            BaseForm myForm = (BaseForm)SerializeSupport.Deserialize(loadData, typeof(BaseForm));

            // 空きナンバーを検索する
            myForm.Number = GetOpeningFormNumber(this.ProjectData.BaseFormData, myForm.Number);

            myForm.Deserialize(loadData);
            myForm.Hide();
            myForm.MdiParent = MainForm.Instance;

            // プロジェクトに追加後、ベースフォーム番号順にソートする
            this.ProjectData.BaseFormData.Add(myForm);
            this.ProjectData.BaseFormData.Sort(
                (frm1, frm2) => Comparer<int>.Default.Compare(frm1.Number, frm2.Number));

            ProjectForm.Redraw();

            myForm.Show();

        }
        // [ファイル]→[フォームインポート]→[旧形式からインポート]
        private void ImportFromOld_Click(object sender, EventArgs e)
        {
            if (ProjectData == null) { return; }

            string fileName = null;
            if (GetOpenFileName(ref fileName, "xml") == false) { return; }

            //XmlSerializerオブジェクトを作成            
            //XmlSerializer serializer = new XmlSerializer(typeof(BaseFormDataFormat));
            Type[] et = new Type[] { typeof(string[]) };

            XmlSerializer serializer = new XmlSerializer(typeof(BaseFormDataFormat), et);

            BaseFormDataFormat oldData;

            using (FileStream fs = new FileStream(fileName, FileMode.Open))
            {
                //XMLファイルから読み込み、逆シリアル化する

                try
                {
                    oldData = (BaseFormDataFormat)serializer.Deserialize(fs);
                }
                catch
                {
                    MessageBox.Show("旧データではありません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            #region 現状のデータにコンバートする。
            BaseFormDataSerializeFormat convData = new BaseFormDataSerializeFormat();
            convData.BackColor = oldData.FormData.BackColor;
            convData.Font = oldData.FormData.Font;
            convData.Number = oldData.FormData.Number;
            convData.TitleName = oldData.FormData.TitleName;
            convData.Text = oldData.FormData.Text;
            convData.Size = oldData.FormData.Size;

            convData.SymbolData = new List<SymbolDataSerializeFormat>();

            for (int i = oldData.SymbolData.Count() - 1; i >= 0; i--)
            {
                SymbolDataFormat oldSymbolData = oldData.SymbolData[i];
                SymbolDataSerializeFormat newSymbolData = new SymbolDataSerializeFormat();
                newSymbolData.SymbolName = oldSymbolData.SymbolName;
                newSymbolData.Type = oldSymbolData.Type;
                newSymbolData.PropertyData = new List<PropertyDataSerializeFormat>();
                foreach (PropertyDataFormat oldPropertyData in oldSymbolData.PropertyData)
                {
                    PropertyDataSerializeFormat newPropertyData = new PropertyDataSerializeFormat();
                    newPropertyData.Name = oldPropertyData.Name;
                    newPropertyData.Value = oldPropertyData.Value;
                    newSymbolData.PropertyData.Add(newPropertyData);
                }
                convData.SymbolData.Add(newSymbolData);
            }

            convData.ShapeData = new List<ShapeDataSerializeFormat>();

            for (int i = oldData.ShapeData.Count() - 1; i >= 0; i--)
            {
                ShapeDataFormat oldShapeData = oldData.ShapeData[i];
                ShapeDataSerializeFormat newShapeData = new ShapeDataSerializeFormat();

                foreach (PropertyDataFormat oldPropertyData in oldShapeData.PropertyData)
                {
                    if (oldPropertyData.Name == "ShapeType")
                    {
                        newShapeData.Type = (string)oldPropertyData.Value;
                        break;
                    }
                }
                newShapeData.PropertyData = new List<PropertyDataSerializeFormat>();
                foreach (PropertyDataFormat oldPropertyData in oldShapeData.PropertyData)
                {
                    PropertyDataSerializeFormat newPropertyData = new PropertyDataSerializeFormat();
                    newPropertyData.Name = oldPropertyData.Name;
                    newPropertyData.Value = oldPropertyData.Value;
                    newShapeData.PropertyData.Add(newPropertyData);
                }
                convData.ShapeData.Add(newShapeData);
            }
            #endregion

            //ベースフォームデシリアライズ
            BaseForm myForm = (BaseForm)SerializeSupport.Deserialize(convData, typeof(BaseForm));

            //空きナンバーを検索する
            myForm.Number = GetOpeningFormNumber(this.ProjectData.BaseFormData, myForm.Number);

            myForm.Deserialize(convData);
            myForm.MdiParent = MainForm.Instance;
            myForm.Show();
            myForm.Hide();
            myForm.Refresh();

            ProjectData.BaseFormData.Add(myForm);

            ProjectForm.Redraw();
        }
        // [ファイル]→[フォームエクスポート]
        private void ExportFormToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProjectForm.ExportMenu_Click(null, null);
        }

        // [編集]→[グリッド間隔設定]
        private void GridSettingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form frm = new GridSettingForm();
            frm.ShowDialog();
        }

        // [表示]→[I/Oリスト]
        private void ShowIOListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IoListForm.Instance.Show();   // IO一覧の表示
        }

        // [ウィンドウ]→[重ねて表示]
        private void WindowCascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.LayoutMdi(MdiLayout.Cascade);
        }
        // [ウィンドウ]→[上下に並べて表示]
        private void WindowHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.LayoutMdi(MdiLayout.TileHorizontal);
        }
        // [ウィンドウ]→[左右に並べて表示]
        private void WindowVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.LayoutMdi(MdiLayout.TileVertical);
        }
        // [ウィンドウ]→[アイコンの整列]
        private void WindowIconToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.LayoutMdi(MdiLayout.ArrangeIcons);
        }

        #endregion

        #region ツールメニュー

        private void UndoToolStripSplitButton_Click(object sender, EventArgs e)
        {
            if (CommandManager.UndoCommandStack.Count != 0)
            {
                ICommand UndoCommand = CommandManager.UndoCommandStack.Pop();
                UndoCommand.Undo();
                CommandManager.RedoCommandStack.Push(UndoCommand);
            }
        }

        private void RedoToolStripSplitButton_Click(object sender, EventArgs e)
        {
            if (CommandManager.RedoCommandStack.Count != 0)
            {
                ICommand RedoCommand = CommandManager.RedoCommandStack.Pop();
                RedoCommand.Redo();
                CommandManager.UndoCommandStack.Push(RedoCommand, false);
            }
        }

        private void ShapeToolStripButton_Click(object sender, EventArgs e)
        {
            ToolStripButton Tsb = (ToolStripButton)sender;

            if (Tsb.Checked == true)
            {
                //自分以外の選択を解除
                this.LineToolStripButton.Checked = false;
                this.LinesToolStripButton.Checked = false;
                this.CircleToolStripButton.Checked = false;
                this.SquareToolStripButton.Checked = false;
                this.PolygonToolStripButton.Checked = false;
                this.TextToolStripButton.Checked = false;
                Tsb.Checked = true;

                //ListViewの選択を解除
                if (SymbolListView.FocusedItem != null)
                {
                    ListViewItem SelectedItem = SymbolListView.Items[SymbolListView.FocusedItem.Index];
                    SelectedItem.Selected = false;
                }
            }

        }

        private void GridToolStripButton_Click(object sender, EventArgs e)
        {
            foreach (Form frm in this.MdiChildren)
            {
                frm.Refresh();
            }
        }

        private void ArrowToolStripButton_Click(object sender, EventArgs e)
        {
            MainCtrl.DrawSelectFalse();
        }

        private void ZoomCombo_Click(object sender, EventArgs e)
        {
            //描画選択解除
            MainCtrl.DrawSelectFalse();
        }
        private void ZoomCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            ToolStripControlHost MyCombo = (ToolStripControlHost)sender;
            BaseForm MyFrm = (BaseForm)this.ActiveMdiChild;
            if (MyFrm == null) { return; }
            MyFrm.Zoom = Convert.ToDouble(MyCombo.Text.Substring(0, MyCombo.Text.Length - 1)) / 100;
        }

        #endregion

        #region イベント処理

        private void SymbolView_List_Click(object sender, EventArgs e)
        {
            this.LineToolStripButton.Checked = false;
            this.LinesToolStripButton.Checked = false;
            this.CircleToolStripButton.Checked = false;
            this.SquareToolStripButton.Checked = false;
            this.PolygonToolStripButton.Checked = false;
            this.TextToolStripButton.Checked = false;
        }

        private void SymbolView_List_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            #region Escキー押下時
            if (e.KeyCode == Keys.Escape)
            {
                //ShapeToolStrip選択を解除
                LineToolStripButton.Checked = false;
                LinesToolStripButton.Checked = false;
                CircleToolStripButton.Checked = false;
                SquareToolStripButton.Checked = false;
                PolygonToolStripButton.Checked = false;
                TextToolStripButton.Checked = false;

                //ListViewの選択を解除
                ListViewItem SelectedItem = SymbolListView.Items[SymbolListView.FocusedItem.Index];
                SelectedItem.Selected = false;

                Cursor.Current = Cursors.Default;

            }
            #endregion

        }

        private void PropertyGrid_Main_Enter(object sender, EventArgs e)
        {
            //描画選択解除
            MainCtrl.DrawSelectFalse();
        }

        #endregion


        /// <summary>
        /// レイアウトをXMLファイルに保存
        /// </summary>
        /// <remarks></remarks>
        private void SaveDockLayout()
        {
            //XMLにレイアウト情報を書き出す
            this.dockPanel1.SaveAsXml(SAVE_LAYOUT_PATH);
        }

        /// <summary>
        /// XMLファイルからレイアウトを復元
        /// </summary>
        /// <remarks></remarks>
        private void LoadDockLayout()
        {
            //デリゲート生成
            DeserializeDockContent deserializeDockContent
                = new DeserializeDockContent(GetDockContentFromPersistString);

            this.dockPanel1.LoadFromXml(SAVE_LAYOUT_PATH, deserializeDockContent);
        }

        /// <summary>
        /// PersistStringからDockContentを返却
        /// </summary>
        /// <param name="persistString">DockContentの名前</param>
        /// <returns></returns>
        private IDockContent GetDockContentFromPersistString(string persistString)
        {

            Global.LogManager.Write("フォーム復元：" + persistString);
            //persistStringに応じて、対応するDockContentを生成し、それを返却
            if (persistString.Equals(typeof(SymbolListForm).ToString()))
            {
                SymbolListForm = new SymbolListForm();
                return SymbolListForm;
            }
            else if (persistString.Equals(typeof(ProjectForm).ToString()))
            {
                ProjectForm = new ProjectForm();
                return ProjectForm;
            }
            else if (persistString.Equals(typeof(PropertyForm).ToString()))
            {
                PropertyForm = new PropertyForm();
                return PropertyForm;
            }

            return null;
        }

        /// <summary>
        /// 現在開かれているプロジェクトをファイルに保存します。
        /// </summary>
        /// <param name="filePath">書き込み先ファイルのパス</param>
        /// <param name="addFlag"></param>
        public void SaveToFile(string filePath, bool addFlag)
        {
            //シリアライズ用クラスの作成
            ProjectDataSerializeFormat serializeData = new ProjectDataSerializeFormat();
            serializeData.ProjectName = ProjectData.ProjectName;
            serializeData.BaseFormData = new List<BaseFormDataSerializeFormat>();
            serializeData.MakerName = ProjectData.MakerName;


            //ベースフォームシリアライズ
            foreach (BaseForm frm in ProjectData.BaseFormData)
            {
                serializeData.BaseFormData.Add(frm.Serialize());
            }

            ProjectDataSerializeFormat.SaveProjectMaker(filePath, serializeData, addFlag);

            this.ProjectData.FilePath = filePath;

            updated = false;
        }

        /// <summary>
        /// 「ファイルを保存する」フォームを表示し、保存先ファイルの選択を要求します。
        /// </summary>
        /// <param name="filePath">選択されたファイルのパス</param>
        /// <param name="defaultFile">デフォルトで選択されるファイル</param>
        /// <param name="exts">有効な拡張子</param>
        /// <returns></returns>
        public bool GetSaveFileName(ref string filePath, string defaultFile, params string[] exts)
        {
            Debug.Assert(exts.Length > 0, "有効な拡張子を一つ以上指定して下さい。");

            //SaveFileDialogクラスのインスタンスを作成
            SaveFileDialog sfd = new SaveFileDialog();

            //はじめのファイル名を指定する
            sfd.FileName = defaultFile + "." + exts[0];

            //はじめに表示されるフォルダを指定する
            if (this.ProjectData.FileCreated)
            {
                sfd.InitialDirectory = Path.GetDirectoryName(this.ProjectData.FilePath);
            }
            else
            {
                sfd.InitialDirectory = Directory.GetCurrentDirectory();
            }

            string[] filterFiles = new string[exts.Length];
            for (int i = 0; i < exts.Length; i++)
            {
                filterFiles[i] = string.Format(
                    "{0}ファイル(*.{1})|*.{1}", exts[i].ToUpper(), exts[i].ToLower());
            }

            //[ファイルの種類]に表示される選択肢を指定する
            sfd.Filter = string.Join("|", filterFiles);
            //[ファイルの種類]ではじめに
            //「すべてのファイル」が選択されているようにする
            //sfd.FilterIndex = 2;
            //タイトルを設定する
            sfd.Title = "保存先のファイルを選択してください";
            //ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
            sfd.RestoreDirectory = true;

            //既に存在するファイル名を指定したとき警告する
            //デフォルトでTrueなので指定する必要はない
            sfd.OverwritePrompt = true;

            //存在しないパスが指定されたとき警告を表示する
            //デフォルトでTrueなので指定する必要はない
            sfd.CheckPathExists = true;

            //ダイアログを表示する
            if (sfd.ShowDialog() != DialogResult.OK) { return false; }

            //アクティブな子フォームを取得
            //Form ActiveForm = this.ActiveMdiChild;

            //保存先のファイル名
            filePath = sfd.FileName;
            return true;

        }

        /// <summary>
        /// ファイルよりプロジェクトをロードします。
        /// </summary>
        /// <param name="filePath">読み込まれるファイルパス</param>
        public void LoadFromFile(string filePath)
        {
            // プロジェクトデータをロードする。
            ProjectDataSerializeFormat projectData = ProjectDataSerializeFormat.Load(filePath);
            if (projectData == null)
            {
                MessageBox.Show("プロジェクトデータではありません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // ベースフォームデータを復元する
            List<BaseForm> frmList = new List<BaseForm>();
            foreach (BaseFormDataSerializeFormat frmData in projectData.BaseFormData)
            {
                try
                {
                    // ベースフォームデシリアライズ
                    BaseForm frm = (BaseForm)SerializeSupport.Deserialize(frmData, typeof(BaseForm));
                    Global.LogManager.Write(
                        string.Format("[PROC]Load BaseForm : {0}", frmData.TitleName));

                    // ベースフォームナンバーが重複している場合の調整
                    frm.Number = GetOpeningFormNumber(frmList, frm.Number);

                    // ベースフォームに対する初期処理
                    frm.Deserialize(frmData);               // シンボル･図形の生成
                    frm.MdiParent = MainForm.Instance;
                    frm.Hide();
                    Global.LogManager.Write(
                        string.Format("[PROC]Complete Load BaseForm : {0}", frmData.TitleName));

                    frmList.Add(frm);
                }
                catch (Exception e)
                {
                    // 「MissingMethodException - get_LogManagerが存在しない」が発生した時は、
                    // Symbol.Data内のLogManager.dllとCommonClassLibrary.dllを削除すると解決する

                    Global.LogManager.Write(
                        string.Format("[ERROR]BaseForm読み込みエラー : ファイル={0}, ベースフォーム={1}, 内容={2}",
                            filePath, frmData.TitleName, e.Message));
                }
            }

            // ロードデータを格納して保持する
            ProjectDataFormat loadData = new ProjectDataFormat();
            loadData.FilePath = filePath;
            loadData.ProjectName = projectData.ProjectName;
            loadData.MakerName = projectData.MakerName;
            loadData.BaseFormData = new List<BaseForm>(frmList);
            loadData.IOMonitorData = new List<IOMonitorSymbolFormat>(projectData.IOMonitorData);

            this.ProjectData = loadData;

            // SystemSettingForm作成
            SystemSettingForm = new SystemSettingForm();
            SystemSettingForm.MdiParent = this;
            SystemSettingForm.Hide();

            ProjectForm.Redraw();
        }

        /// <summary>
        /// 「ファイルを開く」フォームを表示し、読み込み先ファイルの選択を要求します。
        /// </summary>
        /// <param name="filePath">選択されたファイルのパス</param>
        /// <param name="exts">有効なファイルの拡張子</param>
        /// <returns></returns>
        private bool GetOpenFileName(ref string filePath, params string[] exts)
        {
            Debug.Assert(exts.Length > 0, "有効な拡張子を一つ以上指定して下さい。");

            //OpenFileDialogクラスのインスタンスを作成
            OpenFileDialog ofd = new OpenFileDialog();

            //はじめに表示されるフォルダを指定する
            if (this.ProjectData != null && this.ProjectData.FileCreated)
            {
                ofd.InitialDirectory = Path.GetDirectoryName(this.ProjectData.FilePath);
                ofd.FileName = Path.GetFileName(this.ProjectData.FilePath);
            }
            else
            {
                ofd.InitialDirectory = Directory.GetCurrentDirectory();
                ofd.FileName = Directory.GetFiles(ofd.InitialDirectory, "*" + exts[0])
                                            .FirstOrDefault() ?? "default" + exts[0];
            }

            string[] filterFiles = new string[exts.Length];
            for (int i = 0; i < exts.Length; i++)
            {
                filterFiles[i] = string.Format(
                    "{0}ファイル(*.{1})|*.{1}", exts[i].ToUpper(), exts[i].ToLower());
            }

            //[ファイルの種類]に表示される選択肢を指定する
            ofd.Filter = string.Join("|", filterFiles);

            //タイトルを設定する
            ofd.Title = "開くファイルを選択してください";

            //ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
            ofd.RestoreDirectory = true;

            //存在しないファイルの名前が指定されたとき警告を表示する            
            ofd.CheckFileExists = true;

            //存在しないパスが指定されたとき警告を表示する            
            ofd.CheckPathExists = true;

            //ダイアログを表示する
            if (ofd.ShowDialog() != DialogResult.OK) { return false; }

            filePath = ofd.FileName;

            return true;
        }

        /// <summary>
        /// プロジェクトに新しいベースフォームを追加します。
        /// </summary>
        private void FormNewCreate()
        {
            //空きナンバーを検索する
            int number = GetOpeningFormNumber(this.ProjectData.BaseFormData, 0);

            BaseForm BaseForm = new BaseForm();
            BaseForm.MdiParent = this;
            BaseForm.Number = number;
            BaseForm.BaseFormSize = BaseForm.Size;

            ProjectData.BaseFormData.Add(BaseForm);

            BaseForm.Show();

            ProjectForm.Redraw();

            BaseForm.Focus();
        }

        /// <summary>
        /// 空きBaseFormナンバーを取得します。
        /// </summary>
        /// <param name="source">空きナンバーを検索するBaseFormリスト</param>
        /// <param name="startNum">基準ナンバー</param>
        /// <returns>空きBaseFormナンバー</returns>
        public int GetOpeningFormNumber(IEnumerable<BaseForm> source, int startNum)
        //public int GetOpeningFormNumber(IEnumerable<IBaseForm> source, int startNum)
        {
            int num = startNum;

            while (source.SingleOrDefault((baseForm) => baseForm.Number == num) != null)
            {
                num++;
            }

            return num;
        }


        #region OldClass(使用禁止)

        [Obsolete]
        public class BaseFormDataFormat
        {
            private FormDataFormat _FormData = new FormDataFormat();

            private List<SymbolDataFormat> _SymbolData = new List<SymbolDataFormat>();

            private List<ShapeDataFormat> _ShapeData = new List<ShapeDataFormat>();

            public FormDataFormat FormData
            {
                get { return _FormData; }
                set { _FormData = value; }
            }

            public List<SymbolDataFormat> SymbolData
            {
                get { return _SymbolData; }
                set { _SymbolData = value; }

            }

            public List<ShapeDataFormat> ShapeData
            {
                get { return _ShapeData; }
                set { _ShapeData = value; }

            }
        }
        [Obsolete]
        public class FormDataFormat
        {
            public object TitleName { get; set; }
            public object Number { get; set; }
            public object Size { get; set; }
            public object Text { get; set; }
            public object BackColor { get; set; }
            public object Font { get; set; }
            public object BackgroundImage { get; set; }
            public object BackgroundImageLayout { get; set; }

        }
        [Obsolete]
        public class SymbolDataFormat
        {
            public Object ActionInstance { get; set; }
            public string Type { get; set; }
            public string SymbolName { get; set; }
            private List<PropertyDataFormat> _PropertyData = new List<PropertyDataFormat>();

            public List<PropertyDataFormat> PropertyData
            {
                get { return _PropertyData; }
                set { _PropertyData = value; }
            }

        }
        [Obsolete]
        public class PropertyDataFormat
        {
            public string Name { get; set; }
            public object Value { get; set; }
        }
        [Obsolete]
        public class ShapeDataFormat
        {
            public string Type { get; set; }
            private List<PropertyDataFormat> _PropertyData = new List<PropertyDataFormat>();

            public List<PropertyDataFormat> PropertyData
            {
                get { return _PropertyData; }
                set
                {
                    _PropertyData = value;
                }
            }

        }
        #endregion
    }

    // プロジェクトデータを表す
    public class ProjectDataFormat
    {
        public ProjectDataFormat()
        {
            this.FilePath = string.Empty;
            this.ProjectName = string.Empty;
            this.BaseFormData = new List<BaseForm>();
            this.MakerName = string.Empty;
            this.IOMonitorData = new List<IOMonitorSymbolFormat>();
        }

        public string FilePath
        {
            get;
            set;
        }

        public string ProjectName
        {
            get;
            set;
        }

        public List<BaseForm> BaseFormData
        {
            get;
            set;
        }

        public string MakerName
        {
            get;
            set;
        }

        public List<IOMonitorSymbolFormat> IOMonitorData
        {
            get;
            set;
        }

        public bool FileCreated
        {
            get
            {
                return !string.IsNullOrEmpty(this.FilePath);
            }
        }
    }

    [Serializable]
    public class OriginalCollection<T> : Collection<T>
    {
        public event EventHandler ObjectAdded;
        public event EventHandler ObjectRemoved;
        public event EventHandler ObjectRenew;

        public OriginalCollection()
            : base()
        {
            if (this.ObjectRenew != null) { this.ObjectRenew(this, null); }
        }

        public OriginalCollection(IList<T> list)
            : base(list)
        {
            if (this.ObjectRenew != null) { this.ObjectRenew(this, null); }
        }

        public new void Add(T item)
        {
            if (item.GetType().BaseType == typeof(ShapeObject))
            {
                if (item.GetType() == typeof(ViewObject))
                {
                    base.Add(item);
                    if (this.ObjectAdded != null) { this.ObjectAdded(item, null); }
                    return;
                }

                int Index = this.Count;
                for (int i = this.Count - 1; i >= 0; i--)
                {
                    T InObj = this.Items[i];
                    if (InObj.GetType() == typeof(ViewObject))
                    {
                        Index = i;
                    }
                }
                this.Insert(Index, item);

            }
            else
            {
                base.Add(item);
                if (this.ObjectAdded != null) { this.ObjectAdded(item, null); }
            }

        }

        public void AddRange(IEnumerable<T> collection)
        {
            foreach (T obj in collection)
            {
                this.Add(obj);
            }
        }

        public new void Clear()
        {
            base.Clear();
            if (this.ObjectRenew != null) { this.ObjectRenew(this, null); }
        }

        public new bool Remove(T item)
        {
            bool success = base.Remove(item);
            if (this.ObjectRemoved != null) { this.ObjectRemoved(item, null); }
            return success;
        }

        public int RemoveAll(Predicate<T> match)
        {
            List<T> list = new List<T>(this.Items);
            List<T> removeList = list.FindAll(match);
            int num = removeList.Count;
            foreach (T item in removeList)
            {
                this.Remove(item);

            }
            return num;
        }

        public new void Insert(int index, T item)
        {
            base.Insert(index, item);
            if (this.ObjectAdded != null) { this.ObjectAdded(item, null); }
        }

        public List<T> FindAll(Predicate<T> match)
        {
            List<T> list = new List<T>();
            for (int i = 0; i < this.Count; i++)
            {
                if (match(this.Items[i]))
                {
                    list.Add(this.Items[i]);
                }
            }
            return list;

        }
        //
        // 概要:
        //     指定した System.Comparison<T> を使用して、System.Collections.Generic.List<T> 全体内の要素を並べ替えます。
        //
        // パラメータ:
        //   comparison:
        //     要素を比較する場合に使用する System.Comparison<T>。
        //
        // 例外:
        //   System.ArgumentNullException:
        //     comparison が null です。
        //
        //   System.ArgumentException:
        //     comparison の実装によって並べ替え中にエラーが発生しました。たとえば、comparison は項目をその項目自身と比較する場合に 0 を返さない可能性があります。
        //
        public void Sort(Comparison<T> comparison)
        {
            if (comparison == null) { throw new ArgumentNullException("comparison"); }

            if (this.Count > 0)
            {
                T[] lst = this.Items.ToArray();
                Array.Sort<T>(lst, comparison);
                base.Clear();

                foreach (T item in lst)
                {
                    if (item.GetType().BaseType == typeof(ShapeObject))
                    {
                        if (item.GetType() == typeof(ViewObject))
                        {
                            base.Add(item);
                            return;
                        }

                        int Index = this.Count;
                        for (int i = this.Count - 1; i >= 0; i--)
                        {
                            T InObj = this.Items[i];
                            if (InObj.GetType() == typeof(ViewObject))
                            {
                                Index = i;
                            }
                        }
                        this.Insert(Index, item);

                    }
                    else
                    {
                        base.Add(item);
                    }
                }
            }
        }
    }
}
